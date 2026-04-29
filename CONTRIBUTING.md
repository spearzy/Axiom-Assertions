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
- Include tests for behaviour changes.
- Update docs when public behaviour changes.
- Do not push directly to main; create a branch and open a pull request.
- Explain consumer value, package impact, API shape, and any performance or failure-output impact in the pull request.

## AI-Assisted Contributions

AI-assisted contributions are allowed.

If you use AI tools to help with implementation, tests, docs, or refactoring:

- understand the code you are submitting,
- review the output for correctness, maintainability, licensing, and security,
- add or update tests as needed,
- make sure the final change matches Axiom's design principles.

Do not submit unreviewed generated code.

Do not paste third-party code unless its license is compatible with this repository and any required attribution is included.

The contributor who opens the pull request is responsible for the final change, whether AI tools were used or not.

## Code Style

- Follow analyser guidance.
- Use dotnet format to format changes.
- Keep failure output deterministic and testable.
- Preserve pay-to-play behaviour and avoid unnecessary pass-path cost.
