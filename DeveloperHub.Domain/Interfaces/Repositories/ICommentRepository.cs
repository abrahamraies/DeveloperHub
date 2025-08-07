using DeveloperHub.Domain.Entities;

namespace DeveloperHub.Domain.Interfaces.Repositories
{
	public interface ICommentRepository
	{
		Task<Comment?> GetByIdAsync(Guid id);
		Task<IEnumerable<Comment>> GetByProjectIdAsync(Guid projectId);
		Task<IEnumerable<Comment>> GetByUserIdAsync(Guid userId);
		Task AddAsync(Comment comment);
		Task DeleteAsync(Comment comment);
	}
}
