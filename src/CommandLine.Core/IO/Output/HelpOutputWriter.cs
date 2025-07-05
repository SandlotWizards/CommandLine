using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using System;

namespace SandlotWizards.CommandLineParser.IO.Output
{
    public class HelpOutputWriter : IOutputWriter
    {
        public void WriteHeader() { ActionLog.Global.PrintHeader(ConsoleColor.Cyan); }
        public void WriteTrailer() { ActionLog.Global.PrintTrailer(ConsoleColor.Cyan); }
        public void WriteResult(CommandResult result) => WriteHelp(string.Join("\n", result.Messages ?? Array.Empty<string>()));
        public void WriteHelp(string helpText) => ActionLog.Global.Message(helpText);
        public void WriteError(Exception ex) => ActionLog.Global.Error(ex.Message);

        public void WriteSplash(string splashText)
        {
            ActionLog.Global.Message(splashText, ConsoleColor.Yellow);
        }
    }
}
