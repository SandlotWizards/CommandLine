using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Execution;

public static class HookPipeline
{
    public static Func<CommandContext, Task> BeforeHookAsync { get; set; } = ctx => Task.CompletedTask;
    public static Func<CommandContext, Task> AfterHookAsync { get; set; } = ctx => Task.CompletedTask;

    public static async Task RunWithHooksAsync(ICommand command, CommandContext context)
    {
        await BeforeHookAsync(context);

        if (context.IsDryRun)
        {
            ActionLog.Global.Info($"[DRY-RUN] Skipping execution of '{context.CommandName}'.");
        }
        else
        {
            await command.ExecuteAsync(context);
        }

        await AfterHookAsync(context);
    }
}
