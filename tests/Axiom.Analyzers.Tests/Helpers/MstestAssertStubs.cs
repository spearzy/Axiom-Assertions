namespace Axiom.Analyzers.Tests.Helpers;

internal static class MstestAssertStubs
{
    public const string Source =
        """
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        namespace Microsoft.VisualStudio.TestTools.UnitTesting
        {
            public static class Assert
            {
                public static void AreEqual<T>(T expected, T actual) { }
                public static void AreEqual<T>(T expected, T actual, string message) { }
                public static void AreEqual(double expected, double actual, double delta) { }
                public static void AreEqual(string? expected, string? actual, bool ignoreCase) { }
                public static void AreNotEqual<T>(T expected, T actual) { }
                public static void AreNotEqual<T>(T expected, T actual, string message) { }
                public static void AreNotEqual(double expected, double actual, double delta) { }
                public static void AreNotEqual(string? expected, string? actual, bool ignoreCase) { }
                public static void IsNull(object? value) { }
                public static void IsNull(object? value, string message) { }
                public static void IsNotNull(object? value) { }
                public static void IsNotNull(object? value, string message) { }
                public static void IsTrue(bool condition) { }
                public static void IsTrue(bool condition, string message) { }
                public static void IsFalse(bool condition) { }
                public static void IsFalse(bool condition, string message) { }
                public static void AreSame(object? expected, object? actual) { }
                public static void AreSame(object? expected, object? actual, string message) { }
                public static void AreNotSame(object? expected, object? actual) { }
                public static void AreNotSame(object? expected, object? actual, string message) { }
                public static void IsInstanceOfType(object? value, Type expectedType) { }
                public static void IsInstanceOfType(object? value, Type expectedType, string message) { }
                public static void IsNotInstanceOfType(object? value, Type wrongType) { }
                public static void IsNotInstanceOfType(object? value, Type wrongType, string message) { }
                public static Task<TException> ThrowsExceptionAsync<TException>(Func<Task> action) where TException : Exception => throw null!;
                public static Task<TException> ThrowsExceptionAsync<TException>(Func<Task> action, string message) where TException : Exception => throw null!;
                public static Task<TException> ThrowsExceptionAsync<TException>(Func<Task> action, string message, params object[] parameters) where TException : Exception => throw null!;
                public static Task<TException> ThrowsExactlyAsync<TException>(Func<Task> action, string message = "", params object[] parameters) where TException : Exception => throw null!;
                public static Task<TException> ThrowsAsync<TException>(Func<Task> action, string message = "", params object[] parameters) where TException : Exception => throw null!;
                public static Task<TException> ThrowsAsync<TException>(Func<Task> action, Func<Exception?, string> messageBuilder) where TException : Exception => throw null!;
                public static void IsGreaterThan<T>(T lowerBound, T value, string message = "") where T : IComparable<T> { }
                public static void IsGreaterThanOrEqualTo<T>(T lowerBound, T value, string message = "") where T : IComparable<T> { }
                public static void IsLessThan<T>(T upperBound, T value, string message = "") where T : IComparable<T> { }
                public static void IsLessThanOrEqualTo<T>(T upperBound, T value, string message = "") where T : IComparable<T> { }
                public static void IsInRange<T>(T minValue, T maxValue, T value, string message = "") where T : struct { }
                public static void Contains(string substring, string value) { }
                public static void Contains(string substring, string value, StringComparison comparisonType) { }
                public static void Contains(string substring, string value, string message) { }
                public static void Contains<T>(T expected, IEnumerable<T> collection) { }
                public static void Contains<T>(T expected, IEnumerable<T> collection, IEqualityComparer<T> comparer) { }
                public static void DoesNotContain(string substring, string value) { }
                public static void DoesNotContain(string substring, string value, StringComparison comparisonType) { }
                public static void DoesNotContain(string substring, string value, string message) { }
                public static void DoesNotContain<T>(T unexpected, IEnumerable<T> collection) { }
                public static void DoesNotContain<T>(T unexpected, IEnumerable<T> collection, IEqualityComparer<T> comparer) { }
                public static void StartsWith(string expectedPrefix, string value) { }
                public static void StartsWith(string expectedPrefix, string value, StringComparison comparisonType) { }
                public static void StartsWith(string expectedPrefix, string value, string message) { }
                public static void EndsWith(string expectedSuffix, string value) { }
                public static void EndsWith(string expectedSuffix, string value, StringComparison comparisonType) { }
                public static void EndsWith(string expectedSuffix, string value, string message) { }
            }

            public static class StringAssert
            {
                public static void Contains(string value, string substring) { }
                public static void Contains(string value, string substring, string message) { }
                public static void StartsWith(string value, string prefix) { }
                public static void StartsWith(string value, string prefix, string message) { }
                public static void EndsWith(string value, string suffix) { }
                public static void EndsWith(string value, string suffix, string message) { }
            }

            public static class CollectionAssert
            {
                public static void Contains(ICollection? collection, object? element) { }
                public static void Contains(ICollection? collection, object? element, string message) { }
                public static void DoesNotContain(ICollection? collection, object? element) { }
                public static void DoesNotContain(ICollection? collection, object? element, string message) { }
                public static void AllItemsAreUnique(ICollection? collection) { }
                public static void AllItemsAreUnique(ICollection? collection, string message) { }
                public static void AreEqual(ICollection? expected, ICollection? actual) { }
                public static void AreEquivalent(ICollection? expected, ICollection? actual) { }
            }
        }
        """;
}
