using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;
using System.Threading.Tasks;

public class GoodbyeWorldCommand : ICommand
{
    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "world";
        var message = $"Goodbye, {name}!";
        ActionLog.Global.Message(message);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}