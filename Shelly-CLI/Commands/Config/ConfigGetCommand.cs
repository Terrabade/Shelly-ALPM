using System.ComponentModel;
using Shelly_CLI.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Config;

public class ConfigGetSettings : CommandSettings
{
    [CommandArgument(0, "<KEY>")]
    [Description("The configuration key to get")]
    public string Key { get; set; } = string.Empty;
}

public class ConfigGetCommand : Command<ConfigGetSettings>
{
    public override int Execute(CommandContext context, ConfigGetSettings settings)
    {
        var value = ConfigManager.GetConfigValue(settings.Key);
        if (value == null)
        {
            AnsiConsole.MarkupLine($"[red]Unknown configuration key: {settings.Key.EscapeMarkup()}[/]");
            return 1;
        }

        AnsiConsole.WriteLine(value);
        return 0;
    }
}
