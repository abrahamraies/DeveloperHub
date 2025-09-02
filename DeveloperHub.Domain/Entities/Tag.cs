namespace DeveloperHub.Domain.Entities;
public class Tag : BaseEntity
{
	public required string Name { get; set; }
	public ICollection<ProjectTag> ProjectTags { get; init; } = [];
}
