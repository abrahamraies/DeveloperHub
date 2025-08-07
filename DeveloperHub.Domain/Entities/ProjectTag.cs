namespace DeveloperHub.Domain.Entities;
public class ProjectTag
{
	public Guid ProjectId { get; set; }
	public Project Project { get; set; } = default!;

	public Guid TagId { get; set; }
	public Tag Tag { get; set; } = default!;
}
