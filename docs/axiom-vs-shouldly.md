---
title: Axiom vs Shouldly
description: Compare Axiom Assertions and Shouldly side by side for .NET teams evaluating assertion style, deterministic output, batching, migration cost, and when to stay with Shouldly.
---

# Axiom vs Shouldly

This page is for teams comparing Axiom with Shouldly for .NET tests.

Shouldly is older, established, and intentionally prose-like. Axiom is newer, narrower, and more explicit. The decision is mostly about style and maintenance priorities, not a universal winner.

## Who This Comparison Is For

Use this page if you are:

- choosing between a natural-language assertion style and a more explicit fluent style
- deciding whether deterministic failure output or `Batch` aggregation matters enough to evaluate Axiom
- maintaining a Shouldly suite and wondering whether migration would improve clarity
- checking whether Axiom's optional JSON, HTTP, or vector packages fit tests Shouldly does not focus on directly

If your team mainly values Shouldly's conversational style, Shouldly may remain the better fit.

## What Each Library Optimizes For

| Area | Axiom | Shouldly |
| --- | --- | --- |
| Assertion style | Explicit `actual.Should().Be(...)` method shape | Natural-language `actual.ShouldBe(...)` style |
| Failure output | Deterministic, structured output is central | Readable, conversational failure messages |
| Multiple failures | Explicit `Batch` aggregation | Simpler assertion flow without Axiom's batch model |
| Package shape | Main package plus optional JSON, HTTP, and vector/retrieval packages | General assertion package focused on readable test assertions |
| Migration fit | Better fit when explicitness and package boundaries matter | Better fit when the existing prose-like style is valued |

## Same-Intent Examples

| Intent | Axiom | Shouldly |
| --- | --- | --- |
| Scalar equality | `actual.Should().Be(expected);` | `actual.ShouldBe(expected);` |
| String containment | `name.Should().Contain("Ada");` | `name.ShouldContain("Ada");` |
| Collection membership | `roles.Should().Contain("admin");` | `roles.ShouldContain("admin");` |
| Exception assertion | `new Action(() => work()).Should().Throw<InvalidOperationException>();` | `Should.Throw<InvalidOperationException>(() => work());` |
| Group related failures | `using var batch = Assert.Batch("profile");` then normal assertions | usually keep assertions independent or use existing test-framework patterns |

The core style difference is visible quickly: Shouldly reads more like prose, while Axiom keeps the assertion receiver and method name explicit.

## Where Axiom Is A Strong Fit

Axiom is worth evaluating when:

- deterministic failure messages matter more than prose-like assertion syntax
- you want explicit grouped failures with `Batch`
- you want analyzer-backed migration from supported xUnit, NUnit, or MSTest shapes
- optional JSON, HTTP, vector, embedding, or retrieval assertions would reduce custom helper code
- the team prefers a method-driven style that is predictable across assertion categories

## Where Shouldly Is Still The Safer Choice

Shouldly is likely the safer or easier choice when:

- the team already likes and understands Shouldly's natural-language style
- the current suite is clear and stable
- migration would mostly rename assertions without solving a real maintenance problem
- you do not need Axiom's batching, analyzer, optional package, or deterministic-output emphasis

## Migration Cost And Friction

A Shouldly-to-Axiom migration is usually manual. Axiom's migration analyzers currently focus on supported xUnit, NUnit, and MSTest assertion shapes, not broad Shouldly rewrites.

Simple assertions are easy to translate by intent, but helper-heavy suites and tests that rely on Shouldly-specific message style should be reviewed carefully. The practical first step is a small project or folder, not a whole-suite conversion.

## When Not To Switch

Do not switch if the only reason is novelty.

Stay with Shouldly when:

- the prose-like style is the main feature your team values
- your tests already fail clearly enough for the team
- Axiom's optional packages do not solve a current problem
- migration cost would be higher than the expected maintenance benefit

## Where To Go Next

- Use [Migrating to Axiom](migrating-to-axiom.md) for staged migration planning.
- Use the [Assertion Reference](assertion-reference.md) to check current Axiom coverage.
- Use the [Equivalency guide](equivalency.md) if object-graph comparison is part of the decision.
- Read [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md) if you also want the broader fluent-library comparison.
