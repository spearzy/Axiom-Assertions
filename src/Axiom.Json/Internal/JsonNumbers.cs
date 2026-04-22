using System.Globalization;
using System.Text.Json;

namespace Axiom.Json;

internal readonly record struct JsonNumberExpectation(string CanonicalText, string DisplayText)
{
    public static JsonNumberExpectation FromDecimal(decimal value)
    {
        var serialized = JsonSerializer.Serialize(value);
        return new JsonNumberExpectation(JsonNumberCanonicalizer.Normalize(serialized), serialized);
    }

    public static JsonNumberExpectation FromDouble(double value)
    {
        var serialized = JsonSerializer.Serialize(value);
        return new JsonNumberExpectation(JsonNumberCanonicalizer.Normalize(serialized), serialized);
    }
}

internal static class JsonNumberCanonicalizer
{
    public static bool AreEquivalent(string left, string right) => Normalize(left) == Normalize(right);

    public static string Normalize(string rawNumber)
    {
        ArgumentNullException.ThrowIfNull(rawNumber);

        var index = 0;
        var isNegative = rawNumber[index] == '-';
        if (isNegative)
        {
            index++;
        }

        var integerStart = index;
        while (index < rawNumber.Length && char.IsDigit(rawNumber[index]))
        {
            index++;
        }

        var integerDigits = rawNumber[integerStart..index];
        var fractionDigits = string.Empty;
        if (index < rawNumber.Length && rawNumber[index] == '.')
        {
            index++;
            var fractionStart = index;
            while (index < rawNumber.Length && char.IsDigit(rawNumber[index]))
            {
                index++;
            }

            fractionDigits = rawNumber[fractionStart..index];
        }

        var exponent = 0;
        if (index < rawNumber.Length && (rawNumber[index] == 'e' || rawNumber[index] == 'E'))
        {
            index++;
            var exponentSign = 1;
            if (rawNumber[index] == '+')
            {
                index++;
            }
            else if (rawNumber[index] == '-')
            {
                exponentSign = -1;
                index++;
            }

            var exponentDigits = rawNumber[index..];
            exponent = exponentSign * int.Parse(exponentDigits, CultureInfo.InvariantCulture);
        }

        var digits = (integerDigits + fractionDigits).TrimStart('0');
        if (digits.Length == 0)
        {
            return "0";
        }

        var scale = fractionDigits.Length - exponent;
        string normalized;
        if (scale <= 0)
        {
            normalized = digits + new string('0', -scale);
        }
        else if (digits.Length > scale)
        {
            var splitIndex = digits.Length - scale;
            normalized = digits[..splitIndex] + "." + digits[splitIndex..];
            normalized = normalized.TrimEnd('0').TrimEnd('.');
        }
        else
        {
            normalized = "0." + new string('0', scale - digits.Length) + digits;
            normalized = normalized.TrimEnd('0').TrimEnd('.');
        }

        if (normalized == "0")
        {
            return normalized;
        }

        return isNegative ? "-" + normalized : normalized;
    }
}
