using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class FluentAssertionsMigrationBasicTests
{
    public static TheoryData<string, string, string, string, string> DirectMappings => new()
    {
        { "AXM1088", "int expected, int actual", "actual.Should().Be(expected)", "actual.Should().Be(expected)", "" },
        { "AXM1089", "int unexpected, int actual", "actual.Should().NotBe(unexpected)", "actual.Should().NotBe(unexpected)", "" },
        { "AXM1090", "object? value", "value.Should().BeNull()", "value.Should().BeNull()", "" },
        { "AXM1091", "object? value", "value.Should().NotBeNull()", "value.Should().NotBeNull()", "" },
        { "AXM1092", "bool condition", "condition.Should().BeTrue()", "condition.Should().BeTrue()", "using Axiom.Assertions.Extensions;\n" },
        { "AXM1093", "bool condition", "condition.Should().BeFalse()", "condition.Should().BeFalse()", "using Axiom.Assertions.Extensions;\n" },
        { "AXM1094", "string value", "value.Should().BeEmpty()", "value.Should().BeEmpty()", "" },
        { "AXM1095", "string value", "value.Should().NotBeEmpty()", "value.Should().NotBeEmpty()", "" },
        { "AXM1096", "string actual", "actual.Should().Contain(\"sub\")", "actual.Should().Contain(\"sub\")", "" },
        { "AXM1097", "string actual", "actual.Should().NotContain(\"sub\")", "actual.Should().NotContain(\"sub\")", "" },
        { "AXM1098", "string actual", "actual.Should().StartWith(\"pre\")", "actual.Should().StartWith(\"pre\")", "" },
        { "AXM1099", "string actual", "actual.Should().EndWith(\"suf\")", "actual.Should().EndWith(\"suf\")", "" },
        { "AXM1100", "object expected, object actual", "actual.Should().BeSameAs(expected)", "actual.Should().BeSameAs(expected)", "" },
        { "AXM1101", "object unexpected, object actual", "actual.Should().NotBeSameAs(unexpected)", "actual.Should().NotBeSameAs(unexpected)", "" },
        { "AXM1102", "object value", "value.Should().BeOfType<string>()", "value.Should().BeOfType<string>()", "" },
        { "AXM1103", "object value", "value.Should().BeAssignableTo<IDisposable>()", "value.Should().BeAssignableTo<IDisposable>()", "" },
    };

    [Theory]
    [MemberData(nameof(DirectMappings))]
    public async Task DirectFluentAssertionsChain_IsFlagged_AndFixed(
        string diagnosticId,
        string parameters,
        string fluentExpression,
        string axiomExpression,
        string extraAxiomUsing)
    {
        Assert.StartsWith("AXM", diagnosticId, StringComparison.Ordinal);

        var source = $$"""
                using System;
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check({{parameters}})
                    {
                        {{fluentExpression}};
                    }
                }
                """;

        var fixedSource = $$"""
                using System;
                using Axiom.Assertions;
                {{extraAxiomUsing}}
                public sealed class Sample
                {
                    public void Check({{parameters}})
                    {
                        {{axiomExpression}};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<FluentAssertionsMigrationAnalyzer, FluentAssertionsMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task CollectionBeEmpty_IsFlagged_AndFixed_WithCollectionExtensionsUsing()
    {
        const string source =
            """
                using System.Collections.Generic;
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        values.Should().BeEmpty();
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        values.Should().BeEmpty();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<FluentAssertionsMigrationAnalyzer, FluentAssertionsMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MixedFluentAssertionsFile_KeepsFluentAssertionsUsing_AndQualifiesMigratedShouldCall()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, int other)
                    {
                        actual.Should().Be(expected);
                        other.Should().NotBe(0, "because this remains a FluentAssertions call");
                    }
                }
                """;

        const string fixedSource =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, int other)
                    {
                        Axiom.Assertions.GenericShouldExtensions.Should(actual).Be(expected);
                        other.Should().NotBe(0, "because this remains a FluentAssertions call");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<FluentAssertionsMigrationAnalyzer, FluentAssertionsMigrationCodeFixProvider>(source, fixedSource);
    }
}
