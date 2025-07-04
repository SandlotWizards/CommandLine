using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class GreetCommand : ICommand
{
    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        ActionLog.Global.Message("Hello! Welcome to the Sandlot Wizards CLI.");
        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "Greeting displayed." }
        });
    }
}
