namespace DeveloperHub.Application.DTOs
{
	public record CreateUserDto(
		string Username,
		string Email,
		string Password
	);
}
