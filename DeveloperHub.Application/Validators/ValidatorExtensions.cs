using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperHub.Application.Validators
{
	public static class ValidatorExtensions
	{
		public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services)
		{
			var assembly = typeof(T).Assembly;

			var validatorTypes = assembly.GetExportedTypes()
				.Where(t => t.GetInterfaces()
					.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

			foreach (var validatorType in validatorTypes)
			{
				var validatorInterface = validatorType.GetInterfaces()
					.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

				services.AddScoped(validatorInterface, validatorType);
			}

			return services;
		}
	}
}
