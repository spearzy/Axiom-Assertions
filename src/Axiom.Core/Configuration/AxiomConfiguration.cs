namespace Axiom;

public sealed class AxiomConfiguration
{
    public IComparerProvider ComparerProvider { get; set; } = DefaultComparerProvider.Instance;
    public IValueFormatter ValueFormatter { get; set; } = DefaultValueFormatter.Instance;
}
