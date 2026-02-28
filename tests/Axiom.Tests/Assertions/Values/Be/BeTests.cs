namespace Axiom.Tests.Assertions.Values.Be;

public sealed class BeTests
{
    [Fact]
    public void Be_DoesNotThrow_WhenValuesAreEqual()
    {
        var value = 42;

        var ex = Xunit.Record.Exception(() => value.Should().Be(42));

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void Be_Throws_WhenValuesDiffer()
    {
        var value = 42;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));

        const string expected = "Expected value to be 7, but found 42.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_Throws_WithReason_WhenProvided()
    {
        var value = 42;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            value.Should().Be(7, "input should align with seeded data"));

        Xunit.Assert.Contains("because input should align with seeded data", ex.Message);
    }
}
