using System;
using System.Threading.Tasks;
using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class VersionCommand : ICommand
{
    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        ActionLog.Global.Message("Lore CLI version 1.0.0");
        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "Version displayed." }
        });
    }
}
