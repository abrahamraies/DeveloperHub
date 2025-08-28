using DeveloperHub.API.Extensions;
using DeveloperHub.Application.DTOs;
using DeveloperHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperHub.API.Controllers
{
	[ApiController]
	[Route("api/auth/github")]
	public class GitHubController(IGitHubAuthService gitHubService, IConfiguration configuration, IUserService userService, IProjectService projectService) : ControllerBase
	{
		private readonly IGitHubAuthService _gitHubService = gitHubService;
		private readonly IConfiguration _configuration = configuration;
		private readonly IUserService _userService = userService;
		private readonly IProjectService _projectService = projectService;

		[HttpGet]
		public IActionResult Authenticate()
		{
			var clientId = _configuration["GitHub:ClientId"];
			var redirectUri = _configuration["GitHub:RedirectUri"];
			var scope = "read:user,public_repo";

			var githubLoginUrl = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";
			return Redirect(githubLoginUrl);
		}

		[HttpGet("repos")]
		[Authorize]
		public async Task<IActionResult> GetRepos([FromQuery] string token)
		{
			var repos = await _gitHubService.GetUserReposAsync(token);
			return Ok(repos);
		}

		[HttpPost("callback")]
		public async Task<IActionResult> Callback([FromForm] string code)
		{
			try
			{
				var accessToken = await _gitHubService.GetAccessTokenAsync(code);
				var githubUser = await _gitHubService.GetUserAsync(accessToken);

				var userId = User.GetUserId();
				var user = await _userService.GetByIdAsync(userId);

				if (user != null && string.IsNullOrWhiteSpace(user.GitHubUrl))
				{
					var updateUserDto = new UpdateUserDto
					{
						GitHubUrl = $"https://github.com/{githubUser.Login}"
					};
					await _userService.UpdateUserAsync(userId, updateUserDto, userId);
				}

				return Ok(new
				{
					githubId = githubUser.Id,
					username = githubUser.Login,
					avatarUrl = githubUser.AvatarUrl,
					email = githubUser.Email,
					accessToken
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = "Error al conectar con GitHub", message = ex.Message });
			}
		}

		[HttpPost("import")]
		[Authorize]
		public async Task<IActionResult> ImportRepo([FromQuery] string token, [FromQuery] string owner, [FromQuery] string repoName)
		{
			try
			{
				var repo = await _gitHubService.GetRepoByNameAsync(token, owner, repoName);
				if (repo is null)
					return NotFound(new { error = "No se encontró el repositorio en GitHub" });

				var userId = User.GetUserId();

				var dto = new CreateProjectDto(
					repo.Name,
					repo.Description ?? "Sin descripción",
					repo.HtmlUrl,
					null,
					repo.Topics
				);

				var project = await _projectService.CreateAsync(dto, userId);
				return CreatedAtAction("GetById", "Projects", new { id = project.Id }, project);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = "No se pudo importar el proyecto", message = ex.Message });
			}
		}
	}

}
