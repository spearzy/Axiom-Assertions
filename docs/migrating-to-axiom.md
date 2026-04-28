---
title: Migrating to Axiom Assertions for .NET
description: Move a .NET test suite onto Axiom Assertions in stages, with focused guidance for analyzer-driven rewrites, structural assertions, and manual migration decisions.
---

# Migrating to Axiom

This page is for teams moving an existing test suite onto Axiom Assertions in stages.

If your starting point is mostly xUnit `Assert.*`, the quickest companion guide is [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md).

The built-in migration analyzers help with a growing set of safe, mechanical rewrites. Structural assertions still need engineering judgment. That is normal. This guide tries to make that judgment simpler.

## What Can Be Migrated Automatically Today

The current analyzer and code-fix support focuses on high-confidence xUnit `Assert.*` shapes plus conservative NUnit and MSTest migration suggestions. The detailed rule inventory lives in [Analyzers](analyzers.md); this page keeps the migration shape readable.

### xUnit

| Area | Examples of supported rewrites |
| --- | --- |
| Scalar assertions | equality and inequality, null checks, booleans, emptiness, collection containment, reference identity, type checks |
| String assertions | substring, prefix, and suffix checks, including safe `StringComparison` overloads |
| Returned values | dictionary key lookup and `Single(...)` rewrites append `.WhoseValue` or `.SingleItem` when the old xUnit result was consumed |
| Exceptions | synchronous `Throws<TException>(...)`, awaited `ThrowsAsync<TException>(...)`, awaited `ThrowsAsync<TException>(paramName, ...)` with a non-null constant name, and awaited `ThrowsAnyAsync<TException>(...)` |

### NUnit

| Area | Examples of supported rewrites |
| --- | --- |
| `Is.*` constraints | equality, inequality, null checks, booleans, emptiness, reference identity, ordered comparisons, ranges, and generic type constraints |
| `Does.*` constraints | direct string contains, not-contains, starts-with, and ends-with checks when the expected value is clear |
| `Has.*` constraints | `Has.Count.EqualTo(...)` |
| Async exceptions | `Assert.ThrowsAsync<TException>(...)` and `Assert.CatchAsync<TException>(...)` in async contexts |

### MSTest

| Area | Examples of supported rewrites |
| --- | --- |
| `Assert.*` | equality, inequality, null checks, booleans, reference identity, type checks, ordered comparisons, and ranges |
| `StringAssert.*` | contains, starts-with, and ends-with checks when the expected value is clear |
| `CollectionAssert.*` | contains and does-not-contain checks |
| Async exceptions | awaited `ThrowsExceptionAsync<TException>(...)`, `ThrowsExactlyAsync<TException>(...)`, and `ThrowsAsync<TException>(...)` |

For dictionary-key migration, the receiver has to fit Axiom's `ContainKey` / `NotContainKey` surface. The current support covers the common mutable, read-only, concurrent, and immutable dictionary shapes.

These suggestions ship in:

- `Axiom.Assertions` by default
- `Axiom.Analyzers` if you only want the diagnostics

The NUnit and MSTest support is intentionally narrower than the xUnit support today. These waves only handle shapes that map directly to Axiom without carrying extra framework-specific semantics across.

## What Still Needs Manual Migration

The migration tooling is conservative on purpose.

It skips cases where the rewrite is not obviously semantics-preserving yet.

For xUnit, keep these manual:

- precision, inspectors, and message-bearing overloads
- string-equality overloads such as `Assert.Equal(..., ignoreCase: ...)`
- comparer-bearing equality overloads that would need a specialized string assertion rewrite
- `Memory<char>` / `Span<char>` prefix and suffix overloads
- prefix and suffix checks where the expected value is not an obvious non-null constant string
- nongeneric `Assert.Single(subject)` when the returned value is consumed
- exception assertions where the returned exception is used outside the safe supported shapes

For NUnit, keep these manual:

- richer constraint chains and comparer or tolerance variants
- message-bearing `Assert.That(...)` overloads
- `Has.*` chains beyond `Has.Count.EqualTo(int)`
- runtime `Type` constraints and `Is.Not.TypeOf<T>()`
- async exception assertions outside an async context
- `AsyncTestDelegate` variable rewrites
- xUnit-style async `paramName` / `ThrowsAnyAsync` shapes, which NUnit does not expose

For MSTest, keep these manual:

- async exception assertions that are not awaited
- message-bearing async exception overloads
- xUnit-style async `paramName` / `ThrowsAnyAsync` shapes, which MSTest does not expose
- message-bearing ordered-value overloads
- non-comparable ordering, comparer, precision, and richer assertion-family overloads
- `CollectionAssert.AreEqual(...)`, `CollectionAssert.AreEquivalent(...)`, and other structural collection comparisons

Most structural-comparison assertions still need a deliberate manual migration.

If a code fix appears, it is meant to be a safe one. If it does not appear, treat the migration as a normal test rewrite rather than a missed trick.

## Choosing Between Scalar And Structural Migration

A simple rule helps here.

If the original test is checking one fact at a time, migrate it to scalar Axiom assertions.

```csharp
actual.Name.Should().Be("Bob");
actual.Email.Should().Contain("@");
```

If the original test is really comparing one object graph with another, move it to `BeEquivalentTo(...)` instead of rewriting it into many scalar assertions.

```csharp
actual.Should().BeEquivalentTo(expected);
```

That keeps the test aligned with the shape of the behavior you actually care about.

## Migrating Structural Assertions

### Simple Object-Graph Comparison

If the original assertion is conceptually "these two objects should match member-for-member", start here:

```csharp
actual.Should().BeEquivalentTo(expected);
```

Do this before adding options. The defaults are strict and easy to reason about.

### Cross-Type Comparison

If the types differ but they represent the same concept and the member names already line up, disable strict runtime type matching:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.RequireStrictRuntimeTypes = false);
```

That is the normal Axiom target for DTO-to-domain or snapshot-to-response comparisons when the shapes are mostly the same.

### Renamed Members

If the shapes are the same but some member names changed, add only the mappings you need.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address.Postcode, x => x.Location.ZipCode);
});
```

Use typed mappings when you can. They are easier to read and safer to refactor.

If the mapping has to come from strings, use `MatchMemberName(...)` instead. As with typed mappings, repeated identical mappings are fine, but conflicting remaps fail fast.

### Custom Comparers, Ordering, And Tolerances

If the original test used domain-specific comparison rules, carry those rules over directly instead of flattening the test.

Examples:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);

actual.Should().BeEquivalentTo(expected, options =>
    options.DateTimeTolerance = TimeSpan.FromSeconds(1));

actual.Should().BeEquivalentTo(expected, options =>
    options.UseComparer<OrderSnapshot>(x => x.Code, StringComparer.OrdinalIgnoreCase));
```

This is usually the right migration shape when the structural comparison is still the core idea and only a few comparison rules need to change.

## When Manual Migration Is Better

Some structural assertions should stay manual even if they look close to a possible automation target.

Keep the migration manual when:

- the original assertion mixes many domain-specific comparison rules
- the original test intentionally compares only a small stable subset of the graph
- the original assertion relies on custom comparers or tolerances that need review
- the original test is easier to understand as a few scalar assertions than as a broad graph comparison
- the structural comparison has unclear semantics and needs a test-design decision, not a syntax rewrite

That last point matters. A manual migration is often the right move when the old test itself was not very explicit.

## Example: Rewriting A Structural Test

Before:

```csharp
Assert.Equal(expected.Name, actual.Name);
Assert.Equal(expected.Email, actual.Email);
Assert.Equal(expected.Address.Postcode, actual.Address.Postcode);
Assert.Equal(expected.Address.CountryCode, actual.Address.CountryCode);
```

After:

```csharp
actual.Should().BeEquivalentTo(expected);
```

If one member was renamed:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
});
```

If order does not matter for one collection:

```csharp
actual.Should().BeEquivalentTo(expected, options =>
    options.CollectionOrder = EquivalencyCollectionOrder.Any);
```

## Recommended Migration Flow

For most teams, the smoothest sequence is:

1. Install `Axiom.Assertions`.
2. Apply the obvious analyzer-driven rewrites first.
3. Move structural comparisons to `BeEquivalentTo(...)` manually.
4. Add only the minimum equivalency options needed for that test.
5. Keep unclear or overloaded cases manual until the intended semantics are explicit.

That keeps the migration trustworthy.

## Where To Go Next

- Read [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) for the xUnit-focused path
- Read [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md) or [Axiom vs Shouldly](axiom-vs-shouldly.md) if you are still choosing an assertion library
- For the structural-comparison rules, defaults, and failure-output examples, read [Equivalency](equivalency.md)
