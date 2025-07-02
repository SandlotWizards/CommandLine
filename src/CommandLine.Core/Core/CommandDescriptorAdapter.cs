using SandlotWizards.CommandLineParser.Core;
using System;

public class CommandDescriptorAdapter : IRoutableCommandDescriptor
{
    public string Noun { get; }
    public string Verb { get; }
    private readonly ICommand _handler;

    public CommandDescriptorAdapter(string noun, string verb, ICommand handler)
    {
        Noun = noun;
        Verb = verb;
        _handler = handler;
    }

    public ICommand Resolve(IServiceProvider _) => _handler;
}