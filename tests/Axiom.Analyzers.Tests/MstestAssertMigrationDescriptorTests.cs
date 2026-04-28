using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationDescriptorTests
{
    [Fact]
    public void DiagnosticDescriptors_AreStable()
    {
        var analyzer = new MstestAssertMigrationAnalyzer();
        var diagnostics = analyzer.SupportedDiagnostics.OrderBy(static rule => rule.Id).ToArray();

        Assert.Collection(
            diagnostics,
            rule =>
            {
                Assert.Equal("AXM1032", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.AreEqual to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.AreEqual(expected, actual) can be migrated to 'actual.Should().Be(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1033", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.AreNotEqual to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.AreNotEqual(expected, actual) can be migrated to 'actual.Should().NotBe(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1034", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsNull to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsNull(value) can be migrated to 'value.Should().BeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1035", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsNotNull to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsNotNull(value) can be migrated to 'value.Should().NotBeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1036", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsTrue to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsTrue(condition) can be migrated to 'condition.Should().BeTrue()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1037", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsFalse to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsFalse(condition) can be migrated to 'condition.Should().BeFalse()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1038", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.AreSame to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.AreSame(expected, actual) can be migrated to 'actual.Should().BeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1039", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.AreNotSame to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.AreNotSame(expected, actual) can be migrated to 'actual.Should().NotBeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1047", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsInstanceOfType to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsInstanceOfType(value, typeof(T)) can be migrated to 'value.Should().BeAssignableTo<T>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1048", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsNotInstanceOfType to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsNotInstanceOfType(value, typeof(T)) can be migrated to 'value.Should().NotBeAssignableTo<T>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1049", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest StringAssert.Contains to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest StringAssert.Contains(actual, expectedSubstring) can be migrated to 'actual.Should().Contain(expectedSubstring)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1050", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest StringAssert.StartsWith to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest StringAssert.StartsWith(actual, expectedPrefix) can be migrated to 'actual.Should().StartWith(expectedPrefix)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1051", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest StringAssert.EndsWith to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest StringAssert.EndsWith(actual, expectedSuffix) can be migrated to 'actual.Should().EndWith(expectedSuffix)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1052", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest CollectionAssert.Contains to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest CollectionAssert.Contains(collection, expected) can be migrated to 'collection.Should().Contain(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1053", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest CollectionAssert.DoesNotContain to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest CollectionAssert.DoesNotContain(collection, unexpected) can be migrated to 'collection.Should().NotContain(unexpected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1068", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.ThrowsExceptionAsync to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.ThrowsExceptionAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1069", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.ThrowsExactlyAsync to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.ThrowsExactlyAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1070", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.ThrowsAsync to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.ThrowsAsync<TException>(...) can be migrated to 'await ...Should().ThrowAsync<TException>()', appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1071", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsGreaterThan to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsGreaterThan(lowerBound, value) can be migrated to 'value.Should().BeGreaterThan(lowerBound)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1072", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsGreaterThanOrEqualTo to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsGreaterThanOrEqualTo(lowerBound, value) can be migrated to 'value.Should().BeGreaterThanOrEqualTo(lowerBound)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1073", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsLessThan to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsLessThan(upperBound, value) can be migrated to 'value.Should().BeLessThan(upperBound)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1074", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsLessThanOrEqualTo to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsLessThanOrEqualTo(upperBound, value) can be migrated to 'value.Should().BeLessThanOrEqualTo(upperBound)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1075", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate MSTest Assert.IsInRange to Axiom", rule.Title.ToString());
                Assert.Equal("MSTest Assert.IsInRange(minValue, maxValue, value) can be migrated to 'value.Should().BeInRange(minValue, maxValue)'", rule.MessageFormat.ToString());
            });
    }
}
