using SandlotWizards.CommandLineParser.Core;
using System;

public class CommandDescriptorAdapter : IRoutableCommandDescriptor
{
    private readonly ICommand _command;
    public string Noun { get; }
    public string Verb { get; }

    public string? Description => null; // or a default/fallback
    public string? Group => null;
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public CommandDescriptorAdapter(string noun, string verb, ICommand command)
    {
        Noun = noun;
        Verb = verb;
        _command = command;
    }

    public ICommand Resolve(IServiceProvider services) => _command;
}
