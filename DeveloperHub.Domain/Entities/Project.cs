using DeveloperHub.Domain.ValueObjects;

namespace DeveloperHub.Domain.Entities;
public class Project : BaseEntity
{
	public required string Title { get; set; }
	public required string Description { get; set; }
	public required ProjectUrl GitHubUrl { get; set; }
	public ProjectUrl? DiscordUrl { get; set; }
	public required Guid OwnerId { get; init; }
	public User Owner { get; init; } = default!;

	private readonly List<Comment> _comments = [];
	public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

	private readonly List<ProjectTag> _projectTags = [];
	public IReadOnlyCollection<ProjectTag> ProjectTags => _projectTags.AsReadOnly();

	public void Update(
		string title,
		string description,
		ProjectUrl gitHubUrl,
		ProjectUrl? discordUrl)
	{
		Title = title;
		Description = description;
		GitHubUrl = gitHubUrl;
		DiscordUrl = discordUrl;
	}

	public void AddTag(Tag tag)
	{
		if (tag == null) throw new ArgumentNullException(nameof(tag));
		_projectTags.Add(new ProjectTag { Project = this, Tag = tag });
	}
}