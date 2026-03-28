using System.Collections.Immutable;
using System.Composition;
using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XunitAssertMigrationCodeFixProvider)), Shared]
public sealed class XunitAssertMigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        XunitAssertMigrationSpecs.All.Select(static spec => spec.DiagnosticId).ToImmutableArray();

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

        if (root.FindNode(context.Span, getInnermostNodeForTie: true) is not InvocationExpressionSyntax invocationSyntax)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel is null ||
            semanticModel.GetOperation(invocationSyntax, context.CancellationToken) is not IInvocationOperation invocationOperation)
        {
            return;
        }

        var symbols = XunitAssertMigrationSymbols.Create(semanticModel.Compilation);
        if (!XunitAssertMigrationMatcher.TryMatch(invocationOperation, symbols, out var match))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                match.Spec.CodeFixTitle,
                cancellationToken => ApplyFixAsync(context.Document, match, cancellationToken),
                equivalenceKey: match.Spec.DiagnosticId),
            context.Diagnostics);
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        XunitAssertMigrationMatch match,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var replacementExpression = BuildReplacementExpression(match)
            .WithTriviaFrom(match.InvocationSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var rewrittenRoot = compilationUnit.ReplaceNode(match.InvocationSyntax, replacementExpression);
        rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions");

        if (match.RequiresAssertionsExtensionsNamespace)
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions.Extensions");
        }

        return document.WithSyntaxRoot(rewrittenRoot);
    }

    private static CompilationUnitSyntax AddUsingIfMissing(
        CompilationUnitSyntax compilationUnit,
        string namespaceName)
    {
        foreach (var usingDirective in compilationUnit.Usings)
        {
            if (usingDirective.Alias is null &&
                !usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) &&
                usingDirective.Name?.ToString() == namespaceName)
            {
                return compilationUnit;
            }
        }

        return compilationUnit.AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))
                .WithAdditionalAnnotations(Formatter.Annotation));
    }

    private static ExpressionSyntax BuildReplacementExpression(XunitAssertMigrationMatch match)
    {
        var subjectExpression = PrepareSubjectExpression(match.SubjectExpression);
        var shouldInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                subjectExpression,
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList());

        var assertionMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            SyntaxFactory.IdentifierName(GetAxiomMethodName(match.Spec.Kind)));

        return match.ExpectedExpression is null
            ? SyntaxFactory.InvocationExpression(assertionMethod, SyntaxFactory.ArgumentList())
            : SyntaxFactory.InvocationExpression(
                assertionMethod,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.ExpectedExpression.WithoutTrivia()))));
    }

    private static ExpressionSyntax PrepareSubjectExpression(ExpressionSyntax subjectExpression)
    {
        var withoutTrivia = subjectExpression.WithoutTrivia();
        if (NeedsParentheses(withoutTrivia))
        {
            return SyntaxFactory.ParenthesizedExpression(withoutTrivia);
        }

        return withoutTrivia;
    }

    private static bool NeedsParentheses(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax => false,
            GenericNameSyntax => false,
            MemberAccessExpressionSyntax => false,
            ElementAccessExpressionSyntax => false,
            InvocationExpressionSyntax => false,
            ObjectCreationExpressionSyntax => false,
            ImplicitObjectCreationExpressionSyntax => false,
            ThisExpressionSyntax => false,
            BaseExpressionSyntax => false,
            LiteralExpressionSyntax => false,
            ParenthesizedExpressionSyntax => false,
            _ => true,
        };
    }

    private static string GetAxiomMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Be => "Be",
            XunitAssertMigrationKind.NotBe => "NotBe",
            XunitAssertMigrationKind.BeNull => "BeNull",
            XunitAssertMigrationKind.NotBeNull => "NotBeNull",
            XunitAssertMigrationKind.BeTrue => "BeTrue",
            XunitAssertMigrationKind.BeFalse => "BeFalse",
            XunitAssertMigrationKind.BeEmpty => "BeEmpty",
            XunitAssertMigrationKind.NotBeEmpty => "NotBeEmpty",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
