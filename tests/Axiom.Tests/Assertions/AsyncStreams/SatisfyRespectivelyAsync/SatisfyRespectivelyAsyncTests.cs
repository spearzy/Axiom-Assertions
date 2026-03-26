namespace Axiom.Tests.Assertions.AsyncStreams.SatisfyRespectivelyAsync;

public sealed class SatisfyRespectivelyAsyncTests
{
    [Fact]
    public async Task SatisfyRespectivelyAsync_Passes_WhenItemCountAndAssertionsMatch()
    {
        var values = CreateAsyncSequence(
            new Order(10m),
            new Order(20m),
            new Order(30m));

        var assertions = values.Should();
        var continuation = await assertions.SatisfyRespectivelyAsync(
            first => first.Total.Should().Be(10m),
            second => second.Total.Should().Be(20m),
            third => third.Total.Should().Be(30m));

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_Throws_WhenStreamIsShorterThanAssertions()
    {
        var values = CreateAsyncSequence(1, 2);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0)));

        Assert.Equal(
            "Expected values to satisfy assertions respectively (same order and count), but found async stream had fewer items than assertions (expected 3, found 2).",
            ex.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_Throws_WhenStreamIsLongerThanAssertions()
    {
        var values = CreateAsyncSequence(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0)));

        Assert.Equal(
            "Expected values to satisfy assertions respectively (same order and count), but found async stream had more items than assertions (expected 2, found 3).",
            ex.Message);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_Throws_WhenAnItemAssertionFails()
    {
        var values = CreateAsyncSequence(10, -1, 30);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0),
                item => item.Should().BeGreaterThan(0)));

        Assert.Contains("Expected values to satisfy assertions respectively (failing index 1)", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected item to be greater than 0, but found -1.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_PreservesOrderSensitiveBehavior()
    {
        var values = CreateAsyncSequence(10, 20);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                first => first.Should().Be(20),
                second => second.Should().Be(10)));

        Assert.Contains("Expected values to satisfy assertions respectively (failing index 0)", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected first to be 20, but found 10.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_DoesNotThrow_WhenBothStreamAndAssertionsAreEmpty()
    {
        var values = CreateAsyncSequence<int>();

        var assertions = values.Should();
        var continuation = await assertions.SatisfyRespectivelyAsync(Array.Empty<Action<int>>());

        Assert.Same(assertions, continuation.And);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_ThrowsArgumentNullException_WhenAssertionsArrayIsNull()
    {
        var values = CreateAsyncSequence(1, 2, 3);
        Action<int>[]? assertionsForItems = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(assertionsForItems!));

        Assert.Equal("assertionsForItems", ex.ParamName);
    }

    [Fact]
    public async Task SatisfyRespectivelyAsync_ThrowsArgumentNullException_WhenAnAssertionItemIsNull()
    {
        var values = CreateAsyncSequence(1, 2);
        Action<int>? second = null;

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await values.Should().SatisfyRespectivelyAsync(
                item => item.Should().BeGreaterThan(0),
                second!));

        Assert.Equal("assertionsForItems", ex.ParamName);
        Assert.Contains("assertionsForItems[1]", ex.Message, StringComparison.Ordinal);
    }

    private static async IAsyncEnumerable<T> CreateAsyncSequence<T>(params T[] items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed record Order(decimal Total);
}
