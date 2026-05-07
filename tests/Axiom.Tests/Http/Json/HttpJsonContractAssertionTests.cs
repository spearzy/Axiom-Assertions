using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.Json;

public sealed class HttpJsonContractAssertionTests
{
    [Fact]
    public void BeValidJson_Passes_WhenResponseBodyIsValidJson()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": 1 }", "application/json");

        var ex = Record.Exception(() => response.Should().BeValidJson());

        Assert.Null(ex);
    }

    [Fact]
    public void BeValidJson_Throws_WhenResponseBodyIsInvalidJson()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": ", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().BeValidJson());

        Assert.Equal(
            "Expected response JSON body to be valid JSON, but found invalid JSON in response JSON body (line 0, byte 8).",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenResponseHasNoContent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().BeValidJson());

        Assert.Equal("Expected response to have valid JSON body, but found missing response body content.", ex.Message);
    }

    [Fact]
    public void HaveJsonProperties_Passes_WhenRootObjectContainsAllProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"id\": \"evt_123\", \"status\": \"queued\", \"extra\": true }",
            "application/json");

        var ex = Record.Exception(() => response.Should().HaveJsonProperties("id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonProperties_Throws_WhenRootObjectIsMissingProperties()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": \"evt_123\" }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonProperties("id", "status"));

        Assert.Equal(
            "Expected response JSON body to have JSON properties [\"id\", \"status\"] at path $, but found JSON object at $ missing properties [\"status\"].",
            ex.Message);
    }

    [Fact]
    public void HaveJsonPropertiesAtPath_Passes_WhenNestedObjectContainsAllProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"customer\": { \"id\": \"cus_123\", \"name\": \"Bob\", \"tier\": \"gold\" } }",
            "application/json");

        var ex = Record.Exception(() => response.Should().HaveJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveJsonPropertiesAtPath_Throws_WhenNestedObjectIsMissingProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"customer\": { \"id\": \"cus_123\" } }",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Equal(
            "Expected response JSON body to have JSON properties [\"id\", \"name\"] at path $.customer, but found JSON object at $.customer missing properties [\"name\"].",
            ex.Message);
    }

    [Fact]
    public void HaveOnlyJsonProperties_Passes_WhenRootObjectHasExactlyThoseProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"status\": \"queued\", \"id\": \"evt_123\" }",
            "application/json");

        var ex = Record.Exception(() => response.Should().HaveOnlyJsonProperties("id", "status"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveOnlyJsonProperties_Throws_WhenRootObjectHasMissingAndExtraProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"id\": \"evt_123\", \"debug\": true }",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveOnlyJsonProperties("id", "status"));

        Assert.Equal(
            "Expected response JSON body to have only JSON properties [\"id\", \"status\"] at path $, but found JSON object properties mismatch at $: missing [\"status\"]; extra [\"debug\"].",
            ex.Message);
    }

    [Fact]
    public void HaveOnlyJsonPropertiesAtPath_Passes_WhenNestedObjectHasExactlyThoseProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"customer\": { \"name\": \"Bob\", \"id\": \"cus_123\" } }",
            "application/json");

        var ex = Record.Exception(() => response.Should().HaveOnlyJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveOnlyJsonPropertiesAtPath_Throws_WhenNestedObjectHasExtraProperties()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"customer\": { \"id\": \"cus_123\", \"name\": \"Bob\", \"debug\": true } }",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveOnlyJsonPropertiesAtPath("$.customer", "id", "name"));

        Assert.Equal(
            "Expected response JSON body to have only JSON properties [\"id\", \"name\"] at path $.customer, but found JSON object properties mismatch at $.customer: missing []; extra [\"debug\"].",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Passes_WhenStringValueIsAllowed()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"queued\" }", "application/json");

        var ex = Record.Exception(() => response.Should().HaveAllowedValueAtPath("$.status", "queued", "processing"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Passes_WithAllowedValueCollection()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"queued\" }", "application/json");
        IReadOnlyCollection<string> allowedStatuses = ["queued", "processing", "complete"];

        var ex = Record.Exception(() => response.Should().HaveAllowedValueAtPath("$.status", allowedStatuses));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Throws_WhenStringValueIsNotAllowed()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"failed\" }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveAllowedValueAtPath("$.status", "queued", "complete"));

        Assert.Equal(
            "Expected response JSON body to have JSON string at path $.status equal to one of [\"queued\", \"complete\"], but found JSON string \"failed\" at $.status; allowed values [\"queued\", \"complete\"].",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenResolvedObjectValueIsWrongKind()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"customer\": [] }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonPropertiesAtPath("$.customer", "id"));

        Assert.Equal(
            "Expected response JSON body to have JSON properties [\"id\"] at path $.customer, but found JSON array at $.customer; expected object.",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValueAtPath_Throws_WhenResolvedValueIsWrongKind()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": 1 }", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveAllowedValueAtPath("$.status", "queued"));

        Assert.Equal(
            "Expected response JSON body to have JSON string at path $.status equal to one of [\"queued\"], but found JSON number 1 at $.status; expected string.",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenPathDoesNotExist()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.OK,
            "{ \"customer\": { \"id\": \"cus_123\" } }",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveOnlyJsonPropertiesAtPath("$.customer.profile", "name"));

        Assert.Equal(
            "Expected response JSON body to have only JSON properties [\"name\"] at path $.customer.profile, but found missing JSON path $.customer.profile.",
            ex.Message);
    }

    [Fact]
    public void ContractAssertions_ThrowArgumentException_WhenPathSyntaxIsInvalid()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"customer\": {} }", "application/json");

        var ex = Assert.Throws<ArgumentException>(() => response.Should().HaveJsonPropertiesAtPath("$.customer[", "id"));

        Assert.Equal("path", ex.ParamName);
        Assert.Contains("invalid array index segment", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ContractAssertions_Throw_WhenResponseJsonIsInvalid()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"customer\": ", "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveJsonProperties("id"));

        Assert.Equal(
            "Expected response JSON body to have JSON properties [\"id\"] at path $, but found invalid JSON in response JSON body (line 0, byte 14).",
            ex.Message);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentException_WhenAllowedValuesAreEmpty()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"queued\" }", "application/json");
        IReadOnlyCollection<string> allowedValues = [];

        var ex = Assert.Throws<ArgumentException>(() => response.Should().HaveAllowedValueAtPath("$.status", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentNullException_WhenAllowedValuesAreNull()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"queued\" }", "application/json");
        IReadOnlyCollection<string>? allowedValues = null;

        var ex = Assert.Throws<ArgumentNullException>(() => response.Should().HaveAllowedValueAtPath("$.status", allowedValues!));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveAllowedValueAtPath_ThrowsArgumentException_WhenAllowedValuesContainNull()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"status\": \"queued\" }", "application/json");
        IReadOnlyCollection<string> allowedValues = ["queued", null!];

        var ex = Assert.Throws<ArgumentException>(() => response.Should().HaveAllowedValueAtPath("$.status", allowedValues));

        Assert.Equal("allowedValues", ex.ParamName);
    }

    [Fact]
    public void HaveJsonProperties_CollectionOverload_IncludesBecauseInFailure()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": \"evt_123\" }", "application/json");
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveJsonProperties(requiredProperties, "because API contracts should stay stable"));

        Assert.Contains("because API contracts should stay stable", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveJsonProperties_CollectionOverload_PropagatesCallerMetadataToFailureStrategy()
    {
        var strategy = new CapturingFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);
        const string callerFilePath = "/tests/http-json-contracts.cs";
        const int callerLineNumber = 218;
        using var response = HttpResponseFactory.Create(HttpStatusCode.OK, "{ \"id\": \"evt_123\" }", "application/json");
        IReadOnlyCollection<string> requiredProperties = ["id", "status"];

        var ex = Assert.Throws<CapturedFailureException>(() =>
            response.Should().HaveJsonProperties(
                requiredProperties,
                "because API contracts should stay stable",
                callerFilePath,
                callerLineNumber));

        Assert.Equal(callerFilePath, ex.CallerFilePath);
        Assert.Equal(callerLineNumber, ex.CallerLineNumber);
        Assert.Contains(ex, strategy.Failures);
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
