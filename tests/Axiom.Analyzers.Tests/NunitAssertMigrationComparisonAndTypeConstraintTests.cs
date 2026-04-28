using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationComparisonAndTypeConstraintTests
{
    [Fact]
    public async Task AssertThat_GreaterThan_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Assert.That(actual, Is.GreaterThan(expected));
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().BeGreaterThan(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_GreaterThanOrEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Assert.That(actual, Is.GreaterThanOrEqualTo(expected));
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().BeGreaterThanOrEqualTo(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_LessThan_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Assert.That(actual, Is.LessThan(expected));
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().BeLessThan(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_LessThanOrEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Assert.That(actual, Is.LessThanOrEqualTo(expected));
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().BeLessThanOrEqualTo(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_InRange_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int minimum, int maximum)
                    {
                        Assert.That(actual, Is.InRange(minimum, maximum));
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int minimum, int maximum)
                    {
                        actual.Should().BeInRange(minimum, maximum);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_TypeOf_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        Assert.That(value, Is.TypeOf<string>());
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        value.Should().BeOfType<string>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_InstanceOf_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(Dog value)
                    {
                        Assert.That(value, Is.InstanceOf<Animal>());
                    }
                }

                public class Animal { }
                public sealed class Dog : Animal { }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(Dog value)
                    {
                        value.Should().BeAssignableTo<Animal>();
                    }
                }

                public class Animal { }
                public sealed class Dog : Animal { }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_AssignableTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(Dog value)
                    {
                        Assert.That(value, Is.AssignableTo<ITagged>());
                    }
                }

                public interface ITagged { }
                public sealed class Dog : ITagged { }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(Dog value)
                    {
                        value.Should().BeAssignableTo<ITagged>();
                    }
                }

                public interface ITagged { }
                public sealed class Dog : ITagged { }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_NotInstanceOf_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        Assert.That(value, Is.Not.InstanceOf<string>());
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        value.Should().NotBeAssignableTo<string>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_NotAssignableTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        Assert.That(value, Is.Not.AssignableTo<string>());
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        value.Should().NotBeAssignableTo<string>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task OrderedComparison_WithStringSubject_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string expected)
                    {
                        Assert.That(actual, Is.GreaterThan(expected));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task OrderedComparison_WithComparerChain_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected, object comparer)
                    {
                        Assert.That(actual, Is.GreaterThan(expected).Using(comparer));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task InRange_WithComparerChain_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int minimum, int maximum, object comparer)
                    {
                        Assert.That(actual, Is.InRange(minimum, maximum).Using(comparer));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task RuntimeTypeOfConstraint_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value, Type expectedType)
                    {
                        Assert.That(value, Is.TypeOf(expectedType));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task RuntimeInstanceOfConstraint_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value, Type expectedType)
                    {
                        Assert.That(value, Is.InstanceOf(expectedType));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task RuntimeAssignableToConstraint_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value, Type expectedType)
                    {
                        Assert.That(value, Is.AssignableTo(expectedType));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NotTypeOfConstraint_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object value)
                    {
                        Assert.That(value, Is.Not.TypeOf<string>());
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }
}
