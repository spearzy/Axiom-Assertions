using System.Globalization;
using System.Text.Json;

namespace Axiom.Json;

internal static class JsonEquivalencyComparer
{
    public static JsonMismatch? FindFirstDifference(JsonElement actual, JsonElement expected, string path)
    {
        if (actual.ValueKind != expected.ValueKind)
        {
            return JsonMismatch.ValueKindMismatch(path, expected.ValueKind, actual.ValueKind);
        }

        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                return FindObjectDifference(actual, expected, path);
            case JsonValueKind.Array:
                return FindArrayDifference(actual, expected, path);
            case JsonValueKind.String:
                return actual.GetString() == expected.GetString()
                    ? null
                    : JsonMismatch.ValueMismatch(path, expected.GetRawText(), actual.GetRawText());
            case JsonValueKind.Number:
                return JsonNumberCanonicalizer.AreEquivalent(actual.GetRawText(), expected.GetRawText())
                    ? null
                    : JsonMismatch.ValueMismatch(path, expected.GetRawText(), actual.GetRawText());
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                return actual.GetRawText() == expected.GetRawText()
                    ? null
                    : JsonMismatch.ValueMismatch(path, expected.GetRawText(), actual.GetRawText());
            default:
                return JsonMismatch.ValueMismatch(path, expected.GetRawText(), actual.GetRawText());
        }
    }

    private static JsonMismatch? FindObjectDifference(JsonElement actual, JsonElement expected, string path)
    {
        var actualProperties = GroupProperties(actual);
        var expectedProperties = GroupProperties(expected);

        var expectedNames = expectedProperties.Keys.OrderBy(static name => name, StringComparer.Ordinal).ToArray();
        foreach (var propertyName in expectedNames)
        {
            var propertyPath = JsonPath.Append(path, JsonPathSegment.Property(propertyName));
            if (!actualProperties.TryGetValue(propertyName, out var actualValues))
            {
                return JsonMismatch.MissingProperty(propertyPath);
            }

            var expectedValues = expectedProperties[propertyName];
            var pairCount = Math.Min(expectedValues.Count, actualValues.Count);
            for (var i = 0; i < pairCount; i++)
            {
                var mismatch = FindFirstDifference(actualValues[i], expectedValues[i], propertyPath);
                if (mismatch is not null)
                {
                    return mismatch;
                }
            }

            if (expectedValues.Count > actualValues.Count)
            {
                return JsonMismatch.MissingProperty(propertyPath);
            }

            if (actualValues.Count > expectedValues.Count)
            {
                return JsonMismatch.ExtraProperty(propertyPath);
            }
        }

        var extraName = actualProperties.Keys
            .Where(name => !expectedProperties.ContainsKey(name))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .FirstOrDefault();

        return extraName is null ? null : JsonMismatch.ExtraProperty(JsonPath.Append(path, JsonPathSegment.Property(extraName)));
    }

    private static JsonMismatch? FindArrayDifference(JsonElement actual, JsonElement expected, string path)
    {
        var expectedLength = expected.GetArrayLength();
        var actualLength = actual.GetArrayLength();
        if (actualLength != expectedLength)
        {
            return JsonMismatch.ArrayLengthMismatch(path, expectedLength, actualLength);
        }

        var index = 0;
        using var expectedEnumerator = expected.EnumerateArray();
        using var actualEnumerator = actual.EnumerateArray();
        while (expectedEnumerator.MoveNext() && actualEnumerator.MoveNext())
        {
            var itemPath = JsonPath.Append(path, JsonPathSegment.Index(index));
            var mismatch = FindFirstDifference(actualEnumerator.Current, expectedEnumerator.Current, itemPath);
            if (mismatch is not null)
            {
                return mismatch.Value.Kind is JsonMismatchKind.ValueMismatch or JsonMismatchKind.ValueKindMismatch
                    && mismatch.Value.Path == itemPath
                    ? mismatch.Value.AsArrayItemMismatch()
                    : mismatch.Value;
            }

            index++;
        }

        return null;
    }

    private static Dictionary<string, List<JsonElement>> GroupProperties(JsonElement element)
    {
        var grouped = new Dictionary<string, List<JsonElement>>(StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            if (!grouped.TryGetValue(property.Name, out var values))
            {
                values = [];
                grouped[property.Name] = values;
            }

            values.Add(property.Value);
        }

        return grouped;
    }
}

internal readonly record struct JsonMismatch(JsonMismatchKind Kind, string Path, string Expected, string Actual)
{
    public static JsonMismatch MissingProperty(string path) => new(JsonMismatchKind.MissingProperty, path, string.Empty, string.Empty);

    public static JsonMismatch ExtraProperty(string path) => new(JsonMismatchKind.ExtraProperty, path, string.Empty, string.Empty);

    public static JsonMismatch ValueKindMismatch(string path, JsonValueKind expected, JsonValueKind actual)
        => new(JsonMismatchKind.ValueKindMismatch, path, JsonAssertionSupport.FormatValueKind(expected), JsonAssertionSupport.FormatValueKind(actual));

    public static JsonMismatch ValueMismatch(string path, string expected, string actual)
        => new(JsonMismatchKind.ValueMismatch, path, expected, actual);

    public static JsonMismatch ArrayLengthMismatch(string path, int expectedLength, int actualLength)
        => new(
            JsonMismatchKind.ArrayLengthMismatch,
            path,
            expectedLength.ToString(CultureInfo.InvariantCulture),
            actualLength.ToString(CultureInfo.InvariantCulture));

    public JsonMismatch AsArrayItemMismatch() => this with { Kind = JsonMismatchKind.ArrayItemMismatch };

    public string RenderActualDetail()
    {
        return Kind switch
        {
            JsonMismatchKind.MissingProperty => $"missing property {Path}",
            JsonMismatchKind.ExtraProperty => $"extra property {Path}",
            JsonMismatchKind.ValueKindMismatch => $"JSON value kind mismatch at {Path}: expected {Expected} but found {Actual}",
            JsonMismatchKind.ValueMismatch => $"JSON value mismatch at {Path}: expected {Expected} but found {Actual}",
            JsonMismatchKind.ArrayItemMismatch => $"JSON array item mismatch at {Path}: expected {Expected} but found {Actual}",
            JsonMismatchKind.ArrayLengthMismatch => $"JSON array length mismatch at {Path}: expected {Expected} but found {Actual}",
            _ => throw new InvalidOperationException($"Unsupported JSON mismatch kind '{Kind}'.")
        };
    }
}

internal enum JsonMismatchKind
{
    MissingProperty,
    ExtraProperty,
    ValueKindMismatch,
    ValueMismatch,
    ArrayItemMismatch,
    ArrayLengthMismatch,
}
