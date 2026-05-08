using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class FluentAssertionsMigrationDescriptorTests
{
    [Fact]
    public void DiagnosticDescriptors_AreStable()
    {
        var analyzer = new FluentAssertionsMigrationAnalyzer();
        var diagnostics = analyzer.SupportedDiagnostics.OrderBy(static rule => rule.Id).ToArray();

        var expected = new[]
        {
            ("AXM1088", "Migrate FluentAssertions Be to Axiom", "FluentAssertions actual.Should().Be(expected) can be migrated to Axiom 'actual.Should().Be(expected)'"),
            ("AXM1089", "Migrate FluentAssertions NotBe to Axiom", "FluentAssertions actual.Should().NotBe(unexpected) can be migrated to Axiom 'actual.Should().NotBe(unexpected)'"),
            ("AXM1090", "Migrate FluentAssertions BeNull to Axiom", "FluentAssertions value.Should().BeNull() can be migrated to Axiom 'value.Should().BeNull()'"),
            ("AXM1091", "Migrate FluentAssertions NotBeNull to Axiom", "FluentAssertions value.Should().NotBeNull() can be migrated to Axiom 'value.Should().NotBeNull()'"),
            ("AXM1092", "Migrate FluentAssertions BeTrue to Axiom", "FluentAssertions condition.Should().BeTrue() can be migrated to Axiom 'condition.Should().BeTrue()'"),
            ("AXM1093", "Migrate FluentAssertions BeFalse to Axiom", "FluentAssertions condition.Should().BeFalse() can be migrated to Axiom 'condition.Should().BeFalse()'"),
            ("AXM1094", "Migrate FluentAssertions BeEmpty to Axiom", "FluentAssertions subject.Should().BeEmpty() can be migrated to Axiom 'subject.Should().BeEmpty()'"),
            ("AXM1095", "Migrate FluentAssertions NotBeEmpty to Axiom", "FluentAssertions subject.Should().NotBeEmpty() can be migrated to Axiom 'subject.Should().NotBeEmpty()'"),
            ("AXM1096", "Migrate FluentAssertions string Contain to Axiom", "FluentAssertions actual.Should().Contain(expectedSubstring) can be migrated to Axiom 'actual.Should().Contain(expectedSubstring)'"),
            ("AXM1097", "Migrate FluentAssertions string NotContain to Axiom", "FluentAssertions actual.Should().NotContain(unexpectedSubstring) can be migrated to Axiom 'actual.Should().NotContain(unexpectedSubstring)'"),
            ("AXM1098", "Migrate FluentAssertions StartWith to Axiom", "FluentAssertions actual.Should().StartWith(expectedPrefix) can be migrated to Axiom 'actual.Should().StartWith(expectedPrefix)'"),
            ("AXM1099", "Migrate FluentAssertions EndWith to Axiom", "FluentAssertions actual.Should().EndWith(expectedSuffix) can be migrated to Axiom 'actual.Should().EndWith(expectedSuffix)'"),
            ("AXM1100", "Migrate FluentAssertions BeSameAs to Axiom", "FluentAssertions actual.Should().BeSameAs(expected) can be migrated to Axiom 'actual.Should().BeSameAs(expected)'"),
            ("AXM1101", "Migrate FluentAssertions NotBeSameAs to Axiom", "FluentAssertions actual.Should().NotBeSameAs(unexpected) can be migrated to Axiom 'actual.Should().NotBeSameAs(unexpected)'"),
            ("AXM1102", "Migrate FluentAssertions BeOfType to Axiom", "FluentAssertions value.Should().BeOfType<TExpected>() can be migrated to Axiom 'value.Should().BeOfType<TExpected>()'"),
            ("AXM1103", "Migrate FluentAssertions BeAssignableTo to Axiom", "FluentAssertions value.Should().BeAssignableTo<TExpected>() can be migrated to Axiom 'value.Should().BeAssignableTo<TExpected>()'"),
        };

        Assert.Equal(expected.Length, diagnostics.Length);
        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index].Item1, diagnostics[index].Id);
            Assert.Equal("Migration", diagnostics[index].Category);
            Assert.Equal(DiagnosticSeverity.Info, diagnostics[index].DefaultSeverity);
            Assert.Equal(expected[index].Item2, diagnostics[index].Title.ToString());
            Assert.Equal(expected[index].Item3, diagnostics[index].MessageFormat.ToString());
        }
    }
}
