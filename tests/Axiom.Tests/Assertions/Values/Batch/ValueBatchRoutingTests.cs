using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.Batch;

public sealed class ValueBatchRoutingTests
{
    [Fact]
    public void Be_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));
    }

    [Fact]
    public void Be_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        var ex = Xunit.Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            value.Should().Be(7);
        });

        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromValueAssertions()
    {
        var value = 42;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("values");
            value.Should().Be(7);
            value.Should().NotBe(42);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Xunit.Assert.Contains("Batch 'values' failed with 2 assertion failure(s):", message);
        Xunit.Assert.Contains("1) Expected value to be 7, but found 42.", message);
        Xunit.Assert.Contains("2) Expected value to not be 42, but found 42.", message);
    }
}
