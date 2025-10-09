# Lettermint C# SDK

A simple and elegant C# SDK for sending emails via Lettermint with a fluent API.
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

Add Lettermint to your service collection:

```csharp
builder.Services.AddLettermint(options =>
{
    options.ApiKey = "your-api-key-here";
});
```


### 2. Inject and Use

Inject `ILettermintClient` into your services or controllers:

```csharp
public class EmailService(ILettermintClient _lettermint)
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

## Usage Examples

### Simple Text Email

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Hello from Lettermint")
    .Text("This is a plain text email.")
    .SetAsOutgoing()
    .SendAsync();
```

### HTML Email

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Newsletter")
    .Html("<h1>Welcome!</h1><p>Thank you for subscribing.</p>")
    .SetAsOutgoing()
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
    .Html("<h1>Welcome!</h1><p>Thank you for subscribing.</p>")
    .SetAsOutgoing()
    .SetAsBroadcast()
    .IdempotencyKey("12345678")
    .SendAsync();
```


## License

MIT

## Support

For issues and questions, please visit [GitHub Issues](https://github.com/yourusername/lettermint-csharp/issues).