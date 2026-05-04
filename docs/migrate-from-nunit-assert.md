---
title: Migrate from NUnit Assert to Axiom
description: A practical staged walkthrough for moving NUnit Assert.That tests to Axiom Assertions while keeping richer constraint chains manual.
---

# Migrate from NUnit Assert to Axiom

The NUnit path is useful, but it is narrower and more conservative than the xUnit path. NUnit's constraint model can express a lot of meaning in a small chain, so Axiom only suggests rewrites when the mapping is direct.

## When This Migration Path Is A Good Fit

This path is a good fit when your NUnit tests mostly use simple `Assert.That(...)` constraints and direct async exception assertions.

It works best for tests built from clear `Is.*`, `Does.*`, and `Has.Count.EqualTo(...)` shapes: equality, nullability, booleans, emptiness, string containment, count, reference identity, ordered comparisons, ranges, generic type constraints, and direct async exception checks.

It is not a good fit for a fully automatic migration of rich constraint chains. Those deserve manual review.

## What Axiom Can Rewrite Safely Today

The analyzer supports conservative rewrites for:

- `Is.EqualTo(...)`, `Is.Not.EqualTo(...)`, `Is.Null`, `Is.Not.Null`, `Is.True`, `Is.False`, `Is.Empty`, and `Is.Not.Empty`
- `Does.Contain(...)`, `Does.Not.Contain(...)`, `Does.StartWith(...)`, and `Does.EndWith(...)` on string subjects when the expected value is clear
- `Has.Count.EqualTo(...)`
- `Is.SameAs(...)` and `Is.Not.SameAs(...)`
- `Is.GreaterThan(...)`, `Is.GreaterThanOrEqualTo(...)`, `Is.LessThan(...)`, `Is.LessThanOrEqualTo(...)`, and `Is.InRange(...)`
- `Is.TypeOf<T>()`, `Is.InstanceOf<T>()`, `Is.AssignableTo<T>()`, `Is.Not.InstanceOf<T>()`, and `Is.Not.AssignableTo<T>()`
- `Assert.ThrowsAsync<TException>(...)` and `Assert.CatchAsync<TException>(...)` in async contexts

Use the [Analyzer reference](analyzers.md) when you need exact rule IDs.

## A Recommended First Migration Pass

Start with the boring constraints first. They are the safest and easiest to review.

A good first pass is:

1. Rewrite equality, null, boolean, and emptiness assertions.
2. Rewrite simple string `Does.*` assertions.
3. Rewrite count, reference identity, ordered/range, and generic type constraints.
4. Review async exception rewrites separately.
5. Leave richer chains and custom comparison logic for manual migration.

The aim is not to make every NUnit assertion look like Axiom immediately. The aim is to remove the obvious mechanical cases and keep the risky ones visible.

## Before/After Examples

Common `Is.*`, `Does.*`, and `Has.*` constraints:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.That(actual, Is.EqualTo(expected));
Assert.That(value, Is.Not.Null);
Assert.That(condition, Is.True);
Assert.That(values, Has.Count.EqualTo(3));
Assert.That(actual, Does.Contain("sub"));
Assert.That(actual, Does.StartWith("pre"));
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
value.Should().NotBeNull();
condition.Should().BeTrue();
values.Should().HaveCount(3);
actual.Should().Contain("sub");
actual.Should().StartWith("pre");
```

Ordered, range, and type constraints:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
Assert.That(count, Is.GreaterThan(minimum));
Assert.That(count, Is.InRange(minimum, maximum));
Assert.That(value, Is.TypeOf<object>());
Assert.That(value, Is.InstanceOf<object>());
Assert.That(value, Is.Not.AssignableTo<string>());
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
count.Should().BeGreaterThan(minimum);
count.Should().BeInRange(minimum, maximum);
value.Should().BeOfType<object>();
value.Should().BeAssignableTo<object>();
value.Should().NotBeAssignableTo<string>();
```

Async exception assertions:

Before:

<!-- axiom-context=migration-gallery -->
```csharp
await NUnit.Framework.Assert.ThrowsAsync<InvalidOperationException>(
    async () => await Task.FromException(new InvalidOperationException()));
await NUnit.Framework.Assert.CatchAsync<Exception>(
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

Keep these manual during a NUnit migration:

- richer constraint chains where several expectations are composed together
- comparer and tolerance variants
- message-bearing `Assert.That(...)` overloads
- `Has.*` chains beyond `Has.Count.EqualTo(...)`
- runtime `Type` constraints
- `Is.Not.TypeOf<T>()`
- async exception assertions outside an async context
- `AsyncTestDelegate` variable rewrites
- prefix or suffix constraints where the expected value is not obviously non-null

The narrower analyzer path is deliberate. NUnit constraints can hide important semantics, and a skipped diagnostic is safer than a lossy rewrite.

## Practical Staged Rollout

A realistic NUnit rollout is:

1. Pick one fixture or namespace with simple assertions.
2. Apply analyzer fixes for direct `Is.*`, `Does.*`, and `Has.Count.EqualTo(...)` cases.
3. Review ordered/range/type rewrites as a separate batch.
4. Review async exception rewrites separately from scalar assertions.
5. Mark richer constraint chains for manual migration.
6. Move structural comparisons to `BeEquivalentTo(...)` only when that is the actual test intent.

This lets the team build confidence without pretending NUnit's full constraint model is mechanically replaceable.

## When Not To Migrate Yet

Do not migrate yet if the suite mainly uses expressive NUnit constraint chains, custom comparers, tolerances, or assertion messages that carry important debugging context.

In that case, start by identifying which tests are truly scalar and which are better treated as structural or domain-specific assertions.
