using Axiom.Analyzers.NunitMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class NunitScalarMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(NunitAssertMigrationMatch match)
    {
        return NunitAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression,
            match.AdditionalExpectedExpression,
            match.TypeArgumentSyntax);
    }

    private static string GetMethodName(NunitAssertMigrationKind kind)
    {
        return kind switch
        {
            NunitAssertMigrationKind.Be => "Be",
            NunitAssertMigrationKind.NotBe => "NotBe",
            NunitAssertMigrationKind.BeNull => "BeNull",
            NunitAssertMigrationKind.NotBeNull => "NotBeNull",
            NunitAssertMigrationKind.BeTrue => "BeTrue",
            NunitAssertMigrationKind.BeFalse => "BeFalse",
            NunitAssertMigrationKind.BeEmpty => "BeEmpty",
            NunitAssertMigrationKind.NotBeEmpty => "NotBeEmpty",
            NunitAssertMigrationKind.ContainSubstring => "Contain",
            NunitAssertMigrationKind.NotContainSubstring => "NotContain",
            NunitAssertMigrationKind.StartWith => "StartWith",
            NunitAssertMigrationKind.EndWith => "EndWith",
            NunitAssertMigrationKind.Contain => "Contain",
            NunitAssertMigrationKind.NotContain => "NotContain",
            NunitAssertMigrationKind.HaveCount => "HaveCount",
            NunitAssertMigrationKind.HaveUniqueItems => "HaveUniqueItems",
            NunitAssertMigrationKind.BeSameAs => "BeSameAs",
            NunitAssertMigrationKind.NotBeSameAs => "NotBeSameAs",
            NunitAssertMigrationKind.BeGreaterThan => "BeGreaterThan",
            NunitAssertMigrationKind.BeGreaterThanOrEqualTo => "BeGreaterThanOrEqualTo",
            NunitAssertMigrationKind.BeLessThan => "BeLessThan",
            NunitAssertMigrationKind.BeLessThanOrEqualTo => "BeLessThanOrEqualTo",
            NunitAssertMigrationKind.BeInRange => "BeInRange",
            NunitAssertMigrationKind.BeOfType => "BeOfType",
            NunitAssertMigrationKind.BeInstanceOf => "BeAssignableTo",
            NunitAssertMigrationKind.BeAssignableTo => "BeAssignableTo",
            NunitAssertMigrationKind.NotBeInstanceOf => "NotBeAssignableTo",
            NunitAssertMigrationKind.NotBeAssignableTo => "NotBeAssignableTo",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
