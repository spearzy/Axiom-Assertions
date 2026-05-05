using System.Text.Json;
using Axiom.Json;

namespace Axiom.Tests.Json.Paths;

public sealed class JsonContainerPathAssertionTests
{
    [Fact]
    public void ContainerPathAssertions_Pass_ForSupportedSubjectInputTypes()
    {
        const string actual = """
            {
              "customer": {
                "name": "Ada",
                "roles": ["admin", "author"]
              }
            }
            """;
        using var document = JsonDocument.Parse(actual);

        var stringEx = Record.Exception(() => actual.Should().HaveJsonObjectAtPath("$.customer"));
        var documentEx = Record.Exception(() => document.Should().HaveJsonArrayAtPath("$.customer.roles"));
        var elementLengthEx = Record.Exception(() => document.RootElement.Should().HaveJsonArrayLengthAtPath("$.customer.roles", 2));
        var elementCountEx = Record.Exception(() => document.RootElement.Should().HaveJsonPropertyCountAtPath("$.customer", 2));

        Assert.Null(stringEx);
        Assert.Null(documentEx);
        Assert.Null(elementLengthEx);
        Assert.Null(elementCountEx);
    }

    [Fact]
    public void HaveJsonObjectAtPath_Throws_WhenResolvedValueIsNotObject()
    {
        const string actual = """
            { "roles": ["admin"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonObjectAtPath("$.roles"));

        Assert.Equal(
            "Expected actual to have JSON object at path $.roles, but found JSON array at $.roles (expected Object).",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayAtPath_Throws_WhenResolvedValueIsNotArray()
    {
        const string actual = """
            { "customer": { "id": 7 } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonArrayAtPath("$.customer"));

        Assert.Equal(
            "Expected actual to have JSON array at path $.customer, but found JSON object at $.customer (expected Array).",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_Passes_WhenLengthMatches()
    {
        const string actual = """
            { "roles": ["admin", "author"] }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonArrayLengthAtPath("$.roles", 2));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_Throws_WhenLengthDiffers()
    {
        const string actual = """
            { "roles": ["admin", "author"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonArrayLengthAtPath("$.roles", 3));

        Assert.Equal(
            "Expected actual to have JSON array at path $.roles with length 3, but found JSON array length 2 at $.roles.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_Throws_WhenResolvedValueIsNotArray()
    {
        const string actual = """
            { "roles": "admin" }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonArrayLengthAtPath("$.roles", 1));

        Assert.Equal(
            "Expected actual to have JSON array at path $.roles with length 1, but found JSON string \"admin\" at $.roles (expected Array).",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPropertyCountAtPath_Passes_WhenPropertyCountMatches()
    {
        const string actual = """
            { "customer": { "id": 7, "name": "Ada" } }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonPropertyCountAtPath("$.customer", 2));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPropertyCountAtPath_Throws_WhenPropertyCountDiffers()
    {
        const string actual = """
            { "customer": { "id": 7, "name": "Ada" } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPropertyCountAtPath("$.customer", 1));

        Assert.Equal(
            "Expected actual to have JSON object at path $.customer with property count 1, but found JSON object property count 2 at $.customer.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPropertyCountAtPath_Throws_WhenResolvedValueIsNotObject()
    {
        const string actual = """
            { "customer": ["Ada"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPropertyCountAtPath("$.customer", 1));

        Assert.Equal(
            "Expected actual to have JSON object at path $.customer with property count 1, but found JSON array at $.customer (expected Object).",
            ex.Message);
    }

    [Fact]
    public void ContainerPathAssertions_Throw_WhenPathDoesNotExist()
    {
        const string actual = """
            { "customer": { "id": 7 } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonArrayLengthAtPath("$.customer.roles", 1));

        Assert.Equal(
            "Expected actual to have JSON array at path $.customer.roles with length 1, but found missing path $.customer.roles.",
            ex.Message);
    }

    [Fact]
    public void ContainerPathAssertions_ThrowArgumentException_WhenPathSyntaxIsInvalid()
    {
        const string actual = """
            { "customer": { "id": 7 } }
            """;

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().HaveJsonObjectAtPath("$.customer["));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContainerPathAssertions_Throw_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"customer\": ";

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonObjectAtPath("$.customer"));

        Assert.Equal(
            "Expected actual to have JSON object at path $.customer, but found invalid JSON at line 0, byte 14.",
            ex.Message);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_ThrowsArgumentOutOfRangeException_WhenExpectedLengthIsNegative()
    {
        const string actual = """
            { "roles": [] }
            """;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => actual.Should().HaveJsonArrayLengthAtPath("$.roles", -1));

        Assert.Equal("expectedLength", ex.ParamName);
    }

    [Fact]
    public void HaveJsonPropertyCountAtPath_ThrowsArgumentOutOfRangeException_WhenExpectedCountIsNegative()
    {
        const string actual = """
            { "customer": {} }
            """;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => actual.Should().HaveJsonPropertyCountAtPath("$.customer", -1));

        Assert.Equal("expectedCount", ex.ParamName);
    }

    [Fact]
    public void HaveJsonArrayLengthAtPath_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string actual = """
            { "roles": ["admin"] }
            """;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().HaveJsonArrayLengthAtPath("$.roles", 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }
}
