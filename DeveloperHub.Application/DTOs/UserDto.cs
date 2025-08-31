namespace DeveloperHub.Application.DTOs
{
	public record UserDto(
		Guid Id,
		string Username,
		string Email,
		bool EmailVerified,
		string? GitHubUrl,
		string? DiscordUrl,
		string? ProfileImageUrl,
		string Role
	);
}
