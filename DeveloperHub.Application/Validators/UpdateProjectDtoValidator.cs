using DeveloperHub.Application.DTOs;
using FluentValidation;

namespace DeveloperHub.Application.Validators
{
	public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
	{
		public UpdateProjectDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Project title is required.")
				.MaximumLength(100).WithMessage("Project title cannot exceed 100 characters.");

			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Project description is required.")
				.MaximumLength(1000).WithMessage("Project description cannot exceed 1000 characters.");

			RuleFor(x => x.GitHubUrl)
				.NotEmpty().WithMessage("GitHub URL is required.");

			RuleFor(x => x.DiscordUrl)
				.NotEmpty().WithMessage("Discord URL is required.");

			RuleFor(x => x.Tags)
				.NotEmpty().WithMessage("At least one tag is required.")
				.Must(tags => tags.Count <= 10)
				.WithMessage("A project cannot have more than 10 tags.");
		}
	}
}
