using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AwaitAsyncAssertionResultCodeFixProvider)), Shared]
public sealed class AwaitAsyncAssertionResultCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AxiomAnalyzerIds.AwaitAsyncAssertionResult];

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        if (root.FindNode(context.Span, getInnermostNodeForTie: true) is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (invocation.FirstAncestorOrSelf<ExpressionStatementSyntax>() is not ExpressionStatementSyntax statement)
        {
            return;
        }

        if (!SupportsAwaitInsertion(statement))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Await async Axiom assertion",
                cancellationToken => AddAwaitAsync(context.Document, statement, invocation, cancellationToken),
                equivalenceKey: "AwaitAsyncAxiomAssertion"),
            context.Diagnostics);
    }

    private static bool SupportsAwaitInsertion(StatementSyntax statement)
    {
        for (SyntaxNode? current = statement.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    return methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword);

                case LocalFunctionStatementSyntax localFunction:
                    return localFunction.Modifiers.Any(SyntaxKind.AsyncKeyword);

                case AnonymousFunctionExpressionSyntax anonymousFunction:
                    return anonymousFunction.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);
            }
        }

        return false;
    }

    private static async Task<Document> AddAwaitAsync(
        Document document,
        ExpressionStatementSyntax statement,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var awaitExpression = SyntaxFactory.AwaitExpression(invocation.WithoutTrivia());
        StatementSyntax replacement = statement.Expression switch
        {
            AssignmentExpressionSyntax { Left: IdentifierNameSyntax { Identifier.ValueText: "_" } } =>
                SyntaxFactory.ExpressionStatement(awaitExpression),
            _ => statement.WithExpression(awaitExpression),
        };

        replacement = replacement.WithTriviaFrom(statement);
        var newRoot = root.ReplaceNode(statement, replacement);
        return document.WithSyntaxRoot(newRoot);
    }
}
