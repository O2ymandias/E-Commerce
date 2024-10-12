using ECommerce.Core.Entities.Helpers;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Service.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ECommerce.Service
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _emailSettings;
		private readonly ILogger<EmailSender> _logger;

		public EmailSender(IOptions<EmailSettings> emailSettingsOptions,
			ILogger<EmailSender> logger)
		{
			_emailSettings = emailSettingsOptions.Value;
			_logger = logger;
		}
		public async Task SendEmailAsync(Email email)
		{
			var mimeMessage = await CreateMimeMessageAsync(email);

			using var client = new SmtpClient();
			try
			{
				await ConnectAndAuthenticateAsync(client);
				await client.SendAsync(mimeMessage);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An Error Occurred Trying To Send Email");
			}
			finally
			{
				await client.DisconnectAsync(true);
			}
		}


		private async Task<MimeMessage> CreateMimeMessageAsync(Email email)
		{
			var mimeMessage = new MimeMessage()
			{
				Sender = MailboxAddress.Parse(_emailSettings.SenderEmail),
				Subject = email.Subject,
			};

			mimeMessage.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.SenderEmail));
			mimeMessage.To.Add(MailboxAddress.Parse(email.To));

			var body = new BodyBuilder() { TextBody = email.Body };

			if (email.Attachments?.Count > 0)
			{
				byte[] buffer;

				foreach (var file in email.Attachments)
				{
					if (file.Length > 0)
					{
						using var stream = new MemoryStream();
						await file.CopyToAsync(stream);
						buffer = stream.ToArray();
						body.Attachments.Add(file.FileName, buffer);
					}
				}
			}
			mimeMessage.Body = body.ToMessageBody();
			return mimeMessage;
		}
		private async Task ConnectAndAuthenticateAsync(SmtpClient client)
		{
			await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
		}
	}
}
