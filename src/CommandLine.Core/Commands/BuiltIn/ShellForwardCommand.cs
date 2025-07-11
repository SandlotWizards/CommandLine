﻿using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using System;
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
        var allArgs = new[] { _noun, _verb }
            .Concat(context.PositionalArgs)
            .Concat(context.ForwardableArgs)
            .ToList();

        var outputAsJson = context.Arguments.TryGetValue("output", out var format) &&
                           format?.ToLowerInvariant() == "json";

        if (outputAsJson)
        {
            allArgs.AddRange(new[] { "--output", "json" });
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _exe,
                Arguments = string.Join(' ', allArgs),
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        process.Start();

        _ = Task.Run(async () =>
        {
            using var reader = Console.In;
            using var writer = process.StandardInput;
            while (!process.HasExited && reader is not null)
            {
                var line = await reader.ReadLineAsync();
                if (line is null) break;
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
            }
        });

        _ = Task.Run(async () =>
        {
            using var stderr = process.StandardError;
            string? errLine;
            while ((errLine = await stderr.ReadLineAsync()) != null)
            {
                ActionLog.Global.Warning("[stderr] " + errLine);
            }
        });

        string raw = string.Empty;
        using var stdout = process.StandardOutput;
        int ch;
        while ((ch = stdout.Read()) != -1)
        {
            char character = (char)ch;
            raw += character;
            Console.Write(character);
        }

        await process.WaitForExitAsync();

        if (outputAsJson)
        {
            try
            {
                return JsonSerializer.Deserialize<CommandResult>(raw, new JsonSerializerOptions
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
                    Data = new { Raw = raw }
                };
            }
        }

        return new CommandResult
        {
            Status = "success",
            Messages = new[] { "Command executed successfully." },
            Data = new { Raw = raw }
        };
    }
}
