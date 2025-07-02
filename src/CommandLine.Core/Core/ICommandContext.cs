
using System;
using System.Collections.Generic;

public interface ICommandContext
{
    Dictionary<string, string> Arguments { get; set; }
    string CommandName { get; set; }
    TimeSpan? Elapsed { get; }
    DateTime? EndTimestamp { get; set; }
    bool IsDryRun { get; set; }
    bool IsValid { get; }
    Dictionary<string, object> Metadata { get; set; }
    string Noun { get; set; }
    string[] OriginalArgs { get; set; }
    CommandContext? ParentContext { get; set; }
    DateTime StartTimestamp { get; set; }
    string? ValidationMessage { get; set; }
    string Verb { get; set; }

    T Resolve<T>() where T : notnull;
}