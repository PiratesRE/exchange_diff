using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncWatermarkElementEncoder
	{
		public string Encode(string elementToEncode)
		{
			SyncUtilities.ThrowIfArgumentNull("elementToEncode", elementToEncode);
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				elementToEncode.Length,
				SyncWatermarkElementEncoder.Delimiter,
				elementToEncode
			});
		}

		public bool TryDecodeElementFrom(string toDecode, int offset, out string decodedElement, out int charactersConsumed)
		{
			SyncUtilities.ThrowIfArgumentNull("toDecode", toDecode);
			SyncUtilities.ThrowIfArgumentLessThanZero("offset", offset);
			decodedElement = null;
			charactersConsumed = 0;
			int num = toDecode.IndexOf(SyncWatermarkElementEncoder.Delimiter, offset);
			if (num == -1)
			{
				return false;
			}
			string s = toDecode.Substring(offset, num - offset);
			int num2;
			if (!int.TryParse(s, out num2))
			{
				return false;
			}
			int num3 = num + 1;
			int num4 = num3 + num2;
			if (num4 > toDecode.Length)
			{
				return false;
			}
			charactersConsumed = num4 - offset;
			decodedElement = toDecode.Substring(num3, num2);
			return true;
		}

		internal static readonly char Delimiter = '|';
	}
}
