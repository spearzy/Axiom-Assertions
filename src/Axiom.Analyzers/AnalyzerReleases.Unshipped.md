### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1015 | Migration | Info | Suggest migrating xUnit Assert.IsType<T>(actual) to actual.Should().BeOfType<T>().
AXM1016 | Migration | Info | Suggest migrating xUnit Assert.IsAssignableFrom<T>(actual) to actual.Should().BeAssignableTo<T>().
AXM1017 | Migration | Info | Suggest migrating xUnit Assert.Contains(expectedSubstring, actualString) to actualString.Should().Contain(expectedSubstring).
AXM1018 | Migration | Info | Suggest migrating xUnit Assert.DoesNotContain(expectedSubstring, actualString) to actualString.Should().NotContain(expectedSubstring).
