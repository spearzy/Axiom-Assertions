namespace Axiom.Tests.Assertions.AsyncStreams.BeInAscendingOrderAsync;

public sealed class BeInAscendingOrderAsyncTests
{
    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenItemsAreSortedAscending()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var assertions = values.Should();
        var continuation = await assertions.BeInAscendingOrderAsync();

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenItemsContainEqualNeighbours()
    {
        var values = CreateAsyncSequence(1, 1, 2, 2);

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Passes_WhenStreamIsEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var ex = await Record.ExceptionAsync(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Null(ex);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WhenOrderIsViolated()
    {
        var values = CreateAsyncSequence(1, 3, 2, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Equal(
            "Expected values to be in ascending order, but found first out-of-order pair at index 2: previous 3 then current 2.",
            ex.Message);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WhenSubjectIsNull()
    {
        IAsyncEnumerable<int>? values = null;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync());

        Assert.Equal("Expected values to be in ascending order, but found <null>.", ex.Message);
    }

    [Fact]
    public async Task BeInAscendingOrderAsync_Throws_WithReason_WhenProvided()
    {
        var values = CreateAsyncSequence(1, 3, 2, 4);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().BeInAscendingOrderAsync("events should stay sorted"));

        Assert.Contains("because events should stay sorted", ex.Message, StringComparison.Ordinal);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
