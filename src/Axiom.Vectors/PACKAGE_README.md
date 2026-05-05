# Axiom.Vectors

`Axiom.Vectors` is an optional extension package on top of `Axiom.Assertions`.

Most users should start with `Axiom.Assertions` and add `Axiom.Vectors` only when they need vector, embedding, or ranked retrieval assertions.

## Install

```bash
dotnet add package Axiom.Vectors
```

This package is designed to be used with `Axiom.Assertions`.

## Use this package when you want

- dimension, normalization, and numeric-validity checks for vectors
- similarity and distance assertions for embeddings
- ranked retrieval assertions such as top-k, rank, recall, precision, reciprocal rank, mean reciprocal rank, and hit rate

## Example

```csharp
using Axiom.Assertions;
using Axiom.Vectors;

var rankedResults = new[] { "account-overview", "reset-password", "billing-update" };
var relevantItems = new[] { "reset-password", "billing-update" };

rankedResults.Should().ContainInTopK("reset-password", 3);
rankedResults.Should().HaveRecallAt(3, relevantItems, expectedRecall: 1.0);
rankedResults.Should().HavePrecisionAt(3, relevantItems, expectedPrecision: 0.666666666666667, tolerance: 0.001);
```

## Install a different package when

- you only need the main fluent assertion library: install `Axiom.Assertions`
- you only need low-level batching or configuration primitives: install `Axiom.Core`
- you only need analyzers and code fixes: install `Axiom.Analyzers`
- you want JSON assertions: install `Axiom.Json` on top of `Axiom.Assertions`
- you want `HttpResponseMessage` assertions: install `Axiom.Http` on top of `Axiom.Assertions`

Documentation: [spearzy.github.io/Axiom-Assertions](https://spearzy.github.io/Axiom-Assertions/)
Getting started: [Getting Started](https://spearzy.github.io/Axiom-Assertions/getting-started/)
