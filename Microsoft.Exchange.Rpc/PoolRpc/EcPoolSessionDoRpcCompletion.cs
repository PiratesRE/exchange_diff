using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class EcPoolSessionDoRpcCompletion : AsyncRpcCompletionBase, IPoolSessionDoRpcCompletion
	{
		public unsafe EcPoolSessionDoRpcCompletion(SafeRpcAsyncStateHandle pAsyncState, uint* pulFlags, uint* pcbOut, byte** ppbOut, uint* pcbAuxOut, byte** ppbAuxOut)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = pcbAuxOut;
			this.m_ppbAuxOut = ppbAuxOut;
			this.m_pulFlags = pulFlags;
			this.m_pcbOut = pcbOut;
			this.m_ppbOut = ppbOut;
		}

		public unsafe virtual void CompleteAsyncCall(uint flags, ArraySegment<byte> response, ArraySegment<byte> auxiliaryOut)
		{
			bool flag = false;
			bool flag2 = true;
			int num = 0;
			int num2 = 0;
			byte* ptr = null;
			int num3 = 0;
			byte* ptr2 = null;
			IntPtr intPtr = IntPtr.Zero;
			if (!this.m_pAsyncState.IsInvalid)
			{
				try
				{
					<Module>.MToUBytesSegment(auxiliaryOut, (int*)(&num3), &ptr2);
					<Module>.MToUBytesSegment(response, (int*)(&num2), &ptr);
					*(int*)this.m_pulFlags = (int)flags;
					IntPtr intPtr2 = this.m_pAsyncState.Detach();
					intPtr = intPtr2;
					if (IntPtr.Zero != intPtr2)
					{
						*(int*)this.m_pulFlags = (int)flags;
						*(int*)this.m_pcbOut = num2;
						*(long*)this.m_ppbOut = ptr;
						ptr = null;
						*(int*)this.m_pcbAuxOut = num3;
						*(long*)this.m_ppbAuxOut = ptr2;
						ptr2 = null;
						<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
					}
					flag = true;
					flag2 = false;
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
					if (flag2)
					{
						this.AbortAsyncCall(1726U);
					}
					else if (!flag)
					{
						int errorCode = (0 != num) ? num : -2147467259;
						this.FailAsyncCall(errorCode, auxiliaryOut);
					}
				}
			}
		}

		private unsafe uint* m_pulFlags;

		private unsafe uint* m_pcbOut;

		private unsafe byte** m_ppbOut;
	}
}
