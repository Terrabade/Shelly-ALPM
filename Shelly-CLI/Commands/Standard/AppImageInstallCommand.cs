using System.Diagnostics;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Standard;

public class AppImageInstallCommand : AsyncCommand<AppImageInstallSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, AppImageInstallSettings settings)
    {
        if (settings.PackageLocation == null)
        {
            if (Program.IsUiMode)
            {
                await Console.Error.WriteLineAsync("Error: No package specified");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: No package specified[/]");
            }

            return 1;
        }

        if (!File.Exists(settings.PackageLocation))
        {
            if (Program.IsUiMode)
            {
                await Console.Error.WriteLineAsync("Error: Specified file does not exist.");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Specified file does not exist.[/]");
            }

            return 1;
        }

        RootElevator.EnsureRootExectuion();
        if (await IsAppImage(settings.PackageLocation))
        {
            return await InstallAppImage(settings);
        }

        return 0;
    }

    private static async Task<int> InstallAppImage(AppImageInstallSettings settings)
    {
        var filePath = Path.GetFullPath(settings.PackageLocation!);
        var appName = Path.GetFileNameWithoutExtension(filePath);
        var workingDir = Path.Combine(Path.GetTempPath(), "Shelly", appName);
        
        if (Directory.Exists(workingDir)) Directory.Delete(workingDir, true);
        Directory.CreateDirectory(workingDir);

        const string installDir = "/opt/shelly";
        Directory.CreateDirectory(installDir);

        AnsiConsole.MarkupLine($"[blue]Extracting AppImage...[/]");
        SetFilePermissions(filePath, "a+x");

        var extractProcess = Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            Arguments = "--appimage-extract",
            WorkingDirectory = workingDir,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        await extractProcess!.WaitForExitAsync();

        var squashfsRoot = Path.Combine(workingDir, "squashfs-root");
        if (!Directory.Exists(squashfsRoot))
        {
            AnsiConsole.MarkupLine("[red]Error: Failed to extract AppImage.[/]");
            return 1;
        }

        var desktopFile = Directory.GetFiles(squashfsRoot, "*.desktop", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (desktopFile == null)
        {
            AnsiConsole.MarkupLine("[yellow]Warning: No .desktop file found in AppImage.[/]");
        }

        string? iconName = null;
        if (desktopFile != null)
        {
            var lines = await File.ReadAllLinesAsync(desktopFile);
            var iconLine = lines.FirstOrDefault(l => l.StartsWith("Icon="));
            if (iconLine != null)
            {
                iconName = iconLine.Split('=', 2)[1].Trim();
            }
        }

        string? iconPath = null;
        if (!string.IsNullOrEmpty(iconName))
        {
            iconPath = Directory.GetFiles(squashfsRoot, $"{iconName}.*", SearchOption.AllDirectories).FirstOrDefault();
        }
        
        if (iconPath == null)
        {
            iconPath = Path.Combine(squashfsRoot, ".DirIcon");
            if (!File.Exists(iconPath)) iconPath = null;
        }

        var finalIconPath = "application-x-executable";
        if (iconPath != null)
        {
            const string iconDir = "/usr/share/icons/hicolor/scalable/apps";
            Directory.CreateDirectory(iconDir);
            
            var extension = Path.GetExtension(iconPath);
            if (string.IsNullOrEmpty(extension) || extension == ".DirIcon") extension = ".svg";

            var destIconName = $"{CleanInvalidNames(appName).ToLower()}{extension}";
            var destIconPath = Path.Combine(iconDir, destIconName);
            
            try 
            {
                File.Copy(iconPath, destIconPath, true);
                finalIconPath = destIconName;
                AnsiConsole.MarkupLine($"[green]Exported icon to: {destIconPath}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[yellow]Warning: Could not copy icon: {ex.Message}[/]");
            }
        }

        var destAppImagePath = Path.Combine(installDir, $"{appName}.AppImage");
        File.Copy(filePath, destAppImagePath, true);
        SetFilePermissions(destAppImagePath, "a+x");
        AnsiConsole.MarkupLine($"[green]Installed AppImage to: {destAppImagePath}[/]");

        if (desktopFile != null)
        {
            try
            {
                var desktopContent = await File.ReadAllLinesAsync(desktopFile);
                var patchedContent = new StringBuilder();
                foreach (var line in desktopContent)
                {
                    if (line.StartsWith("Exec="))
                    {
                        patchedContent.AppendLine($"Exec={destAppImagePath}");
                    }
                    else if (line.StartsWith("Icon="))
                    {
                        patchedContent.AppendLine($"Icon={finalIconPath}");
                    }
                    else
                    {
                        patchedContent.AppendLine(line);
                    }
                }

                const string desktopDir = "/usr/share/applications";
                Directory.CreateDirectory(desktopDir);
                var cleanName = CleanInvalidNames(appName);
                var desktopFilePath = Path.Combine(desktopDir, $"{cleanName}.desktop");
                
                await File.WriteAllTextAsync(desktopFilePath, patchedContent.ToString());
                SetFilePermissions(desktopFilePath, "644");
                UpdateDesktopDatabase(desktopDir);
                AnsiConsole.MarkupLine($"[green]Installed original desktop entry: {desktopFilePath}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[yellow]Warning: Could not install original desktop entry: {ex.Message}[/]");
                CreateDesktopEntry(appName, destAppImagePath, icon: finalIconPath);
            }
        }
        else
        {
            CreateDesktopEntry(appName, destAppImagePath, icon: finalIconPath);
        }
        
        try { Directory.Delete(workingDir, true); } catch { /* ignore */ }

        return 0;
    }

    private static Task<bool> IsAppImage(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return Task.FromResult(string.Equals(extension, ".AppImage", StringComparison.OrdinalIgnoreCase));
    }

    private static void SetFilePermissions(string filePath, string permissions)
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"{permissions} \"{filePath}\"",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not set file permissions: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    private static void CreateDesktopEntry(
        string appName,
        string executablePath,
        string? comment = null,
        string icon = "application-x-executable",
        bool terminal = false,
        string categories = "Utility;")
    {
        const string desktopDir = "/usr/share/applications";
        var cleanName = CleanInvalidNames(appName);
        var desktopFilePath = Path.Combine(desktopDir, $"{cleanName}.desktop");

        var content = new StringBuilder();
        content.AppendLine("[Desktop Entry]");
        content.AppendLine("Version=1.0");
        content.AppendLine("Type=Application");
        content.AppendLine($"Name={appName}");
        content.AppendLine($"Comment={comment ?? $"{appName} application"}");
        content.AppendLine($"Exec={executablePath}");
        content.AppendLine($"Icon={icon}");
        content.AppendLine($"Terminal={terminal.ToString().ToLower()}");
        content.AppendLine($"Categories={categories}");
        content.AppendLine("StartupNotify=true");

        try
        {
            Directory.CreateDirectory(desktopDir);
            File.WriteAllText(desktopFilePath, content.ToString());
            SetFilePermissions(desktopFilePath, "644");
            UpdateDesktopDatabase(desktopDir);

            AnsiConsole.MarkupLine($"[green]Desktop entry created: {desktopFilePath.EscapeMarkup()}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not create desktop entry: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    private static string CleanInvalidNames(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("/", "-")
            .Replace("\\", "-");
    }

    private static void UpdateDesktopDatabase(string desktopDir)
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "update-desktop-database",
                Arguments = $"\"{desktopDir}\"",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not set desktop database: {ex.Message.EscapeMarkup()}[/]");
        }
    }
}