using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Exceptions;

namespace DeveloperHub.Domain.Entities;
public class User : BaseEntity
{
	public required string Username { get; set; }
	public required string Email { get; set; }
	public required string PasswordHash { get; set; }

	public string? GitHubUrl { get; set; }
	public string? DiscordUrl { get; set; }
	public string? ProfileImageUrl { get; set; }
	public string? PasswordResetToken { get; set; }
	public DateTime? PasswordResetTokenExpiry { get; set; }
	public bool EmailVerified { get; set; } = false;
	public string? VerificationToken { get; set; }
	public DateTime? VerificationTokenExpiry { get; set; }

	public UserRole Role { get; set; } = UserRole.User;

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
		Username = newUsername;
	}

	public void ChangeEmail(string newEmail)
	{
		Email = newEmail;
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

	public void ChangeProfileImageUrl(string? url)
	{
		if (url is not null)
			url = url.Trim();

		ProfileImageUrl = string.IsNullOrWhiteSpace(url) ? null : url;
	}

	public void SetPasswordResetToken(string token, DateTime expiry)
	{
		PasswordResetToken = token;
		PasswordResetTokenExpiry = expiry;
	}

	public void ClearPasswordResetToken()
	{
		PasswordResetToken = null;
		PasswordResetTokenExpiry = null;
	}

	public void UpdatePassword(string hashedPassword)
	{
		PasswordHash = hashedPassword;
		ClearPasswordResetToken();
	}

	public void SetEmailVerified()
	{
		EmailVerified = true;
		VerificationToken = null;
		VerificationTokenExpiry = null;
	}

	public void SetVerificationToken(string token, DateTime expiry)
	{
		VerificationToken = token;
		VerificationTokenExpiry = expiry;
	}
}