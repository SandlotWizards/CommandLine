using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Core;

public interface ICommand
{
    Task ExecuteAsync(CommandContext context);
}
