using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.SubscriptionNotification
{
	internal abstract class SubscriptionNotificationRpcServer : RpcServerBase
	{
		public abstract byte[] InvokeSubscriptionNotificationEndPoint(int version, byte[] pInBytes);

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool IsRpcConnectionError(int errorCode)
		{
			return errorCode == 1753 || errorCode == 1722 || errorCode == 1727;
		}

		public SubscriptionNotificationRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISubscriptionNotification_v1_0_s_ifspec;
	}
}
