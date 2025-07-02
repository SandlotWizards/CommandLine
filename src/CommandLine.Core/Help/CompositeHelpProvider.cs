using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Help;

public class CompositeHelpProvider : IHelpProvider
{
    private readonly List<IHelpProvider> _providers;

    public CompositeHelpProvider(params IHelpProvider[] providers)
    {
        _providers = providers.ToList();
    }

    public async Task<string> GetHelpAsync(string commandName)
    {
        foreach (var provider in _providers)
        {
            var help = await provider.GetHelpAsync(commandName);
            if (!string.IsNullOrWhiteSpace(help))
                return help;
        }

        return $"Help for command: {commandName} not found.";
    }
}
