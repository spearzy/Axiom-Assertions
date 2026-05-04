---
title: Migrating to Axiom Assertions for .NET
description: Plan a staged migration from xUnit, NUnit, or MSTest assertions to Axiom Assertions, with links to framework-specific walkthroughs and manual migration guidance.
---

# Migrating to Axiom

This page is the migration hub. It is intentionally short.

Use it to choose the right walkthrough, decide what should be automated, and identify the tests that need manual review instead of a mechanical rewrite.

## Choose The Walkthrough

| Starting point | Use this guide | Current fit |
| --- | --- | --- |
| xUnit `Assert.*` | [Migrate from xUnit Assert to Axiom](migrate-from-xunit-assert.md) | Strongest analyzer-backed path today. |
| NUnit `Assert.That(...)` and async exception assertions | [Migrate from NUnit Assert to Axiom](migrate-from-nunit-assert.md) | Useful but deliberately narrower than xUnit. |
| MSTest `Assert.*`, `StringAssert.*`, and `CollectionAssert.*` | [Migrate from MSTest Assert to Axiom](migrate-from-mstest-assert.md) | Useful for direct scalar, string, collection containment, ordering, and async exception shapes. |

The complete rule inventory stays in the [Analyzer reference](analyzers.md). The walkthroughs focus on how to make progress in a real codebase without turning every supported shape into another table.

## Recommended Migration Shape

For most teams, the practical sequence is:

1. Install `Axiom.Assertions` in one test project.
2. Run the analyzer-backed fixes for the framework you are migrating from.
3. Review the diff as a behaviour-preserving syntax change, not as a redesign.
4. Keep structural, precision, comparer, tolerance, and message-heavy tests manual.
5. Move object-graph comparisons to [Equivalency](equivalency.md) only when that is the real assertion intent.
6. Repeat by project, folder, or test category instead of trying to migrate everything at once.

That keeps the migration honest. If a code fix appears, it should be safe. If it does not appear, treat that test as a normal engineering rewrite.

## What The Analyzers Are For

The migration analyzers are designed for high-confidence rewrites. They are most useful for tests that check one clear fact at a time:

- scalar equality and null checks
- simple string checks
- collection containment or count checks
- reference and type checks
- direct ordered/range checks
- supported exception assertion shapes

They are not meant to guess through richer framework-specific semantics.

## Where Migration Should Stay Manual

Keep the migration manual when the old assertion depends on:

- custom comparer, culture, precision, tolerance, or message semantics
- structural collection comparison
- broad object-graph comparison
- richer NUnit constraint chains
- MSTest collection equivalency or ordered collection comparison
- xUnit string equality options that do not map directly to current Axiom string assertions
- exception assertions where the returned exception is used outside the supported consumed-result patterns

For structural assertions, start with the [Equivalency guide](equivalency.md). It is usually better to migrate the intent once than to rewrite many scalar assertions mechanically.

## Where To Go Next

- Use the [xUnit walkthrough](migrate-from-xunit-assert.md) for the strongest automated migration path.
- Use the [NUnit walkthrough](migrate-from-nunit-assert.md) for conservative `Assert.That(...)` and async exception rewrites.
- Use the [MSTest walkthrough](migrate-from-mstest-assert.md) for direct `Assert.*`, `StringAssert.*`, and `CollectionAssert.*` rewrites.
- Use the [Analyzer reference](analyzers.md) when you need exact rule IDs and supported shapes.
- Use [Equivalency](equivalency.md) when migration turns into structural comparison design.
- Use [Axiom vs FluentAssertions](axiom-vs-fluentassertions.md) or [Axiom vs Shouldly](axiom-vs-shouldly.md) when the real question is whether to switch assertion libraries at all.
