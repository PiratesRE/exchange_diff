using System;

namespace Microsoft.Exchange.Data.Mime
{
	[Flags]
	public enum DecodingFlags
	{
		None = 0,
		Rfc2047 = 1,
		Rfc2231 = 2,
		Jis = 4,
		Utf8 = 8,
		Dbcs = 16,
		AllEncodings = 65535,
		FallbackToRaw = 65536,
		AllowControlCharacters = 131072
	}
}
