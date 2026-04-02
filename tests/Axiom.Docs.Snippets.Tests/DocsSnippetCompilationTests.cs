namespace Axiom.Docs.Snippets.Tests;

public sealed class DocsSnippetCompilationTests
{
    private static readonly DocsSnippetCompiler Compiler = new();

    public static IEnumerable<object[]> RelevantSnippets()
    {
        return DocsSnippetExtractor.RelevantSnippets.Select(snippet => new object[] { snippet });
    }

    [Theory]
    [MemberData(nameof(RelevantSnippets))]
    public void RelevantSnippet_CompilesAgainstCurrentCodebase(DocsSnippet snippet)
    {
        // One path verifies every relevant docs snippet so failures stay consistent.
        var result = Compiler.Compile(snippet);
        Assert.True(result.Success, result.FailureMessage);
    }
}
