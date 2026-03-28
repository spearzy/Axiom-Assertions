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
                operationContext => AnalyzeExpressionStatement(operationContext, symbols),
                OperationKind.ExpressionStatement);
        });
    }

    private static void AnalyzeExpressionStatement(
        OperationAnalysisContext context,
        XunitAssertMigrationSymbols symbols)
    {
        var statement = (IExpressionStatementOperation)context.Operation;
        if (statement.Operation is not IInvocationOperation invocation ||
            !XunitAssertMigrationMatcher.TryMatch(invocation, symbols, out var match))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(match.Spec.Rule, invocation.Syntax.GetLocation()));
    }
}
