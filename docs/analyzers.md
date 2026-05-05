# Analyzers

Installing `Axiom.Assertions` gives you the Axiom Roslyn analyzers/code fixes automatically. That is the default install path for most users.

`Axiom.Analyzers` still exists as an optional standalone package if you only want the diagnostics without the runtime assertion library.

```bash
dotnet add package Axiom.Assertions
```

Install the standalone package only if you want the diagnostics on their own:

```bash
dotnet add package Axiom.Analyzers
```

If you are developing against a direct `ProjectReference` to `Axiom.Assertions.csproj`, add the analyzer projects explicitly as analyzer references in the consuming project:

```xml
<ItemGroup>
  <ProjectReference Include=".../Axiom.Assertions.csproj" />
  <ProjectReference Include=".../Axiom.Analyzers.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false"
                    SetTargetFramework="TargetFramework=netstandard2.0" />
  <ProjectReference Include=".../Axiom.Analyzers.CodeFixes.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false"
                    SetTargetFramework="TargetFramework=netstandard2.0" />
</ItemGroup>
```

That keeps the normal NuGet package flow unchanged while giving local project-reference consumers the same analyzer/code-fix assets explicitly.

The current rules focus on a few high-value areas:

- ignored async Axiom assertion results
- `Batch` instances created without `using`
- high-confidence xUnit `Assert.*` migration suggestions
- conservative NUnit `Assert.That(...)` migration suggestions
- conservative MSTest `Assert.*` migration suggestions

## Migration Coverage At A Glance

The migration analyzers are intentionally conservative. They only suggest rewrites when the target Axiom assertion preserves the old value flow and assertion semantics.

| Framework | Current migration coverage |
| --- | --- |
| xUnit | scalar assertions, strings including safe `StringComparison` overloads, dictionary key lookup, `Single(...)`, type/reference checks, simple range checks, synchronous exceptions, and awaited async exception assertions |
| NUnit | common `Is.*`, `Does.*`, and `Has.Count.EqualTo(...)` constraints, plus ordered/range/type constraints and async exception assertions in async contexts |
| MSTest | scalar assertions, reference/type checks, direct string/collection containment, collection uniqueness, ordered/range checks, and awaited async exception assertions |

For staged migration guidance, start with [Migrating to Axiom](migrating-to-axiom.md), then use the [xUnit](migrate-from-xunit-assert.md), [NUnit](migrate-from-nunit-assert.md), or [MSTest](migrate-from-mstest-assert.md) walkthrough.

## Async Assertions Must Be Awaited

Rule: `AXM0001`

This rule flags ignored async Axiom assertion calls from:

- `AsyncActionAssertions`
- `AsyncFunctionAssertions<T>`
- `TaskAssertions`
- `TaskAssertions<T>`

Examples include:

- `ThrowAsync(...)`
- `NotThrowAsync()`
- `Succeed()`
- `SucceedWithin(...)`
- `BeFaultedWith(...)`

Async Axiom assertions are lazy until their returned `ValueTask` is awaited or returned. If the result is ignored, the test can appear to pass even though the assertion never executed.

## Before / After

Before:

```csharp
loader.Should().Succeed();
```

After:

```csharp
await loader.Should().Succeed();
```

The analyzer also offers a code fix in async contexts where prepending `await` is safe.

## Batch Must Be Disposed

Rule: `AXM0002`

This rule flags `Batch` instances created without `using`.

Before:

```csharp
var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

After:

```csharp
using var batch = Assert.Batch("user");
user.Name.Should().NotBeNull();
```

`Batch` flushes aggregated failures when it is disposed. If it is created without `using`, failures may never be emitted at the end of the scope.

The analyzer offers a code fix for the common local declaration case by converting `var batch = ...;` to `using var batch = ...;`.

## xUnit Assert Migration Suggestions

Rules:

- `AXM1001` for `Assert.Equal(expected, actual)` and safe non-string comparer overloads
- `AXM1002` for `Assert.NotEqual(expected, actual)` and safe non-string comparer overloads
- `AXM1003` for `Assert.Null(value)`
- `AXM1004` for `Assert.NotNull(value)`
- `AXM1005` for `Assert.True(condition)`
- `AXM1006` for `Assert.False(condition)`
- `AXM1007` for `Assert.Empty(subject)`
- `AXM1008` for `Assert.NotEmpty(subject)`
- `AXM1009` for `Assert.Contains(item, collection)`
- `AXM1010` for `Assert.DoesNotContain(item, collection)`
- `AXM1011` for `Assert.Single(subject)`, appending `.SingleItem` when the single item is used
- `AXM1012` for `Assert.Same(expected, actual)`
- `AXM1013` for `Assert.NotSame(expected, actual)`
- `AXM1014` for `Assert.Throws<TException>(...)`, including non-null constant `paramName` + `Action` overloads and appending `.Thrown` when the exception is used
- `AXM1015` for `Assert.IsType<T>(actual)`
- `AXM1016` for `Assert.IsAssignableFrom<T>(actual)`
- `AXM1017` for `Assert.Contains(expectedSubstring, actualString)` and `StringComparison` overloads
- `AXM1018` for `Assert.DoesNotContain(expectedSubstring, actualString)` and `StringComparison` overloads
- `AXM1019` for `Assert.Single(collection, predicate)`, appending `.SingleItem` when the matched item is used
- `AXM1020` for `Assert.Contains(key, dictionary)`, appending `.WhoseValue` when the associated value is used
- `AXM1021` for `Assert.DoesNotContain(key, dictionary)`
- `AXM1022` for `Assert.StartsWith(expectedPrefix, actualString)` and `StringComparison` overloads when the prefix is an obvious non-null constant string
- `AXM1023` for `Assert.EndsWith(expectedSuffix, actualString)` and `StringComparison` overloads when the suffix is an obvious non-null constant string
- `AXM1054` for awaited `Assert.ThrowsAsync<TException>(...)`, including non-null constant `paramName` + `Func<Task>` overloads and appending `.Thrown` when the exception is used
- `AXM1055` for awaited `Assert.ThrowsAnyAsync<TException>(...)`, appending `.Thrown` when the exception is used
- `AXM1076` for `Assert.IsNotAssignableFrom<T>(actual)`
- `AXM1077` for `Assert.InRange(actual, low, high)`

The migration support is intentionally narrow and high-confidence. It only offers diagnostics and code fixes for xUnit assertion shapes that map cleanly to Axiom's fluent API without changing value flow or subtle overload semantics.

Before:

<!-- axiom-context=migration-gallery axiom-framework=xunit -->
```csharp
Assert.Equal(expected, actual);
Assert.Equal(42, 42, EqualityComparer<int>.Default);
Assert.True(condition);
Assert.Empty(values);
Assert.Contains(expected, values);
Assert.Contains("sub", actual);
Assert.Contains("sub", actual, StringComparison.OrdinalIgnoreCase);
Assert.StartsWith("pre", actual, StringComparison.OrdinalIgnoreCase);
Assert.EndsWith("suf", actual, StringComparison.OrdinalIgnoreCase);
Assert.Contains(key, lookup);
var found = Assert.Contains(key, lookup);
var item = Assert.Single(values);
var match = Assert.Single(values, value => value > 0);
Assert.IsNotAssignableFrom<IDisposable>(actualObject);
Assert.InRange(count, minimum, maximum);
Assert.Throws<InvalidOperationException>(() => work());
var ex = Assert.Throws<ArgumentNullException>("name", () => work());
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
42.Should().Be(42, EqualityComparer<int>.Default);
condition.Should().BeTrue();
values.Should().BeEmpty();
values.Should().Contain(expected);
actual.Should().Contain("sub");
actual.Should().Contain("sub", StringComparison.OrdinalIgnoreCase);
actual.Should().StartWith("pre", StringComparison.OrdinalIgnoreCase);
actual.Should().EndWith("suf", StringComparison.OrdinalIgnoreCase);
lookup.Should().ContainKey(key);
var found = lookup.Should().ContainKey(key).WhoseValue;
var item = values.Should().ContainSingle().SingleItem;
var match = values.Should().ContainSingle(value => value > 0).SingleItem;
actualObject.Should().NotBeAssignableTo<IDisposable>();
count.Should().BeInRange(minimum, maximum);
new Action(() => work()).Should().Throw<InvalidOperationException>();
var ex = new Action(() => work()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
```

The migration suggestions use semantic matching, so they only target xUnit's real `Assert` API. They do not flag custom helper classes named `Assert`.

The dictionary-key rules follow Axiom's `ContainKey` and `NotContainKey` receiver shape. They support `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`, `Dictionary<TKey, TValue>`, `ReadOnlyDictionary<TKey, TValue>`, `ConcurrentDictionary<TKey, TValue>`, and `ImmutableDictionary<TKey, TValue>`.

They also intentionally skip shapes that are not obviously semantics-preserving yet, including:

- precision, inspectors, and user-message overloads
- string-equality overloads that rely on xUnit's dedicated string semantics such as `Assert.Equal(..., ignoreCase: ...)`
- comparer-bearing equality overloads that would need to land on Axiom's specialized string assertion surface rather than a direct local-comparer value assertion
- `Assert.StartsWith(...)` and `Assert.EndsWith(...)` overloads that use `Memory<char>` or `Span<char>`
- `Assert.StartsWith(...)` and `Assert.EndsWith(...)` when the expected prefix or suffix is not an obvious non-null constant string
- nongeneric `Assert.Single(subject)` calls when the returned item is used
- `Assert.Throws<TException>(...)` consumed-result shapes outside the `string? paramName, Action testCode` overload
- `Assert.Throws<TException>(paramName, ...)` when `paramName` is not an obvious non-null constant string
- non-awaited `Assert.ThrowsAsync<TException>(...)` and `Assert.ThrowsAnyAsync<TException>(...)` shapes
- `Assert.ThrowsAsync<TException>(paramName, ...)` when `paramName` is not an obvious non-null constant string
- `Assert.IsNotType<T>(...)`, because Axiom does not currently expose an exact type-exclusion assertion
- `Assert.NotInRange(...)` and comparer-bearing range overloads

## NUnit Assert Migration Suggestions

Rules:

- `AXM1024` for `Assert.That(actual, Is.EqualTo(expected))`
- `AXM1025` for `Assert.That(actual, Is.Not.EqualTo(expected))`
- `AXM1026` for `Assert.That(value, Is.Null)`
- `AXM1027` for `Assert.That(value, Is.Not.Null)`
- `AXM1028` for `Assert.That(condition, Is.True)`
- `AXM1029` for `Assert.That(condition, Is.False)`
- `AXM1030` for `Assert.That(collection, Is.Empty)`
- `AXM1031` for `Assert.That(collection, Is.Not.Empty)`
- `AXM1040` for `Assert.That(actual, Does.Contain(expectedSubstring))` on string subjects
- `AXM1041` for `Assert.That(actual, Does.Not.Contain(expectedSubstring))` on string subjects
- `AXM1042` for `Assert.That(actual, Does.StartWith(expectedPrefix))` when the prefix is an obvious non-null constant string
- `AXM1043` for `Assert.That(actual, Does.EndWith(expectedSuffix))` when the suffix is an obvious non-null constant string
- `AXM1044` for `Assert.That(collection, Has.Count.EqualTo(expectedCount))`
- `AXM1045` for `Assert.That(actual, Is.SameAs(expected))`
- `AXM1046` for `Assert.That(actual, Is.Not.SameAs(expected))`
- `AXM1056` for `Assert.That(actual, Is.GreaterThan(expected))`
- `AXM1057` for `Assert.That(actual, Is.GreaterThanOrEqualTo(expected))`
- `AXM1058` for `Assert.That(actual, Is.LessThan(expected))`
- `AXM1059` for `Assert.That(actual, Is.LessThanOrEqualTo(expected))`
- `AXM1060` for `Assert.That(actual, Is.InRange(minimum, maximum))`
- `AXM1061` for `Assert.That(actual, Is.TypeOf<TExpected>())`
- `AXM1062` for `Assert.That(actual, Is.InstanceOf<TExpected>())`
- `AXM1063` for `Assert.That(actual, Is.AssignableTo<TExpected>())`
- `AXM1064` for `Assert.That(actual, Is.Not.InstanceOf<TExpected>())`
- `AXM1065` for `Assert.That(actual, Is.Not.AssignableTo<TExpected>())`
- `AXM1066` for `Assert.ThrowsAsync<TException>(...)` in async contexts, appending `.Thrown` when the returned exception is used
- `AXM1067` for `Assert.CatchAsync<TException>(...)` in async contexts, appending `.Thrown` when the returned exception is used
- `AXM1078` for `Assert.That(collection, Has.Member(expected))`
- `AXM1079` for `Assert.That(collection, Has.No.Member(unexpected))`
- `AXM1080` for `Assert.That(collection, Is.Unique)`

The NUnit migration support is still intentionally narrow. It covers `Does.*`, direct `Has.*` collection constraints, ordered value, range, reference identity, generic type constraints, and async exception assertions that map directly onto the current Axiom surface without guessing through richer constraint chains.

Before:

<!-- axiom-context=migration-gallery axiom-framework=nunit -->
```csharp
Assert.That(actual, Is.EqualTo(expected));
Assert.That(value, Is.Not.Null);
Assert.That(condition, Is.True);
Assert.That(values, Is.Empty);
Assert.That(actual, Does.Contain("sub"));
Assert.That(actual, Does.Not.Contain("archived"));
Assert.That(actual, Does.StartWith("pre"));
Assert.That(actual, Does.EndWith("suf"));
Assert.That(values, Has.Count.EqualTo(2));
Assert.That(values, Has.Member("active"));
Assert.That(values, Has.No.Member("blocked"));
Assert.That(values, Is.Unique);
Assert.That(value, Is.SameAs(value));
Assert.That(2, Is.GreaterThan(1));
Assert.That(2, Is.InRange(1, 3));
Assert.That(value, Is.TypeOf<object>());
Assert.That(value, Is.InstanceOf<object>());
Assert.That(value, Is.Not.AssignableTo<string>());
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
value.Should().NotBeNull();
condition.Should().BeTrue();
values.Should().BeEmpty();
actual.Should().Contain("sub");
actual.Should().NotContain("archived");
actual.Should().StartWith("pre");
actual.Should().EndWith("suf");
values.Should().HaveCount(2);
values.Should().Contain("active");
values.Should().NotContain("blocked");
values.Should().HaveUniqueItems();
value.Should().BeSameAs(value);
2.Should().BeGreaterThan(1);
2.Should().BeInRange(1, 3);
value.Should().BeOfType<object>();
value.Should().BeAssignableTo<object>();
value.Should().NotBeAssignableTo<string>();
```

These suggestions use semantic matching against NUnit's real APIs. They intentionally skip tolerance/comparer variations, message-bearing overloads, richer `Does.*` chains, richer `Has.*` chains beyond direct count/member checks, runtime `Type` constraints, `Is.Not.TypeOf<T>()`, `Is.Not.Unique`, async exception assertions outside an async context, `AsyncTestDelegate` variable rewrites, and prefix/suffix constraints where the expected value is not an obvious non-null constant string.

NUnit does not expose xUnit's `Assert.ThrowsAnyAsync<TException>(...)` or `Assert.ThrowsAsync<TException>(paramName, ...)` shapes. The NUnit derived-exception async assertion shape is `Assert.CatchAsync<TException>(...)`, which maps to Axiom's `ThrowAsync<TException>()`.

## MSTest Assert Migration Suggestions

Rules:

- `AXM1032` for `Assert.AreEqual(expected, actual)`
- `AXM1033` for `Assert.AreNotEqual(expected, actual)`
- `AXM1034` for `Assert.IsNull(value)`
- `AXM1035` for `Assert.IsNotNull(value)`
- `AXM1036` for `Assert.IsTrue(condition)`
- `AXM1037` for `Assert.IsFalse(condition)`
- `AXM1038` for `Assert.AreSame(expected, actual)`
- `AXM1039` for `Assert.AreNotSame(expected, actual)`
- `AXM1047` for `Assert.IsInstanceOfType(value, typeof(T))`
- `AXM1048` for `Assert.IsNotInstanceOfType(value, typeof(T))`
- `AXM1049` for `StringAssert.Contains(actual, expectedSubstring)`
- `AXM1050` for `StringAssert.StartsWith(actual, expectedPrefix)` when `expectedPrefix` is an obvious non-null constant string
- `AXM1051` for `StringAssert.EndsWith(actual, expectedSuffix)` when `expectedSuffix` is an obvious non-null constant string
- `AXM1052` for `CollectionAssert.Contains(collection, expected)`
- `AXM1053` for `CollectionAssert.DoesNotContain(collection, unexpected)`
- `AXM1068` for awaited `Assert.ThrowsExceptionAsync<TException>(...)`, appending `.Thrown` when the returned exception is used
- `AXM1069` for awaited `Assert.ThrowsExactlyAsync<TException>(...)`, appending `.Thrown` when the returned exception is used
- `AXM1070` for awaited `Assert.ThrowsAsync<TException>(...)`, appending `.Thrown` when the returned exception is used
- `AXM1071` for `Assert.IsGreaterThan(lowerBound, value)`
- `AXM1072` for `Assert.IsGreaterThanOrEqualTo(lowerBound, value)`
- `AXM1073` for `Assert.IsLessThan(upperBound, value)`
- `AXM1074` for `Assert.IsLessThanOrEqualTo(upperBound, value)`
- `AXM1075` for `Assert.IsInRange(minValue, maxValue, value)`
- `AXM1081` for `Assert.Contains(expectedSubstring, actual)`, including `StringComparison` overloads
- `AXM1082` for `Assert.DoesNotContain(unexpectedSubstring, actual)`, including `StringComparison` overloads
- `AXM1083` for `Assert.StartsWith(expectedPrefix, actual)`, including `StringComparison` overloads, when `expectedPrefix` is an obvious non-null constant string
- `AXM1084` for `Assert.EndsWith(expectedSuffix, actual)`, including `StringComparison` overloads, when `expectedSuffix` is an obvious non-null constant string
- `AXM1085` for `Assert.Contains(expected, collection)`
- `AXM1086` for `Assert.DoesNotContain(unexpected, collection)`
- `AXM1087` for `CollectionAssert.AllItemsAreUnique(collection)`

MSTest migrations only cover `Assert`, `StringAssert`, and `CollectionAssert` shapes that map directly to Axiom without carrying extra message, comparer, precision, or structural-comparison semantics across.

Before:

<!-- axiom-context=migration-gallery axiom-framework=mstest -->
```csharp
Assert.AreEqual(expected, actual);
Assert.IsNull(value);
Assert.IsFalse(condition);
Assert.IsInstanceOfType(value, typeof(IDisposable));
Assert.IsGreaterThan(minimum, count);
Assert.IsInRange(minimum, maximum, count);
Assert.StartsWith("ord-", actual);
StringAssert.Contains(actual, "archived");
CollectionAssert.DoesNotContain(values, "blocked");
CollectionAssert.AllItemsAreUnique(values);
```

After:

<!-- axiom-context=migration-gallery -->
```csharp
actual.Should().Be(expected);
value.Should().BeNull();
condition.Should().BeFalse();
value.Should().BeAssignableTo<IDisposable>();
count.Should().BeGreaterThan(minimum);
count.Should().BeInRange(minimum, maximum);
actual.Should().StartWith("ord-");
actual.Should().Contain("archived");
values.Should().NotContain("blocked");
values.Should().HaveUniqueItems();
```

Async exception migrations intentionally require an awaited MSTest call. Exact-type MSTest shapes (`ThrowsExceptionAsync<TException>` and `ThrowsExactlyAsync<TException>`) map to `ThrowExactlyAsync<TException>()`; derived-type `ThrowsAsync<TException>` maps to `ThrowAsync<TException>()`.

MSTest does not expose xUnit's `ThrowsAnyAsync<TException>` or async `paramName` assertion shapes. Message-bearing MSTest async exception overloads remain manual migrations.

Ordered-value migrations preserve MSTest's bound-first argument order: `Assert.IsGreaterThan(lowerBound, value)` becomes `value.Should().BeGreaterThan(lowerBound)`.

These suggestions use semantic matching against MSTest's real `Assert`, `StringAssert`, and `CollectionAssert` APIs. They intentionally skip message-bearing, comparer, precision, structural-comparison, non-comparable ordering, and other richer MSTest assertion families, plus `StringAssert.StartsWith(...)`, `StringAssert.EndsWith(...)`, `Assert.StartsWith(...)`, and `Assert.EndsWith(...)` when the expected prefix or suffix is not an obvious non-null constant string.

For practical rollout guidance, see [Migrating to Axiom](migrating-to-axiom.md) and the framework-specific walkthroughs for [xUnit](migrate-from-xunit-assert.md), [NUnit](migrate-from-nunit-assert.md), and [MSTest](migrate-from-mstest-assert.md).
