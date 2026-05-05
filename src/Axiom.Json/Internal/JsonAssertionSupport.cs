using System.Text.Json;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Json;

internal static class JsonAssertionSupport
{
    public static void Fail(
        string subjectLabel,
        Expectation expectation,
        object? actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var failure = new Failure(subjectLabel, expectation, actual, because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static string SubjectLabel(string? subjectExpression) => string.IsNullOrWhiteSpace(subjectExpression) ? "value" : subjectExpression;

    public static string DescribeInvalidSubjectJson(string subjectLabel, string detail)
    {
        return subjectLabel is "value" or "actual"
            ? $"invalid subject JSON ({detail})"
            : $"invalid JSON in {subjectLabel} ({detail})";
    }

    public static string DescribeWrongValueKind(JsonElement element, string path, string expectedKind)
        => $"{DescribeElement(element)} at {path}; expected {expectedKind.ToLowerInvariant()}";

    public static string DescribeElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => $"JSON string {element.GetRawText()}",
            JsonValueKind.Number => $"JSON number {element.GetRawText()}",
            JsonValueKind.True or JsonValueKind.False => $"JSON boolean {element.GetRawText()}",
            JsonValueKind.Null => "JSON null",
            JsonValueKind.Object => "JSON object",
            JsonValueKind.Array => "JSON array",
            _ => $"JSON {FormatValueKind(element.ValueKind).ToLowerInvariant()}"
        };
    }

    public static string FormatValue(object? value) => AxiomServices.Configuration.ValueFormatter.Format(value);

    public static string FormatValueKind(JsonValueKind valueKind)
        => valueKind switch
        {
            JsonValueKind.True or JsonValueKind.False => "boolean",
            _ => valueKind.ToString().ToLowerInvariant()
        };
}
