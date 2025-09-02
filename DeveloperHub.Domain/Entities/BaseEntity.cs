namespace DeveloperHub.Domain.Entities;

public abstract class BaseEntity
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; private set; }

	public void SetUpdatedNow()
	{
		UpdatedAt = DateTime.UtcNow;
	}
}