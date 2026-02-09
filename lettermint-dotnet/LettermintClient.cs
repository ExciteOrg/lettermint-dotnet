using Lettermint.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lettermint;

public class LettermintClient(HttpClient _httpClient, IOptions<LettermintOptions> _options, IEmailWhitelistValidator _validator) : ILettermintClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public EmailBuilder Email => new(this, _validator);

    public async Task<List<EmailResponse>> SendEmailsBatchAsync(List<EmailRequest> requests, CancellationToken cancellationToken = default)
    {
        Validate(requests);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "send/batch")
        {
            Content = JsonContent.Create(requests, options: JsonOptions)
        };

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<EmailResponse>>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<EmailResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        Validate([request]);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "send")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };

        if (request.IdempotencyKey != null)
            httpRequest.Headers.Add("Idempotency-Key", request.IdempotencyKey);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<EmailResponse>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    private void Validate(List<EmailRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0) throw new ArgumentException("At least one email request is required.", nameof(requests));
        foreach (var request in requests)
        {
            if (string.IsNullOrWhiteSpace(request.From)) throw new ArgumentException("From address is required.");
            if (request.To == null || request.To.Count == 0) throw new ArgumentException("At least one recipient is required.");
            if (string.IsNullOrWhiteSpace(request.Subject)) throw new ArgumentException("Subject is required.");
        }
    }
}