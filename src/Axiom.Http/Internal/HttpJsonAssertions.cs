using System.Text.Json;
using Axiom.Core.Failures;
using Axiom.Json;

namespace Axiom.Http;

internal static class HttpJsonAssertions
{
    private const string ProblemJsonMediaType = "application/problem+json";

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson, nameof(expectedJson))));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertBeValidJson(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have valid JSON body", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeValidJson(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonPropertiesAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectedSet = JsonAssertionBridge.FormatPropertyNames(propertyNames);
        var displayPath = JsonAssertionBridge.GetDisplayPath(path);
        var expectationText = exact
            ? $"to have only JSON properties {expectedSet} at path {displayPath}"
            : $"to have JSON properties {expectedSet} at path {displayPath}";
        var expectation = new Expectation(expectationText, IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHavePropertiesAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            propertyNames,
            exact,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveAllowedValueAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var allowedSet = JsonAssertionBridge.FormatAllowedValues(allowedValues);
        var displayPath = JsonAssertionBridge.GetDisplayPath(path);
        var expectation = new Expectation(
            $"to have JSON string at path {displayPath} equal to one of {allowedSet}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveAllowedValueAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonObjectItemsWithPropertiesAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectedSet = JsonAssertionBridge.FormatPropertyNames(propertyNames);
        var displayPath = JsonAssertionBridge.GetDisplayPath(path);
        var expectationText = exact
            ? $"to have JSON object items with only properties {expectedSet} at path {displayPath}"
            : $"to have JSON object items with properties {expectedSet} at path {displayPath}";
        var expectation = new Expectation(expectationText, IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveObjectItemsWithPropertiesAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            propertyNames,
            exact,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveAllowedValuesAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var allowedSet = JsonAssertionBridge.FormatAllowedValues(allowedValues);
        var displayPath = JsonAssertionBridge.GetDisplayPath(path);
        var expectation = new Expectation(
            $"to have JSON string values at path {displayPath} equal to one of {allowedSet}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveAllowedValuesAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHavePath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotHaveJsonPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to not have JSON path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertNotHavePath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonStringAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON string at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonObjectAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON object at path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveObjectAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonArrayAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON array at path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveArrayAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonArrayLengthAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        int expectedLength,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (expectedLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedLength), "expectedLength must be greater than or equal to 0.");
        }

        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON array at path {path} with length {expectedLength}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveArrayLengthAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedLength,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonPropertyCountAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        int expectedCount,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        if (expectedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedCount), "expectedCount must be greater than or equal to 0.");
        }

        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON object at path {path} with property count {expectedCount}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHavePropertyCountAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNumberAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        decimal expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON number at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNumberAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        double expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON number at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBooleanAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON boolean at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveBooleanAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNullAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON null at path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNullAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetails(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details", IncludeExpectedValue: false);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var titleFailure = JsonAssertionBridge.GetValueKindFailureDetailAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.title",
            JsonValueKind.String);
        if (titleFailure is not null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                DescribeProblemDetailsMemberFailure("$.title", "string", titleFailure),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var statusFailure = JsonAssertionBridge.GetValueKindFailureDetailAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.status",
            JsonValueKind.Number);
        if (statusFailure is not null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                DescribeProblemDetailsMemberFailure("$.status", "number", statusFailure),
                because,
                callerFilePath,
                callerLineNumber);
        }
    }

    public static void AssertHaveValidationErrors(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have validation errors", IncludeExpectedValue: false);
        if (!TryGetValidationErrors(
            subject,
            subjectExpression,
            expectation,
            because,
            callerFilePath,
            callerLineNumber,
            out var document,
            out _))
        {
            return;
        }

        document!.Dispose();
    }

    public static void AssertHaveValidationErrorFor(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string key,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(key);

        var displayKey = JsonAssertionBridge.FormatValue(key);
        var expectation = new Expectation($"to have validation error for {displayKey}", IncludeExpectedValue: false);
        if (!TryGetValidationMessagesForKey(
            subject,
            subjectExpression,
            expectation,
            key,
            requireNonEmpty: true,
            because,
            callerFilePath,
            callerLineNumber,
            out var document,
            out _))
        {
            return;
        }

        document!.Dispose();
    }

    public static void AssertHaveValidationErrorMessageFor(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string key,
        string expectedMessage,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(expectedMessage);

        var displayKey = JsonAssertionBridge.FormatValue(key);
        var expected = JsonAssertionBridge.FormatValue(expectedMessage);
        var expectation = new Expectation($"to have validation error message for {displayKey} equal to {expected}", IncludeExpectedValue: false);
        if (!TryGetValidationMessagesForKey(
            subject,
            subjectExpression,
            expectation,
            key,
            requireNonEmpty: false,
            because,
            callerFilePath,
            callerLineNumber,
            out var document,
            out var messages))
        {
            return;
        }

        using var parsedDocument = document!;
        if (messages.Contains(expectedMessage, StringComparer.Ordinal))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            HttpAssertionSupport.SubjectLabel(subjectExpression),
            expectation,
            new HttpDisplay(FormatStringSet(messages)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveValidationErrorMessagesFor(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string key,
        IReadOnlyCollection<string> expectedMessages,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(key);
        var expected = ValidateExpectedValidationMessages(expectedMessages);

        var displayKey = JsonAssertionBridge.FormatValue(key);
        var expectedSet = FormatStringSet(expected);
        var expectation = new Expectation($"to have validation error messages for {displayKey} including {expectedSet}", IncludeExpectedValue: false);
        if (!TryGetValidationMessagesForKey(
            subject,
            subjectExpression,
            expectation,
            key,
            requireNonEmpty: false,
            because,
            callerFilePath,
            callerLineNumber,
            out var document,
            out var actualMessages))
        {
            return;
        }

        using var parsedDocument = document!;
        var missing = expected
            .Where(message => !actualMessages.Contains(message, StringComparer.Ordinal))
            .ToArray();
        if (missing.Length == 0)
        {
            return;
        }

        HttpAssertionSupport.Fail(
            HttpAssertionSupport.SubjectLabel(subjectExpression),
            expectation,
            new HttpDisplay(FormatStringSet(actualMessages)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsTitle(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedTitle,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details title", expectedTitle);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.title",
            expectedTitle,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsStatus(
        HttpResponseMessage? subject,
        string? subjectExpression,
        int expectedStatus,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details status", new HttpDisplay(expectedStatus.ToString()));
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.status",
            expectedStatus,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsType(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedType,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details type", expectedType);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.type",
            expectedType,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsDetail(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedDetail,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details detail", expectedDetail);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.detail",
            expectedDetail,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        JsonDocument expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson)));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        JsonElement expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson)));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static bool TryGetValidationMessagesForKey(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string key,
        bool requireNonEmpty,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out JsonDocument? document,
        out string[] messages)
    {
        messages = [];
        if (!TryGetValidationErrors(
            subject,
            subjectExpression,
            expectation,
            because,
            callerFilePath,
            callerLineNumber,
            out document,
            out var errors))
        {
            return false;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var displayKey = JsonAssertionBridge.FormatValue(key);
        if (!errors.TryGetProperty(key, out var errorValue))
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay($"missing validation error key {displayKey}"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (errorValue.ValueKind != JsonValueKind.Array)
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay($"validation error key {displayKey} must be an array, but found {DescribeJsonElement(errorValue)}"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        var collected = new List<string>();
        var index = 0;
        foreach (var item in errorValue.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String)
            {
                return FailAndDispose(
                    document,
                    subjectLabel,
                    expectation,
                    new HttpDisplay($"validation error key {displayKey} message at index {index} must be string, but found {DescribeJsonElement(item)}"),
                    because,
                    callerFilePath,
                    callerLineNumber);
            }

            collected.Add(item.GetString()!);
            index++;
        }

        messages = [.. collected];
        if (requireNonEmpty && messages.Length == 0)
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay($"validation error key {displayKey} contains no messages"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        return true;
    }

    private static bool TryGetValidationErrors(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out JsonDocument? document,
        out JsonElement errors)
    {
        document = null;
        errors = default;

        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return false;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        try
        {
            document = JsonDocument.Parse(bodyText);
        }
        catch (JsonException ex)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay($"invalid JSON in {HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression)} ({BuildInvalidJsonDetail(ex)})"),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay($"ProblemDetails body must be object, but found {DescribeJsonElement(root)}"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (!TryValidateProblemDetailsMember(root, "$.title", JsonValueKind.String, "string", out var memberFailure) ||
            !TryValidateProblemDetailsMember(root, "$.status", JsonValueKind.Number, "number", out memberFailure))
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                memberFailure,
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (!root.TryGetProperty("errors", out errors))
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay("missing JSON property $.errors"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        if (errors.ValueKind != JsonValueKind.Object)
        {
            return FailAndDispose(
                document,
                subjectLabel,
                expectation,
                new HttpDisplay($"JSON property $.errors must be object, but found {DescribeJsonElement(errors)}"),
                because,
                callerFilePath,
                callerLineNumber);
        }

        return true;
    }

    private static bool TryValidateProblemDetailsMember(
        JsonElement root,
        string path,
        JsonValueKind expectedKind,
        string expectedKindText,
        out HttpDisplay failure)
    {
        failure = default;
        var propertyName = path["$.".Length..];
        if (!root.TryGetProperty(propertyName, out var member))
        {
            failure = new HttpDisplay($"missing required ProblemDetails member {path}");
            return false;
        }

        if (member.ValueKind != expectedKind)
        {
            failure = new HttpDisplay($"ProblemDetails member {path} must be {expectedKindText}, but found {DescribeJsonElement(member)}");
            return false;
        }

        return true;
    }

    private static bool FailAndDispose(
        JsonDocument? document,
        string subjectLabel,
        Expectation expectation,
        HttpDisplay actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        try
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, actual, because, callerFilePath, callerLineNumber);
        }
        finally
        {
            document?.Dispose();
        }

        return false;
    }

    private static string[] ValidateExpectedValidationMessages(IReadOnlyCollection<string> expectedMessages)
    {
        ArgumentNullException.ThrowIfNull(expectedMessages);
        if (expectedMessages.Count == 0)
        {
            throw new ArgumentException("expectedMessages must contain at least one value.", nameof(expectedMessages));
        }

        var validated = new string[expectedMessages.Count];
        var index = 0;
        foreach (var message in expectedMessages)
        {
            if (message is null)
            {
                throw new ArgumentException("expectedMessages must not contain null values.", nameof(expectedMessages));
            }

            validated[index++] = message;
        }

        return validated;
    }

    private static string FormatStringSet(IEnumerable<string> values)
        => "[" + string.Join(", ", values.Select(JsonAssertionBridge.FormatValue)) + "]";

    private static string DescribeJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => $"JSON string {element.GetRawText()}",
            JsonValueKind.Number => $"JSON number {element.GetRawText()}",
            JsonValueKind.True or JsonValueKind.False => $"JSON boolean {element.GetRawText()}",
            JsonValueKind.Null => "JSON null",
            JsonValueKind.Object => "JSON object",
            JsonValueKind.Array => "JSON array",
            _ => $"JSON {element.ValueKind.ToString().ToLowerInvariant()}"
        };
    }

    private static string BuildInvalidJsonDetail(JsonException exception)
    {
        var line = exception.LineNumber ?? 0;
        var bytePosition = exception.BytePositionInLine ?? 0;
        return $"line {line}, byte {bytePosition}";
    }

    private static bool TryGetBodyText(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out string bodyText)
    {
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        if (subject is null)
        {
            bodyText = string.Empty;
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return false;
        }

        var content = HttpAssertionSupport.ReadContent(subject);
        if (string.IsNullOrEmpty(content))
        {
            bodyText = string.Empty;
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                HttpAssertionSupport.MissingBodyContent,
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        bodyText = content;
        return true;
    }

    private static bool TryGetProblemDetailsBody(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out string bodyText)
    {
        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out bodyText))
        {
            return false;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var contentType = subject!.Content!.Headers.ContentType;
        if (contentType is null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay($"missing content type for response body (expected {ProblemJsonMediaType})"),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        if (!string.Equals(contentType.MediaType, ProblemJsonMediaType, StringComparison.OrdinalIgnoreCase))
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay($"content type {HttpAssertionSupport.DescribeContentType(contentType)} (expected {ProblemJsonMediaType})"),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        return true;
    }

    private static HttpDisplay DescribeProblemDetailsMemberFailure(string path, string expectedKind, string failureDetail)
    {
        if (string.Equals(failureDetail, $"missing JSON path {path}", StringComparison.Ordinal))
        {
            return new HttpDisplay($"missing required ProblemDetails member {path}");
        }

        const string separator = " but found ";
        var separatorIndex = failureDetail.IndexOf(separator, StringComparison.Ordinal);
        if (separatorIndex >= 0)
        {
            var actual = failureDetail[(separatorIndex + separator.Length)..];
            return new HttpDisplay($"ProblemDetails member {path} must be {expectedKind}, but found {actual}");
        }

        var wrongKindSuffix = $" at {path}; expected {expectedKind}";
        if (failureDetail.EndsWith(wrongKindSuffix, StringComparison.Ordinal))
        {
            var actual = failureDetail[..^wrongKindSuffix.Length];
            return new HttpDisplay($"ProblemDetails member {path} must be {expectedKind}, but found {actual}");
        }

        return new HttpDisplay(failureDetail);
    }
}
