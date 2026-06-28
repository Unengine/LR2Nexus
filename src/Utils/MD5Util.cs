
using System.Security.Cryptography;
using System.Text;

namespace LR2Nexus.Utils
{
	public static class MD5Util
	{
		public static string CalculateMD5(object[] dataRaw)
		{
			string concatenatedData = string.Concat(dataRaw.Select(x => x?.ToString() ?? string.Empty));
			byte[] dataBytes = Encoding.UTF8.GetBytes(concatenatedData);
			byte[] hashBytes = MD5.HashData(dataBytes);

			StringBuilder sb = new();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}

		public static string CalculateMD5(string input)
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			byte[] hashBytes = MD5.HashData(inputBytes);

			StringBuilder sb = new();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("x2"));
			}
			return sb.ToString();
		}
	}
}
