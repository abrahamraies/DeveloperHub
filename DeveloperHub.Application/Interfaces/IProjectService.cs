using DeveloperHub.Application.DTOs;

namespace DeveloperHub.Application.Interfaces
{
	public interface IProjectService
	{
		Task<ProjectDto?> GetByIdAsync(Guid id);
		Task<IEnumerable<ProjectSummaryDto>> GetByUserIdAsync(Guid userId);
		Task<IEnumerable<ProjectSummaryDto>> GetByTagAsync(string tagName);
		Task<PaginatedResult<ProjectSummaryDto>> GetPagedAsync(int pageNumber, int pageSize);
		Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid userId);
		Task UpdateAsync(Guid id, UpdateProjectDto dto, Guid userId);
		Task DeleteAsync(Guid id, Guid userId);
		Task<bool> IsOwnerAsync(Guid projectId, Guid userId);
	}
}
