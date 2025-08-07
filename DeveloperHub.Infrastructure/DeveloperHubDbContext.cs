using DeveloperHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperHub.Infrastructure;

public class DeveloperHubDbContext(DbContextOptions<DeveloperHubDbContext> options) : DbContext(options)
{
	// DbSets
	public DbSet<User> Users => Set<User>();
	public DbSet<Project> Projects => Set<Project>();
	public DbSet<Tag> Tags => Set<Tag>();
	public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
	public DbSet<Comment> Comments => Set<Comment>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		ConfigureUser(modelBuilder);
		ConfigureProject(modelBuilder);
		ConfigureProjectTag(modelBuilder);
		ConfigureComment(modelBuilder);
	}

	private void ConfigureUser(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(builder =>
		{
			builder.HasIndex(u => u.Email).IsUnique();
		});
	}

	private void ConfigureProject(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Project>(builder =>
		{
			builder.HasIndex(p => p.Title);

			builder.HasOne(p => p.Owner)
				   .WithMany(u => u.Projects)
				   .HasForeignKey(p => p.OwnerId);

			builder.HasMany(p => p.Comments)
				   .WithOne(c => c.Project)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(p => p.ProjectTags)
				   .WithOne(pt => pt.Project)
				   .HasForeignKey(pt => pt.ProjectId);

			// Value Objects (Owned Types)
			builder.OwnsOne(p => p.GitHubUrl, owned =>
			{
				owned.Property(o => o.Url).HasColumnName("GitHubUrl").IsRequired();
				owned.Property(o => o.Type).HasColumnName("GitHubUrlType").IsRequired();
			});

			builder.OwnsOne(p => p.DiscordUrl, owned =>
			{
				owned.Property(o => o.Url).HasColumnName("DiscordUrl");
				owned.Property(o => o.Type).HasColumnName("DiscordUrlType");
			});
		});
	}

	private void ConfigureProjectTag(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectTag>(builder =>
		{
			builder.HasKey(pt => new { pt.ProjectId, pt.TagId });

			builder.HasOne(pt => pt.Project)
				   .WithMany(p => p.ProjectTags)
				   .HasForeignKey(pt => pt.ProjectId);

			builder.HasOne(pt => pt.Tag)
				   .WithMany(t => t.ProjectTags)
				   .HasForeignKey(pt => pt.TagId);
		});
	}

	private void ConfigureComment(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Comment>(builder =>
		{
			builder.HasOne(c => c.User)
				   .WithMany(u => u.Comments)
				   .HasForeignKey(c => c.UserId);

			builder.HasOne(c => c.Project)
				   .WithMany(p => p.Comments)
				   .HasForeignKey(c => c.ProjectId);
		});
	}
}
