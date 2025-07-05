using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.BuiltIn;

[ExcludeFromCommandDiscovery]
public class ShellForwardCommand : IRoutableCommand
{
    private readonly string _exe;
    private readonly string _noun;
    private readonly string _verb;

    public ShellForwardCommand(string exe, string noun, string verb)
    {
        _exe = exe;
        _noun = noun;
        _verb = verb;
    }

    public string Noun => _noun;
    public string Verb => _verb;
    public string? Description => "Forwards command to a local shell-executed .NET tool.";
    public string? Group => "Command Forwarders";
    public bool IsEnabled => true;
    public bool ShowInList => false;

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
