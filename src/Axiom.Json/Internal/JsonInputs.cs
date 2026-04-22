using System.Text.Json;

namespace Axiom.Json;

internal readonly record struct JsonInput(JsonInputKind Kind, string? RawJson, JsonDocument? Document, JsonElement Element, bool HasValue)
{
    public static JsonInput FromString(string? rawJson) => new(JsonInputKind.String, rawJson, null, default, rawJson is not null);

    public static JsonInput FromDocument(JsonDocument? document) => new(JsonInputKind.Document, null, document, default, document is not null);

    public static JsonInput FromElement(JsonElement element) => new(JsonInputKind.Element, null, null, element, true);

    public static JsonInput FromNullableElement(JsonElement? element)
        => element.HasValue ? FromElement(element.Value) : new JsonInput(JsonInputKind.Element, null, null, default, false);
}

internal enum JsonInputKind
{
    String,
    Document,
    Element,
}

internal sealed class JsonParsedValue : IDisposable
{
    private readonly JsonDocument? _ownedDocument;

    private JsonParsedValue(bool hasValue, bool isValid, JsonElement root, JsonDocument? ownedDocument, string? invalidDetail)
    {
        HasValue = hasValue;
        IsValid = isValid;
        Root = root;
        _ownedDocument = ownedDocument;
        InvalidDetail = invalidDetail;
        DisplayText = hasValue && isValid ? JsonSerializer.Serialize(root) : invalidDetail ?? "<null>";
    }

    public bool HasValue { get; }
    public bool IsValid { get; }
    public JsonElement Root { get; }
    public string? InvalidDetail { get; }
    public string DisplayText { get; }

    public static JsonParsedValue ParseSubject(JsonInput input)
    {
        return input.Kind switch
        {
            JsonInputKind.String => ParseSubjectString(input.RawJson),
            JsonInputKind.Document => input.Document is null
                ? new JsonParsedValue(false, true, default, null, null)
                : new JsonParsedValue(true, true, input.Document.RootElement, null, null),
            JsonInputKind.Element => ParseSubjectElement(input),
            _ => throw new InvalidOperationException($"Unsupported JSON input kind '{input.Kind}'.")
        };
    }

    public static JsonParsedValue ParseExpected(JsonInput input, string argumentName)
    {
        return input.Kind switch
        {
            JsonInputKind.String => ParseExpectedString(input.RawJson, argumentName),
            JsonInputKind.Document => input.Document is null
                ? throw new ArgumentNullException(argumentName)
                : new JsonParsedValue(true, true, input.Document.RootElement, null, null),
            JsonInputKind.Element => ParseExpectedElement(input, argumentName),
            _ => throw new InvalidOperationException($"Unsupported JSON input kind '{input.Kind}'.")
        };
    }

    public void Dispose()
    {
        _ownedDocument?.Dispose();
    }

    private static JsonParsedValue ParseSubjectString(string? rawJson)
    {
        if (rawJson is null)
        {
            return new JsonParsedValue(false, true, default, null, null);
        }

        try
        {
            var document = JsonDocument.Parse(rawJson);
            return new JsonParsedValue(true, true, document.RootElement, document, null);
        }
        catch (JsonException ex)
        {
            return new JsonParsedValue(true, false, default, null, BuildInvalidJsonDetail(ex));
        }
    }

    private static JsonParsedValue ParseExpectedString(string? rawJson, string argumentName)
    {
        ArgumentNullException.ThrowIfNull(rawJson, argumentName);

        try
        {
            var document = JsonDocument.Parse(rawJson);
            return new JsonParsedValue(true, true, document.RootElement, document, null);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"{argumentName} must be valid JSON ({BuildInvalidJsonDetail(ex)}).", argumentName);
        }
    }

    private static JsonParsedValue ParseSubjectElement(JsonInput input)
    {
        if (!input.HasValue)
        {
            return new JsonParsedValue(false, true, default, null, null);
        }

        return input.Element.ValueKind == JsonValueKind.Undefined
            ? new JsonParsedValue(true, false, default, null, "undefined JsonElement")
            : new JsonParsedValue(true, true, input.Element, null, null);
    }

    private static JsonParsedValue ParseExpectedElement(JsonInput input, string argumentName)
    {
        if (!input.HasValue)
        {
            throw new ArgumentNullException(argumentName);
        }

        return input.Element.ValueKind == JsonValueKind.Undefined
            ? throw new ArgumentException($"{argumentName} must not be an undefined JsonElement.", argumentName)
            : new JsonParsedValue(true, true, input.Element, null, null);
    }

    private static string BuildInvalidJsonDetail(JsonException exception)
    {
        var line = exception.LineNumber ?? 0;
        var bytePosition = exception.BytePositionInLine ?? 0;
        return $"invalid JSON at line {line}, byte {bytePosition}";
    }
}

internal readonly record struct JsonDisplay(string Text)
{
    public override string ToString() => Text;
}
