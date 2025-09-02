using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services;

public class CommentService(
	ICommentRepository commentRepository,
	IProjectRepository projectRepository,
	IUserRepository userRepository,
	IValidator<CreateCommentDto> validator,
	IMapper mapper
) : ICommentService
{
	public async Task<CommentDto> GetCommentByIdAsync(Guid id)
	{
		var comment = await commentRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Comment with ID {id} not found");

		return mapper.Map<CommentDto>(comment);
	}

	public async Task<IEnumerable<CommentDto>> GetByProjectIdAsync(Guid projectId)
	{
		var comments = await commentRepository.GetByProjectIdAsync(projectId);
		return mapper.Map<IEnumerable<CommentDto>>(comments);
	}

	public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(Guid userId)
	{
		var comments = await commentRepository.GetByUserIdAsync(userId);
		return mapper.Map<IEnumerable<CommentDto>>(comments);
	}

	public async Task<CommentDto> CreateAsync(CreateCommentDto commentDto, Guid projectId, Guid userId)
	{
		await validator.ValidateAndThrowAsync(commentDto);

		var project = await projectRepository.GetByIdAsync(projectId)
			?? throw new KeyNotFoundException($"Project with ID {projectId} not found");

		var user = await userRepository.GetByIdAsync(userId)
			?? throw new KeyNotFoundException($"User with ID {userId} not found");

		var comment = new Comment
		{
			Content = commentDto.Content,
			UserId = user.Id,
			ProjectId = project.Id
		};
		await commentRepository.AddAsync(comment);

		return mapper.Map<CommentDto>(comment);
	}

	public async Task DeleteAsync(Guid id, Guid userId)
	{
		var comment = await commentRepository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"Comment with ID {id} not found");

		if (comment.UserId != userId && comment.User.Role != UserRole.Admin)
			throw new UnauthorizedAccessException("You don't have permission to delete this comment");

		await commentRepository.DeleteAsync(comment);
	}
}
