using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiQueryRowsFlags
	{
		None = 0,
		EphemeralIds = 2
	}
}
