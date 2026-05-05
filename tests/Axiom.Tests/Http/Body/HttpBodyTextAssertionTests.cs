using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.Body;

public sealed class HttpBodyTextAssertionTests
{
    [Fact]
    public void HaveBodyText_Passes_WhenBodyMatchesExactly()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "created");

        var ex = Record.Exception(() => response.Should().HaveBodyText("created"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveBodyText_Throws_WhenBodyDiffers()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "created");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveBodyText("queued"));

        Assert.Contains("Expected response body to be \"queued\"", ex.Message, StringComparison.Ordinal);
        Assert.Contains("but found", ex.Message, StringComparison.Ordinal);
        Assert.Contains("\"created\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainBodyText_Passes_WhenBodyContainsExpectedSubstring()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "order created");

        var ex = Record.Exception(() => response.Should().ContainBodyText("created"));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainBodyText_Throws_WhenBodyDoesNotContainExpectedSubstring()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "order queued");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().ContainBodyText("created"));

        Assert.Contains("Expected response body to contain \"created\"", ex.Message, StringComparison.Ordinal);
        Assert.Contains("but found", ex.Message, StringComparison.Ordinal);
        Assert.Contains("\"order queued\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BodyTextAssertions_ThrowClearly_WhenResponseHasNoContent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var exactEx = Assert.Throws<InvalidOperationException>(() => response.Should().HaveBodyText("created"));
        var containEx = Assert.Throws<InvalidOperationException>(() => response.Should().ContainBodyText("created"));

        Assert.Equal("Expected response to have body text \"created\", but found missing response body content.", exactEx.Message);
        Assert.Equal("Expected response to contain body text \"created\", but found missing response body content.", containEx.Message);
    }
}
