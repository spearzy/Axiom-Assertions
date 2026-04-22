using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Axiom.Json;

public static class ShouldExtensions
{
    public static JsonAssertions Should(
        this JsonDocument? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(JsonInput.FromDocument(subject), subjectExpression);

    public static JsonAssertions Should(
        this JsonElement subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(JsonInput.FromElement(subject), subjectExpression);

    public static JsonAssertions Should(
        this JsonElement? subject,
        [CallerArgumentExpression("subject")] string? subjectExpression = null)
        => new(JsonInput.FromNullableElement(subject), subjectExpression);
}
