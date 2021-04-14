using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ReplicationFlags
	{
		None = 0,
		NowStatus = 1,
		StatusRequest = 2,
		ExpressStatusRequest = 4,
		NewBackfillTimeout = 8,
		OutstandingBackfillTimeout = 16
	}
}
