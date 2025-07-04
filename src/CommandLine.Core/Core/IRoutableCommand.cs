namespace SandlotWizards.CommandLineParser.Core;

public interface IRoutableCommand : ICommand
{
    string Noun { get; }
    string Verb { get; }
    string? Description { get; }
    string? Group { get; }

    bool IsEnabled { get; }
    bool ShowInList { get; }
}
