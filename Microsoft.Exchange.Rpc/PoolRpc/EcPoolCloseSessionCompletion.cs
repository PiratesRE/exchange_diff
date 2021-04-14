using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class EcPoolCloseSessionCompletion : AsyncRpcCompletionBase, IPoolCloseSessionCompletion
	{
		public EcPoolCloseSessionCompletion(SafeRpcAsyncStateHandle pAsyncState)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = null;
			this.m_ppbAuxOut = null;
		}

		public unsafe virtual void CompleteAsyncCall()
		{
			if (!this.m_pAsyncState.IsInvalid)
			{
				IntPtr intPtr = this.m_pAsyncState.Detach();
				IntPtr intPtr2 = intPtr;
				if (IntPtr.Zero != intPtr)
				{
					int num = 0;
					<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr2.ToPointer(), (void*)(&num));
				}
			}
		}
	}
}
