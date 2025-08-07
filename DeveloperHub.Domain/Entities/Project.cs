using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Exceptions;
using DeveloperHub.Domain.Guards;
using DeveloperHub.Domain.ValueObjects;

namespace DeveloperHub.Domain.Entities;
public class Project : BaseEntity
{
	private Project() { }

	public Project(
		string title,
		string description,
		string gitHubUrl,
		string? discordUrl,
		Guid ownerId)
	{
		Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
		Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
		GitHubUrl = new ProjectUrl(gitHubUrl, UrlType.GitHub);
		DiscordUrl = discordUrl != null ? new ProjectUrl(discordUrl, UrlType.Discord) : null;
		OwnerId = ownerId;
	}

	public string Title { get; private set; }
	public string Description { get; private set; }
	public ProjectUrl GitHubUrl { get; private set; }
	public ProjectUrl? DiscordUrl { get; private set; }
	public Guid OwnerId { get; private set; }
	public User Owner { get; private set; } = default!;

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
		Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
		Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
		GitHubUrl = gitHubUrl;
		DiscordUrl = discordUrl;
	}

	public void AddTag(Tag tag)
	{
		if (tag == null) throw new ArgumentNullException(nameof(tag));
		_projectTags.Add(new ProjectTag { Project = this, Tag = tag });
	}
}