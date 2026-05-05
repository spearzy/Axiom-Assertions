### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1076 | Migration | Info | Suggest migrating xUnit Assert.IsNotAssignableFrom<T>(value) to value.Should().NotBeAssignableTo<T>().
AXM1077 | Migration | Info | Suggest migrating xUnit Assert.InRange(actual, low, high) to actual.Should().BeInRange(low, high).
AXM1078 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.Member(expected)) to collection.Should().Contain(expected).
AXM1079 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.No.Member(unexpected)) to collection.Should().NotContain(unexpected).
AXM1080 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Unique) to collection.Should().HaveUniqueItems().
AXM1081 | Migration | Info | Suggest migrating MSTest Assert.Contains(expectedSubstring, actual) to actual.Should().Contain(expectedSubstring).
AXM1082 | Migration | Info | Suggest migrating MSTest Assert.DoesNotContain(unexpectedSubstring, actual) to actual.Should().NotContain(unexpectedSubstring).
AXM1083 | Migration | Info | Suggest migrating MSTest Assert.StartsWith(expectedPrefix, actual) to actual.Should().StartWith(expectedPrefix).
AXM1084 | Migration | Info | Suggest migrating MSTest Assert.EndsWith(expectedSuffix, actual) to actual.Should().EndWith(expectedSuffix).
AXM1085 | Migration | Info | Suggest migrating MSTest Assert.Contains(expected, collection) to collection.Should().Contain(expected).
AXM1086 | Migration | Info | Suggest migrating MSTest Assert.DoesNotContain(unexpected, collection) to collection.Should().NotContain(unexpected).
AXM1087 | Migration | Info | Suggest migrating MSTest CollectionAssert.AllItemsAreUnique(collection) to collection.Should().HaveUniqueItems().
