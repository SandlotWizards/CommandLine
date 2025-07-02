using System.Threading.Tasks;
namespace SandlotWizards.CommandLineParser.Help;

public interface IHelpProvider
{
    public Task<string> GetHelpAsync(string commandName);
}
