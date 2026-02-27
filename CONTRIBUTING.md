# Contributing

Thanks for your interest in contributing to Axiom.

## Development Setup

```bash
dotnet restore Axiom.sln
dotnet build Axiom.sln -c Debug
dotnet test Axiom.sln -c Debug
```

## Pull Requests

- Keep changes focused and small.
- Include tests for behavior changes.
- Update docs when public behavior changes.

## Code Style

- Follow `.editorconfig` and analyzer guidance.
- Keep failure output deterministic and testable.
