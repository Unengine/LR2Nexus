using System.Security.Cryptography;
using System.Text;

namespace LR2Nexus.Utils
{
	public static class MD5Util
	{
		public static MD5Hash CalculateMD5(object[] dataRaw)
		{
			string concatenatedData = string.Concat(dataRaw.Select(x => x?.ToString() ?? string.Empty));
			return CalculateMD5(concatenatedData);
		}

		public static MD5Hash CalculateMD5(string input)
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			byte[] hashBytes = MD5.HashData(inputBytes);

			StringBuilder sb = new();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("x2"));
			}

			return new MD5Hash { Body = sb.ToString() };
		}
	}
}

namespace LR2Nexus
{
	public readonly struct MD5Hash(string body) : IEquatable<MD5Hash>
	{
		public required string Body { get; init; } = body;
		public override string ToString() => Body;
		public bool Equals(MD5Hash other) => Body == other.Body;
		public override int GetHashCode() => Body.GetHashCode();
		public override bool Equals(object? obj) => obj is MD5Hash other && Equals(other);
		public static bool operator ==(MD5Hash left, MD5Hash right) => left.Equals(right);
		public static bool operator !=(MD5Hash left, MD5Hash right) => !(left == right);
	}
}