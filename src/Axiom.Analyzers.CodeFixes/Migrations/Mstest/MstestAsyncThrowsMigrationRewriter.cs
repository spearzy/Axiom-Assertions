using Axiom.Analyzers.MstestMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class MstestAsyncThrowsMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(
        MstestAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        var actionExpression = CanUseDirectThrowAsyncReceiver(match.SubjectExpression, semanticModel)
            ? MstestAssertMigrationCodeFixProvider.PrepareSubjectExpression(match.SubjectExpression)
            : SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.ParseTypeName("Func<Task>"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.SubjectExpression.WithoutTrivia()))),
                initializer: null);

        var rewrittenExpression = MstestAssertMigrationCodeFixProvider.BuildShouldCall(
            actionExpression,
            GetMethodName(match.Spec.Kind),
            typeArgumentSyntax: match.TypeArgumentSyntax);

        ExpressionSyntax replacementExpression = SyntaxFactory.AwaitExpression(rewrittenExpression);
        if (match.AppendThrown)
        {
            replacementExpression = MstestAssertMigrationCodeFixProvider.AppendMemberAccess(
                SyntaxFactory.ParenthesizedExpression(replacementExpression),
                "Thrown");
        }

        return replacementExpression;
    }

    public static string GetCodeFixTitle(MstestAssertMigrationMatch match)
    {
        var methodName = GetMethodName(match.Spec.Kind);
        return match.AppendThrown
            ? $"Convert to '(await ...Should().{methodName}<TException>()).Thrown'"
            : match.Spec.CodeFixTitle;
    }

    public static bool RequiresSystemAndTaskNamespaces(
        MstestAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return !CanUseDirectThrowAsyncReceiver(match.SubjectExpression, semanticModel);
    }

    private static string GetMethodName(MstestAssertMigrationKind kind)
    {
        return kind switch
        {
            MstestAssertMigrationKind.ThrowExactlyAsync => "ThrowExactlyAsync",
            MstestAssertMigrationKind.ThrowAsync => "ThrowAsync",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    private static bool CanUseDirectThrowAsyncReceiver(
        ExpressionSyntax subjectExpression,
        SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(subjectExpression);
        if (symbolInfo.Symbol is IMethodSymbol)
        {
            return false;
        }

        foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
        {
            if (candidateSymbol is IMethodSymbol)
            {
                return false;
            }
        }

        var typeInfo = semanticModel.GetTypeInfo(subjectExpression);
        var funcType = semanticModel.Compilation.GetTypeByMetadataName("System.Func`1");
        var taskType = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        if (funcType is null || taskType is null)
        {
            return false;
        }

        var funcOfTaskType = funcType.Construct(taskType);
        return SymbolEqualityComparer.Default.Equals(typeInfo.Type, funcOfTaskType) ||
               SymbolEqualityComparer.Default.Equals(typeInfo.ConvertedType, funcOfTaskType);
    }
}
