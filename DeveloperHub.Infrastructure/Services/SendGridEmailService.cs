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

		private async Task SendAsync(string to, string subject, string plainText, string htmlContent)
		{
			var client = new SendGridClient(_apiKey);
			var from = new EmailAddress(_fromEmail, _fromName);
			var toEmail = new EmailAddress(to);

			var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainText, htmlContent);
			await client.SendEmailAsync(msg);
		}

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			await SendAsync(to, subject, body, body);
		}

		public async Task SendVerificationEmail(string to, string token)
		{
			var verificationUrl = $"http://localhost:5173/auth/verify-email?token={token}";

			var plainText =
				$"Hola,\n\n" +
				$"Gracias por registrarte en DeveloperHub 🚀.\n\n" +
				$"Por favor confirma tu correo haciendo clic en el siguiente enlace:\n{verificationUrl}\n\n" +
				$"Si no creaste esta cuenta, puedes ignorar este mensaje.";

			var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; color: #333; padding: 20px;'>
                    <h2 style='color: #4F46E5;'>Bienvenido a DeveloperHub 🚀</h2>
                    <p>Gracias por registrarte en nuestra plataforma.</p>
                    <p>Para activar tu cuenta y comenzar a colaborar con otros desarrolladores, confirma tu correo electrónico haciendo clic en el botón de abajo:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{verificationUrl}' 
                           style='background: linear-gradient(90deg,#2563eb,#7c3aed);
                                  color: #fff; 
                                  text-decoration: none; 
                                  padding: 12px 24px; 
                                  border-radius: 8px;
                                  font-weight: bold;'>
                            Verificar mi correo
                        </a>
                    </p>
                    <p>Si no creaste esta cuenta, simplemente ignora este mensaje.</p>
                    <hr style='margin-top: 30px;' />
                    <p style='font-size: 12px; color: #888;'>© {DateTime.UtcNow.Year} DeveloperHub. Todos los derechos reservados.</p>
                </div>";

			await SendAsync(to, "Confirma tu cuenta en DeveloperHub", plainText, htmlContent);
		}
	}
}
