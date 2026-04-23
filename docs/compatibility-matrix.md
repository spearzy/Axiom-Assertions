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
- `Axiom.Analyzers` is not a runtime assertion package. It provides Axiom diagnostics and current migration/code-fix coverage for xUnit, NUnit, and MSTest assertion shapes.
- `Axiom.Assertions` is the default install path for most users. Install `Axiom.Json`, `Axiom.Http`, or `Axiom.Vectors` only when the test suite needs those assertion areas.

## Related Guides

- [Getting Started](getting-started.md)
- [Assertion Reference](assertion-reference.md)
- [JSON](json.md)
- [HTTP and API assertions](http.md)
- [Vectors](vectors.md)
- [Analyzers](analyzers.md)
