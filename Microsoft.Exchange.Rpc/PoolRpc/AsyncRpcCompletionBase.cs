using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class AsyncRpcCompletionBase : IRpcAsyncCompletion
	{
		public unsafe AsyncRpcCompletionBase(SafeRpcAsyncStateHandle pAsyncState, uint* pcbAuxOut, byte** ppbAuxOut)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = pcbAuxOut;
			this.m_ppbAuxOut = ppbAuxOut;
		}

		public virtual void AbortAsyncCall(uint exceptionCode)
		{
			this.m_pAsyncState.AbortCall(exceptionCode);
		}

		public unsafe virtual void FailAsyncCall(int errorCode, ArraySegment<byte> auxiliaryOut)
		{
			int num = errorCode;
			int num2 = 0;
			byte* ptr = null;
			IntPtr intPtr = IntPtr.Zero;
			bool flag = true;
			if (!this.m_pAsyncState.IsInvalid)
			{
				try
				{
					if (null != this.m_ppbAuxOut)
					{
						<Module>.MToUBytesSegment(auxiliaryOut, (int*)(&num2), &ptr);
					}
					IntPtr intPtr2 = this.m_pAsyncState.Detach();
					intPtr = intPtr2;
					if (IntPtr.Zero != intPtr2)
					{
						uint* pcbAuxOut = this.m_pcbAuxOut;
						if (null != pcbAuxOut)
						{
							*(int*)pcbAuxOut = num2;
						}
						byte** ppbAuxOut = this.m_ppbAuxOut;
						if (null != ppbAuxOut)
						{
							*(long*)ppbAuxOut = ptr;
							ptr = null;
						}
						<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
					}
					flag = false;
				}
				finally
				{
					if (ptr != null)
					{
						<Module>.MIDL_user_free((void*)ptr);
					}
					if (flag)
					{
						this.AbortAsyncCall(1726U);
					}
				}
			}
		}

		protected SafeRpcAsyncStateHandle m_pAsyncState;

		protected unsafe uint* m_pcbAuxOut;

		protected unsafe byte** m_ppbAuxOut;
	}
}
