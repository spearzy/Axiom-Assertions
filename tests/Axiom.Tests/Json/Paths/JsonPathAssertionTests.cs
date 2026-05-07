using System.Text.Json;
using Axiom.Json;

namespace Axiom.Tests.Json.Paths;

public sealed class JsonPathAssertionTests
{
    [Fact]
    public void HaveJsonPath_Passes_WhenPathExists()
    {
        const string actual = """
            { "user": { "roles": ["admin", "author"] } }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonPath("$.user.roles[1]"));

        Assert.Null(ex);
    }

    [Fact]
    public void NotHaveJsonPath_Passes_WhenPathDoesNotExist()
    {
        using var document = JsonDocument.Parse("""
            { "user": { "id": 7 } }
            """);

        var ex = Record.Exception(() => document.Should().NotHaveJsonPath("$.user.email"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPath_Throws_WhenPathDoesNotExist()
    {
        const string actual = """
            { "user": { "id": 7 } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPath("$.user.email"));

        Assert.Equal("Expected actual to have JSON path $.user.email, but found missing JSON path $.user.email.", ex.Message);
    }

    [Fact]
    public void HaveJsonPath_Throws_WhenTraversalStopsAtWrongKind()
    {
        const string actual = """
            { "user": [] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPath("$.user.email"));

        Assert.Equal(
            "Expected actual to have JSON path $.user.email, but found could not resolve JSON path $.user.email: expected object at $.user but found array.",
            ex.Message);
    }

    [Fact]
    public void NotHaveJsonPath_Throws_WhenPathExists()
    {
        const string actual = """
            { "user": { "id": 7 } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().NotHaveJsonPath("$.user.id"));

        Assert.Equal(
            "Expected actual to not have JSON path $.user.id, but found present path $.user.id with JSON number 7.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonStringAtPath_Passes_WhenValueMatches()
    {
        const string actual = """
            { "user": { "name": "Bob" } }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonStringAtPath("$.user.name", "Bob"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonNumberAtPath_Passes_WhenNormalizedNumericValueMatches()
    {
        const string actual = """
            { "score": 1e0 }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonNumberAtPath("$.score", 1.0m));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonBooleanAtPath_Passes_WhenValueMatches()
    {
        using var document = JsonDocument.Parse("""
            { "active": true }
            """);

        var ex = Record.Exception(() => document.RootElement.Should().HaveJsonBooleanAtPath("$.active", true));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonNullAtPath_Passes_WhenValueIsNull()
    {
        const string actual = """
            { "deletedAt": null }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonNullAtPath("$.deletedAt"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonStringAtPath_Throws_WhenValueDiffers()
    {
        const string actual = """
            { "user": { "name": "Grace" } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonStringAtPath("$.user.name", "Bob"));

        Assert.Equal(
            "Expected actual to have JSON string at path $.user.name equal to \"Bob\", but found JSON string \"Grace\" at $.user.name.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonNumberAtPath_Throws_WhenValueKindDiffers()
    {
        const string actual = """
            { "score": "1" }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonNumberAtPath("$.score", 1m));

        Assert.Equal(
            "Expected actual to have JSON number at path $.score equal to 1, but found JSON string \"1\" at $.score; expected number.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonNullAtPath_Throws_WhenValueIsNotNull()
    {
        const string actual = """
            { "deletedAt": false }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonNullAtPath("$.deletedAt"));

        Assert.Equal(
            "Expected actual to have JSON null at path $.deletedAt, but found JSON boolean false at $.deletedAt; expected null.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPath_Throws_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"user\": ";

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPath("$.user.id"));

        Assert.Equal(
            "Expected actual to have JSON path $.user.id, but found invalid subject JSON (line 0, byte 10).",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPath_ThrowsArgumentException_WhenPathSyntaxIsInvalid()
    {
        const string actual = """
            { "user": { "id": 7 } }
            """;

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().HaveJsonPath("$.user["));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }
}
