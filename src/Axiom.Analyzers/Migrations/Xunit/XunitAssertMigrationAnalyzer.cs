using System.Collections.Immutable;
using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class XunitAssertMigrationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => XunitAssertMigrationSpecs.SupportedDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = XunitAssertMigrationSymbols.Create(startContext.Compilation);
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
        XunitAssertMigrationSymbols symbols)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (!XunitAssertMigrationMatcher.TryMatch(invocation, symbols, out var match))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(match.Spec.Rule, invocation.Syntax.GetLocation()));
    }
}
