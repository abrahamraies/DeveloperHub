using DeveloperHub.Application.DTOs;
using FluentValidation;

namespace DeveloperHub.Application.Validators
{
	public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
	{
		public CreateCommentDtoValidator()
		{
			RuleFor(x => x.Content)
				.NotEmpty().WithMessage("Comment content is required.")
				.MinimumLength(5).WithMessage("Comment must be at least 5 characters long.")
				.MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
		}
	}
}
