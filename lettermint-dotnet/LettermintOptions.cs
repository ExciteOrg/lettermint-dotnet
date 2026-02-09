namespace Lettermint;

public class LettermintOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.lettermint.co/v1/";

    /// <summary>
    /// Email whitelist for development/testing environments.
    /// When configured, only emails matching these patterns will be sent; others are redirected to a test address.
    ///
    /// Supported formats:
    /// - Exact email: "user@example.com"
    /// - Domain wildcard: "*@example.com" (allows any email at this domain)
    /// - Plus-addressing: "user@example.com" will also match "user+tag@example.com"
    ///
    /// Leave empty to disable filtering (all emails allowed - use in production).
    /// </summary>
    public List<string> EmailWhitelist { get; set; } = [];
}
