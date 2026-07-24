using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lettermint_dotnet.TeamApi.Models;

public class LettermintDomainsPage
{
    [JsonPropertyName("data")]
    public List<LettermintDomain> Data { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("next_page_url")]
    public string? NextPageUrl { get; set; }

    [JsonPropertyName("prev_cursor")]
    public string? PrevCursor { get; set; }

    [JsonPropertyName("prev_page_url")]
    public string? PrevPageUrl { get; set; }
}

public class LettermintMessageResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public class LettermintDomain
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("status_changed_at")]
    public string? StatusChangedAt { get; set; }

    [JsonPropertyName("dns_records")]
    public List<LettermintDnsRecord>? DnsRecords { get; set; }

    [JsonPropertyName("projects")]
    public List<LettermintProject>? Projects { get; set; }
}

public class LettermintDnsRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("hostname")]
    public string Hostname { get; set; }

    [JsonPropertyName("fqdn")]
    public string Fqdn { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("verified_at")]
    public string? VerifiedAt { get; set; }

    [JsonPropertyName("last_checked_at")]
    public string? LastCheckedAt { get; set; }
}

public class LettermintProject
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter<LettermintDomainStatus>))]
public enum LettermintDomainStatus
{
    [JsonStringEnumMemberName("verified")] Verified,
    [JsonStringEnumMemberName("partially_verified")] PartiallyVerified,
    [JsonStringEnumMemberName("pending_verification")] PendingVerification,
    [JsonStringEnumMemberName("failed_verification")] FailedVerification,
}

public static class LettermintDomainStatusExtensions
{
    /// <summary>Returns the wire value used in query strings (e.g. "pending_verification").</summary>
    public static string ToWireValue(this LettermintDomainStatus status) => status switch
    {
        LettermintDomainStatus.Verified => "verified",
        LettermintDomainStatus.PartiallyVerified => "partially_verified",
        LettermintDomainStatus.PendingVerification => "pending_verification",
        LettermintDomainStatus.FailedVerification => "failed_verification",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };
}
