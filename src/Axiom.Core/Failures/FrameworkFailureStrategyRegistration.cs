namespace Axiom.Core.Failures;

internal sealed record FrameworkFailureStrategyRegistration(
    FrameworkFailureStrategyDefinition Definition,
    IFailureStrategy Strategy);
