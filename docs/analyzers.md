# Analyzers

Installing `Axiom.Assertions` gives you the Axiom Roslyn analyzers/code fixes automatically.

`Axiom.Analyzers` still exists as an optional standalone package if you only want the diagnostics without the runtime assertion library.

The first shipped rule focuses on one of the easiest errors to make with async assertions: calling the assertion, then dropping the returned `ValueTask` so the assertion never actually runs.

## Async Assertions Must Be Awaited

Rule: `AXM0001`

This rule flags ignored async Axiom assertion calls from:

- `AsyncActionAssertions`
- `AsyncFunctionAssertions<T>`
- `TaskAssertions`
- `TaskAssertions<T>`

Examples include:

- `ThrowAsync(...)`
- `NotThrowAsync()`
- `Succeed()`
- `SucceedWithin(...)`
- `BeFaultedWith(...)`

Async Axiom assertions are lazy until their returned `ValueTask` is awaited or returned. If the result is ignored, the test can appear to pass even though the assertion never executed.

## Before / After

Before:

```csharp
loader.Should().Succeed();
```

After:

```csharp
await loader.Should().Succeed();
```

The analyzer also offers a code fix in async contexts where prepending `await` is safe.
