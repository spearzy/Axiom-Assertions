using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public sealed class AsyncActionAssertions
{
    public AsyncActionAssertions(Func<ValueTask> subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public Func<ValueTask> Subject { get; }
    public string? SubjectExpression { get; }

    public async ValueTask<AndContinuation<AsyncActionAssertions>> ThrowAsync<TException>(string? because = null)
        where TException : Exception
    {
        Exception? capturedException = null;
        try
        {
            await Subject().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }

        if (capturedException is TException)
        {
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
        Fail(FailureMessageRenderer.Render(failure));

        return new AndContinuation<AsyncActionAssertions>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static void Fail(string message)
    {
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
