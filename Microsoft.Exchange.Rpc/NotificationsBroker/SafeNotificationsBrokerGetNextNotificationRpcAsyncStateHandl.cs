using System;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class SafeNotificationsBrokerGetNextNotificationRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNotificationsBrokerGetNextNotificationRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNotificationsBrokerGetNextNotificationRpcAsyncStateHandle()
		{
		}
	}
}
