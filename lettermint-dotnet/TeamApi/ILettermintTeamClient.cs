using Lettermint;
using lettermint_dotnet.SendingApi;
using lettermint_dotnet.TeamApi.Models;

namespace lettermint_dotnet.TeamApi;

/// <summary>
/// Client for the Lettermint Team API.
/// Authenticates with <see cref="LettermintOptions.TeamApiKey"/>, which is separate
/// from the sending API key used by <see cref="ILettermintSendingClient"/>.
/// </summary>
public interface ILettermintTeamClient
{
    Task<LettermintDomainsPage?> ListDomains(LettermintDomainStatus? filterStatus = null, string? filterDomain = null, int pageSize = 30, string? cursor = null, CancellationToken cancellationToken = default);
    Task<LettermintDomain?> GetDomainDetails(string domainId, CancellationToken cancellationToken = default);
    Task<LettermintDomain?> CreateDomain(string domain, CancellationToken cancellationToken = default);
    Task DeleteDomain(string domainId, CancellationToken cancellationToken = default);
    Task<LettermintVerifyAllDnsRecordsResult> VerifyAllDnsRecords(string domainId, CancellationToken cancellationToken = default);
    Task<LettermintDomain?> UpdateProjects(string domainId, IEnumerable<string> projectIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the HTML body of a message.
    /// </summary>
    /// <param name="messageId">The id of the message.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The HTML body, or <c>null</c> when the message has no HTML part.</returns>
    Task<string?> GetMessageHtml(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the plain text body of a message.
    /// </summary>
    /// <param name="messageId">The id of the message.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The plain text body, or <c>null</c> when the message has no text part.</returns>
    Task<string?> GetMessageText(string messageId, CancellationToken cancellationToken = default);
}
