using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using System.Runtime.CompilerServices;

namespace Axiom.Tests.Assertions.Dictionaries.ContainKey;

public sealed class ContainKeyTests
{
    [Fact]
    public void ContainKey_ReturnsContinuation_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainKey("alpha");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainKey_ExposesWhoseValue_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var continuation = values.Should().ContainKey("alpha");

        Assert.Equal(1, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_IReadOnlyDictionary_ExposesWhoseValue_WhenKeyExists()
    {
        IReadOnlyDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var continuation = values.Should().ContainKey("beta");

        Assert.Equal(2, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_IDictionary_ExposesWhoseValue_WhenKeyExists()
    {
        IDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var continuation = values.Should().ContainKey("alpha");

        Assert.Equal(1, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_DictionaryOnlyImplementation_ExposesWhoseValue_WhenKeyExists()
    {
        IDictionary<string, int> values = new DictionaryOnlyAdapter<string, int>(
            new Dictionary<string, int>
            {
                ["alpha"] = 1,
                ["beta"] = 2,
            });

        var continuation = values.Should().ContainKey("beta");

        Assert.Equal(2, continuation.WhoseValue);
    }

    [Fact]
    public void ContainKey_IDictionaryBackedByDictionary_DoesNotAllocateExtraWrapperPerCall()
    {
        const int iterations = 1_000;

        IDictionary<string, int> interfaceValues = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };
        Dictionary<string, int> concreteValues = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        WarmUpContainKey(interfaceValues, concreteValues);

        var concreteAllocations = MeasureAllocations(() =>
        {
            for (var i = 0; i < iterations; i++)
            {
                _ = concreteValues.Should().ContainKey("alpha");
            }
        });

        var interfaceAllocations = MeasureAllocations(() =>
        {
            for (var i = 0; i < iterations; i++)
            {
                _ = interfaceValues.Should().ContainKey("alpha");
            }
        });

        var extraAllocations = interfaceAllocations - concreteAllocations;
        Assert.InRange(extraAllocations, long.MinValue, 4_096);
    }

    [Fact]
    public void ContainKey_Throws_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["beta"] = 2,
            ["alpha"] = 1,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainKey("gamma"));

        const string expected = "Expected values to contain key \"gamma\", but found keys were [\"alpha\", \"beta\"].";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainKey_Throws_WithReason_WhenProvided()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainKey("gamma", "authorisation rules require a gamma key"));

        Assert.Contains("because authorisation rules require a gamma key", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WhoseValue_ThrowsExplicitMessage_WhenContainKeyFailedInsideBatch()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
        };

        var batch = new Axiom.Core.Batch();
        var continuation = values.Should().ContainKey("gamma");

        var ex = Assert.Throws<InvalidOperationException>(() => _ = continuation.WhoseValue);

        const string failureMessage = "Expected values to contain key \"gamma\", but found keys were [\"alpha\"].";
        var expected = $"WhoseValue is unavailable because ContainKey failed with error: {failureMessage}";
        Assert.Equal(expected, ex.Message);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    private static void WarmUpContainKey(IDictionary<string, int> interfaceValues, Dictionary<string, int> concreteValues)
    {
        for (var i = 0; i < 10; i++)
        {
            _ = interfaceValues.Should().ContainKey("alpha");
            _ = concreteValues.Should().ContainKey("alpha");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long MeasureAllocations(Action action)
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private sealed class DictionaryOnlyAdapter<TKey, TValue>(IDictionary<TKey, TValue> inner) : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        public TValue this[TKey key]
        {
            get => inner[key];
            set => inner[key] = value;
        }

        public ICollection<TKey> Keys => inner.Keys;
        public ICollection<TValue> Values => inner.Values;
        public int Count => inner.Count;
        public bool IsReadOnly => inner.IsReadOnly;

        public void Add(TKey key, TValue value) => inner.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => inner.Add(item);
        public void Clear() => inner.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => inner.Contains(item);
        public bool ContainsKey(TKey key) => inner.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => inner.GetEnumerator();
        public bool Remove(TKey key) => inner.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => inner.Remove(item);
        public bool TryGetValue(TKey key, out TValue value) => inner.TryGetValue(key, out value!);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
