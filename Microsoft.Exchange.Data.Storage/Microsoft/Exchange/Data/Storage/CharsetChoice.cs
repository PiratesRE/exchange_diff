using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum CharsetChoice
	{
		AlwaysUtf8 = 1,
		AutoDetect,
		UserCharset
	}
}
