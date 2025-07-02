using System.Collections.Generic;

namespace SandlotWizards.CommandLineParser.Core;

public class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<(string Noun, string Verb), ICommand> _commands = new();

    public void Register(string noun, string verb, ICommand command)
    {
        _commands[(noun, verb)] = command;
    }

    public bool TryGet(string noun, string verb, out ICommand? command)
    {
        return _commands.TryGetValue((noun, verb), out command);
    }
}
