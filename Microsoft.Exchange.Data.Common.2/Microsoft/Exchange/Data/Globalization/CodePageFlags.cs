using System;

namespace Microsoft.Exchange.Data.Globalization
{
	[Flags]
	internal enum CodePageFlags : byte
	{
		None = 0,
		Detectable = 1,
		SevenBit = 2
	}
}
