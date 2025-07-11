﻿namespace SandlotWizards.CommandLineParser.Models
{
    public class PassengerCommand
    {
        public string Noun { get; set; } = default!;
        public string Verb { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Group { get; set; } = default!;
        public bool IsEnabled { get; set; } = default!;
        public bool ShowInList { get; set; } = default!;
    }
}
