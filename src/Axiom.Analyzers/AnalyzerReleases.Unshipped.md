### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1076 | Migration | Info | Suggest migrating xUnit Assert.IsNotAssignableFrom<T>(value) to value.Should().NotBeAssignableTo<T>().
AXM1077 | Migration | Info | Suggest migrating xUnit Assert.InRange(actual, low, high) to actual.Should().BeInRange(low, high).
AXM1078 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.Member(expected)) to collection.Should().Contain(expected).
AXM1079 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Has.No.Member(unexpected)) to collection.Should().NotContain(unexpected).
AXM1080 | Migration | Info | Suggest migrating NUnit Assert.That(collection, Is.Unique) to collection.Should().HaveUniqueItems().
