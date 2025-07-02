using System;

namespace SandlotWizards.CommandLineParser.Core;

public class RoutableCommandDescriptor : IRoutableCommandDescriptor
{
    public string Noun { get; }
    public string Verb { get; }
    private readonly Func<IServiceProvider, ICommand> _factory;

    public RoutableCommandDescriptor(string noun, string verb, Func<IServiceProvider, ICommand> factory)
    {
        Noun = noun;
        Verb = verb;
        _factory = factory;
    }

    public ICommand Resolve(IServiceProvider services) => _factory(services);
}
