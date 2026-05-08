using System.Collections.Immutable;
using System.Composition;
using Axiom.Analyzers.FluentAssertionsMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FluentAssertionsMigrationCodeFixProvider)), Shared]
public sealed class FluentAssertionsMigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        FluentAssertionsMigrationSpecs.All.Select(static spec => spec.DiagnosticId).ToImmutableArray();

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

        var symbols = FluentAssertionsMigrationSymbols.Create(semanticModel.Compilation);
        if (!FluentAssertionsMigrationMatcher.TryMatch(invocationOperation, symbols, out var match))
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
        FluentAssertionsMigrationMatch match,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return document;
        }

        var hasOtherFluentAssertionsUsages = HasOtherFluentAssertionsUsages(compilationUnit, match, semanticModel, cancellationToken);
        var replacementExpression = BuildReplacementExpression(match, qualifyShouldCall: hasOtherFluentAssertionsUsages)
            .WithTriviaFrom(match.InvocationSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var rewrittenRoot = compilationUnit.ReplaceNode(match.InvocationSyntax, replacementExpression);
        rewrittenRoot = hasOtherFluentAssertionsUsages
            ? rewrittenRoot
            : RemoveUsingIfPresent(AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions"), "FluentAssertions");

        if (match.RequiresAssertionsExtensionsNamespace)
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions.Extensions");
        }

        return document.WithSyntaxRoot(rewrittenRoot);
    }

    private static ExpressionSyntax BuildReplacementExpression(
        FluentAssertionsMigrationMatch match,
        bool qualifyShouldCall)
    {
        var shouldInvocation = qualifyShouldCall
            ? BuildQualifiedShouldInvocation(match)
            : SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    PrepareSubjectExpression(match.SubjectExpression),
                    SyntaxFactory.IdentifierName("Should")),
                SyntaxFactory.ArgumentList());

        var assertionMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            BuildMethodName(GetMethodName(match.Spec.Kind), match.TypeArgumentSyntax));

        return match.ExpectedExpression is null
            ? SyntaxFactory.InvocationExpression(assertionMethod, SyntaxFactory.ArgumentList())
            : SyntaxFactory.InvocationExpression(
                assertionMethod,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.ExpectedExpression.WithoutTrivia()))));
    }

    private static InvocationExpressionSyntax BuildQualifiedShouldInvocation(FluentAssertionsMigrationMatch match)
    {
        var extensionType = match.UseStringShouldExtension
            ? "Axiom.Assertions.ShouldExtensions"
            : "Axiom.Assertions.GenericShouldExtensions";

        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ParseName(extensionType),
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(PrepareSubjectExpression(match.SubjectExpression)))));
    }

    private static string GetMethodName(FluentAssertionsMigrationKind kind)
    {
        return kind switch
        {
            FluentAssertionsMigrationKind.Be => "Be",
            FluentAssertionsMigrationKind.NotBe => "NotBe",
            FluentAssertionsMigrationKind.BeNull => "BeNull",
            FluentAssertionsMigrationKind.NotBeNull => "NotBeNull",
            FluentAssertionsMigrationKind.BeTrue => "BeTrue",
            FluentAssertionsMigrationKind.BeFalse => "BeFalse",
            FluentAssertionsMigrationKind.BeEmpty => "BeEmpty",
            FluentAssertionsMigrationKind.NotBeEmpty => "NotBeEmpty",
            FluentAssertionsMigrationKind.ContainSubstring => "Contain",
            FluentAssertionsMigrationKind.NotContainSubstring => "NotContain",
            FluentAssertionsMigrationKind.StartWith => "StartWith",
            FluentAssertionsMigrationKind.EndWith => "EndWith",
            FluentAssertionsMigrationKind.BeSameAs => "BeSameAs",
            FluentAssertionsMigrationKind.NotBeSameAs => "NotBeSameAs",
            FluentAssertionsMigrationKind.BeOfType => "BeOfType",
            FluentAssertionsMigrationKind.BeAssignableTo => "BeAssignableTo",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    private static SimpleNameSyntax BuildMethodName(
        string methodName,
        TypeSyntax? typeArgumentSyntax)
    {
        if (typeArgumentSyntax is null)
        {
            return SyntaxFactory.IdentifierName(methodName);
        }

        return SyntaxFactory.GenericName(
            SyntaxFactory.Identifier(methodName),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList(typeArgumentSyntax.WithoutTrivia())));
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

    private static CompilationUnitSyntax RemoveUsingIfPresent(
        CompilationUnitSyntax compilationUnit,
        string namespaceName)
    {
        foreach (var usingDirective in compilationUnit.Usings)
        {
            if (usingDirective.Alias is null &&
                !usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) &&
                usingDirective.Name?.ToString() == namespaceName)
            {
                return compilationUnit.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia) ?? compilationUnit;
            }
        }

        return compilationUnit;
    }

    private static bool HasOtherFluentAssertionsUsages(
        CompilationUnitSyntax compilationUnit,
        FluentAssertionsMigrationMatch match,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        foreach (var node in compilationUnit.DescendantNodes())
        {
            if (match.InvocationSyntax.Span.Contains(node.Span) || node is UsingDirectiveSyntax)
            {
                continue;
            }

            if (semanticModel.GetSymbolInfo(node, cancellationToken).Symbol is { } symbol &&
                IsFluentAssertionsSymbol(symbol))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFluentAssertionsSymbol(ISymbol symbol)
    {
        var containingNamespace = symbol switch
        {
            INamedTypeSymbol namedType => namedType.ContainingNamespace?.ToDisplayString(),
            IMethodSymbol method => method.ContainingType?.ContainingNamespace?.ToDisplayString(),
            IPropertySymbol property => property.ContainingType?.ContainingNamespace?.ToDisplayString(),
            IFieldSymbol field => field.ContainingType?.ContainingNamespace?.ToDisplayString(),
            _ => symbol.ContainingNamespace?.ToDisplayString(),
        };

        return containingNamespace is not null &&
               (containingNamespace == "FluentAssertions" ||
                containingNamespace.StartsWith("FluentAssertions.", StringComparison.Ordinal));
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
}
