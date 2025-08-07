using DeveloperHub.API.Middleware;

namespace DeveloperHub.API.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlingMiddleware>();
			return app;
		}
	}
}
