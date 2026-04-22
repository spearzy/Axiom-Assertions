using System.Runtime.CompilerServices;
using System.Text.Json;
using Axiom.Assertions.Chaining;

namespace Axiom.Json;

public sealed class JsonAssertions
{
    internal JsonAssertions(JsonInput subjectInput, string? subjectExpression)
    {
        SubjectInput = subjectInput;
        SubjectExpression = subjectExpression;
    }

    internal JsonInput SubjectInput { get; }
    internal string? SubjectExpression { get; }

    public AndContinuation<JsonAssertions> BeJsonEquivalentTo(
        string expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedJson);

        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromString(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> BeJsonEquivalentTo(
        JsonDocument expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedJson);

        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromDocument(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> BeJsonEquivalentTo(
        JsonElement expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromElement(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> NotBeJsonEquivalentTo(
        string unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(unexpectedJson);

        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromString(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> NotBeJsonEquivalentTo(
        JsonDocument unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(unexpectedJson);

        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromDocument(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> NotBeJsonEquivalentTo(
        JsonElement unexpectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertNotBeJsonEquivalentTo(
            SubjectInput,
            SubjectExpression,
            JsonInput.FromElement(unexpectedJson),
            nameof(unexpectedJson),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertHaveJsonPath(
            SubjectInput,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> NotHaveJsonPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertNotHaveJsonPath(
            SubjectInput,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonStringAtPath(
        string path,
        string expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedValue);

        JsonAssertionEngine.AssertHaveJsonStringAtPath(
            SubjectInput,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonNumberAtPath(
        string path,
        decimal expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            SubjectInput,
            SubjectExpression,
            path,
            JsonNumberExpectation.FromDecimal(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonNumberAtPath(
        string path,
        double expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (double.IsNaN(expectedValue) || double.IsInfinity(expectedValue))
        {
            throw new ArgumentOutOfRangeException(nameof(expectedValue), "expectedValue must be a finite JSON number.");
        }

        JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            SubjectInput,
            SubjectExpression,
            path,
            JsonNumberExpectation.FromDouble(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonBooleanAtPath(
        string path,
        bool expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertHaveJsonBooleanAtPath(
            SubjectInput,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }

    public AndContinuation<JsonAssertions> HaveJsonNullAtPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        JsonAssertionEngine.AssertHaveJsonNullAtPath(
            SubjectInput,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<JsonAssertions>(this);
    }
}
