using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Execution;

public class CommandContext : ICommandContext
{
    public string Noun { get; set; } = string.Empty;
    public string Verb { get; set; } = string.Empty;

    public string CommandName { get; set; } = string.Empty;
    public Dictionary<string, string> Arguments { get; set; } = new();
    
    public string[] OriginalArgs { get; set; } = Array.Empty<string>();
    public string? ValidationMessage { get; set; }
    public bool IsValid => string.IsNullOrEmpty(ValidationMessage);
    public bool IsDryRun => Metadata.TryGetValue("DryRun", out var value) && value is true;

    public CommandContext? ParentContext { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public DateTime StartTimestamp { get; set; } = DateTime.UtcNow;
    public DateTime? EndTimestamp { get; set; }
    public TimeSpan? Elapsed => EndTimestamp.HasValue ? EndTimestamp - StartTimestamp : null;

    public string[] PositionalArgs { get; set; } = Array.Empty<string>();

    public string[] ForwardableArgs => Arguments
        .Where(kvp => !Reserved.Contains(kvp.Key.ToLowerInvariant()))
        .SelectMany(kvp => new[] { $"--{kvp.Key}", kvp.Value ?? string.Empty })
        .ToArray();

    public static readonly HashSet<string> Reserved = new(StringComparer.OrdinalIgnoreCase)
    {
        "help", "version", "output"
    };

    public T Resolve<T>() where T : notnull
    {
        if (Metadata.TryGetValue("ServiceProvider", out var spObj) &&
            spObj is IServiceProvider sp)
        {
            return sp.GetRequiredService<T>();
        }

        throw new InvalidOperationException("ServiceProvider is not available in CommandContext.");
    }
}
