using System.Globalization;
using System.Text.Json;

namespace Axiom.Json;

internal static class JsonAssertionBridge
{
    public static void ValidatePath(string path) => _ = JsonPath.Parse(path);

    public static string GetDisplayPath(string path) => JsonPath.Parse(path).DisplayPath;

    public static string FormatValue(object? value) => JsonAssertionSupport.FormatValue(value);

    public static string FormatPropertyNames(IReadOnlyCollection<string> propertyNames)
        => JsonContractAssertions.FormatPropertyNames(propertyNames);

    public static string FormatAllowedValues(IReadOnlyCollection<string> allowedValues)
        => JsonContractAssertions.FormatAllowedValues(allowedValues);

    public static string DescribeExpectedJson(string expectedJson, string argumentName)
    {
        using var expected = JsonParsedValue.ParseExpected(JsonInput.FromString(expectedJson), argumentName);
        return expected.DisplayText;
    }

    public static string DescribeExpectedJson(JsonDocument expectedJson)
        => JsonSerializer.Serialize(expectedJson.RootElement);

    public static string DescribeExpectedJson(JsonElement expectedJson)
        => JsonSerializer.Serialize(expectedJson);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        string expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromString(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        JsonDocument expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromDocument(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        JsonElement expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromElement(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeValidJson(
        string subjectJson,
        string subjectLabel,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeValidJson(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHavePath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertNotHavePath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertNotHaveJsonPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveStringAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonStringAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveObjectAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonObjectAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveArrayAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonArrayAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveArrayLengthAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        int expectedLength,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonArrayLengthAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedLength,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHavePropertyCountAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        int expectedCount,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonPropertyCountAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        decimal expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            JsonNumberExpectation.FromDecimal(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        double expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            JsonNumberExpectation.FromDouble(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        int expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            new JsonNumberExpectation(
                expectedValue.ToString(CultureInfo.InvariantCulture),
                expectedValue.ToString(CultureInfo.InvariantCulture)),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveBooleanAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonBooleanAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNullAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNullAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHavePropertiesAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonPropertiesAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            propertyNames,
            exact,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveAllowedValueAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveAllowedValueAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);

    public static string? GetValueKindFailureDetailAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        JsonValueKind expectedKind)
    {
        using var parsed = JsonParsedValue.ParseSubject(JsonInput.FromString(subjectJson));

        if (!parsed.IsValid)
        {
            return JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, parsed.InvalidDetail!);
        }

        var resolution = JsonPathResolver.ResolvePath(parsed.Root, JsonPath.Parse(path));
        if (!resolution.Success)
        {
            return resolution.FailureDetail!;
        }

        if (resolution.Value.ValueKind == expectedKind)
        {
            return null;
        }

        return JsonAssertionSupport.DescribeWrongValueKind(
            resolution.Value,
            path,
            JsonAssertionSupport.FormatValueKind(expectedKind));
    }
}
