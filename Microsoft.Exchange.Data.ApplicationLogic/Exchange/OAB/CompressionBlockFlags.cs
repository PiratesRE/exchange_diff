using System;

namespace Microsoft.Exchange.OAB
{
	[Flags]
	internal enum CompressionBlockFlags
	{
		NotCompressed = 0,
		Compressed = 1
	}
}
