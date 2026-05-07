using System.Text.Json;
using Axiom.Json;

namespace Axiom.Tests.Json.Equivalency;

public sealed class JsonEquivalencyTests
{
    [Fact]
    public void BeJsonEquivalentTo_Passes_WhenObjectsAreEquivalentDespitePropertyOrder()
    {
        const string actual = """
            { "name": "Bob", "id": 1, "roles": ["admin", "author"] }
            """;
        const string expected = """
            { "roles": ["admin", "author"], "id": 1.0, "name": "Bob" }
            """;

        var ex = Record.Exception(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeJsonEquivalentTo_Passes_ForJsonDocumentAndJsonElementInputs()
    {
        using var actual = JsonDocument.Parse("""
            { "customer": { "id": 7, "active": true } }
            """);
        using var expected = JsonDocument.Parse("""
            { "customer": { "active": true, "id": 7.0 } }
            """);

        var ex = Record.Exception(() => actual.RootElement.Should().BeJsonEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenArrayOrderDiffers()
    {
        const string actual = """
            { "ids": [2, 1] }
            """;
        const string expected = """
            { "ids": [1, 2] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"ids\":[1,2]}, but found JSON array item mismatch at $.ids[0]: expected 1 but found 2.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenPropertyIsMissing()
    {
        const string actual = """
            { "id": 1 }
            """;
        const string expected = """
            { "id": 1, "name": "Bob" }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"id\":1,\"name\":\"Bob\"}, but found missing property $.name.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenPropertyIsExtra()
    {
        const string actual = """
            { "id": 1, "name": "Bob" }
            """;
        const string expected = """
            { "id": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"id\":1}, but found extra property $.name.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenScalarValueDiffers()
    {
        const string actual = """
            { "id": 2 }
            """;
        const string expected = """
            { "id": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"id\":1}, but found JSON value mismatch at $.id: expected 1 but found 2.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenValueKindDiffers()
    {
        const string actual = """
            { "id": "1" }
            """;
        const string expected = """
            { "id": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"id\":1}, but found JSON value kind mismatch at $.id: expected number but found string.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenArrayLengthDiffers()
    {
        const string actual = """
            { "ids": [1, 2, 3] }
            """;
        const string expected = """
            { "ids": [1, 2] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"ids\":[1,2]}, but found JSON array length mismatch at $.ids: expected 2 but found 3.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_UsesNormalizedNumericComparison()
    {
        const string actual = """
            { "score": 1e0, "ratio": 0.10 }
            """;
        const string expected = """
            { "ratio": 0.1, "score": 1.0 }
            """;

        var ex = Record.Exception(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeJsonEquivalentTo_Passes_WhenDocumentsDiffer()
    {
        const string actual = """
            { "id": 1 }
            """;
        const string unexpected = """
            { "id": 2 }
            """;

        var ex = Record.Exception(() => actual.Should().NotBeJsonEquivalentTo(unexpected));

        Assert.Null(ex);
    }

    [Fact]
    public void NotBeJsonEquivalentTo_Throws_WhenDocumentsAreEquivalent()
    {
        const string actual = """
            { "id": 1, "name": "Bob" }
            """;
        const string unexpected = """
            { "name": "Bob", "id": 1.0 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().NotBeJsonEquivalentTo(unexpected));

        Assert.Equal(
            "Expected actual to not be JSON equivalent to {\"name\":\"Bob\",\"id\":1.0}, but found equivalent JSON {\"id\":1,\"name\":\"Bob\"}.",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"id\": ";
        const string expected = """
            { "id": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal(
            "Expected actual to be JSON equivalent to {\"id\":1}, but found invalid subject JSON (line 0, byte 8).",
            ex.Message);
    }

    [Fact]
    public void NotBeJsonEquivalentTo_Throws_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"id\": ";
        const string unexpected = """
            { "id": 2 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().NotBeJsonEquivalentTo(unexpected));

        Assert.Equal(
            "Expected actual to not be JSON equivalent to {\"id\":2}, but found invalid subject JSON (line 0, byte 8).",
            ex.Message);
    }

    [Fact]
    public void BeJsonEquivalentTo_ThrowsArgumentException_WhenExpectedJsonIsInvalid()
    {
        const string actual = """
            { "id": 1 }
            """;
        const string expected = "{ \"id\": ";

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal("expectedJson", ex.ParamName);
        Assert.Contains("expectedJson must be valid expected JSON", ex.Message, StringComparison.Ordinal);
        Assert.Contains("line 0, byte 8", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeJsonEquivalentTo_Throws_WhenSubjectIsNull()
    {
        string? actual = null;
        const string expected = """
            { "id": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeJsonEquivalentTo(expected));

        Assert.Equal("Expected actual to be JSON equivalent to {\"id\":1}, but found <null>.", ex.Message);
    }
}
