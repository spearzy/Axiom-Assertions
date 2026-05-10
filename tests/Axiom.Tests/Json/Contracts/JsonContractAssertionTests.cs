using System.Text.Json;
using Axiom.Json;

namespace Axiom.Tests.Json.Contracts;

public sealed class JsonContractAssertionTests
{
    [Fact]
    public void BeValidJson_Passes_WhenRawStringIsValidJson()
    {
        const string actual = """
            { "id": 7 }
            """;

        var ex = Record.Exception(() => actual.Should().BeValidJson());

        Assert.Null(ex);
    }

    [Fact]
    public void BeValidJson_Throws_WhenRawStringIsInvalidJson()
    {
        const string actual = "{ \"id\": ";

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeValidJson());

        Assert.Equal("Expected actual to be valid JSON, but found invalid subject JSON (line 0, byte 8).", ex.Message);
    }

    [Fact]
    public void HaveJsonProperties_Passes_WhenRootObjectContainsAllProperties()
    {
        const string actual = """
            { "status": "queued", "id": "ord_123", "extra": true }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonProperties("id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonProperties_Passes_ForSupportedParsedSubjectInputs()
    {
        const string rawJson = """
            { "status": "queued", "id": "ord_123" }
            """;
        using var document = JsonDocument.Parse(rawJson);
        JsonElement? nullableElement = document.RootElement;

        var documentEx = Record.Exception(() => document.Should().HaveJsonProperties("id", "status"));
        var elementEx = Record.Exception(() => document.RootElement.Should().HaveOnlyJsonProperties("status", "id"));
        var nullableElementEx = Record.Exception(() => nullableElement.Should().HaveJsonProperties("id"));

        Assert.Null(documentEx);
        Assert.Null(elementEx);
        Assert.Null(nullableElementEx);
    }

    [Fact]
    public void HaveJsonProperties_Throws_WhenRootObjectIsMissingProperties()
    {
        const string actual = """
            { "id": "ord_123" }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonProperties("id", "status"));

        Assert.Equal(
            "Expected actual to have JSON properties [\"id\", \"status\"] at path $, but found JSON object at $ missing properties [\"status\"].",
            ex.Message);
    }

    [Fact]
    public void HaveJsonProperties_CollectionOverload_IncludesBecauseInFailure()
    {
        const string actual = """
            { "id": "ord_123" }
            """;
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonProperties(requiredProperties, "because API contracts should stay stable"));

        Assert.Contains("because API contracts should stay stable", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveJsonPropertiesAtPath_Passes_WhenNestedObjectContainsAllProperties()
    {
        const string actual = """
            {
              "customer": {
                "id": "cus_123",
                "name": "Bob",
                "tier": "gold"
              }
            }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPropertiesAtPath_Throws_WhenNestedObjectIsMissingProperties()
    {
        const string actual = """
            { "customer": { "id": "cus_123" } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Equal(
            "Expected actual to have JSON properties [\"id\", \"name\"] at path $.customer, but found JSON object at $.customer missing properties [\"name\"].",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPropertiesAtPath_CollectionOverload_IncludesBecauseInFailure()
    {
        const string rawJson = """
            { "customer": { "id": "cus_123" } }
            """;
        using var document = JsonDocument.Parse(rawJson);
        IReadOnlyCollection<string> requiredProperties = ["id", "name"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            document.Should().HaveJsonPropertiesAtPath(
                "$.customer",
                requiredProperties,
                "because customer payloads should include display names"));

        Assert.Contains("because customer payloads should include display names", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveOnlyJsonProperties_Passes_WhenRootObjectHasExactlyThoseProperties()
    {
        const string actual = """
            { "status": "queued", "id": "ord_123" }
            """;

        var ex = Record.Exception(() => actual.Should().HaveOnlyJsonProperties("id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveOnlyJsonProperties_Throws_WhenRootObjectHasMissingAndExtraProperties()
    {
        const string actual = """
            { "id": "ord_123", "debug": true }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveOnlyJsonProperties("id", "status"));

        Assert.Equal(
            "Expected actual to have only JSON properties [\"id\", \"status\"] at path $, but found JSON object properties mismatch at $: missing [\"status\"]; extra [\"debug\"].",
            ex.Message);
    }

    [Fact]
    public void HaveOnlyJsonProperties_CollectionOverload_IncludesBecauseInFailure()
    {
        const string actual = """
            { "id": "ord_123", "debug": true }
            """;
        IReadOnlyCollection<string> expectedProperties = ["id", "status"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveOnlyJsonProperties(expectedProperties, "because response shape should not drift"));

        Assert.Contains("because response shape should not drift", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveOnlyJsonPropertiesAtPath_Passes_WhenNestedObjectHasExactlyThoseProperties()
    {
        const string actual = """
            { "customer": { "name": "Bob", "id": "cus_123" } }
            """;

        var ex = Record.Exception(() => actual.Should().HaveOnlyJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveOnlyJsonPropertiesAtPath_Throws_WhenNestedObjectHasExtraProperties()
    {
        const string actual = """
            { "customer": { "id": "cus_123", "name": "Bob", "debug": true } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveOnlyJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Equal(
            "Expected actual to have only JSON properties [\"id\", \"name\"] at path $.customer, but found JSON object properties mismatch at $.customer: missing []; extra [\"debug\"].",
            ex.Message);
    }

    [Fact]
    public void HaveOnlyJsonPropertiesAtPath_CollectionOverload_IncludesBecauseInFailure()
    {
        const string rawJson = """
            { "customer": { "id": "cus_123", "name": "Bob", "debug": true } }
            """;
        using var document = JsonDocument.Parse(rawJson);
        IReadOnlyCollection<string> expectedProperties = ["id", "name"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            document.Should().HaveOnlyJsonPropertiesAtPath(
                "$.customer",
                expectedProperties,
                "because customer contracts should be exact"));

        Assert.Contains("because customer contracts should be exact", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Passes_WhenStringValueIsInAllowedSet()
    {
        const string actual = """
            { "intent": "create_order" }
            """;

        var ex = Record.Exception(() => actual.Should().HaveAllowedValueAtPath("$.intent", "create_order", "cancel_order"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Passes_WithAllowedValueCollection()
    {
        const string actual = """
            { "status": "queued" }
            """;
        string[] allowedStatuses = ["queued", "processing", "complete"];

        var ex = Record.Exception(() => actual.Should().HaveAllowedValueAtPath("$.status", allowedStatuses));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Passes_ForSupportedParsedSubjectInputs()
    {
        const string rawJson = """
            { "status": "queued" }
            """;
        using var document = JsonDocument.Parse(rawJson);
        JsonElement? nullableElement = document.RootElement;

        var documentEx = Record.Exception(() => document.Should().HaveAllowedValueAtPath("$.status", "queued", "complete"));
        var elementEx = Record.Exception(() => document.RootElement.Should().HaveAllowedValueAtPath("$.status", "queued", "complete"));
        var nullableElementEx = Record.Exception(() => nullableElement.Should().HaveAllowedValueAtPath("$.status", "queued", "complete"));

        Assert.Null(documentEx);
        Assert.Null(elementEx);
        Assert.Null(nullableElementEx);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ParamsOverload_Throws_WhenStringValueIsNotAllowed()
    {
        const string actual = """
            { "status": "failed" }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveAllowedValueAtPath("$.status", "queued", "complete"));

        Assert.Equal(
            "Expected actual to have JSON string at path $.status equal to one of [\"queued\", \"complete\"], but found JSON string \"failed\" at $.status; allowed values [\"queued\", \"complete\"].",
            ex.Message);
    }

    [Fact]
    public void HaveJsonProperties_CollectionOverload_PropagatesCallerMetadataToFailureStrategy()
    {
        var strategy = new CapturingFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);
        const string callerFilePath = "/tests/json-contracts.cs";
        const int callerLineNumber = 412;
        const string actual = """
            { "id": "ord_123" }
            """;
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<CapturedFailureException>(() =>
            actual.Should().HaveJsonProperties(
                requiredProperties,
                "because API contracts should stay stable",
                callerFilePath,
                callerLineNumber));

        Assert.Equal(callerFilePath, ex.CallerFilePath);
        Assert.Equal(callerLineNumber, ex.CallerLineNumber);
        Assert.Contains(ex, strategy.Failures);
    }

    [Fact]
    public void ObjectContractAssertions_Throw_WhenResolvedValueIsWrongKind()
    {
        const string actual = """
            { "customer": ["Bob"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonPropertiesAtPath("$.customer", "id"));

        Assert.Equal(
            "Expected actual to have JSON properties [\"id\"] at path $.customer, but found JSON array at $.customer; expected object.",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Throws_WhenResolvedValueIsWrongKind()
    {
        const string actual = """
            { "status": 1 }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveAllowedValueAtPath("$.status", "queued"));

        Assert.Equal(
            "Expected actual to have JSON string at path $.status equal to one of [\"queued\"], but found JSON number 1 at $.status; expected string.",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenPathDoesNotExist()
    {
        const string actual = """
            { "customer": { "id": "cus_123" } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveOnlyJsonPropertiesAtPath("$.customer.profile", "name"));

        Assert.Equal(
            "Expected actual to have only JSON properties [\"name\"] at path $.customer.profile, but found missing JSON path $.customer.profile.",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_ThrowArgumentException_WhenPathSyntaxIsInvalid()
    {
        const string actual = """
            { "customer": { "id": "cus_123" } }
            """;

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().HaveJsonPropertiesAtPath("$.customer[", "id"));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"customer\": ";

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().HaveJsonProperties("id"));

        Assert.Equal(
            "Expected actual to have JSON properties [\"id\"] at path $, but found invalid subject JSON (line 0, byte 14).",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentException_WhenAllowedValuesAreEmpty()
    {
        const string actual = """
            { "status": "queued" }
            """;
        string[] allowedValues = [];

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().HaveAllowedValueAtPath("$.status", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentNullException_WhenAllowedValuesAreNull()
    {
        const string actual = """
            { "status": "queued" }
            """;
        string[]? allowedValues = null;

        var ex = Assert.Throws<ArgumentNullException>(() => actual.Should().HaveAllowedValueAtPath("$.status", allowedValues!));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentException_WhenAllowedValuesContainNull()
    {
        const string actual = """
            { "status": "queued" }
            """;
        string[] allowedValues = ["queued", null!];

        var ex = Assert.Throws<ArgumentException>(() => actual.Should().HaveAllowedValueAtPath("$.status", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveJsonObjectItemsWithPropertiesAtPath_Passes_WhenEveryArrayItemContainsProperties()
    {
        const string actual = """
            {
              "items": [
                { "id": "ord_1", "status": "queued", "extra": true },
                { "id": "ord_2", "status": "complete" }
              ]
            }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void ArrayContractAssertions_Pass_ForSupportedParsedSubjectInputs()
    {
        const string rawJson = """
            { "items": [{ "id": "ord_1", "status": "queued" }], "statuses": ["queued"] }
            """;
        using var document = JsonDocument.Parse(rawJson);
        JsonElement? nullableElement = document.RootElement;

        var documentEx = Record.Exception(() => document.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id"));
        var elementEx = Record.Exception(() => document.RootElement.Should().HaveAllowedValuesAtPath("$.statuses", "queued"));
        var nullableElementEx = Record.Exception(() => nullableElement.Should().HaveJsonObjectItemsWithOnlyPropertiesAtPath("$.items", "id", "status"));

        Assert.Null(documentEx);
        Assert.Null(elementEx);
        Assert.Null(nullableElementEx);
    }

    [Fact]
    public void HaveJsonObjectItemsWithPropertiesAtPath_Throws_WhenAnItemIsMissingProperties()
    {
        const string actual = """
            {
              "items": [
                { "id": "ord_1", "status": "queued" },
                { "id": "ord_2" }
              ]
            }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id", "status"));

        Assert.Equal(
            "Expected actual to have JSON object items with properties [\"id\", \"status\"] at path $.items, but found JSON object at $.items[1] missing properties [\"status\"].",
            ex.Message);
    }

    [Fact]
    public void HaveJsonObjectItemsWithOnlyPropertiesAtPath_Passes_WhenEveryArrayItemHasExactlyThoseProperties()
    {
        const string actual = """
            {
              "items": [
                { "status": "queued", "id": "ord_1" },
                { "id": "ord_2", "status": "complete" }
              ]
            }
            """;

        var ex = Record.Exception(() => actual.Should().HaveJsonObjectItemsWithOnlyPropertiesAtPath("$.items", "id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonObjectItemsWithOnlyPropertiesAtPath_Throws_WhenAnItemHasMissingAndExtraProperties()
    {
        const string actual = """
            {
              "items": [
                { "id": "ord_1", "status": "queued" },
                { "id": "ord_2", "debug": true }
              ]
            }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithOnlyPropertiesAtPath("$.items", "id", "status"));

        Assert.Equal(
            "Expected actual to have JSON object items with only properties [\"id\", \"status\"] at path $.items, but found JSON object properties mismatch at $.items[1]: missing [\"status\"]; extra [\"debug\"].",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_Passes_WhenEveryArrayStringIsAllowed()
    {
        const string actual = """
            { "statuses": ["queued", "processing", "complete"] }
            """;

        var ex = Record.Exception(() => actual.Should().HaveAllowedValuesAtPath("$.statuses", "queued", "processing", "complete"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_Passes_WithAllowedValueCollection()
    {
        const string actual = """
            { "statuses": ["queued", "complete"] }
            """;
        IReadOnlyCollection<string> allowedStatuses = ["queued", "processing", "complete"];

        var ex = Record.Exception(() => actual.Should().HaveAllowedValuesAtPath("$.statuses", allowedStatuses));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_Throws_WhenAnArrayStringIsNotAllowed()
    {
        const string actual = """
            { "statuses": ["queued", "failed"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveAllowedValuesAtPath("$.statuses", "queued", "complete"));

        Assert.Equal(
            "Expected actual to have JSON string values at path $.statuses equal to one of [\"queued\", \"complete\"], but found JSON string \"failed\" at $.statuses[1]; allowed values [\"queued\", \"complete\"].",
            ex.Message);
    }

    [Fact]
    public void ArrayObjectContractAssertions_Throw_WhenArrayItemIsWrongKind()
    {
        const string actual = """
            { "items": [{ "id": "ord_1" }, "oops"] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id"));

        Assert.Equal(
            "Expected actual to have JSON object items with properties [\"id\"] at path $.items, but found JSON string \"oops\" at $.items[1]; expected object.",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_Throws_WhenArrayItemIsWrongKind()
    {
        const string actual = """
            { "statuses": ["queued", 1] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveAllowedValuesAtPath("$.statuses", "queued"));

        Assert.Equal(
            "Expected actual to have JSON string values at path $.statuses equal to one of [\"queued\"], but found JSON number 1 at $.statuses[1]; expected string.",
            ex.Message);
    }

    [Fact]
    public void ArrayContractAssertions_Throw_WhenResolvedValueIsNotArray()
    {
        const string actual = """
            { "items": { "id": "ord_1" } }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id"));

        Assert.Equal(
            "Expected actual to have JSON object items with properties [\"id\"] at path $.items, but found JSON object at $.items; expected array.",
            ex.Message);
    }

    [Fact]
    public void ArrayContractAssertions_Throw_WhenPathDoesNotExist()
    {
        const string actual = """
            { "data": [] }
            """;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithOnlyPropertiesAtPath("$.items", "id"));

        Assert.Equal(
            "Expected actual to have JSON object items with only properties [\"id\"] at path $.items, but found missing JSON path $.items.",
            ex.Message);
    }

    [Fact]
    public void ArrayContractAssertions_ThrowArgumentException_WhenPathSyntaxIsInvalid()
    {
        const string actual = """
            { "items": [] }
            """;

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items[", "id"));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ArrayContractAssertions_Throw_WhenSubjectJsonIsInvalid()
    {
        const string actual = "{ \"items\": ";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id"));

        Assert.Equal(
            "Expected actual to have JSON object items with properties [\"id\"] at path $.items, but found invalid subject JSON (line 0, byte 11).",
            ex.Message);
    }

    [Fact]
    public void HaveJsonObjectItemsWithPropertiesAtPath_ThrowsArgumentNullException_WhenPropertyNamesAreNull()
    {
        const string actual = """
            { "items": [] }
            """;
        IReadOnlyCollection<string>? propertyNames = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", propertyNames!));

        Assert.Equal("propertyNames", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_ThrowsArgumentException_WhenAllowedValuesAreEmpty()
    {
        const string actual = """
            { "statuses": ["queued"] }
            """;
        IReadOnlyCollection<string> allowedValues = [];

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().HaveAllowedValuesAtPath("$.statuses", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_ThrowsArgumentNullException_WhenAllowedValuesAreNull()
    {
        const string actual = """
            { "statuses": ["queued"] }
            """;
        IReadOnlyCollection<string>? allowedValues = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().HaveAllowedValuesAtPath("$.statuses", allowedValues!));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValuesAtPath_ThrowsArgumentException_WhenAllowedValuesContainNull()
    {
        const string actual = """
            { "statuses": ["queued"] }
            """;
        IReadOnlyCollection<string> allowedValues = ["queued", null!];

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().HaveAllowedValuesAtPath("$.statuses", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveJsonObjectItemsWithPropertiesAtPath_CollectionOverload_IncludesBecauseInFailure()
    {
        const string actual = """
            { "items": [{ "id": "ord_1" }] }
            """;
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath(
                "$.items",
                requiredProperties,
                "because item contracts should stay stable"));

        Assert.Contains("because item contracts should stay stable", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveJsonObjectItemsWithPropertiesAtPath_CollectionOverload_PropagatesCallerMetadataToFailureStrategy()
    {
        var strategy = new CapturingFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);
        const string callerFilePath = "/tests/json-array-contracts.cs";
        const int callerLineNumber = 321;
        const string actual = """
            { "items": [{ "id": "ord_1" }] }
            """;
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<CapturedFailureException>(() =>
            actual.Should().HaveJsonObjectItemsWithPropertiesAtPath(
                "$.items",
                requiredProperties,
                "because item contracts should stay stable",
                callerFilePath,
                callerLineNumber));

        Assert.Equal(callerFilePath, ex.CallerFilePath);
        Assert.Equal(callerLineNumber, ex.CallerLineNumber);
        Assert.Contains(ex, strategy.Failures);
    }

    [Fact]
    public void ContractAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        const string actual = """
            { "status": "failed" }
            """;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => actual.Should().HaveAllowedValueAtPath("$.status", "queued"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    private sealed class CapturingFailureStrategy : IFailureStrategy
    {
        public List<CapturedFailureException> Failures { get; } = new();

        public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
        {
            var ex = new CapturedFailureException(message, callerFilePath, callerLineNumber);
            Failures.Add(ex);
            throw ex;
        }
    }

    private sealed class CapturedFailureException : Exception
    {
        public CapturedFailureException(string message, string? callerFilePath, int callerLineNumber)
            : base(message)
        {
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        public string? CallerFilePath { get; }
        public int CallerLineNumber { get; }
    }
}
