namespace Axiom.Core.Failures;

internal static class AssertionFailureCapture
{
    private static readonly AsyncLocal<CaptureScope?> CurrentScope = new();
    private static int _captureInvocationCount;

    internal static CapturedAssertionFailures Capture(Action assertionCallback)
    {
        ArgumentNullException.ThrowIfNull(assertionCallback);
        Interlocked.Increment(ref _captureInvocationCount);

        var scope = new CaptureScope(CurrentScope.Value);
        CurrentScope.Value = scope;

        try
        {
            assertionCallback();
        }
        catch (CapturedAssertionFailureException ex) when (ReferenceEquals(ex.Scope, scope))
        {
        }
        finally
        {
            CurrentScope.Value = scope.Previous;
        }

        return new CapturedAssertionFailures(scope.Failures);
    }

    internal static int CaptureInvocationCount => Volatile.Read(ref _captureInvocationCount);

    internal static void ResetProbe()
    {
        Interlocked.Exchange(ref _captureInvocationCount, 0);
    }

    internal static void CaptureIfActive(string message)
    {
        var scope = CurrentScope.Value;
        if (scope is null)
        {
            return;
        }

        scope.AddFailure(message);
        throw new CapturedAssertionFailureException(scope);
    }

    internal readonly record struct CapturedAssertionFailures(IReadOnlyList<string> Failures)
    {
        public bool HasFailures => Failures.Count > 0;

        public string? FirstFailureMessage => HasFailures ? Failures[0] : null;
    }

    private sealed class CaptureScope(CaptureScope? previous)
    {
        private List<string>? _failures;

        public CaptureScope? Previous { get; } = previous;

        public IReadOnlyList<string> Failures => _failures ?? (IReadOnlyList<string>)Array.Empty<string>();

        public void AddFailure(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            (_failures ??= []).Add(message);
        }
    }

    private sealed class CapturedAssertionFailureException(CaptureScope scope) : Exception
    {
        public CaptureScope Scope { get; } = scope;
    }
}
