using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal enum RtfTextRunKind : byte
	{
		Ltrch,
		Rtlch,
		Loch,
		Hich,
		Dbch,
		Max,
		None = 255
	}
}
