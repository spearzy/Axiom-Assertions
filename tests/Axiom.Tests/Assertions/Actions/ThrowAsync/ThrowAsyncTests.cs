using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.ThrowAsync;

public sealed class ThrowAsyncTests
{
    [Fact]
    public async Task ThrowAsync_Passes_WhenExpectedExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Xunit.Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowAsync_Passes_WhenDerivedExceptionTypeIsThrown()
    {
        Func<ValueTask> action = static () => ValueTask.FromException(new ArgumentNullException("value"));

        var ex = await Xunit.Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<ArgumentException>());

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WhenNoExceptionIsThrown()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found <no exception>.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WhenDifferentExceptionTypeIsThrown()
    {
        Func<Task> action = static () => Task.FromException(new ArgumentException("bad"));

        var ex = await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        var expected = $"Expected action to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public async Task ThrowAsync_Throws_WithReason_WhenProvided()
    {
        Func<Task> action = static () => Task.CompletedTask;

        var ex = await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>("this code path must fail fast"));

        Assert.Contains("because this code path must fail fast", ex.Message);
    }
}
