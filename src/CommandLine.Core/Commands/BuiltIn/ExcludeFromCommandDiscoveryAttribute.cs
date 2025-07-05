using System;

namespace SandlotWizards.CommandLineParser.Commands.BuiltIn
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExcludeFromCommandDiscoveryAttribute : Attribute { }
}
