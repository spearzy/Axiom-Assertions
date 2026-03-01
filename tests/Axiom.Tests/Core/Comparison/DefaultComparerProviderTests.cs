namespace Axiom.Tests.Core.Comparison;

public sealed class DefaultComparerProviderTests
{
    [Fact]
    public void TryGetEqualityComparer_ReturnsTrue_ForString()
    {
        var provider = DefaultComparerProvider.Instance;

        var found = provider.TryGetEqualityComparer<string>(out var comparer);

        Assert.True(found);
        Assert.NotNull(comparer);
        Assert.True(comparer!.Equals("a", "a"));
        Assert.False(comparer.Equals("a", "b"));
    }

    [Fact]
    public void TryGetEqualityComparer_ReturnsTrue_ForInt()
    {
        var provider = DefaultComparerProvider.Instance;

        var found = provider.TryGetEqualityComparer<int>(out var comparer);

        Assert.True(found);
        Assert.NotNull(comparer);
        Assert.True(comparer!.Equals(42, 42));
        Assert.False(comparer.Equals(42, 43));
    }
}
