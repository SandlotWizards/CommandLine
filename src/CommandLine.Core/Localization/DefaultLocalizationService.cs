using System.Collections.Generic;
using System.Threading.Tasks;
namespace SandlotWizards.CommandLineParser.Localization;

public class DefaultLocalizationService : ILocalizationService
{
    private readonly Dictionary<string, string> _fallback = new()
    {
        ["UnknownCommand"] = "Unknown command: '{0}'",
        ["DidYouMean"] = "Did you mean '{0}'?",
        ["HelpPrompt"] = "Type `lore --help` to see available commands.",
        ["Command"] = "Command: {0}",
        ["Description"] = "Description:",
        ["Usage"] = "Usage:",
        ["Examples"] = "Examples:",
        ["Group"] = "Group:"
    };

    public string Get(string key) =>
        _fallback.TryGetValue(key, out var value) ? value : $"[[{key}]]";

    public string Get(string key, params object[] args) =>
        string.Format(Get(key), args);
}
