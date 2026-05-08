using System.Collections.Immutable;
using Axiom.Analyzers.FluentAssertionsMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FluentAssertionsMigrationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => FluentAssertionsMigrationSpecs.SupportedDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = FluentAssertionsMigrationSymbols.Create(startContext.Compilation);
            if (!symbols.IsEnabled)
            {
                return;
            }

            startContext.RegisterOperationAction(
                operationContext => AnalyzeInvocation(operationContext, symbols),
                OperationKind.Invocation);
        });
    }

    private static void AnalyzeInvocation(
        OperationAnalysisContext context,
        FluentAssertionsMigrationSymbols symbols)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (!FluentAssertionsMigrationMatcher.TryMatch(invocation, symbols, out var match))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(match.Spec.Rule, invocation.Syntax.GetLocation()));
    }
}
