using System;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class SafeNotificationsBrokerSubscribeRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNotificationsBrokerSubscribeRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNotificationsBrokerSubscribeRpcAsyncStateHandle()
		{
		}
	}
}
