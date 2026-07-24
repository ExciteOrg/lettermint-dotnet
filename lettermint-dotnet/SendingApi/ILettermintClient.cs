using lettermint_dotnet.SendingApi.Models;


namespace lettermint_dotnet.SendingApi;

public interface ILettermintClient
{
    EmailBuilder Email { get; }
    Task<EmailResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);
    Task<List<EmailResponse>> SendEmailsBatchAsync(List<EmailRequest> requests, CancellationToken cancellationToken = default);
}
