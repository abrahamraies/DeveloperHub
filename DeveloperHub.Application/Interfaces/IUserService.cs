using DeveloperHub.Application.DTOs;
using DeveloperHub.Domain.Enums;

namespace DeveloperHub.Application.Interfaces
{
	public interface IUserService
	{
		Task<UserDto> GetByIdAsync(Guid id);
		Task<PaginatedResult<UserDto>> GetAllAsync(int pageNumber, int pageSize);
		Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, Guid currentUserId);
		Task ChangeRoleAsync(Guid userId, UserRole newRole, Guid changedByUserId);
	}
}
