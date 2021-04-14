using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiRetrievePropertyFlags
	{
		None = 0,
		SkipObjects = 1,
		EphemeralIds = 2,
		ValidBits = 3
	}
}
