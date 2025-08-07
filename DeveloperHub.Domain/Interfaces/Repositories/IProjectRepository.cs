using DeveloperHub.Domain.Entities;

namespace DeveloperHub.Domain.Interfaces.Repositories
{
	public interface IProjectRepository
	{
		Task<Project?> GetByIdAsync(Guid id);
		Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId);
		Task<IEnumerable<Project>> GetByTagAsync(string tagName);
		Task<IEnumerable<Project>> GetPagedListAsync(int pageNumber, int pageSize);
		Task AddAsync(Project project);
		Task DeleteAsync(Project project);
		Task UpdateAsync(Project project);
		Task<bool> ExistsAsync(Guid id);
		Task<bool> IsOwnerAsync(Guid projectId, Guid userId);
		Task<int> GetTotalCountAsync();
	}
}
