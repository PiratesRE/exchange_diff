using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal enum RtfTokenId
	{
		None,
		EndOfFile,
		Begin,
		End,
		Binary,
		SingleRunLast = 4,
		Keywords,
		Text = 7
	}
}
