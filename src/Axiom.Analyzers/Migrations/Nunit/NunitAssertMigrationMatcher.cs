using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.NunitMigration;

internal sealed class NunitAssertMigrationMatch
{
    public NunitAssertMigrationMatch(
        NunitAssertMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        ExpressionSyntax? additionalExpectedExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace,
        bool appendThrown)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        AdditionalExpectedExpression = additionalExpectedExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
        AppendThrown = appendThrown;
    }

    public NunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public ExpressionSyntax? AdditionalExpectedExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
    public bool AppendThrown { get; }
}

internal static class NunitAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        NunitAssertMigrationSymbols symbols,
        out NunitAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !symbols.IsNunitAssert(invocation.TargetMethod.ContainingType) ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        return invocation.TargetMethod.Name == "That"
            ? TryMatchAssertThat(invocation, invocationSyntax, symbols, out match)
            : TryMatchAsyncThrows(invocation, invocationSyntax, symbols, out match);
    }

    private static bool TryMatchAssertThat(
        IInvocationOperation invocation,
        InvocationExpressionSyntax invocationSyntax,
        NunitAssertMigrationSymbols symbols,
        out NunitAssertMigrationMatch match)
    {
        match = null!;

        if (invocation.Arguments.Length != 2 ||
            invocation.TargetMethod.Parameters.Length < 2 ||
            !symbols.IsResolveConstraint(invocation.TargetMethod.Parameters[1].Type))
        {
            return false;
        }

        var constraintOperation = UnwrapConversions(invocation.Arguments[1].Value);
        foreach (var spec in NunitAssertMigrationSpecs.All)
        {
            if (!TryMatchConstraint(
                    constraintOperation,
                    spec.Kind,
                    symbols,
                    out var expectedExpression,
                    out var expectedArgument,
                    out var additionalExpectedExpression,
                    out var additionalExpectedArgument,
                    out var typeArgumentSyntax))
            {
                continue;
            }

            var subjectType = GetReceiverType(invocation.Arguments[0]);
            if (subjectType is null ||
                !IsSupportedSubject(
                    invocation.Arguments[0],
                    subjectType,
                    expectedArgument,
                    additionalExpectedArgument,
                    spec.Kind,
                    symbols))
            {
                continue;
            }

            match = new NunitAssertMigrationMatch(
                spec,
                invocationSyntax,
                invocationSyntax.ArgumentList.Arguments[0].Expression,
                expectedExpression,
                additionalExpectedExpression,
                typeArgumentSyntax,
                RequiresAssertionsExtensionsNamespace(spec.Kind),
                appendThrown: false);

            return true;
        }

        return false;
    }

    private static bool TryMatchAsyncThrows(
        IInvocationOperation invocation,
        InvocationExpressionSyntax invocationSyntax,
        NunitAssertMigrationSymbols symbols,
        out NunitAssertMigrationMatch match)
    {
        match = null!;

        if (!IsInAsyncContext(invocationSyntax))
        {
            return false;
        }

        foreach (var spec in NunitAssertMigrationSpecs.All)
        {
            if (!TryMatchAsyncThrowsInvocation(
                    invocation,
                    invocationSyntax,
                    spec.Kind,
                    symbols,
                    out var subjectExpression,
                    out var typeArgumentSyntax))
            {
                continue;
            }

            match = new NunitAssertMigrationMatch(
                spec,
                invocationSyntax,
                subjectExpression,
                expectedExpression: null,
                additionalExpectedExpression: null,
                typeArgumentSyntax,
                requiresAssertionsExtensionsNamespace: false,
                appendThrown: IsResultConsumed(invocation));

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

    private static bool TryMatchConstraint(
        IOperation constraintOperation,
        NunitAssertMigrationKind kind,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument,
        out ExpressionSyntax? additionalExpectedExpression,
        out IArgumentOperation? additionalExpectedArgument,
        out TypeSyntax? typeArgumentSyntax)
    {
        expectedExpression = null;
        expectedArgument = null;
        additionalExpectedExpression = null;
        additionalExpectedArgument = null;
        typeArgumentSyntax = null;

        return kind switch
        {
            NunitAssertMigrationKind.Be => TryMatchEqualTo(constraintOperation, symbols, negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotBe => TryMatchEqualTo(constraintOperation, symbols, negated: true, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeNull => TryMatchPropertyConstraint(constraintOperation, symbols, "Null", negated: false),
            NunitAssertMigrationKind.NotBeNull => TryMatchPropertyConstraint(constraintOperation, symbols, "Null", negated: true),
            NunitAssertMigrationKind.BeTrue => TryMatchPropertyConstraint(constraintOperation, symbols, "True", negated: false),
            NunitAssertMigrationKind.BeFalse => TryMatchPropertyConstraint(constraintOperation, symbols, "False", negated: false),
            NunitAssertMigrationKind.BeEmpty => TryMatchPropertyConstraint(constraintOperation, symbols, "Empty", negated: false),
            NunitAssertMigrationKind.NotBeEmpty => TryMatchPropertyConstraint(constraintOperation, symbols, "Empty", negated: true),
            NunitAssertMigrationKind.ContainSubstring => TryMatchDoesStringMethod(constraintOperation, symbols, "Contain", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotContainSubstring => TryMatchDoesStringMethod(constraintOperation, symbols, "Contain", negated: true, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.StartWith => TryMatchDoesStringMethod(constraintOperation, symbols, "StartWith", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.EndWith => TryMatchDoesStringMethod(constraintOperation, symbols, "EndWith", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.HaveCount => TryMatchCountEqualTo(constraintOperation, symbols, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeSameAs => TryMatchSameAs(constraintOperation, symbols, negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotBeSameAs => TryMatchSameAs(constraintOperation, symbols, negated: true, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeGreaterThan => TryMatchIsSingleArgumentMethod(constraintOperation, symbols, "GreaterThan", out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeGreaterThanOrEqualTo => TryMatchIsSingleArgumentMethod(constraintOperation, symbols, "GreaterThanOrEqualTo", out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeLessThan => TryMatchIsSingleArgumentMethod(constraintOperation, symbols, "LessThan", out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeLessThanOrEqualTo => TryMatchIsSingleArgumentMethod(constraintOperation, symbols, "LessThanOrEqualTo", out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeInRange => TryMatchInRange(constraintOperation, symbols, out expectedExpression, out expectedArgument, out additionalExpectedExpression, out additionalExpectedArgument),
            NunitAssertMigrationKind.BeOfType => TryMatchIsGenericTypeMethod(constraintOperation, symbols, "TypeOf", negated: false, out typeArgumentSyntax),
            NunitAssertMigrationKind.BeInstanceOf => TryMatchIsGenericTypeMethod(constraintOperation, symbols, "InstanceOf", negated: false, out typeArgumentSyntax),
            NunitAssertMigrationKind.BeAssignableTo => TryMatchIsGenericTypeMethod(constraintOperation, symbols, "AssignableTo", negated: false, out typeArgumentSyntax),
            NunitAssertMigrationKind.NotBeInstanceOf => TryMatchIsGenericTypeMethod(constraintOperation, symbols, "InstanceOf", negated: true, out typeArgumentSyntax),
            NunitAssertMigrationKind.NotBeAssignableTo => TryMatchIsGenericTypeMethod(constraintOperation, symbols, "AssignableTo", negated: true, out typeArgumentSyntax),
            _ => false,
        };
    }

    private static bool TryMatchAsyncThrowsInvocation(
        IInvocationOperation invocation,
        InvocationExpressionSyntax invocationSyntax,
        NunitAssertMigrationKind kind,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax subjectExpression,
        out TypeSyntax? typeArgumentSyntax)
    {
        subjectExpression = null!;
        typeArgumentSyntax = null;

        var expectedMethodName = kind switch
        {
            NunitAssertMigrationKind.ThrowExactlyAsync => "ThrowsAsync",
            NunitAssertMigrationKind.ThrowAsync => "CatchAsync",
            _ => null,
        };

        if (expectedMethodName is null ||
            invocation.TargetMethod.Name != expectedMethodName ||
            !invocation.TargetMethod.IsGenericMethod ||
            invocation.TargetMethod.TypeArguments.Length != 1 ||
            invocation.TargetMethod.Parameters.Length != 1 ||
            invocation.Arguments.Length != 1 ||
            !symbols.IsAsyncTestDelegate(invocation.TargetMethod.Parameters[0].Type) ||
            !TryGetSingleGenericTypeArgument(invocationSyntax, out typeArgumentSyntax) ||
            !IsSupportedAsyncTestDelegateArgument(invocation.Arguments[0]))
        {
            return false;
        }

        subjectExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        return true;
    }

    private static bool IsSupportedAsyncTestDelegateArgument(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        return operation is IAnonymousFunctionOperation or IMethodReferenceOperation ||
               operation is IDelegateCreationOperation
               {
                   Target: IAnonymousFunctionOperation or IMethodReferenceOperation,
               };
    }

    private static bool TryMatchEqualTo(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "EqualTo" ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsIsType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchPropertyConstraint(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string propertyName,
        bool negated)
    {
        if (constraintOperation is not IPropertyReferenceOperation propertyReference ||
            propertyReference.Property.Name != propertyName)
        {
            return false;
        }

        if (negated)
        {
            return symbols.IsConstraintExpressionType(propertyReference.Property.ContainingType) &&
                   IsDirectNotExpression(propertyReference.Instance, symbols);
        }

        return propertyReference.Instance is null &&
               symbols.IsIsType(propertyReference.Property.ContainingType);
    }

    private static bool TryMatchDoesStringMethod(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string methodName,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != methodName ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectDoesNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsDoesType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchCountEqualTo(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "EqualTo" ||
            invocation.Arguments.Length != 1 ||
            !symbols.IsCountConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
            !IsDirectHasCountExpression(invocation.Instance, symbols))
        {
            return false;
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchSameAs(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "SameAs" ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsIsType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchIsSingleArgumentMethod(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string methodName,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != methodName ||
            invocation.Arguments.Length != 1 ||
            invocation.Instance is not null ||
            !symbols.IsIsType(invocation.TargetMethod.ContainingType))
        {
            return false;
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchInRange(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax? minimumExpression,
        out IArgumentOperation? minimumArgument,
        out ExpressionSyntax? maximumExpression,
        out IArgumentOperation? maximumArgument)
    {
        minimumExpression = null;
        minimumArgument = null;
        maximumExpression = null;
        maximumArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "InRange" ||
            invocation.Arguments.Length != 2 ||
            invocation.Instance is not null ||
            !symbols.IsIsType(invocation.TargetMethod.ContainingType))
        {
            return false;
        }

        minimumExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        minimumArgument = invocation.Arguments[0];
        maximumExpression = invocationSyntax.ArgumentList.Arguments[1].Expression;
        maximumArgument = invocation.Arguments[1];
        return true;
    }

    private static bool TryMatchIsGenericTypeMethod(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string methodName,
        bool negated,
        out TypeSyntax? typeArgumentSyntax)
    {
        typeArgumentSyntax = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != methodName ||
            invocation.TargetMethod.TypeArguments.Length != 1 ||
            invocation.Arguments.Length != 0)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsIsType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        return TryGetSingleGenericTypeArgument(invocationSyntax, out typeArgumentSyntax);
    }

    private static bool TryGetSingleGenericTypeArgument(
        InvocationExpressionSyntax invocationSyntax,
        out TypeSyntax? typeArgumentSyntax)
    {
        typeArgumentSyntax = null;

        var nameSyntax = invocationSyntax.Expression switch
        {
            MemberAccessExpressionSyntax { Name: GenericNameSyntax genericName } => genericName,
            GenericNameSyntax genericName => genericName,
            _ => null,
        };

        if (nameSyntax?.TypeArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        typeArgumentSyntax = nameSyntax.TypeArgumentList.Arguments[0];
        return true;
    }

    private static bool IsDirectNotExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Not" &&
               propertyReference.Instance is null &&
               symbols.IsIsType(propertyReference.Property.ContainingType) &&
               symbols.IsConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsDirectDoesNotExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Not" &&
               propertyReference.Instance is null &&
               symbols.IsDoesType(propertyReference.Property.ContainingType) &&
               symbols.IsConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsDirectHasCountExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Count" &&
               propertyReference.Instance is null &&
               symbols.IsHasType(propertyReference.Property.ContainingType) &&
               symbols.IsCountConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsSupportedSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        IArgumentOperation? additionalExpectedArgument,
        NunitAssertMigrationKind kind,
        NunitAssertMigrationSymbols symbols)
    {
        return kind switch
        {
            NunitAssertMigrationKind.Be or NunitAssertMigrationKind.NotBe
                => IsSupportedEqualitySubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeNull or NunitAssertMigrationKind.NotBeNull
                => IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsNullMigrationReceiver),
            NunitAssertMigrationKind.BeTrue or NunitAssertMigrationKind.BeFalse
                => IsSupportedReceiverExpression(subjectArgument, subjectType, static type => type.SpecialType == SpecialType.System_Boolean),
            NunitAssertMigrationKind.BeEmpty or NunitAssertMigrationKind.NotBeEmpty
                => IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEmptyMigrationReceiver),
            NunitAssertMigrationKind.ContainSubstring or NunitAssertMigrationKind.NotContainSubstring
                => IsSupportedStringConstraintSubject(subjectArgument, subjectType, expectedArgument, symbols, requireConstantExpected: false),
            NunitAssertMigrationKind.StartWith or NunitAssertMigrationKind.EndWith
                => IsSupportedStringConstraintSubject(subjectArgument, subjectType, expectedArgument, symbols, requireConstantExpected: true),
            NunitAssertMigrationKind.HaveCount
                => IsSupportedCountSubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeSameAs or NunitAssertMigrationKind.NotBeSameAs
                => IsSupportedReferenceEqualitySubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeGreaterThan or
            NunitAssertMigrationKind.BeGreaterThanOrEqualTo or
            NunitAssertMigrationKind.BeLessThan or
            NunitAssertMigrationKind.BeLessThanOrEqualTo
                => IsSupportedOrderedComparisonSubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeInRange
                => IsSupportedOrderedComparisonSubject(subjectArgument, subjectType, expectedArgument, symbols) &&
                   additionalExpectedArgument is not null &&
                   IsSupportedOrderedExpectedExpression(additionalExpectedArgument, subjectType, symbols),
            NunitAssertMigrationKind.BeOfType or
            NunitAssertMigrationKind.BeInstanceOf or
            NunitAssertMigrationKind.BeAssignableTo or
            NunitAssertMigrationKind.NotBeInstanceOf or
            NunitAssertMigrationKind.NotBeAssignableTo
                => IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsTypeAssertionMigrationReceiver),
            _ => false,
        };
    }

    private static bool IsSupportedEqualitySubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        return expectedArgument is not null &&
               IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEqualityMigrationReceiver) &&
               !IsUnsupportedEqualityType(subjectType, symbols) &&
               IsSupportedEqualityExpectedExpression(expectedArgument, subjectType, symbols);
    }

    private static bool IsSupportedEqualityExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        NunitAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            if (IsNullLikeExpectedExpression(operation))
            {
                return NunitAssertMigrationSymbols.IsNullableOrReferenceType(subjectType);
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

    private static bool IsUnsupportedEqualityType(ITypeSymbol type, NunitAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static bool IsSupportedStringConstraintSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols,
        bool requireConstantExpected)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEqualityMigrationReceiver) ||
            subjectType.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        var expectedOperation = UnwrapConversions(expectedArgument.Value);
        if (expectedOperation.Type?.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        if (!requireConstantExpected)
        {
            return true;
        }

        return expectedOperation.ConstantValue is { HasValue: true, Value: string };
    }

    private static bool IsSupportedCountSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsCountMigrationReceiver))
        {
            return false;
        }

        var expectedType = UnwrapConversions(expectedArgument.Value).Type;
        return expectedType?.SpecialType == SpecialType.System_Int32;
    }

    private static bool IsSupportedReferenceEqualitySubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsReferenceEqualityMigrationReceiver))
        {
            return false;
        }

        var expectedType = UnwrapConversions(expectedArgument.Value).Type;
        if (expectedType is null)
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(expectedType, subjectType);
        return conversion.Exists && conversion.IsImplicit;
    }

    private static bool IsSupportedOrderedComparisonSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        return expectedArgument is not null &&
               IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsOrderedComparisonMigrationReceiver) &&
               IsSupportedOrderedExpectedExpression(expectedArgument, subjectType, symbols);
    }

    private static bool IsSupportedOrderedExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        NunitAssertMigrationSymbols symbols)
    {
        var expectedType = UnwrapConversions(expectedArgument.Value).Type;
        if (expectedType is null ||
            IsUnsupportedOrderedComparisonType(expectedType, symbols))
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(expectedType, subjectType);
        return conversion.Exists && conversion.IsImplicit;
    }

    private static bool IsUnsupportedOrderedComparisonType(ITypeSymbol type, NunitAssertMigrationSymbols symbols)
    {
        return type.SpecialType == SpecialType.System_String ||
               symbols.IsEnumerableLike(type) ||
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

    internal static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static bool IsInAsyncContext(SyntaxNode syntax)
    {
        foreach (var ancestor in syntax.Ancestors())
        {
            switch (ancestor)
            {
                case AnonymousFunctionExpressionSyntax anonymousFunction:
                    return anonymousFunction.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword);
                case LocalFunctionStatementSyntax localFunction:
                    return localFunction.Modifiers.Any(SyntaxKind.AsyncKeyword);
                case BaseMethodDeclarationSyntax methodDeclaration:
                    return methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword);
            }
        }

        return false;
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

    private static bool RequiresAssertionsExtensionsNamespace(NunitAssertMigrationKind kind)
        => kind is NunitAssertMigrationKind.BeTrue or
                    NunitAssertMigrationKind.BeFalse or
                    NunitAssertMigrationKind.BeEmpty or
                    NunitAssertMigrationKind.NotBeEmpty or
                    NunitAssertMigrationKind.HaveCount;
}
