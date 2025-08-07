using DeveloperHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperHub.Domain.Interfaces.Repositories
{
	public interface ITagRepository
	{
		Task<Tag?> GetByIdAsync(Guid id);
		Task<Tag?> GetByNameAsync(string name);
		Task<IEnumerable<Tag>> GetAllAsync();
		Task<IEnumerable<Tag>> GetByNamesAsync(IEnumerable<string> names);
		Task<Tag> AddAsync(Tag tag);
		Task<Tag> GetOrCreateAsync(string name);
		Task UpdateAsync(Tag tag);
		Task DeleteAsync(Tag tag);
		Task AddProjectTagAsync(ProjectTag projectTag);
		Task RemoveProjectTagAsync(ProjectTag projectTag);
		Task<bool> NameExistsAsync(string name);
	}
}
