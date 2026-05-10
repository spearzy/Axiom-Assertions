using System.Runtime.CompilerServices;
using System.Text.Json;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Json;

public static class StringJsonAssertionExtensions
{
    public static AndContinuation<StringAssertions> BeValidJson(
        this StringAssertions assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertBeValidJson(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonProperties(
        this StringAssertions assertions,
        params string[] propertyNames)
        => HaveJsonProperties(assertions, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveJsonProperties(
        this StringAssertions assertions,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonPath.RootDisplayPath,
            propertyNames,
            exact: false,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        params string[] propertyNames)
        => HaveJsonPropertiesAtPath(assertions, path, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveJsonPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            propertyNames,
            exact: false,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveOnlyJsonProperties(
        this StringAssertions assertions,
        params string[] propertyNames)
        => HaveOnlyJsonProperties(assertions, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveOnlyJsonProperties(
        this StringAssertions assertions,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonPath.RootDisplayPath,
            propertyNames,
            exact: true,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveOnlyJsonPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        params string[] propertyNames)
        => HaveOnlyJsonPropertiesAtPath(assertions, path, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveOnlyJsonPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            propertyNames,
            exact: true,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveAllowedValueAtPath(
        this StringAssertions assertions,
        string path,
        params string[] allowedValues)
        => HaveAllowedValueAtPath(assertions, path, (IReadOnlyCollection<string>)allowedValues);

    public static AndContinuation<StringAssertions> HaveAllowedValueAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveAllowedValueAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonObjectItemsWithPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        params string[] propertyNames)
        => HaveJsonObjectItemsWithPropertiesAtPath(assertions, path, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveJsonObjectItemsWithPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonObjectItemsWithPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            propertyNames,
            exact: false,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonObjectItemsWithOnlyPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        params string[] propertyNames)
        => HaveJsonObjectItemsWithOnlyPropertiesAtPath(assertions, path, (IReadOnlyCollection<string>)propertyNames);

    public static AndContinuation<StringAssertions> HaveJsonObjectItemsWithOnlyPropertiesAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonObjectItemsWithPropertiesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            propertyNames,
            exact: true,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveAllowedValuesAtPath(
        this StringAssertions assertions,
        string path,
        params string[] allowedValues)
        => HaveAllowedValuesAtPath(assertions, path, (IReadOnlyCollection<string>)allowedValues);

    public static AndContinuation<StringAssertions> HaveAllowedValuesAtPath(
        this StringAssertions assertions,
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveAllowedValuesAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> BeJsonEquivalentTo(
        this StringAssertions assertions,
        string expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedJson);

        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromString(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> BeJsonEquivalentTo(
        this StringAssertions assertions,
        JsonDocument expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedJson);

        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromDocument(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> BeJsonEquivalentTo(
        this StringAssertions assertions,
        JsonElement expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromElement(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> NotBeJsonEquivalentTo(
        this StringAssertions assertions,
        string unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(unexpectedJson);

        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromString(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> NotBeJsonEquivalentTo(
        this StringAssertions assertions,
        JsonDocument unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(unexpectedJson);

        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromDocument(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> NotBeJsonEquivalentTo(
        this StringAssertions assertions,
        JsonElement unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            JsonInput.FromElement(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonPath(
        this StringAssertions assertions,
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> NotHaveJsonPath(
        this StringAssertions assertions,
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertNotHaveJsonPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonStringAtPath(
        this StringAssertions assertions,
        string path,
        string expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedValue);

        JsonAssertionEngine.AssertHaveJsonStringAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonObjectAtPath(
        this StringAssertions assertions,
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonObjectAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonArrayAtPath(
        this StringAssertions assertions,
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonArrayAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonArrayLengthAtPath(
        this StringAssertions assertions,
        string path,
        int expectedLength,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonArrayLengthAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            expectedLength,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonPropertyCountAtPath(
        this StringAssertions assertions,
        string path,
        int expectedCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonPropertyCountAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonNumberAtPath(
        this StringAssertions assertions,
        string path,
        decimal expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            JsonNumberExpectation.FromDecimal(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonNumberAtPath(
        this StringAssertions assertions,
        string path,
        double expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);
        if (double.IsNaN(expectedValue) || double.IsInfinity(expectedValue))
        {
            throw new ArgumentOutOfRangeException(nameof(expectedValue), "expectedValue must be a finite JSON number.");
        }

        JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            JsonNumberExpectation.FromDouble(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonBooleanAtPath(
        this StringAssertions assertions,
        string path,
        bool expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonBooleanAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }

    public static AndContinuation<StringAssertions> HaveJsonNullAtPath(
        this StringAssertions assertions,
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        JsonAssertionEngine.AssertHaveJsonNullAtPath(
            JsonInput.FromString(assertions.Subject),
            assertions.SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<StringAssertions>(assertions);
    }
}
