using System.Threading.Tasks.Sources;

namespace Axiom.Tests.Assertions.Tasks;

public sealed class AsyncFunctionAssertionsTests
{
    [Fact]
    public async Task Succeed_ReturnsWhoseResult_ForFuncOfTaskOfT()
    {
        Func<Task<int>> subject = () => Task.FromResult(42);

        var assertions = subject.Should();
        var continuation = await assertions.Succeed();

        Assert.Same(assertions, continuation.And);
        Assert.Equal(42, continuation.WhoseResult);
    }

    [Fact]
    public async Task Succeed_ReturnsWhoseResult_ForFuncOfValueTaskOfT()
    {
        Func<ValueTask<string>> subject = () => ValueTask.FromResult("axiom");

        var assertions = subject.Should();
        var continuation = await assertions.Succeed();

        Assert.Same(assertions, continuation.And);
        Assert.Equal("axiom", continuation.WhoseResult);
    }

    [Fact]
    public async Task ThrowAsync_CapturesSynchronousException_BeforeTaskIsReturned()
    {
        Func<Task<int>> subject = () => throw new InvalidOperationException("sync");

        var continuation = await subject.Should().ThrowAsync<InvalidOperationException>();

        Assert.Equal("sync", continuation.Thrown.Message);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Passes_ForExactAsynchronousFault()
    {
        Func<Task<int>> subject = () => Task.FromException<int>(new ArgumentNullException("value"));

        var continuation = await subject.Should().ThrowExactlyAsync<ArgumentNullException>();

        Assert.Equal("value", continuation.Thrown.ParamName);
    }

    [Fact]
    public async Task ThrowExactlyAsync_Throws_ForBaseExceptionType()
    {
        Func<Task<int>> subject = () => Task.FromException<int>(new ArgumentNullException("value"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().ThrowExactlyAsync<ArgumentException>());

        Assert.Equal(
            $"Expected subject to throw exactly {typeof(ArgumentException)}, but found {typeof(ArgumentNullException)}.",
            ex.Message);
    }

    [Fact]
    public async Task BeFaultedWith_Passes_AndExposesThrown_ForFaultedReturnedTask()
    {
        Func<Task<int>> subject = () => Task.FromException<int>(new ArgumentNullException("value"));

        var continuation = await subject.Should().BeFaultedWith<ArgumentNullException>();

        Assert.Equal("value", continuation.Thrown.ParamName);
    }

    [Fact]
    public async Task BeCanceled_Passes_ForCanceledReturnedTask()
    {
        Func<Task<int>> subject = () => Task.FromCanceled<int>(new CancellationToken(canceled: true));

        var assertions = subject.Should();
        var continuation = await assertions.BeCanceled();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task CompleteWithin_Throws_WhenReturnedTaskDoesNotFinishInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task<int>> subject = () => completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().CompleteWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(1);

        Assert.Equal("Expected subject to complete within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task NotCompleteWithin_Passes_WhenReturnedTaskDoesNotFinishInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task<int>> subject = () => completion.Task;

        var assertions = subject.Should();
        var continuation = await assertions.NotCompleteWithin(TimeSpan.FromMilliseconds(10));

        completion.TrySetResult(1);

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task NotCompleteWithin_Throws_WhenReturnedTaskCompletesInTime()
    {
        Func<Task<int>> subject = () => Task.FromResult(5);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().NotCompleteWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal("Expected subject to not complete within 00:00:00.0100000, but found <completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task NotThrowAsync_Passes_WhenReturnedTaskCompletesSuccessfully()
    {
        Func<Task<int>> subject = () => Task.FromResult(5);

        var assertions = subject.Should();
        var continuation = await assertions.NotThrowAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task NotThrowAsync_Throws_WhenInvocationThrowsSynchronously()
    {
        Func<Task<int>> subject = () => throw new ArgumentException("bad");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().NotThrowAsync());

        Assert.Equal("Expected subject to not throw, but found System.ArgumentException.", ex.Message);
    }

    [Fact]
    public async Task Succeed_Throws_WhenReturnedTaskFaults()
    {
        Func<Task<int>> subject = () => Task.FromException<int>(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().Succeed());

        Assert.Equal("Expected subject to succeed, but found System.InvalidOperationException.", ex.Message);
    }

    [Fact]
    public async Task SucceedWithin_ReturnsWhoseResult_ForFastReturnedTask()
    {
        Func<Task<int>> subject = () => Task.FromResult(99);

        var continuation = await subject.Should().SucceedWithin(TimeSpan.FromMilliseconds(10));

        Assert.Equal(99, continuation.WhoseResult);
    }

    [Fact]
    public async Task SucceedWithin_Throws_WhenInvocationThrowsSynchronously()
    {
        Func<Task<int>> subject = () => throw new InvalidOperationException("sync");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().SucceedWithin(TimeSpan.FromMilliseconds(10)));

        Assert.Equal("Expected subject to succeed within 00:00:00.0100000, but found System.InvalidOperationException.", ex.Message);
    }

    [Fact]
    public async Task BeCanceled_Throws_WhenReturnedTaskSucceeds()
    {
        Func<Task<int>> subject = () => Task.FromResult(7);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().BeCanceled());

        Assert.Equal("Expected subject to be canceled, but found <completed successfully>.", ex.Message);
    }

    [Fact]
    public async Task BeCanceledWithin_Passes_ForCanceledReturnedTask()
    {
        Func<Task<int>> subject = () => Task.FromCanceled<int>(new CancellationToken(canceled: true));

        var assertions = subject.Should();
        var continuation = await assertions.BeCanceledWithin(TimeSpan.FromMilliseconds(10));

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeCanceledWithin_Throws_WhenReturnedTaskDoesNotFinishInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task<int>> subject = () => completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().BeCanceledWithin(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(1);

        Assert.Equal("Expected subject to be canceled within 00:00:00.0100000, but found <not completed within timeout>.", ex.Message);
    }

    [Fact]
    public async Task BeFaultedWith_Throws_WhenReturnedTaskSucceeds()
    {
        Func<Task<int>> subject = () => Task.FromResult(5);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().BeFaultedWith<ArgumentException>());

        Assert.Equal($"Expected subject to be faulted with {typeof(ArgumentException)}, but found <completed successfully>.", ex.Message);
    }

    [Fact]
    public async Task BeFaultedWithWithin_Passes_ForFaultedReturnedTask()
    {
        Func<Task<int>> subject = () => Task.FromException<int>(new InvalidOperationException("boom"));

        var continuation = await subject.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10));

        Assert.Equal("boom", continuation.Thrown.Message);
    }

    [Fact]
    public async Task BeFaultedWithWithin_Throws_WhenReturnedTaskDoesNotFinishInTime()
    {
        var completion = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Func<Task<int>> subject = () => completion.Task;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await subject.Should().BeFaultedWithWithin<InvalidOperationException>(TimeSpan.FromMilliseconds(10)));

        completion.TrySetResult(1);

        Assert.Contains("Expected subject to be faulted with", ex.Message, StringComparison.Ordinal);
        Assert.Contains(typeof(InvalidOperationException).ToString(), ex.Message, StringComparison.Ordinal);
        Assert.Contains("<not completed within timeout>", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Assertions_CacheSingleUseValueTaskResult_WithoutDoubleConsumption()
    {
        var source = new SingleUseValueTaskSource<int>(42);
        var invocationCount = 0;
        Func<ValueTask<int>> subject = () =>
        {
            invocationCount++;
            return new ValueTask<int>(source, source.Version);
        };

        var assertions = subject.Should();
        var success = await assertions.Succeed();
        var noThrow = await assertions.NotThrowAsync();

        Assert.Equal(42, success.WhoseResult);
        Assert.Same(assertions, noThrow.And);
        Assert.Equal(1, invocationCount);
        Assert.Equal(1, source.GetResultCalls);
    }

    [Fact]
    public async Task ThrowAsync_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Func<Task<int>> subject = () => Task.FromResult(1);

        using var batch = new Axiom.Core.Batch();
        var callEx = await Record.ExceptionAsync(async () =>
            await subject.Should().ThrowAsync<InvalidOperationException>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    private sealed class SingleUseValueTaskSource<TResult> : IValueTaskSource<TResult>
    {
        private ManualResetValueTaskSourceCore<TResult> _core;
        private int _getResultCalls;

        public SingleUseValueTaskSource(TResult result)
        {
            _core = new ManualResetValueTaskSourceCore<TResult>
            {
                RunContinuationsAsynchronously = true
            };
            _core.SetResult(result);
        }

        public int GetResultCalls => _getResultCalls;

        public short Version => _core.Version;

        public TResult GetResult(short token)
        {
            if (Interlocked.Increment(ref _getResultCalls) > 1)
            {
                throw new InvalidOperationException("ValueTask result was consumed more than once.");
            }

            return _core.GetResult(token);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            _core.OnCompleted(continuation, state, token, flags);
        }
    }
}
