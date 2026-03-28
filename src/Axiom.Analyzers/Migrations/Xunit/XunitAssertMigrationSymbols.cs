using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.XunitMigration;

internal sealed class XunitAssertMigrationSymbols
{
    private XunitAssertMigrationSymbols(
        INamedTypeSymbol? xunitAssertType,
        INamedTypeSymbol? enumerableType,
        INamedTypeSymbol? genericEnumerableType,
        INamedTypeSymbol? asyncEnumerableType,
        INamedTypeSymbol? spanType,
        INamedTypeSymbol? readOnlySpanType,
        INamedTypeSymbol? memoryType,
        INamedTypeSymbol? readOnlyMemoryType)
    {
        XunitAssertType = xunitAssertType;
        EnumerableType = enumerableType;
        GenericEnumerableType = genericEnumerableType;
        AsyncEnumerableType = asyncEnumerableType;
        SpanType = spanType;
        ReadOnlySpanType = readOnlySpanType;
        MemoryType = memoryType;
        ReadOnlyMemoryType = readOnlyMemoryType;
    }

    public INamedTypeSymbol? XunitAssertType { get; }

    private INamedTypeSymbol? EnumerableType { get; }
    private INamedTypeSymbol? GenericEnumerableType { get; }
    private INamedTypeSymbol? AsyncEnumerableType { get; }
    private INamedTypeSymbol? SpanType { get; }
    private INamedTypeSymbol? ReadOnlySpanType { get; }
    private INamedTypeSymbol? MemoryType { get; }
    private INamedTypeSymbol? ReadOnlyMemoryType { get; }

    public bool IsEnabled => XunitAssertType is not null;

    public static XunitAssertMigrationSymbols Create(Compilation compilation)
    {
        return new XunitAssertMigrationSymbols(
            compilation.GetTypeByMetadataName("Xunit.Assert"),
            compilation.GetTypeByMetadataName("System.Collections.IEnumerable"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Span`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlySpan`1"),
            compilation.GetTypeByMetadataName("System.Memory`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlyMemory`1"));
    }

    public bool IsXunitAssert(INamedTypeSymbol? containingType)
        => XunitAssertType is not null &&
           containingType is not null &&
           SymbolEqualityComparer.Default.Equals(containingType.OriginalDefinition, XunitAssertType);

    public bool IsEnumerableLike(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type is IArrayTypeSymbol)
        {
            return true;
        }

        if (EnumerableType is not null &&
            SymbolEqualityComparer.Default.Equals(type, EnumerableType))
        {
            return true;
        }

        if (GenericEnumerableType is not null &&
            type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, GenericEnumerableType))
        {
            return true;
        }

        return ImplementsInterface(type, EnumerableType) || ImplementsInterface(type, GenericEnumerableType);
    }

    public bool IsAsyncEnumerableLike(ITypeSymbol type)
    {
        if (AsyncEnumerableType is null)
        {
            return false;
        }

        if (type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, AsyncEnumerableType))
        {
            return true;
        }

        return ImplementsInterface(type, AsyncEnumerableType);
    }

    public bool IsSpanOrMemoryLike(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var originalDefinition = namedType.OriginalDefinition;
        return SymbolEqualityComparer.Default.Equals(originalDefinition, SpanType) ||
               SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlySpanType) ||
               SymbolEqualityComparer.Default.Equals(originalDefinition, MemoryType) ||
               SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlyMemoryType);
    }

    private static bool ImplementsInterface(ITypeSymbol type, INamedTypeSymbol? interfaceType)
    {
        if (interfaceType is null)
        {
            return false;
        }

        foreach (var candidate in type.AllInterfaces)
        {
            var originalDefinition = candidate.OriginalDefinition;
            if (SymbolEqualityComparer.Default.Equals(candidate, interfaceType) ||
                SymbolEqualityComparer.Default.Equals(originalDefinition, interfaceType))
            {
                return true;
            }
        }

        return false;
    }
}
