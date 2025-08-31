using AutoMapper;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Domain.Entities;
using DeveloperHub.Domain.Enums;
using DeveloperHub.Domain.ValueObjects;

namespace DeveloperHub.Application.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserDto>()
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

			CreateMap<UpdateUserDto, User>()
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl))
				.ForMember(dest => dest.DiscordUrl, opt => opt.MapFrom(src => src.DiscordUrl));

			CreateMap<Tag, TagDto>();

			CreateMap<Project, ProjectDto>()
				.ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner.Username))
				.ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl.Url))
				.ForMember(dest => dest.DiscordUrl, opt => opt.MapFrom(src => src.DiscordUrl != null ? src.DiscordUrl.Url : null))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProjectTags.Select(pt => pt.Tag.Name)))
				.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

			CreateMap<Project, ProjectSummaryDto>()
				.ForMember(dest => dest.OwnerUsername, opt => opt.MapFrom(src => src.Owner.Username))
				.ForMember(dest => dest.OwnerProfileImageUrl, opt => opt.MapFrom(src => src.Owner.ProfileImageUrl))
				.ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProjectTags.Select(pt => pt.Tag.Name).ToList()));

			CreateMap<UpdateProjectDto, Project>()
				.ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src =>
					src.GitHubUrl != null ? new ProjectUrl(src.GitHubUrl, UrlType.GitHub) : null))
				.ForMember(dest => dest.DiscordUrl, opt => opt.MapFrom(src =>
					src.DiscordUrl != null ? new ProjectUrl(src.DiscordUrl, UrlType.Discord) : null))
				.ForMember(dest => dest.ProjectTags, opt => opt.Ignore());

			CreateMap<Comment, CommentDto>()
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

			CreateMap<Tag, TagDto>();
			CreateMap<CreateTagDto, Tag>();
		}
	}
}
