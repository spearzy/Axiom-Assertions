---
title: HTTP and API Assertions for .NET with Axiom.Http
description: Use Axiom.Http for deterministic HttpResponseMessage assertions in .NET tests, including exact status codes, headers, content types, JSON bodies, and ProblemDetails checks.
---

# HTTP and API Assertions

`Axiom.Http` is the optional Axiom package for practical `HttpResponseMessage` assertions.

It stays focused on common API-test needs:

- exact status-code assertions
- header presence, absence, exact-value, and contained-value assertions
- focused content-type assertions
- body-text assertions
- JSON body assertions that reuse `Axiom.Json`
- minimal ProblemDetails assertions

## Install

```bash
dotnet add package Axiom.Http
```

Use it alongside `Axiom.Assertions`:

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Assertions;
using Axiom.Http;
```

## Supported Subject

The package is intentionally centered on `HttpResponseMessage`.

```csharp
using System.Net;
using System.Net.Http;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK);

var assertions = response.Should();
```

## Exact Status Codes

Exact status-code assertions are the primary shape for this package.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.Created)
{
    Content = new StringContent("{}", Encoding.UTF8, "application/json")
};

response.Should().HaveStatusCode(HttpStatusCode.Created);
response.Should().HaveStatusCode(201);
response.Should().NotHaveStatusCode(HttpStatusCode.BadRequest);
response.Should().NotHaveStatusCode(404);
```

## Headers and Content Type

Header lookup checks both response headers and content headers.

`HaveHeaderValue(...)` expects exactly one header value.
`ContainHeaderValue(...)` checks that any value on the header matches exactly.
`HaveHeaderValues(...)` checks exact count and exact order.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent("{\"id\":1}", Encoding.UTF8, "application/json")
};

response.Headers.Add("ETag", "\"v1\"");
response.Headers.Add("X-Trace", ["a", "b"]);

response.Should().HaveHeader("ETag");
response.Should().NotHaveHeader("Retry-After");
response.Should().HaveHeaderValue("ETag", "\"v1\"");
response.Should().ContainHeaderValue("X-Trace", "b");
response.Should().HaveHeaderValues("X-Trace", ["a", "b"]);
response.Should().HaveContentType("application/json");
response.Should().HaveContentTypeWithCharset("application/json", "utf-8");
```

## Body Text

Use body-text assertions when the response body is not JSON, or when exact text is the behaviour under test.

```csharp
using System.Net;
using System.Net.Http;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent("order created")
};

response.Should().HaveBodyText("order created");
response.Should().ContainBodyText("created");
```

## JSON Response Bodies

JSON body assertions use `Axiom.Json`.

This means JSON body equivalency and JSON path behavior stay aligned with the JSON package:

- object property order does not matter
- array order does matter
- numeric comparison uses normalized JSON numeric values

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent(
        """
        { "id": 1, "name": "Bob", "roles": ["admin", "author"] }
        """,
        Encoding.UTF8,
        "application/json")
};

var expectedJson = """
    { "roles": ["admin", "author"], "name": "Bob", "id": 1.0 }
    """;

response.Should().HaveJsonBodyEquivalentTo(expectedJson);
response.Should().BeValidJson();
response.Should().HaveJsonProperties("id", "name", "roles");
response.Should().HaveJsonPath("$.roles[1]");
response.Should().HaveJsonArrayAtPath("$.roles");
response.Should().HaveJsonArrayLengthAtPath("$.roles", 2);
response.Should().HaveJsonStringAtPath("$.name", "Bob");
response.Should().HaveJsonNumberAtPath("$.id", 1m);
```

Use contract assertions when the response body shape matters but JSON schema validation would be too much:

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.OK)
{
    Content = new StringContent(
        """
        {
          "id": "evt_123",
          "type": "order.created",
          "status": "queued",
          "customer": {
            "id": "cus_123",
            "name": "Bob"
          },
          "items": [
            { "id": "ord_1", "status": "queued" },
            { "id": "ord_2", "status": "processing" }
          ],
          "statuses": ["queued", "processing"]
        }
        """,
        Encoding.UTF8,
        "application/json")
};

response.Should().BeValidJson();
response.Should().HaveJsonProperties("id", "type", "status", "customer", "items");
response.Should().HaveJsonPropertiesAtPath("$.customer", "id", "name");
response.Should().HaveAllowedValueAtPath("$.status", "queued", "processing", "complete");
response.Should().HaveJsonObjectItemsWithPropertiesAtPath("$.items", "id", "status");
response.Should().HaveJsonObjectItemsWithOnlyPropertiesAtPath("$.items", "id", "status");
response.Should().HaveAllowedValuesAtPath("$.statuses", "queued", "processing", "complete");
```

Array-wide contract checks use the same simple path model as `Axiom.Json`: the path must resolve to an array, and wildcard selection is not part of the current path syntax.

If you want JSON assertions over raw JSON strings, `JsonDocument`, or `JsonElement` directly, use [JSON](json.md).

## ProblemDetails

Axiom.Http includes small, direct ProblemDetails assertions for common error-response checks.

These assertions expect a response body with `application/problem+json`.

```csharp
using System.Net;
using System.Net.Http;
using System.Text;
using Axiom.Http;

using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
{
    Content = new StringContent(
        """
        {
          "type": "https://example.test/problems/validation",
          "title": "Validation failed",
          "status": 400,
          "detail": "Name is required.",
          "instance": "/orders/123"
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
response.Should().HaveJsonStringAtPath("$.instance", "/orders/123");
```

For additional ProblemDetails members and extension members, use the JSON-at-path assertions rather than dedicated ProblemDetails methods.

## Current Limits

`Axiom.Http` is intentionally narrow:

- `HttpResponseMessage` only
- no ASP.NET-specific result helpers
- no test-server helpers
- no category-level status-code assertions
- no snapshot features
- no broad ProblemDetails extension-member DSL
- no full API-client helper layer

For the full method list, see the [Assertion Reference](assertion-reference.md).
