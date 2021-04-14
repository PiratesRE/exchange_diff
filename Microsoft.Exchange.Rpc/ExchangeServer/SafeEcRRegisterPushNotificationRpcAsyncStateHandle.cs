using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class SafeEcRRegisterPushNotificationRpcAsyncStateHandle : SafeRpcAsyncStateHandle
	{
		public SafeEcRRegisterPushNotificationRpcAsyncStateHandle(IntPtr handle) : base(handle)
		{
		}

		public SafeEcRRegisterPushNotificationRpcAsyncStateHandle()
		{
		}
	}
}
