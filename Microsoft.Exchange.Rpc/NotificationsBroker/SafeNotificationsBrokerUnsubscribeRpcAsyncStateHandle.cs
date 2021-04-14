using System;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class SafeNotificationsBrokerUnsubscribeRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeNotificationsBrokerUnsubscribeRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeNotificationsBrokerUnsubscribeRpcAsyncStateHandle()
		{
		}
	}
}
