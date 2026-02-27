namespace Axiom;

public static class Assert
{
    public static Batch Batch(string? name = null) => new(name);
}
