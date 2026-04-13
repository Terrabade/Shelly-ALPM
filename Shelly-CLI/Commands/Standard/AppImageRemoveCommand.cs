using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Standard;

public class AppImageRemoveCommand : AsyncCommand<AppImageRemoveSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, AppImageRemoveSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            if (Program.IsUiMode)
            {
                await Console.Error.WriteLineAsync("Error: No AppImage name specified");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: No AppImage name specified[/]");
            }

            return 1;
        }

        RootElevator.EnsureRootExectuion();

        const string installDir = "/opt/shelly";
        if (!Directory.Exists(installDir))
        {
            AnsiConsole.MarkupLine("[yellow]Info: /opt/shelly directory does not exist. No AppImages to remove.[/]");
            return 0;
        }

        var appImages = Directory.GetFiles(installDir, "*.AppImage", SearchOption.TopDirectoryOnly);
        var matches = appImages
            .Where(f => Path.GetFileName(f).Contains(settings.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No AppImage matching \"{settings.Name}\" found in {installDir}[/]");
            return 0;
        }

        string targetAppImage;
        if (matches.Count == 1)
        {
            targetAppImage = matches[0];
        }
        else
        {
            if (settings.NoConfirm)
            {
                targetAppImage = matches[0];
                AnsiConsole.MarkupLine(
                    $"[yellow]Multiple matches found, picking first one due to --no-confirm: {Path.GetFileName(targetAppImage)}[/]");
            }
            else
            {
                targetAppImage = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Multiple AppImages matched. Which one do you want to [red]remove[/]?")
                        .AddChoices(matches.Select(Path.GetFileName).Cast<string>())
                );
                targetAppImage = matches.First(m => Path.GetFileName(m) == targetAppImage);
            }
        }

        if (!settings.NoConfirm &&
            !AnsiConsole.Confirm($"Are you sure you want to remove [red]{Path.GetFileName(targetAppImage)}[/]?"))
        {
            return 0;
        }

        return await RemoveAppImage(targetAppImage);
    }

    private static async Task<int> RemoveAppImage(string appImagePath)
    {
        var appName = Path.GetFileNameWithoutExtension(appImagePath);
        var cleanName = CleanInvalidNames(appName);
        const string desktopDir = "/usr/share/applications";
        var desktopFilePath = Path.Combine(desktopDir, $"{cleanName}.desktop");

        try
        {
            if (File.Exists(appImagePath))
            {
                File.Delete(appImagePath);
                AnsiConsole.MarkupLine($"[green]Removed AppImage: {appImagePath}[/]");
            }

            if (File.Exists(desktopFilePath))
            {
                File.Delete(desktopFilePath);
                AnsiConsole.MarkupLine($"[green]Removed desktop entry: {desktopFilePath}[/]");
                UpdateDesktopDatabase(desktopDir);
            }
            else
            {
                var potentialDesktopFiles = Directory.GetFiles(desktopDir, "*.desktop")
                    .Where(f => Path.GetFileName(f).Contains(cleanName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var df in potentialDesktopFiles)
                {
                    var content = await File.ReadAllLinesAsync(df);
                    if (!content.Any(l => l.StartsWith("Exec=") && l.Contains(appImagePath))) continue;
                    File.Delete(df);
                    AnsiConsole.MarkupLine($"[green]Removed desktop entry: {df}[/]");
                    UpdateDesktopDatabase(desktopDir);
                    break;
                }
            }

            const string iconDir = "/usr/share/icons/hicolor/scalable/apps";
            if (Directory.Exists(iconDir))
            {
                var potentialIcons = Directory.GetFiles(iconDir, $"{cleanName}.*");
                foreach (var icon in potentialIcons)
                {
                    File.Delete(icon);
                    AnsiConsole.MarkupLine($"[green]Removed icon: {icon}[/]");
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error during removal: {ex.Message}[/]");
            return 1;
        }

        return 0;
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