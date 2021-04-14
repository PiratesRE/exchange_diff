using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal enum TextRunType : ushort
	{
		Invalid,
		Skip = 0,
		Markup = 4096,
		NonSpace = 8192,
		FirstShort = 12288,
		InlineObject = 12288,
		NbSp = 16384,
		Space = 20480,
		Tabulation = 24576,
		NewLine = 28672,
		BlockBoundary = 32768
	}
}
