# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

- _Nothing yet._

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
