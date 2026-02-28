namespace Axiom.Tests.Assertions.Values.Comparison;

public sealed class ValueAssertionComparerProviderTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Be_UsesConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new SameParityIntComparerProvider());

        var value = 3;
        var ex = Xunit.Record.Exception(() => value.Should().Be(5));

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void NotBe_UsesConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new SameParityIntComparerProvider());

        var value = 3;
        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().NotBe(7));

        const string expected = "Expected value to not be 7, but found 3.";
        Xunit.Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void Be_FallsBackToDefaultComparer_WhenProviderDoesNotSupplyOne()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new EmptyComparerProvider());

        var value = 42;
        var ex = Xunit.Record.Exception(() => value.Should().Be(42));

        Xunit.Assert.Null(ex);
    }

    private sealed class SameParityIntComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(int))
            {
                comparer = (IEqualityComparer<T>)(object)new SameParityIntComparer();
                return true;
            }

            comparer = null;
            return false;
        }
    }

    private sealed class SameParityIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }

    private sealed class EmptyComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            comparer = null;
            return false;
        }
    }
}
