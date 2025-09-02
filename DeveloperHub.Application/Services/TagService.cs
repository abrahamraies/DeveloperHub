using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services;

public class TagService(
	ITagRepository tagRepository,
	IValidator<CreateTagDto> validator,
	IMapper mapper
) : ITagService
{
	public async Task<TagDto> GetByIdAsync(Guid id)
	{
		var tag = await tagRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Tag with ID {id} not found");

		return mapper.Map<TagDto>(tag);
	}

	public async Task<TagDto> GetByNameAsync(string name)
	{
		var tag = await tagRepository.GetByNameAsync(name)
			?? throw new KeyNotFoundException($"Tag with name '{name}' not found");

		return mapper.Map<TagDto>(tag);
	}

	public async Task<IEnumerable<TagDto>> GetAllAsync()
	{
		var tags = await tagRepository.GetAllAsync();
		return mapper.Map<IEnumerable<TagDto>>(tags);
	}

	public async Task<TagDto> CreateAsync(CreateTagDto dto)
	{
		await validator.ValidateAndThrowAsync(dto);

		if (await tagRepository.NameExistsAsync(dto.Name))
			throw new ValidationException("Tag name already exists");

		var tag = new Tag { Name = dto.Name };
		var createdTag = await tagRepository.AddAsync(tag);

		return mapper.Map<TagDto>(createdTag);
	}

	public async Task<TagDto> UpdateAsync(Guid id, CreateTagDto dto)
	{
		await validator.ValidateAndThrowAsync(dto);

		var tag = await tagRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Tag with ID {id} not found");

		if (await tagRepository.NameExistsAsync(dto.Name) &&
					!tag.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
		{
			throw new ValidationException("Tag name already exists");
		}

		tag.Name = dto.Name;
		await tagRepository.UpdateAsync(tag);

		return mapper.Map<TagDto>(tag);
	}

	public async Task DeleteAsync(Guid id)
	{
		var tag = await tagRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Tag with ID {id} not found");

		await tagRepository.DeleteAsync(tag);
	}
}
