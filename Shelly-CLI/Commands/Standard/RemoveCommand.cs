using PackageManager.Alpm;
using Shelly_CLI.ConsoleLayouts;
using Shelly_CLI.Utility;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Standard;

public class RemoveCommand : AsyncCommand<RemovePackageSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RemovePackageSettings settings)
    {
        if (Program.IsUiMode)
        {
            return HandleUiModeRemove(settings);
        }

        if (settings.Packages.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: No packages specified[/]");
            return 1;
        }

        RootElevator.EnsureRootExectuion();
        var packageList = settings.Packages.ToList();

        AnsiConsole.MarkupLine($"[yellow]Packages to remove:[/] {string.Join(", ", packageList.Select(p => p.EscapeMarkup()))}");

        if (!Program.IsUiMode)
        {
            if (!AnsiConsole.Confirm("Do you want to proceed?"))
            {
                AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
                return 0;
            }
        }

        using var manager = new AlpmManager();

        AnsiConsole.MarkupLine("[yellow]Initializing ALPM...[/]");
        manager.Initialize(true);

        AnsiConsole.MarkupLine("[yellow]Removing packages...[/]");


        var flags = AlpmTransFlag.None;
        if (settings.Cascade)
        {
            flags |= AlpmTransFlag.NoSave | AlpmTransFlag.Recurse;
        }
        else if (settings.Ripple)
        {
            flags |= AlpmTransFlag.Cascade;
        }

        await SplitOutput.Output(manager, x => x.RemovePackages(packageList, flags), settings.NoConfirm);

        if (settings.RemoveConfig)
        {
            HandleConfigRemoval(settings.Packages);
        }

        AnsiConsole.MarkupLine("[green]Packages removed successfully![/]");
        return 0;
    }

    private static int HandleConfigRemoval(string[] packageNames)
    {
        foreach (var package in packageNames)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), package);
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to find directory for {package} moving on");
            }
        }

        return 0;
    }

    private static int HandleUiModeRemove(RemovePackageSettings settings)
    {
        if (settings.Packages.Length == 0)
        {
            Console.Error.WriteLine("Error: No packages specified");
            return 1;
        }

        using var manager = new AlpmManager();
        try
        {
            var packageList = settings.Packages.ToList();

            // Handle questions
            manager.Question += (sender, args) => { QuestionHandler.HandleQuestion(args, true, settings.NoConfirm); };

            // Handle progress events
            manager.Progress += (sender, args) => { Console.Error.WriteLine($"{args.PackageName}: {args.Percent}%"); };

            Console.Error.WriteLine("Initializing ALPM...");
            manager.Initialize(true);

            Console.Error.WriteLine($"Removing packages: {string.Join(", ", packageList)}");
            var flags = AlpmTransFlag.None;
            if (settings.Cascade)
            {
                flags |= AlpmTransFlag.NoSave | AlpmTransFlag.Recurse;
            }
            else if (settings.Ripple)
            {
                flags |= AlpmTransFlag.Cascade;
            }

            manager.HookRun += (sender, args) => { Console.Error.WriteLine($"[ALPM_HOOK]{args.Description}"); };
            manager.RemovePackages(packageList, flags);
            if (settings.RemoveConfig)
            {
                HandleConfigRemoval(settings.Packages);
            }

            Console.Error.WriteLine("Packages removed successfully!");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Removal failed: {ex.Message}");
            return 1;
        }
    }
}