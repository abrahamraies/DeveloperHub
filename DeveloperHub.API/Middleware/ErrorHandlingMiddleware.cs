using FluentValidation;
using System.Text.Json;

namespace DeveloperHub.API.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlingMiddleware> _logger;

		public ErrorHandlingMiddleware
		(
			RequestDelegate next,
			ILogger<ErrorHandlingMiddleware> logger
		)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			_logger.LogError(exception, "An unhandled exception has occurred");

			var statusCode = StatusCodes.Status500InternalServerError;
			var message = "An error occurred while processing your request";

			switch (exception)
			{
				case KeyNotFoundException:
					statusCode = StatusCodes.Status404NotFound;
					message = exception.Message;
					break;
				case UnauthorizedAccessException:
					statusCode = StatusCodes.Status401Unauthorized;
					message = exception.Message;
					break;
				case ValidationException:
					statusCode = StatusCodes.Status400BadRequest;
					message = exception.Message;
					break;
				case InvalidOperationException:
					statusCode = StatusCodes.Status409Conflict;
					message = exception.Message;
					break;
			}

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;

			var response = new
			{
				StatusCode = statusCode,
				Message = message,
				Details = exception.Message
			};

			return context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
