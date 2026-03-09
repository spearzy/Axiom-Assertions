using System.Collections;
using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToExpressionSelectorTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void GivenIgnoreSelector_WhenNestedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "AB1 2CD", Country = "UK" },
        };
        var expected = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "ZZ1 9ZZ", Country = "UK" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.Ignore<Person>(x => x.Address!.Postcode)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenOnlyCompareSelector_WhenUnselectedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new Person { Name = "Bob", Age = 36 };
        var expected = new Person { Name = "Bob", Age = 99 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.OnlyCompare<Person>(x => x.Name)));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenOnlyCompareSelectors_WhenSelectedMemberDiffers_ThenThrows()
    {
        var actual = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "AB1 2CD", Country = "UK" },
        };
        var expected = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "ZZ1 9ZZ", Country = "UK" },
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.OnlyCompare<Person>(
                    x => x.Name,
                    x => x.Address!.Postcode)));

        Assert.Contains("actual.Address.Postcode", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenUseComparerSelector_WhenStringMemberDiffersByCase_ThenUsesConfiguredComparer()
    {
        var actual = new Person { Name = "Bob", Age = 30 };
        var expected = new Person { Name = "Bob", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparer<Person>(x => x.Name, StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenUseCollectionItemComparerSelector_WhenItemsMatchByConfiguredRule_ThenDoesNotThrow()
    {
        var actual = new Order
        {
            Items =
            [
                new LineItem { Sku = "A-1", Quantity = 1 },
                new LineItem { Sku = "B-2", Quantity = 2 },
            ]
        };

        var expected = new Order
        {
            Items =
            [
                new LineItem { Sku = "A-1", Quantity = 100 },
                new LineItem { Sku = "B-2", Quantity = 200 },
            ]
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseCollectionItemComparer<Order>(x => x.Items, new LineItemSkuComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenRelativeIgnorePath_WhenNestedValueDiffers_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "AB1 2CD", Country = "UK" },
        };
        var expected = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "ZZ1 9ZZ", Country = "UK" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnorePath("Address")));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenNonMemberSelector_WhenIgnoring_ThenThrowsArgumentException()
    {
        var actual = new Person { Name = "Bob", Age = 36 };
        var expected = new Person { Name = "Bob", Age = 36 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.Ignore<Person>(x => x.Name!.ToUpperInvariant())));

        Assert.Equal("memberSelector", ex.ParamName);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
        public Address? Address { get; init; }
    }

    private sealed class Address
    {
        public string? Postcode { get; init; }
        public string? Country { get; init; }
    }

    private sealed class Order
    {
        public LineItem[] Items { get; init; } = [];
    }

    private sealed class LineItem
    {
        public string? Sku { get; init; }
        public int Quantity { get; init; }
    }

    private sealed class LineItemSkuComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            if (x is null || y is null)
            {
                return x is null && y is null;
            }

            if (x is not LineItem left || y is not LineItem right)
            {
                return object.Equals(x, y);
            }

            return string.Equals(left.Sku, right.Sku, StringComparison.Ordinal);
        }

        public int GetHashCode(object obj)
        {
            if (obj is LineItem item)
            {
                return StringComparer.Ordinal.GetHashCode(item.Sku ?? string.Empty);
            }

            return obj.GetHashCode();
        }
    }
}
