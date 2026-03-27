namespace Axiom.Core.Failures;

internal sealed class AutoDetectFailureStrategy : IFailureStrategy
{
    internal static AutoDetectFailureStrategy Instance { get; } = new();

    private readonly Lazy<IFailureStrategy> _resolvedStrategy;

    internal AutoDetectFailureStrategy()
        : this(AutoDetectFailureStrategyResolver.ResolveDefault)
    {
    }

    internal AutoDetectFailureStrategy(Func<IFailureStrategy> resolveStrategy)
    {
        ArgumentNullException.ThrowIfNull(resolveStrategy);
        _resolvedStrategy = new Lazy<IFailureStrategy>(resolveStrategy, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    internal IFailureStrategy ResolvedStrategy => _resolvedStrategy.Value;

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);
        _resolvedStrategy.Value.Fail(message, callerFilePath, callerLineNumber);
    }
}
