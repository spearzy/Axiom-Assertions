namespace Axiom.Tests.EntryPoints.Should;

public sealed class ActionShouldEntryPointTests
{
    [Fact]
    public void Should_ForAction_CapturesSubjectAndExpression()
    {
        Action action = static () => { };

        var assertions = action.Should();

        Xunit.Assert.Same(action, assertions.Subject);
        Xunit.Assert.Equal("action", assertions.SubjectExpression);
    }
}
