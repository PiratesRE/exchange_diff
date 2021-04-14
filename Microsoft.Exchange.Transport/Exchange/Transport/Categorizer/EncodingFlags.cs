using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	[Flags]
	internal enum EncodingFlags
	{
		Preference = 131072,
		Mime = 262144,
		MimeText = 393216,
		MimeHtml = 917504,
		MimeHtmlText = 1441792,
		UUEncode = 2228224,
		UUEncodeBinHex = 131072
	}
}
