# Support

Axiom is maintained on a best-effort basis.

## Supported Packages

This repository currently ships and maintains:

- Axiom.Assertions
- Axiom.Core
- Axiom.Analyzers
- Axiom.Json
- Axiom.Http
- Axiom.Vectors

## Supported Target Frameworks

Runtime packages currently target:

- net8.0
- net9.0
- net10.0

Axiom.Analyzers is a Roslyn analyzer package and ships as a netstandard2.0 analyzer assembly.

## Supported Test Frameworks

Axiom.Assertions auto-detects these frameworks at runtime and uses their native assertion exception types when they are present:

- xUnit
- NUnit
- MSTest

Axiom.Json, Axiom.Http, and Axiom.Vectors are intended to be used alongside Axiom.Assertions in test projects using those frameworks.

## Response Model

Support is best effort.

Bugs, docs issues, migration gaps, and analyzer false positives are welcome through GitHub issues.
Security issues should be reported privately through GitHub private vulnerability reporting.
Response times vary and there is no formal SLA.

## What Best Effort Means

Best effort means:

- fixes are prioritised by correctness, breakage risk, and maintainer capacity,
- backports are not guaranteed,
- older releases may not receive updates,
- release timing depends on maintainer availability.
