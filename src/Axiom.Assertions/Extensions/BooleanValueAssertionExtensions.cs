using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static class BooleanValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<bool>> BeTrue(
        this ValueAssertions<bool> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return assertions.Be(true, because, callerFilePath, callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<bool?>> BeTrue(
        this ValueAssertions<bool?> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return assertions.Be(true, because, callerFilePath, callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<bool>> BeFalse(
        this ValueAssertions<bool> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return assertions.Be(false, because, callerFilePath, callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<bool?>> BeFalse(
        this ValueAssertions<bool?> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        return assertions.Be(false, because, callerFilePath, callerLineNumber);
    }
}
