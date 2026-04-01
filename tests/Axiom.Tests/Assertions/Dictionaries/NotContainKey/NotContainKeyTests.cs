using Axiom.Assertions;
using Axiom.Assertions.Extensions;
using System.Runtime.CompilerServices;

namespace Axiom.Tests.Assertions.Dictionaries.NotContainKey;

public sealed class NotContainKeyTests
{
    [Fact]
    public void NotContainKey_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainKey("gamma");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainKey_Throws_WhenKeyExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContainKey("alpha"));

        const string expected = "Expected values to not contain key \"alpha\", but found key was present with value 1.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContainKey_IDictionary_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        IDictionary<string, int> values = new Dictionary<string, int>
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainKey("gamma");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainKey_DictionaryOnlyImplementation_ReturnsContinuation_WhenKeyDoesNotExist()
    {
        IDictionary<string, int> values = new DictionaryOnlyAdapter<string, int>(
            new Dictionary<string, int>
            {
                ["alpha"] = 1,
                ["beta"] = 2,
            });

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContainKey("gamma");

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContainKey_IDictionaryBackedByDictionary_DoesNotAllocateExtraWrapperPerCall()
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

        WarmUpNotContainKey(interfaceValues, concreteValues);

        var concreteAllocations = MeasureAllocations(() =>
        {
            for (var i = 0; i < iterations; i++)
            {
                _ = concreteValues.Should().NotContainKey("gamma");
            }
        });

        var interfaceAllocations = MeasureAllocations(() =>
        {
            for (var i = 0; i < iterations; i++)
            {
                _ = interfaceValues.Should().NotContainKey("gamma");
            }
        });

        var extraAllocations = interfaceAllocations - concreteAllocations;
        Assert.InRange(extraAllocations, long.MinValue, 4_096);
    }

    private static void WarmUpNotContainKey(IDictionary<string, int> interfaceValues, Dictionary<string, int> concreteValues)
    {
        for (var i = 0; i < 10; i++)
        {
            _ = interfaceValues.Should().NotContainKey("gamma");
            _ = concreteValues.Should().NotContainKey("gamma");
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
