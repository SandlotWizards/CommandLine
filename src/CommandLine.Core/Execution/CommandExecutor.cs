using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Execution;

public static class CommandExecutor
{
    public static async Task ExecuteAsync(ICommand command, CommandContext context)
    {
        using (ActionLog.Global.BeginStep($"Executing '{context.CommandName}'"))
        {
            await HookPipeline.RunWithHooksAsync(command, context);
        }
    }
}
