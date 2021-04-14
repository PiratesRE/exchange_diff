using System;

namespace Microsoft.Exchange.Rpc.UnifiedPolicyNotification
{
	internal abstract class UnifiedPolicyNotificationRpcServer : RpcServerBase
	{
		public abstract byte[] Notify(int version, int type, byte[] inputParameterBytes);

		public UnifiedPolicyNotificationRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IUnifiedPolicyNotification_v1_0_s_ifspec;
	}
}
