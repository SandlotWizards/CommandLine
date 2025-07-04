using System;

namespace SandlotWizards.CommandLineParser.Core;

public class RoutableCommandDescriptor : IRoutableCommandDescriptor
{
    private readonly Func<IServiceProvider, ICommand> _factory;

    public RoutableCommandDescriptor(IRoutableCommand command)
    {
        Noun = command.Noun;
        Verb = command.Verb;
        Description = command.Description;
        Group = command.Group;
        IsEnabled = command.IsEnabled;
        ShowInList = command.ShowInList;
        _factory = _ => command; // already instantiated
    }

    public RoutableCommandDescriptor(
        string noun,
        string verb,
        Func<IServiceProvider, ICommand> factory,
        string? description = null,
        string? group = null,
        bool isEnabled = true,
        bool showInList = true)
    {
        Noun = noun;
        Verb = verb;
        Description = description;
        Group = group;
        IsEnabled = isEnabled;
        ShowInList = showInList;
        _factory = factory;
    }

    public string Noun { get; }
    public string Verb { get; }
    public string? Description { get; }
    public string? Group { get; }
    public bool IsEnabled { get; }
    public bool ShowInList { get; }

    public ICommand Resolve(IServiceProvider services) => _factory(services);
}
