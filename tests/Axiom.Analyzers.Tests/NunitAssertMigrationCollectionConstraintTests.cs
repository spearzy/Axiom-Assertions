using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationCollectionConstraintTests
{
    [Fact]
    public async Task AssertThat_HasMember_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expected)
                    {
                        {|AXM1078:Assert.That(values, Has.Member(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_HasNoMember_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values, int unexpected)
                    {
                        {|AXM1079:Assert.That(values, Has.No.Member(unexpected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_IsUnique_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        {|AXM1080:Assert.That(values, Is.Unique)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task HasMember_StringSubject_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string value)
                    {
                        Assert.That(value, Has.Member('a'));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task HasMember_WithComparerChain_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<string> values, string expected, object comparer)
                    {
                        Assert.That(values, Has.Member(expected).Using(comparer));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task IsNotUnique_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        Assert.That(values, Is.Not.Unique);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }
}
