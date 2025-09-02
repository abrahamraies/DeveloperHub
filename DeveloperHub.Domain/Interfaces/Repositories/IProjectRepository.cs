using DeveloperHub.Domain.Entities;

namespace DeveloperHub.Domain.Interfaces.Repositories
{
	public interface IProjectRepository
	{
		Task<Project?> GetByIdAsync(Guid id);
		Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize);
		Task<IEnumerable<Project>> GetByTagAsync(string tagName, int pageNumber, int pageSize);
		Task<IEnumerable<Project>> GetPagedListAsync(
			int pageNumber,
			int pageSize,
			string? search = null,
			List<string> tags = null!);

		Task<int> GetTotalCountAsync(string? search = null, List<string> tags = null!);
		Task AddAsync(Project project);
		Task DeleteAsync(Project project);
		Task UpdateAsync(Project project);
		Task<bool> ExistsAsync(Guid id);
		Task<bool> IsOwnerAsync(Guid projectId, Guid userId);
		Task<int> GetUserTotalCountAsync(Guid userId);
		Task<Project?> GetByGitHubUrlAsync(string githubUrl);
	}
}