using System.Text.Json.Serialization;

namespace DeveloperHub.Application.DTOs
{
	public record GitHubRepoDto
	{
		[JsonPropertyName("id")]
		public long Id { get; init; }

		[JsonPropertyName("name")]
		public string Name { get; init; } = string.Empty;

		[JsonPropertyName("full_name")]
		public string FullName { get; init; } = string.Empty;

		[JsonPropertyName("description")]
		public string? Description { get; init; }

		[JsonPropertyName("html_url")]
		public string HtmlUrl { get; init; } = string.Empty;

		[JsonPropertyName("language")]
		public string? Language { get; init; }

		[JsonPropertyName("topics")]
		public List<string> Topics { get; init; } = [];

		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; init; }

		[JsonPropertyName("updated_at")]
		public DateTime UpdatedAt { get; init; }

		[JsonPropertyName("stargazers_count")]
		public int Stars { get; init; }

		[JsonPropertyName("forks_count")]
		public int Forks { get; init; }
	}
}
