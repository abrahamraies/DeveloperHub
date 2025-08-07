using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
			var result = await _authService.RegisterAsync(dto);
			return Ok(result);
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
			if (User.Identity != null && User.Identity.IsAuthenticated)
				return Unauthorized();

			var userId = User.GetUserId();
			var user = await _authService.GetCurrentUserAsync(userId);
			return Ok(user);
		}
	}
}
