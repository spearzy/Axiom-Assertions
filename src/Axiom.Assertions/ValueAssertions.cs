using Axiom.Core;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions;

public sealed class ValueAssertions<T>
{
    public ValueAssertions(T subject, string? subjectExpression)
    {
        Subject = subject;
        SubjectExpression = subjectExpression;
    }

    public T Subject { get; }
    public string? SubjectExpression { get; }

    public AndContinuation<ValueAssertions<T>> Be(T expected, string? because = null)
    {
        var comparer = GetComparer();
        if (!comparer.Equals(Subject, expected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be", expected),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    public AndContinuation<ValueAssertions<T>> NotBe(T unexpected, string? because = null)
    {
        var comparer = GetComparer();
        if (comparer.Equals(Subject, unexpected))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be", unexpected),
                Subject,
                because);
            Fail(FailureMessageRenderer.Render(failure));
        }

        return new AndContinuation<ValueAssertions<T>>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static IEqualityComparer<T> GetComparer()
    {
        // Prefer configured provider; fall back to runtime default if provider declines.
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<T>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<T>.Default;
    }

    private static void Fail(string message)
    {
        var batch = Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        throw new InvalidOperationException(message);
    }
}
