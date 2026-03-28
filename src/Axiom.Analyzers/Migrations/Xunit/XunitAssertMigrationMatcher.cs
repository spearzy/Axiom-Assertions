using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal sealed class XunitAssertMigrationMatch
{
    public XunitAssertMigrationMatch(
        XunitAssertMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        bool requiresAssertionsExtensionsNamespace)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
    }

    public XunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
}

internal static class XunitAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        out XunitAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !symbols.IsXunitAssert(invocation.TargetMethod.ContainingType) ||
            !XunitAssertMigrationSpecs.TryGetByMethodName(invocation.TargetMethod.Name, out var spec))
        {
            return false;
        }

        if (invocation.TargetMethod.Parameters.Length != spec.RequiredArgumentCount ||
            invocation.Arguments.Length != spec.RequiredArgumentCount ||
            invocationSyntax.ArgumentList.Arguments.Count != spec.RequiredArgumentCount ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        if (!IsSafeSupportedOverload(invocation.TargetMethod, spec, symbols))
        {
            return false;
        }

        var arguments = invocationSyntax.ArgumentList.Arguments;
        var subjectExpression = GetSubjectExpression(spec.Kind, arguments);
        var expectedExpression = GetExpectedExpression(spec.Kind, arguments);

        match = new XunitAssertMigrationMatch(
            spec,
            invocationSyntax,
            subjectExpression,
            expectedExpression,
            RequiresAssertionsExtensionsNamespace(spec.Kind, invocation.TargetMethod.Parameters[0].Type));

        return true;
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
        IMethodSymbol method,
        XunitAssertMigrationSpec spec,
        XunitAssertMigrationSymbols symbols)
    {
        switch (spec.Kind)
        {
            case XunitAssertMigrationKind.Be:
            case XunitAssertMigrationKind.NotBe:
                return IsSupportedEqualityOverload(method, symbols);

            case XunitAssertMigrationKind.BeTrue:
            case XunitAssertMigrationKind.BeFalse:
                return method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean;

            case XunitAssertMigrationKind.BeEmpty:
            case XunitAssertMigrationKind.NotBeEmpty:
                return !symbols.IsAsyncEnumerableLike(method.Parameters[0].Type);

            case XunitAssertMigrationKind.BeNull:
            case XunitAssertMigrationKind.NotBeNull:
                return true;

            default:
                return false;
        }
    }

    private static bool IsSupportedEqualityOverload(
        IMethodSymbol method,
        XunitAssertMigrationSymbols symbols)
    {
        if (method.Parameters.Length != 2)
        {
            return false;
        }

        return !IsUnsupportedEqualityType(method.Parameters[0].Type, symbols) &&
               !IsUnsupportedEqualityType(method.Parameters[1].Type, symbols);
    }

    private static bool IsUnsupportedEqualityType(
        ITypeSymbol type,
        XunitAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static ExpressionSyntax GetSubjectExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is XunitAssertMigrationKind.Be or XunitAssertMigrationKind.NotBe
            ? arguments[1].Expression
            : arguments[0].Expression;

    private static ExpressionSyntax? GetExpectedExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is XunitAssertMigrationKind.Be or XunitAssertMigrationKind.NotBe
            ? arguments[0].Expression
            : null;

    private static bool RequiresAssertionsExtensionsNamespace(
        XunitAssertMigrationKind kind,
        ITypeSymbol subjectType)
    {
        return kind switch
        {
            XunitAssertMigrationKind.BeTrue => true,
            XunitAssertMigrationKind.BeFalse => true,
            XunitAssertMigrationKind.BeEmpty => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.NotBeEmpty => subjectType.SpecialType != SpecialType.System_String,
            _ => false,
        };
    }
}
