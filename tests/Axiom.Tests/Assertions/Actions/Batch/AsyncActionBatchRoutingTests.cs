using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.Batch;

public sealed class AsyncActionBatchRoutingTests
{
    [Fact]
    public async Task ThrowAsync_OutsideBatch_ThrowsImmediately()
    {
        Func<Task> action = static () => Task.CompletedTask;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());
    }

    [Fact]
    public async Task ThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Func<Task> action = static () => Task.CompletedTask;

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await action.Should().ThrowAsync<InvalidOperationException>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_FromAsyncActionAssertions()
    {
        Func<Task> noThrow = static () => Task.CompletedTask;
        Func<ValueTask> wrongThrow = static () => ValueTask.FromException(new ArgumentException("bad"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async actions");
            await noThrow.Should().ThrowAsync<InvalidOperationException>();
            await wrongThrow.Should().ThrowAsync<InvalidOperationException>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'async actions' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrow to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains($"2) Expected wrongThrow to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.", message);
    }

    [Fact]
    public async Task ThrowExactlyAsync_OutsideBatch_ThrowsImmediately()
    {
        Func<Task> action = static () => Task.CompletedTask;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().ThrowExactlyAsync<InvalidOperationException>());
    }

    [Fact]
    public async Task ThrowExactlyAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Func<Task> action = static () => Task.CompletedTask;

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await action.Should().ThrowExactlyAsync<InvalidOperationException>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task NotThrowAsync_OutsideBatch_ThrowsImmediately()
    {
        Func<Task> action = static () => Task.FromException(new ArgumentException("bad"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await action.Should().NotThrowAsync());
    }

    [Fact]
    public async Task NotThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Func<Task> action = static () => Task.FromException(new ArgumentException("bad"));

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await action.Should().NotThrowAsync());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public async Task Batch_Dispose_ThrowsCombinedFailures_FromStrictAndNotThrowAsyncAssertions()
    {
        Func<Task> noThrow = static () => Task.CompletedTask;
        Func<ValueTask> hasThrow = static () => ValueTask.FromException(new ArgumentException("bad"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            using var batch = new Axiom.Core.Batch("async-action-extended");
            await noThrow.Should().ThrowExactlyAsync<InvalidOperationException>();
            await hasThrow.Should().NotThrowAsync();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'async-action-extended' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrow to throw exactly {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains("2) Expected hasThrow to not throw, but found System.ArgumentException.", message);
    }
}
