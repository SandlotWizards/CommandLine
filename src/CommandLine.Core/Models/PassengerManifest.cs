using System.Collections.Generic;

namespace SandlotWizards.CommandLineParser.Models
{
    public class PassengerManifest
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string EntryPoint { get; set; } = default!;
        public List<PassengerCommand> Commands { get; set; } = new();
    }
}
