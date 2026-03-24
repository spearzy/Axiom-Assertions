using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AwaitAsyncAssertionResultAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor Rule = new(
        AxiomAnalyzerIds.AwaitAsyncAssertionResult,
        "Await or return async Axiom assertions",
        "The async Axiom assertion '{0}' is ignored; await or return it so the assertion runs",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Async Axiom assertions return ValueTask and must be awaited or returned. Dropping the result means the assertion never runs.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var symbols = AnalyzerSymbols.Create(startContext.Compilation);
            if (!symbols.IsEnabled)
            {
                return;
            }

            startContext.RegisterOperationAction(
                operationContext => AnalyzeExpressionStatement(operationContext, symbols),
                OperationKind.ExpressionStatement);

            startContext.RegisterOperationAction(
                operationContext => AnalyzeDiscardAssignment(operationContext, symbols),
                OperationKind.SimpleAssignment);
        });
    }

    private static void AnalyzeExpressionStatement(
        OperationAnalysisContext context,
        AnalyzerSymbols symbols)
    {
        var expressionStatement = (IExpressionStatementOperation)context.Operation;
        if (!TryGetInvocation(expressionStatement.Operation, out var invocation))
        {
            return;
        }

        ReportIfIgnoredAxiomAssertion(context, invocation, symbols);
    }

    private static void AnalyzeDiscardAssignment(
        OperationAnalysisContext context,
        AnalyzerSymbols symbols)
    {
        var assignment = (ISimpleAssignmentOperation)context.Operation;
        if (assignment.Target is not IDiscardOperation)
        {
            return;
        }

        if (!TryGetInvocation(assignment.Value, out var invocation))
        {
            return;
        }

        ReportIfIgnoredAxiomAssertion(context, invocation, symbols);
    }

    private static void ReportIfIgnoredAxiomAssertion(
        OperationAnalysisContext context,
        IInvocationOperation invocation,
        AnalyzerSymbols symbols)
    {
        if (!symbols.IsTargetMethod(invocation.TargetMethod))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            invocation.Syntax.GetLocation(),
            invocation.TargetMethod.Name));
    }

    private static bool TryGetInvocation(IOperation operation, out IInvocationOperation invocation)
    {
        switch (operation)
        {
            case IInvocationOperation directInvocation:
                invocation = directInvocation;
                return true;

            case IConversionOperation { IsImplicit: true } conversion:
                return TryGetInvocation(conversion.Operand, out invocation);

            default:
                invocation = null!;
                return false;
        }
    }

    private sealed class AnalyzerSymbols
    {
        private readonly ImmutableHashSet<INamedTypeSymbol> _targetTypes;
        private readonly INamedTypeSymbol? _valueTaskType;
        private readonly INamedTypeSymbol? _genericValueTaskType;

        private AnalyzerSymbols(
            INamedTypeSymbol? valueTaskType,
            INamedTypeSymbol? genericValueTaskType,
            ImmutableHashSet<INamedTypeSymbol> targetTypes)
        {
            _valueTaskType = valueTaskType;
            _genericValueTaskType = genericValueTaskType;
            _targetTypes = targetTypes;
        }

        public bool IsEnabled => _valueTaskType is not null && _genericValueTaskType is not null && _targetTypes.Count > 0;

        public static AnalyzerSymbols Create(Compilation compilation)
        {
            return new AnalyzerSymbols(
                compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask"),
                compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1"),
                CreateTargetTypes(compilation));
        }

        public bool IsTargetMethod(IMethodSymbol method)
        {
            if (!ReturnsValueTask(method.ReturnType))
            {
                return false;
            }

            var containingType = method.ContainingType?.OriginalDefinition;
            return containingType is not null && _targetTypes.Contains(containingType, SymbolEqualityComparer.Default);
        }

        private bool ReturnsValueTask(ITypeSymbol returnType)
        {
            if (returnType is not INamedTypeSymbol namedType)
            {
                return false;
            }

            var originalDefinition = namedType.OriginalDefinition;
            return SymbolEqualityComparer.Default.Equals(originalDefinition, _valueTaskType) ||
                   SymbolEqualityComparer.Default.Equals(originalDefinition, _genericValueTaskType);
        }

        private static ImmutableHashSet<INamedTypeSymbol> CreateTargetTypes(Compilation compilation)
        {
            var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            AddType(compilation, builder, "Axiom.Assertions.AssertionTypes.AsyncActionAssertions");
            AddType(compilation, builder, "Axiom.Assertions.AssertionTypes.AsyncFunctionAssertions`1");
            AddType(compilation, builder, "Axiom.Assertions.AssertionTypes.TaskAssertions");
            AddType(compilation, builder, "Axiom.Assertions.AssertionTypes.TaskAssertions`1");
            return builder.ToImmutable();
        }

        private static void AddType(
            Compilation compilation,
            ImmutableHashSet<INamedTypeSymbol>.Builder builder,
            string metadataName)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            if (symbol is not null)
            {
                builder.Add(symbol);
            }
        }
    }
}
