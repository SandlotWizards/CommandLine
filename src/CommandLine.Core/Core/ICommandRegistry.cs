namespace SandlotWizards.CommandLineParser.Core;

public interface ICommandRegistry
{
    void Register(string noun, string verb, ICommand command);
    bool TryGet(string noun, string verb, out ICommand? command);
}
