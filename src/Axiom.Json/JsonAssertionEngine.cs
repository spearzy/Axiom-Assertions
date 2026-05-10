namespace Axiom.Json;

internal static class JsonAssertionEngine
{
    public static void AssertBeValidJson(
        JsonInput subjectInput,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonContractAssertions.AssertBeValidJson(
            subjectInput,
            subjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeJsonEquivalentTo(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonInput expectedInput,
        string expectedArgumentName,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonEquivalencyAssertions.AssertBeEquivalentTo(
            subjectInput,
            subjectExpression,
            expectedInput,
            expectedArgumentName,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertNotBeJsonEquivalentTo(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonInput unexpectedInput,
        string unexpectedArgumentName,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonEquivalencyAssertions.AssertNotBeEquivalentTo(
            subjectInput,
            subjectExpression,
            unexpectedInput,
            unexpectedArgumentName,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHavePath(
            subjectInput,
            subjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertNotHaveJsonPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertNotHavePath(
            subjectInput,
            subjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonStringAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveStringAtPath(
            subjectInput,
            subjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonObjectAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveObjectAtPath(
            subjectInput,
            subjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonArrayAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveArrayAtPath(
            subjectInput,
            subjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonArrayLengthAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        int expectedLength,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveArrayLengthAtPath(
            subjectInput,
            subjectExpression,
            path,
            expectedLength,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonPropertyCountAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        int expectedCount,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHavePropertyCountAtPath(
            subjectInput,
            subjectExpression,
            path,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonNumberAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        JsonNumberExpectation expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveNumberAtPath(
            subjectInput,
            subjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonBooleanAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveBooleanAtPath(
            subjectInput,
            subjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonNullAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonPathAssertions.AssertHaveNullAtPath(
            subjectInput,
            subjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonPropertiesAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonContractAssertions.AssertHavePropertiesAtPath(
            subjectInput,
            subjectExpression,
            path,
            propertyNames,
            exact,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveAllowedValueAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonContractAssertions.AssertHaveAllowedValueAtPath(
            subjectInput,
            subjectExpression,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveJsonObjectItemsWithPropertiesAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> propertyNames,
        bool exact,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonContractAssertions.AssertHaveObjectItemsWithPropertiesAtPath(
            subjectInput,
            subjectExpression,
            path,
            propertyNames,
            exact,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveAllowedValuesAtPath(
        JsonInput subjectInput,
        string? subjectExpression,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonContractAssertions.AssertHaveAllowedValuesAtPath(
            subjectInput,
            subjectExpression,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);
}
