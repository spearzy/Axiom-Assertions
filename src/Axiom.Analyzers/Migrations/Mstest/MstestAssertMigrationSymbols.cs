using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.MstestMigration;

internal sealed class MstestAssertMigrationSymbols
{
    private MstestAssertMigrationSymbols(
        Compilation compilation,
        INamedTypeSymbol? mstestAssertType,
        INamedTypeSymbol? mstestCollectionAssertType,
        INamedTypeSymbol? mstestStringAssertType,
        INamedTypeSymbol? enumerableType,
        INamedTypeSymbol? genericEnumerableType,
        INamedTypeSymbol? asyncEnumerableType,
        INamedTypeSymbol? dictionaryType,
        INamedTypeSymbol? readOnlyDictionaryType,
        INamedTypeSymbol? nonGenericDictionaryType,
        INamedTypeSymbol? actionType,
        INamedTypeSymbol? funcType,
        INamedTypeSymbol? taskType,
        INamedTypeSymbol? genericTaskType,
        INamedTypeSymbol? valueTaskType,
        INamedTypeSymbol? genericValueTaskType,
        INamedTypeSymbol? spanType,
        INamedTypeSymbol? readOnlySpanType,
        INamedTypeSymbol? memoryType,
        INamedTypeSymbol? readOnlyMemoryType)
    {
        Compilation = compilation;
        MstestAssertType = mstestAssertType;
        MstestCollectionAssertType = mstestCollectionAssertType;
        MstestStringAssertType = mstestStringAssertType;
        EnumerableType = enumerableType;
        GenericEnumerableType = genericEnumerableType;
        AsyncEnumerableType = asyncEnumerableType;
        DictionaryType = dictionaryType;
        ReadOnlyDictionaryType = readOnlyDictionaryType;
        NonGenericDictionaryType = nonGenericDictionaryType;
        ActionType = actionType;
        FuncType = funcType;
        TaskType = taskType;
        GenericTaskType = genericTaskType;
        ValueTaskType = valueTaskType;
        GenericValueTaskType = genericValueTaskType;
        SpanType = spanType;
        ReadOnlySpanType = readOnlySpanType;
        MemoryType = memoryType;
        ReadOnlyMemoryType = readOnlyMemoryType;
    }

    public Compilation Compilation { get; }
    public INamedTypeSymbol? MstestAssertType { get; }
    public INamedTypeSymbol? MstestCollectionAssertType { get; }
    public INamedTypeSymbol? MstestStringAssertType { get; }
    private INamedTypeSymbol? EnumerableType { get; }
    private INamedTypeSymbol? GenericEnumerableType { get; }
    private INamedTypeSymbol? AsyncEnumerableType { get; }
    private INamedTypeSymbol? DictionaryType { get; }
    private INamedTypeSymbol? ReadOnlyDictionaryType { get; }
    private INamedTypeSymbol? NonGenericDictionaryType { get; }
    private INamedTypeSymbol? ActionType { get; }
    private INamedTypeSymbol? FuncType { get; }
    private INamedTypeSymbol? TaskType { get; }
    private INamedTypeSymbol? GenericTaskType { get; }
    private INamedTypeSymbol? ValueTaskType { get; }
    private INamedTypeSymbol? GenericValueTaskType { get; }
    private INamedTypeSymbol? SpanType { get; }
    private INamedTypeSymbol? ReadOnlySpanType { get; }
    private INamedTypeSymbol? MemoryType { get; }
    private INamedTypeSymbol? ReadOnlyMemoryType { get; }

    public bool IsEnabled =>
        MstestAssertType is not null ||
        MstestCollectionAssertType is not null ||
        MstestStringAssertType is not null;

    public static MstestAssertMigrationSymbols Create(Compilation compilation)
    {
        return new MstestAssertMigrationSymbols(
            compilation,
            compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert"),
            compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert"),
            compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert"),
            compilation.GetTypeByMetadataName("System.Collections.IEnumerable"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IDictionary`2"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyDictionary`2"),
            compilation.GetTypeByMetadataName("System.Collections.IDictionary"),
            compilation.GetTypeByMetadataName("System.Action"),
            compilation.GetTypeByMetadataName("System.Func`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1"),
            compilation.GetTypeByMetadataName("System.Span`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlySpan`1"),
            compilation.GetTypeByMetadataName("System.Memory`1"),
            compilation.GetTypeByMetadataName("System.ReadOnlyMemory`1"));
    }

    public bool IsSupportedAssertType(
        INamedTypeSymbol? containingType,
        MstestAssertTarget target)
    {
        if (containingType is null)
        {
            return false;
        }

        var originalDefinition = containingType.OriginalDefinition;
        return target switch
        {
            MstestAssertTarget.Assert => MstestAssertType is not null &&
                SymbolEqualityComparer.Default.Equals(originalDefinition, MstestAssertType),
            MstestAssertTarget.CollectionAssert => MstestCollectionAssertType is not null &&
                SymbolEqualityComparer.Default.Equals(originalDefinition, MstestCollectionAssertType),
            MstestAssertTarget.StringAssert => MstestStringAssertType is not null &&
                SymbolEqualityComparer.Default.Equals(originalDefinition, MstestStringAssertType),
            _ => false,
        };
    }

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

        if (EnumerableType is not null && SymbolEqualityComparer.Default.Equals(type, EnumerableType))
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

    public bool IsGenericEnumerableLike(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type is IArrayTypeSymbol)
        {
            return true;
        }

        if (GenericEnumerableType is not null &&
            type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, GenericEnumerableType))
        {
            return true;
        }

        return ImplementsInterface(type, GenericEnumerableType);
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
        return (SpanType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, SpanType)) ||
               (ReadOnlySpanType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlySpanType)) ||
               (MemoryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, MemoryType)) ||
               (ReadOnlyMemoryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlyMemoryType));
    }

    public bool SupportsEqualityMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsNullMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsReferenceEqualityMigrationReceiver(ITypeSymbol type)
        => !UsesSpecializedShouldReceiver(type);

    public bool SupportsTypeAssertionMigrationReceiver(ITypeSymbol type)
        => !UsesSpecializedShouldReceiver(type);

    public bool SupportsStringContainmentMigrationReceiver(ITypeSymbol type)
        => IsStringType(type);

    public bool SupportsCollectionContainmentMigrationReceiver(ITypeSymbol type)
        => IsGenericEnumerableLike(type) &&
           !IsAsyncEnumerableLike(type) &&
           !IsDictionaryLike(type);

    public bool TryGetEnumerableElementType(ITypeSymbol type, out ITypeSymbol? elementType)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            elementType = arrayType.ElementType;
            return true;
        }

        if (GenericEnumerableType is not null &&
            type is INamedTypeSymbol namedType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, GenericEnumerableType) &&
            namedType.TypeArguments.Length == 1)
        {
            elementType = namedType.TypeArguments[0];
            return true;
        }

        foreach (var candidate in type.AllInterfaces)
        {
            if (GenericEnumerableType is not null &&
                SymbolEqualityComparer.Default.Equals(candidate.OriginalDefinition, GenericEnumerableType) &&
                candidate.TypeArguments.Length == 1)
            {
                elementType = candidate.TypeArguments[0];
                return true;
            }
        }

        elementType = null;
        return false;
    }

    public bool IsDictionaryLike(ITypeSymbol type)
    {
        if (NonGenericDictionaryType is not null &&
            SymbolEqualityComparer.Default.Equals(type, NonGenericDictionaryType))
        {
            return true;
        }

        if (type is INamedTypeSymbol namedType)
        {
            var originalDefinition = namedType.OriginalDefinition;
            if ((DictionaryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, DictionaryType)) ||
                (ReadOnlyDictionaryType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, ReadOnlyDictionaryType)))
            {
                return true;
            }
        }

        return ImplementsInterface(type, NonGenericDictionaryType) ||
               ImplementsInterface(type, DictionaryType) ||
               ImplementsInterface(type, ReadOnlyDictionaryType);
    }

    public static bool IsNullableOrReferenceType(ITypeSymbol type)
    {
        if (type.IsReferenceType)
        {
            return true;
        }

        return type is INamedTypeSymbol namedType &&
               namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    private bool UsesSpecializedShouldReceiver(ITypeSymbol type)
    {
        if (ActionType is not null && SymbolEqualityComparer.Default.Equals(type, ActionType))
        {
            return true;
        }

        if (IsStringType(type))
        {
            return true;
        }

        if (IsTaskLike(type) || IsAsyncEnumerableLike(type))
        {
            return true;
        }

        return IsFuncReturningTaskLike(type);
    }

    private static bool IsStringType(ITypeSymbol type)
        => type.SpecialType == SpecialType.System_String;

    private bool IsTaskLike(ITypeSymbol type)
    {
        if (TaskType is not null && SymbolEqualityComparer.Default.Equals(type, TaskType))
        {
            return true;
        }

        if (ValueTaskType is not null && SymbolEqualityComparer.Default.Equals(type, ValueTaskType))
        {
            return true;
        }

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var originalDefinition = namedType.OriginalDefinition;
        return (GenericTaskType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, GenericTaskType)) ||
               (GenericValueTaskType is not null && SymbolEqualityComparer.Default.Equals(originalDefinition, GenericValueTaskType));
    }

    private bool IsFuncReturningTaskLike(ITypeSymbol type)
    {
        if (FuncType is null ||
            type is not INamedTypeSymbol namedType ||
            !SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, FuncType) ||
            namedType.TypeArguments.Length != 1)
        {
            return false;
        }

        return IsTaskLike(namedType.TypeArguments[0]);
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
