namespace Axiom.Analyzers.Tests.Helpers;

internal static class FluentAssertionsStubs
{
    public const string Source =
        """
        using System;
        using System.Collections.Generic;

        namespace FluentAssertions
        {
            public static class AssertionExtensions
            {
                public static FluentAssertions.Primitives.StringAssertions Should(this string? subject) => throw null!;
                public static FluentAssertions.Primitives.BooleanAssertions Should(this bool subject) => throw null!;
                public static FluentAssertions.Primitives.ObjectAssertions<T> Should<T>(this T subject) => throw null!;
            }
        }

        namespace FluentAssertions
        {
            public sealed class AndConstraint<TAssertions>
            {
                public TAssertions And => throw null!;
            }

            public sealed class AndWhichConstraint<TAssertions, TSubject>
            {
                public TAssertions And => throw null!;
                public TSubject Which => throw null!;
            }
        }

        namespace FluentAssertions.Primitives
        {
            public sealed class ObjectAssertions<T>
            {
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> NotBe(T unexpected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> BeNull(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> NotBeNull(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> BeEmpty(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> NotBeEmpty(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> Contain<TElement>(TElement expected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> NotContain<TElement>(TElement unexpected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> BeSameAs(T expected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> NotBeSameAs(T unexpected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndWhichConstraint<ObjectAssertions<T>, TExpected> BeOfType<TExpected>(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndWhichConstraint<ObjectAssertions<T>, TExpected> BeAssignableTo<TExpected>(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> BeEquivalentTo<TExpectation>(TExpectation expectation, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<ObjectAssertions<T>> BeApproximately(double expected, double precision, string because = "", params object[] becauseArgs) => throw null!;
            }

            public sealed class StringAssertions
            {
                public FluentAssertions.AndConstraint<StringAssertions> Be(string? expected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> NotBe(string? unexpected, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> BeNull(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> NotBeNull(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> BeEmpty(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> NotBeEmpty(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> Contain(string expectedSubstring, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> NotContain(string unexpectedSubstring, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> StartWith(string expectedPrefix, string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<StringAssertions> EndWith(string expectedSuffix, string because = "", params object[] becauseArgs) => throw null!;
            }

            public sealed class BooleanAssertions
            {
                public FluentAssertions.AndConstraint<BooleanAssertions> BeTrue(string because = "", params object[] becauseArgs) => throw null!;
                public FluentAssertions.AndConstraint<BooleanAssertions> BeFalse(string because = "", params object[] becauseArgs) => throw null!;
            }
        }
        """;
}
