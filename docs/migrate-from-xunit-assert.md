---
title: Migrate from xUnit Assert to Axiom
description: A practical staged walkthrough for moving xUnit Assert tests to Axiom Assertions with analyzer-backed rewrites and manual review boundaries.
---

# Migrate from xUnit Assert to Axiom

The xUnit path is the strongest analyzer-backed migration path in Axiom today. It is still intentionally conservative: the analyzer helps with safe, direct rewrites and leaves ambiguous tests alone.

## When This Migration Path Is A Good Fit

This path is a good fit when your xUnit tests mostly use straightforward `Assert.*` calls and you want to move gradually to fluent assertions without redesigning the whole suite at once.

It works best when tests already check one clear fact per assertion: equality, nullability, string containment, collection membership, dictionary keys, single-item expectations, type checks, or direct exception assertions.

It is a weaker fit when your suite leans heavily on custom comparers, precision rules, string-equality options, inspectors, or broad structural comparisons.

## What Axiom Can Rewrite Safely Today

The xUnit analyzer support covers common scalar, string, dictionary-key, `Single(...)`, and exception shapes when the rewrite preserves value flow.

Examples include:

- `Assert.Equal(expected, actual)` to `actual.Should().Be(expected)`
- `Assert.Contains("sub", actual, StringComparison.OrdinalIgnoreCase)` to `actual.Should().Contain("sub", StringComparison.OrdinalIgnoreCase)`
- `Assert.Contains(key, lookup)` to `lookup.Should().ContainKey(key)` and `.WhoseValue` when the old returned value was used
- `Assert.Single(values)` and `Assert.Single(values, predicate)` to `ContainSingle(...)` and `.SingleItem` when needed
- `Assert.Throws<TException>(...)`, awaited `Assert.ThrowsAsync<TException>(...)`, and awaited `Assert.ThrowsAnyAsync<TException>(...)` when the target Axiom exception assertion is exact

For exact rule IDs and edge cases, use the [Analyzer reference](analyzers.md).

## A Recommended First Migration Pass

Start with one project or folder. Apply only analyzer-provided code fixes, then review the diff with this question: did the assertion intent stay the same?

Good first-pass candidates:

- scalar checks
- simple string checks
- simple collection checks
- dictionary key checks
- `Single(...)` checks where the returned value flow remains obvious
- direct exception assertions

Do not use the first pass to redesign structural assertions. Mark those for manual review.

## Before/After Examples

Scalar and string assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.Equal(expected, actual);
Assert.NotNull(actual);
Assert.Contains("sub", actual);
Assert.Contains("sub", actual, StringComparison.OrdinalIgnoreCase);
Assert.StartsWith("pre", actual);
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
actual.Should().NotBeNull();
actual.Should().Contain("sub");
actual.Should().Contain("sub", StringComparison.OrdinalIgnoreCase);
actual.Should().StartWith("pre");
```

Dictionary-key and `Single(...)` assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.Contains(key, lookup);
var found = Assert.Contains(key, lookup);
var item = Assert.Single(values);
var match = Assert.Single(values, IsPositive);
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
lookup.Should().ContainKey(key);
var found = lookup.Should().ContainKey(key).WhoseValue;
var item = values.Should().ContainSingle().SingleItem;
var match = values.Should().ContainSingle(IsPositive).SingleItem;
```

Exception assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.Throws<InvalidOperationException>(() => work());
var ex = Assert.Throws<ArgumentNullException>("name", () => work());
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
new Action(() => work()).Should().Throw<InvalidOperationException>();
var ex = new Action(() => work()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
```

Async exception assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
await Xunit.Assert.ThrowsAsync<InvalidOperationException>(
    () => Task.FromException(new InvalidOperationException()));
await Xunit.Assert.ThrowsAnyAsync<Exception>(
    () => Task.FromException(new ArgumentException()));
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
await new Func<Task>(() => Task.FromException(new InvalidOperationException()))
    .Should()
    .ThrowExactlyAsync<InvalidOperationException>();
await new Func<Task>(() => Task.FromException(new ArgumentException()))
    .Should()
    .ThrowAsync<Exception>();
```

## What To Keep Manual For Now

Keep these manual during an xUnit migration:

- structural comparisons that should become `BeEquivalentTo(...)`
- string-equality special cases such as ignore-case equality options
- precision and tolerance assertions
- comparer edge cases that do not map directly to a current local-comparer assertion
- inspector overloads and message-bearing overloads
- consumed exception cases outside the supported `.Thrown` flows
- span or memory string overloads

Manual does not mean unsupported forever. It means the analyzer should not guess.

## Practical Staged Rollout

A realistic rollout is:

1. Enable `Axiom.Assertions` in one test project.
2. Apply xUnit analyzer fixes for simple scalar and string assertions.
3. Review dictionary-key, `Single(...)`, and exception rewrites carefully because they can carry returned values.
4. Move structural tests to `BeEquivalentTo(...)` only when the test intent is object-graph comparison.
5. Repeat by folder or feature area.
6. Keep a short list of skipped patterns so the team knows which tests need manual treatment.

This keeps the migration small enough to review and easy to pause.

## When Not To Migrate Yet

Do not migrate yet if the current xUnit assertions are clear, the team is not feeling friction, or most of the suite depends on precision, comparer, inspector, or structural semantics that need review.

In that case, start by improving the tests that are unclear. A later Axiom migration will be safer if the assertion intent is already explicit.
