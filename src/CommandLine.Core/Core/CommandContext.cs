using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace SandlotWizards.CommandLineParser.Core;

public class CommandContext : ICommandContext
{
    public string Noun { get; set; } = string.Empty;
    public string Verb { get; set; } = string.Empty;

    public string CommandName { get; set; } = string.Empty;
    public Dictionary<string, string> Arguments { get; set; } = new();
    public bool IsDryRun { get; set; }

    public string[] OriginalArgs { get; set; } = Array.Empty<string>();
    public string? ValidationMessage { get; set; }
    public bool IsValid => string.IsNullOrEmpty(ValidationMessage);

    public CommandContext? ParentContext { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public DateTime StartTimestamp { get; set; } = DateTime.UtcNow;
    public DateTime? EndTimestamp { get; set; }
    public TimeSpan? Elapsed => EndTimestamp.HasValue ? EndTimestamp - StartTimestamp : null;

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
