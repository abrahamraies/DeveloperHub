using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services
{
	public class TagService : ITagService
	{
		private readonly ITagRepository _tagRepository;
		private readonly IValidator<CreateTagDto> _validator;
		private readonly IMapper _mapper;

		public TagService
		(
			ITagRepository tagRepository,
			IValidator<CreateTagDto> validator,
			IMapper mapper
		)
		{
			_tagRepository = tagRepository;
			_validator = validator;
			_mapper = mapper;
		}

		public async Task<TagDto> GetByIdAsync(Guid id)
		{
			var tag = await _tagRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Tag with ID {id} not found");

			return _mapper.Map<TagDto>(tag);
		}

		public async Task<TagDto> GetByNameAsync(string name)
		{
			var tag = await _tagRepository.GetByNameAsync(name)
				?? throw new KeyNotFoundException($"Tag with name '{name}' not found");

			return _mapper.Map<TagDto>(tag);
		}

		public async Task<IEnumerable<TagDto>> GetAllAsync()
		{
			var tags = await _tagRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<TagDto>>(tags);
		}

		public async Task<TagDto> CreateAsync(CreateTagDto dto)
		{
			await _validator.ValidateAndThrowAsync(dto);

			if (await _tagRepository.NameExistsAsync(dto.Name))
				throw new ValidationException("Tag name already exists");

			var tag = new Tag { Name = dto.Name };
			var createdTag = await _tagRepository.AddAsync(tag);

			return _mapper.Map<TagDto>(createdTag);
		}

		public async Task<TagDto> UpdateAsync(Guid id, CreateTagDto dto)
		{
			await _validator.ValidateAndThrowAsync(dto);

			var tag = await _tagRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Tag with ID {id} not found");

			if (await _tagRepository.NameExistsAsync(dto.Name) &&
						!tag.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
			{
				throw new ValidationException("Tag name already exists");
			}

			tag.Name = dto.Name;
			await _tagRepository.UpdateAsync(tag);

			return _mapper.Map<TagDto>(tag);
		}

		public async Task DeleteAsync(Guid id)
		{
			var tag = await _tagRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Tag with ID {id} not found");

			await _tagRepository.DeleteAsync(tag);
		}
	}
}
