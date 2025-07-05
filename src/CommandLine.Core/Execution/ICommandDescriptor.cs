using System.Collections.Generic;

namespace SandlotWizards.CommandLineParser.Execution;

public interface ICommandDescriptor
{
    List<string> Aliases { get; init; }
    string Description { get; init; }
    List<string> Examples { get; init; }
    string Group { get; init; }
    ICommand Handler { get; init; }
    string HelpText { get; init; }
    string Name { get; init; }
    string WhyExplanation { get; init; }

    bool Matches(string input);
}