#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "Usage: $0 <package-source-directory> [axiom-json-version]"
  exit 1
fi

package_source="$1"
package_version="${2:-}"

if [[ ! -d "$package_source" ]]; then
  echo "Package source directory does not exist: $package_source"
  exit 1
fi

if [[ -z "$package_version" ]]; then
  package_file="$(ls "$package_source"/Axiom.Json.*.nupkg 2>/dev/null | head -n 1 || true)"
  if [[ -z "$package_file" ]]; then
    echo "Could not find Axiom.Json package in: $package_source"
    exit 1
  fi

  package_name="$(basename "$package_file")"
  package_version="${package_name#Axiom.Json.}"
  package_version="${package_version%.nupkg}"
fi

smoke_root="$(mktemp -d)"
trap 'rm -rf "$smoke_root"' EXIT

consumer_project="$smoke_root/Axiom.Json.Smoke"
local_packages_cache="$smoke_root/.nuget/packages"
nuget_config="$smoke_root/NuGet.config"

cat > "$nuget_config" <<EOF2
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_source" />
  </packageSources>
</configuration>
EOF2

dotnet new console -n Axiom.Json.Smoke -f net10.0 -o "$consumer_project" --no-restore >/dev/null

cd "$consumer_project"
dotnet add package Axiom.Json --version "$package_version" --source "$package_source" --no-restore >/dev/null

cat > Program.cs <<'EOF2'
using System;
using System.Text.Json;
using Axiom.Assertions;
using Axiom.Json;

var actualJson = """
    {
      "customer": {
        "id": 7,
        "name": "Ada",
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
        "name": "Ada",
        "id": 7.0,
        "deletedAt": null
      }
    }
    """;

actualJson.Should().BeJsonEquivalentTo(expectedJson);
actualJson.Should().HaveJsonPath("$.customer.roles[1]");
actualJson.Should().HaveJsonStringAtPath("$.customer.name", "Ada");
actualJson.Should().HaveJsonNumberAtPath("$.customer.id", 7m);
actualJson.Should().HaveJsonBooleanAtPath("$.customer.active", true);
actualJson.Should().HaveJsonNullAtPath("$.customer.deletedAt");
actualJson.Should().NotHaveJsonPath("$.customer.email");
actualJson.Should().NotBeJsonEquivalentTo("""
    { "customer": { "id": 8, "name": "Ada", "active": true, "roles": ["admin", "author"], "deletedAt": null } }
    """);

using var document = JsonDocument.Parse(actualJson);
document.Should().BeJsonEquivalentTo(expectedJson);
document.RootElement.Should().HaveJsonPath("$.customer.id");

Console.WriteLine("Axiom.Json smoke passed.");
EOF2

dotnet run --project Axiom.Json.Smoke.csproj --configfile "$nuget_config" --packages "$local_packages_cache"
