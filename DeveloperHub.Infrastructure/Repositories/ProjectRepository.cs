using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeveloperHub.Infrastructure.Repositories;

public class ProjectRepository(DeveloperHubDbContext context) : IProjectRepository
{
	private readonly DeveloperHubDbContext _context = context;

	public async Task<Project?> GetByIdAsync(Guid id)
	{
		return await _context.Projects
			.Include(p => p.Owner)
			.Include(p => p.ProjectTags)
				.ThenInclude(pt => pt.Tag)
			.Include(p => p.Comments)
				.ThenInclude(c => c.User)
			.FirstOrDefaultAsync(p => p.Id == id);
	}

	public async Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize)
	{
		return await _context.Projects
			.AsNoTracking()
			.Include(p => p.Owner)
				.Include(p => p.Comments)
			.Include(p => p.ProjectTags)
				.ThenInclude(pt => pt.Tag)
			.Where(p => p.OwnerId == userId)
			.OrderByDescending(p => p.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<IEnumerable<Project>> GetByTagAsync(string tagName, int pageNumber, int pageSize)
	{
		return await _context.Projects
			.AsNoTracking()
			.Include(p => p.Owner)
			.Include(p => p.ProjectTags)
				.ThenInclude(pt => pt.Tag)
			.Where(p => p.ProjectTags.Any(pt => pt.Tag.Name == tagName))
			.OrderByDescending(p => p.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<IEnumerable<Project>> GetPagedListAsync(
		int pageNumber,
		int pageSize,
		string? search = null,
		List<string> tags = null!)
	{
		var query = _context.Projects
			.AsNoTracking()
			.Include(p => p.Owner)
				.Include(p => p.Comments)
			.Include(p => p.ProjectTags)
				.ThenInclude(pt => pt.Tag)
			.AsQueryable();

		query = ApplyFilters(query, search, tags);

		return await query
			.OrderByDescending(p => p.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<int> GetTotalCountAsync(string? search = null, List<string> tags = null!)
	{
		var query = _context.Projects.AsQueryable();
		query = ApplyFilters(query, search, tags);
		return await query.CountAsync();
	}

	public async Task AddAsync(Project project)
	{
		_context.Projects.Add(project);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Project project)
	{
		_context.Projects.Remove(project);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(Project project)
	{
		_context.Projects.Update(project);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> ExistsAsync(Guid id)
	{
		return await _context.Projects.AnyAsync(p => p.Id == id);
	}

	public async Task<bool> IsOwnerAsync(Guid projectId, Guid userId)
	{
		return await _context.Projects
			.AnyAsync(p => p.Id == projectId && p.OwnerId == userId);
	}

	public async Task<int> GetUserTotalCountAsync(Guid userId)
	{
		return await _context.Projects.CountAsync(p => p.OwnerId == userId);
	}

	public async Task<Project?> GetByGitHubUrlAsync(string githubUrl)
	{
		return await _context.Projects.FirstOrDefaultAsync(p => p.GitHubUrl.Url == githubUrl);
	}

	private IQueryable<Project> ApplyFilters(IQueryable<Project> query, string? search, List<string>? tags)
	{
		if (!string.IsNullOrWhiteSpace(search))
		{
			var searchTerm = search.Trim();
			query = query.Where(p =>
				p.Title.Contains(searchTerm) ||
				p.Description.Contains(searchTerm));
		}

		if (tags != null && tags.Any())
		{
			var upperTags = tags.Select(t => t.ToUpper()).ToList();
			query = query.Where(p => p.ProjectTags.Any(pt => upperTags.Contains(pt.Tag.Name.ToUpper())));
		}

		return query;
	}
}