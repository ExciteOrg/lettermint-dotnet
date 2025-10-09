using Lettermint.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lettermint;

public class LettermintClient(HttpClient _httpClient) : ILettermintClient
{
    //private readonly HttpClient _httpClient;

    public EmailBuilder Email => new(this);

    //public LettermintClient(HttpClient httpClient)
    //{
    //    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    //}

    public async Task<EmailResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.From)) throw new ArgumentException("From address is required.");
        if (request.To == null || request.To.Count == 0) throw new ArgumentException("At least one recipient is required.");
        if (string.IsNullOrWhiteSpace(request.Subject)) throw new ArgumentException("Subject is required.");

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "send")
        {
            Content = JsonContent.Create(request, options: jsonOptions)
        };
        
        if (request.IdempotencyKey != null)
            httpRequest.Headers.Add("Idempotency-Key", request.IdempotencyKey);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            //if (!errorContent.Contains("This idempotency key has already been used")) // I had some emails generate this error but i cant track it again. We have ti find a better way to track the error than string contains
            throw new Exception($"Lettermint API error ({response.StatusCode}): {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<EmailResponse>(cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize response.");
    }
}