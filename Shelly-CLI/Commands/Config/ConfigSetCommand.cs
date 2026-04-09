using System.ComponentModel;
using Shelly_CLI.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Config;

public class ConfigSetSettings : CommandSettings
{
    [CommandArgument(0, "<KEY>")]
    [Description("The configuration key to set")]
    public string Key { get; set; } = string.Empty;

    [CommandArgument(1, "<VALUE>")]
    [Description("The value to set")]
    public string Value { get; set; } = string.Empty;
}

public class ConfigSetCommand : Command<ConfigSetSettings>
{
    public override int Execute(CommandContext context, ConfigSetSettings settings)
    {
        var success = ConfigManager.UpdateConfig(settings.Key, settings.Value);
        if (!success)
        {
            AnsiConsole.MarkupLine($"[red]Failed to set configuration key: {settings.Key.EscapeMarkup()}[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]{settings.Key.EscapeMarkup()}[/] set to [blue]{settings.Value.EscapeMarkup()}[/]");
        return 0;
    }
}
