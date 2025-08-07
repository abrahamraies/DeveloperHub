using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using DeveloperHub.Application.Services;
using DeveloperHub.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperHub.Application.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddValidatorsFromAssemblyContaining<RegisterDto>();

			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IProjectService, ProjectService>();
			services.AddScoped<ICommentService, CommentService>();
			services.AddScoped<ITagService, TagService>();
			services.AddScoped<IUserService, UserService>();

			return services;
		}
	}
}
