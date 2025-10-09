using System.Text.Json.Serialization;

namespace Lettermint.Models;
public class EmailRequest
{
    public string From { get; set; } = string.Empty;
    public List<string> To { get; set; } = [];
    public string Subject { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string? Html { get; set; }
    public string? Tag { get; set; }
    public string Route { get; set; } = "outgoing";
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
    [JsonIgnore]
    public string? IdempotencyKey { get; set; }
}
