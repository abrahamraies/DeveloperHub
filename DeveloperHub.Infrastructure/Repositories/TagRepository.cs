using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeveloperHub.Infrastructure.Repositories
{
	public class TagRepository(DeveloperHubDbContext context) : ITagRepository
	{
		private readonly DeveloperHubDbContext _context = context;

		public async Task<Tag?> GetByIdAsync(Guid id)
		{
			return await _context.Tags.FindAsync(id);
		}

		public async Task<Tag?> GetByNameAsync(string name)
		{
			return await _context.Tags
				.FirstOrDefaultAsync(t => t.Name.ToLower() == name);
		}

		public async Task<IEnumerable<Tag>> GetAllAsync()
		{
			return await _context.Tags.ToListAsync();
		}

		public async Task<IEnumerable<Tag>> GetByNamesAsync(IEnumerable<string> names)
		{
			return await _context.Tags
				.Where(t => names.Contains(t.Name))
				.ToListAsync();
		}

		public async Task<Tag> AddAsync(Tag tag)
		{
			_context.Tags.Add(tag);
			await _context.SaveChangesAsync();
			return tag;
		}

		public async Task<Tag> GetOrCreateAsync(string name)
		{
			var tag = await GetByNameAsync(name);
			if (tag != null)
				return tag;

			var newTag = new Tag { Name = name };
			return await AddAsync(newTag);
		}

		public async Task UpdateAsync(Tag tag)
		{
			_context.Tags.Update(tag);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Tag tag)
		{
			_context.Tags.Remove(tag);
			await _context.SaveChangesAsync();
		}

		public async Task AddProjectTagAsync(ProjectTag projectTag)
		{
			_context.ProjectTags.Add(projectTag);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveProjectTagAsync(ProjectTag projectTag)
		{
			_context.ProjectTags.Remove(projectTag);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> NameExistsAsync(string name)
		{
			return await _context.Tags.AnyAsync(t => t.Name == name);
		}
	}
}
