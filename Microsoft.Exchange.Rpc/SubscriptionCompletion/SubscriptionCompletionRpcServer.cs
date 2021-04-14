using System;

namespace Microsoft.Exchange.Rpc.SubscriptionCompletion
{
	internal abstract class SubscriptionCompletionRpcServer : RpcServerBase
	{
		public abstract byte[] SubscriptionComplete(int version, byte[] pInBytes);

		public SubscriptionCompletionRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISubscriptionCompletion_v1_0_s_ifspec;
	}
}
