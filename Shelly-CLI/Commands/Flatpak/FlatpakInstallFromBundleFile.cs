using System.Diagnostics.CodeAnalysis;
using PackageManager.Flatpak;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Flatpak;

public class FlatpakInstallFromBundleFile : Command<FlatpakBundleInstallSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] FlatpakBundleInstallSettings settings)
    {
        AnsiConsole.MarkupLine("[yellow]Installing flatpak bundle...[/]");
        var result = FlatpakManager.InstallAppFromBundle(settings.BundlePath, settings.SystemWide);

        AnsiConsole.MarkupLine("[yellow]Installed: " + result.EscapeMarkup() + "[/]");

        return 0;
    }
}
