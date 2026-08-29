[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.Apollo.Runners.OpenApiClient/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/Soenneker.Apollo.Runners.OpenApiClient/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.Apollo.Runners.OpenApiClient/daily-automatic-update.yml?style=for-the-badge&label=Daily%20Update)](https://github.com/soenneker/Soenneker.Apollo.Runners.OpenApiClient/actions/workflows/daily-automatic-update.yml)

# Soenneker.Apollo.Runners.OpenApiClient

The automation runner that regenerates and publishes changes to `Soenneker.Apollo.OpenApiClient` from Apollo's OpenAPI document.

This repository is an executable used by the client-maintenance workflow. It is not a NuGet package or an application dependency.

## Workflow

One run performs the following operations:

1. Clones `soenneker/soenneker.apollo.openapiclient` into a temporary directory.
2. Downloads Apollo's OpenAPI JSON document.
3. Writes a corrected copy through `IOpenApiFixer`.
4. Ensures Kiota is installed.
5. Deletes generated source content while preserving the project file.
6. Regenerates `ApolloOpenApiClient` and its models with Kiota.
7. Restores and builds the generated client in Release configuration.
8. If the build succeeds, commits and pushes the generated changes with the message `Automated update`.

The target checkout is temporary; the runner does not regenerate files in its own repository.

## Configuration

Apollo's published document is used by default:

```text
https://docs.apollo.io/openapi/apollo-rest-api.json
```

Override it through configuration when validating another document:

```json
{
  "Apollo": {
    "ClientGenerationUrl": "https://example.com/apollo-openapi.json"
  }
}
```

The executable requires these environment variables:

| Variable | Purpose |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Selects the host environment and logging configuration. |
| `GH__TOKEN` | Authenticates the final Git push. |
| `GIT__NAME` | Commit author name used by the automation. |
| `GIT__EMAIL` | Commit author email used by the automation. |

Additional configuration required by the shared runner, Git, download, and logging utilities must also be available to the host.

## Run locally

```bash
dotnet run --project src/Soenneker.Apollo.Runners.OpenApiClient
```

Set the required environment variables before starting the process. The runner has no dry-run mode: a successful generation and build proceeds to commit and push. Use credentials and a target configuration intended for automation, and do not run it merely to preview generated output.

Pressing Ctrl+C requests cancellation. Cancellation stops pending work where supported, but it does not roll back downloads, generated files, commits, or remote operations that have already completed.

## Failure behavior

- A failed OpenAPI download terminates the run with an exception.
- Individual cleanup failures are logged so the workflow can continue where possible.
- A failed generated-client build is logged and prevents the commit/push step.
- An unhandled workflow exception sets the process exit code to `1`.
- Cancellation before a normal result uses exit code `-1`.

## Internal service

`IFileOperationsUtil.Process(CancellationToken)` owns the complete clone/download/fix/generate/build/push workflow. `Startup.SetupIoC` registers it with the shared runner dependencies and a hosted service that shuts down the process after one execution.
