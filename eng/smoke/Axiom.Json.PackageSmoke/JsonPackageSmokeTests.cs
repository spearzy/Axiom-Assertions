using System.Text.Json;
using Axiom.Assertions;
using Axiom.Json;

namespace Axiom.Json.PackageSmoke;

public sealed class JsonPackageSmokeTests
{
    [Fact]
    public void JsonAssertions_WorkThroughThePublishedPackage()
    {
        var actualJson = """
            {
              "customer": {
                "id": 7,
                "name": "Bob",
                "active": true,
                "roles": ["admin", "author"],
                "deletedAt": null
              }
            }
            """;

        var expectedJson = """
            {
              "customer": {
                "roles": ["admin", "author"],
                "active": true,
                "name": "Bob",
                "id": 7.0,
                "deletedAt": null
              }
            }
            """;

        actualJson.Should().BeJsonEquivalentTo(expectedJson);
        actualJson.Should().HaveJsonPath("$.customer.roles[1]");
        actualJson.Should().HaveJsonStringAtPath("$.customer.name", "Bob");
        actualJson.Should().HaveJsonNumberAtPath("$.customer.id", 7m);
        actualJson.Should().HaveJsonBooleanAtPath("$.customer.active", true);
        actualJson.Should().HaveJsonNullAtPath("$.customer.deletedAt");
        actualJson.Should().NotHaveJsonPath("$.customer.email");
        actualJson.Should().NotBeJsonEquivalentTo("""
            { "customer": { "id": 8, "name": "Bob", "active": true, "roles": ["admin", "author"], "deletedAt": null } }
            """);

        using var document = JsonDocument.Parse(actualJson);

        document.Should().BeJsonEquivalentTo(expectedJson);
        document.RootElement.Should().HaveJsonPath("$.customer.id");
    }
}
