using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EventSubtreeFlag
	{
		NonIPMSubtree = 1,
		IPMSubtree = 2,
		All = 3
	}
}
