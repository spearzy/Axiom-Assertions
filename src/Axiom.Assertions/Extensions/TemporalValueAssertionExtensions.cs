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

    public static AndContinuation<ValueAssertions<DateOnly>> BeBefore(
        this ValueAssertions<DateOnly> assertions,
        DateOnly expected,
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

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeAfter(
        this ValueAssertions<DateOnly> assertions,
        DateOnly expected,
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

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<DateOnly>> BeWithin(
        this ValueAssertions<DateOnly> assertions,
        DateOnly expected,
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

        return new AndContinuation<ValueAssertions<DateOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeBefore(
        this ValueAssertions<TimeOnly> assertions,
        TimeOnly expected,
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

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeAfter(
        this ValueAssertions<TimeOnly> assertions,
        TimeOnly expected,
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

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TimeOnly>> BeWithin(
        this ValueAssertions<TimeOnly> assertions,
        TimeOnly expected,
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

        return new AndContinuation<ValueAssertions<TimeOnly>>(assertions);
    }
}
