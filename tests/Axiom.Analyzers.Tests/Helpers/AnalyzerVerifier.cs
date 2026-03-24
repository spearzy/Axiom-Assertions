using Axiom.Analyzers.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Axiom.Analyzers.Tests.Helpers;

internal static class AnalyzerVerifier
{
    public static async Task VerifyAnalyzerAsync(string source)
    {
        var test = new CSharpAnalyzerTest<AwaitAsyncAssertionResultAnalyzer, XUnitVerifier>
        {
            TestCode = source,
        };

        test.TestState.Sources.Add(AxiomAssertionStubs.Source);
        await test.RunAsync();
    }

    public static async Task VerifyCodeFixAsync(string source, string fixedSource)
    {
        var test = new CSharpCodeFixTest<AwaitAsyncAssertionResultAnalyzer, AwaitAsyncAssertionResultCodeFixProvider, XUnitVerifier>
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.TestState.Sources.Add(AxiomAssertionStubs.Source);
        test.FixedState.Sources.Add(AxiomAssertionStubs.Source);
        await test.RunAsync();
    }
}
