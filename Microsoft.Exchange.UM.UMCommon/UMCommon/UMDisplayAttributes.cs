using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Flags]
	internal enum UMDisplayAttributes
	{
		None = 0,
		ZeroTrailingSpaces = 2,
		OneTrailingSpace = 4,
		TwoTrailingSpaces = 8,
		ConsumeLeadingSpaces = 16
	}
}
