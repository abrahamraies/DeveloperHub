using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DeveloperHub.Application.Services
{
	public class CommentService : ICommentService
	{
		private readonly ICommentRepository _commentRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IUserRepository _userRepository;
		private readonly IValidator<CreateCommentDto> _validator;
		private readonly IMapper _mapper;

		public CommentService
		(
			ICommentRepository commentRepository,
			IProjectRepository projectRepository,
			IUserRepository userRepository,
			IValidator<CreateCommentDto> validator,
			IMapper mapper
		)
		{
			_commentRepository = commentRepository;
			_projectRepository = projectRepository;
			_userRepository = userRepository;
			_validator = validator;
			_mapper = mapper;
		}

		public async Task<CommentDto> GetCommentByIdAsync(Guid id)
		{
			var comment = await _commentRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Comment with ID {id} not found");

			return _mapper.Map<CommentDto>(comment);
		}

		public async Task<IEnumerable<CommentDto>> GetByProjectIdAsync(Guid projectId)
		{
			var comments = await _commentRepository.GetByProjectIdAsync(projectId);
			return _mapper.Map<IEnumerable<CommentDto>>(comments);
		}

		public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(Guid userId)
		{
			var comments = await _commentRepository.GetByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<CommentDto>>(comments);
		}

		public async Task<CommentDto> CreateAsync(CreateCommentDto commentDto, Guid projectId, Guid userId)
		{
			await _validator.ValidateAndThrowAsync(commentDto);

			var project = await _projectRepository.GetByIdAsync(projectId)
				?? throw new KeyNotFoundException($"Project with ID {projectId} not found");

			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new KeyNotFoundException($"User with ID {userId} not found");

			var comment = new Comment(commentDto.Content, user.Id, project.Id);
			await _commentRepository.AddAsync(comment);

			return _mapper.Map<CommentDto>(comment);
		}

		public async Task DeleteAsync(Guid id, Guid userId)
		{
			var comment = await _commentRepository.GetByIdAsync(id)
				?? throw new KeyNotFoundException($"Comment with ID {id} not found");

			if (comment.UserId != userId && comment.User.Role != UserRole.Admin)
				throw new UnauthorizedAccessException("You don't have permission to delete this comment");

			await _commentRepository.DeleteAsync(comment);
		}
	}
}
