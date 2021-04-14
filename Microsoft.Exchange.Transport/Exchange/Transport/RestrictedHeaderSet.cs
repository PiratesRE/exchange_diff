using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum RestrictedHeaderSet
	{
		None = 0,
		Organization = 1,
		Forest = 2,
		MTA = 4,
		All = -1
	}
}
