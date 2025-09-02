using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Exceptions;

namespace DeveloperHub.Domain.ValueObjects;
public record ProjectUrl
{
	public string Url { get; init; }
	public UrlType Type { get; init; }

	public ProjectUrl(string url, UrlType type)
	{
		if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
			throw new DomainException("The provided URL is not valid.");

		if (!IsValid(url, type))
			throw new DomainException($"Invalid {type} URL format");

		Url = url;
		Type = type;
	}

	private static bool IsValid(string url, UrlType type) =>
		type switch
		{
			UrlType.GitHub => url.StartsWith("https://github.com/") && url.Split('/').Length >= 5,
			UrlType.Discord => url.StartsWith("https://discord.gg/"),
			_ => true
		};
}