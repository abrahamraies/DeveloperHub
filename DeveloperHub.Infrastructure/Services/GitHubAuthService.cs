using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DeveloperHub.Infrastructure.Services
{
	public class GitHubAuthService(HttpClient httpClient, IConfiguration configuration) : IGitHubAuthService
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly IConfiguration _configuration = configuration;

		private static readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public async Task<string> GetAccessTokenAsync(string code)
		{
			var clientId = _configuration["GitHub:ClientId"];
			var clientSecret = _configuration["GitHub:ClientSecret"];
			var redirectUri = _configuration["GitHub:RedirectUri"];

			var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "client_id", clientId },
				{ "client_secret", clientSecret },
				{ "code", code },
				{ "redirect_uri", redirectUri }
			})
			};

			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();
			using var json = JsonDocument.Parse(content);

			if (json.RootElement.TryGetProperty("access_token", out var token))
				return token.GetString()!;

			throw new InvalidOperationException("No se pudo obtener el access token de GitHub");
		}

		public async Task<GitHubUserDto> GetUserAsync(string accessToken)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeveloperHub");

			var response = await _httpClient.GetStringAsync("https://api.github.com/user");
			return JsonSerializer.Deserialize<GitHubUserDto>(response, _jsonOptions)
				   ?? throw new InvalidOperationException("No se pudo obtener el usuario de GitHub");
		}

		public async Task<List<GitHubRepoDto>> GetUserReposAsync(string accessToken)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeveloperHub");

			var response = await _httpClient.GetStringAsync("https://api.github.com/user/repos?type=public");
			return JsonSerializer.Deserialize<List<GitHubRepoDto>>(response, _jsonOptions) ?? [];
		}

		public async Task<GitHubRepoDto?> GetRepoByNameAsync(string accessToken, string owner, string repoName)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DeveloperHub");

			var url = $"https://api.github.com/repos/{owner}/{repoName}";
			var response = await _httpClient.GetFromJsonAsync<GitHubRepoDto>(url, _jsonOptions);

			return response;
		}
	}
}
