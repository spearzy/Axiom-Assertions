namespace Axiom.Tests.Core.Configuration;

public sealed class AutoDetectFailureStrategyTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Assertions_UseXunitFailureStrategy_ByDefault_WithoutExplicitSetup()
    {
        AxiomServices.Reset();

        const int value = 42;
        var ex = Assert.Throws<Xunit.Sdk.XunitException>(() => value.Should().Be(7));

        Assert.Equal("Expected value to be 7, but found 42.", ex.Message);

        var strategy = Assert.IsType<AutoDetectFailureStrategy>(AxiomServices.Configuration.FailureStrategy);
        Assert.Same(XunitFailureStrategy.Instance, strategy.ResolvedStrategy);
    }

    [Fact]
    public void BatchDispose_UsesXunitFailureStrategy_ByDefault_WithoutExplicitSetup()
    {
        AxiomServices.Reset();

        var ex = Assert.Throws<Xunit.Sdk.XunitException>(() =>
        {
            using var batch = Axiom.Core.Assert.Batch("numbers");
            42.Should().Be(7);
            99.Should().Be(100);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'numbers' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected 42 to be 7, but found 42.", message);
        Assert.Contains("2) Expected 99 to be 100, but found 99.", message);
    }

    [Fact]
    public void Resolver_ChoosesNUnitStrategy_WhenNUnitIsFirstAvailableMatch()
    {
        var strategy = AutoDetectFailureStrategyResolver.Resolve(
            AutoDetectFailureStrategyResolver.BuiltInRegistrations,
            definition => definition == FrameworkFailureStrategyDefinitions.NUnit);

        Assert.Same(NUnitFailureStrategy.Instance, strategy);
    }

    [Fact]
    public void Resolver_ChoosesMSTestStrategy_WhenMSTestIsFirstAvailableMatch()
    {
        var strategy = AutoDetectFailureStrategyResolver.Resolve(
            AutoDetectFailureStrategyResolver.BuiltInRegistrations,
            definition => definition == FrameworkFailureStrategyDefinitions.MSTest);

        Assert.Same(MSTestFailureStrategy.Instance, strategy);
    }

    [Fact]
    public void Resolver_FallsBackToInvalidOperationStrategy_WhenNoSupportedFrameworkIsAvailable()
    {
        var strategy = AutoDetectFailureStrategyResolver.Resolve(
            AutoDetectFailureStrategyResolver.BuiltInRegistrations,
            static _ => false);

        Assert.Same(InvalidOperationFailureStrategy.Instance, strategy);
    }

    [Fact]
    public void AutoDetectFailureStrategy_CachesResolvedStrategy()
    {
        var resolveCount = 0;
        var strategy = new AutoDetectFailureStrategy(() =>
        {
            resolveCount++;
            return InvalidOperationFailureStrategy.Instance;
        });

        var first = Assert.Throws<InvalidOperationException>(() => strategy.Fail("first"));
        var second = Assert.Throws<InvalidOperationException>(() => strategy.Fail("second"));

        Assert.Equal("first", first.Message);
        Assert.Equal("second", second.Message);
        Assert.Equal(1, resolveCount);
    }
}
