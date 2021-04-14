using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal enum TokenId : byte
	{
		None,
		EndOfFile,
		Text,
		EncodingChange
	}
}
