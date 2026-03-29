# Migrating to Axiom

This page is for teams moving an existing test suite onto Axiom a little at a time.

The current migration tooling focuses on high-confidence xUnit `Assert.*` shapes that can be rewritten safely. The goal is to speed up routine migration work without guessing on overloads that change value flow or assertion semantics.

## What Can Be Migrated Automatically Today

The built-in analyzers and code fixes currently cover these xUnit shapes:

| Source style | Axiom style |
| --- | --- |
| `Assert.Equal(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.NotEqual(expected, actual)` | `actual.Should().NotBe(expected)` |
| `Assert.Null(value)` | `value.Should().BeNull()` |
| `Assert.NotNull(value)` | `value.Should().NotBeNull()` |
| `Assert.True(condition)` | `condition.Should().BeTrue()` |
| `Assert.False(condition)` | `condition.Should().BeFalse()` |
| `Assert.Empty(subject)` | `subject.Should().BeEmpty()` |
| `Assert.NotEmpty(subject)` | `subject.Should().NotBeEmpty()` |
| `Assert.Contains(item, collection)` | `collection.Should().Contain(item)` |
| `Assert.DoesNotContain(item, collection)` | `collection.Should().NotContain(item)` |
| `Assert.Single(subject)` | `subject.Should().ContainSingle()` |
| `Assert.Same(expected, actual)` | `actual.Should().BeSameAs(expected)` |
| `Assert.NotSame(expected, actual)` | `actual.Should().NotBeSameAs(expected)` |
| `Assert.Throws<TException>(() => work())` | `new Action(() => work()).Should().Throw<TException>()` |

These suggestions are shipped as Roslyn diagnostics and code fixes in:

- `Axiom.Assertions` by default
- `Axiom.Analyzers` if you only want the diagnostics

## What Still Needs Manual Migration

The current migration tooling intentionally skips shapes where the rewrite is not obviously semantics-preserving yet.

That currently includes:

- `Assert.Contains` and `Assert.DoesNotContain` string overloads
- dictionary-key containment overloads
- overloads with custom comparers, precision, inspectors, or messages
- `Assert.Single(...)` when you use the returned value
- `Assert.Throws<TException>(...)` when you use the returned exception

That means the migration tooling is conservative by design. If a suggestion appears, it is intended to be one you can trust in a real codebase.

## Example: Moving a Small Test

Before:

```csharp
using Xunit;

public sealed class UserTests
{
    [Fact]
    public void Creates_admin_user()
    {
        var user = CreateUser();

        Assert.NotNull(user);
        Assert.Equal("admin", user.Role);
        Assert.Contains("read", user.Permissions);
        Assert.Single(user.Tenants);
    }
}
```

After:

```csharp
using Axiom.Assertions;
using Axiom.Assertions.Extensions;

public sealed class UserTests
{
    [Fact]
    public void Creates_admin_user()
    {
        var user = CreateUser();

        user.Should().NotBeNull();
        user.Role.Should().Be("admin");
        user.Permissions.Should().Contain("read");
        user.Tenants.Should().ContainSingle();
    }
}
```

## Structural Comparison Maps To `BeEquivalentTo(...)`

When you are migrating tests that compare object graphs rather than scalars, the usual Axiom target is `BeEquivalentTo(...)`.

```csharp
actual.Should().BeEquivalentTo(expected);
```

When types do not line up member-for-member, use Axiom's typed member mapping support rather than flattening the test back to many scalar asserts.

```csharp
actual.Should().BeEquivalentTo(expected, options =>
{
    options.MatchMember<ActualUser, ExpectedUser>(a => a.DisplayName, e => e.Name);
});
```

See [Equivalency](equivalency.md) for the fuller object-graph guidance and diagnostics model.

## Recommended Migration Flow

For most teams, the smoothest approach is:

1. Install `Axiom.Assertions`.
2. Let the built-in migration diagnostics suggest safe rewrites.
3. Apply the obvious code fixes first.
4. Migrate more complex structural comparisons manually to `BeEquivalentTo(...)`.
5. Keep the unsupported edge cases manual until Axiom ships broader migration coverage.

That keeps the migration tooling helpful without asking it to guess.
