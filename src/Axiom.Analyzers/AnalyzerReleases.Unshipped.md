### New Rules

Rule ID | Category | Severity | Notes
------- | -------- | -------- | -----
AXM1088 | Migration | Info | Suggest migrating direct FluentAssertions actual.Should().Be(expected) chains to Axiom actual.Should().Be(expected).
AXM1089 | Migration | Info | Suggest migrating direct FluentAssertions actual.Should().NotBe(unexpected) chains to Axiom actual.Should().NotBe(unexpected).
AXM1090 | Migration | Info | Suggest migrating direct FluentAssertions value.Should().BeNull() chains to Axiom value.Should().BeNull().
AXM1091 | Migration | Info | Suggest migrating direct FluentAssertions value.Should().NotBeNull() chains to Axiom value.Should().NotBeNull().
AXM1092 | Migration | Info | Suggest migrating direct FluentAssertions condition.Should().BeTrue() chains to Axiom condition.Should().BeTrue().
AXM1093 | Migration | Info | Suggest migrating direct FluentAssertions condition.Should().BeFalse() chains to Axiom condition.Should().BeFalse().
AXM1094 | Migration | Info | Suggest migrating direct FluentAssertions subject.Should().BeEmpty() chains to Axiom subject.Should().BeEmpty().
AXM1095 | Migration | Info | Suggest migrating direct FluentAssertions subject.Should().NotBeEmpty() chains to Axiom subject.Should().NotBeEmpty().
AXM1096 | Migration | Info | Suggest migrating direct FluentAssertions string actual.Should().Contain(expectedSubstring) chains to Axiom actual.Should().Contain(expectedSubstring).
AXM1097 | Migration | Info | Suggest migrating direct FluentAssertions string actual.Should().NotContain(unexpectedSubstring) chains to Axiom actual.Should().NotContain(unexpectedSubstring).
AXM1098 | Migration | Info | Suggest migrating direct FluentAssertions string actual.Should().StartWith(expectedPrefix) chains to Axiom actual.Should().StartWith(expectedPrefix).
AXM1099 | Migration | Info | Suggest migrating direct FluentAssertions string actual.Should().EndWith(expectedSuffix) chains to Axiom actual.Should().EndWith(expectedSuffix).
AXM1100 | Migration | Info | Suggest migrating direct FluentAssertions actual.Should().BeSameAs(expected) chains to Axiom actual.Should().BeSameAs(expected).
AXM1101 | Migration | Info | Suggest migrating direct FluentAssertions actual.Should().NotBeSameAs(unexpected) chains to Axiom actual.Should().NotBeSameAs(unexpected).
AXM1102 | Migration | Info | Suggest migrating direct FluentAssertions value.Should().BeOfType<TExpected>() chains to Axiom value.Should().BeOfType<TExpected>().
AXM1103 | Migration | Info | Suggest migrating direct FluentAssertions value.Should().BeAssignableTo<TExpected>() chains to Axiom value.Should().BeAssignableTo<TExpected>().
