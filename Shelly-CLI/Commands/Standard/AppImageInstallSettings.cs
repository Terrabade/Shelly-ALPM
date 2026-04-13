using System.ComponentModel;
using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Standard;

public class AppImageInstallSettings : CommandSettings 
{
    [CommandOption("-l | --location")]
    [Description("Location of the .AppImage to be installed")]
    public string? PackageLocation { get; set; }

    [CommandOption("-n|--no-confirm")]
    [Description("Proceed without asking for user confirmation")]
    public bool NoConfirm { get; set; }
}

public class AppImageRemoveSettings : CommandSettings
{
    [CommandOption("-n | --name")]
    [Description("Name of the AppImage to be removed")]
    public string? Name { get; set; }

    [CommandOption("-c|--no-confirm")]
    [Description("Proceed without asking for user confirmation")]
    public bool NoConfirm { get; set; }
}