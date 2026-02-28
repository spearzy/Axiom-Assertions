namespace Axiom.Tests.Assertions.Actions.Throw;

public sealed class ThrowTests
{
    [Fact]
    public void Throw_Passes_WhenExpectedExceptionTypeIsThrown()
    {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = Xunit.Record.Exception(() => action.Should().Throw<InvalidOperationException>());

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void Throw_Passes_WhenDerivedExceptionTypeIsThrown()
    {
        Action action = static () => throw new ArgumentNullException("value");

        var ex = Xunit.Record.Exception(() => action.Should().Throw<ArgumentException>());

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void Throw_Throws_WhenNoExceptionIsThrown()
    {
        Action action = static () => { };

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Throw_Throws_WhenDifferentExceptionTypeIsThrown()
    {
        Action action = static () => throw new ArgumentException("bad");

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Throw_Throws_WithReason_WhenProvided()
    {
        Action action = static () => { };

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>("this code path must fail fast"));

        Xunit.Assert.Contains("because this code path must fail fast", ex.Message);
    }
}
