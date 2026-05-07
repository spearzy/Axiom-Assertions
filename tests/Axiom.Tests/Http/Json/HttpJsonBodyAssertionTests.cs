using System.Net;
using System.Text.Json;
using Axiom.Http;

namespace Axiom.Tests.Http.Json;

public sealed class HttpJsonBodyAssertionTests
{
    [Fact]
    public void HaveJsonBodyEquivalentTo_Passes_WhenBodyIsEquivalent()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            """
            { "id": 1, "name": "Bob", "roles": ["admin", "author"] }
            """,
            "application/json");

        const string expected = """
            { "roles": ["admin", "author"], "name": "Bob", "id": 1.0 }
            """;

        var ex = Record.Exception(() => response.Should().HaveJsonBodyEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPath_AndScalarAssertions_Pass_WhenBodyMatches()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            """
            { "user": { "name": "Bob", "active": true, "score": 1e0, "deletedAt": null } }
            """,
            "application/json");

        var ex = Record.Exception(() =>
        {
            response.Should().HaveJsonPath("$.user.name");
            response.Should().HaveJsonStringAtPath("$.user.name", "Bob");
            response.Should().HaveJsonBooleanAtPath("$.user.active", true);
            response.Should().HaveJsonNumberAtPath("$.user.score", 1m);
            response.Should().HaveJsonNullAtPath("$.user.deletedAt");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void ContainerPathAssertions_Pass_WhenBodyMatches()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            """
            {
              "user": {
                "name": "Bob",
                "roles": ["admin", "author"]
              }
            }
            """,
            "application/json");

        var ex = Record.Exception(() =>
        {
            response.Should().HaveJsonObjectAtPath("$.user");
            response.Should().HaveJsonArrayAtPath("$.user.roles");
            response.Should().HaveJsonArrayLengthAtPath("$.user.roles", 2);
            response.Should().HaveJsonPropertyCountAtPath("$.user", 2);
        });

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonBodyEquivalentTo_Throws_WithJsonMismatchDetail()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 2 }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonBodyEquivalentTo("{ \"id\": 1 }"));

        Assert.Equal(
            "Expected response JSON body to be JSON equivalent to {\"id\":1}, but found JSON value mismatch at $.id: expected 1 but found 2.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPath_Throws_WhenResponseHasNoContent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonPath("$.id"));

        Assert.Equal("Expected response to have JSON path $.id, but found missing response body content.", ex.Message);
    }

    [Fact]
    public void HaveJsonObjectAtPath_Throws_WhenResolvedValueIsNotObject()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"user\": [] }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonObjectAtPath("$.user"));

        Assert.Equal(
            "Expected response JSON body to have JSON object at path $.user, but found JSON array at $.user; expected object.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayAtPath_Throws_WhenResolvedValueIsNotArray()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"roles\": {} }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonArrayAtPath("$.roles"));

        Assert.Equal(
            "Expected response JSON body to have JSON array at path $.roles, but found JSON object at $.roles; expected array.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_Throws_WhenLengthDiffers()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"roles\": [\"admin\"] }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonArrayLengthAtPath("$.roles", 2));

        Assert.Equal(
            "Expected response JSON body to have JSON array at path $.roles with length 2, but found JSON array length mismatch at $.roles: expected 2 but found 1.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPropertyCountAtPath_Throws_WhenPropertyCountDiffers()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"user\": { \"name\": \"Bob\" } }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonPropertyCountAtPath("$.user", 2));

        Assert.Equal(
            "Expected response JSON body to have JSON object at path $.user with property count 2, but found JSON object property count mismatch at $.user: expected 2 but found 1.",
            ex.Message);
    }

    [Fact]
    public void ContainerPathAssertions_Throw_WhenPathDoesNotExist()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"user\": {} }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonArrayLengthAtPath("$.user.roles", 1));

        Assert.Equal(
            "Expected response JSON body to have JSON array at path $.user.roles with length 1, but found missing JSON path $.user.roles.",
            ex.Message);
    }

    [Fact]
    public void ContainerPathAssertions_ThrowArgumentException_WhenPathSyntaxIsInvalid()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"user\": {} }", "application/json");

        var ex = Assert.Throws<ArgumentException>(() => response.Should().HaveJsonObjectAtPath("$.user["));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainerPathAssertions_Throw_WhenBodyJsonIsInvalid()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"user\": ", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonObjectAtPath("$.user"));

        Assert.Equal(
            "Expected response JSON body to have JSON object at path $.user, but found invalid JSON in response JSON body (line 0, byte 10).",
            ex.Message);
    }

    [Fact]
    public void ContainerPathAssertions_ThrowArgumentOutOfRangeException_WhenExpectedCountIsNegative()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"roles\": [] }", "application/json");

        var lengthEx = Assert.Throws<ArgumentOutOfRangeException>(
            () => response.Should().HaveJsonArrayLengthAtPath("$.roles", -1));
        var countEx = Assert.Throws<ArgumentOutOfRangeException>(
            () => response.Should().HaveJsonPropertyCountAtPath("$", -1));

        Assert.Equal("expectedLength", lengthEx.ParamName);
        Assert.Equal("expectedCount", countEx.ParamName);
    }

    [Fact]
    public void HaveJsonBodyEquivalentTo_Passes_ForJsonDocumentExpectedInput()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 1 }", "application/json");
        using var expected = JsonDocument.Parse("{ \"id\": 1.0 }");

        var ex = Record.Exception(() => response.Should().HaveJsonBodyEquivalentTo(expected));

        Assert.Null(ex);
    }
}
