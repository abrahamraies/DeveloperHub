using DeveloperHub.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DeveloperHub.Infrastructure.Services
{
	public class SendGridEmailService : IEmailService
	{
		private readonly string _apiKey;
		private readonly string _fromEmail;
		private readonly string _fromName;

		public SendGridEmailService(string apiKey, string fromEmail, string fromName)
		{
			_apiKey = apiKey;
			_fromEmail = fromEmail;
			_fromName = fromName;
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var client = new SendGridClient(_apiKey);
			var from = new EmailAddress(_fromEmail, _fromName);
			var toEmail = new EmailAddress(to);

			var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);
			await client.SendEmailAsync(msg);
		}
	}
}
