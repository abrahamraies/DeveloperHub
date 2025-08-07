using DeveloperHub.Application.Interfaces.Security;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperHub.WebApi.Data;

public static class SeedData
{
	public static async Task InitializeAsync(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DeveloperHubDbContext>();
		var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

		await context.Database.MigrateAsync();

		if (context.Users.Any()) return;

		var adminUser = new User(
			"admin",
			"admin@developerhub.com",
			passwordHasher.Hash("Admin123!"),
			UserRole.Admin
		);

		var regularUser = new User(
			"johndoe",
			"john@developerhub.com",
			passwordHasher.Hash("User123!"),
			UserRole.User
		);

		context.Users.AddRange(adminUser, regularUser);
		await context.SaveChangesAsync();

		var tags = new List<Tag>
		{
			new Tag { Name = "dotnet" },
			new Tag { Name = "react" },
			new Tag { Name = "javascript" },
			new Tag { Name = "typescript" }
		};

		context.Tags.AddRange(tags);
		await context.SaveChangesAsync();

		var project1 = new Project(
			"Clean Architecture Demo",
			"Ejemplo de Clean Architecture en .NET 8",
			"https://github.com/example/clean-arch-demo",
			null,
			adminUser.Id
		);

		var project2 = new Project(
			"React Portfolio",
			"Portafolio personal con React",
			"https://github.com/example/react-portfolio",
			null,
			regularUser.Id
		);

		context.Projects.AddRange(project1, project2);
		await context.SaveChangesAsync();

		var projectTags = new List<ProjectTag>
		{
			new() { Project = project1, Tag = tags[0] },
			new() { Project = project2, Tag = tags[1] },
			new() { Project = project2, Tag = tags[2] },
		};

		context.Set<ProjectTag>().AddRange(projectTags);
		await context.SaveChangesAsync();

		var comment1 = new Comment("Excelente ejemplo de arquitectura limpia!", regularUser.Id, project1.Id);
		var comment2 = new Comment("Me gustó mucho tu portafolio, muy bien estructurado.", adminUser.Id, project2.Id);

		context.Comments.AddRange(comment1, comment2);
		await context.SaveChangesAsync();
	}
}
