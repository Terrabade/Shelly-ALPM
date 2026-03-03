using System.Text.Json;
using Shelly_CLI.Commands.Aur;
using Shelly_CLI.Commands.Flatpak;
using Shelly_CLI.Commands.Standard;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands;

public class DefaultCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        //TODO: UPDATE TO READ LOCAL USER NOT ROOT INSTEAD OF CURRENT SITUATION
        var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "shelly", "config.json");
        if (!File.Exists(configPath))
        {
            return 1;
        }

        var json = await File.ReadAllTextAsync(configPath);
        var config = JsonSerializer.Deserialize<ShellyConfig>(json, ShellyCLIJsonContext.Default.ShellyConfig);
        if (config == null)
        {
            return 1;
        }

        return config.DefaultExecution switch
        {
            Shelly_CLI.DefaultCommand.UpgradeStandard => new UpgradeCommand().Execute(context, new UpgradeSettings()),
            Shelly_CLI.DefaultCommand.UpgradeFlatpak => new FlatpakUpgrade().Execute(context),
            Shelly_CLI.DefaultCommand.UpgradeAur => await new AurUpgradeCommand().ExecuteAsync(context, new AurUpgradeSettings()),
            Shelly_CLI.DefaultCommand.UpgradeAll => new UpgradeCommand().Execute(context, new UpgradeSettings { All = true }),
            Shelly_CLI.DefaultCommand.Sync => new SyncCommand().Execute(context, new SyncSettings()),
            Shelly_CLI.DefaultCommand.SyncForce => new SyncCommand().Execute(context, new SyncSettings { Force = true }),
            Shelly_CLI.DefaultCommand.ListInstalled => new ListInstalledCommand().Execute(context, new ListSettings()),
            _ => 1
        };
    }
}