using PackageManager.Alpm;
using Shelly_CLI.ConsoleLayouts;
using Shelly_CLI.Utility;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Standard;

public class InstallCommand : AsyncCommand<InstallPackageSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, InstallPackageSettings settings)
    {
        if (Program.IsUiMode)
        {
            return HandleUiModeInstall(context, settings);
        }

        if (settings.Packages.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: No packages specified[/]");
            return 1;
        }

        RootElevator.EnsureRootExectuion();

        var packageList = settings.Packages.ToList();

        AnsiConsole.MarkupLine($"[yellow]Packages to install:[/] {string.Join(", ", packageList.Select(p => p.EscapeMarkup()))}");

        if (!AnsiConsole.Confirm("Do you want to proceed?"))
        {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
            return 0;
        }


        using var manager = new AlpmManager();
        AnsiConsole.MarkupLine("[yellow]Initializing ALPM...[/]");
        manager.Initialize(true);
        if (settings.Upgrade)
        {
            AnsiConsole.Markup("[yellow]Running system upgrade[/yellow]");
            await SplitOutput.Output(manager, x => x.SyncSystemUpdate(), settings.NoConfirm);
        }

        if (settings.BuildDepsOn)
        {
            if (settings.Packages.Length > 1)
            {
                AnsiConsole.MarkupLine("[yellow]Cannot build dependencies for multiple packages at once.[/]");
                return 0;
            }

            if (settings.MakeDepsOn)
            {
                AnsiConsole.MarkupLine("[yellow]Installing packages...[/]");
                await SplitOutput.Output(manager, x => x.InstallDependenciesOnly(packageList.First(), true),
                    settings.NoConfirm);
                return 0;
            }

            AnsiConsole.MarkupLine("[yellow]Installing packages...[/]");
            await SplitOutput.Output(manager, x => x.InstallDependenciesOnly(packageList.First()),
                settings.NoConfirm);
            AnsiConsole.MarkupLine("[green]Packages installed successfully![/]");
            return 0;
        }

        if (settings.NoDeps)
        {
            AnsiConsole.MarkupLine("[yellow]Skipping dependency installation.[/]");
            AnsiConsole.MarkupLine("[yellow]Installing packages...[/]");
            await SplitOutput.Output(manager, x => x.InstallPackages(packageList, AlpmTransFlag.NoDeps),
                settings.NoConfirm);
            AnsiConsole.MarkupLine("[green]Packages installed successfully![/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[yellow]Installing packages...[/]");

        await SplitOutput.Output(manager, x => x.InstallPackages(packageList), settings.NoConfirm);
        Console.WriteLine(); // Final newline after last package

        AnsiConsole.MarkupLine("[green]Packages installed successfully![/]");
        return 0;
    }

    private static int HandleUiModeInstall(CommandContext context, InstallPackageSettings settings)
    {
        if (settings.Packages.Length == 0)
        {
            Console.Error.WriteLine("Error: No packages specified");
            return 1;
        }

        if (settings.Upgrade)
        {
            var command = new UpgradeCommand();
            command.ExecuteAsync(context, new UpgradeSettings()
            {
                JsonOutput = true,
            }).Wait();
        }

        using var manager = new AlpmManager();
        manager.Question += (_, args) => { QuestionHandler.HandleQuestion(args, true, settings.NoConfirm); };
        Console.Error.WriteLine("Initializing ALPM...");
        manager.Initialize(true);

        if (settings.BuildDepsOn)
        {
            if (settings.Packages.Length > 1)
            {
                Console.WriteLine("Cannot build dependencies for multiple packages at once.");
                return -1;
            }

            if (settings.MakeDepsOn)
            {
                Console.Error.WriteLine("Installing packages...");
                manager.InstallDependenciesOnly(settings.Packages.ToList().First(), true);
                return 0;
            }

            Console.Error.WriteLine("Installing packages...");
            manager.InstallDependenciesOnly(settings.Packages.ToList().First());
            Console.Error.WriteLine("Packages installed successfully!");
            return 0;
        }

        if (settings.NoDeps)
        {
            Console.Error.WriteLine("Skipping dependency installation.");
            Console.Error.WriteLine("Installing packages...");
            manager.InstallPackages(settings.Packages.ToList(), AlpmTransFlag.NoDeps);
            Console.Error.WriteLine("Packages installed successfully!");
            return 0;
        }

        Console.WriteLine("Installing packages...");
        manager.Progress += (_, args) => { Console.WriteLine($"{args.PackageName}: {args.Percent}%"); };
        manager.HookRun += (_, args) => { Console.Error.WriteLine($"[ALPM_HOOK]{args.Description}"); };
        try
        {
            manager.InstallPackages(settings.Packages.ToList());
            Console.Error.WriteLine("Finished installing packages.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ALPM_ERROR]Failed to install packages: {ex.Message}");
            return 1;
        }

        return 0;
    }
}