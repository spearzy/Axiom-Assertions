namespace Axiom.Core.Failures;

internal static class AutoDetectFailureStrategyResolver
{
    internal static IReadOnlyList<FrameworkFailureStrategyRegistration> BuiltInRegistrations { get; } =
    [
        new(FrameworkFailureStrategyDefinitions.Xunit, XunitFailureStrategy.Instance),
        new(FrameworkFailureStrategyDefinitions.NUnit, NUnitFailureStrategy.Instance),
        new(FrameworkFailureStrategyDefinitions.MSTest, MSTestFailureStrategy.Instance)
    ];

    internal static IFailureStrategy ResolveDefault()
    {
        return Resolve(BuiltInRegistrations, FrameworkFailureExceptionFactory.IsAvailable);
    }

    internal static IFailureStrategy Resolve(
        IReadOnlyList<FrameworkFailureStrategyRegistration> registrations,
        Func<FrameworkFailureStrategyDefinition, bool> isAvailable)
    {
        ArgumentNullException.ThrowIfNull(registrations);
        ArgumentNullException.ThrowIfNull(isAvailable);

        foreach (var registration in registrations)
        {
            if (isAvailable(registration.Definition))
            {
                return registration.Strategy;
            }
        }

        return InvalidOperationFailureStrategy.Instance;
    }
}
