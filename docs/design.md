# Axiom Design Notes

This document captures the design direction for Axiom.

## Core Goals

- Pay-for-play assertions with low pass-path overhead.
- Deterministic failure output suitable for strict tests.
- Batch-based failure aggregation (`Batch`) for grouped assertions.
- First-class extensibility for custom assertions and formatting/comparison behaviour.

## Architecture (Planned)

- `Axiom.Core`: batch, failure model, formatting pipeline, entrypoints.
- `Axiom.Assertions`: built-in assertion sets and chaining APIs.
- `Axiom.Analyzers`: Roslyn analysers/code fixes (initial scaffold).
- `Axiom.Vectors`: vector-type adapters (initial scaffold).
- `Axiom.Benchmarks`: BenchmarkDotNet performance suite.

## Open Questions

- Final shape of formatter/comparer registration.
- API boundaries between `Axiom.Core` and `Axiom.Assertions`.
- Error message layout and stable sorting/formatting rules.
