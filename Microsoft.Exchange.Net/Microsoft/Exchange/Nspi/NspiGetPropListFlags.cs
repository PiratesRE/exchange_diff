using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiGetPropListFlags
	{
		None = 0,
		SkipObjects = 1,
		EphemeralIds = 2
	}
}
