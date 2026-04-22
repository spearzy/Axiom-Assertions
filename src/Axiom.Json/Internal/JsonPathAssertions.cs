using System.Text.Json;
using Axiom.Core.Failures;

namespace Axiom.Json;

internal static class JsonPathAssertions
{
    public static void AssertHavePath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to have JSON path {parsedPath.DisplayPath}", IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(subject.InvalidDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var resolution = JsonPathResolver.ResolvePath(subject.Root, parsedPath);
        if (resolution.Success)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay(resolution.FailureDetail!),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotHavePath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to not have JSON path {parsedPath.DisplayPath}", IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(subject.InvalidDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var resolution = JsonPathResolver.ResolvePath(subject.Root, parsedPath);
        if (!resolution.Success)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay($"present path {parsedPath.DisplayPath} with {JsonAssertionSupport.DescribeElement(resolution.Value)}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveStringAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        AssertScalarAtPath(
            subjectInput,
            subjectExpression,
            parsedPath,
            $"to have JSON string at path {parsedPath.DisplayPath} equal to {JsonAssertionSupport.FormatValue(expectedValue)}",
            because,
            callerFilePath,
            callerLineNumber,
            static element => element.ValueKind == JsonValueKind.String,
            static element => element.GetString(),
            expectedValue,
            StringComparer.Ordinal,
            static (element, jsonPath) => JsonAssertionSupport.DescribeWrongValueKind(element, jsonPath.DisplayPath, "String"));
    }

    public static void AssertHaveNumberAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        JsonNumberExpectation expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation(
            $"to have JSON number at path {parsedPath.DisplayPath} equal to {expectedValue.DisplayText}",
            IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(subject.InvalidDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var resolution = JsonPathResolver.ResolvePath(subject.Root, parsedPath);
        if (!resolution.Success)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(resolution.FailureDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (resolution.Value.ValueKind != JsonValueKind.Number)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(JsonAssertionSupport.DescribeWrongValueKind(resolution.Value, parsedPath.DisplayPath, "Number")),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var actualText = resolution.Value.GetRawText();
        if (JsonNumberCanonicalizer.AreEquivalent(actualText, expectedValue.CanonicalText))
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay($"JSON number {actualText} at {parsedPath.DisplayPath}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveBooleanAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        AssertScalarAtPath(
            subjectInput,
            subjectExpression,
            parsedPath,
            $"to have JSON boolean at path {parsedPath.DisplayPath} equal to {JsonAssertionSupport.FormatValue(expectedValue)}",
            because,
            callerFilePath,
            callerLineNumber,
            static element => element.ValueKind is JsonValueKind.True or JsonValueKind.False,
            static element => element.GetBoolean(),
            expectedValue,
            EqualityComparer<bool>.Default,
            static (element, jsonPath) => JsonAssertionSupport.DescribeWrongValueKind(element, jsonPath.DisplayPath, "Boolean"));
    }

    public static void AssertHaveNullAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var parsedPath = JsonPath.Parse(path);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to have JSON null at path {parsedPath.DisplayPath}", IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(subject.InvalidDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var resolution = JsonPathResolver.ResolvePath(subject.Root, parsedPath);
        if (!resolution.Success)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(resolution.FailureDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (resolution.Value.ValueKind == JsonValueKind.Null)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay($"{JsonAssertionSupport.DescribeElement(resolution.Value)} at {parsedPath.DisplayPath}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertScalarAtPath<TValue>(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonPath parsedPath,
        string expectationText,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        Func<JsonElement, bool> kindPredicate,
        Func<JsonElement, TValue?> valueAccessor,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        Func<JsonElement, JsonPath, string> wrongKindFormatter)
        where TValue : notnull
    {
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation(expectationText, IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(subject.InvalidDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var resolution = JsonPathResolver.ResolvePath(subject.Root, parsedPath);
        if (!resolution.Success)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(resolution.FailureDetail!),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (!kindPredicate(resolution.Value))
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(wrongKindFormatter(resolution.Value, parsedPath)),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var actualValue = valueAccessor(resolution.Value);
        if (actualValue is not null && comparer.Equals(actualValue, expectedValue))
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay($"{JsonAssertionSupport.DescribeElement(resolution.Value)} at {parsedPath.DisplayPath}"),
            because,
            callerFilePath,
            callerLineNumber);
    }
}
