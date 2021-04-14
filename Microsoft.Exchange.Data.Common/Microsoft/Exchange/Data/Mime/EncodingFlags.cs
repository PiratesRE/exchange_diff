using System;

namespace Microsoft.Exchange.Data.Mime
{
	[Flags]
	public enum EncodingFlags : byte
	{
		None = 0,
		ForceReencode = 1,
		EnableRfc2231 = 2,
		QuoteDisplayNameBeforeRfc2047Encoding = 4,
		AllowUTF8 = 8
	}
}
