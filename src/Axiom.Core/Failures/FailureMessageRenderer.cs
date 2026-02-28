using Axiom.Core.Configuration;
using Axiom.Core.Formatting;

namespace Axiom.Core.Failures;

public static class FailureMessageRenderer
{
    public static string Render(Failure failure, IValueFormatter? formatter = null)
    {
        var valueFormatter = formatter ?? AxiomServices.Configuration.ValueFormatter;
        var expectation = failure.Expectation;
        var reasonClause = RenderReasonClause(failure.Reason);

        if (expectation.IncludeExpectedValue)
        {
            return $"Expected {failure.Subject} {expectation.Description} {valueFormatter.Format(expectation.Expected)}{reasonClause}, but found {valueFormatter.Format(failure.Actual)}.";
        }

        return $"Expected {failure.Subject} {expectation.Description}{reasonClause}, but found {valueFormatter.Format(failure.Actual)}.";
    }

    private static string RenderReasonClause(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return string.Empty;
        }

        return $" because {reason.Trim()}";
    }
}
