using Axiom.Analyzers.NunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class NunitAsyncThrowsMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(NunitAssertMigrationMatch match)
    {
        var actionExpression = SyntaxFactory.ObjectCreationExpression(
            SyntaxFactory.ParseTypeName("Func<Task>"),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(match.SubjectExpression.WithoutTrivia()))),
            initializer: null);

        var rewrittenExpression = NunitAssertMigrationCodeFixProvider.BuildShouldCall(
            actionExpression,
            GetMethodName(match.Spec.Kind),
            typeArgumentSyntax: match.TypeArgumentSyntax);

        ExpressionSyntax replacementExpression = SyntaxFactory.AwaitExpression(rewrittenExpression);
        if (match.AppendThrown)
        {
            replacementExpression = NunitAssertMigrationCodeFixProvider.AppendMemberAccess(
                SyntaxFactory.ParenthesizedExpression(replacementExpression),
                "Thrown");
        }

        return replacementExpression;
    }

    public static string GetCodeFixTitle(NunitAssertMigrationMatch match)
    {
        var methodName = GetMethodName(match.Spec.Kind);
        return match.AppendThrown
            ? $"Convert to '(await ...Should().{methodName}<TException>()).Thrown'"
            : match.Spec.CodeFixTitle;
    }

    private static string GetMethodName(NunitAssertMigrationKind kind)
    {
        return kind switch
        {
            NunitAssertMigrationKind.ThrowExactlyAsync => "ThrowExactlyAsync",
            NunitAssertMigrationKind.ThrowAsync => "ThrowAsync",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
