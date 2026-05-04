---
title: Migrate from MSTest Assert to Axiom
description: A practical staged walkthrough for moving MSTest Assert, StringAssert, and CollectionAssert tests to Axiom Assertions while keeping structural cases manual.
---

# Migrate from MSTest Assert to Axiom

The MSTest path is focused on direct assertion shapes that map cleanly to Axiom. It is useful for scalar assertions, string checks, simple collection containment, ordered/range checks, type/reference checks, and awaited async exception assertions.

It is not intended to rewrite MSTest's broader collection comparison or message-heavy overloads automatically.

## When This Migration Path Is A Good Fit

This path is a good fit when your MSTest suite uses straightforward `Assert.*`, `StringAssert.*`, or simple `CollectionAssert.*` checks.

It works best when tests are already written as direct facts: expected equals actual, value is null, string contains text, collection contains an item, value is in range, or async work throws a specific exception.

It is a weaker fit when the suite relies on `CollectionAssert.AreEqual(...)`, `CollectionAssert.AreEquivalent(...)`, comparer-heavy assertions, precision, message overloads, or broader assertion families.

## What Axiom Can Rewrite Safely Today

The analyzer supports conservative rewrites for:

- `Assert.AreEqual(...)` and `Assert.AreNotEqual(...)`
- `Assert.IsNull(...)`, `Assert.IsNotNull(...)`, `Assert.IsTrue(...)`, and `Assert.IsFalse(...)`
- `Assert.AreSame(...)` and `Assert.AreNotSame(...)`
- `Assert.IsInstanceOfType(...)` and `Assert.IsNotInstanceOfType(...)`
- `StringAssert.Contains(...)`, `StringAssert.StartsWith(...)`, and `StringAssert.EndsWith(...)` when the expected value is clear
- `CollectionAssert.Contains(...)` and `CollectionAssert.DoesNotContain(...)`
- `Assert.IsGreaterThan(...)`, `Assert.IsGreaterThanOrEqualTo(...)`, `Assert.IsLessThan(...)`, `Assert.IsLessThanOrEqualTo(...)`, and `Assert.IsInRange(...)`
- awaited `Assert.ThrowsExceptionAsync<TException>(...)`, `Assert.ThrowsExactlyAsync<TException>(...)`, and `Assert.ThrowsAsync<TException>(...)`

Use the [Analyzer reference](analyzers.md) when you need exact rule IDs.

## A Recommended First Migration Pass

Start with the direct `Assert.*` calls first, then move outward.

A good first pass is:

1. Rewrite equality, null, boolean, reference, and type checks.
2. Rewrite simple `StringAssert.*` checks.
3. Rewrite simple collection containment checks.
4. Review ordered/range rewrites separately because MSTest uses bound-first argument order.
5. Review awaited async exception rewrites separately.
6. Leave structural collection comparisons and message-bearing overloads manual.

## Before/After Examples

Scalar, type, and reference assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.AreEqual(expected, actual);
Assert.IsNull(value);
Assert.IsInstanceOfType(value, typeof(IDisposable));
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
value.Should().BeNull();
value.Should().BeAssignableTo<IDisposable>();
```

Reference identity follows the same direct pattern: `Assert.AreSame(expected, actual)` becomes `actual.Should().BeSameAs(expected)`, and `Assert.AreNotSame(expected, actual)` becomes `actual.Should().NotBeSameAs(expected)`.

String and collection containment assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
StringAssert.Contains(actual, "sub");
CollectionAssert.Contains(values, "expected");
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Contain("sub");
values.Should().Contain("expected");
```

Ordered and range assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsGreaterThan(minimum, count);
Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInRange(minimum, maximum, count);
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
count.Should().BeGreaterThan(minimum);
count.Should().BeInRange(minimum, maximum);
```

Async exception assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<InvalidOperationException>(
    async () => await Task.FromException(new InvalidOperationException()));
await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsAsync<Exception>(
    async () => await Task.FromException(new ArgumentException()));
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

Keep these manual during an MSTest migration:

- `CollectionAssert.AreEqual(...)`, `CollectionAssert.AreEquivalent(...)`, and structural collection comparison
- precision, comparer, and message-bearing overloads
- broader MSTest assertion families that are not direct scalar/string/containment checks
- async exception assertions that are not awaited
- xUnit-style async `paramName` and `ThrowsAnyAsync<TException>` shapes, which MSTest does not expose
- ordered assertions where the values are not clearly comparable
- structural object comparisons that should become `BeEquivalentTo(...)`

Manual review is especially important for collection comparison. Containment is not the same thing as sequence equality or equivalency.

## Practical Staged Rollout

A realistic MSTest rollout is:

1. Pick one test project or namespace.
2. Apply analyzer fixes for scalar `Assert.*` calls.
3. Review `StringAssert.*` and simple `CollectionAssert.*` rewrites.
4. Review ordered/range rewrites with attention to bound-first argument order.
5. Review awaited async exception rewrites separately.
6. Keep structural collection and object-graph assertions manual.

This avoids a noisy rewrite where the easy cases hide the tests that need actual design decisions.

## When Not To Migrate Yet

Do not migrate yet if the suite mainly depends on MSTest collection equivalency, custom comparer behaviour, precision rules, or assertion messages that carry important debugging context.

In that case, first separate direct scalar checks from structural tests. The scalar checks can move later; structural tests should be designed deliberately.
