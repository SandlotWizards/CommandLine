using SandlotWizards.CommandLineParser.Help;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class CustomHelpProvider : IHelpProvider
{
    private readonly Dictionary<string, string> _helpEntries;

    public CustomHelpProvider(string jsonPath)
    {
        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);
            _helpEntries = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
        else
        {
            _helpEntries = new();
        }
    }

    public Task<string> GetHelpAsync(string commandName)
    {
        return Task.FromResult(_helpEntries.TryGetValue(commandName, out var text) ? text : string.Empty);
    }
}
