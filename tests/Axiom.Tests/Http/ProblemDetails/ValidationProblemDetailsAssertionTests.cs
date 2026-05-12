using System.Net;
using Axiom.Http;

namespace Axiom.Tests.Http.ProblemDetails;

public sealed class ValidationProblemDetailsAssertionTests
{
    [Fact]
    public void ValidationErrorAssertions_Pass_ForValidationProblemResponse()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required.", "Name must not be empty."],
            "Email": ["Email is invalid."]
            """);

        var ex = Record.Exception(() =>
        {
            response.Should().HaveValidationErrors();
            response.Should().HaveValidationErrorFor("Name");
            response.Should().HaveValidationErrorMessageFor("Name", "Name is required.");
            response.Should().HaveValidationErrorMessagesFor("Name", "Name is required.", "Name must not be empty.");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenContentTypeIsNotProblemJson()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "status": 400, "errors": {} }""",
            "application/json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found content type application/json; charset=utf-8 (expected application/problem+json).",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenContentTypeIsMissing()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "status": 400, "errors": {} }""",
            "application/problem+json");
        response.Content!.Headers.ContentType = null;

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found missing content type for response body (expected application/problem+json).",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenResponseHasNoContent()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.BadRequest);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal("Expected response to have validation errors, but found missing response body content.", ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenBodyIsInvalidJson()
    {
        using var response = HttpResponseFactory.Create(HttpStatusCode.BadRequest, "{ \"title\": ", "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found invalid JSON in response problem details body (line 0, byte 11).",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenTitleIsMissing()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "status": 400, "errors": {} }""",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found missing required ProblemDetails member $.title.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenStatusHasWrongKind()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "status": "400", "errors": {} }""",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found ProblemDetails member $.status must be number, but found JSON string \"400\".",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenStatusIsMissing()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "errors": {} }""",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found missing required ProblemDetails member $.status.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenErrorsIsMissing()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "status": 400 }""",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found missing JSON property $.errors.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrors_Throws_WhenErrorsIsNotObject()
    {
        using var response = HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            """{ "title": "Validation failed", "status": 400, "errors": [] }""",
            "application/problem+json");

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrors());

        Assert.Equal(
            "Expected response to have validation errors, but found JSON property $.errors must be object, but found JSON array.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorFor_Passes_WhenKeyHasStringArrayMessages()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);

        var ex = Record.Exception(() => response.Should().HaveValidationErrorFor("Name"));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrorFor_Throws_WhenKeyIsMissing()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrorFor("Email"));

        Assert.Equal(
            "Expected response to have validation error for \"Email\", but found missing validation error key \"Email\".",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorFor_Throws_WhenKeyValueIsNotArray()
    {
        using var response = CreateValidationResponse(
            """
            "Name": "Name is required."
            """);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrorFor("Name"));

        Assert.Equal(
            "Expected response to have validation error for \"Name\", but found validation error key \"Name\" must be an array, but found JSON string \"Name is required.\".",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorFor_Throws_WhenMessageArrayIsEmpty()
    {
        using var response = CreateValidationResponse(
            """
            "Name": []
            """);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrorFor("Name"));

        Assert.Equal(
            "Expected response to have validation error for \"Name\", but found validation error key \"Name\" contains no messages.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorFor_Throws_WhenArrayContainsNonStringMessage()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required.", 42]
            """);

        var ex = Assert.Throws<InvalidOperationException>(() => response.Should().HaveValidationErrorFor("Name"));

        Assert.Equal(
            "Expected response to have validation error for \"Name\", but found validation error key \"Name\" message at index 1 must be string, but found JSON number 42.",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorMessageFor_Passes_WhenExpectedMessageIsPresent()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required.", "Name must not be empty."]
            """);

        var ex = Record.Exception(() => response.Should().HaveValidationErrorMessageFor("Name", "Name is required."));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrorMessageFor_Throws_WhenExpectedMessageIsMissing()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name must not be empty."]
            """);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveValidationErrorMessageFor("Name", "Name is required."));

        Assert.Equal(
            "Expected response to have validation error message for \"Name\" equal to \"Name is required.\", but found [\"Name must not be empty.\"].",
            ex.Message);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_Passes_WhenAllExpectedMessagesArePresent()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required.", "Name must not be empty."]
            """);
        IReadOnlyCollection<string> expectedMessages = ["Name is required.", "Name must not be empty."];

        var ex = Record.Exception(() => response.Should().HaveValidationErrorMessagesFor("Name", expectedMessages));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_Passes_WhenActualMessagesContainExtras()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required.", "Name must not be empty.", "Name is too long."]
            """);

        var ex = Record.Exception(() => response.Should().HaveValidationErrorMessagesFor("Name", "Name is required."));

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_Throws_WhenOneExpectedMessageIsMissing()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name must not be empty."]
            """);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveValidationErrorMessagesFor("Name", "Name is required.", "Name must not be empty."));

        Assert.Equal(
            "Expected response to have validation error messages for \"Name\" including [\"Name is required.\", \"Name must not be empty.\"], but found [\"Name must not be empty.\"].",
            ex.Message);
    }

    [Fact]
    public void ValidationErrorAssertions_UseExactKeysWithDotsAndBrackets()
    {
        using var response = CreateValidationResponse(
            """
            "Address.Postcode": ["Postcode is required."],
            "Items[0].Name": ["Item name is required."],
            "customer.email": ["Email is invalid."]
            """);

        var ex = Record.Exception(() =>
        {
            response.Should().HaveValidationErrorMessageFor("Address.Postcode", "Postcode is required.");
            response.Should().HaveValidationErrorMessageFor("Items[0].Name", "Item name is required.");
            response.Should().HaveValidationErrorMessageFor("customer.email", "Email is invalid.");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void ValidationErrorAssertions_AllowEmptyStringKey()
    {
        using var response = CreateValidationResponse(
            """
            "": ["The request body is invalid."]
            """);

        var ex = Record.Exception(() =>
        {
            response.Should().HaveValidationErrorFor("");
            response.Should().HaveValidationErrorMessageFor("", "The request body is invalid.");
        });

        Assert.Null(ex);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_ParamsOverload_DelegatesToCollectionValidation()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);

        var ex = Assert.Throws<ArgumentException>(() => response.Should().HaveValidationErrorMessagesFor("Name"));

        Assert.Equal("expectedMessages", ex.ParamName);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);

        var ex = Assert.Throws<ArgumentNullException>(() =>
            response.Should().HaveValidationErrorMessagesFor(null!, "Name is required."));

        Assert.Equal("key", ex.ParamName);
    }

    [Fact]
    public void HaveValidationErrorMessageFor_ThrowsArgumentNullException_WhenExpectedMessageIsNull()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);

        var ex = Assert.Throws<ArgumentNullException>(() =>
            response.Should().HaveValidationErrorMessageFor("Name", null!));

        Assert.Equal("expectedMessage", ex.ParamName);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_ThrowsArgumentNullException_WhenExpectedMessagesAreNull()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);
        IReadOnlyCollection<string>? expectedMessages = null;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            response.Should().HaveValidationErrorMessagesFor("Name", expectedMessages!));

        Assert.Equal("expectedMessages", ex.ParamName);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_ThrowsArgumentException_WhenExpectedMessagesContainNull()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name is required."]
            """);
        IReadOnlyCollection<string> expectedMessages = ["Name is required.", null!];

        var ex = Assert.Throws<ArgumentException>(() =>
            response.Should().HaveValidationErrorMessagesFor("Name", expectedMessages));

        Assert.Equal("expectedMessages", ex.ParamName);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_CollectionOverload_IncludesBecauseInFailure()
    {
        using var response = CreateValidationResponse(
            """
            "Name": ["Name must not be empty."]
            """);
        IReadOnlyCollection<string> expectedMessages = ["Name is required."];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            response.Should().HaveValidationErrorMessagesFor(
                "Name",
                expectedMessages,
                "because clients should see stable validation messages"));

        Assert.Contains("because clients should see stable validation messages", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void HaveValidationErrorMessagesFor_CollectionOverload_PropagatesCallerMetadataToFailureStrategy()
    {
        var strategy = new CapturingFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);
        const string callerFilePath = "/tests/validation-problem-details.cs";
        const int callerLineNumber = 96;
        using var response = CreateValidationResponse(
            """
            "Name": ["Name must not be empty."]
            """);
        IReadOnlyCollection<string> expectedMessages = ["Name is required."];

        var ex = Assert.Throws<CapturedFailureException>(() =>
            response.Should().HaveValidationErrorMessagesFor(
                "Name",
                expectedMessages,
                "because clients should see stable validation messages",
                callerFilePath,
                callerLineNumber));

        Assert.Equal(callerFilePath, ex.CallerFilePath);
        Assert.Equal(callerLineNumber, ex.CallerLineNumber);
        Assert.Contains(ex, strategy.Failures);
    }

    [Fact]
    public void HaveValidationErrorFor_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        using var response = CreateValidationResponse(
            """
            "Name": []
            """);
        using var batch = new Axiom.Core.Batch();

        var callEx = Record.Exception(() => response.Should().HaveValidationErrorFor("Name"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    private static HttpResponseMessage CreateValidationResponse(string errorsMembers)
        => HttpResponseFactory.Create(
            HttpStatusCode.BadRequest,
            $$"""
            {
              "type": "https://example.test/problems/validation",
              "title": "Validation failed",
              "status": 400,
              "errors": {
                {{errorsMembers}}
              }
            }
            """,
            "application/problem+json");

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
