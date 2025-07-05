using System;

namespace SandlotWizards.CommandLineParser.BuiltIn
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExcludeFromCommandDiscoveryAttribute : Attribute { }
}
