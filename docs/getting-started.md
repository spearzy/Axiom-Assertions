# Getting Started

## Which Package Should I Install?

Most test projects should install `Axiom.Assertions`.

```bash
dotnet add package Axiom.Assertions
```

That gives you:

- the fluent `Should()` API
- `Axiom.Core` transitively
- bundled Axiom analyzers/code fixes

Install `Axiom.Analyzers` separately only if you want the diagnostics without the runtime assertion library:

```bash
dotnet add package Axiom.Analyzers
```

Install `Axiom.Vectors` when you want vector and embedding-focused assertions:

```bash
dotnet add package Axiom.Vectors
```

## Basic Setup

For most teams, there is no required setup step. Install `Axiom.Assertions`, write assertions, and Axiom will use native xUnit, NUnit, or MSTest assertion exceptions automatically when it detects those frameworks.

Add shared setup only when you want custom defaults across a test project:

```csharp
using Axiom.Assertions.Configuration;
public static class AxiomSetup
{
    public static void Apply()
    {
        AxiomSettings.Configure(options =>
        {
            options.Core.RegexMatchTimeout = TimeSpan.FromMilliseconds(500);

            options.Equivalency.RequireStrictRuntimeTypes = false;
            options.Equivalency.FailOnMissingMembers = false;
            options.Equivalency.FailOnExtraMembers = false;
        });
    }
}
```

Call `AxiomSetup.Apply()` once from your test framework startup when you want those shared defaults:

- xUnit: fixture constructor
- NUnit: `[SetUpFixture]` + `[OneTimeSetUp]`
- MSTest: `[AssemblyInitialize]`

## First Assertions

```csharp
using Axiom.Assertions;
using Axiom.Core;

user.Name.Should().NotBeNull();
user.Email.Should().Contain("@");
user.Age.Should().BeGreaterThan(18);

using var batch = Assert.Batch("profile");
user.Name.Should().StartWith("A");
user.Roles.Should().Contain("admin");
```

If you want to override the default failure strategy, shared comparers, formatters, or equivalency defaults later, add an `AxiomSetup.cs` like the one above.

## Async And Equivalency Examples

```csharp
Task<string> rollout = Task.FromResult("pricing-api");
var continuation = await rollout.Should().SucceedWithin(TimeSpan.FromMilliseconds(50));
continuation.WhoseResult.Should().Be("pricing-api");

actual.Should().BeEquivalentTo(expected, options =>
{
    options.RequireStrictRuntimeTypes = false;
    options.CollectionOrder = EquivalencyCollectionOrder.Any;
});
```

## Vector Example

```csharp
using Axiom.Vectors;

embedding.Should().HaveDimension(1536);
embedding.Should().HaveCosineSimilarityWith(expected).AtLeast(0.995f);
```

## Next Steps

- Browse the full [Assertion Reference](assertion-reference.md)
- Read the [Equivalency](equivalency.md) guide for object-graph configuration
- Read [Custom Assertions](custom-assertions.md) when you want domain-specific extensions
- Read [Analyzers](analyzers.md) for the shipped diagnostics
- Read [Vectors](vectors.md) for embedding-style assertions
