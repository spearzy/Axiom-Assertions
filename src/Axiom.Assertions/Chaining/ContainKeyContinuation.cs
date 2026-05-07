using System.ComponentModel;
using System.Diagnostics;
using Axiom.Core.Failures;

namespace Axiom.Assertions.Chaining;

public readonly struct ContainKeyContinuation<TAssertions, TValue>
{
    private readonly bool _hasValue;
    private readonly TValue _value;
    private readonly string? _containKeyFailureMessage;

    public ContainKeyContinuation(
        TAssertions assertions,
        bool hasValue,
        TValue value,
        string? containKeyFailureMessage)
    {
        And = assertions;
        _hasValue = hasValue;
        _value = value;
        _containKeyFailureMessage = containKeyFailureMessage;
    }

    public TAssertions And { get; }

    public TValue Value => GetValueOrThrow("Value");

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TValue WhoseValue => GetValueOrThrow("WhoseValue");

    private TValue GetValueOrThrow(string accessorName)
    {
        if (_hasValue)
        {
            return _value;
        }

        var message = _containKeyFailureMessage is null
            ? $"{accessorName} is unavailable because ContainKey failed."
            : $"{accessorName} is unavailable because ContainKey failed with error: {_containKeyFailureMessage}";
        AssertionFailureDispatcher.Throw(message);
        throw new UnreachableException();
    }
}
