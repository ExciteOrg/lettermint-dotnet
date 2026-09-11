# CLAUDE.md

Guidance for Claude Code when working in this repository.

## What this is

`LettermintDotNet` — a C# SDK (NuGet package) wrapping the [Lettermint](https://lettermint.co) HTTP API.
It is a library, not an application: there is nothing to "run" except the test project.

Two independent API surfaces, each with its **own API key**:

| Surface | Interface | Auth header | Base path |
| --- | --- | --- | --- |
| Sending API | `ILettermintSendingClient` | `x-lettermint-token: <ApiKey>` | `send`, `send/batch` |
| Team API | `ILettermintTeamClient` | `Authorization: Bearer <TeamApiKey>` | `domains/...` |

`AddLettermint` registers **only** the clients whose key is configured, and throws if neither
key (or no `BaseUrl`) is set — misconfiguration fails fast at startup.

## Commands

```bash
dotnet build                                    # build solution
dotnet run --project Tests/Tests.csproj         # run tests  <-- NOT `dotnet test`
dotnet pack lettermint-dotnet/lettermint-dotnet.csproj -c Release -o ./nupkg
```

**Do not use `dotnet test`.** TUnit runs on Microsoft.Testing.Platform, and the .NET 10 SDK
dispatches `dotnet test` to the unsupported VSTest target. Run the test app directly.
CI (`.github/workflows/PublishToNuget.yml`) does the same.

Target framework is `net9.0` even though the local/CI SDK is 10.x — CI installs both the
9.0 runtime and the 10.0 SDK because there is no major-version roll-forward by default.

## Layout

```
lettermint-dotnet/               # the package
  LettermintOptions.cs           # ApiKey, TeamApiKey, BaseUrl, EmailWhitelist
  ServiceCollectionExtension.cs  # AddLettermint() — conditional client registration
  SendingApi/
    EmailBuilder.cs              # fluent builder; applies the whitelist filter
    LettermintSendingClient.cs   # POST send, POST send/batch
    EmailWhitelistValidator.cs   # exact / *@domain / plus-addressing matching
    Models/                      # EmailRequest, EmailResponse, Attachment
  TeamApi/
    LettermintTeamClient.cs      # domains CRUD, DNS verify, project assignment
    Models/                      # LettermintDomain*, status enum + wire mapping
Tests/                           # TUnit + NSubstitute + Verify
```

## Conventions and gotchas

**Namespaces are inconsistent by design (or by accident) — keep both in mind.**
Public consumer-facing types (`LettermintOptions`, `ServiceCollectionExtensions`) live in
`Lettermint`; clients, builders and models live in `lettermint_dotnet.SendingApi` /
`lettermint_dotnet.TeamApi`. Consumers need `using` for both. Don't "fix" this casually —
it is part of the published API surface.

**The whitelist is applied in `EmailBuilder`, not in the client.** `To`/`Cc`/`Bcc` route
each address through `IEmailWhitelistValidator.ValidateAndFilter`, which rewrites
non-whitelisted addresses to `ok@testing.lettermint.co`. Calling
`SendEmailAsync`/`SendEmailsBatchAsync` with a hand-built `EmailRequest` **bypasses
filtering entirely**. Any new recipient-accepting method on the builder must call the
validator too.

**`IdempotencyKey` is `[JsonIgnore]`** on `EmailRequest` — it is sent as an
`Idempotency-Key` HTTP header, and only on the single-send path, not on batch.

**JSON naming differs per surface.** Both clients use `JsonNamingPolicy.CamelCase` +
`WhenWritingNull`, but Team API models override every property with an explicit
snake_case `[JsonPropertyName]`. Follow the local pattern when adding models.

**Verify snapshot tests require LF endings.** `.gitattributes` pins `*.verified.txt` /
`*.received.txt` to `eol=lf`; without it the snapshot tests fail on Windows. When a
snapshot legitimately changes, review the `.received.txt` then replace the `.verified.txt`.

**`GenerateDocumentationFile` is on**, so public members without XML docs produce CS1591
warnings. The build is not warning-free today (Team API models are undocumented); add
`<summary>` docs on new public members rather than adding to the pile.

**Error handling is uniform:** non-2xx responses throw
`Exception($"Lettermint API error ({StatusCode}): {body}")`. The one exception is
`VerifyAllDnsRecords`, which reports failure via its returned
`LettermintVerifyAllDnsRecordsResult` instead of throwing.

## Releasing

Publishing is triggered by a **GitHub release** (or manual `workflow_dispatch`), which
builds, tests, packs and pushes to NuGet. Bump `<Version>` in
`lettermint-dotnet/lettermint-dotnet.csproj` before tagging — the push uses
`--skip-duplicate`, so forgetting the bump silently publishes nothing.

`README.md` is packed into the NuGet package (`PackageReadmeFile`), so README changes ship
with the package. Keep it in sync when the public API changes.
