namespace Axiom.Tests.Assertions.Actions.Batch;

public sealed class ActionBatchRoutingTests
{
    [Fact]
    public void Throw_OutsideBatch_ThrowsImmediately()
    {
        Action action = static () => { };

        Xunit.Assert.Throws<InvalidOperationException>(() =>
            action.Should().Throw<InvalidOperationException>());
    }

    [Fact]
    public void Throw_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        Action action = static () => { };

        var ex = Xunit.Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            action.Should().Throw<InvalidOperationException>();
        });

        Xunit.Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromActionAssertions()
    {
        Action noThrow = static () => { };
        Action wrongThrow = static () => throw new ArgumentException("bad");

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("actions");
            noThrow.Should().Throw<InvalidOperationException>();
            wrongThrow.Should().Throw<InvalidOperationException>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Xunit.Assert.Contains("Batch 'actions' failed with 2 assertion failure(s):", message);
        Xunit.Assert.Contains($"1) Expected noThrow to throw {typeof(InvalidOperationException)}, but found <no exception>.", message);
        Xunit.Assert.Contains($"2) Expected wrongThrow to throw {typeof(InvalidOperationException)}, but found {typeof(ArgumentException)}.", message);
    }
}
