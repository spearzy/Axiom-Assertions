using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.MstestMigration;

internal sealed class MstestAssertMigrationMatch
{
    public MstestAssertMigrationMatch(
        MstestAssertMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
    }

    public MstestAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
}

internal static class MstestAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols,
        out MstestAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        foreach (var spec in MstestAssertMigrationSpecs.GetByMethodName(invocation.TargetMethod.Name))
        {
            if (!symbols.IsSupportedAssertType(invocation.TargetMethod.ContainingType, spec.Target) ||
                invocation.TargetMethod.Parameters.Length != spec.RequiredArgumentCount ||
                invocation.Arguments.Length != spec.RequiredArgumentCount ||
                invocationSyntax.ArgumentList.Arguments.Count != spec.RequiredArgumentCount ||
                !IsSupportedOverload(invocation, spec.Kind, symbols))
            {
                continue;
            }

            var typeArgumentSyntax = GetTypeArgumentSyntax(spec.Kind, invocationSyntax.ArgumentList.Arguments);
            if (spec.Kind is MstestAssertMigrationKind.BeAssignableTo or MstestAssertMigrationKind.NotBeAssignableTo &&
                typeArgumentSyntax is null)
            {
                continue;
            }

            match = new MstestAssertMigrationMatch(
                spec,
                invocationSyntax,
                GetSubjectExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                GetExpectedExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                typeArgumentSyntax,
                RequiresAssertionsExtensionsNamespace(spec.Kind));

            return true;
        }

        return false;
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

    private static bool IsSupportedOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationKind kind,
        MstestAssertMigrationSymbols symbols)
    {
        return kind switch
        {
            MstestAssertMigrationKind.Be or MstestAssertMigrationKind.NotBe
                => IsSupportedEqualityOverload(invocation, symbols),
            MstestAssertMigrationKind.BeNull or MstestAssertMigrationKind.NotBeNull
                => IsSupportedNullOverload(invocation, symbols),
            MstestAssertMigrationKind.BeTrue or MstestAssertMigrationKind.BeFalse
                => IsSupportedBooleanOverload(invocation),
            MstestAssertMigrationKind.BeSameAs or MstestAssertMigrationKind.NotBeSameAs
                => IsSupportedReferenceEqualityOverload(invocation, symbols),
            MstestAssertMigrationKind.BeAssignableTo or MstestAssertMigrationKind.NotBeAssignableTo
                => IsSupportedTypeAssertionOverload(invocation, symbols),
            MstestAssertMigrationKind.Contain or MstestAssertMigrationKind.NotContain
                => IsSupportedCollectionContainmentOverload(invocation, symbols),
            MstestAssertMigrationKind.ContainSubstring
                => IsSupportedStringContainmentOverload(invocation, symbols),
            MstestAssertMigrationKind.StartWith or MstestAssertMigrationKind.EndWith
                => IsSupportedStringPrefixSuffixOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedEqualityOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsEqualityMigrationReceiver) &&
               !IsUnsupportedEqualityType(subjectType, symbols) &&
               IsSupportedEqualityExpectedExpression(invocation.Arguments[0], subjectType, symbols);
    }

    private static bool IsSupportedNullOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, symbols.SupportsNullMigrationReceiver);
    }

    private static bool IsSupportedBooleanOverload(IInvocationOperation invocation)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               subjectType.SpecialType == SpecialType.System_Boolean &&
               IsSupportedReceiverExpression(
                   invocation.Arguments[0],
                   subjectType,
                   static type => type.SpecialType == SpecialType.System_Boolean);
    }

    private static bool IsSupportedReferenceEqualityOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsReferenceEqualityMigrationReceiver);
    }

    private static bool IsSupportedTypeAssertionOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, symbols.SupportsTypeAssertionMigrationReceiver) &&
               TryGetTypeArgumentSyntax(invocation.Arguments[1], out _);
    }

    private static bool IsSupportedCollectionContainmentOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, symbols.SupportsCollectionContainmentMigrationReceiver) &&
               symbols.TryGetEnumerableElementType(subjectType, out var elementType) &&
               elementType is not null &&
               IsSupportedCollectionExpectedExpression(invocation.Arguments[1], elementType, symbols);
    }

    private static bool IsSupportedStringContainmentOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (!HasStringArguments(invocation))
        {
            return false;
        }

        return IsSupportedStringReceiverExpression(invocation.Arguments[0], symbols);
    }

    private static bool IsSupportedStringPrefixSuffixOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (!HasStringArguments(invocation))
        {
            return false;
        }

        return IsSupportedExpectedConstantStringExpression(invocation.Arguments[1]) &&
               IsSupportedStringReceiverExpression(invocation.Arguments[0], symbols);
    }

    private static bool HasStringArguments(IInvocationOperation invocation)
    {
        var method = invocation.TargetMethod;
        return method.Parameters.Length == 2 &&
               invocation.Arguments.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               method.Parameters[1].Type.SpecialType == SpecialType.System_String;
    }

    private static bool IsSupportedCollectionExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol elementType,
        MstestAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            if (IsNullLikeExpectedExpression(operation))
            {
                return MstestAssertMigrationSymbols.IsNullableOrReferenceType(elementType);
            }

            return false;
        }

        if (symbols.IsAsyncEnumerableLike(operation.Type) ||
            symbols.IsSpanOrMemoryLike(operation.Type))
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(operation.Type, elementType);
        return conversion.Exists && conversion.IsImplicit;
    }

    private static bool IsSupportedEqualityExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        MstestAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            if (IsNullLikeExpectedExpression(operation))
            {
                return MstestAssertMigrationSymbols.IsNullableOrReferenceType(subjectType);
            }

            return false;
        }

        if (IsUnsupportedEqualityType(operation.Type, symbols))
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(operation.Type, subjectType);
        return conversion.Exists && conversion.IsImplicit;
    }

    private static bool IsUnsupportedEqualityType(
        ITypeSymbol type,
        MstestAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static bool IsSupportedReceiverExpression(
        IArgumentOperation argument,
        ITypeSymbol subjectType,
        Func<ITypeSymbol, bool> receiverPredicate)
    {
        var operation = UnwrapConversions(argument.Value);
        if (operation.Syntax is LiteralExpressionSyntax literalExpression &&
            (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
             literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            return false;
        }

        return operation.Type is not null && receiverPredicate(subjectType);
    }

    private static bool IsSupportedStringReceiverExpression(
        IArgumentOperation argument,
        MstestAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(argument.Value);
        if (operation.Syntax is LiteralExpressionSyntax literalExpression &&
            (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
             literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            return false;
        }

        return operation.Type is not null &&
               symbols.SupportsStringContainmentMigrationReceiver(operation.Type);
    }

    private static bool IsSupportedExpectedConstantStringExpression(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        var constantValue = operation.ConstantValue;
        return constantValue.HasValue && constantValue.Value is string;
    }

    private static bool IsNullLikeExpectedExpression(IOperation operation)
    {
        return operation.Syntax is LiteralExpressionSyntax literalExpression &&
               (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
                literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)) ||
               operation.ConstantValue is { HasValue: true, Value: null };
    }

    private static ITypeSymbol? GetReceiverType(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        return operation.Type;
    }

    private static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static ExpressionSyntax GetSubjectExpression(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is MstestAssertMigrationKind.Be or
                 MstestAssertMigrationKind.NotBe or
                 MstestAssertMigrationKind.BeSameAs or
                 MstestAssertMigrationKind.NotBeSameAs
            ? arguments[1].Expression
            : arguments[0].Expression;

    private static ExpressionSyntax? GetExpectedExpression(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind switch
        {
            MstestAssertMigrationKind.Be or
            MstestAssertMigrationKind.NotBe or
            MstestAssertMigrationKind.BeSameAs or
            MstestAssertMigrationKind.NotBeSameAs => arguments[0].Expression,
            MstestAssertMigrationKind.Contain or
            MstestAssertMigrationKind.NotContain or
            MstestAssertMigrationKind.ContainSubstring or
            MstestAssertMigrationKind.StartWith or
            MstestAssertMigrationKind.EndWith => arguments[1].Expression,
            _ => null,
        };

    private static TypeSyntax? GetTypeArgumentSyntax(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is MstestAssertMigrationKind.BeAssignableTo or MstestAssertMigrationKind.NotBeAssignableTo
            ? TryGetTypeArgumentSyntax(arguments[1], out var typeArgumentSyntax) ? typeArgumentSyntax : null
            : null;

    private static bool TryGetTypeArgumentSyntax(
        ArgumentSyntax argument,
        out TypeSyntax? typeArgumentSyntax)
    {
        if (argument.Expression is TypeOfExpressionSyntax typeOfExpression)
        {
            typeArgumentSyntax = typeOfExpression.Type;
            return true;
        }

        typeArgumentSyntax = null;
        return false;
    }

    private static bool TryGetTypeArgumentSyntax(
        IArgumentOperation argument,
        out TypeSyntax? typeArgumentSyntax)
    {
        if (argument.Syntax is ArgumentSyntax argumentSyntax)
        {
            return TryGetTypeArgumentSyntax(argumentSyntax, out typeArgumentSyntax);
        }

        typeArgumentSyntax = null;
        return false;
    }

    private static bool RequiresAssertionsExtensionsNamespace(MstestAssertMigrationKind kind)
        => kind is MstestAssertMigrationKind.BeTrue or
                 MstestAssertMigrationKind.BeFalse or
                 MstestAssertMigrationKind.Contain or
                 MstestAssertMigrationKind.NotContain;
}
