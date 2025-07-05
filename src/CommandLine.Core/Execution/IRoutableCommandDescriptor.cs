using SandlotWizards.CommandLineParser.Execution;
using System;

public interface IRoutableCommandDescriptor
{
    string Noun { get; }
    string Verb { get; }
    string? Description { get; }
    string? Group { get; }

    bool IsEnabled { get; }
    bool ShowInList { get; }

    ICommand Resolve(IServiceProvider services);
}
