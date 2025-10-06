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
public class EmailService
{
    private readonly ILettermintClient _lettermint;

    public EmailService(ILettermintClient lettermint)
    {
        _lettermint = lettermint;
    }

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
    .SendAsync();
```

### HTML Email

```csharp
var response = await _lettermint.Email
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Newsletter")
    .Html("<h1>Welcome!</h1><p>Thank you for subscribing.</p>")
    .SendAsync();
```



## Configuration Options

```csharp
builder.Services.AddLettermint(options =>
{
    // Required: Your Lettermint API key
    options.ApiKey = "your-api-key";
    
    // Optional: Custom API base URL (defaults to https://api.lettermint.com/v1)
    options.BaseUrl = "https://api.lettermint.com/v1";
    
    // Optional: Add custom headers to all requests
    options.CustomHeaders = new Dictionary<string, string>
    {
        { "X-Custom-Header", "value" }
    };
});
```

## License

MIT

## Support

For issues and questions, please visit [GitHub Issues](https://github.com/yourusername/lettermint-csharp/issues).