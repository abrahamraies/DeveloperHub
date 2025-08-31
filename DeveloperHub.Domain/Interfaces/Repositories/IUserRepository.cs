using DeveloperHub.Domain.Entities;

namespace DeveloperHub.Domain.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetByIdAsync(Guid id);
		Task<User?> GetByEmailAsync(string email);
		Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize);
		Task<int> GetTotalCountAsync();
		Task AddAsync(User user);
		Task UpdateAsync(User user);
		Task<bool> EmailExistsAsync(string email);
		Task<User?> GetByConfirmationTokenAsync(string token);
	}
}
