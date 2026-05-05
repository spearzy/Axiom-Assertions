### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1076 | Migration | Info | Suggest migrating xUnit Assert.IsNotAssignableFrom<T>(value) to value.Should().NotBeAssignableTo<T>().
AXM1077 | Migration | Info | Suggest migrating xUnit Assert.InRange(actual, low, high) to actual.Should().BeInRange(low, high).
