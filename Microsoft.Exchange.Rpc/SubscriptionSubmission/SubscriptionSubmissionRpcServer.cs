using System;

namespace Microsoft.Exchange.Rpc.SubscriptionSubmission
{
	internal abstract class SubscriptionSubmissionRpcServer : RpcServerBase
	{
		public abstract byte[] SubscriptionSubmit(int version, byte[] pInBytes);

		public SubscriptionSubmissionRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISubscriptionSubmission_v1_0_s_ifspec;
	}
}
