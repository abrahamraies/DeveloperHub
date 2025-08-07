using DeveloperHub.Domain.Exceptions;

namespace DeveloperHub.Domain.Guards;
public static class Guard
{
	public static class Against
	{
		public static string NullOrWhiteSpace(string value, string parameterName, string? message = null)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new DomainException(message ??
					$"Required parameter '{parameterName}' cannot be null or whitespace.");

			return value;
		}
	}
}