using System;
using System.Threading.Tasks;
using SandlotWizards.CommandLineParser.Core;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class GreetCommand : ICommand
{
    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        Console.WriteLine("Hello! Welcome to the Sandlot Wizards CLI.");
        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "Greeting displayed." }
        });
    }
}
