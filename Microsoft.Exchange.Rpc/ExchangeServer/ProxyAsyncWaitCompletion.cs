using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class ProxyAsyncWaitCompletion : IProxyAsyncWaitCompletion
	{
		public unsafe ProxyAsyncWaitCompletion(SafeRpcAsyncStateHandle pAsyncState, uint* pulFlagsOut)
		{
		}

		public unsafe virtual void CompleteAsyncCall([MarshalAs(UnmanagedType.U1)] bool notificationsPending, int errorCode)
		{
			int num = errorCode;
			IntPtr intPtr = IntPtr.Zero;
			if (!this.m_pAsyncState.IsInvalid)
			{
				IntPtr intPtr2 = this.m_pAsyncState.Detach();
				intPtr = intPtr2;
				if (IntPtr.Zero != intPtr2)
				{
					uint* pulFlagsOut = this.m_pulFlagsOut;
					if (pulFlagsOut != null)
					{
						ProxyAsyncWaitCompletion.OutFlags outFlags = notificationsPending ? ProxyAsyncWaitCompletion.OutFlags.Pending : ProxyAsyncWaitCompletion.OutFlags.None;
						*(int*)pulFlagsOut = (int)outFlags;
						this.m_pulFlagsOut = null;
					}
					<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
				}
			}
		}

		private SafeRpcAsyncStateHandle m_pAsyncState = pAsyncState;

		private unsafe uint* m_pulFlagsOut = pulFlagsOut;

		private enum OutFlags
		{
			None,
			Pending
		}
	}
}
