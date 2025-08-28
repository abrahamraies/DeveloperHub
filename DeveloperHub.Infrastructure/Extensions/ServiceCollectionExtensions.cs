using DeveloperHub.Application.Interfaces;
using DeveloperHub.Application.Interfaces.Security;
using DeveloperHub.Domain.Interfaces.Repositories;
using DeveloperHub.Infrastructure.Repositories;
using DeveloperHub.Infrastructure.Security;
using DeveloperHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperHub.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<DeveloperHubDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IProjectRepository, ProjectRepository>();
			services.AddScoped<ICommentRepository, CommentRepository>();
			services.AddScoped<ITagRepository, TagRepository>();
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.AddHttpClient<IGitHubAuthService, GitHubAuthService>();

			return services;
		}
	}
}
