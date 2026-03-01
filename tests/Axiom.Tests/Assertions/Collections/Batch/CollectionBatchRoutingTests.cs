using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.Batch;

public sealed class CollectionBatchRoutingTests
{
    [Fact]
    public void Contain_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().Contain(9));
    }

    [Fact]
    public void Contain_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().Contain(9));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromCollectionAssertions()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collections");
            values.Should().Contain(9);
            values.Should().HaveCount(2);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collections' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to contain 9, but found System.Int32[].", message);
        Assert.Contains("2) Expected values to have count 2, but found 3.", message);
    }
}
