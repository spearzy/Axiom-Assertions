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
