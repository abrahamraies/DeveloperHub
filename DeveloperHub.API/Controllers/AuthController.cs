using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperHub.API.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController(IAuthService authService) : ControllerBase
	{
		private readonly IAuthService _authService = authService;

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			try
			{
				await _authService.RegisterAsync(dto);
				return Ok(new { message = "Usuario registrado. Revisa tu email para verificar tu cuenta." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var result = await _authService.LoginAsync(dto);
			return Ok(result);
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> GetCurrentUser()
		{
			if (User.Identity != null && !User.Identity.IsAuthenticated)
				return Unauthorized();

			var userId = User.GetUserId();
			var user = await _authService.GetCurrentUserAsync(userId);
			return Ok(user);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.ForgotPasswordAsync(dto.Email);
			if (!result)
				return BadRequest(new { message = "No se encontró un usuario con ese email." });

			return Ok(new { message = "Si el email está registrado, hemos enviado un enlace de recuperación." });
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
			if (!result)
				return BadRequest(new { message = "El enlace de recuperación no es válido o ha expirado." });

			return Ok(new { message = "Tu contraseña ha sido restablecida con éxito." });
		}

		[HttpPost("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			var userId = User.GetUserId();
			var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

			if (!result)
				return BadRequest(new { message = "La contraseña actual es incorrecta." });

			return Ok(new { message = "Contraseña actualizada correctamente." });
		}

		[HttpGet("verify-email")]
		public async Task<IActionResult> VerifyEmail([FromQuery] string token)
		{
			if (string.IsNullOrWhiteSpace(token))
				return BadRequest(new { message = "Token es requerido" });

			var success = await _authService.ConfirmEmailAsync(token);

			if (!success)
				return BadRequest("Token inválido o expirado.");

			return Ok("Email confirmado correctamente. Ahora puedes iniciar sesión.");
		}

		[HttpPost("resend-verification")]
		public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationDto dto)
		{
			try
			{
				var success = await _authService.ResendVerificationEmailAsync(dto.Email);

				if (!success)
					return Ok(new { message = "Si el email está registrado, se reenviará la verificación." });

				return Ok(new { message = "Email de verificación reenviado." });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
