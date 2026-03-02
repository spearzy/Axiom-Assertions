using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Axiom.Assertions.Equivalency;

internal static class EquivalencyEngine
{
    public static IReadOnlyList<EquivalencyDifference> Compare(
        object? actual,
        object? expected,
        string rootPath,
        EquivalencyOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentNullException.ThrowIfNull(options);

        var differences = new List<EquivalencyDifference>();
        var visitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, rootPath, options, differences, visitedPairs);
        return differences;
    }

    private static void CompareNode(
        object? actual,
        object? expected,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        if (IsPathIgnored(path, options))
        {
            return;
        }

        if (ReferenceEquals(actual, expected))
        {
            return;
        }

        if (actual is null || expected is null)
        {
            differences.Add(new EquivalencyDifference(path, expected, actual, "One value was <null> and the other was not."));
            return;
        }

        var actualType = actual.GetType();
        var expectedType = expected.GetType();
        if (!AreTypesCompatible(actualType, expectedType, options))
        {
            differences.Add(new EquivalencyDifference(
                path,
                expected,
                actual,
                $"Runtime types differ: expected {expectedType.FullName}, but found {actualType.FullName}."));
            return;
        }

        if (actual is string actualString && expected is string expectedString)
        {
            if (!string.Equals(actualString, expectedString, options.StringComparison))
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "String values differ."));
            }

            return;
        }

        if (actual is IEnumerable actualEnumerable &&
            expected is IEnumerable expectedEnumerable &&
            actual is not string &&
            expected is not string)
        {
            CompareEnumerable(actualEnumerable, expectedEnumerable, path, options, differences, visitedPairs);
            return;
        }

        if (IsLeafType(actualType) || IsLeafType(expectedType))
        {
            if (!Equals(actual, expected))
            {
                differences.Add(new EquivalencyDifference(path, expected, actual, "Values differ."));
            }

            return;
        }

        if (!actualType.IsValueType && !expectedType.IsValueType)
        {
            // Cycles should not fail: if this pair was already compared, stop walking here.
            var pair = new ReferencePair(actual, expected);
            if (!visitedPairs.Add(pair))
            {
                return;
            }
        }

        CompareMembers(actual, expected, actualType, expectedType, path, options, differences, visitedPairs);
    }

    private static void CompareMembers(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        var actualMembers = GetComparableMembers(actualType, options);
        var expectedMembers = GetComparableMembers(expectedType, options);

        var memberNames = actualMembers.Keys
            .Concat(expectedMembers.Keys)
            .Distinct(StringComparer.Ordinal)
            // Stable ordering keeps failure output deterministic across runtimes.
            .OrderBy(static name => name, StringComparer.Ordinal);

        foreach (var memberName in memberNames)
        {
            if (options.IgnoredMemberNames.Contains(memberName))
            {
                continue;
            }

            var memberPath = $"{path}.{memberName}";
            if (IsPathIgnored(memberPath, options))
            {
                continue;
            }

            var hasActual = actualMembers.TryGetValue(memberName, out var actualGetter);
            var hasExpected = expectedMembers.TryGetValue(memberName, out var expectedGetter);

            if (!hasActual && hasExpected)
            {
                var expectedValue = expectedGetter!(expected);
                differences.Add(new EquivalencyDifference(memberPath, expectedValue, null, "Member missing on actual type."));
                continue;
            }

            if (hasActual && !hasExpected)
            {
                var actualValue = actualGetter!(actual);
                differences.Add(new EquivalencyDifference(memberPath, null, actualValue, "Member missing on expected type."));
                continue;
            }

            var actualValueAtMember = actualGetter!(actual);
            var expectedValueAtMember = expectedGetter!(expected);
            CompareNode(actualValueAtMember, expectedValueAtMember, memberPath, options, differences, visitedPairs);
        }
    }

    private static void CompareEnumerable(
        IEnumerable actualEnumerable,
        IEnumerable expectedEnumerable,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences,
        HashSet<ReferencePair> visitedPairs)
    {
        var actualItems = actualEnumerable.Cast<object?>().ToList();
        var expectedItems = expectedEnumerable.Cast<object?>().ToList();

        if (options.CollectionOrder == EquivalencyCollectionOrder.Any)
        {
            CompareEnumerableAnyOrder(actualItems, expectedItems, path, options, differences);
            return;
        }

        var sharedCount = Math.Min(actualItems.Count, expectedItems.Count);
        for (var index = 0; index < sharedCount; index++)
        {
            CompareNode(actualItems[index], expectedItems[index], $"{path}[{index}]", options, differences, visitedPairs);
        }

        for (var index = sharedCount; index < expectedItems.Count; index++)
        {
            differences.Add(new EquivalencyDifference(
                $"{path}[{index}]",
                expectedItems[index],
                null,
                "Item missing on actual collection."));
        }

        for (var index = sharedCount; index < actualItems.Count; index++)
        {
            differences.Add(new EquivalencyDifference(
                $"{path}[{index}]",
                null,
                actualItems[index],
                "Extra item on actual collection."));
        }
    }

    private static void CompareEnumerableAnyOrder(
        List<object?> actualItems,
        List<object?> expectedItems,
        string path,
        EquivalencyOptions options,
        List<EquivalencyDifference> differences)
    {
        // Simple one-pass matching: each actual element can satisfy at most one expected element.
        var usedActualIndexes = new bool[actualItems.Count];

        for (var expectedIndex = 0; expectedIndex < expectedItems.Count; expectedIndex++)
        {
            var expectedItem = expectedItems[expectedIndex];
            var matched = false;
            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActualIndexes[actualIndex])
                {
                    continue;
                }

                // In Any-order mode we still require deep equivalency for each candidate pair.
                if (!ItemsEquivalentDeep(
                        actualItems[actualIndex],
                        expectedItem,
                        $"{path}[{expectedIndex}]",
                        options))
                {
                    continue;
                }

                usedActualIndexes[actualIndex] = true;
                matched = true;
                break;
            }

            if (!matched)
            {
                differences.Add(new EquivalencyDifference(
                    $"{path}[{expectedIndex}]",
                    expectedItem,
                    null,
                    "Expected item was not found in actual collection."));
            }
        }

        for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
        {
            if (usedActualIndexes[actualIndex])
            {
                continue;
            }

            differences.Add(new EquivalencyDifference(
                $"{path}[{actualIndex}]",
                null,
                actualItems[actualIndex],
                "Actual collection contains an extra item."));
        }
    }

    private static bool ItemsEquivalentDeep(
        object? actual,
        object? expected,
        string path,
        EquivalencyOptions options)
    {
        var localDifferences = new List<EquivalencyDifference>();
        var localVisitedPairs = new HashSet<ReferencePair>(ReferencePairComparer.Instance);
        CompareNode(actual, expected, path, options, localDifferences, localVisitedPairs);
        return localDifferences.Count == 0;
    }

    private static Dictionary<string, Func<object, object?>> GetComparableMembers(Type type, EquivalencyOptions options)
    {
        var members = new Dictionary<string, Func<object, object?>>(StringComparer.Ordinal);

        if (options.IncludePublicProperties)
        {
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetMethod is null || property.GetMethod.IsStatic)
                {
                    continue;
                }

                if (property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                members.TryAdd(property.Name, property.GetValue);
            }
        }

        if (options.IncludePublicFields)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsStatic)
                {
                    continue;
                }

                members.TryAdd(field.Name, field.GetValue);
            }
        }

        return members;
    }

    private static bool IsPathIgnored(string path, EquivalencyOptions options)
    {
        if (options.IgnoredPaths.Contains(path))
        {
            return true;
        }

        // Ignoring "a.b" also ignores children like "a.b.c" and "a.b[0]".
        foreach (var ignoredPath in options.IgnoredPaths)
        {
            if (path.StartsWith($"{ignoredPath}.", StringComparison.Ordinal) ||
                path.StartsWith($"{ignoredPath}[", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool AreTypesCompatible(Type actualType, Type expectedType, EquivalencyOptions options)
    {
        if (options.RequireStrictRuntimeTypes)
        {
            return actualType == expectedType;
        }

        return actualType.IsAssignableFrom(expectedType) || expectedType.IsAssignableFrom(actualType);
    }

    private static bool IsLeafType(Type type)
    {
        var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;

        if (nonNullableType.IsPrimitive || nonNullableType.IsEnum)
        {
            return true;
        }

        return nonNullableType == typeof(decimal) ||
               nonNullableType == typeof(string) ||
               nonNullableType == typeof(DateTime) ||
               nonNullableType == typeof(DateTimeOffset) ||
               nonNullableType == typeof(TimeSpan) ||
               nonNullableType == typeof(Guid);
    }

    private readonly record struct ReferencePair(object Actual, object Expected);

    private sealed class ReferencePairComparer : IEqualityComparer<ReferencePair>
    {
        public static ReferencePairComparer Instance { get; } = new();

        public bool Equals(ReferencePair x, ReferencePair y)
            => ReferenceEquals(x.Actual, y.Actual) && ReferenceEquals(x.Expected, y.Expected);

        public int GetHashCode(ReferencePair pair)
            => HashCode.Combine(RuntimeHelpers.GetHashCode(pair.Actual), RuntimeHelpers.GetHashCode(pair.Expected));
    }
}
