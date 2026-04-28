## Release 0.7.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM0001 | Usage | Warning | Detect ignored async Axiom assertion calls.
AXM0002 | Usage | Warning | Detect Batch instances that are created without being disposed.

## Release 0.10.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1001 | Migration | Info | Suggest migrating xUnit Assert.Equal(expected, actual) to actual.Should().Be(expected).
AXM1002 | Migration | Info | Suggest migrating xUnit Assert.NotEqual(expected, actual) to actual.Should().NotBe(expected).
AXM1003 | Migration | Info | Suggest migrating xUnit Assert.Null(value) to value.Should().BeNull().
AXM1004 | Migration | Info | Suggest migrating xUnit Assert.NotNull(value) to value.Should().NotBeNull().
AXM1005 | Migration | Info | Suggest migrating xUnit Assert.True(condition) to condition.Should().BeTrue().
AXM1006 | Migration | Info | Suggest migrating xUnit Assert.False(condition) to condition.Should().BeFalse().
AXM1007 | Migration | Info | Suggest migrating xUnit Assert.Empty(subject) to subject.Should().BeEmpty().
AXM1008 | Migration | Info | Suggest migrating xUnit Assert.NotEmpty(subject) to subject.Should().NotBeEmpty().
AXM1009 | Migration | Info | Suggest migrating xUnit Assert.Contains(item, collection) to collection.Should().Contain(item).
AXM1010 | Migration | Info | Suggest migrating xUnit Assert.DoesNotContain(item, collection) to collection.Should().NotContain(item).
AXM1011 | Migration | Info | Suggest migrating xUnit Assert.Single(subject) to subject.Should().ContainSingle().
AXM1012 | Migration | Info | Suggest migrating xUnit Assert.Same(expected, actual) to actual.Should().BeSameAs(expected).
AXM1013 | Migration | Info | Suggest migrating xUnit Assert.NotSame(expected, actual) to actual.Should().NotBeSameAs(expected).
AXM1014 | Migration | Info | Suggest migrating xUnit Assert.Throws<TException>(...) to an Axiom .Should().Throw<TException>() assertion when the result is not consumed.

## Release 1.1.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1015 | Migration | Info | Suggest migrating xUnit Assert.IsType<T>(actual) to actual.Should().BeOfType<T>().
AXM1016 | Migration | Info | Suggest migrating xUnit Assert.IsAssignableFrom<T>(actual) to actual.Should().BeAssignableTo<T>().
AXM1017 | Migration | Info | Suggest migrating xUnit Assert.Contains(expectedSubstring, actualString) to actualString.Should().Contain(expectedSubstring).
AXM1018 | Migration | Info | Suggest migrating xUnit Assert.DoesNotContain(expectedSubstring, actualString) to actualString.Should().NotContain(expectedSubstring).
AXM1019 | Migration | Info | Suggest migrating xUnit Assert.Single(collection, predicate) to collection.Should().ContainSingle(predicate), appending .SingleItem when the matched item is used.
AXM1020 | Migration | Info | Suggest migrating xUnit Assert.Contains(key, dictionary) to dictionary.Should().ContainKey(key), appending .WhoseValue when the associated value is used.
AXM1021 | Migration | Info | Suggest migrating xUnit Assert.DoesNotContain(key, dictionary) to dictionary.Should().NotContainKey(key).
AXM1022 | Migration | Info | Suggest migrating xUnit Assert.StartsWith(expectedPrefix, actualString) to actualString.Should().StartWith(expectedPrefix).
AXM1023 | Migration | Info | Suggest migrating xUnit Assert.EndsWith(expectedSuffix, actualString) to actualString.Should().EndWith(expectedSuffix).

## Release 1.2.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1024 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.EqualTo(expected)) to actual.Should().Be(expected).
AXM1025 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.EqualTo(expected)) to actual.Should().NotBe(expected).
AXM1026 | Migration | Info | Suggest migrating NUnit Assert.That(value, Is.Null) to value.Should().BeNull().
AXM1027 | Migration | Info | Suggest migrating NUnit Assert.That(value, Is.Not.Null) to value.Should().NotBeNull().
AXM1028 | Migration | Info | Suggest migrating NUnit Assert.That(condition, Is.True) to condition.Should().BeTrue().
AXM1029 | Migration | Info | Suggest migrating NUnit Assert.That(condition, Is.False) to condition.Should().BeFalse().
AXM1030 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Empty) to collection.Should().BeEmpty().
AXM1031 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Not.Empty) to collection.Should().NotBeEmpty().

## Release 1.3.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1032 | Migration | Info | Suggest migrating MSTest Assert.AreEqual(expected, actual) to actual.Should().Be(expected).
AXM1033 | Migration | Info | Suggest migrating MSTest Assert.AreNotEqual(expected, actual) to actual.Should().NotBe(expected).
AXM1034 | Migration | Info | Suggest migrating MSTest Assert.IsNull(value) to value.Should().BeNull().
AXM1035 | Migration | Info | Suggest migrating MSTest Assert.IsNotNull(value) to value.Should().NotBeNull().
AXM1036 | Migration | Info | Suggest migrating MSTest Assert.IsTrue(condition) to condition.Should().BeTrue().
AXM1037 | Migration | Info | Suggest migrating MSTest Assert.IsFalse(condition) to condition.Should().BeFalse().
AXM1038 | Migration | Info | Suggest migrating MSTest Assert.AreSame(expected, actual) to actual.Should().BeSameAs(expected).
AXM1039 | Migration | Info | Suggest migrating MSTest Assert.AreNotSame(expected, actual) to actual.Should().NotBeSameAs(expected).

## Release 2.0.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1040 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.Contain(expectedSubstring)) to actual.Should().Contain(expectedSubstring).
AXM1041 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.Not.Contain(expectedSubstring)) to actual.Should().NotContain(expectedSubstring).
AXM1042 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.StartWith(expectedPrefix)) to actual.Should().StartWith(expectedPrefix).
AXM1043 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Does.EndWith(expectedSuffix)) to actual.Should().EndWith(expectedSuffix).
AXM1044 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.Count.EqualTo(expectedCount)) to collection.Should().HaveCount(expectedCount).
AXM1045 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.SameAs(expected)) to actual.Should().BeSameAs(expected).
AXM1046 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.SameAs(expected)) to actual.Should().NotBeSameAs(expected).
AXM1047 | Migration | Info | Suggest migrating MSTest Assert.IsInstanceOfType(value, typeof(T)) to value.Should().BeAssignableTo<T>().
AXM1048 | Migration | Info | Suggest migrating MSTest Assert.IsNotInstanceOfType(value, typeof(T)) to value.Should().NotBeAssignableTo<T>().
AXM1049 | Migration | Info | Suggest migrating MSTest StringAssert.Contains(actual, expectedSubstring) to actual.Should().Contain(expectedSubstring).
AXM1050 | Migration | Info | Suggest migrating MSTest StringAssert.StartsWith(actual, expectedPrefix) to actual.Should().StartWith(expectedPrefix).
AXM1051 | Migration | Info | Suggest migrating MSTest StringAssert.EndsWith(actual, expectedSuffix) to actual.Should().EndWith(expectedSuffix).
AXM1052 | Migration | Info | Suggest migrating MSTest CollectionAssert.Contains(collection, expected) to collection.Should().Contain(expected).
AXM1053 | Migration | Info | Suggest migrating MSTest CollectionAssert.DoesNotContain(collection, unexpected) to collection.Should().NotContain(unexpected).

## Release 2.3.0

### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1054 | Migration | Info | Suggest migrating awaited xUnit Assert.ThrowsAsync<TException>(...) to an Axiom awaitable .Should().ThrowExactlyAsync<TException>() assertion, chaining .WithParamName(...) for non-null constant param-name overloads and appending .Thrown when the exception is used.
AXM1055 | Migration | Info | Suggest migrating awaited xUnit Assert.ThrowsAnyAsync<TException>(...) to an Axiom awaitable .Should().ThrowAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1056 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.GreaterThan(expected)) to actual.Should().BeGreaterThan(expected).
AXM1057 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.GreaterThanOrEqualTo(expected)) to actual.Should().BeGreaterThanOrEqualTo(expected).
AXM1058 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.LessThan(expected)) to actual.Should().BeLessThan(expected).
AXM1059 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.LessThanOrEqualTo(expected)) to actual.Should().BeLessThanOrEqualTo(expected).
AXM1060 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.InRange(minimum, maximum)) to actual.Should().BeInRange(minimum, maximum).
AXM1061 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.TypeOf<TExpected>()) to actual.Should().BeOfType<TExpected>().
AXM1062 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.InstanceOf<TExpected>()) to actual.Should().BeAssignableTo<TExpected>().
AXM1063 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.AssignableTo<TExpected>()) to actual.Should().BeAssignableTo<TExpected>().
AXM1064 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.InstanceOf<TExpected>()) to actual.Should().NotBeAssignableTo<TExpected>().
AXM1065 | Migration | Info | Suggest migrating NUnit Assert.That(actual, Is.Not.AssignableTo<TExpected>()) to actual.Should().NotBeAssignableTo<TExpected>().
AXM1066 | Migration | Info | Suggest migrating NUnit Assert.ThrowsAsync<TException>(...) to an Axiom awaitable .Should().ThrowExactlyAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1067 | Migration | Info | Suggest migrating NUnit Assert.CatchAsync<TException>(...) to an Axiom awaitable .Should().ThrowAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1068 | Migration | Info | Suggest migrating awaited MSTest Assert.ThrowsExceptionAsync<TException>(...) to an Axiom awaitable .Should().ThrowExactlyAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1069 | Migration | Info | Suggest migrating awaited MSTest Assert.ThrowsExactlyAsync<TException>(...) to an Axiom awaitable .Should().ThrowExactlyAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1070 | Migration | Info | Suggest migrating awaited MSTest Assert.ThrowsAsync<TException>(...) to an Axiom awaitable .Should().ThrowAsync<TException>() assertion, appending .Thrown when the exception is used.
AXM1071 | Migration | Info | Suggest migrating MSTest Assert.IsGreaterThan(lowerBound, value) to value.Should().BeGreaterThan(lowerBound).
AXM1072 | Migration | Info | Suggest migrating MSTest Assert.IsGreaterThanOrEqualTo(lowerBound, value) to value.Should().BeGreaterThanOrEqualTo(lowerBound).
AXM1073 | Migration | Info | Suggest migrating MSTest Assert.IsLessThan(upperBound, value) to value.Should().BeLessThan(upperBound).
AXM1074 | Migration | Info | Suggest migrating MSTest Assert.IsLessThanOrEqualTo(upperBound, value) to value.Should().BeLessThanOrEqualTo(upperBound).
AXM1075 | Migration | Info | Suggest migrating MSTest Assert.IsInRange(minValue, maxValue, value) to value.Should().BeInRange(minValue, maxValue).
