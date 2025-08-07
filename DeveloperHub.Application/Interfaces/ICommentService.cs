using DeveloperHub.Application.DTOs;

namespace DeveloperHub.Application.Interfaces
{
	public interface ICommentService
	{
		Task<CommentDto> GetCommentByIdAsync(Guid id);
		Task<IEnumerable<CommentDto>> GetByProjectIdAsync(Guid projectId);
		Task<IEnumerable<CommentDto>> GetByUserIdAsync(Guid userId);
		Task<CommentDto> CreateAsync(CreateCommentDto commentDto, Guid projectId, Guid userId);
		Task DeleteAsync(Guid id, Guid userId);
	}
}
