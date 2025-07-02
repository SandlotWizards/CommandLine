using SandlotWizards.CommandLineParser.Core;

namespace SandlotWizards.CommandLineParser.Parsing
{
    public interface IContextParser
    {
        CommandContext Parse(string[] args);
    }
}