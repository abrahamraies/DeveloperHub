using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IValidator<UpdateUserDto> _updateValidator;
		private readonly IMapper _mapper;

		public UserService
		(
			IUserRepository userRepository,
			IValidator<UpdateUserDto> updateValidator,
			IMapper mapper
		)
		{
			_userRepository = userRepository;
			_updateValidator = updateValidator;
			_mapper = mapper;
		}

		public async Task<UserDto> GetByIdAsync(Guid id)
		{
			var user = await _userRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"User with ID {id} not found");

			return _mapper.Map<UserDto>(user);
		}

		public async Task<PaginatedResult<UserDto>> GetAllAsync(int pageNumber, int pageSize)
		{
			var users = await _userRepository.GetAllAsync(pageNumber, pageSize);
			var totalCount = await _userRepository.GetTotalCountAsync();

			var userDtos = _mapper.Map<List<UserDto>>(users);
			return new PaginatedResult<UserDto>(userDtos, totalCount, pageNumber, pageSize);
		}

		public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, Guid currentUserId)
		{
			var user = await _userRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"User with ID {id} not found");

			if (id != currentUserId)
			{
				var currentUser = await _userRepository.GetByIdAsync(currentUserId)
					?? throw new KeyNotFoundException($"Current user with ID {currentUserId} not found");

				if (currentUser.Role != UserRole.Admin)
					throw new UnauthorizedAccessException("You don't have permission to update this user");
			}

			if(!string.IsNullOrWhiteSpace(updateUserDto.Email) && !string.IsNullOrWhiteSpace(updateUserDto.Username))
				await _updateValidator.ValidateAndThrowAsync(updateUserDto);

			if (updateUserDto.Username is not null)
				user.ChangeUsername(updateUserDto.Username);

			if (updateUserDto.Email is not null)
				user.ChangeEmail(updateUserDto.Email);

			if (updateUserDto.GitHubUrl is not null)
				user.ChangeGitHubUrl(updateUserDto.GitHubUrl);

			if (updateUserDto.DiscordUrl is not null)
				user.ChangeDiscordUrl(updateUserDto.DiscordUrl);

			if (id != currentUserId && updateUserDto.Role is not null)
			{
				var currentUser = await _userRepository.GetByIdAsync(currentUserId);
				var newRole = updateUserDto.Role.Value;

				if (newRole != user.Role)
					user.ChangeRole(newRole, currentUser!);
			}

			await _userRepository.UpdateAsync(user);

			return _mapper.Map<UserDto>(user);
		}

		public async Task ChangeRoleAsync(Guid userId, UserRole newRole, Guid changedByUserId)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException($"User with ID {userId} not found");

			var changedByUser = await _userRepository.GetByIdAsync(changedByUserId)
				?? throw new KeyNotFoundException($"User with ID {changedByUserId} not found");

			if (changedByUser.Role != UserRole.Admin)
				throw new UnauthorizedAccessException("Only admins can change user roles");

			user.ChangeRole(newRole, changedByUser);

			await _userRepository.UpdateAsync(user);
		}
	}
}
