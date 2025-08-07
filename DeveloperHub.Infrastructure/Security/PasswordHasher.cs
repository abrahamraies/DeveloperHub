using DeveloperHub.Application.Interfaces.Security;
using System.Security.Cryptography;

namespace DeveloperHub.Infrastructure.Security
{
	public class PasswordHasher : IPasswordHasher
	{
		private const int SaltSize = 16;
		private const int KeySize = 32;
		private const int Iterations = 10000;

		public string Hash(string password)
		{
			using var rng = RandomNumberGenerator.Create();
			var salt = new byte[SaltSize];
			rng.GetBytes(salt);

			var key = Rfc2898DeriveBytes.Pbkdf2(
				password,
				salt,
				Iterations,
				HashAlgorithmName.SHA256,
				KeySize
			);

			return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
		}

		public bool Verify(string password, string hash)
		{
			var parts = hash.Split('.');
			if (parts.Length != 2)
				return false;

			var salt = Convert.FromBase64String(parts[0]);
			var key = Convert.FromBase64String(parts[1]);

			var newKey = Rfc2898DeriveBytes.Pbkdf2(
				password,
				salt,
				Iterations,
				HashAlgorithmName.SHA256,
				KeySize
			);

			return newKey.SequenceEqual(key);
		}
	}
}
