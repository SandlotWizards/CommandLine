using SandlotWizards.CommandLineParser.Execution;
using System;

namespace SandlotWizards.CommandLineParser.IO.Output
{
    public interface IOutputWriter
    {
        void WriteHeader();
        void WriteTrailer();
        void WriteResult(CommandResult result);
        void WriteHelp(string helpText);
        void WriteError(Exception ex);
        void WriteSplash(string splashText); 
    }
}
