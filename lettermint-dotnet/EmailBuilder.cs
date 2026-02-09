using Lettermint.Models;
using lettermint_dotnet.Models;

namespace Lettermint;
public class EmailBuilder
{
    private readonly EmailRequest _request = new();
    private readonly ILettermintClient _client;
    private readonly IEmailWhitelistValidator _whitelistValidator;

    public EmailBuilder(ILettermintClient client, IEmailWhitelistValidator whitelistValidator)
    {
        _client = client;
        _whitelistValidator = whitelistValidator;
    }

    /// <summary>
    /// Sets the sender's email address.
    /// </summary>
    /// <param name="fromEmail">The sender email address (e.g., "john@example.com").</param>
    public EmailBuilder From(string fromEmail)
    {
        _request.From = fromEmail;
        return this;
    }

    /// <summary>
    /// Sets the sender's name and email address.
    /// </summary>
    /// <param name="fromName">The sender's name.</param>
    /// <param name="fromEmail">The sender's email.</param>
    public EmailBuilder From(string fromName, string fromEmail)
    {
        _request.From = $"{fromName} <{fromEmail}>";
        return this;
    }

    /// <summary>
    /// Adds a recipient email address.
    /// </summary>
    /// <param name="toEmail">The recipient's email address (e.g., "alice@example.com").</param>
    public EmailBuilder To(string toEmail)
    {
        var validatedEmail = _whitelistValidator.ValidateAndFilter(toEmail);
        _request.To.Add(validatedEmail);
        return this;
    }

    /// <summary>
    /// Adds a recipient with name and email address.
    /// </summary>
    /// <param name="toName">The recipient's name.</param>
    /// <param name="toEmail">The recipient's email.</param>
    public EmailBuilder To(string toName, string toEmail)
    {
        var validatedEmail = _whitelistValidator.ValidateAndFilter(toEmail);
        _request.To.Add($"{toName} <{validatedEmail}>");
        return this;
    }

    /// <summary>
    /// Adds a CC recipient email address.
    /// </summary>
    /// <param name="ccEmail">The recipient's email address (e.g., "manager@example.com").</param>
    public EmailBuilder Cc(string ccEmail)
    {
        _request.Cc ??= [];
        var validatedEmail = _whitelistValidator.ValidateAndFilter(ccEmail);
        _request.Cc.Add(validatedEmail);
        return this;
    }

    /// <summary>
    /// Adds a CC recipient with name and email address.
    /// </summary>
    /// <param name="ccName">The recipient's name.</param>
    /// <param name="ccEmail">The recipient's email.</param>
    public EmailBuilder Cc(string ccName, string ccEmail)
    {
        _request.Cc ??= [];
        var validatedEmail = _whitelistValidator.ValidateAndFilter(ccEmail);
        _request.Cc.Add($"{ccName} <{validatedEmail}>");
        return this;
    }

    /// <summary>
    /// Adds a BCC recipient email address.
    /// </summary>
    /// <param name="bccEmail">The recipient's email address (e.g., "admin@example.com").</param>
    public EmailBuilder Bcc(string bccEmail)
    {
        _request.Bcc ??= [];
        var validatedEmail = _whitelistValidator.ValidateAndFilter(bccEmail);
        _request.Bcc.Add(validatedEmail);
        return this;
    }

    /// <summary>
    /// Adds a BCC recipient with name and email address.
    /// </summary>
    /// <param name="bccName">The recipient's name.</param>
    /// <param name="bccEmail">The recipient's email.</param>
    public EmailBuilder Bcc(string bccName, string bccEmail)
    {
        _request.Bcc ??= [];
        var validatedEmail = _whitelistValidator.ValidateAndFilter(bccEmail);
        _request.Bcc.Add($"{bccName} <{validatedEmail}>");
        return this;
    }
    /// <summary>
    /// Sets a tag of the email
    /// </summary>
    public EmailBuilder Tag(string? tag)
    {
        if(!string.IsNullOrEmpty(tag))

            _request.Tag = tag;
        return this;
    }
    /// <summary>
    /// Sets an idempotencykey to avoid sending multiple of the same emails, 24 hour window of keys.
    /// </summary>
    public EmailBuilder IdempotencyKey(string idemppotencyKey)
    {
        _request.IdempotencyKey = idemppotencyKey;
        return this;
    }
    /// <summary>
    /// Sets the subject
    /// </summary>
    public EmailBuilder Subject(string subject)
    {
        _request.Subject = subject;
        return this;
    }
    /// <summary>
    /// Sets the text of the email. Only one of html and text can be set
    /// </summary>
    public EmailBuilder Text(string text)
    {
        _request.Text = text;
        return this;
    }
    /// <summary>
    /// Sets the html of the email. Only one of html and text can be set
    /// </summary>
    public EmailBuilder Html(string html)
    {
        _request.Html = html;
        return this;
    }
    /// <summary>
    /// Sets the mail route as Outgoing (Transactional)
    /// </summary>
    public EmailBuilder SetAsOutgoing()
    {
        _request.Route = "outgoing";
        return this;
    }
    /// <summary>
    /// Sets the mail route as broadcast (Marketing)
    /// </summary>
    public EmailBuilder SetAsBroadcast()
    {
        _request.Route = "broadcast";
        return this;
    }
    /// <summary>
    /// Set an attachment. Content is the base64 value of the file. Content_id can be null and is used for inline images
    /// </summary>
    public EmailBuilder AddAttachment(string fileName, string content, string? content_id)
    {
        if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(content))
        {
            _request.Attachments.Add(new Attachment { Filename = fileName, Content = content, Content_id = content_id });
        }
        return this;
    }
    /// <summary>
    /// Builds and returns the email request without sending it.
    /// Use this when you want to send multiple emails in batch.
    /// </summary>
    public EmailRequest Build()
    {
        return _request;
    }
    /// <summary>
    /// Send the email
    /// </summary>
    public async Task<EmailResponse> SendAsync(CancellationToken cancellationToken = default)
    {
        return await _client.SendEmailAsync(_request, cancellationToken);
    }
}
