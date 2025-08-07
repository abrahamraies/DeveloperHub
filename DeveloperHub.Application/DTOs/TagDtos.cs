namespace DeveloperHub.Application.DTOs
{
	public record CreateTagDto(string Name);
	public record TagDto(Guid Id, string Name);
}
