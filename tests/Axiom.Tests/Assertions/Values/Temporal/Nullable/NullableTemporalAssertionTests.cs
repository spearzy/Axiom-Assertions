using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.Nullable;

public sealed class NullableTemporalAssertionTests
{
    [Fact]
    public void NullableDateTimeTemporalAssertions_Pass_WhenSubjectHasValue()
    {
        DateTime? actual = new DateTime(2026, 05, 07, 10, 00, 00, DateTimeKind.Utc);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions
            .BeBefore(actual.Value.AddMinutes(1)).And
            .BeOnOrBefore(actual.Value).And
            .BeAfter(actual.Value.AddMinutes(-1)).And
            .BeOnOrAfter(actual.Value).And
            .BeWithin(actual.Value.AddMilliseconds(100), TimeSpan.FromMilliseconds(200)).And
            .NotBeWithin(actual.Value.AddSeconds(1), TimeSpan.FromMilliseconds(200)).And
            .BeBetween(actual.Value.AddMinutes(-1), actual.Value.AddMinutes(1));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NullableDateTimeTemporalAssertions_FailThroughNormalPath_WhenSubjectIsNull()
    {
        DateTime? actual = null;
        var expected = new DateTime(2026, 05, 07, 10, 00, 00, DateTimeKind.Utc);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeBefore(expected));

        Assert.Contains("Expected actual to be before", ex.Message, StringComparison.Ordinal);
        Assert.Contains("found <null>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableDateTimeOffsetTemporalAssertions_Pass_WhenSubjectHasValue()
    {
        DateTimeOffset? actual = new DateTimeOffset(2026, 05, 07, 10, 00, 00, TimeSpan.Zero);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions
            .BeBefore(actual.Value.AddMinutes(1)).And
            .BeOnOrBefore(actual.Value).And
            .BeAfter(actual.Value.AddMinutes(-1)).And
            .BeOnOrAfter(actual.Value).And
            .BeWithin(actual.Value.AddMilliseconds(100), TimeSpan.FromMilliseconds(200)).And
            .NotBeWithin(actual.Value.AddSeconds(1), TimeSpan.FromMilliseconds(200)).And
            .BeBetween(actual.Value.AddMinutes(-1), actual.Value.AddMinutes(1));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NullableDateTimeOffsetTemporalAssertions_FailThroughNormalPath_WhenSubjectIsNull()
    {
        DateTimeOffset? actual = null;
        var expected = new DateTimeOffset(2026, 05, 07, 10, 00, 00, TimeSpan.Zero);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Contains("Expected actual to be within 00:00:00.5000000 of", ex.Message, StringComparison.Ordinal);
        Assert.Contains("found <null>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableDateOnlyTemporalAssertions_Pass_WhenSubjectHasValue()
    {
        DateOnly? actual = new DateOnly(2026, 05, 07);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions
            .BeBefore(actual.Value.AddDays(1)).And
            .BeOnOrBefore(actual.Value).And
            .BeAfter(actual.Value.AddDays(-1)).And
            .BeOnOrAfter(actual.Value).And
            .BeWithin(actual.Value.AddDays(1), TimeSpan.FromDays(1)).And
            .NotBeWithin(actual.Value.AddDays(2), TimeSpan.FromDays(1)).And
            .BeBetween(actual.Value.AddDays(-1), actual.Value.AddDays(1));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NullableDateOnlyTemporalAssertions_FailThroughNormalPath_WhenSubjectIsNull()
    {
        DateOnly? actual = null;
        var expected = new DateOnly(2026, 05, 07);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeBetween(expected.AddDays(-1), expected.AddDays(1)));

        Assert.Contains("Expected actual to be between", ex.Message, StringComparison.Ordinal);
        Assert.Contains("found <null>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableTimeOnlyTemporalAssertions_Pass_WhenSubjectHasValue()
    {
        TimeOnly? actual = new TimeOnly(10, 00, 00);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions
            .BeBefore(actual.Value.Add(TimeSpan.FromMinutes(1))).And
            .BeOnOrBefore(actual.Value).And
            .BeAfter(actual.Value.Add(TimeSpan.FromMinutes(-1))).And
            .BeOnOrAfter(actual.Value).And
            .BeWithin(actual.Value.Add(TimeSpan.FromMilliseconds(100)), TimeSpan.FromMilliseconds(200)).And
            .NotBeWithin(actual.Value.Add(TimeSpan.FromSeconds(1)), TimeSpan.FromMilliseconds(200)).And
            .BeBetween(actual.Value.Add(TimeSpan.FromMinutes(-1)), actual.Value.Add(TimeSpan.FromMinutes(1)));

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NullableTimeOnlyTemporalAssertions_FailThroughNormalPath_WhenSubjectIsNull()
    {
        TimeOnly? actual = null;
        var expected = new TimeOnly(10, 00, 00);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeOnOrAfter(expected));

        Assert.Contains("Expected actual to be on or after", ex.Message, StringComparison.Ordinal);
        Assert.Contains("found <null>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NullableTemporalAssertions_ValidateToleranceBeforeNullSubjectFailure()
    {
        DateTime? dateTime = null;
        DateTimeOffset? dateTimeOffset = null;
        DateOnly? dateOnly = null;
        TimeOnly? timeOnly = null;

        Assert.Equal("tolerance", Assert.Throws<ArgumentOutOfRangeException>(() =>
            dateTime.Should().BeWithin(DateTime.UtcNow, TimeSpan.MinValue)).ParamName);
        Assert.Equal("tolerance", Assert.Throws<ArgumentOutOfRangeException>(() =>
            dateTimeOffset.Should().BeWithin(DateTimeOffset.UtcNow, TimeSpan.MinValue)).ParamName);
        Assert.Equal("tolerance", Assert.Throws<ArgumentOutOfRangeException>(() =>
            dateOnly.Should().BeWithin(DateOnly.FromDateTime(DateTime.UtcNow), TimeSpan.MinValue)).ParamName);
        Assert.Equal("tolerance", Assert.Throws<ArgumentOutOfRangeException>(() =>
            timeOnly.Should().BeWithin(TimeOnly.FromDateTime(DateTime.UtcNow), TimeSpan.MinValue)).ParamName);
    }

    [Fact]
    public void NullableTemporalAssertion_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        DateTime? actual = new DateTime(2026, 05, 07, 10, 00, 00, DateTimeKind.Utc);
        var expected = actual.Value.AddSeconds(2);

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().BeWithin(expected, TimeSpan.FromMilliseconds(500)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
