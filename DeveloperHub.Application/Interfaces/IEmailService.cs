namespace DeveloperHub.Application.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(string to, string subject, string body);
		Task SendVerificationEmail(string to, string token);
	}
}
