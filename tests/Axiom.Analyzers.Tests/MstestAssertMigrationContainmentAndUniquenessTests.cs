using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationContainmentAndUniquenessTests
{
    [Fact]
    public async Task AssertContains_String_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1081:Assert.Contains("archived", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().Contain("archived");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_String_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1082:Assert.DoesNotContain("blocked", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().NotContain("blocked");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringComparison_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1082:Assert.DoesNotContain("blocked", actual, StringComparison.OrdinalIgnoreCase)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().NotContain("blocked", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringComparison_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1081:Assert.Contains("archived", actual, StringComparison.OrdinalIgnoreCase)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().Contain("archived", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertStartsWith_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1083:Assert.StartsWith("pre", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertStartsWith_StringComparison_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1083:Assert.StartsWith("pre", actual, StringComparison.OrdinalIgnoreCase)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertEndsWith_StringComparison_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1084:Assert.EndsWith("suf", actual, StringComparison.OrdinalIgnoreCase)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().EndWith("suf", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_Collection_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expected)
                    {
                        {|AXM1085:Assert.Contains(expected, values)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expected)
                    {
                        values.Should().Contain(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_Collection_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values, int unexpected)
                    {
                        {|AXM1086:Assert.DoesNotContain(unexpected, values)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(List<int> values, int unexpected)
                    {
                        values.Should().NotContain(unexpected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task CollectionAssertAllItemsAreUnique_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        {|AXM1087:CollectionAssert.AllItemsAreUnique(values)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        values.Should().HaveUniqueItems();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringMessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.Contains("archived", actual, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertContains_CollectionComparerOverload_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<string> values, string expected)
                    {
                        Assert.Contains(expected, values, EqualityComparer<string>.Default);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_NonConstantPrefix_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedPrefix)
                    {
                        Assert.StartsWith(expectedPrefix, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionAssertAllItemsAreUnique_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        CollectionAssert.AllItemsAreUnique(values, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }
}
