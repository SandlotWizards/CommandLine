using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Parsing;
public static class Tokenizer
{
    public static List<string> Tokenize(string input)
    {
        return [.. input.Split(' ')];
    }
}
