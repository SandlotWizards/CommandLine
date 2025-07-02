using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Help;

public class DefaultHelpProvider : IHelpProvider
{
    public Task<string> GetHelpAsync(string commandName)
    {
        return Task.FromResult($"Help for command: {commandName} not found.");
    }
}
