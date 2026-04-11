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
