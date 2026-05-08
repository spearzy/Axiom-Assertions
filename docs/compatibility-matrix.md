---
title: Axiom Compatibility Matrix
description: Current compatibility for Axiom packages, including target frameworks, test framework support, and package prerequisites.
---

# Compatibility Matrix

This page states the current shipped compatibility only.

## Package Matrix

| Package | Target frameworks | xUnit | NUnit | MSTest | Prerequisites | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| `Axiom.Assertions` | `net8.0`, `net9.0`, `net10.0` | Yes | Yes | Yes | None | Main fluent assertion package. Pulls in `Axiom.Core` and bundles the Axiom analyzers/code fixes. |
| `Axiom.Json` | `net8.0`, `net9.0`, `net10.0` | Yes | Yes | Yes | `Axiom.Assertions` | Optional JSON assertions over raw JSON `string`, `JsonDocument`, and `JsonElement`. |
| `Axiom.Http` | `net8.0`, `net9.0`, `net10.0` | Yes | Yes | Yes | `Axiom.Assertions`, `Axiom.Json` | Optional `HttpResponseMessage` assertions. Reuses `Axiom.Json` for JSON body assertions. |
| `Axiom.Vectors` | `net8.0`, `net9.0`, `net10.0` | Yes | Yes | Yes | `Axiom.Assertions` | Optional vector, embedding, and ranked retrieval assertions. |
| `Axiom.Core` | `net8.0`, `net9.0`, `net10.0` | N/A | N/A | N/A | None | Low-level batching, formatting, failure, and configuration primitives. No test-framework-specific integration. |
| `Axiom.Analyzers` | `netstandard2.0` analyzer package | Yes | Yes | Yes | None | Roslyn analyzer/code-fix package. Framework columns here refer to current analyzer and migration support, not runtime assertions. |

## Notes

- For `Axiom.Assertions`, `Axiom.Json`, `Axiom.Http`, and `Axiom.Vectors`, xUnit, NUnit, and MSTest support means Axiom can run inside test projects using those frameworks.
- `Axiom.Assertions` auto-detects xUnit, NUnit, and MSTest at runtime and uses the matching framework-native assertion exception type when one of those frameworks is present.
- `Axiom.Core` is framework-agnostic infrastructure. It does not provide framework-specific assertion behavior by itself.
- `Axiom.Analyzers` is not a runtime assertion package. It provides Axiom diagnostics plus current migration/code-fix coverage for framework-native asserts and a narrow set of assertion-library chains.
- `Axiom.Assertions` is the default install path for most users. Install `Axiom.Json`, `Axiom.Http`, or `Axiom.Vectors` only when the test suite needs those assertion areas.

## Starter Projects

The repository includes minimal starter projects for the supported test frameworks:

| Framework | Starter |
| --- | --- |
| xUnit | [Axiom.Assertions.Starter.Xunit](https://github.com/spearzy/Axiom-Assertions/tree/main/starters/Axiom.Assertions.Starter.Xunit) |
| NUnit | [Axiom.Assertions.Starter.NUnit](https://github.com/spearzy/Axiom-Assertions/tree/main/starters/Axiom.Assertions.Starter.NUnit) |
| MSTest | [Axiom.Assertions.Starter.MSTest](https://github.com/spearzy/Axiom-Assertions/tree/main/starters/Axiom.Assertions.Starter.MSTest) |

Each starter uses the normal `Axiom.Assertions` install path and keeps the example intentionally small.

## Analyzer Migration Coverage

The current migration analyzers split into two migration categories.

Framework-assert migrations replace assertions from the test framework itself:

- xUnit: scalar, string, dictionary-key, single-item, synchronous exception, and awaited async exception shapes.
- NUnit: common `Is.*`, `Does.*`, `Has.Count.EqualTo(...)`, ordered/range/type, and async exception shapes.
- MSTest: scalar, reference/type, string, collection containment, ordered/range, and awaited async exception shapes.

Assertion-library migrations replace another assertion library where the mapping is direct:

- FluentAssertions: direct, standalone equality, null, boolean, empty, string, reference identity, exact type, and assignable type chains.
- Shouldly: mostly manual today.

Unsupported or lossy framework-specific overloads and richer assertion-library chains remain manual migrations.

For rollout guidance, start with [Migrating to Axiom](migrating-to-axiom.md), then use the framework walkthroughs or side-by-side library guides linked from there.

## Related Guides

- [Getting Started](getting-started.md)
- [Assertion Reference](assertion-reference.md)
- [JSON](json.md)
- [HTTP and API assertions](http.md)
- [Vectors](vectors.md)
- [Analyzers](analyzers.md)
