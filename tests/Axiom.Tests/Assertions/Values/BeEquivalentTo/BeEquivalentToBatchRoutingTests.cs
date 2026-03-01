using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToBatchRoutingTests
{
    [Fact]
    public void GivenNoBatch_WhenNotEquivalent_ThenThrowsImmediately()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));
    }

    [Fact]
    public void GivenBatch_WhenNotEquivalent_ThenAssertionCallSiteDoesNotThrow()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        var batch = new Axiom.Core.Batch();
        try
        {
            var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));
            Assert.Null(ex);
        }
        finally
        {
            _ = Record.Exception(batch.Dispose);
        }
    }

    [Fact]
    public void GivenBatch_WhenNotEquivalent_ThenDisposeThrowsCombinedFailure()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("equivalency");
            actual.Should().BeEquivalentTo(expected);
        });

        Assert.Contains("Batch 'equivalency' failed with 1 assertion failure(s):", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Expected actual to be equivalent to", ex.Message, StringComparison.Ordinal);
    }
}
