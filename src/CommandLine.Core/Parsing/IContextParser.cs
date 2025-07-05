using SandlotWizards.CommandLineParser.Execution;

namespace SandlotWizards.CommandLineParser.Parsing
{
    public interface IContextParser
    {
        CommandContext Parse(string[] args);
    }
}