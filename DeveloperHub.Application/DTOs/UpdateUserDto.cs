using DeveloperHub.Domain.Enums;

namespace DeveloperHub.Application.DTOs
{
	public record UpdateUserDto(
		string? Username,
		string? Email,
		string? GitHubUrl,
		string? DiscordUrl,
		string? ProfileImageUrl,
		UserRole? Role
	);
}
