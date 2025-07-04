using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;

namespace SandlotWizards.CommandLineParser.Output
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
    }
}
