using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class TemporalAssertionEngine
{
    public static void AssertBeBefore(
        DateTime? subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be before", static (left, right) => left < right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateTime? subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or before", static (left, right) => left <= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTime? subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be after", static (left, right) => left > right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateTime? subject, string? subjectExpression, DateTime expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or after", static (left, right) => left >= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTime? subject, string? subjectExpression, DateTime expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => (left - right).Duration(), because, callerFilePath, callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateTime? subject, string? subjectExpression, DateTime expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => (left - right).Duration(), because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBetween(
        DateTime? subject, string? subjectExpression, DateTime lowerBound, DateTime upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalRange(
            subject, subjectExpression, lowerBound, upperBound, "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be before", static (left, right) => left < right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or before", static (left, right) => left <= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be after", static (left, right) => left > right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or after", static (left, right) => left >= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => (left - right).Duration(), because, callerFilePath, callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => (left - right).Duration(), because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBetween(
        DateTimeOffset? subject, string? subjectExpression, DateTimeOffset lowerBound, DateTimeOffset upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalRange(
            subject, subjectExpression, lowerBound, upperBound, "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        DateOnly? subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be before", static (left, right) => left < right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        DateOnly? subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or before", static (left, right) => left <= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        DateOnly? subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be after", static (left, right) => left > right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        DateOnly? subject, string? subjectExpression, DateOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or after", static (left, right) => left >= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        DateOnly? subject, string? subjectExpression, DateOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            static (left, right) => TimeSpan.FromDays(Math.Abs(left.DayNumber - right.DayNumber)),
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertNotBeWithin(
        DateOnly? subject, string? subjectExpression, DateOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            static (left, right) => TimeSpan.FromDays(Math.Abs(left.DayNumber - right.DayNumber)),
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBetween(
        DateOnly? subject, string? subjectExpression, DateOnly lowerBound, DateOnly upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalRange(
            subject, subjectExpression, lowerBound, upperBound, "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBefore(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be before", static (left, right) => left < right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrBefore(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or before", static (left, right) => left <= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeAfter(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be after", static (left, right) => left > right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeOnOrAfter(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, string? because,
        string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalComparison(
            subject, subjectExpression, expected, "to be on or after", static (left, right) => left >= right,
            because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeWithin(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "to be within",
            static (difference, normalisedTolerance) => difference <= normalisedTolerance,
            AbsoluteTimeOnlyDifference, because, callerFilePath, callerLineNumber);
    }

    public static void AssertNotBeWithin(
        TimeOnly? subject, string? subjectExpression, TimeOnly expected, TimeSpan tolerance,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalTolerance(
            subject, subjectExpression, expected, tolerance, "not to be within",
            static (difference, normalisedTolerance) => difference > normalisedTolerance,
            AbsoluteTimeOnlyDifference, because, callerFilePath, callerLineNumber);
    }

    public static void AssertBeBetween(
        TimeOnly? subject, string? subjectExpression, TimeOnly lowerBound, TimeOnly upperBound,
        string? because, string? callerFilePath, int callerLineNumber)
    {
        AssertNullableTemporalRange(
            subject, subjectExpression, lowerBound, upperBound, "to be between",
            static (value, lower, upper) => value >= lower && value <= upper,
            because, callerFilePath, callerLineNumber);
    }

    private static void AssertNullableTemporalComparison<T>(
        T? subject, string? subjectExpression, T expected, string expectationText, Func<T, T, bool> passes,
        string? because, string? callerFilePath, int callerLineNumber)
        where T : struct
    {
        if (subject.HasValue && passes(subject.Value, expected))
        {
            return;
        }

        FailNullableTemporal(subject, subjectExpression, new Expectation(expectationText, expected), because,
            callerFilePath, callerLineNumber);
    }

    private static void AssertNullableTemporalTolerance<T>(
        T? subject, string? subjectExpression, T expected, TimeSpan tolerance, string expectationText,
        Func<TimeSpan, TimeSpan, bool> passes, Func<T, T, TimeSpan> difference,
        string? because, string? callerFilePath, int callerLineNumber)
        where T : struct
    {
        var normalisedTolerance = NormaliseTolerance(tolerance);
        if (subject.HasValue && passes(difference(subject.Value, expected), normalisedTolerance))
        {
            return;
        }

        FailNullableTemporal(
            subject,
            subjectExpression,
            new Expectation($"{expectationText} {normalisedTolerance} of", expected),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertNullableTemporalRange<T>(
        T? subject, string? subjectExpression, T lowerBound, T upperBound, string expectationText,
        Func<T, T, T, bool> passes, string? because, string? callerFilePath, int callerLineNumber)
        where T : struct
    {
        if (subject.HasValue && passes(subject.Value, lowerBound, upperBound))
        {
            return;
        }

        FailNullableTemporal(
            subject,
            subjectExpression,
            new Expectation(expectationText, new InclusiveRange<T>(lowerBound, upperBound)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void FailNullableTemporal<T>(
        T? subject, string? subjectExpression, Expectation expectation, string? because,
        string? callerFilePath, int callerLineNumber)
        where T : struct
    {
        var failure = new Failure(SubjectLabel(subjectExpression), expectation, subject, because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }
}
