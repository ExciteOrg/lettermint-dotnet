using lettermint_dotnet.Models;


namespace lettermint_dotnet;

public interface ILettermintClient
{
    EmailBuilder Email { get; }
    Task<EmailResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);
}
