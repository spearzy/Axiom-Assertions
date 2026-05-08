using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.FluentAssertionsMigration;

internal sealed class FluentAssertionsMigrationSymbols
{
    private FluentAssertionsMigrationSymbols(
        Compilation compilation,
        INamedTypeSymbol? assertionExtensionsType,
        INamedTypeSymbol? enumerableType,
        INamedTypeSymbol? genericEnumerableType,
        INamedTypeSymbol? asyncEnumerableType,
        INamedTypeSymbol? actionType,
        INamedTypeSymbol? funcType,
        INamedTypeSymbol? taskType,
        INamedTypeSymbol? genericTaskType,
        INamedTypeSymbol? valueTaskType,
        INamedTypeSymbol? genericValueTaskType)
    {
        Compilation = compilation;
        AssertionExtensionsType = assertionExtensionsType;
        EnumerableType = enumerableType;
        GenericEnumerableType = genericEnumerableType;
        AsyncEnumerableType = asyncEnumerableType;
        ActionType = actionType;
        FuncType = funcType;
        TaskType = taskType;
        GenericTaskType = genericTaskType;
        ValueTaskType = valueTaskType;
        GenericValueTaskType = genericValueTaskType;
    }

    public Compilation Compilation { get; }
    private INamedTypeSymbol? AssertionExtensionsType { get; }
    private INamedTypeSymbol? EnumerableType { get; }
    private INamedTypeSymbol? GenericEnumerableType { get; }
    private INamedTypeSymbol? AsyncEnumerableType { get; }
    private INamedTypeSymbol? ActionType { get; }
    private INamedTypeSymbol? FuncType { get; }
    private INamedTypeSymbol? TaskType { get; }
    private INamedTypeSymbol? GenericTaskType { get; }
    private INamedTypeSymbol? ValueTaskType { get; }
    private INamedTypeSymbol? GenericValueTaskType { get; }

    public bool IsEnabled => AssertionExtensionsType is not null;

    public static FluentAssertionsMigrationSymbols Create(Compilation compilation)
    {
        return new FluentAssertionsMigrationSymbols(
            compilation,
            compilation.GetTypeByMetadataName("FluentAssertions.AssertionExtensions"),
            compilation.GetTypeByMetadataName("System.Collections.IEnumerable"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1"),
            compilation.GetTypeByMetadataName("System.Action"),
            compilation.GetTypeByMetadataName("System.Func`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask"),
            compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1"));
    }

    public bool IsFluentAssertionsShould(IMethodSymbol method)
    {
        var originalMethod = method.ReducedFrom ?? method;
        return AssertionExtensionsType is not null &&
               originalMethod.Name == "Should" &&
               (originalMethod.IsExtensionMethod || method.ReducedFrom is not null) &&
               originalMethod.ContainingType is not null &&
               SymbolEqualityComparer.Default.Equals(originalMethod.ContainingType.OriginalDefinition, AssertionExtensionsType);
    }

    public static bool IsFluentAssertionsAssertionMethod(IMethodSymbol method)
    {
        var containingNamespace = method.ContainingType?.ContainingNamespace?.ToDisplayString();
        return containingNamespace is not null &&
               (containingNamespace == "FluentAssertions" || containingNamespace.StartsWith("FluentAssertions.", StringComparison.Ordinal));
    }

    public bool SupportsEqualityMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsNullMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || !UsesSpecializedShouldReceiver(type);

    public bool SupportsReferenceEqualityMigrationReceiver(ITypeSymbol type)
        => !UsesSpecializedShouldReceiver(type);

    public bool SupportsTypeAssertionMigrationReceiver(ITypeSymbol type)
        => !UsesSpecializedShouldReceiver(type);

    public bool SupportsStringMigrationReceiver(ITypeSymbol type)
        => IsStringType(type);

    public bool SupportsEmptyMigrationReceiver(ITypeSymbol type)
        => IsStringType(type) || (IsEnumerableLike(type) && !IsAsyncEnumerableLike(type));

    public bool RequiresAssertionsExtensionsNamespace(
        FluentAssertionsMigrationKind kind,
        ITypeSymbol subjectType)
    {
        return kind switch
        {
            FluentAssertionsMigrationKind.BeTrue => true,
            FluentAssertionsMigrationKind.BeFalse => true,
            FluentAssertionsMigrationKind.BeEmpty => !IsStringType(subjectType),
            FluentAssertionsMigrationKind.NotBeEmpty => !IsStringType(subjectType),
            _ => false,
        };
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

    private bool IsEnumerableLike(ITypeSymbol type)
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

    private bool IsAsyncEnumerableLike(ITypeSymbol type)
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

    private static bool IsStringType(ITypeSymbol type)
        => type.SpecialType == SpecialType.System_String;

    private static bool ImplementsInterface(ITypeSymbol type, INamedTypeSymbol? interfaceType)
    {
        if (interfaceType is null)
        {
            return false;
        }

        foreach (var candidate in type.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(candidate.OriginalDefinition, interfaceType))
            {
                return true;
            }
        }

        return false;
    }
}
