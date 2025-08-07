using DeveloperHub.Application.DTOs;
using FluentValidation;

namespace DeveloperHub.Application.Validators
{
	public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
	{
		public UpdateUserDtoValidator()
		{
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Username is required.")
				.MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");

			RuleFor(x => x.GitHubUrl)
				.Must(url => string.IsNullOrEmpty(url) || BeValidUrl(url))
				.When(x => !string.IsNullOrEmpty(x.GitHubUrl))
				.WithMessage("Invalid GitHub URL format.");

			RuleFor(x => x.DiscordUrl)
				.Must(url => string.IsNullOrEmpty(url) || BeValidUrl(url))
				.When(x => !string.IsNullOrEmpty(x.DiscordUrl))
				.WithMessage("Invalid Discord URL format.");
		}

		private bool BeValidUrl(string url)
		{
			return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
				   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}
	}
}
