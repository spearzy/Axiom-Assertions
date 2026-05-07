using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

namespace Axiom.Http.PackageSmoke;

public sealed class HttpPackageSmokeTests
{
    [Fact]
    public void HttpResponseAssertions_WorkThroughThePublishedPackage()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                """
                { "id": 1, "name": "Bob", "roles": ["admin", "author"] }
                """,
                Encoding.UTF8,
                "application/json")
        };

        response.Headers.Add("ETag", "\"v1\"");
        response.Headers.Add("X-Trace", ["a", "b"]);

        response.Should().HaveStatusCode(HttpStatusCode.OK);
        response.Should().HaveStatusCode(200);
        response.Should().NotHaveStatusCode(HttpStatusCode.BadRequest);
        response.Should().HaveHeader("ETag");
        response.Should().HaveHeaderValue("ETag", "\"v1\"");
        response.Should().HaveHeaderValues("X-Trace", ["a", "b"]);
        response.Should().HaveContentType("application/json");
        response.Should().HaveContentTypeWithCharset("application/json", "utf-8");
        response.Should().HaveJsonBodyEquivalentTo("""{ "roles": ["admin", "author"], "name": "Bob", "id": 1.0 }""");
        response.Should().HaveJsonPath("$.roles[1]");
        response.Should().HaveJsonStringAtPath("$.name", "Bob");
        response.Should().HaveJsonNumberAtPath("$.id", 1m);
    }

    [Fact]
    public void ProblemDetailsAssertions_WorkThroughThePublishedPackage()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                """
                {
                  "type": "https://example.test/problems/validation",
                  "title": "Validation failed",
                  "status": 400,
                  "detail": "Name is required."
                }
                """,
                Encoding.UTF8,
                "application/problem+json")
        };

        response.Should().HaveProblemDetails();
        response.Should().HaveProblemDetailsTitle("Validation failed");
        response.Should().HaveProblemDetailsStatus(400);
        response.Should().HaveProblemDetailsType("https://example.test/problems/validation");
        response.Should().HaveProblemDetailsDetail("Name is required.");
    }
}
