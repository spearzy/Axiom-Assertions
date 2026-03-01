using System.Text;

namespace Axiom.Core.Output;

public static class AssertionOutputRenderer
{
    private const string AnsiReset = "\u001b[0m";
    private const string AnsiGreen = "\u001b[32m";
    private const string AnsiRed = "\u001b[31m";
    private const string AnsiCyan = "\u001b[36m";
    private const string AnsiDim = "\u001b[2m";

    public static string RenderPass(
        string assertionName,
        string? subjectLabel,
        string? callerFilePath,
        int callerLineNumber,
        AssertionOutputOptions options)
    {
        var builder = new StringBuilder();
        builder.Append(Colourise("PASS", AnsiGreen, options.UseColours));
        builder.Append(' ');
        builder.Append(assertionName);
        builder.Append(' ');
        builder.Append(string.IsNullOrWhiteSpace(subjectLabel) ? "<subject>" : subjectLabel);

        AppendLocation(builder, callerFilePath, callerLineNumber, options.UseColours);
        return builder.ToString();
    }

    public static string RenderFailure(
        string failureMessage,
        string? callerFilePath,
        int callerLineNumber,
        AssertionOutputOptions options)
    {
        var builder = new StringBuilder();
        builder.Append(Colourise("FAIL", AnsiRed, options.UseColours));
        builder.Append(' ');
        builder.Append(failureMessage);

        AppendLocation(builder, callerFilePath, callerLineNumber, options.UseColours);
        if (options.IncludeSourceLine)
        {
            AppendSourceLine(builder, callerFilePath, callerLineNumber, options.UseColours);
        }

        return builder.ToString();
    }

    private static void AppendLocation(
        StringBuilder builder,
        string? callerFilePath,
        int callerLineNumber,
        bool useColours)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath) || callerLineNumber <= 0)
        {
            return;
        }

        builder.AppendLine();
        builder.Append(Colourise("  at ", AnsiDim, useColours));
        builder.Append(Colourise(Path.GetFileName(callerFilePath), AnsiCyan, useColours));
        builder.Append(':');
        builder.Append(callerLineNumber);
    }

    private static void AppendSourceLine(
        StringBuilder builder,
        string? callerFilePath,
        int callerLineNumber,
        bool useColours)
    {
        var sourceLine = TryReadSourceLine(callerFilePath, callerLineNumber);
        if (string.IsNullOrWhiteSpace(sourceLine))
        {
            return;
        }

        builder.AppendLine();
        builder.Append(Colourise("  > ", AnsiDim, useColours));
        builder.Append(sourceLine);
    }

    private static string? TryReadSourceLine(string? callerFilePath, int callerLineNumber)
    {
        if (string.IsNullOrWhiteSpace(callerFilePath) || callerLineNumber <= 0 || !File.Exists(callerFilePath))
        {
            return null;
        }

        try
        {
            using var reader = new StreamReader(callerFilePath);
            for (var currentLine = 1; currentLine < callerLineNumber; currentLine++)
            {
                if (reader.ReadLine() is null)
                {
                    return null;
                }
            }

            return reader.ReadLine()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    private static string Colourise(string value, string colour, bool useColours)
    {
        return useColours ? $"{colour}{value}{AnsiReset}" : value;
    }
}
