using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class OABFileHash
	{
		public static string GetHash(Stream stream)
		{
			string result;
			using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
			{
				using (NoCloseStream noCloseStream = new NoCloseStream(stream))
				{
					stream.Seek(0L, SeekOrigin.Begin);
					byte[] value = sha1CryptoServiceProvider.ComputeHash(noCloseStream);
					result = BitConverter.ToString(value).Replace("-", string.Empty);
				}
			}
			return result;
		}
	}
}
