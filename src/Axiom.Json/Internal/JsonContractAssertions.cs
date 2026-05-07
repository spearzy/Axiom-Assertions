using System.Text.Json;
using Axiom.Core.Failures;

namespace Axiom.Json;

internal static class JsonContractAssertions
{
    public static void AssertBeValidJson(
        JsonInput subjectInput,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation("to be valid JSON", IncludeExpectedValue: false);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (subject.IsValid)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay(JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, subject.InvalidDetail!)),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHavePropertiesAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var properties = ValidatePropertyNames(propertyNames);
        var parsedPath = JsonPath.Parse(path);
        var expectedSet = FormatStringSet(properties);
        var expectationText = exact
            ? $"to have only JSON properties {expectedSet} at path {parsedPath.DisplayPath}"
            : $"to have JSON properties {expectedSet} at path {parsedPath.DisplayPath}";

        AssertObjectContractAtPath(
            subjectInput,
            subjectExpression,
            parsedPath,
            expectationText,
            because,
            callerFilePath,
            callerLineNumber,
            element => CheckProperties(element, parsedPath.DisplayPath, properties, exact));
    }

    public static void AssertHaveAllowedValueAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var allowed = ValidateAllowedValues(allowedValues);
        var parsedPath = JsonPath.Parse(path);
        var allowedSet = FormatStringSet(allowed);
        var expectation = new Expectation(
            $"to have JSON string at path {parsedPath.DisplayPath} equal to one of {allowedSet}",
            IncludeExpectedValue: false);

        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);

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
                new JsonDisplay(JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, subject.InvalidDetail!)),
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

        if (resolution.Value.ValueKind != JsonValueKind.String)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(JsonAssertionSupport.DescribeWrongValueKind(resolution.Value, parsedPath.DisplayPath, "String")),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var actual = resolution.Value.GetString();
        if (actual is not null && allowed.Contains(actual, StringComparer.Ordinal))
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay($"JSON string {JsonAssertionSupport.FormatValue(actual)} at {parsedPath.DisplayPath}; allowed values {allowedSet}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertObjectContractAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonPath parsedPath,
        string expectationText,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        Func<JsonElement, string?> failureFactory)
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
                new JsonDisplay(JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, subject.InvalidDetail!)),
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

        if (resolution.Value.ValueKind != JsonValueKind.Object)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new JsonDisplay(JsonAssertionSupport.DescribeWrongValueKind(resolution.Value, parsedPath.DisplayPath, "Object")),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var failure = failureFactory(resolution.Value);
        if (failure is null)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new JsonDisplay(failure),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static string? CheckProperties(JsonElement element, string path, string[] expectedProperties, bool exact)
    {
        var actual = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            actual.Add(property.Name);
        }

        var expected = new HashSet<string>(expectedProperties, StringComparer.Ordinal);
        var missing = expected
            .Where(property => !actual.Contains(property))
            .OrderBy(static property => property, StringComparer.Ordinal)
            .ToArray();
        if (!exact)
        {
            return missing.Length == 0
                ? null
                : $"JSON object at {path} missing properties {FormatStringSet(missing)}";
        }

        var extra = actual
            .Where(property => !expected.Contains(property))
            .OrderBy(static property => property, StringComparer.Ordinal)
            .ToArray();
        if (missing.Length == 0 && extra.Length == 0)
        {
            return null;
        }

        return $"JSON object properties mismatch at {path}: missing {FormatStringSet(missing)}; extra {FormatStringSet(extra)}";
    }

    private static string[] ValidatePropertyNames(IReadOnlyCollection<string> propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames);
        return ValidateStringSet(propertyNames, nameof(propertyNames), allowEmpty: true);
    }

    private static string[] ValidateAllowedValues(IReadOnlyCollection<string> allowedValues)
    {
        ArgumentNullException.ThrowIfNull(allowedValues);
        return ValidateStringSet(allowedValues, nameof(allowedValues), allowEmpty: false);
    }

    private static string[] ValidateStringSet(IReadOnlyCollection<string> values, string argumentName, bool allowEmpty)
    {
        if (!allowEmpty && values.Count == 0)
        {
            throw new ArgumentException($"{argumentName} must contain at least one value.", argumentName);
        }

        var validated = new string[values.Count];
        var index = 0;
        foreach (var value in values)
        {
            if (value is null)
            {
                throw new ArgumentException($"{argumentName} must not contain null values.", argumentName);
            }

            validated[index++] = value;
        }

        return validated;
    }

    private static string FormatStringSet(IEnumerable<string> values)
        => "[" + string.Join(", ", values.Select(JsonAssertionSupport.FormatValue)) + "]";
}
