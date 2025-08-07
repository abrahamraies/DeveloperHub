using System.Security.Claims;

namespace DeveloperHub.API.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid GetUserId(this ClaimsPrincipal user)
		{
			var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
			{
				throw new UnauthorizedAccessException("User ID claim is missing or invalid");
			}

			return userId;
		}
	}
}
