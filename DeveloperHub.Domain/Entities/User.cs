using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Exceptions;
using DeveloperHub.Domain.Guards;

namespace DeveloperHub.Domain.Entities;
public class User : BaseEntity
{
	private User() { }
	public User(string username, string email, string passwordHash, UserRole role)
	{
		Username = username;
		Email = email;
		PasswordHash = passwordHash;
		Role = role;
	}

	private string _username = string.Empty;
	public string Username
	{
		get => _username;
		private set => _username = Guard.Against.NullOrWhiteSpace(value, nameof(Username));
	}

	private string _email = string.Empty;
	public string Email
	{
		get => _email;
		private set => _email = Guard.Against.NullOrWhiteSpace(value, nameof(Email));
	}

	private string _passwordHash = string.Empty;
	public string PasswordHash
	{
		get => _passwordHash;
		private set => _passwordHash = Guard.Against.NullOrWhiteSpace(value, nameof(PasswordHash));
	}

	public string? GitHubUrl { get; private set; }
	public string? DiscordUrl { get; private set; }

	public UserRole Role { get; private set; } = UserRole.User;

	private readonly List<Project> _projects = [];
	public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

	private readonly List<Comment> _comments = [];
	public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

	public void ChangeRole(UserRole newRole, User changedBy)
	{
		if (changedBy.Role != UserRole.Admin)
			throw new DomainException("Only admins can change user roles.");

		Role = newRole;
		SetUpdatedNow();
	}

	public void ChangeUsername(string newUsername)
	{
		Username = Guard.Against.NullOrWhiteSpace(newUsername, nameof(newUsername));
	}

	public void ChangeEmail(string newEmail)
	{
		Email = Guard.Against.NullOrWhiteSpace(newEmail, nameof(newEmail));
	}

	public void ChangeGitHubUrl(string? newGitHubUrl)
	{
		if (newGitHubUrl is not null)
			newGitHubUrl = newGitHubUrl.Trim();

		GitHubUrl = string.IsNullOrWhiteSpace(newGitHubUrl) ? null : newGitHubUrl;
	}

	public void ChangeDiscordUrl(string? newDiscordUrl)
	{
		if (newDiscordUrl is not null)
			newDiscordUrl = newDiscordUrl.Trim();

		DiscordUrl = string.IsNullOrWhiteSpace(newDiscordUrl) ? null : newDiscordUrl;
	}
}