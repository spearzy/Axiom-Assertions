using System.Text.Json;
using Axiom.Json;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class JsonShouldEntryPointTests
{
    [Fact]
    public void JsonDocumentShould_ReturnsJsonAssertions()
    {
        using var document = JsonDocument.Parse("""
            { "id": 1 }
            """);

        var assertions = document.Should();

        Assert.IsType<JsonAssertions>(assertions);
    }

    [Fact]
    public void JsonElementShould_ReturnsJsonAssertions()
    {
        using var document = JsonDocument.Parse("""
            { "id": 1 }
            """);

        var assertions = document.RootElement.Should();

        Assert.IsType<JsonAssertions>(assertions);
    }
}
