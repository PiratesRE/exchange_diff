using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum NotificationListenerState
	{
		Idle,
		Connecting,
		ConnectingForDeletedNofications,
		Listening,
		Disconnecting
	}
}
