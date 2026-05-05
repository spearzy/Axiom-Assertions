using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationOrderedValueTests
{
    [Fact]
    public async Task AssertInRange_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(int actual, int low, int high)
                    {
                        {|AXM1077:Assert.InRange(actual, low, high)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int low, int high)
                    {
                        actual.Should().BeInRange(low, high);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertInRange_StringSubject_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual, string low, string high)
                    {
                        Assert.InRange(actual, low, high);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertInRange_ComparerOverload_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(int actual, int low, int high, IComparer<int> comparer)
                    {
                        Assert.InRange(actual, low, high, comparer);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertNotInRange_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(int actual, int low, int high)
                    {
                        Assert.NotInRange(actual, low, high);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertInRange_IsFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(int actual, int low, int high)
                    {
                        {|AXM1077:Xunit.Assert.InRange(actual, low, high)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }
}
