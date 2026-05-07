using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Axiom.Assertions.Chaining;

namespace Axiom.Http;

public sealed class HttpResponseAssertions
{
    internal HttpResponseAssertions(HttpResponseMessage? subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    internal HttpResponseMessage? Subject { get; }
    internal string? SubjectExpression { get; }

    public AndContinuation<HttpResponseAssertions> HaveStatusCode(
        HttpStatusCode expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertHaveStatusCode(
            Subject,
            SubjectExpression,
            (int)expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> NotHaveStatusCode(
        HttpStatusCode unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertNotHaveStatusCode(
            Subject,
            SubjectExpression,
            (int)unexpected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveStatusCode(
        int expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertHaveStatusCode(
            Subject,
            SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> NotHaveStatusCode(
        int unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertNotHaveStatusCode(
            Subject,
            SubjectExpression,
            unexpected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveHeader(
        string name,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertHaveHeader(
            Subject,
            SubjectExpression,
            name,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> NotHaveHeader(
        string name,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertNotHaveHeader(
            Subject,
            SubjectExpression,
            name,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveHeaderValue(
        string name,
        string expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedValue);

        HttpResponseAssertionEngine.AssertHaveHeaderValue(
            Subject,
            SubjectExpression,
            name,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> ContainHeaderValue(
        string name,
        string expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedValue);

        HttpResponseAssertionEngine.AssertContainHeaderValue(
            Subject,
            SubjectExpression,
            name,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveHeaderValues(
        string name,
        string[] expectedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedValues);

        HttpResponseAssertionEngine.AssertHaveHeaderValues(
            Subject,
            SubjectExpression,
            name,
            expectedValues,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveBodyText(
        string expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expected);

        HttpResponseAssertionEngine.AssertHaveBodyText(
            Subject,
            SubjectExpression,
            expected,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> ContainBodyText(
        string expectedSubstring,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedSubstring);

        HttpResponseAssertionEngine.AssertContainBodyText(
            Subject,
            SubjectExpression,
            expectedSubstring,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveContentType(
        string expectedMediaType,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpResponseAssertionEngine.AssertHaveContentType(
            Subject,
            SubjectExpression,
            expectedMediaType,
            expectedCharset: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveContentTypeWithCharset(
        string expectedMediaType,
        string expectedCharset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedCharset);

        HttpResponseAssertionEngine.AssertHaveContentType(
            Subject,
            SubjectExpression,
            expectedMediaType,
            expectedCharset,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonBodyEquivalentTo(
        string expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedJson);

        HttpJsonAssertions.AssertHaveJsonBodyEquivalentTo(
            Subject,
            SubjectExpression,
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonBodyEquivalentTo(
        JsonDocument expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedJson);

        HttpJsonAssertions.AssertHaveJsonBodyEquivalentTo(
            Subject,
            SubjectExpression,
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonBodyEquivalentTo(
        JsonElement expectedJson,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonBodyEquivalentTo(
            Subject,
            SubjectExpression,
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> BeValidJson(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertBeValidJson(
            Subject,
            SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonProperties(
        params string[] propertyNames)
        => HaveJsonProperties((IReadOnlyCollection<string>)propertyNames);

    public AndContinuation<HttpResponseAssertions> HaveJsonProperties(
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPropertiesAtPath(
            Subject,
            SubjectExpression,
            "$",
            propertyNames,
            exact: false,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonPropertiesAtPath(
        string path,
        params string[] propertyNames)
        => HaveJsonPropertiesAtPath(path, (IReadOnlyCollection<string>)propertyNames);

    public AndContinuation<HttpResponseAssertions> HaveJsonPropertiesAtPath(
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPropertiesAtPath(
            Subject,
            SubjectExpression,
            path,
            propertyNames,
            exact: false,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveOnlyJsonProperties(
        params string[] propertyNames)
        => HaveOnlyJsonProperties((IReadOnlyCollection<string>)propertyNames);

    public AndContinuation<HttpResponseAssertions> HaveOnlyJsonProperties(
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPropertiesAtPath(
            Subject,
            SubjectExpression,
            "$",
            propertyNames,
            exact: true,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveOnlyJsonPropertiesAtPath(
        string path,
        params string[] propertyNames)
        => HaveOnlyJsonPropertiesAtPath(path, (IReadOnlyCollection<string>)propertyNames);

    public AndContinuation<HttpResponseAssertions> HaveOnlyJsonPropertiesAtPath(
        string path,
        IReadOnlyCollection<string> propertyNames,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPropertiesAtPath(
            Subject,
            SubjectExpression,
            path,
            propertyNames,
            exact: true,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveAllowedValueAtPath(
        string path,
        params string[] allowedValues)
        => HaveAllowedValueAtPath(path, (IReadOnlyCollection<string>)allowedValues);

    public AndContinuation<HttpResponseAssertions> HaveAllowedValueAtPath(
        string path,
        IReadOnlyCollection<string> allowedValues,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveAllowedValueAtPath(
            Subject,
            SubjectExpression,
            path,
            allowedValues,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPath(
            Subject,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> NotHaveJsonPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertNotHaveJsonPath(
            Subject,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonObjectAtPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonObjectAtPath(
            Subject,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonArrayAtPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonArrayAtPath(
            Subject,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonArrayLengthAtPath(
        string path,
        int expectedLength,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonArrayLengthAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedLength,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonPropertyCountAtPath(
        string path,
        int expectedCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonPropertyCountAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonStringAtPath(
        string path,
        string expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedValue);

        HttpJsonAssertions.AssertHaveJsonStringAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonNumberAtPath(
        string path,
        decimal expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonNumberAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonNumberAtPath(
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

        HttpJsonAssertions.AssertHaveJsonNumberAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonBooleanAtPath(
        string path,
        bool expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonBooleanAtPath(
            Subject,
            SubjectExpression,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveJsonNullAtPath(
        string path,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveJsonNullAtPath(
            Subject,
            SubjectExpression,
            path,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveProblemDetails(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveProblemDetails(
            Subject,
            SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveProblemDetailsTitle(
        string expectedTitle,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedTitle);

        HttpJsonAssertions.AssertHaveProblemDetailsTitle(
            Subject,
            SubjectExpression,
            expectedTitle,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveProblemDetailsStatus(
        int expectedStatus,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        HttpJsonAssertions.AssertHaveProblemDetailsStatus(
            Subject,
            SubjectExpression,
            expectedStatus,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveProblemDetailsType(
        string expectedType,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedType);

        HttpJsonAssertions.AssertHaveProblemDetailsType(
            Subject,
            SubjectExpression,
            expectedType,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }

    public AndContinuation<HttpResponseAssertions> HaveProblemDetailsDetail(
        string expectedDetail,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(expectedDetail);

        HttpJsonAssertions.AssertHaveProblemDetailsDetail(
            Subject,
            SubjectExpression,
            expectedDetail,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<HttpResponseAssertions>(this);
    }
}
