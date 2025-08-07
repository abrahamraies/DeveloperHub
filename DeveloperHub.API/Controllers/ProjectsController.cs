using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeveloperHub.API.Controllers
{
	[ApiController]
	[Route("api/projects")]
	public class ProjectsController(
		IProjectService projectService,
		IHttpContextAccessor httpContextAccessor) : ControllerBase
	{
		private readonly IProjectService _projectService = projectService;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var project = await _projectService.GetByIdAsync(id);
			if (project == null)
			{
				return NotFound($"Project with ID {id} not found");
			}
			return Ok(project);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll(
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var result = await _projectService.GetPagedAsync(pageNumber, pageSize);
			return Ok(result);
		}

		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> GetByUserId(
			Guid userId,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var projects = await _projectService.GetByUserIdAsync(userId);
			return Ok(projects);
		}

		[HttpGet("tag/{tagName}")]
		public async Task<IActionResult> GetByTag(
			string tagName,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 10)
		{
			var projects = await _projectService.GetByTagAsync(tagName);
			return Ok(projects);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
		{
			var userId = User.GetUserId();
			var project = await _projectService.CreateAsync(dto, userId);
			return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
		}

		[HttpPut("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
		{
			var userId = User.GetUserId();
			await _projectService.UpdateAsync(id, dto, userId);
			return NoContent();
		}

		[HttpDelete("{id:guid}")]
		[Authorize]
		public async Task<IActionResult> Delete(Guid id)
		{
			var userId = User.GetUserId();
			await _projectService.DeleteAsync(id, userId);
			return NoContent();
		}
	}
}
