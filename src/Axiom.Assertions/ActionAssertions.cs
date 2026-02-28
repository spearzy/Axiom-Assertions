using Axiom.Core;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public sealed class ActionAssertions
{
    public ActionAssertions(Action subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public Action Subject { get; }
    public string? SubjectExpression { get; }

    public AndContinuation<ActionAssertions> Throw<TException>(string? because = null)
        where TException : Exception
    {
        Exception? capturedException = null;
        try
        {
            // Execute once and capture what happened so we can evaluate it deterministically.
            Subject();
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }

        // Accept the requested type or any subtype (common assertion-library expectation).
        if (capturedException is TException)
        {
            return new AndContinuation<ActionAssertions>(this);
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

        return new AndContinuation<ActionAssertions>(this);
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
            // In a batch we aggregate; root batch dispose will throw one combined exception.
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
