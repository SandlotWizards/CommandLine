using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Core;

public interface ICommand
{
    Task<CommandResult?> ExecuteAsync(CommandContext context);
}
