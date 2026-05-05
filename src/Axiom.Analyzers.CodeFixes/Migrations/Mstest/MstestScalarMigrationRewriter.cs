using Axiom.Analyzers.MstestMigration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

internal static class MstestScalarMigrationRewriter
{
    public static ExpressionSyntax BuildReplacementExpression(MstestAssertMigrationMatch match)
    {
        return MstestAssertMigrationCodeFixProvider.BuildShouldCall(
            match.SubjectExpression,
            GetMethodName(match.Spec.Kind),
            match.ExpectedExpression,
            match.AdditionalArgumentExpression,
            match.TypeArgumentSyntax);
    }

    private static string GetMethodName(MstestAssertMigrationKind kind)
    {
        return kind switch
        {
            MstestAssertMigrationKind.Be => "Be",
            MstestAssertMigrationKind.NotBe => "NotBe",
            MstestAssertMigrationKind.BeNull => "BeNull",
            MstestAssertMigrationKind.NotBeNull => "NotBeNull",
            MstestAssertMigrationKind.BeTrue => "BeTrue",
            MstestAssertMigrationKind.BeFalse => "BeFalse",
            MstestAssertMigrationKind.BeSameAs => "BeSameAs",
            MstestAssertMigrationKind.NotBeSameAs => "NotBeSameAs",
            MstestAssertMigrationKind.BeAssignableTo => "BeAssignableTo",
            MstestAssertMigrationKind.NotBeAssignableTo => "NotBeAssignableTo",
            MstestAssertMigrationKind.Contain => "Contain",
            MstestAssertMigrationKind.NotContain => "NotContain",
            MstestAssertMigrationKind.ContainExpectedFirst => "Contain",
            MstestAssertMigrationKind.NotContainExpectedFirst => "NotContain",
            MstestAssertMigrationKind.ContainSubstring => "Contain",
            MstestAssertMigrationKind.ContainSubstringExpectedFirst => "Contain",
            MstestAssertMigrationKind.NotContainSubstringExpectedFirst => "NotContain",
            MstestAssertMigrationKind.StartWith => "StartWith",
            MstestAssertMigrationKind.StartWithExpectedFirst => "StartWith",
            MstestAssertMigrationKind.EndWith => "EndWith",
            MstestAssertMigrationKind.EndWithExpectedFirst => "EndWith",
            MstestAssertMigrationKind.HaveUniqueItems => "HaveUniqueItems",
            MstestAssertMigrationKind.BeGreaterThan => "BeGreaterThan",
            MstestAssertMigrationKind.BeGreaterThanOrEqualTo => "BeGreaterThanOrEqualTo",
            MstestAssertMigrationKind.BeLessThan => "BeLessThan",
            MstestAssertMigrationKind.BeLessThanOrEqualTo => "BeLessThanOrEqualTo",
            MstestAssertMigrationKind.BeInRange => "BeInRange",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
