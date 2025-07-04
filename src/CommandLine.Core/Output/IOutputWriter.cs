using SandlotWizards.CommandLineParser.Core;
using System;

namespace SandlotWizards.CommandLineParser.Output
{
    public interface IOutputWriter
    {
        void WriteHeader();
        void WriteTrailer();
        void WriteResult(CommandResult result);
        void WriteHelp(string helpText);
        void WriteError(Exception ex);
    }
}
