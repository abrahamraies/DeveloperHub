using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeveloperHub.Infrastructure.Repositories
{
	public class UserRepository(DeveloperHubDbContext context) : IUserRepository
	{
		private readonly DeveloperHubDbContext _context = context;

		public async Task<User?> GetByIdAsync(Guid id) =>
			await _context.Users.FindAsync(id);

		public async Task<User?> GetByEmailAsync(string email) =>
			await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

		public async Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize) =>
			await _context.Users
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

		public async Task<int> GetTotalCountAsync() =>
			await _context.Users.CountAsync();

		public async Task AddAsync(User user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _context.Users.AnyAsync(u => u.Email.ToLower() == email);
		}

		public async Task<User?> GetByConfirmationTokenAsync(string token)
		{
			return await _context.Users
				.FirstOrDefaultAsync(u => u.VerificationToken == token);
		}
	}
}
