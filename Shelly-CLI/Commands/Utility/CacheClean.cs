using Spectre.Console.Cli;

namespace Shelly_CLI.Commands.Utility;

public class CacheClean : AsyncCommand<CacheCleanSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, CacheCleanSettings settings)
    {
        if (Program.IsUiMode)
        {
            return Task.FromResult(0);
        }
        
        throw new NotImplementedException();
    }
}