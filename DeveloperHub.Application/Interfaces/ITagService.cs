using DeveloperHub.Application.DTOs;

namespace DeveloperHub.Application.Interfaces
{
	public interface ITagService
	{
		Task<TagDto> GetByIdAsync(Guid id);
		Task<TagDto> GetByNameAsync(string name);
		Task<IEnumerable<TagDto>> GetAllAsync();
		Task<TagDto> CreateAsync(CreateTagDto dto);
		Task<TagDto> UpdateAsync(Guid id, CreateTagDto dto);
		Task DeleteAsync(Guid id);
	}
}
