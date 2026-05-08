using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.FluentAssertionsMigration;

internal sealed class FluentAssertionsMigrationMatch
{
    public FluentAssertionsMigrationMatch(
        FluentAssertionsMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace,
        bool useStringShouldExtension)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
        UseStringShouldExtension = useStringShouldExtension;
    }

    public FluentAssertionsMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
    public bool UseStringShouldExtension { get; }
}

internal static class FluentAssertionsMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        FluentAssertionsMigrationSymbols symbols,
        out FluentAssertionsMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !FluentAssertionsMigrationSymbols.IsFluentAssertionsAssertionMethod(invocation.TargetMethod) ||
            !HasOnlyPositionalArguments(invocationSyntax) ||
            IsResultConsumed(invocation) ||
            !TryGetDirectShouldChain(invocation, invocationSyntax, symbols, out var subjectExpression, out var subjectType))
        {
            return false;
        }

        foreach (var spec in FluentAssertionsMigrationSpecs.GetByMethodName(invocation.TargetMethod.Name))
        {
            if (invocationSyntax.ArgumentList.Arguments.Count != spec.RequiredArgumentCount ||
                !IsSafeSupportedOverload(invocation, spec.Kind, subjectType, symbols))
            {
                continue;
            }

            var typeArgumentSyntax = GetTypeArgumentSyntax(spec.Kind, invocationSyntax);
            if (spec.Kind is FluentAssertionsMigrationKind.BeOfType or FluentAssertionsMigrationKind.BeAssignableTo &&
                typeArgumentSyntax is null)
            {
                continue;
            }

            match = new FluentAssertionsMigrationMatch(
                spec,
                invocationSyntax,
                subjectExpression,
                GetExpectedExpression(invocationSyntax),
                typeArgumentSyntax,
                symbols.RequiresAssertionsExtensionsNamespace(spec.Kind, subjectType),
                subjectType.SpecialType == SpecialType.System_String);

            return true;
        }

        return false;
    }

    private static bool TryGetDirectShouldChain(
        IInvocationOperation assertionInvocation,
        InvocationExpressionSyntax assertionInvocationSyntax,
        FluentAssertionsMigrationSymbols symbols,
        out ExpressionSyntax subjectExpression,
        out ITypeSymbol subjectType)
    {
        subjectExpression = null!;
        subjectType = null!;

        if (assertionInvocationSyntax.Expression is not MemberAccessExpressionSyntax assertionAccess ||
            assertionAccess.Expression is not InvocationExpressionSyntax shouldInvocationSyntax ||
            shouldInvocationSyntax.Expression is not MemberAccessExpressionSyntax shouldAccess ||
            shouldAccess.Name.Identifier.ValueText != "Should")
        {
            return false;
        }

        subjectExpression = shouldAccess.Expression;
        var candidateSubjectType = GetSubjectType(UnwrapConversions(assertionInvocation.Instance)?.Type, symbols);
        if (candidateSubjectType is null)
        {
            return false;
        }

        subjectType = candidateSubjectType;
        return true;
    }

    private static IOperation? UnwrapConversions(IOperation? operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static ITypeSymbol? GetSubjectType(
        ITypeSymbol? assertionReceiverType,
        FluentAssertionsMigrationSymbols symbols)
    {
        return assertionReceiverType switch
        {
            INamedTypeSymbol { Name: "StringAssertions" }
                => symbols.Compilation.GetSpecialType(SpecialType.System_String),
            INamedTypeSymbol { Name: "BooleanAssertions" }
                => symbols.Compilation.GetSpecialType(SpecialType.System_Boolean),
            INamedTypeSymbol { Name: "ObjectAssertions", TypeArguments.Length: 1 } namedType
                => namedType.TypeArguments[0],
            _ => null,
        };
    }

    private static bool HasOnlyPositionalArguments(InvocationExpressionSyntax invocationSyntax)
    {
        foreach (var argument in invocationSyntax.ArgumentList.Arguments)
        {
            if (argument.NameColon is not null ||
                !argument.RefKindKeyword.IsKind(SyntaxKind.None))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        FluentAssertionsMigrationKind kind,
        ITypeSymbol subjectType,
        FluentAssertionsMigrationSymbols symbols)
    {
        if (HasUnsupportedTypeOrBecauseArguments(invocation))
        {
            return false;
        }

        return kind switch
        {
            FluentAssertionsMigrationKind.Be or FluentAssertionsMigrationKind.NotBe
                => symbols.SupportsEqualityMigrationReceiver(subjectType),
            FluentAssertionsMigrationKind.BeNull or FluentAssertionsMigrationKind.NotBeNull
                => symbols.SupportsNullMigrationReceiver(subjectType),
            FluentAssertionsMigrationKind.BeTrue or FluentAssertionsMigrationKind.BeFalse
                => subjectType.SpecialType == SpecialType.System_Boolean,
            FluentAssertionsMigrationKind.BeEmpty or FluentAssertionsMigrationKind.NotBeEmpty
                => symbols.SupportsEmptyMigrationReceiver(subjectType),
            FluentAssertionsMigrationKind.ContainSubstring or
            FluentAssertionsMigrationKind.NotContainSubstring or
            FluentAssertionsMigrationKind.StartWith or
            FluentAssertionsMigrationKind.EndWith
                => symbols.SupportsStringMigrationReceiver(subjectType),
            FluentAssertionsMigrationKind.BeSameAs or FluentAssertionsMigrationKind.NotBeSameAs
                => symbols.SupportsReferenceEqualityMigrationReceiver(subjectType),
            FluentAssertionsMigrationKind.BeOfType or FluentAssertionsMigrationKind.BeAssignableTo
                => symbols.SupportsTypeAssertionMigrationReceiver(subjectType) &&
                   invocation.TargetMethod.IsGenericMethod &&
                   invocation.TargetMethod.TypeArguments.Length == 1,
            _ => false,
        };
    }

    private static bool HasUnsupportedTypeOrBecauseArguments(IInvocationOperation invocation)
    {
        foreach (var argument in invocation.Arguments)
        {
            if (argument.IsImplicit)
            {
                continue;
            }

            if (argument.Parameter is null)
            {
                return true;
            }

            // FluentAssertions carries optional because/becauseArgs parameters on many assertions.
            // Only the assertion-value arguments map cleanly to Axiom here; any explanation or params
            // arguments are deliberately left as manual migration work.
            if (argument.Parameter.Name is "because" or "becauseArgs")
            {
                return true;
            }
        }

        return false;
    }

    private static ExpressionSyntax? GetExpectedExpression(InvocationExpressionSyntax invocationSyntax)
    {
        return invocationSyntax.ArgumentList.Arguments.Count == 1
            ? invocationSyntax.ArgumentList.Arguments[0].Expression
            : null;
    }

    private static TypeSyntax? GetTypeArgumentSyntax(
        FluentAssertionsMigrationKind kind,
        InvocationExpressionSyntax invocationSyntax)
    {
        if (kind is not FluentAssertionsMigrationKind.BeOfType and not FluentAssertionsMigrationKind.BeAssignableTo)
        {
            return null;
        }

        return invocationSyntax.Expression switch
        {
            MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax genericName
            } when genericName.TypeArgumentList.Arguments.Count == 1
                => genericName.TypeArgumentList.Arguments[0],
            _ => null,
        };
    }

    private static bool IsResultConsumed(IInvocationOperation invocation)
    {
        IOperation current = invocation;
        while (current.Parent is IConversionOperation conversion)
        {
            current = conversion;
        }

        return current.Parent is not IExpressionStatementOperation;
    }
}
