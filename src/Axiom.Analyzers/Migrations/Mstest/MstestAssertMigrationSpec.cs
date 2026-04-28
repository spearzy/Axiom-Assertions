using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.MstestMigration;

internal enum MstestAssertMigrationKind
{
    Be,
    NotBe,
    BeNull,
    NotBeNull,
    BeTrue,
    BeFalse,
    BeSameAs,
    NotBeSameAs,
    BeAssignableTo,
    NotBeAssignableTo,
    Contain,
    NotContain,
    ContainSubstring,
    StartWith,
    EndWith,
    BeGreaterThan,
    BeGreaterThanOrEqualTo,
    BeLessThan,
    BeLessThanOrEqualTo,
    BeInRange,
    ThrowExactlyAsync,
    ThrowAsync,
}

internal enum MstestAssertTarget
{
    Assert,
    CollectionAssert,
    StringAssert,
}

internal sealed class MstestAssertMigrationSpec
{
    public MstestAssertMigrationSpec(
        string diagnosticId,
        MstestAssertTarget target,
        string methodName,
        int requiredArgumentCount,
        MstestAssertMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
        Target = target;
        MethodName = methodName;
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
            description: "Suggests a high-confidence migration from a supported MSTest Assert.* call to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public MstestAssertTarget Target { get; }
    public string MethodName { get; }
    public int RequiredArgumentCount { get; }
    public MstestAssertMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }
    public DiagnosticDescriptor Rule { get; }
}

internal static class MstestAssertMigrationSpecs
{
    public static ImmutableArray<MstestAssertMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreEqual,
            MstestAssertTarget.Assert,
            "AreEqual",
            2,
            MstestAssertMigrationKind.Be,
            "Migrate MSTest Assert.AreEqual to Axiom",
            "MSTest Assert.AreEqual(expected, actual) can be migrated to 'actual.Should().Be(expected)'",
            "Convert to 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreNotEqual,
            MstestAssertTarget.Assert,
            "AreNotEqual",
            2,
            MstestAssertMigrationKind.NotBe,
            "Migrate MSTest Assert.AreNotEqual to Axiom",
            "MSTest Assert.AreNotEqual(expected, actual) can be migrated to 'actual.Should().NotBe(expected)'",
            "Convert to 'actual.Should().NotBe(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsNull,
            MstestAssertTarget.Assert,
            "IsNull",
            1,
            MstestAssertMigrationKind.BeNull,
            "Migrate MSTest Assert.IsNull to Axiom",
            "MSTest Assert.IsNull(value) can be migrated to 'value.Should().BeNull()'",
            "Convert to 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsNotNull,
            MstestAssertTarget.Assert,
            "IsNotNull",
            1,
            MstestAssertMigrationKind.NotBeNull,
            "Migrate MSTest Assert.IsNotNull to Axiom",
            "MSTest Assert.IsNotNull(value) can be migrated to 'value.Should().NotBeNull()'",
            "Convert to 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsTrue,
            MstestAssertTarget.Assert,
            "IsTrue",
            1,
            MstestAssertMigrationKind.BeTrue,
            "Migrate MSTest Assert.IsTrue to Axiom",
            "MSTest Assert.IsTrue(condition) can be migrated to 'condition.Should().BeTrue()'",
            "Convert to 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsFalse,
            MstestAssertTarget.Assert,
            "IsFalse",
            1,
            MstestAssertMigrationKind.BeFalse,
            "Migrate MSTest Assert.IsFalse to Axiom",
            "MSTest Assert.IsFalse(condition) can be migrated to 'condition.Should().BeFalse()'",
            "Convert to 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreSame,
            MstestAssertTarget.Assert,
            "AreSame",
            2,
            MstestAssertMigrationKind.BeSameAs,
            "Migrate MSTest Assert.AreSame to Axiom",
            "MSTest Assert.AreSame(expected, actual) can be migrated to 'actual.Should().BeSameAs(expected)'",
            "Convert to 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreNotSame,
            MstestAssertTarget.Assert,
            "AreNotSame",
            2,
            MstestAssertMigrationKind.NotBeSameAs,
            "Migrate MSTest Assert.AreNotSame to Axiom",
            "MSTest Assert.AreNotSame(expected, actual) can be migrated to 'actual.Should().NotBeSameAs(expected)'",
            "Convert to 'actual.Should().NotBeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsInstanceOfType,
            MstestAssertTarget.Assert,
            "IsInstanceOfType",
            2,
            MstestAssertMigrationKind.BeAssignableTo,
            "Migrate MSTest Assert.IsInstanceOfType to Axiom",
            "MSTest Assert.IsInstanceOfType(value, typeof(T)) can be migrated to 'value.Should().BeAssignableTo<T>()'",
            "Convert to 'value.Should().BeAssignableTo<T>()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsNotInstanceOfType,
            MstestAssertTarget.Assert,
            "IsNotInstanceOfType",
            2,
            MstestAssertMigrationKind.NotBeAssignableTo,
            "Migrate MSTest Assert.IsNotInstanceOfType to Axiom",
            "MSTest Assert.IsNotInstanceOfType(value, typeof(T)) can be migrated to 'value.Should().NotBeAssignableTo<T>()'",
            "Convert to 'value.Should().NotBeAssignableTo<T>()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestStringAssertContains,
            MstestAssertTarget.StringAssert,
            "Contains",
            2,
            MstestAssertMigrationKind.ContainSubstring,
            "Migrate MSTest StringAssert.Contains to Axiom",
            "MSTest StringAssert.Contains(actual, expectedSubstring) can be migrated to 'actual.Should().Contain(expectedSubstring)'",
            "Convert to 'actual.Should().Contain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestStringAssertStartsWith,
            MstestAssertTarget.StringAssert,
            "StartsWith",
            2,
            MstestAssertMigrationKind.StartWith,
            "Migrate MSTest StringAssert.StartsWith to Axiom",
            "MSTest StringAssert.StartsWith(actual, expectedPrefix) can be migrated to 'actual.Should().StartWith(expectedPrefix)'",
            "Convert to 'actual.Should().StartWith(expectedPrefix)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestStringAssertEndsWith,
            MstestAssertTarget.StringAssert,
            "EndsWith",
            2,
            MstestAssertMigrationKind.EndWith,
            "Migrate MSTest StringAssert.EndsWith to Axiom",
            "MSTest StringAssert.EndsWith(actual, expectedSuffix) can be migrated to 'actual.Should().EndWith(expectedSuffix)'",
            "Convert to 'actual.Should().EndWith(expectedSuffix)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestCollectionAssertContains,
            MstestAssertTarget.CollectionAssert,
            "Contains",
            2,
            MstestAssertMigrationKind.Contain,
            "Migrate MSTest CollectionAssert.Contains to Axiom",
            "MSTest CollectionAssert.Contains(collection, expected) can be migrated to 'collection.Should().Contain(expected)'",
            "Convert to 'collection.Should().Contain(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestCollectionAssertDoesNotContain,
            MstestAssertTarget.CollectionAssert,
            "DoesNotContain",
            2,
            MstestAssertMigrationKind.NotContain,
            "Migrate MSTest CollectionAssert.DoesNotContain to Axiom",
            "MSTest CollectionAssert.DoesNotContain(collection, unexpected) can be migrated to 'collection.Should().NotContain(unexpected)'",
            "Convert to 'collection.Should().NotContain(unexpected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertThrowsExceptionAsync,
            MstestAssertTarget.Assert,
            "ThrowsExceptionAsync",
            1,
            MstestAssertMigrationKind.ThrowExactlyAsync,
            "Migrate MSTest Assert.ThrowsExceptionAsync to Axiom",
            "MSTest Assert.ThrowsExceptionAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used",
            "Convert to 'await ...Should().ThrowExactlyAsync<TException>()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertThrowsExactlyAsync,
            MstestAssertTarget.Assert,
            "ThrowsExactlyAsync",
            1,
            MstestAssertMigrationKind.ThrowExactlyAsync,
            "Migrate MSTest Assert.ThrowsExactlyAsync to Axiom",
            "MSTest Assert.ThrowsExactlyAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used",
            "Convert to 'await ...Should().ThrowExactlyAsync<TException>()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertThrowsAsync,
            MstestAssertTarget.Assert,
            "ThrowsAsync",
            1,
            MstestAssertMigrationKind.ThrowAsync,
            "Migrate MSTest Assert.ThrowsAsync to Axiom",
            "MSTest Assert.ThrowsAsync<TException>(...) can be migrated to 'await ...Should().ThrowAsync<TException>()', appending '.Thrown' when the exception is used",
            "Convert to 'await ...Should().ThrowAsync<TException>()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsGreaterThan,
            MstestAssertTarget.Assert,
            "IsGreaterThan",
            2,
            MstestAssertMigrationKind.BeGreaterThan,
            "Migrate MSTest Assert.IsGreaterThan to Axiom",
            "MSTest Assert.IsGreaterThan(lowerBound, value) can be migrated to 'value.Should().BeGreaterThan(lowerBound)'",
            "Convert to 'value.Should().BeGreaterThan(lowerBound)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsGreaterThanOrEqualTo,
            MstestAssertTarget.Assert,
            "IsGreaterThanOrEqualTo",
            2,
            MstestAssertMigrationKind.BeGreaterThanOrEqualTo,
            "Migrate MSTest Assert.IsGreaterThanOrEqualTo to Axiom",
            "MSTest Assert.IsGreaterThanOrEqualTo(lowerBound, value) can be migrated to 'value.Should().BeGreaterThanOrEqualTo(lowerBound)'",
            "Convert to 'value.Should().BeGreaterThanOrEqualTo(lowerBound)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsLessThan,
            MstestAssertTarget.Assert,
            "IsLessThan",
            2,
            MstestAssertMigrationKind.BeLessThan,
            "Migrate MSTest Assert.IsLessThan to Axiom",
            "MSTest Assert.IsLessThan(upperBound, value) can be migrated to 'value.Should().BeLessThan(upperBound)'",
            "Convert to 'value.Should().BeLessThan(upperBound)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsLessThanOrEqualTo,
            MstestAssertTarget.Assert,
            "IsLessThanOrEqualTo",
            2,
            MstestAssertMigrationKind.BeLessThanOrEqualTo,
            "Migrate MSTest Assert.IsLessThanOrEqualTo to Axiom",
            "MSTest Assert.IsLessThanOrEqualTo(upperBound, value) can be migrated to 'value.Should().BeLessThanOrEqualTo(upperBound)'",
            "Convert to 'value.Should().BeLessThanOrEqualTo(upperBound)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsInRange,
            MstestAssertTarget.Assert,
            "IsInRange",
            3,
            MstestAssertMigrationKind.BeInRange,
            "Migrate MSTest Assert.IsInRange to Axiom",
            "MSTest Assert.IsInRange(minValue, maxValue, value) can be migrated to 'value.Should().BeInRange(minValue, maxValue)'",
            "Convert to 'value.Should().BeInRange(minValue, maxValue)'")
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static IEnumerable<MstestAssertMigrationSpec> GetByMethodName(string methodName)
        => All.Where(spec => spec.MethodName == methodName);
}
