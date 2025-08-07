using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeveloperHub.Infrastructure.Repositories
{
	public class CommentRepository(DeveloperHubDbContext context) : ICommentRepository
	{
		private readonly DeveloperHubDbContext _context = context;

		public async Task<Comment?> GetByIdAsync(Guid id)
		{
			return await _context.Comments
				.Include(c => c.User)
				.Include(c => c.Project)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<IEnumerable<Comment>> GetByProjectIdAsync(Guid projectId)
		{
			return await _context.Comments
				.Include(c => c.User)
				.Where(c => c.ProjectId == projectId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();
		}

		public async Task<IEnumerable<Comment>> GetByUserIdAsync(Guid userId)
		{
			return await _context.Comments
				.Include(c => c.Project)
				.Include(u => u.User)
				.Where(c => c.UserId == userId)
				.ToListAsync();
		}

		public async Task AddAsync(Comment comment)
		{
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Comment comment)
		{
			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();
		}
	}
}
