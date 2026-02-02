using Lettermint;
using Lettermint.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Tests;

/// <summary>
/// Tests for the EmailBuilder fluent API
/// </summary>
public class UnitTest
{
    private ILettermintClient _mockClient = null!;
    private EmailBuilder _builder = null!;

    [Before(Test)]
    public void Setup()
    {
        _mockClient = Substitute.For<ILettermintClient>();
        _builder = new EmailBuilder(_mockClient);
    }

    [Test]
    public async Task Test1()
    {
        var expectedResponse = new EmailResponse { MessageId = "msg-123", Status = "sent" };
        _mockClient.SendEmailAsync(Arg.Any<EmailRequest>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        // Act
        var request = await _builder
            .From("John Doe <john@example.com>")
            .To("alice@example.com")
            .Cc("manager@example.com")
            .Bcc("admin@example.com")
            .Tag("login")
            .Subject("Quarterly Report")
            .Html("<h1>Report</h1>")
            .Text("Report content")
            .AddAttachment("filname.ics", "1234564574573453453", null)
            .IdempotencyKey("12345678")
            .SetAsOutgoing()
            .SendAsync();

        // Assert
        var response = await _mockClient.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(r =>
                r.From == "John Doe <john@example.com>" &&
                r.To.Count == 1 &&
                r.To.Contains("alice@example.com") &&
                r.Cc != null && r.Cc.Contains("manager@example.com") &&
                r.Bcc != null && r.Bcc.Contains("admin@example.com") &&
                r.Tag == "login" &&
                r.Subject == "Quarterly Report" &&
                r.Html == "<h1>Report</h1>" &&
                r.Text == "Report content" &&
                r.IdempotencyKey == "12345678" &&
                r.Route == "outgoing" &&
                r.Attachments.Count == 1
                
                ),
            
            Arg.Any<CancellationToken>());
    }
    [Test]
    public Task AddLettermint_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedApiKey = "my-secret-key";
        var expectedBaseUrl = "https://custom.api.com";

        // Act
        services.AddLettermint(options =>
        {
            options.ApiKey = expectedApiKey;
            options.BaseUrl = expectedBaseUrl;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<IOptions<LettermintOptions>>();

        return Verify(options);
    }

    //[Test]
    //public async Task SendTestEmail()
    //{
    //    var services = new ServiceCollection();


    //    // Act
    //    services.AddLettermint(options =>
    //    {
    //        options.ApiKey = "insertKeyHere";
    //    });

    //    var serviceProvider = services.BuildServiceProvider();
    //    // Assert
    //    var client = serviceProvider.GetService<ILettermintClient>();

    //    await client.Email.To("lars@excite.dk").Subject("Test").Html("<h1>Report</h1>").IdempotencyKey("123").Tag("Test").From("info@netvaerksportalen.com").SetAsOutgoing().SendAsync();
    //}
}
