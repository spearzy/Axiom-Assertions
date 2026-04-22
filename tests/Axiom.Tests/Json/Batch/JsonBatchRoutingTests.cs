using Axiom.Json;

namespace Axiom.Tests.Json.Batch;

public sealed class JsonBatchRoutingTests
{
    [Fact]
    public void BeJsonEquivalentTo_OutsideBatch_ThrowsImmediately()
    {
        const string actual = """
            { "id": 2 }
            """;
        const string expected = """
            { "id": 1 }
            """;

        Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));
    }

    [Fact]
    public void BeJsonEquivalentTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string actual = """
            { "id": 2 }
            """;
        const string expected = """
            { "id": 1 }
            """;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveJsonPath_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string actual = """
            { "user": { "id": 7 } }
            """;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().HaveJsonPath("$.user.email"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
