namespace DeveloperHub.Application.DTOs
{
	public record RegisterDto(string Username, string Email, string Password);
	public record LoginDto(string Email, string Password);
	public record AuthResponseDto(Guid Id, string Token);
	public record ForgotPasswordDto(string Email);
	public record ResetPasswordDto(string Token, string NewPassword);
	public record ChangePasswordDto(string CurrentPassword, string NewPassword);
	public record ResendVerificationDto(string Email);
}
