namespace Axiom.Tests.Assertions.Actions.Batch;

public sealed class AsyncActionBatchRoutingTests
{
    [Fact]
    public async Task ThrowAsync_OutsideBatch_ThrowsImmediately()
    {
        Func<Task> action = static () => Task.CompletedTask;

        await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());
    }

    [Fact]
    public async Task ThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Func<Task> action = static () => Task.CompletedTask;

        using var batch = new Axiom.Core.Batch();
        var callEx = await Xunit.Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        Xunit.Assert.Null(callEx);
        Xunit.Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_FromAsyncActionAssertions()
    {
        Func<Task> noThrow = static () => Task.CompletedTask;
        Func<ValueTask> wrongThrow = static () => ValueTask.FromException(new ArgumentException("bad"));

        var ex = await Xunit.Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async actions");
            await noThrow.Should().ThrowAsync<InvalidOperationException>();
            await wrongThrow.Should().ThrowAsync<InvalidOperationException>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Xunit.Assert.Contains("Batch 'async actions' failed with 2 assertion failure(s):", message);
        Xunit.Assert.Contains($"1) Expected noThrow to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Xunit.Assert.Contains($"2) Expected wrongThrow to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.", message);
    }
}
