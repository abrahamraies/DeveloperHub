namespace DeveloperHub.Domain.Entities;
public class Tag : BaseEntity
{
	public string Name { get; set; } = default!;
	public ICollection<ProjectTag> ProjectTags { get; set; } = [];
}
