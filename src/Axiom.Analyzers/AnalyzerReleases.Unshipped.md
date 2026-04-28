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
