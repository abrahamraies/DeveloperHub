using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services
{
	public class ProjectService : IProjectService
	{
		private readonly IProjectRepository _projectRepository;
		private readonly IUserRepository _userRepository;
		private readonly ITagRepository _tagRepository;
		private readonly IValidator<CreateProjectDto> _createValidator;
		private readonly IValidator<UpdateProjectDto> _updateValidator;
		private readonly IMapper _mapper;

		public ProjectService
		(
			IProjectRepository projectRepository,
			IUserRepository userRepository,
			ITagRepository tagRepository,
			IValidator<CreateProjectDto> createValidator,
			IValidator<UpdateProjectDto> updateValidator,
			IMapper mapper
		)
		{
			_projectRepository = projectRepository;
			_userRepository = userRepository;
			_tagRepository = tagRepository;
			_createValidator = createValidator;
			_updateValidator = updateValidator;
			_mapper = mapper;
		}

		public async Task<ProjectDto?> GetByIdAsync(Guid id)
		{
			var project = await _projectRepository.GetByIdAsync(id);
			return project != null ? _mapper.Map<ProjectDto>(project) : null;
		}

		public async Task<IEnumerable<ProjectSummaryDto>> GetByUserIdAsync(Guid userId)
		{
			var projects = await _projectRepository.GetByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<ProjectSummaryDto>>(projects);
		}

		public async Task<IEnumerable<ProjectSummaryDto>> GetByTagAsync(string tagName)
		{
			var projects = await _projectRepository.GetByTagAsync(tagName);
			return _mapper.Map<IEnumerable<ProjectSummaryDto>>(projects);
		}

		public async Task<PaginatedResult<ProjectSummaryDto>> GetPagedAsync(
			int pageNumber,
			int pageSize,
			string? search = null,
			List<string> tags = null!)
		{
			pageNumber = pageNumber < 1 ? 1 : pageNumber;
			pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

			var projects = await _projectRepository.GetPagedListAsync(
				pageNumber,
				pageSize,
				search,
				tags ?? new List<string>()
			);

			var totalCount = await _projectRepository.GetTotalCountAsync(search, tags ?? new List<string>());

			var projectDtos = _mapper.Map<List<ProjectSummaryDto>>(projects);
			return new PaginatedResult<ProjectSummaryDto>(projectDtos, totalCount, pageNumber, pageSize);
		}

		public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid userId)
		{
			await _createValidator.ValidateAndThrowAsync(dto);

			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException($"User with ID {userId} not found");

			var project = new Project(
				dto.Title,
				dto.Description,
				dto.GitHubUrl,
				dto.DiscordUrl,
				user.Id
			);

			var tags = await GetOrCreateTags(dto.Tags);
			foreach (var tag in tags)
			{
				project.AddTag(tag);
			}

			await _projectRepository.AddAsync(project);

			return _mapper.Map<ProjectDto>(project);
		}

		public async Task UpdateAsync(Guid id, UpdateProjectDto dto, Guid userId)
		{
			if (!await _projectRepository.ExistsAsync(id))
				throw new KeyNotFoundException($"Project with ID {id} not found");

			if (!await IsOwnerAsync(id, userId))
				throw new UnauthorizedAccessException("You don't have permission to update this project");

			var project = await _projectRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Project with ID {id} not found");

			project.Update(
				dto.Title ?? project.Title,
				dto.Description ?? project.Description,
				dto.GitHubUrl ?? project.GitHubUrl,
				dto.DiscordUrl ?? project.DiscordUrl
			);

			if (dto.Tags is not null && dto.Tags.Any())
			{
				await UpdateProjectTags(project, dto.Tags);
			}

			await _projectRepository.UpdateAsync(project);
		}

		public async Task DeleteAsync(Guid id, Guid userId)
		{
			if (!await _projectRepository.ExistsAsync(id))
				throw new KeyNotFoundException($"Project with ID {id} not found");

			if (!await IsOwnerAsync(id, userId))
				throw new UnauthorizedAccessException("You don't have permission to delete this project");

			var project = await _projectRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Project with ID {id} not found");

			await _projectRepository.DeleteAsync(project);
		}

		public async Task<bool> IsOwnerAsync(Guid projectId, Guid userId)
		{
			return await _projectRepository.IsOwnerAsync(projectId, userId);
		}

		private async Task<List<Tag>> GetOrCreateTags(List<string> tagNames)
		{
			var tags = new List<Tag>();
			foreach (var tagName in tagNames)
			{
				var tag = await _tagRepository.GetOrCreateAsync(tagName);
				tags.Add(tag);
			}
			return tags;
		}

		private async Task UpdateProjectTags(Project project, List<string> newTagNames)
		{
			var currentTagIds = project.ProjectTags.Select(pt => pt.TagId).ToList();

			var allTags = await _tagRepository.GetByNamesAsync(newTagNames);
			var newTags = newTagNames
				.Where(name => !allTags.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
				.ToList();

			foreach (var tagName in newTags)
			{
				var newTag = await _tagRepository.GetOrCreateAsync(tagName);
				allTags = allTags.Append(newTag).ToList();
			}

			var tagIdsToAdd = allTags
				.Where(t => !currentTagIds.Contains(t.Id))
				.Select(t => t.Id)
				.ToList();

			var tagIdsToRemove = currentTagIds
				.Where(id => !allTags.Any(t => t.Id == id))
				.ToList();

			foreach (var tagId in tagIdsToAdd)
			{
				var projectTag = new ProjectTag
				{
					ProjectId = project.Id,
					TagId = tagId
				};
				await _tagRepository.AddProjectTagAsync(projectTag);
			}

			foreach (var tagId in tagIdsToRemove)
			{
				var projectTag = project.ProjectTags
					.FirstOrDefault(pt => pt.TagId == tagId);

				if (projectTag != null)
				{
					await _tagRepository.RemoveProjectTagAsync(projectTag);
				}
			}
		}
	}
}
