using DeveloperHub.Application.DTOs;
using FluentValidation;

namespace DeveloperHub.Application.Validators
{
	public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
	{
		public CreateProjectDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Project title is required.")
				.MaximumLength(100).WithMessage("Project title cannot exceed 100 characters.");

			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Project description is required.")
				.MaximumLength(1000).WithMessage("Project description cannot exceed 1000 characters.");

			RuleFor(x => x.GitHubUrl)
				.NotEmpty().WithMessage("GitHub URL is required.")
				.Must(BeValidUrl).WithMessage("Invalid GitHub URL format.");

			RuleFor(x => x.DiscordUrl)
				.Must(url => string.IsNullOrEmpty(url) || BeValidUrl(url))
				.When(x => !string.IsNullOrEmpty(x.DiscordUrl))
				.WithMessage("Invalid Discord URL format.");

			RuleFor(x => x.Tags)
				.NotEmpty().WithMessage("At least one tag is required.")
				.Must(tags => tags.Count <= 10)
				.WithMessage("A project cannot have more than 10 tags.");
		}

		private bool BeValidUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
				   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}
	}
}
