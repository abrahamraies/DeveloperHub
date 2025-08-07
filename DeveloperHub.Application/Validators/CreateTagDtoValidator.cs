using DeveloperHub.Application.DTOs;
using FluentValidation;

namespace DeveloperHub.Application.Validators
{
	public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
	{
		public CreateTagDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Tag name is required.")
				.MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters.")
				.Matches("^[a-zA-Z0-9\\s-]+$").WithMessage("Tag name can only contain letters, numbers, spaces and hyphens.");
		}
	}
}
