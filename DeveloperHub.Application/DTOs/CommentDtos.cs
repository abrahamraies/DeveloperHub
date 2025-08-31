namespace DeveloperHub.Application.DTOs
{
	public record CreateCommentDto(string Content);

	public record class CommentDto
	{
		public Guid Id { get; init; }
		public string Content { get; init; } = string.Empty;
		public Guid ProjectId { get; init; }
		public Guid UserId { get; init; }
		public string Username { get; init; } = string.Empty;
		public DateTime CreatedAt { get; init; }
	}
}
