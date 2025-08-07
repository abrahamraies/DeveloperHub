using DeveloperHub.Domain.Entities;

namespace DeveloperHub.Application.Interfaces
{
	public interface IJwtService
	{
		string GenerateToken(User user);
	}
}
