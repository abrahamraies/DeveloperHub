using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperHub.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
	private readonly IUserService _userService = userService;

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		try
		{
			var user = await _userService.GetByIdAsync(id);
			return Ok(user);
		}
		catch (KeyNotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}

	[Authorize(Roles = "Admin")]
	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
	{
		var result = await _userService.GetAllAsync(page, size);
		return Ok(result);
	}

	[HttpPut("{id}")]
	[Authorize]
	public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
	{
		var currentUserId = User.GetUserId();
		var user = await _userService.UpdateUserAsync(id, dto, currentUserId);
		return Ok(user);
	}

	[HttpPut("{userId:guid}/role")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> ChangeRole(Guid userId, [FromBody] UserRole newRole)
	{
		if (!Enum.IsDefined(newRole))
		{
			return BadRequest($"the rol '{newRole}' is invalid.");
		}
		var currentUserId = User.GetUserId();
		await _userService.ChangeRoleAsync(userId, newRole, currentUserId);
		var updatedUser = await _userService.GetByIdAsync(userId);
		return NoContent();
	}
}