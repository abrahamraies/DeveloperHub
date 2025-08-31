using DeveloperHub.Domain.ValueObjects;

namespace DeveloperHub.Application.DTOs
{
	public record CreateProjectDto(string Title, string Description, string GitHubUrl, string? DiscordUrl, List<string> Tags);

	public record class UpdateProjectDto
	{
		public string? Title { get; init; } = default!;
		public string? Description { get; init; } = default!;
		public string? GitHubUrl { get; init; } = default!;
		public string? DiscordUrl { get; init; }
		public List<string>? Tags { get; init; } = [];
	}

	public record class ProjectDto
	{
		public Guid Id { get; init; }
		public string Title { get; init; } = string.Empty;
		public string Description { get; init; } = string.Empty;
		public string GitHubUrl { get; init; } = string.Empty;
		public string? DiscordUrl { get; init; }
		public Guid OwnerId { get; init; }
		public string OwnerUsername { get; init; } = string.Empty;
		public DateTime CreatedAt { get; init; }
		public List<string> Tags { get; init; } = [];
		public List<CommentDto> Comments { get; init; } = [];
	}

	public record class ProjectSummaryDto
	{
		public Guid Id { get; init; }
		public string Title { get; init; } = string.Empty;
		public string Description { get; init; } = string.Empty;
		public string OwnerId { get; init; } = string.Empty;
		public string OwnerUsername { get; init; } = string.Empty;
		public string OwnerProfileImageUrl { get; init; } = string.Empty;
		public DateTime CreatedAt { get; init; }
		public int CommentCount { get; init; }
		public List<string> Tags { get; init; } = [];
	}
}
