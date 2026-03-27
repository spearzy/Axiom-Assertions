using System.Reflection;
using Axiom.Assertions.Configuration;
using Xunit.Sdk;

namespace Axiom.Tests;

public sealed class UseInvalidOperationFailureStrategyByDefaultAttribute : BeforeAfterTestAttribute
{
    public override void Before(MethodInfo methodUnderTest)
    {
        AxiomSettings.Reset();
        AxiomServices.Configure(c => c.FailureStrategy = InvalidOperationFailureStrategy.Instance);
    }

    public override void After(MethodInfo methodUnderTest)
    {
        AxiomSettings.Reset();
        AxiomServices.Configure(c => c.FailureStrategy = InvalidOperationFailureStrategy.Instance);
    }
}
