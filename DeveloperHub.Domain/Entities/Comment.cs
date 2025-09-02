namespace DeveloperHub.Domain.Entities;
public class Comment : BaseEntity
{
	public required string Content { get; set; }

	public required Guid UserId { get; init; }
	public User User { get; init; } = null!;

	public required Guid ProjectId { get; init; }
	public Project Project { get; init; } = null!;
}