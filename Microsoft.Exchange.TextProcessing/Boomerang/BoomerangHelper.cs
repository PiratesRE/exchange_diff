using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	internal static class BoomerangHelper
	{
		internal static byte XorHashToByte(byte[] bytes)
		{
			if (bytes != null && bytes.Length != 0)
			{
				return bytes.Aggregate(186, (byte current, byte byteValue) => current ^ byteValue);
			}
			return 186;
		}

		internal static byte XorHashToByte(string stringToHash)
		{
			return BoomerangHelper.XorHashToByte(Encoding.ASCII.GetBytes(stringToHash));
		}

		internal static byte[] XorHashToByteArray(byte[] bytes, int resultLength)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			byte[] array = new byte[resultLength];
			for (int i = 0; i < bytes.Length; i++)
			{
				byte[] array2 = array;
				int num = i % resultLength;
				array2[num] ^= bytes[i];
			}
			return array;
		}

		internal static long GetTimeIdentifier()
		{
			return DateTime.UtcNow.ToFileTimeUtc() / 10000000L / 86400L;
		}

		internal static bool IsConsumerMailbox(Guid externalOrganizationId)
		{
			return externalOrganizationId == BoomerangHelper.TemplateTenantExternalDirectoryOrganizationIdGuid;
		}

		private static IEnumerable<string> SplitByTokenSize(string data, int tokenSize)
		{
			for (int index = 0; index < data.Length; index += tokenSize)
			{
				yield return data.Substring(index, tokenSize);
			}
			yield break;
		}

		private const byte InitialXorHashValue = 186;

		private const long FiletimeToSecondsDivisor = 10000000L;

		private const long SecondsPerInterval = 86400L;

		private const string TemplateTenantExternalDirectoryOrganizationId = "84df9e7f-e9f6-40af-b435-aaaaaaaaaaaa";

		private static readonly Guid TemplateTenantExternalDirectoryOrganizationIdGuid = new Guid("84df9e7f-e9f6-40af-b435-aaaaaaaaaaaa");
	}
}
