using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.VariantConfiguration
{
	public static class RotationHash
	{
		public static int ComputeHash(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new ArgumentNullException("input");
			}
			input = input.ToLowerInvariant();
			byte[] array = Encoding.UTF8.GetBytes(input);
			lock (RotationHash.staticLock)
			{
				array = RotationHash.Sha256.ComputeHash(array);
			}
			uint num = 0U;
			for (int i = 0; i < 4; i++)
			{
				num <<= 8;
				num |= (uint)array[i];
			}
			return (int)(num % 1000U);
		}

		public const int HashCount = 1000;

		private const int IntByteCount = 4;

		private static readonly HashAlgorithm Sha256 = new SHA256CryptoServiceProvider();

		private static object staticLock = new object();
	}
}
