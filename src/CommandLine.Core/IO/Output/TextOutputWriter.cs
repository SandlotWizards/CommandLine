using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using System;

namespace SandlotWizards.CommandLineParser.IO.Output
{
    public class TextOutputWriter : IOutputWriter
    {
        public void WriteHeader()
        {
            ActionLog.Global.PrintHeader(ConsoleColor.Cyan);
        }

        public void WriteTrailer()
        {
            ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
        }

        public void WriteResult(CommandResult result)
        {
            foreach (var message in result.Messages ?? Array.Empty<string>())
                ActionLog.Global.Message(message);
        }

        public void WriteHelp(string helpText)
        {
            ActionLog.Global.Message(helpText);
        }

        public void WriteError(Exception ex)
        {
            ActionLog.Global.Error(ex.Message);
        }

        public void WriteSplash(string splashText)
        {
            ActionLog.Global.Message(splashText, ConsoleColor.Yellow);
        }
    }
}
