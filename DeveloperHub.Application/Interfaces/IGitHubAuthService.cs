using DeveloperHub.Application.DTOs;

namespace DeveloperHub.Application.Interfaces
{
	public interface IGitHubAuthService
	{
		Task<string> GetAccessTokenAsync(string code);
		Task<GitHubUserDto> GetUserAsync(string accessToken);
		Task<List<GitHubRepoDto>> GetUserReposAsync(string accessToken);
		Task<GitHubRepoDto?> GetRepoByNameAsync(string accessToken, string owner, string repoName);
	}
}
