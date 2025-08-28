using System.Text.Json.Serialization;

namespace DeveloperHub.Application.DTOs
{
	public record GitHubUserDto
	{
		[JsonPropertyName("id")]
		public long Id { get; init; }

		[JsonPropertyName("login")]
		public string Login { get; init; } = string.Empty;

		[JsonPropertyName("name")]
		public string? Name { get; init; }

		[JsonPropertyName("email")]
		public string? Email { get; init; }

		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; init; } = string.Empty;

		[JsonPropertyName("bio")]
		public string? Bio { get; init; }
	}
}
