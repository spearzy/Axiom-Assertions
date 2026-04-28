using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationOrderedValueTests
{
    [Fact]
    public async Task AssertIsGreaterThan_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int lowerBound, int value)
                    {
                        Assert.IsGreaterThan(lowerBound, value);
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int lowerBound, int value)
                    {
                        value.Should().BeGreaterThan(lowerBound);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsGreaterThanOrEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int lowerBound, int value)
                    {
                        Assert.IsGreaterThanOrEqualTo(lowerBound, value);
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int lowerBound, int value)
                    {
                        value.Should().BeGreaterThanOrEqualTo(lowerBound);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsLessThan_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int upperBound, int value)
                    {
                        Assert.IsLessThan(upperBound, value);
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int upperBound, int value)
                    {
                        value.Should().BeLessThan(upperBound);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsLessThanOrEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int upperBound, int value)
                    {
                        Assert.IsLessThanOrEqualTo(upperBound, value);
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int upperBound, int value)
                    {
                        value.Should().BeLessThanOrEqualTo(upperBound);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsInRange_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int minimum, int maximum, int value)
                    {
                        Assert.IsInRange(minimum, maximum, value);
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int minimum, int maximum, int value)
                    {
                        value.Should().BeInRange(minimum, maximum);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task OrderedComparison_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int lowerBound, int value)
                    {
                        Assert.IsGreaterThan(lowerBound, value, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task InRange_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int minimum, int maximum, int value)
                    {
                        Assert.IsInRange(minimum, maximum, value, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task OrderedComparison_StringSubject_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string lowerBound, string value)
                    {
                        Assert.IsGreaterThan(lowerBound, value);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task InRange_NonComparableStruct_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public readonly struct Point
                {
                    public Point(int x) => X = x;
                    public int X { get; }
                }

                public sealed class Sample
                {
                    public void Check(Point minimum, Point maximum, Point value)
                    {
                        Assert.IsInRange(minimum, maximum, value);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }
}
