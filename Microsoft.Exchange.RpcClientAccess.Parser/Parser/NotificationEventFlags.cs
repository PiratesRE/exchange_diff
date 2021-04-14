using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum NotificationEventFlags : byte
	{
		None = 0,
		OstAdded = 1,
		OstRemoved = 2,
		RowFound = 4,
		Reconnect = 8
	}
}
