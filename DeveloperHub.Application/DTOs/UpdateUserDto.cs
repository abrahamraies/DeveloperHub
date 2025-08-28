using DeveloperHub.Domain.Enums;

namespace DeveloperHub.Application.DTOs
{
	public record UpdateUserDto(
		string? Username = null,
		string? Email = null,
		string? GitHubUrl = null,
		string? DiscordUrl = null,
		string? ProfileImageUrl = null,
		UserRole? Role = UserRole.User
	);
}
