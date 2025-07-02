using System;

namespace SandlotWizards.CommandLineParser.Core;

public interface IRoutableCommandDescriptor
{
    string Noun { get; }
    string Verb { get; }
    ICommand Resolve(IServiceProvider services);
}
