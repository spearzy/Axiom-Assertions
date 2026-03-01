using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Actions.Batch;

public sealed class ActionBatchRoutingTests
{
    [Fact]
    public void Throw_OutsideBatch_ThrowsImmediately()
    {
        Action action = static () => { };

        Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());
    }

    [Fact]
    public void Throw_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Action action = static () => { };

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            action.Should().Throw<InvalidOperationException>();
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromActionAssertions()
    {
        Action noThrow = static () => { };
        Action wrongThrow = static () => throw new ArgumentException("bad");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("actions");
            noThrow.Should().Throw<InvalidOperationException>();
            wrongThrow.Should().Throw<InvalidOperationException>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'actions' failed with 2 assertion failure(s):", message);
        Assert.Contains($"1) Expected noThrow to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Assert.Contains($"2) Expected wrongThrow to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.", message);
    }
}
