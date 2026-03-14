using System.Diagnostics.CodeAnalysis;
using PackageManager.Flatpak;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Flatpak;

public class GetAppInstallSize : Command<FlatpakInstallSize>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] FlatpakInstallSize settings)
    {
        var manager = new FlatpakManager();
        var result = manager.GetRemoteSize(settings.Remote,settings.Name,"",settings.Branch);
      
        Console.Write(FormatSize(result));
    
        return 0;
    }

    private static string FormatSize(ulong bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        var i = 0;
        double dblSByte = bytes;
        while (i < suffixes.Length && bytes >= 1024)
        {
            dblSByte = bytes / 1024.0;
            i++;
            bytes /= 1024;
        }

        return $"{dblSByte:0.##} {suffixes[i]}";
    }
}

public class FlatpakInstallSize : CommandSettings
{
    [CommandArgument(0, "<remote>")]
    public string Remote { get; set; } = "";
    
    [CommandArgument(1, "<id>")]
    public string Name { get; set; } = "";
    
    [CommandArgument(2, "<branch>")]
    public string Branch { get; set; } = "";
}
