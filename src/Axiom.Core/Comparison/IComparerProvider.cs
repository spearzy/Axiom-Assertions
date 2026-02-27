namespace Axiom;

public interface IComparerProvider
{
    bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer);
}
