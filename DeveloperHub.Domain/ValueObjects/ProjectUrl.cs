using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Exceptions;
using DeveloperHub.Domain.Guards;

namespace DeveloperHub.Domain.ValueObjects;
public class ProjectUrl
{
	public string Url { get; private set; } = default!;
	public UrlType Type { get; private set; }

	public ProjectUrl(string url, UrlType type)
	{
		Url = Guard.Against.NullOrWhiteSpace(url, nameof(url));
		Type = type;

		if (!IsValid(url, type))
			throw new DomainException($"Invalid {type} URL format");
	}

	private bool IsValid(string url, UrlType type) =>
		type switch
		{
			UrlType.GitHub => url.StartsWith("https://github.com/") &&
							  url.Split('/').Length >= 5,
			UrlType.Discord => url.StartsWith("https://discord.gg/"),
			_ => Uri.TryCreate(url, UriKind.Absolute, out _)
		};
}
