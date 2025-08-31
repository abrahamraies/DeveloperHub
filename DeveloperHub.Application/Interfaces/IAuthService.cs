using DeveloperHub.Application.DTOs;

namespace DeveloperHub.Application.Interfaces
{
	public interface IAuthService
	{
		Task RegisterAsync(RegisterDto dto);
		Task<AuthResponseDto> LoginAsync(LoginDto dto);
		Task<UserDto> GetCurrentUserAsync(Guid userId);
		Task<bool> ForgotPasswordAsync(string email);
		Task<bool> ResetPasswordAsync(string token, string newPassword);
		Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
		Task<bool> ConfirmEmailAsync(string token);
		Task<bool> ResendVerificationEmailAsync(string email);
	}
}
