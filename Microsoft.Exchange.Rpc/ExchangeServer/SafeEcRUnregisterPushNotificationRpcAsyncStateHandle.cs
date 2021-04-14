using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcRUnregisterPushNotificationRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcRUnregisterPushNotificationRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcRUnregisterPushNotificationRpcAsyncStateHandle()
		{
		}
	}
}
