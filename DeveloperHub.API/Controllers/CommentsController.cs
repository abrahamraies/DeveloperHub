using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeveloperHub.API.Controllers
{
	[ApiController]
	[Route("api/comments")]
	public class CommentsController(ICommentService commentService) : ControllerBase
	{
		private readonly ICommentService _commentService = commentService;

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			try
			{
				var comment = await _commentService.GetCommentByIdAsync(id);
				return Ok(comment);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpGet("project/{projectId:guid}")]
		public async Task<IActionResult> GetByProjectId(Guid projectId)
		{
			var comments = await _commentService.GetByProjectIdAsync(projectId);
			return Ok(comments);
		}

		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var comments = await _commentService.GetByUserIdAsync(userId);
			return Ok(comments);
		}

		[Authorize]
		[HttpPost("project/{projectId:guid}")]
		public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateCommentDto dto)
		{
			var userId = User.GetUserId();
			var comment = await _commentService.CreateAsync(dto, projectId, userId);
			return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
		}

		[Authorize]
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var userId = User.GetUserId();
			await _commentService.DeleteAsync(id, userId);
			return NoContent();
		}
	}
}
