---
name: Migration gap
about: Report a missing migration path or unsupported assertion rewrite
---

# Summary

Describe the migration gap in 2-5 sentences.

## Source Framework or Library

- xUnit
- NUnit
- MSTest
- FluentAssertions
- Shouldly
- Other

## Current Assertion Shape

```csharp
// source assertion
```

## Expected Axiom Shape

```csharp
// expected target shape if known
```

## Current Behaviour

- no diagnostic is offered
- diagnostic is offered but the rewrite is incomplete
- diagnostic is offered but the rewrite is wrong
- unsure

## Why This Should Be Safe

Explain why this looks like a high-confidence migration candidate.
