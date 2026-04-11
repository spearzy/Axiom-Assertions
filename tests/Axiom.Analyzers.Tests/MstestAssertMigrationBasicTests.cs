using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationBasicTests
{
    [Fact]
    public async Task AssertAreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreNotEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1033:Assert.AreNotEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().NotBe(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsNull_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1034:Assert.IsNull(value)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().BeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsNotNull_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1035:Assert.IsNotNull(value)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().NotBeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsTrue_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1036:Assert.IsTrue(condition)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        condition.Should().BeTrue();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsFalse_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1037:Assert.IsFalse(condition)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        condition.Should().BeFalse();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreSame_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        {|AXM1038:Assert.AreSame(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        actual.Should().BeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreNotSame_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        {|AXM1039:Assert.AreNotSame(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        actual.Should().NotBeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task FullyQualified_AssertAreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task UsingStaticAssert_AreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MultipleAreEqualAssertions_AreAllFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, int otherExpected, int otherActual)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                        {|AXM1032:Assert.AreEqual(otherExpected, otherActual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, int otherExpected, int otherActual)
                    {
                        actual.Should().Be(expected);
                        otherActual.Should().Be(otherExpected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MixedSupportedAssertions_AreFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, object? value, bool condition)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                        {|AXM1034:Assert.IsNull(value)|};
                        {|AXM1037:Assert.IsFalse(condition)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        Assert.AreEqual(expected, actual, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task PrecisionOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(double expected, double actual)
                    {
                        Assert.AreEqual(expected, actual, 0.1);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ComparerStyleOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string expected, string actual)
                    {
                        Assert.AreEqual(expected, actual, true);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionEquality_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> expected, List<int> actual)
                    {
                        Assert.AreEqual(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NullLiteralReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check()
                    {
                        Assert.IsNull(null);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringReferenceEquality_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string expected, string actual)
                    {
                        Assert.AreSame(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NonMstestAssert_IsNotFlagged()
    {
        const string source =
            """
                public static class Assert
                {
                    public static void AreEqual<T>(T expected, T actual)
                    {
                    }
                }

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        Assert.AreEqual(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsInstanceOfType_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        {|AXM1047:Assert.IsInstanceOfType(value, typeof(IDisposable))|};
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
                    public void Check(object value)
                    {
                        value.Should().BeAssignableTo<IDisposable>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsNotInstanceOfType_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        {|AXM1048:Assert.IsNotInstanceOfType(value, typeof(string))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        value.Should().NotBeAssignableTo<string>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StringAssertContains_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1049:StringAssert.Contains(actual, "archived")|};
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
    public async Task StringAssertStartsWith_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1050:StringAssert.StartsWith(actual, "pre")|};
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
    public async Task StringAssertEndsWith_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1051:StringAssert.EndsWith(actual, "suf")|};
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
                        actual.Should().EndWith("suf");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task CollectionAssertContains_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expected)
                    {
                        {|AXM1052:CollectionAssert.Contains(values, expected)|};
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
    public async Task CollectionAssertDoesNotContain_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values, int unexpected)
                    {
                        {|AXM1053:CollectionAssert.DoesNotContain(values, unexpected)|};
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
    public async Task AssertIsInstanceOfType_WithRuntimeTypeVariable_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object value, Type expectedType)
                    {
                        Assert.IsInstanceOfType(value, expectedType);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringAssertContains_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        StringAssert.Contains(actual, "archived", "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringAssertStartsWith_NonConstantPrefix_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedPrefix)
                    {
                        StringAssert.StartsWith(actual, expectedPrefix);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringAssertEndsWith_NonConstantSuffix_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedSuffix)
                    {
                        StringAssert.EndsWith(actual, expectedSuffix);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionAssertContains_MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expected)
                    {
                        CollectionAssert.Contains(values, expected, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionAssertContains_DictionaryReceiver_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(Dictionary<string, int> valuesByKey)
                    {
                        CollectionAssert.Contains(valuesByKey, new KeyValuePair<string, int>("alpha", 1));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionAssertAreEqual_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> expected, List<int> actual)
                    {
                        CollectionAssert.AreEqual(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionAssertAreEquivalent_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> expected, List<int> actual)
                    {
                        CollectionAssert.AreEquivalent(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomCode_IsNotFlagged()
    {
        const string source =
            """
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, object? value, bool condition, object expectedReference, object actualReference)
                    {
                        actual.Should().Be(expected);
                        actual.Should().NotBe(expected);
                        value.Should().BeNull();
                        value.Should().NotBeNull();
                        condition.Should().BeTrue();
                        condition.Should().BeFalse();
                        value.Should().BeAssignableTo<object>();
                        value.Should().NotBeAssignableTo<string>();
                        "actual".Should().Contain("act");
                        "actual".Should().StartWith("act");
                        "actual".Should().EndWith("ual");
                        actualReference.Should().BeSameAs(expectedReference);
                        actualReference.Should().NotBeSameAs(expectedReference);
                        new[] { 1, 2, 3 }.Should().Contain(2);
                        new[] { 1, 2, 3 }.Should().NotContain(4);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }
}
