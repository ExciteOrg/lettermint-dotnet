using System.Text.Json.Serialization;

namespace lettermint_dotnet.SendingApi.Models;
public class EmailResponse
{
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
