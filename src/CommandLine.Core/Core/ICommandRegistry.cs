using System.Collections.Generic;
using System;

namespace SandlotWizards.CommandLineParser.Core;

public interface ICommandRegistry
{
    void Register(string noun, string verb, ICommand command);
    void Register(string noun, string verb, Func<IServiceProvider, ICommand> factory);
    object? Resolve(string noun, string verb);
    IEnumerable<object> ListAll();
}
