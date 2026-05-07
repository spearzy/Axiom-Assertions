using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.Batch;

public sealed class HttpBatchRoutingTests
{
    [Fact]
    public void HaveStatusCode_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.NotFound);
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveStatusCode(HttpStatusCode.OK));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveJsonBodyEquivalentTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 2 }", "application/json");
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveJsonBodyEquivalentTo("{ \"id\": 1 }"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveBodyText_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "created");
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveBodyText("queued"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"roles\": [\"admin\"] }", "application/json");
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveJsonArrayLengthAtPath("$.roles", 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveJsonProperties_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": \"evt_123\" }", "application/json");
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveJsonProperties("id", "status"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
