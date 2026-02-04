using Lettermint.Models;


namespace Lettermint;

public interface ILettermintClient
{
    EmailBuilder Email { get; }
    Task<EmailResponse> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);
    Task<List<EmailResponse>> SendEmailsBatchAsync(List<EmailRequest> requests, CancellationToken cancellationToken = default);
}
