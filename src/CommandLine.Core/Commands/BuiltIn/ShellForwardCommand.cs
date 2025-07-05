using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Commands.BuiltIn;

[ExcludeFromCommandDiscovery]
public class ShellForwardCommand : IRoutableCommand
{
    private readonly string _exe;
    private readonly string _noun;
    private readonly string _verb;
    private readonly string _description;
    private readonly string _group;
    private readonly bool _isEnabled;
    private readonly bool _showInList;

    public ShellForwardCommand(
        string exe,
        string noun,
        string verb,
        string description,
        string group,
        bool isEnabled,
        bool showInList)
    {
        _exe = exe;
        _noun = noun;
        _verb = verb;
        _description = description;
        _group = group;
        _isEnabled = isEnabled;
        _showInList = showInList;
    }

    public string Noun => _noun;
    public string Verb => _verb;
    public string? Description => _description;
    public string? Group => _group;
    public bool IsEnabled => _isEnabled;
    public bool ShowInList => _showInList;

    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var baseArgs = new[] { _noun, _verb };
        var allArgs = baseArgs
            .Concat(context.PositionalArgs)
            .Concat(context.ForwardableArgs)
            .ToArray();

        var outputAsJson = context.Arguments.TryGetValue("output", out var format)
            && format?.ToLowerInvariant() == "json";

        if (outputAsJson)
        {
            allArgs = allArgs.Concat(new[] { "--output", "json" }).ToArray();
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _exe,
                Arguments = string.Join(' ', allArgs),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (outputAsJson)
        {
            try
            {
                return JsonSerializer.Deserialize<CommandResult>(output, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return new CommandResult
                {
                    Status = "error",
                    Messages = new[] { "Failed to parse JSON output from plugin." },
                    Data = new { Raw = output }
                };
            }
        }

        ActionLog.Global.Message(output);
        return new CommandResult
        {
            Status = "success",
            Messages = new[] { "Command executed successfully." },
            Data = new { Raw = output }
        };
    }
}
