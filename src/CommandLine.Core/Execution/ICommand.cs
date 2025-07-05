using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Execution;

public interface ICommand
{
    Task<CommandResult?> ExecuteAsync(CommandContext context);
}
