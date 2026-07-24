using lettermint_dotnet.TeamApi.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lettermint_dotnet.TeamApi;

public class TeamClient(HttpClient _httpClient) : ITeamClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };


    // GET /domains
    public async Task<LettermintDomainsPage?> ListDomains(LettermintDomainStatus? filterStatus = null, string? filterDomain = null, int pageSize = 30, string? cursor = null, CancellationToken cancellationToken = default)
    {
        var query = $"page[size]={pageSize}";
        if (filterStatus is { } status) query += $"&filter[status]={status.ToWireValue()}";
        if (!string.IsNullOrWhiteSpace(filterDomain)) query += $"&filter[domain]={filterDomain}";
        if (!string.IsNullOrWhiteSpace(cursor)) query += $"&page[cursor]={cursor}";

        var response = await _httpClient.GetAsync($"domains?{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LettermintDomainsPage>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    // GET /domains/{domainId}
    public async Task<LettermintDomain?> GetDomainDetails(string domainId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"domains/{domainId}?include=dnsRecords", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LettermintDomain>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    // POST /domains
    public async Task<LettermintDomain?> CreateDomain(string domain, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("domains", JsonContent.Create(new { domain }, options: JsonOptions), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LettermintDomain>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    // DELETE /domains/{domainId}
    public async Task DeleteDomain(string domainId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"domains/{domainId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }
    }

    // POST /domains/{domainId}/dns-records/verify
    public async Task<string?> VerifyAllDnsRecords(string domainId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"domains/{domainId}/dns-records/verify", null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LettermintMessageResponse>(JsonOptions, cancellationToken);
        return result?.Message;
    }

    // PUT /domains/{domainId}/projects
    public async Task<LettermintDomain?> UpdateProjects(string domainId, IEnumerable<string> projectIds, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsync($"domains/{domainId}/projects", JsonContent.Create(new { project_ids = projectIds }, options: JsonOptions), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<LettermintDomain>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }
}
