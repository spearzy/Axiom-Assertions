namespace Axiom.Tests.Core.Rendering;

public sealed class FailureMessageRendererTests
{
    [Fact]
    public void Render_WithExpectedValue_FormatsStringValuesDeterministically()
    {
        var failure = new Failure(
            "value",
            new Expectation("to start with", "ab"),
            "test");

        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to start with \"ab\", but found \"test\".";
        Assert.Equal(expected, message);
    }

    [Fact]
    public void Render_WithoutExpectedValue_UsesNullTokenForActual()
    {
        var failure = new Failure(
            "value",
            new Expectation("to not be null", IncludeExpectedValue: false),
            null);

        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to not be null, but found <null>.";
        Assert.Equal(expected, message);
    }

    [Fact]
    public void Render_WithReason_IncludesBecauseClause()
    {
        var failure = new Failure(
            "value",
            new Expectation("to be", 7),
            42,
            "input should match seeded data");

        var message = FailureMessageRenderer.Render(failure);

        const string expected = "Expected value to be 7 because input should match seeded data, but found 42.";
        Assert.Equal(expected, message);
    }
}
