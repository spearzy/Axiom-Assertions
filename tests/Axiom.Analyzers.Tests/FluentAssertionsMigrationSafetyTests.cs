using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class FluentAssertionsMigrationSafetyTests
{
    [Fact]
    public async Task FluentAssertionsChain_WithBecauseArgument_IsNotFlagged()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected, "because the explanation should remain manual");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FluentAssertionsChain_WithAndContinuation_IsNotFlagged()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected).And.NotBe(0);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FluentAssertionsChain_WithConsumedTypeResult_IsNotFlagged()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public string Check(object value)
                    {
                        return value.Should().BeOfType<string>().Which;
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FluentAssertionsEquivalencyChain_IsNotFlagged()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(object actual, object expected)
                    {
                        actual.Should().BeEquivalentTo(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FluentAssertionsApproximationChain_IsNotFlagged()
    {
        const string source =
            """
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(double actual)
                    {
                        actual.Should().BeApproximately(10, 0.1);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FluentAssertionsCollectionContainment_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using FluentAssertions;

                public sealed class Sample
                {
                    public void Check(List<string> values)
                    {
                        values.Should().Contain("admin");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CustomShouldMethod_IsNotFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(Widget widget)
                    {
                        widget.Should().Be(42);
                    }
                }

                public sealed class Widget
                {
                    public WidgetAssertions Should() => new();
                }

                public sealed class WidgetAssertions
                {
                    public void Be(int expected) { }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<FluentAssertionsMigrationAnalyzer>(source);
    }
}
