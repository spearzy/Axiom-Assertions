namespace Axiom.Analyzers.Tests.Helpers;

internal static class XunitAssertStubs
{
    public const string Source =
        """
        using System.Collections;
        using System.Collections.Generic;

        namespace Xunit
        {
            public static class Assert
            {
                public static void Equal<T>(T expected, T actual) { }
                public static void Equal(string expected, string actual) { }
                public static void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual) { }
                public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
                public static void Equal(string expected, string actual, bool ignoreCase, bool ignoreLineEndingDifferences, bool ignoreWhiteSpaceDifferences) { }
                public static void Equal(double expected, double actual, int precision) { }

                public static void NotEqual<T>(T expected, T actual) { }
                public static void NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) { }
                public static void NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
                public static void NotEqual(double expected, double actual, int precision) { }

                public static void Null(object? value) { }
                public static T NotNull<T>(T value) => value;

                public static void True(bool condition) { }
                public static void True(bool condition, string userMessage) { }
                public static void True(bool? condition) { }

                public static void False(bool condition) { }
                public static void False(bool condition, string userMessage) { }
                public static void False(bool? condition) { }

                public static void Empty(IEnumerable collection) { }
                public static void Empty(string value) { }
                public static void Empty<T>(IAsyncEnumerable<T> value) { }

                public static void NotEmpty(IEnumerable collection) { }
                public static void NotEmpty(string value) { }
                public static void NotEmpty<T>(IAsyncEnumerable<T> value) { }
            }
        }
        """;
}
