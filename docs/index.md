# Axiom Docs

Axiom is a fluent assertion library for modern .NET tests. It focuses on deterministic failure output, explicit `Batch` aggregation, low pass-path overhead, and a package lineup that stays clear about what each piece is for.

## Package Lineup

### Axiom.Assertions

The default package for most test projects.

- Fluent `Should()` assertions
- Built-in equivalency, async, collection, string, and exception assertions
- Bundled Axiom analyzers/code fixes

```bash
dotnet add package Axiom.Assertions
```

### Axiom.Analyzers

Optional standalone analyzers package.

Install this only if you want the diagnostics without the runtime assertion library.

```bash
dotnet add package Axiom.Analyzers
```

### Axiom.Vectors

Optional package for vector and embedding-style test scenarios.

- dimension checks
- NaN/infinity validation
- approximate equality
- cosine similarity thresholds
- normalization checks

```bash
dotnet add package Axiom.Vectors
```

## Where To Start

- New to Axiom: start with [Getting Started](getting-started.md)
- Need the full API catalog: go to [Assertion Reference](assertion-reference.md)
- Working with structural comparison: go to [Equivalency](equivalency.md)
- Building team/domain assertions: go to [Custom Assertions](custom-assertions.md)
- Using the diagnostics only: go to [Analyzers](analyzers.md)
- Testing embeddings or vector outputs: go to [Vectors](vectors.md)

## What Axiom Is Good At

- Stable, deterministic failure messages
- Explicit grouped failures with `Batch`
- Strong async assertion support
- Configurable object-graph equivalency
- Optional package layers for analyzers and vector assertions
