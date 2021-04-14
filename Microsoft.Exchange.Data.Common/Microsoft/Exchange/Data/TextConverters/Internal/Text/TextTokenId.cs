using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal enum TextTokenId : byte
	{
		None,
		EndOfFile,
		Text,
		EncodingChange
	}
}
