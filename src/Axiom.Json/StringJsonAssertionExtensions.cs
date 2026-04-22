using System.Runtime.CompilerServices;
using System.Text.Json;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Json;

public static class StringJsonAssertionExtensions
{
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
