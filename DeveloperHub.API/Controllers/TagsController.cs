using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperHub.API.Controllers
{
	[ApiController]
	[Route("api/tags")]
	public class TagsController(ITagService tagService) : ControllerBase
	{
		private readonly ITagService _tagService = tagService;

		[HttpGet("{id:guid}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			try
			{
				var tag = await _tagService.GetByIdAsync(id);
				return Ok(tag);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetByName(string name)
		{
			try
			{
				var tag = await _tagService.GetByNameAsync(name);
				return Ok(tag);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var tags = await _tagService.GetAllAsync();
			return Ok(tags);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
		{
			var tag = await _tagService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
		}

		[HttpPut("{id:guid}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(Guid id, [FromBody] CreateTagDto dto)
		{
			var tag = await _tagService.UpdateAsync(id, dto);
			return Ok(tag);
		}

		[HttpDelete("{id:guid}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await _tagService.DeleteAsync(id);
			return NoContent();
		}
	}
}
