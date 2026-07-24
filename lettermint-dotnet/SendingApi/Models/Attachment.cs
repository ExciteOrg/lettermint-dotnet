namespace lettermint_dotnet.SendingApi.Models;

public class Attachment
{
    public string Filename { get; set; }
    public string Content { get; set; }
    public string? Content_id { get; set; }
}
