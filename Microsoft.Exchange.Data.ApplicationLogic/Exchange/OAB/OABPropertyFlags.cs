using System;

namespace Microsoft.Exchange.OAB
{
	[Flags]
	internal enum OABPropertyFlags
	{
		None = 0,
		ANR = 1,
		RDN = 2,
		Index = 4,
		Truncated = 8
	}
}
