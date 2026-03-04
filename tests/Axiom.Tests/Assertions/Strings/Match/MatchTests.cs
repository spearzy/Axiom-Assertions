using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.Match;

public sealed class MatchTests
{
    [Fact]
    public void Match_ReturnsContinuation_WhenValueMatchesPattern()
    {
        const string value = "AB-123";

        var baseAssertions = value.Should();
        var continuation = baseAssertions.Match(@"^[A-Z]{2}-\d{3}$");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void Match_Throws_WhenValueDoesNotMatchPattern()
    {
        const string value = "AB-12";

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Match(@"^[A-Z]{2}-\d{3}$"));

        const string expected = "Expected value to match regex \"^[A-Z]{2}-\\d{3}$\", but found \"AB-12\".";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Match_Throws_WhenValueIsNull()
    {
        string? value = null;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Match(@"\w+"));

        const string expected = "Expected value to match regex \"\\w+\", but found <null>.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Match_Throws_WithReason_WhenProvided()
    {
        const string value = "AB-12";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().Match(@"^[A-Z]{2}-\d{3}$", "ticket IDs must include 3 digits"));

        Assert.Contains("because ticket IDs must include 3 digits", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Match_ThrowsArgumentException_WhenPatternIsInvalid()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentException>(() => value.Should().Match("["));

        Assert.Equal("pattern", ex.ParamName);
        Assert.Contains("Invalid regex pattern", ex.Message, StringComparison.Ordinal);
    }
}
