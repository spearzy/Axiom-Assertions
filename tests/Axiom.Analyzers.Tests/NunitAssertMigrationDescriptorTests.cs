using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationDescriptorTests
{
    [Fact]
    public void DiagnosticDescriptors_AreStable()
    {
        var analyzer = new NunitAssertMigrationAnalyzer();
        var diagnostics = analyzer.SupportedDiagnostics.OrderBy(static rule => rule.Id).ToArray();

        Assert.Collection(
            diagnostics,
            rule =>
            {
                Assert.Equal("AXM1024", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That equal constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.EqualTo(expected)) can be migrated to 'actual.Should().Be(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1025", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-equal constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.Not.EqualTo(expected)) can be migrated to 'actual.Should().NotBe(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1026", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That null constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(value, Is.Null) can be migrated to 'value.Should().BeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1027", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-null constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(value, Is.Not.Null) can be migrated to 'value.Should().NotBeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1028", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That true constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(condition, Is.True) can be migrated to 'condition.Should().BeTrue()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1029", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That false constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(condition, Is.False) can be migrated to 'condition.Should().BeFalse()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1030", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That empty constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Is.Empty) can be migrated to 'collection.Should().BeEmpty()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1031", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-empty constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Is.Not.Empty) can be migrated to 'collection.Should().NotBeEmpty()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1040", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Does.Contain string constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Does.Contain(expectedSubstring)) can be migrated to 'actual.Should().Contain(expectedSubstring)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1041", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Does.Not.Contain string constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Does.Not.Contain(expectedSubstring)) can be migrated to 'actual.Should().NotContain(expectedSubstring)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1042", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Does.StartWith constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Does.StartWith(expectedPrefix)) can be migrated to 'actual.Should().StartWith(expectedPrefix)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1043", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Does.EndWith constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Does.EndWith(expectedSuffix)) can be migrated to 'actual.Should().EndWith(expectedSuffix)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1044", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Has.Count.EqualTo constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Has.Count.EqualTo(expectedCount)) can be migrated to 'collection.Should().HaveCount(expectedCount)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1045", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.SameAs constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.SameAs(expected)) can be migrated to 'actual.Should().BeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1046", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.Not.SameAs constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.Not.SameAs(expected)) can be migrated to 'actual.Should().NotBeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1056", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.GreaterThan constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.GreaterThan(expected)) can be migrated to 'actual.Should().BeGreaterThan(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1057", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.GreaterThanOrEqualTo constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.GreaterThanOrEqualTo(expected)) can be migrated to 'actual.Should().BeGreaterThanOrEqualTo(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1058", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.LessThan constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.LessThan(expected)) can be migrated to 'actual.Should().BeLessThan(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1059", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.LessThanOrEqualTo constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.LessThanOrEqualTo(expected)) can be migrated to 'actual.Should().BeLessThanOrEqualTo(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1060", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.InRange constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.InRange(minimum, maximum)) can be migrated to 'actual.Should().BeInRange(minimum, maximum)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1061", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.TypeOf constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.TypeOf<TExpected>()) can be migrated to 'actual.Should().BeOfType<TExpected>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1062", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.InstanceOf constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.InstanceOf<TExpected>()) can be migrated to 'actual.Should().BeAssignableTo<TExpected>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1063", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.AssignableTo constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.AssignableTo<TExpected>()) can be migrated to 'actual.Should().BeAssignableTo<TExpected>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1064", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.Not.InstanceOf constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.Not.InstanceOf<TExpected>()) can be migrated to 'actual.Should().NotBeAssignableTo<TExpected>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1065", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.Not.AssignableTo constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.Not.AssignableTo<TExpected>()) can be migrated to 'actual.Should().NotBeAssignableTo<TExpected>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1066", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.ThrowsAsync to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.ThrowsAsync<TException>(...) can be migrated to 'await ...Should().ThrowExactlyAsync<TException>()', appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1067", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.CatchAsync to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.CatchAsync<TException>(...) can be migrated to 'await ...Should().ThrowAsync<TException>()', appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1078", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Has.Member constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Has.Member(expected)) can be migrated to 'collection.Should().Contain(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1079", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Has.No.Member constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Has.No.Member(unexpected)) can be migrated to 'collection.Should().NotContain(unexpected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1080", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That Is.Unique constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Is.Unique) can be migrated to 'collection.Should().HaveUniqueItems()'", rule.MessageFormat.ToString());
            });
    }
}
