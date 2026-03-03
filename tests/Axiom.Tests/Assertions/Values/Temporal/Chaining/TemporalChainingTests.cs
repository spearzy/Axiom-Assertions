using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.Chaining;

public sealed class TemporalChainingTests
{
    [Fact]
    public void DateTimeChain_CanBeComposed()
    {
        var value = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        value.Should()
            .BeAfter(value.AddMinutes(-1)).And
            .BeBefore(value.AddMinutes(1)).And
            .BeWithin(value.AddMilliseconds(400), TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void DateTimeOffsetChain_CanBeComposed()
    {
        var value = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        value.Should()
            .BeAfter(value.AddMinutes(-1)).And
            .BeBefore(value.AddMinutes(1)).And
            .BeWithin(value.AddMilliseconds(400), TimeSpan.FromMilliseconds(500));
    }
}
