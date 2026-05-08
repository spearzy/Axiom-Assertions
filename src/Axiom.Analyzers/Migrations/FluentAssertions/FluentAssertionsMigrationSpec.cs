using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.FluentAssertionsMigration;

internal enum FluentAssertionsMigrationKind
{
    Be,
    NotBe,
    BeNull,
    NotBeNull,
    BeTrue,
    BeFalse,
    BeEmpty,
    NotBeEmpty,
    ContainSubstring,
    NotContainSubstring,
    StartWith,
    EndWith,
    BeSameAs,
    NotBeSameAs,
    BeOfType,
    BeAssignableTo,
}

internal sealed class FluentAssertionsMigrationSpec
{
    public FluentAssertionsMigrationSpec(
        string diagnosticId,
        string fluentAssertionsMethodName,
        int requiredArgumentCount,
        FluentAssertionsMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
        FluentAssertionsMethodName = fluentAssertionsMethodName;
        RequiredArgumentCount = requiredArgumentCount;
        Kind = kind;
        Title = title;
        Message = message;
        CodeFixTitle = codeFixTitle;
        Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            Message,
            "Migration",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Suggests a high-confidence migration from a supported FluentAssertions chain to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public string FluentAssertionsMethodName { get; }
    public int RequiredArgumentCount { get; }
    public FluentAssertionsMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }
    public DiagnosticDescriptor Rule { get; }
}

internal static class FluentAssertionsMigrationSpecs
{
    public static ImmutableArray<FluentAssertionsMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBe,
            "Be",
            1,
            FluentAssertionsMigrationKind.Be,
            "Migrate FluentAssertions Be to Axiom",
            "FluentAssertions actual.Should().Be(expected) can be migrated to Axiom 'actual.Should().Be(expected)'",
            "Convert to Axiom 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsNotBe,
            "NotBe",
            1,
            FluentAssertionsMigrationKind.NotBe,
            "Migrate FluentAssertions NotBe to Axiom",
            "FluentAssertions actual.Should().NotBe(unexpected) can be migrated to Axiom 'actual.Should().NotBe(unexpected)'",
            "Convert to Axiom 'actual.Should().NotBe(unexpected)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeNull,
            "BeNull",
            0,
            FluentAssertionsMigrationKind.BeNull,
            "Migrate FluentAssertions BeNull to Axiom",
            "FluentAssertions value.Should().BeNull() can be migrated to Axiom 'value.Should().BeNull()'",
            "Convert to Axiom 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsNotBeNull,
            "NotBeNull",
            0,
            FluentAssertionsMigrationKind.NotBeNull,
            "Migrate FluentAssertions NotBeNull to Axiom",
            "FluentAssertions value.Should().NotBeNull() can be migrated to Axiom 'value.Should().NotBeNull()'",
            "Convert to Axiom 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeTrue,
            "BeTrue",
            0,
            FluentAssertionsMigrationKind.BeTrue,
            "Migrate FluentAssertions BeTrue to Axiom",
            "FluentAssertions condition.Should().BeTrue() can be migrated to Axiom 'condition.Should().BeTrue()'",
            "Convert to Axiom 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeFalse,
            "BeFalse",
            0,
            FluentAssertionsMigrationKind.BeFalse,
            "Migrate FluentAssertions BeFalse to Axiom",
            "FluentAssertions condition.Should().BeFalse() can be migrated to Axiom 'condition.Should().BeFalse()'",
            "Convert to Axiom 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeEmpty,
            "BeEmpty",
            0,
            FluentAssertionsMigrationKind.BeEmpty,
            "Migrate FluentAssertions BeEmpty to Axiom",
            "FluentAssertions subject.Should().BeEmpty() can be migrated to Axiom 'subject.Should().BeEmpty()'",
            "Convert to Axiom 'subject.Should().BeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsNotBeEmpty,
            "NotBeEmpty",
            0,
            FluentAssertionsMigrationKind.NotBeEmpty,
            "Migrate FluentAssertions NotBeEmpty to Axiom",
            "FluentAssertions subject.Should().NotBeEmpty() can be migrated to Axiom 'subject.Should().NotBeEmpty()'",
            "Convert to Axiom 'subject.Should().NotBeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsContainSubstring,
            "Contain",
            1,
            FluentAssertionsMigrationKind.ContainSubstring,
            "Migrate FluentAssertions string Contain to Axiom",
            "FluentAssertions actual.Should().Contain(expectedSubstring) can be migrated to Axiom 'actual.Should().Contain(expectedSubstring)'",
            "Convert to Axiom 'actual.Should().Contain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsNotContainSubstring,
            "NotContain",
            1,
            FluentAssertionsMigrationKind.NotContainSubstring,
            "Migrate FluentAssertions string NotContain to Axiom",
            "FluentAssertions actual.Should().NotContain(unexpectedSubstring) can be migrated to Axiom 'actual.Should().NotContain(unexpectedSubstring)'",
            "Convert to Axiom 'actual.Should().NotContain(unexpectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsStartWith,
            "StartWith",
            1,
            FluentAssertionsMigrationKind.StartWith,
            "Migrate FluentAssertions StartWith to Axiom",
            "FluentAssertions actual.Should().StartWith(expectedPrefix) can be migrated to Axiom 'actual.Should().StartWith(expectedPrefix)'",
            "Convert to Axiom 'actual.Should().StartWith(expectedPrefix)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsEndWith,
            "EndWith",
            1,
            FluentAssertionsMigrationKind.EndWith,
            "Migrate FluentAssertions EndWith to Axiom",
            "FluentAssertions actual.Should().EndWith(expectedSuffix) can be migrated to Axiom 'actual.Should().EndWith(expectedSuffix)'",
            "Convert to Axiom 'actual.Should().EndWith(expectedSuffix)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeSameAs,
            "BeSameAs",
            1,
            FluentAssertionsMigrationKind.BeSameAs,
            "Migrate FluentAssertions BeSameAs to Axiom",
            "FluentAssertions actual.Should().BeSameAs(expected) can be migrated to Axiom 'actual.Should().BeSameAs(expected)'",
            "Convert to Axiom 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsNotBeSameAs,
            "NotBeSameAs",
            1,
            FluentAssertionsMigrationKind.NotBeSameAs,
            "Migrate FluentAssertions NotBeSameAs to Axiom",
            "FluentAssertions actual.Should().NotBeSameAs(unexpected) can be migrated to Axiom 'actual.Should().NotBeSameAs(unexpected)'",
            "Convert to Axiom 'actual.Should().NotBeSameAs(unexpected)'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeOfType,
            "BeOfType",
            0,
            FluentAssertionsMigrationKind.BeOfType,
            "Migrate FluentAssertions BeOfType to Axiom",
            "FluentAssertions value.Should().BeOfType<TExpected>() can be migrated to Axiom 'value.Should().BeOfType<TExpected>()'",
            "Convert to Axiom 'value.Should().BeOfType<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateFluentAssertionsBeAssignableTo,
            "BeAssignableTo",
            0,
            FluentAssertionsMigrationKind.BeAssignableTo,
            "Migrate FluentAssertions BeAssignableTo to Axiom",
            "FluentAssertions value.Should().BeAssignableTo<TExpected>() can be migrated to Axiom 'value.Should().BeAssignableTo<TExpected>()'",
            "Convert to Axiom 'value.Should().BeAssignableTo<TExpected>()'")
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static IEnumerable<FluentAssertionsMigrationSpec> GetByMethodName(string methodName)
    {
        foreach (var spec in All)
        {
            if (spec.FluentAssertionsMethodName == methodName)
            {
                yield return spec;
            }
        }
    }

    public static bool TryGetByDiagnosticId(string diagnosticId, out FluentAssertionsMigrationSpec spec)
    {
        foreach (var candidate in All)
        {
            if (candidate.DiagnosticId == diagnosticId)
            {
                spec = candidate;
                return true;
            }
        }

        spec = null!;
        return false;
    }
}
