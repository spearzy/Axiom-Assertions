using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.XunitMigration;

internal enum XunitAssertMigrationKind
{
    Be,
    NotBe,
    BeNull,
    NotBeNull,
    BeTrue,
    BeFalse,
    BeEmpty,
    NotBeEmpty,
    Contain,
    NotContain,
    ContainSubstring,
    NotContainSubstring,
    ContainSingle,
    BeSameAs,
    NotBeSameAs,
    Throw,
    BeOfType,
    BeAssignableTo,
}

internal sealed class XunitAssertMigrationSpec
{
    public XunitAssertMigrationSpec(
        string diagnosticId,
        string xunitMethodName,
        int requiredArgumentCount,
        XunitAssertMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
        XunitMethodName = xunitMethodName;
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
            description: "Suggests a high-confidence migration from a supported xUnit Assert.* call to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public string XunitMethodName { get; }
    public int RequiredArgumentCount { get; }
    public XunitAssertMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }

    public DiagnosticDescriptor Rule { get; }
}

internal static class XunitAssertMigrationSpecs
{
    public static ImmutableArray<XunitAssertMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateXunitAssertEqual,
            "Equal",
            2,
            XunitAssertMigrationKind.Be,
            "Migrate xUnit Assert.Equal to Axiom",
            "xUnit Assert.Equal(...) can be migrated to 'actual.Should().Be(expected)'",
            "Convert to 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertNotEqual,
            "NotEqual",
            2,
            XunitAssertMigrationKind.NotBe,
            "Migrate xUnit Assert.NotEqual to Axiom",
            "xUnit Assert.NotEqual(...) can be migrated to 'actual.Should().NotBe(expected)'",
            "Convert to 'actual.Should().NotBe(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertNull,
            "Null",
            1,
            XunitAssertMigrationKind.BeNull,
            "Migrate xUnit Assert.Null to Axiom",
            "xUnit Assert.Null(...) can be migrated to 'value.Should().BeNull()'",
            "Convert to 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertNotNull,
            "NotNull",
            1,
            XunitAssertMigrationKind.NotBeNull,
            "Migrate xUnit Assert.NotNull to Axiom",
            "xUnit Assert.NotNull(...) can be migrated to 'value.Should().NotBeNull()'",
            "Convert to 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertTrue,
            "True",
            1,
            XunitAssertMigrationKind.BeTrue,
            "Migrate xUnit Assert.True to Axiom",
            "xUnit Assert.True(...) can be migrated to 'condition.Should().BeTrue()'",
            "Convert to 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertFalse,
            "False",
            1,
            XunitAssertMigrationKind.BeFalse,
            "Migrate xUnit Assert.False to Axiom",
            "xUnit Assert.False(...) can be migrated to 'condition.Should().BeFalse()'",
            "Convert to 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertEmpty,
            "Empty",
            1,
            XunitAssertMigrationKind.BeEmpty,
            "Migrate xUnit Assert.Empty to Axiom",
            "xUnit Assert.Empty(...) can be migrated to 'subject.Should().BeEmpty()'",
            "Convert to 'subject.Should().BeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertNotEmpty,
            "NotEmpty",
            1,
            XunitAssertMigrationKind.NotBeEmpty,
            "Migrate xUnit Assert.NotEmpty to Axiom",
            "xUnit Assert.NotEmpty(...) can be migrated to 'subject.Should().NotBeEmpty()'",
            "Convert to 'subject.Should().NotBeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertContains,
            "Contains",
            2,
            XunitAssertMigrationKind.Contain,
            "Migrate xUnit Assert.Contains to Axiom",
            "xUnit Assert.Contains(...) can be migrated to 'collection.Should().Contain(item)'",
            "Convert to 'collection.Should().Contain(item)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertDoesNotContain,
            "DoesNotContain",
            2,
            XunitAssertMigrationKind.NotContain,
            "Migrate xUnit Assert.DoesNotContain to Axiom",
            "xUnit Assert.DoesNotContain(...) can be migrated to 'collection.Should().NotContain(item)'",
            "Convert to 'collection.Should().NotContain(item)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertContainsSubstring,
            "Contains",
            2,
            XunitAssertMigrationKind.ContainSubstring,
            "Migrate xUnit Assert.Contains string overload to Axiom",
            "xUnit Assert.Contains(expectedSubstring, actualString) can be migrated to 'actualString.Should().Contain(expectedSubstring)'",
            "Convert to 'actualString.Should().Contain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertDoesNotContainSubstring,
            "DoesNotContain",
            2,
            XunitAssertMigrationKind.NotContainSubstring,
            "Migrate xUnit Assert.DoesNotContain string overload to Axiom",
            "xUnit Assert.DoesNotContain(expectedSubstring, actualString) can be migrated to 'actualString.Should().NotContain(expectedSubstring)'",
            "Convert to 'actualString.Should().NotContain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertSingle,
            "Single",
            1,
            XunitAssertMigrationKind.ContainSingle,
            "Migrate xUnit Assert.Single to Axiom",
            "xUnit Assert.Single(...) can be migrated to 'subject.Should().ContainSingle()'",
            "Convert to 'subject.Should().ContainSingle()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertSame,
            "Same",
            2,
            XunitAssertMigrationKind.BeSameAs,
            "Migrate xUnit Assert.Same to Axiom",
            "xUnit Assert.Same(...) can be migrated to 'actual.Should().BeSameAs(expected)'",
            "Convert to 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertNotSame,
            "NotSame",
            2,
            XunitAssertMigrationKind.NotBeSameAs,
            "Migrate xUnit Assert.NotSame to Axiom",
            "xUnit Assert.NotSame(...) can be migrated to 'actual.Should().NotBeSameAs(expected)'",
            "Convert to 'actual.Should().NotBeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertThrows,
            "Throws",
            1,
            XunitAssertMigrationKind.Throw,
            "Migrate xUnit Assert.Throws to Axiom",
            "xUnit Assert.Throws<TException>(...) can be migrated to an Axiom '.Should().Throw<TException>()' assertion",
            "Convert to '.Should().Throw<TException>()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertIsType,
            "IsType",
            1,
            XunitAssertMigrationKind.BeOfType,
            "Migrate xUnit Assert.IsType to Axiom",
            "xUnit Assert.IsType<T>(...) can be migrated to 'value.Should().BeOfType<T>()'",
            "Convert to 'value.Should().BeOfType<T>()'"),
        new(
            AxiomAnalyzerIds.MigrateXunitAssertIsAssignableFrom,
            "IsAssignableFrom",
            1,
            XunitAssertMigrationKind.BeAssignableTo,
            "Migrate xUnit Assert.IsAssignableFrom to Axiom",
            "xUnit Assert.IsAssignableFrom<T>(...) can be migrated to 'value.Should().BeAssignableTo<T>()'",
            "Convert to 'value.Should().BeAssignableTo<T>()'"),
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static IEnumerable<XunitAssertMigrationSpec> GetByMethodName(string methodName)
        => All.Where(candidate => candidate.XunitMethodName == methodName);

    public static bool TryGetByDiagnosticId(string diagnosticId, out XunitAssertMigrationSpec spec)
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
