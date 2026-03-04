using System.Text.Json.Serialization;

namespace Lettermint.Models;
public class EmailResponse
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
