using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static class TemporalValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<DateTime>> BeBefore(
        this ValueAssertions<DateTime> assertions,
        DateTime expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeAfter(
        this ValueAssertions<DateTime> assertions,
        DateTime expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTime>> BeWithin(
        this ValueAssertions<DateTime> assertions,
        DateTime expected,
        TimeSpan tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTime>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeBefore(
        this ValueAssertions<DateTimeOffset> assertions,
        DateTimeOffset expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeBefore(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeAfter(
        this ValueAssertions<DateTimeOffset> assertions,
        DateTimeOffset expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeAfter(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateTimeOffset>> BeWithin(
        this ValueAssertions<DateTimeOffset> assertions,
        DateTimeOffset expected,
        TimeSpan tolerance,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        TemporalAssertionEngine.AssertBeWithin(
            assertions.Subject,
            assertions.SubjectExpression,
            expected,
            tolerance,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<DateTimeOffset>>(assertions);
    }
}
