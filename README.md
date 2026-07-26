# Lettermint C# SDK

A simple and elegant C# SDK for Lettermint with a fluent API.
It covers two separate Lettermint APIs:

- **Sending API** (`ILettermintSendingClient`) — send transactional and marketing emails.
- **Team API** (`ILettermintTeamClient`) — manage domains, DNS records and projects.

The two APIs use **separate API keys**, so you can configure one or both. Only the
clients whose key you provide are registered.

Feel free to open an issue or a pull request to add more features.

## Installation

Install the package via NuGet:

```bash
dotnet add package Lettermint
```

Or via Package Manager Console:

```powershell
Install-Package Lettermint
```

## Quick Start

### 1. Configure the Service

Add Lettermint to your service collection. Provide the key(s) for the API(s) you use:

```csharp
builder.Services.AddLettermint(options =>
{
    options.ApiKey = "your-sending-api-key";   // enables ILettermintSendingClient
    options.TeamApiKey = "your-team-api-key";  // enables ILettermintTeamClient
});
```

- Set **only `ApiKey`** → only the sending client (`ILettermintSendingClient`) is registered.
- Set **only `TeamApiKey`** → only the team client (`ILettermintTeamClient`) is registered.
- Set **both** → both clients are registered.
- Set **neither** → registration throws, so misconfiguration fails fast at startup.

> The two keys authenticate differently under the hood: the sending API uses an
> `x-lettermint-token` header, while the team API uses a `Bearer` token in the
> `Authorization` header. The SDK handles this for you.

#### Email whitelist (sending API)

You can add whitelisted emails. This is good for dev and test environments where you want to make sure you don't hurt your domain reputation.

Supported formats:
- Exact email: `"user@example.com"` (also allows plus addressing `"user+tag@example.com"`)
- Domain wildcard: `"*@example.com"` (allows any email at this domain)
- Leave empty to disable filtering (all emails allowed - use in production)

```csharp
builder.Services.AddLettermint(options =>
{
    options.ApiKey = "your-sending-api-key";
    options.EmailWhitelist = ["email@one.dk", "Email@two.dk"];
});
```

### 2. Inject and Use

Inject `ILettermintSendingClient` (sending) and/or `ILettermintTeamClient` (team) into your services or controllers:

```csharp
public class EmailService(ILettermintSendingClient _lettermint)
{
    public async Task SendWelcomeEmail(string recipientEmail, string name)
    {
        var response = await _lettermint.Email
            .From("noreply@yourdomain.com")
            .To(recipientEmail)
            .Subject("Welcome!")
            .Text($"Hello {name}, welcome to our service!")
            .SendAsync();

        Console.WriteLine($"Email sent! Message ID: {response.MessageId}");
    }
}
```

## Sending API

### Simple Text Email

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Hello from Lettermint")
    .SetTextBody("This is a plain text email.")
    .SetRouteAsOutgoing()
    .SendAsync();
```

### HTML Email

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Newsletter")
    .SetHtmlBody("<h1>Welcome!</h1><p>Thank you for subscribing.</p>")
    .SetRouteAsOutgoing()
    .SendAsync();
```

### All Email methods

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .From("John", "john@john.com")
    .To("recipient@example.com")
    .To("John", "john@john.com")
    .Tag("Login")
    .Subject("Newsletter")
    .SetTextBody("This is a plain text email.")
    .SetHtmlBody("<h1>Welcome!</h1><p>Thank you for subscribing.</p>")
    .SetRouteAsOutgoing()
    .SetRouteAsBroadcast()
    .SetRoute("specificroute")
    .IdempotencyKey("12345678")
    .SendAsync();
```

## Team API

Inject `ILettermintTeamClient` to manage domains, DNS records and projects. All methods accept an
optional `CancellationToken`, and throw on a non-success response with the API's error body —
except `VerifyAllDnsRecords`, which reports failure through its return value.

```csharp
public class DomainService(ILettermintTeamClient _team)
{
    public async Task Example()
    {
        // List domains (paginated), optionally filtered by status or domain name
        var page = await _team.ListDomains(
            filterStatus: LettermintDomainStatus.Verified,
            pageSize: 30);

        foreach (var domain in page!.Data)
            Console.WriteLine($"{domain.Domain} ({domain.Id})");

        // Get a single domain, including its DNS records
        var details = await _team.GetDomainDetails("domain-id");

        // Create a new domain
        var created = await _team.CreateDomain("example.com");

        // Trigger verification of all DNS records for a domain
        var verification = await _team.VerifyAllDnsRecords("domain-id");
        Console.WriteLine($"{verification.Verified}: {verification.Message}");

        // Assign projects to a domain
        var updated = await _team.UpdateProjects("domain-id", ["project-id-1", "project-id-2"]);

        // Delete a domain
        await _team.DeleteDomain("domain-id");
    }
}
```

### Domain status

`LettermintDomainStatus` is a strongly-typed enum used for the `ListDomains` status filter
and mapped to the API's wire values automatically:

| Enum value | Wire value |
| --- | --- |
| `LettermintDomainStatus.Verified` | `verified` |
| `LettermintDomainStatus.PartiallyVerified` | `partially_verified` |
| `LettermintDomainStatus.PendingVerification` | `pending_verification` |
| `LettermintDomainStatus.FailedVerification` | `failed_verification` |

### Team API methods

| Method | HTTP | Description |
| --- | --- | --- |
| `ListDomains(filterStatus?, filterDomain?, pageSize, cursor?, ct)` | `GET /domains` | List domains with optional status/domain filters and cursor pagination. |
| `GetDomainDetails(domainId, ct)` | `GET /domains/{id}?include=dnsRecords` | Get a single domain including its DNS records. |
| `CreateDomain(domain, ct)` | `POST /domains` | Create a new domain. |
| `DeleteDomain(domainId, ct)` | `DELETE /domains/{id}` | Delete a domain. Throws on failure. |
| `VerifyAllDnsRecords(domainId, ct)` | `POST /domains/{id}/dns-records/verify` | Trigger verification of all DNS records; returns a `LettermintVerifyAllDnsRecordsResult` with `Verified` and `Message` instead of throwing on failure. |
| `UpdateProjects(domainId, projectIds, ct)` | `PUT /domains/{id}/projects` | Assign the given project ids to a domain. |

## License

MIT

## Support

For issues and questions, please visit [GitHub Issues](https://github.com/yourusername/lettermint-csharp/issues).
