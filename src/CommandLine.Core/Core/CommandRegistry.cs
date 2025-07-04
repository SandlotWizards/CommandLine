using System;
using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Core;

public class CommandRegistry : ICommandRegistry
{
    private readonly List<object> _commands = new();

    public void Register(string noun, string verb, ICommand command)
    {
        _commands.Add(new CommandDescriptorAdapter(noun, verb, command));
    }

    public void Register(string noun, string verb, Func<IServiceProvider, ICommand> factory)
    {
        _commands.Add(new RoutableCommandDescriptor(noun, verb, factory));
    }

    public void Register(IRoutableCommandDescriptor descriptor)
    {
        _commands.Add(descriptor);
    }

    public object? Resolve(string noun, string verb)
    {
        return _commands.FirstOrDefault(c =>
            (c is CommandDescriptorAdapter a && a.Noun == noun && a.Verb == verb) ||
            (c is RoutableCommandDescriptor r && r.Noun == noun && r.Verb == verb));
    }

    public IEnumerable<object> GetAll() => _commands;
}
