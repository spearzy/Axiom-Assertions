using System.Runtime.CompilerServices;
using Axiom.Assertions.Chaining;
using Axiom.Core;
using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Assertions.AssertionTypes;

public sealed class AsyncActionAssertions(Func<ValueTask> subject, string? subjectExpression)
{
    public Func<ValueTask> Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public async ValueTask<AndContinuation<AsyncActionAssertions>> ThrowAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);

        if (capturedException is TException)
        {
            AssertionOutputWriter.ReportPass(nameof(ThrowAsync), SubjectLabel(), callerFilePath, callerLineNumber);
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw", typeof(TException)),
            actual,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> ThrowExactlyAsync<TException>(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TException : Exception
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);
        if (capturedException?.GetType() == typeof(TException))
        {
            AssertionOutputWriter.ReportPass(nameof(ThrowExactlyAsync), SubjectLabel(), callerFilePath, callerLineNumber);
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        object actual = capturedException is null
            ? NoExceptionToken.Instance
            : capturedException.GetType();

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to throw exactly", typeof(TException)),
            actual,
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> NotThrowAsync(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var capturedException = await CaptureExceptionAsync().ConfigureAwait(false);
        if (capturedException is null)
        {
            AssertionOutputWriter.ReportPass(nameof(NotThrowAsync), SubjectLabel(), callerFilePath, callerLineNumber);
            return new AndContinuation<AsyncActionAssertions>(this);
        }

        var failure = new Failure(
            SubjectLabel(),
            new Expectation("to not throw", IncludeExpectedValue: false),
            capturedException.GetType(),
            because);
        Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    private async ValueTask<Exception?> CaptureExceptionAsync()
    {
        try
        {
            await Subject().ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static void Fail(string message, string? callerFilePath, int callerLineNumber)
    {
        AssertionOutputWriter.ReportFailure(message, callerFilePath, callerLineNumber);

        var batch = Batch.Current;
        if (batch is not null)
        {
            // Keep going inside a batch; root dispose raises one combined exception.
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }

    private sealed class NoExceptionToken
    {
        public static NoExceptionToken Instance { get; } = new();

        public override string ToString()
        {
            return "<no exception>";
        }
    }
}
