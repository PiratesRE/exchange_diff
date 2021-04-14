using System;

namespace Microsoft.Exchange.DxStore.Server
{
	[Flags]
	public enum StoreState
	{
		Unknown = 0,
		Initializing = 1,
		Current = 2,
		Stale = 4,
		Struck = 8,
		CatchingUp = 16,
		NoMajority = 32
	}
}
