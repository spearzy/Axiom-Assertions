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
        AwaitExpressionSyntax? awaitExpressionSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        ExpressionSyntax? additionalArgumentExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace,
        bool appendThrown)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        AwaitExpressionSyntax = awaitExpressionSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        AdditionalArgumentExpression = additionalArgumentExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
        AppendThrown = appendThrown;
    }

    public MstestAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public AwaitExpressionSyntax? AwaitExpressionSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public ExpressionSyntax? AdditionalArgumentExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
    public bool AppendThrown { get; }
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
                !HasSupportedArgumentCount(spec, invocation, invocationSyntax) ||
                !IsSupportedOverload(invocation, spec.Kind, symbols))
            {
                continue;
            }

            var typeArgumentSyntax = GetTypeArgumentSyntax(spec.Kind, invocationSyntax);
            if (spec.Kind is MstestAssertMigrationKind.BeAssignableTo or MstestAssertMigrationKind.NotBeAssignableTo &&
                typeArgumentSyntax is null)
            {
                continue;
            }

            if (IsAsyncThrowsKind(spec.Kind) && typeArgumentSyntax is null)
            {
                continue;
            }

            var awaitExpressionSyntax = GetAwaitExpressionSyntax(invocationSyntax);
            match = new MstestAssertMigrationMatch(
                spec,
                invocationSyntax,
                awaitExpressionSyntax,
                GetSubjectExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                GetExpectedExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                GetAdditionalArgumentExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                typeArgumentSyntax,
                RequiresAssertionsExtensionsNamespace(spec.Kind),
                appendThrown: IsAsyncThrowsKind(spec.Kind) &&
                    awaitExpressionSyntax is not null &&
                    IsAwaitedResultConsumed(awaitExpressionSyntax));

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

    private static bool HasSupportedArgumentCount(
        MstestAssertMigrationSpec spec,
        IInvocationOperation invocation,
        InvocationExpressionSyntax invocationSyntax)
    {
        if (IsExpectedFirstStringKind(spec.Kind))
        {
            return invocation.TargetMethod.Parameters.Length >= spec.RequiredArgumentCount &&
                   invocation.Arguments.Length >= spec.RequiredArgumentCount &&
                   invocationSyntax.ArgumentList.Arguments.Count is 2 or 3;
        }

        if (AllowsMetadataOptionalTrailingParameters(spec.Kind))
        {
            return invocation.TargetMethod.Parameters.Length >= spec.RequiredArgumentCount &&
                   invocation.Arguments.Length >= spec.RequiredArgumentCount &&
                   invocationSyntax.ArgumentList.Arguments.Count == spec.RequiredArgumentCount;
        }

        return invocation.TargetMethod.Parameters.Length == spec.RequiredArgumentCount &&
               invocation.Arguments.Length == spec.RequiredArgumentCount &&
               invocationSyntax.ArgumentList.Arguments.Count == spec.RequiredArgumentCount;
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
            MstestAssertMigrationKind.ContainExpectedFirst or MstestAssertMigrationKind.NotContainExpectedFirst
                => IsSupportedExpectedFirstCollectionContainmentOverload(invocation, symbols),
            MstestAssertMigrationKind.ContainSubstring
                => IsSupportedStringContainmentOverload(invocation, symbols),
            MstestAssertMigrationKind.ContainSubstringExpectedFirst or MstestAssertMigrationKind.NotContainSubstringExpectedFirst
                => IsSupportedExpectedFirstStringContainmentOverload(invocation, symbols),
            MstestAssertMigrationKind.StartWith or MstestAssertMigrationKind.EndWith
                => IsSupportedStringPrefixSuffixOverload(invocation, symbols),
            MstestAssertMigrationKind.StartWithExpectedFirst or MstestAssertMigrationKind.EndWithExpectedFirst
                => IsSupportedExpectedFirstStringPrefixSuffixOverload(invocation, symbols),
            MstestAssertMigrationKind.HaveUniqueItems
                => IsSupportedCollectionUniquenessOverload(invocation, symbols),
            MstestAssertMigrationKind.BeGreaterThan or
            MstestAssertMigrationKind.BeGreaterThanOrEqualTo or
            MstestAssertMigrationKind.BeLessThan or
            MstestAssertMigrationKind.BeLessThanOrEqualTo
                => IsSupportedOrderedComparisonOverload(invocation, symbols),
            MstestAssertMigrationKind.BeInRange
                => IsSupportedRangeOverload(invocation, symbols),
            MstestAssertMigrationKind.ThrowExactlyAsync or MstestAssertMigrationKind.ThrowAsync
                => IsSupportedAsyncThrowOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedAsyncThrowOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        return GetAwaitExpressionSyntax(invocation.Syntax) is not null &&
               method.IsGenericMethod &&
               method.TypeArguments.Length == 1 &&
               method.Parameters.Length >= 1 &&
               symbols.IsFuncReturningTask(method.Parameters[0].Type);
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

    private static bool IsSupportedExpectedFirstCollectionContainmentOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length != 2)
        {
            return false;
        }

        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsCollectionContainmentMigrationReceiver) &&
               symbols.TryGetEnumerableElementType(subjectType, out var elementType) &&
               elementType is not null &&
               IsSupportedCollectionExpectedExpression(invocation.Arguments[0], elementType, symbols);
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

    private static bool IsSupportedExpectedFirstStringContainmentOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (!HasExpectedFirstStringArguments(invocation, symbols))
        {
            return false;
        }

        return IsSupportedStringReceiverExpression(invocation.Arguments[1], symbols);
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

    private static bool IsSupportedExpectedFirstStringPrefixSuffixOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (!HasExpectedFirstStringArguments(invocation, symbols))
        {
            return false;
        }

        return IsSupportedExpectedConstantStringExpression(invocation.Arguments[0]) &&
               IsSupportedStringReceiverExpression(invocation.Arguments[1], symbols);
    }

    private static bool IsSupportedCollectionUniquenessOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length != 1)
        {
            return false;
        }

        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, symbols.SupportsUniqueItemsMigrationReceiver);
    }

    private static bool HasStringArguments(IInvocationOperation invocation)
    {
        var method = invocation.TargetMethod;
        return method.Parameters.Length == 2 &&
               invocation.Arguments.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               method.Parameters[1].Type.SpecialType == SpecialType.System_String;
    }

    private static bool HasExpectedFirstStringArguments(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length is not (2 or 3) ||
            invocation.Arguments[0].Parameter?.Type.SpecialType != SpecialType.System_String ||
            invocation.Arguments[1].Parameter?.Type.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        return invocation.Arguments.Length == 2 ||
               symbols.IsStringComparison(invocation.Arguments[2].Parameter?.Type);
    }

    private static bool IsSupportedOrderedComparisonOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsOrderedValueMigrationReceiver) &&
               IsSupportedOrderedExpectedExpression(invocation.Arguments[0], subjectType, symbols);
    }

    private static bool IsSupportedRangeOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[2]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[2], subjectType, symbols.SupportsOrderedValueMigrationReceiver) &&
               IsSupportedOrderedExpectedExpression(invocation.Arguments[0], subjectType, symbols) &&
               IsSupportedOrderedExpectedExpression(invocation.Arguments[1], subjectType, symbols);
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

    private static bool IsSupportedOrderedExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        MstestAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            return false;
        }

        if (!symbols.SupportsOrderedValueMigrationReceiver(operation.Type))
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
                 MstestAssertMigrationKind.NotBeSameAs or
                 MstestAssertMigrationKind.ContainExpectedFirst or
                 MstestAssertMigrationKind.NotContainExpectedFirst or
                 MstestAssertMigrationKind.ContainSubstringExpectedFirst or
                 MstestAssertMigrationKind.NotContainSubstringExpectedFirst or
                 MstestAssertMigrationKind.StartWithExpectedFirst or
                 MstestAssertMigrationKind.EndWithExpectedFirst or
                 MstestAssertMigrationKind.BeGreaterThan or
                 MstestAssertMigrationKind.BeGreaterThanOrEqualTo or
                 MstestAssertMigrationKind.BeLessThan or
                 MstestAssertMigrationKind.BeLessThanOrEqualTo
            ? arguments[1].Expression
            : kind is MstestAssertMigrationKind.BeInRange
                ? arguments[2].Expression
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
            MstestAssertMigrationKind.ContainExpectedFirst or
            MstestAssertMigrationKind.NotContainExpectedFirst or
            MstestAssertMigrationKind.ContainSubstringExpectedFirst or
            MstestAssertMigrationKind.NotContainSubstringExpectedFirst or
            MstestAssertMigrationKind.StartWithExpectedFirst or
            MstestAssertMigrationKind.EndWithExpectedFirst => arguments[0].Expression,
            MstestAssertMigrationKind.Contain or
            MstestAssertMigrationKind.NotContain or
            MstestAssertMigrationKind.ContainSubstring or
            MstestAssertMigrationKind.StartWith or
            MstestAssertMigrationKind.EndWith => arguments[1].Expression,
            MstestAssertMigrationKind.BeGreaterThan or
            MstestAssertMigrationKind.BeGreaterThanOrEqualTo or
            MstestAssertMigrationKind.BeLessThan or
            MstestAssertMigrationKind.BeLessThanOrEqualTo or
            MstestAssertMigrationKind.BeInRange => arguments[0].Expression,
            _ => null,
        };

    private static ExpressionSyntax? GetAdditionalArgumentExpression(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
    {
        if (kind is MstestAssertMigrationKind.BeInRange)
        {
            return arguments[1].Expression;
        }

        return (kind is MstestAssertMigrationKind.ContainSubstringExpectedFirst or
                        MstestAssertMigrationKind.NotContainSubstringExpectedFirst or
                        MstestAssertMigrationKind.StartWithExpectedFirst or
                        MstestAssertMigrationKind.EndWithExpectedFirst) &&
               arguments.Count == 3
            ? arguments[2].Expression
            : null;
    }

    private static TypeSyntax? GetTypeArgumentSyntax(
        MstestAssertMigrationKind kind,
        InvocationExpressionSyntax invocationSyntax)
    {
        if (kind is MstestAssertMigrationKind.BeAssignableTo or MstestAssertMigrationKind.NotBeAssignableTo)
        {
            return TryGetTypeArgumentSyntax(invocationSyntax.ArgumentList.Arguments[1], out var typeArgumentSyntax)
                ? typeArgumentSyntax
                : null;
        }

        if (!IsAsyncThrowsKind(kind))
        {
            return null;
        }

        return invocationSyntax.Expression switch
        {
            GenericNameSyntax genericName when genericName.TypeArgumentList.Arguments.Count == 1
                => genericName.TypeArgumentList.Arguments[0],
            MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax genericName
            } when genericName.TypeArgumentList.Arguments.Count == 1
                => genericName.TypeArgumentList.Arguments[0],
            _ => null,
        };
    }

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
                 MstestAssertMigrationKind.NotContain or
                 MstestAssertMigrationKind.ContainExpectedFirst or
                 MstestAssertMigrationKind.NotContainExpectedFirst or
                 MstestAssertMigrationKind.HaveUniqueItems;

    private static bool IsAsyncThrowsKind(MstestAssertMigrationKind kind)
        => kind is MstestAssertMigrationKind.ThrowExactlyAsync or MstestAssertMigrationKind.ThrowAsync;

    private static bool IsOrderedValueKind(MstestAssertMigrationKind kind)
        => kind is MstestAssertMigrationKind.BeGreaterThan or
            MstestAssertMigrationKind.BeGreaterThanOrEqualTo or
            MstestAssertMigrationKind.BeLessThan or
            MstestAssertMigrationKind.BeLessThanOrEqualTo or
            MstestAssertMigrationKind.BeInRange;

    private static bool IsExpectedFirstStringKind(MstestAssertMigrationKind kind)
        => kind is MstestAssertMigrationKind.ContainSubstringExpectedFirst or
                   MstestAssertMigrationKind.NotContainSubstringExpectedFirst or
                   MstestAssertMigrationKind.StartWithExpectedFirst or
                   MstestAssertMigrationKind.EndWithExpectedFirst;

    private static bool AllowsMetadataOptionalTrailingParameters(MstestAssertMigrationKind kind)
        => IsAsyncThrowsKind(kind) ||
           IsOrderedValueKind(kind) ||
           kind is MstestAssertMigrationKind.ContainExpectedFirst or
                   MstestAssertMigrationKind.NotContainExpectedFirst or
                   MstestAssertMigrationKind.HaveUniqueItems;

    private static AwaitExpressionSyntax? GetAwaitExpressionSyntax(SyntaxNode syntax)
        => syntax.Parent as AwaitExpressionSyntax;

    private static bool IsAwaitedResultConsumed(AwaitExpressionSyntax awaitExpressionSyntax)
        => awaitExpressionSyntax.Parent is not ExpressionStatementSyntax;
}
