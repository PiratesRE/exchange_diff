using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.SubscriptionNotification
{
	internal class SubscriptionNotificationRpcClient : RpcClientBase
	{
		public SubscriptionNotificationRpcClient(string machineName, NetworkCredential nc) : base(machineName, nc)
		{
		}

		public SubscriptionNotificationRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual byte[] InvokeSubscriptionNotificationEndPoint(int version, byte[] inBlob)
		{
			byte[] result = null;
			byte* ptr = null;
			byte* ptr2 = null;
			int cBytes = 0;
			try
			{
				int num = 0;
				ptr = <Module>.MToUBytesClient(inBlob, &num);
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_InvokeSubscriptionNotificationEndPoint(base.BindingHandle, version, num, ptr, &cBytes, &ptr2);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "SubscriptionNotification");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr2);
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}

		public static int RpcServerTooBusy = 1723;
	}
}
