using Axiom.Core.Failures;

namespace Axiom.Json;

internal static class JsonEquivalencyAssertions
{
    public static void AssertBeEquivalentTo(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonInput expectedInput,
        string expectedArgumentName,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        using var expected = JsonParsedValue.ParseExpected(expectedInput, expectedArgumentName);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var expectedDisplay = new JsonDisplay(expected.DisplayText);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                new Expectation("to be JSON equivalent to", expectedDisplay),
                null,
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                new Expectation("to be JSON equivalent to", expectedDisplay),
                new JsonDisplay(JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, subject.InvalidDetail!)),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var mismatch = JsonEquivalencyComparer.FindFirstDifference(subject.Root, expected.Root, JsonPath.RootDisplayPath);
        if (mismatch is null)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            new Expectation("to be JSON equivalent to", expectedDisplay),
            new JsonDisplay(mismatch.Value.RenderActualDetail()),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotBeEquivalentTo(
        JsonInput subjectInput,
        string? subjectExpression,
        JsonInput unexpectedInput,
        string unexpectedArgumentName,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        using var unexpected = JsonParsedValue.ParseExpected(unexpectedInput, unexpectedArgumentName);
        using var subject = JsonParsedValue.ParseSubject(subjectInput);
        var subjectLabel = JsonAssertionSupport.SubjectLabel(subjectExpression);
        var unexpectedDisplay = new JsonDisplay(unexpected.DisplayText);

        if (!subject.HasValue)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                new Expectation("to not be JSON equivalent to", unexpectedDisplay),
                null,
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (!subject.IsValid)
        {
            JsonAssertionSupport.Fail(
                subjectLabel,
                new Expectation("to not be JSON equivalent to", unexpectedDisplay),
                new JsonDisplay(JsonAssertionSupport.DescribeInvalidSubjectJson(subjectLabel, subject.InvalidDetail!)),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var mismatch = JsonEquivalencyComparer.FindFirstDifference(subject.Root, unexpected.Root, JsonPath.RootDisplayPath);
        if (mismatch is not null)
        {
            return;
        }

        JsonAssertionSupport.Fail(
            subjectLabel,
            new Expectation("to not be JSON equivalent to", unexpectedDisplay),
            new JsonDisplay($"equivalent JSON {subject.DisplayText}"),
            because,
            callerFilePath,
            callerLineNumber);
    }
}
