using SandlotWizards.CommandLineParser.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Parsing;

public class ContextParser : IContextParser
{
    public CommandContext Parse(string[] args)
    {
        var context = new CommandContext();

        if (args.Length == 0)
            return context;

        // determine multilevel command name
        var parts = new List<string>();
        if (!args[0].StartsWith("--"))
            parts.Add(args[0]);

        if (args.Length > 1 && !args[1].StartsWith("--"))
            parts.Add(args[1]);

        context.Noun = parts.Count > 0 ? parts[0] : string.Empty;
        context.Verb = parts.Count > 1 ? parts[1] : string.Empty;
        context.CommandName = string.Join(" ", parts); // still preserved for backward compatibility

        // parse cli arguments
        for (int i = parts.Count; i < args.Length; i++)
        {
            if (args[i].StartsWith("--"))
            {
                var key = args[i].TrimStart('-');
                var value = i + 1 < args.Length && !args[i + 1].StartsWith("--") ? args[++i] : "";
                context.Arguments[key] = value;

                if (string.Equals(key, "dry-run", StringComparison.OrdinalIgnoreCase))
                {
                    context.Metadata["DryRun"] = true;
                }
            }
        }

        // inject environment variable overrides
        foreach (var key in Environment.GetEnvironmentVariables().Keys.Cast<string>())
        {
            if (key.StartsWith("SANDLOT_"))
            {
                var argKey = key["SANDLOT_".Length..].ToLowerInvariant();
                if (!context.Arguments.ContainsKey(argKey))
                {
                    var value = Environment.GetEnvironmentVariable(key);
                    if (value is not null)
                        context.Arguments[argKey] = value;
                }
            }
        }

        return context;
    }
}
