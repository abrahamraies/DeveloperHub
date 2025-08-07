using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Application.Interfaces.Security;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IJwtService _jwtService;
		private readonly IMapper _mapper;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IUserRepository _userRepository;
		private readonly IValidator<RegisterDto> _registerValidator;

		public AuthService
		(
			IJwtService jwtService,
			IMapper mapper,
			IPasswordHasher passwordHasher,
			IUserRepository userRepository,
			IValidator<RegisterDto> registerValidator
		)
		{
			_jwtService = jwtService;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
			_userRepository = userRepository;
			_registerValidator = registerValidator;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
		{
			await _registerValidator.ValidateAndThrowAsync(dto);

			if (await _userRepository.EmailExistsAsync(dto.Email))
				throw new InvalidOperationException("Email is already in use.");

			var user = new User(dto.Username, dto.Email, _passwordHasher.Hash(dto.Password), UserRole.User);

			await _userRepository.AddAsync(user);

			var token = _jwtService.GenerateToken(user);
			return new AuthResponseDto(token, user.Username, user.Email, user.Role.ToString());
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			var user = await _userRepository.GetByEmailAsync(dto.Email);
			if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
				throw new UnauthorizedAccessException("Invalid email or password.");

			if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
				throw new UnauthorizedAccessException("Invalid email or password.");

			var token = _jwtService.GenerateToken(user);
			return new AuthResponseDto(token, user.Username, user.Email, user.Role.ToString());
		}

		public async Task<UserDto> GetCurrentUserAsync(Guid userId)
		{
			var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found.");
			return _mapper.Map<UserDto>(user);
		}
	}
}
