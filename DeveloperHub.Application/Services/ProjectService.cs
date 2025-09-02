using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using DeveloperHub.Domain.ValueObjects;
using FluentValidation;

namespace DeveloperHub.Application.Services;

public class ProjectService(
	IProjectRepository projectRepository,
	IUserRepository userRepository,
	ITagRepository tagRepository,
	IValidator<CreateProjectDto> createValidator,
	IValidator<UpdateProjectDto> updateValidator,
	IMapper mapper
) : IProjectService
{
	public async Task<ProjectDto?> GetByIdAsync(Guid id)
	{
		var project = await projectRepository.GetByIdAsync(id);
		return project != null ? mapper.Map<ProjectDto>(project) : null;
	}

	public async Task<PaginatedResult<ProjectSummaryDto>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize)
	{
		pageNumber = pageNumber < 1 ? 1 : pageNumber;
		pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

		var projects = await projectRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
		var totalCount = await projectRepository.GetUserTotalCountAsync(userId);

		var projectDtos = mapper.Map<List<ProjectSummaryDto>>(projects);
		return new PaginatedResult<ProjectSummaryDto>(projectDtos, totalCount, pageNumber, pageSize);
	}

	public async Task<IEnumerable<ProjectSummaryDto>> GetByTagAsync(string tagName, int pageNumber, int pageSize)
	{
		var projects = await projectRepository.GetByTagAsync(tagName, pageNumber, pageSize);
		return mapper.Map<IEnumerable<ProjectSummaryDto>>(projects);
	}

	public async Task<PaginatedResult<ProjectSummaryDto>> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? search = null,
		List<string> tags = null!)
	{
		pageNumber = pageNumber < 1 ? 1 : pageNumber;
		pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

		var projects = await projectRepository.GetPagedListAsync(
			pageNumber,
			pageSize,
			search,
			tags ?? new List<string>()
		);

		var totalCount = await projectRepository.GetTotalCountAsync(search, tags ?? new List<string>());

		var projectDtos = mapper.Map<List<ProjectSummaryDto>>(projects);
		return new PaginatedResult<ProjectSummaryDto>(projectDtos, totalCount, pageNumber, pageSize);
	}

	public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid userId)
	{
		await createValidator.ValidateAndThrowAsync(dto);

		var existing = await projectRepository.GetByGitHubUrlAsync(dto.GitHubUrl);
		if (existing != null)
			throw new InvalidOperationException($"The project with URL {dto.GitHubUrl} already exists");

		var user = await userRepository.GetByIdAsync(userId)
			?? throw new KeyNotFoundException($"User with ID {userId} not found");

		var project = new Project
		{
			Title = dto.Title,
			Description = dto.Description,
			GitHubUrl = new ProjectUrl(dto.GitHubUrl, UrlType.GitHub),
			DiscordUrl = dto.DiscordUrl != null ? new ProjectUrl(dto.DiscordUrl, UrlType.Discord) : null,
			OwnerId = user.Id
		};

		var tags = await GetOrCreateTags(dto.Tags);
		foreach (var tag in tags)
		{
			project.AddTag(tag);
		}

		await projectRepository.AddAsync(project);

		return mapper.Map<ProjectDto>(project);
	}

	public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto, Guid userId)
	{
		if (!await projectRepository.ExistsAsync(id))
			throw new KeyNotFoundException($"Project with ID {id} not found");

		if (!await IsOwnerAsync(id, userId))
			throw new UnauthorizedAccessException("You don't have permission to update this project");

		var project = await projectRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Project with ID {id} not found");

		project.Update(
			dto.Title ?? project.Title,
			dto.Description ?? project.Description,
			!string.IsNullOrWhiteSpace(dto.GitHubUrl) ? new ProjectUrl(dto.GitHubUrl, UrlType.GitHub) : project.GitHubUrl,
			!string.IsNullOrWhiteSpace(dto.DiscordUrl) ? new ProjectUrl(dto.DiscordUrl, UrlType.Discord) : project.DiscordUrl
		);

		if (dto.Tags is not null && dto.Tags.Any())
		{
			await UpdateProjectTags(project, dto.Tags);
		}

		await projectRepository.UpdateAsync(project);

		return mapper.Map<ProjectDto>(project);
	}

	public async Task DeleteAsync(Guid id, Guid userId)
	{
		if (!await projectRepository.ExistsAsync(id))
			throw new KeyNotFoundException($"Project with ID {id} not found");

		if (!await IsOwnerAsync(id, userId))
			throw new UnauthorizedAccessException("You don't have permission to delete this project");

		var project = await projectRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Project with ID {id} not found");

		await projectRepository.DeleteAsync(project);
	}

	public async Task<bool> IsOwnerAsync(Guid projectId, Guid userId)
	{
		return await projectRepository.IsOwnerAsync(projectId, userId);
	}

	private async Task<List<Tag>> GetOrCreateTags(List<string> tagNames)
	{
		var tags = new List<Tag>();
		foreach (var tagName in tagNames)
		{
			var tag = await tagRepository.GetOrCreateAsync(tagName);
			tags.Add(tag);
		}
		return tags;
	}

	private async Task UpdateProjectTags(Project project, List<string> newTagNames)
	{
		var currentTagIds = project.ProjectTags.Select(pt => pt.TagId).ToList();

		var allTags = await tagRepository.GetByNamesAsync(newTagNames);
		var newTags = newTagNames
			.Where(name => !allTags.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
			.ToList();

		foreach (var tagName in newTags)
		{
			var newTag = await tagRepository.GetOrCreateAsync(tagName);
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
			await tagRepository.AddProjectTagAsync(projectTag);
		}

		foreach (var tagId in tagIdsToRemove)
		{
			var projectTag = project.ProjectTags
				.FirstOrDefault(pt => pt.TagId == tagId);

			if (projectTag != null)
			{
				await tagRepository.RemoveProjectTagAsync(projectTag);
			}
		}
	}
}
