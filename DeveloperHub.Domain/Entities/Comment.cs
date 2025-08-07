using DeveloperHub.Domain.Guards;

namespace DeveloperHub.Domain.Entities;
public class Comment : BaseEntity
{
	private Comment() { }

	public Comment(string content, Guid userId, Guid projectId)
	{
		Content = Guard.Against.NullOrWhiteSpace(content, nameof(content));
		UserId = userId;
		ProjectId = projectId;
	}

	public string Content { get; private set; }

	public Guid UserId { get; private set; }
	public User User { get; private set; } = null!;

	public Guid ProjectId { get; private set; }
	public Project Project { get; private set; } = null!;
}
