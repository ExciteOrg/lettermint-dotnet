namespace Lettermint;

/// <summary>
/// Validates and filters email addresses based on a configured whitelist.
/// Used primarily in development environments to prevent emails to non-whitelisted addresses.
/// </summary>
public interface IEmailWhitelistValidator
{
    /// <summary>
    /// Validates an email address against the whitelist.
    /// Returns the original email if whitelisted, or a safe test email if not.
    /// </summary>
    /// <param name="emailAddress">Plain email address to validate (e.g., "user@example.com").</param>
    /// <returns>The validated email address (original if whitelisted, test email otherwise)</returns>
    string ValidateAndFilter(string emailAddress);

    /// <summary>
    /// Checks if the whitelist is enabled (has entries).
    /// </summary>
    bool IsEnabled { get; }
}
