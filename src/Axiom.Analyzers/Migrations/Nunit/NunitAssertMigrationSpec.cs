using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.NunitMigration;

internal enum NunitAssertMigrationKind
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
    Contain,
    NotContain,
    HaveCount,
    HaveUniqueItems,
    BeSameAs,
    NotBeSameAs,
    BeGreaterThan,
    BeGreaterThanOrEqualTo,
    BeLessThan,
    BeLessThanOrEqualTo,
    BeInRange,
    BeOfType,
    BeInstanceOf,
    BeAssignableTo,
    NotBeInstanceOf,
    NotBeAssignableTo,
    ThrowExactlyAsync,
    ThrowAsync,
}

internal sealed class NunitAssertMigrationSpec
{
    public NunitAssertMigrationSpec(
        string diagnosticId,
        NunitAssertMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
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
            description: "Suggests a high-confidence migration from a supported NUnit Assert.That(...) constraint to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public NunitAssertMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }
    public DiagnosticDescriptor Rule { get; }
}

internal static class NunitAssertMigrationSpecs
{
    public static ImmutableArray<NunitAssertMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEqualTo,
            NunitAssertMigrationKind.Be,
            "Migrate NUnit Assert.That equal constraint to Axiom",
            "NUnit Assert.That(actual, Is.EqualTo(expected)) can be migrated to 'actual.Should().Be(expected)'",
            "Convert to 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotEqualTo,
            NunitAssertMigrationKind.NotBe,
            "Migrate NUnit Assert.That not-equal constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.EqualTo(expected)) can be migrated to 'actual.Should().NotBe(expected)'",
            "Convert to 'actual.Should().NotBe(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNull,
            NunitAssertMigrationKind.BeNull,
            "Migrate NUnit Assert.That null constraint to Axiom",
            "NUnit Assert.That(value, Is.Null) can be migrated to 'value.Should().BeNull()'",
            "Convert to 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotNull,
            NunitAssertMigrationKind.NotBeNull,
            "Migrate NUnit Assert.That not-null constraint to Axiom",
            "NUnit Assert.That(value, Is.Not.Null) can be migrated to 'value.Should().NotBeNull()'",
            "Convert to 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatTrue,
            NunitAssertMigrationKind.BeTrue,
            "Migrate NUnit Assert.That true constraint to Axiom",
            "NUnit Assert.That(condition, Is.True) can be migrated to 'condition.Should().BeTrue()'",
            "Convert to 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatFalse,
            NunitAssertMigrationKind.BeFalse,
            "Migrate NUnit Assert.That false constraint to Axiom",
            "NUnit Assert.That(condition, Is.False) can be migrated to 'condition.Should().BeFalse()'",
            "Convert to 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEmpty,
            NunitAssertMigrationKind.BeEmpty,
            "Migrate NUnit Assert.That empty constraint to Axiom",
            "NUnit Assert.That(collection, Is.Empty) can be migrated to 'collection.Should().BeEmpty()'",
            "Convert to 'collection.Should().BeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotEmpty,
            NunitAssertMigrationKind.NotBeEmpty,
            "Migrate NUnit Assert.That not-empty constraint to Axiom",
            "NUnit Assert.That(collection, Is.Not.Empty) can be migrated to 'collection.Should().NotBeEmpty()'",
            "Convert to 'collection.Should().NotBeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatContainsSubstring,
            NunitAssertMigrationKind.ContainSubstring,
            "Migrate NUnit Assert.That Does.Contain string constraint to Axiom",
            "NUnit Assert.That(actual, Does.Contain(expectedSubstring)) can be migrated to 'actual.Should().Contain(expectedSubstring)'",
            "Convert to 'actual.Should().Contain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatDoesNotContainSubstring,
            NunitAssertMigrationKind.NotContainSubstring,
            "Migrate NUnit Assert.That Does.Not.Contain string constraint to Axiom",
            "NUnit Assert.That(actual, Does.Not.Contain(expectedSubstring)) can be migrated to 'actual.Should().NotContain(expectedSubstring)'",
            "Convert to 'actual.Should().NotContain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatStartsWith,
            NunitAssertMigrationKind.StartWith,
            "Migrate NUnit Assert.That Does.StartWith constraint to Axiom",
            "NUnit Assert.That(actual, Does.StartWith(expectedPrefix)) can be migrated to 'actual.Should().StartWith(expectedPrefix)'",
            "Convert to 'actual.Should().StartWith(expectedPrefix)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEndsWith,
            NunitAssertMigrationKind.EndWith,
            "Migrate NUnit Assert.That Does.EndWith constraint to Axiom",
            "NUnit Assert.That(actual, Does.EndWith(expectedSuffix)) can be migrated to 'actual.Should().EndWith(expectedSuffix)'",
            "Convert to 'actual.Should().EndWith(expectedSuffix)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatHasCount,
            NunitAssertMigrationKind.HaveCount,
            "Migrate NUnit Assert.That Has.Count.EqualTo constraint to Axiom",
            "NUnit Assert.That(collection, Has.Count.EqualTo(expectedCount)) can be migrated to 'collection.Should().HaveCount(expectedCount)'",
            "Convert to 'collection.Should().HaveCount(expectedCount)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatHasMember,
            NunitAssertMigrationKind.Contain,
            "Migrate NUnit Assert.That Has.Member constraint to Axiom",
            "NUnit Assert.That(collection, Has.Member(expected)) can be migrated to 'collection.Should().Contain(expected)'",
            "Convert to 'collection.Should().Contain(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatHasNoMember,
            NunitAssertMigrationKind.NotContain,
            "Migrate NUnit Assert.That Has.No.Member constraint to Axiom",
            "NUnit Assert.That(collection, Has.No.Member(unexpected)) can be migrated to 'collection.Should().NotContain(unexpected)'",
            "Convert to 'collection.Should().NotContain(unexpected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatUnique,
            NunitAssertMigrationKind.HaveUniqueItems,
            "Migrate NUnit Assert.That Is.Unique constraint to Axiom",
            "NUnit Assert.That(collection, Is.Unique) can be migrated to 'collection.Should().HaveUniqueItems()'",
            "Convert to 'collection.Should().HaveUniqueItems()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatSameAs,
            NunitAssertMigrationKind.BeSameAs,
            "Migrate NUnit Assert.That Is.SameAs constraint to Axiom",
            "NUnit Assert.That(actual, Is.SameAs(expected)) can be migrated to 'actual.Should().BeSameAs(expected)'",
            "Convert to 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotSameAs,
            NunitAssertMigrationKind.NotBeSameAs,
            "Migrate NUnit Assert.That Is.Not.SameAs constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.SameAs(expected)) can be migrated to 'actual.Should().NotBeSameAs(expected)'",
            "Convert to 'actual.Should().NotBeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatGreaterThan,
            NunitAssertMigrationKind.BeGreaterThan,
            "Migrate NUnit Assert.That Is.GreaterThan constraint to Axiom",
            "NUnit Assert.That(actual, Is.GreaterThan(expected)) can be migrated to 'actual.Should().BeGreaterThan(expected)'",
            "Convert to 'actual.Should().BeGreaterThan(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatGreaterThanOrEqualTo,
            NunitAssertMigrationKind.BeGreaterThanOrEqualTo,
            "Migrate NUnit Assert.That Is.GreaterThanOrEqualTo constraint to Axiom",
            "NUnit Assert.That(actual, Is.GreaterThanOrEqualTo(expected)) can be migrated to 'actual.Should().BeGreaterThanOrEqualTo(expected)'",
            "Convert to 'actual.Should().BeGreaterThanOrEqualTo(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatLessThan,
            NunitAssertMigrationKind.BeLessThan,
            "Migrate NUnit Assert.That Is.LessThan constraint to Axiom",
            "NUnit Assert.That(actual, Is.LessThan(expected)) can be migrated to 'actual.Should().BeLessThan(expected)'",
            "Convert to 'actual.Should().BeLessThan(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatLessThanOrEqualTo,
            NunitAssertMigrationKind.BeLessThanOrEqualTo,
            "Migrate NUnit Assert.That Is.LessThanOrEqualTo constraint to Axiom",
            "NUnit Assert.That(actual, Is.LessThanOrEqualTo(expected)) can be migrated to 'actual.Should().BeLessThanOrEqualTo(expected)'",
            "Convert to 'actual.Should().BeLessThanOrEqualTo(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatInRange,
            NunitAssertMigrationKind.BeInRange,
            "Migrate NUnit Assert.That Is.InRange constraint to Axiom",
            "NUnit Assert.That(actual, Is.InRange(minimum, maximum)) can be migrated to 'actual.Should().BeInRange(minimum, maximum)'",
            "Convert to 'actual.Should().BeInRange(minimum, maximum)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatTypeOf,
            NunitAssertMigrationKind.BeOfType,
            "Migrate NUnit Assert.That Is.TypeOf constraint to Axiom",
            "NUnit Assert.That(actual, Is.TypeOf<TExpected>()) can be migrated to 'actual.Should().BeOfType<TExpected>()'",
            "Convert to 'actual.Should().BeOfType<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatInstanceOf,
            NunitAssertMigrationKind.BeInstanceOf,
            "Migrate NUnit Assert.That Is.InstanceOf constraint to Axiom",
            "NUnit Assert.That(actual, Is.InstanceOf<TExpected>()) can be migrated to 'actual.Should().BeAssignableTo<TExpected>()'",
            "Convert to 'actual.Should().BeAssignableTo<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatAssignableTo,
            NunitAssertMigrationKind.BeAssignableTo,
            "Migrate NUnit Assert.That Is.AssignableTo constraint to Axiom",
            "NUnit Assert.That(actual, Is.AssignableTo<TExpected>()) can be migrated to 'actual.Should().BeAssignableTo<TExpected>()'",
            "Convert to 'actual.Should().BeAssignableTo<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotInstanceOf,
            NunitAssertMigrationKind.NotBeInstanceOf,
            "Migrate NUnit Assert.That Is.Not.InstanceOf constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.InstanceOf<TExpected>()) can be migrated to 'actual.Should().NotBeAssignableTo<TExpected>()'",
            "Convert to 'actual.Should().NotBeAssignableTo<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotAssignableTo,
            NunitAssertMigrationKind.NotBeAssignableTo,
            "Migrate NUnit Assert.That Is.Not.AssignableTo constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.AssignableTo<TExpected>()) can be migrated to 'actual.Should().NotBeAssignableTo<TExpected>()'",
            "Convert to 'actual.Should().NotBeAssignableTo<TExpected>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThrowsAsync,
            NunitAssertMigrationKind.ThrowExactlyAsync,
            "Migrate NUnit Assert.ThrowsAsync to Axiom",
            "NUnit Assert.ThrowsAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used",
            "Convert to 'await ...Should().ThrowExactlyAsync<TException>()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertCatchAsync,
            NunitAssertMigrationKind.ThrowAsync,
            "Migrate NUnit Assert.CatchAsync to Axiom",
            "NUnit Assert.CatchAsync<TException>(...) can be migrated to 'await ...Should().ThrowAsync<TException>()', appending '.Thrown' when the exception is used",
            "Convert to 'await ...Should().ThrowAsync<TException>()'")
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static bool TryGetByDiagnosticId(string diagnosticId, out NunitAssertMigrationSpec spec)
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
