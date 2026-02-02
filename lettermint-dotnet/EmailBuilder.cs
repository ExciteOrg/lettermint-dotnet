using Lettermint.Models;
using lettermint_dotnet.Models;

namespace Lettermint;
public class EmailBuilder
{
    private readonly EmailRequest _request = new();
    private readonly ILettermintClient _client;

    public EmailBuilder(ILettermintClient client)
    {
        _client = client;
    }
    /// <summary>
    /// Sets the sender's name and/or email address.
    /// </summary>
    /// <param name="from">The sender email address. Can also include display name like "John Doe &lt;john@example.com&gt;".</param>
    public EmailBuilder From(string from)
    {
        _request.From = from;
        return this;
    }
    /// <summary>
    /// Sets the sender's name and email address.
    /// </summary>
    /// <param name="fromName">The senders name.</param>
    /// <param name="fromEmail">The senders email.</param>
    public EmailBuilder From(string fromName, string fromEmail)
    {
        _request.From = $"{fromName} <{fromEmail}>";
        return this;
    }
    /// <summary>
    /// Sets the receivers name and/or email address.
    /// </summary>
    /// <param name="to">The receivers email address. Can also include display name like "John Doe &lt;john@example.com&gt;".</param>
    public EmailBuilder To(string to)
    {
        _request.To.Add(to);
        return this;
    }
    /// <summary>
    /// Sets the receivers name and email address.
    /// </summary>
    /// <param name="toName">The senders name.</param>
    /// <param name="toEmail">The senders email.</param>
    public EmailBuilder To(string toName, string toEmail)
    {
        _request.To.Add($"{toName} <{toEmail}>");
        return this;
    }
    /// <summary>
    /// Sets the cc receivers name and/or email address.
    /// </summary>
    /// <param name="cc">The receivers email address. Can also include display name like "John Doe &lt;john@example.com&gt;".</param>
    public EmailBuilder Cc(string cc)
    {
        _request.Cc ??= [];
        _request.Cc.Add(cc);
        return this;
    }
    /// <summary>
    /// Sets the bcc receivers name and/or email address.
    /// </summary>
    /// <param name="bcc">The receivers email address. Can also include display name like "John Doe &lt;john@example.com&gt;".</param>
    public EmailBuilder Bcc(string bcc)
    {
        _request.Bcc ??= [];
        _request.Bcc.Add(bcc);
        return this;
    }
    /// <summary>
    /// Sets a tag of the email
    /// </summary>
    public EmailBuilder Tag(string tag)
    {
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
    /// Set an attachment. Content_id can be null and is used for inline images
    /// </summary>
    public EmailBuilder AddAttachment(string fileName, string content, string? content_id)
    {
        _request.Attachments.Add(new Attachment { Filename = fileName, Content = content, Content_id = content_id });
        return this;
    }
    /// <summary>
    /// Send the email
    /// </summary>
    public async Task<EmailResponse> SendAsync(CancellationToken cancellationToken = default)
    {
        return await _client.SendEmailAsync(_request, cancellationToken);
    }
}
