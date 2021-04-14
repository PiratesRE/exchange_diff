using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal static class HashExtension
	{
		static HashExtension()
		{
			HashExtension.authSalt = new byte[8];
			RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider();
			rngcryptoServiceProvider.GetBytes(HashExtension.authSalt);
		}

		public static bool CompareHash(byte[] a, byte[] b)
		{
			return a.Length == b.Length && Win32.memcmp(a, b, (long)a.Length) == 0;
		}

		public static byte[] GetPasswordHash(byte[] password)
		{
			byte[] hash;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				byte[] array = new byte[password.Length + HashExtension.authSalt.Length];
				try
				{
					Array.Copy(password, array, password.Length);
					Array.Copy(HashExtension.authSalt, 0, array, password.Length, HashExtension.authSalt.Length);
					sha256Cng.ComputeHash(array);
				}
				finally
				{
					Array.Clear(array, 0, array.Length);
				}
				hash = sha256Cng.Hash;
			}
			return hash;
		}

		private static readonly byte[] authSalt;

		public static readonly byte[] InvalidHash = new byte[0];
	}
}
