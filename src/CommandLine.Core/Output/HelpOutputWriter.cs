using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;

namespace SandlotWizards.CommandLineParser.Output
{
    public class HelpOutputWriter : IOutputWriter
    {
        public void WriteHeader() { ActionLog.Global.PrintHeader(ConsoleColor.Cyan); }
        public void WriteTrailer() { ActionLog.Global.PrintTrailer(ConsoleColor.Cyan); }
        public void WriteResult(CommandResult result) => WriteHelp(string.Join("\n", result.Messages ?? Array.Empty<string>()));
        public void WriteHelp(string helpText) => ActionLog.Global.Message(helpText);
        public void WriteError(Exception ex) => ActionLog.Global.Error(ex.Message);
    }
}
