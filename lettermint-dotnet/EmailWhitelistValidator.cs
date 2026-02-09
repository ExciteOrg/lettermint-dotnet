using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lettermint;

/// <summary>
/// Validates email addresses against a whitelist configuration.
/// Supports exact email matches and domain wildcards (e.g., "*@example.com").
/// </summary>
public class EmailWhitelistValidator : IEmailWhitelistValidator
{
    private readonly HashSet<string> _exactMatches;
    private readonly HashSet<string> _domainWildcards;
    private readonly ILogger<EmailWhitelistValidator>? _logger;
    private const string TestEmail = "ok@testing.lettermint.co";

    public bool IsEnabled => _exactMatches.Count > 0 || _domainWildcards.Count > 0;

    public EmailWhitelistValidator(IOptions<LettermintOptions> options, ILogger<EmailWhitelistValidator>? logger = null)
    {
        _logger = logger;
        _exactMatches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _domainWildcards = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        ProcessWhitelist(options.Value.EmailWhitelist);
    }

    private void ProcessWhitelist(List<string> whitelist)
    {
        foreach (var entry in whitelist)
        {
            if (string.IsNullOrWhiteSpace(entry))
                continue;

            var normalized = entry.Trim();

            // Handle domain wildcards like "*@example.com"
            if (normalized.StartsWith("*@"))
            {
                var domain = normalized.Substring(1); // Store as "@example.com"
                _domainWildcards.Add(domain);
            }
            else
            {
                // Store the base part (before +) for plus-addressing support
                var atIndex = normalized.IndexOf('@');
                if (atIndex > 0)
                {
                    var localPart = normalized.Substring(0, atIndex);
                    var plusIndex = localPart.IndexOf('+');

                    if (plusIndex > 0)
                    {
                        // Store base email without plus-addressing
                        var baseEmail = localPart.Substring(0, plusIndex) + normalized.Substring(atIndex);
                        _exactMatches.Add(baseEmail);
                    }
                    else
                    {
                        _exactMatches.Add(normalized);
                    }
                }
            }
        }
    }

    public string ValidateAndFilter(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return emailAddress;

        // If whitelist is disabled (empty), allow all emails
        if (!IsEnabled)
            return emailAddress;

        var email = emailAddress.Trim();

        // Check if email is whitelisted
        if (IsWhitelisted(email))
            return emailAddress;

        // Not whitelisted - replace with test email
        _logger?.LogWarning(
            "Email address {Email} is not whitelisted. Redirecting to {TestEmail}",
            email,
            TestEmail);

        return TestEmail;
    }

    private bool IsWhitelisted(string email)
    {
        email = email.Trim();

        // Check exact match first
        if (_exactMatches.Contains(email))
            return true;

        // Check plus-addressing variant (user+tag@domain.com -> user@domain.com)
        var atIndex = email.IndexOf('@');
        if (atIndex > 0)
        {
            var localPart = email.Substring(0, atIndex);
            var domain = email.Substring(atIndex);
            var plusIndex = localPart.IndexOf('+');

            if (plusIndex > 0)
            {
                var baseEmail = localPart.Substring(0, plusIndex) + domain;
                if (_exactMatches.Contains(baseEmail))
                    return true;
            }

            // Check domain wildcard (e.g., *@example.com)
            if (_domainWildcards.Contains(domain))
                return true;
        }

        return false;
    }
}
