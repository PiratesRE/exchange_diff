using System;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class CryptoUtil
	{
		public static byte[] GetSha1Hash(byte[] inputBytes)
		{
			byte[] result;
			using (SHA1Cng sha1Cng = new SHA1Cng())
			{
				result = sha1Cng.ComputeHash(inputBytes);
			}
			return result;
		}
	}
}
