using Lettermint;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Tests;

public class EmailWhitelistValidatorTests
{
    private const string TestEmail = "ok@testing.lettermint.co";

    [Test]
    public async Task ValidateAndFilter_WithVariousScenarios_ReturnsExpectedResults()
    {
        // Arrange
        var options = Options.Create(new LettermintOptions
        {
            EmailWhitelist = [
                "allowed@example.com",
                "*@company.com",
                "user+newsletter@test.com"
            ]
        });
        var validator = new EmailWhitelistValidator(options);

        var testCases = new List<(string Input, string Expected, string Reason)>
        {
            // Exact matches
            ("allowed@example.com", "allowed@example.com", "Exact match should pass"),
            ("ALLOWED@EXAMPLE.COM", "ALLOWED@EXAMPLE.COM", "Case insensitive match"),

            // Domain wildcards
            ("anyone@company.com", "anyone@company.com", "Domain wildcard should match"),
            ("admin@company.com", "admin@company.com", "Domain wildcard should match any user"),

            // Plus-addressing
            ("allowed+tag@example.com", "allowed+tag@example.com", "Plus-addressing should match base email"),
            ("user@test.com", "user@test.com", "Base email should match plus-addressed whitelist entry"),
            ("user+anything@test.com", "user+anything@test.com", "Any plus-tag should match base"),

            // Blocked emails
            ("blocked@example.com", TestEmail, "Non-whitelisted email should redirect"),
            ("user@other.com", TestEmail, "Different domain should be blocked"),

            // Edge cases
            ("", "", "Empty string should pass through"),
            ("   ", "   ", "Whitespace should pass through"),
        };

        // Act & Assert
        var results = testCases.Select(tc => new
        {
            tc.Input,
            tc.Expected,
            tc.Reason,
            Actual = validator.ValidateAndFilter(tc.Input)
        }).ToList();

        await Verify(results);
    }

    [Test]
    public async Task ValidateAndFilter_WhenWhitelistIsEmpty_AllowsAllEmails()
    {
        // Arrange
        var options = Options.Create(new LettermintOptions
        {
            EmailWhitelist = []
        });
        var validator = new EmailWhitelistValidator(options);

        var emails = new List<string>
        {
            "anyone@anywhere.com",
            "user@example.com",
            "admin@company.com",
            "test+tag@domain.com"
        };

        // Act
        var results = emails.Select(email => validator.ValidateAndFilter(email)).ToList();

        // Assert
        await Verify(results);
    }
}
