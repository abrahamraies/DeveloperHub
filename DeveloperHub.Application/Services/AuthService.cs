using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Application.Interfaces.Security;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;
using System.Net;

namespace DeveloperHub.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IEmailService _emailService;
		private readonly IJwtService _jwtService;
		private readonly IMapper _mapper;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IUserRepository _userRepository;
		private readonly IValidator<RegisterDto> _registerValidator;

		public AuthService
		(
			IEmailService emailService,
			IJwtService jwtService,
			IMapper mapper,
			IPasswordHasher passwordHasher,
			IUserRepository userRepository,
			IValidator<RegisterDto> registerValidator
		)
		{
			_emailService = emailService;
			_jwtService = jwtService;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
			_userRepository = userRepository;
			_registerValidator = registerValidator;
		}

		public async Task RegisterAsync(RegisterDto dto)
		{
			await _registerValidator.ValidateAndThrowAsync(dto);

			if (await _userRepository.EmailExistsAsync(dto.Email))
				throw new InvalidOperationException("Email is already in use.");

			var user = new User(dto.Username, dto.Email, _passwordHasher.Hash(dto.Password), UserRole.User);

			var verificationToken = Guid.NewGuid().ToString("N");
			var expiry = DateTime.UtcNow.AddHours(24);

			user.SetVerificationToken(verificationToken, expiry);
			await _userRepository.AddAsync(user);

			await _emailService.SendVerificationEmail(user.Email, verificationToken);
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			var user = await _userRepository.GetByEmailAsync(dto.Email);
			if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
				throw new UnauthorizedAccessException("Invalid email or password.");

			if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
				throw new UnauthorizedAccessException("Invalid email or password.");

			if (!user.EmailVerified)
				throw new InvalidOperationException("You must confirm your email before logging in.");

			var token = _jwtService.GenerateToken(user);
			return new AuthResponseDto(user.Id, token);
		}

		public async Task<UserDto> GetCurrentUserAsync(Guid userId)
		{
			var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found.");
			return _mapper.Map<UserDto>(user);
		}

		public async Task<bool> ForgotPasswordAsync(string email)
		{
			var user = await _userRepository.GetByEmailAsync(email);
			if (user == null) return false;

			var token = Guid.NewGuid().ToString("N");
			user.SetPasswordResetToken(token, DateTime.UtcNow.AddHours(1));
			await _userRepository.UpdateAsync(user);

			var resetLink = $"http://localhost:5173/reset-password?token={token}";
			await _emailService.SendEmailAsync(user.Email, "Recuperar contraseña",
				$"Haz click en el siguiente enlace para resetear tu contraseña: {resetLink}");

			return true;
		}

		public async Task<bool> ResetPasswordAsync(string token, string newPassword)
		{
			var user = (await _userRepository.GetAllAsync(1, int.MaxValue))
				.FirstOrDefault(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTime.UtcNow);

			if (user == null) return false;

			var hashedPassword = _passwordHasher.Hash(newPassword);
			user.UpdatePassword(hashedPassword);

			await _userRepository.UpdateAsync(user);
			return true;
		}

		public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
		{
			var user = await _userRepository.GetByIdAsync(userId);
			if (user == null) throw new KeyNotFoundException("User not found.");

			if (!_passwordHasher.Verify(currentPassword, user.PasswordHash))
				return false;

			var hashedPassword = _passwordHasher.Hash(newPassword);
			user.UpdatePassword(hashedPassword);

			await _userRepository.UpdateAsync(user);
			return true;
		}

		public async Task<bool> ConfirmEmailAsync(string token)
		{
			var user = await _userRepository.GetByConfirmationTokenAsync(token);

			if (user == null || user.VerificationTokenExpiry < DateTime.UtcNow)
				return false;

			user.SetEmailVerified();
			await _userRepository.UpdateAsync(user);

			return true;
		}

		public async Task<bool> ResendVerificationEmailAsync(string email)
		{
			var user = await _userRepository.GetByEmailAsync(email);
			if (user == null)
				return false;

			if (user.EmailVerified)
				throw new InvalidOperationException("The email has already been verified.");

			var verificationToken = Guid.NewGuid().ToString("N");
			var expiry = DateTime.UtcNow.AddHours(24);

			user.SetVerificationToken(verificationToken, expiry);
			await _userRepository.UpdateAsync(user);

			await _emailService.SendVerificationEmail(user.Email, verificationToken);

			return true;
		}
	}
}
