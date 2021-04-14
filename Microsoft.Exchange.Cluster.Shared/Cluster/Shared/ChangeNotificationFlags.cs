using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	[Flags]
	public enum ChangeNotificationFlags
	{
		Key = 1,
		Value = 2,
		WatchSubtree = 4
	}
}
