# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

- _Nothing yet._

## [0.2.1] - 2026-03-08

### Added

- String equality/null assertions: `Be(expected)`, `NotBe(unexpected)`, and `BeNull()`.
- Typed throw extraction via `.Thrown` for throw assertions.
- Dictionary value extraction via `ContainKey(key).WhoseValue`.
- Collection assertion expansions:
  - `ContainSingle().SingleItem`
  - `ContainSingle(predicate)`
  - `HaveUniqueItemsBy(selector[, comparer])`
  - `SatisfyRespectively(assertions...)`
  - Collection order assertions with key selector/comparer support.
- Direct async subject entrypoints for `Task`, `Task<T>`, `ValueTask`, and `ValueTask<T>` using `Should()`.

### Changed

- Optimised collection assertion pass-path by deferring sequence/entry formatting until failure.
- Optimised ordered equivalency collection comparison to stream both enumerables side-by-side; list materialisation now only occurs for any-order comparison mode.
- Replaced C# 14 `field` usage in `SingleItem` continuation for broader compiler compatibility.

### Fixed

- Consumer smoke test template issues for MSTest/NUnit variants.
- MSTest smoke assertion argument order issue in CI validation.

## [0.2.0] - 2026-03-07

### Added

- `BeOneOf(expectedValues)` and `NotBeOneOf(unexpectedValues)` assertions.
- `Satisfy(predicate)` and `NotSatisfy(predicate)` assertions.
- Exception detail chaining assertions:
  - `WithMessage(expectedMessage[, comparison])`
  - `WithParamName(expectedParamName)`
  - `WithInnerException<TInnerException>()`
- Async exception detail chaining support on `ThrowAsync<TException>()` and `ThrowExactlyAsync<TException>()`.

### Changed

- Added `StringComparison` overloads for string assertions such as `StartWith`, `EndWith`, and `Contain`.
- Added consumer smoke tests for xUnit, NUnit, and MSTest using locally packed Axiom artefacts in CI/release validation.
- Automated GitHub release notes generation from `CHANGELOG.md`.
- Updated README method list and examples to include newly implemented assertions.

## [0.1.0] - 2026-03-05

### Added

- First stable release of `Axiom.Core` and `Axiom.Assertions`.
- `BeApproximately(expected, tolerance)` assertions for `double`, `float`, and `decimal`, including chaining and batch routing support.

### Changed

- Moved `Should()` entrypoint extensions to `Axiom.Assertions` to remove extra namespace import friction.
- Updated install guidance in GitHub and NuGet readmes to stable package commands.
- Updated CI/release dependencies (`actions/checkout`, `actions/setup-dotnet`, `actions/upload-artifact`, `actions/download-artifact`) and test tooling dependencies (`Microsoft.NET.Test.Sdk`, `coverlet.collector`, `coverlet.msbuild`) for current platform support.

## [0.1.0-preview.1] - 2026-03-05

### Added

- First public preview packages: `Axiom.Core` and `Axiom.Assertions`.
- Fluent `Should()` assertions with chaining via `.And`.
- Batch-based aggregation using `Batch` with deterministic combined failures.
- Core assertion coverage for values, strings, collections, dictionaries, exceptions, temporal values, and configurable equivalency.
