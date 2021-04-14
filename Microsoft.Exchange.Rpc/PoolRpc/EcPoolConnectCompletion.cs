using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class EcPoolConnectCompletion : AsyncRpcCompletionBase, IPoolConnectCompletion
	{
		public unsafe EcPoolConnectCompletion(SafeRpcAsyncStateHandle pAsyncState, void** pcpxh, uint* pcbOut, byte** ppbOut, uint* pcbAuxOut, byte** ppbAuxOut)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = pcbAuxOut;
			this.m_ppbAuxOut = ppbAuxOut;
			this.m_pcpxh = pcpxh;
			this.m_pcbOut = pcbOut;
			this.m_ppbOut = ppbOut;
		}

		public unsafe virtual void CompleteAsyncCall(IntPtr contextHandle, uint flags, uint maxPoolSize, Guid poolGuid, ArraySegment<byte> auxiliaryOut)
		{
			bool flag = false;
			bool flag2 = true;
			int num = 0;
			byte* ptr = null;
			int num2 = 0;
			tagRPC_BLOCK_HEADER tagRPC_BLOCK_HEADER = 0;
			initblk(ref tagRPC_BLOCK_HEADER + 2, 0, 2L);
			tagRPC_POOL_CONNECT_RESPONSE_HEADER tagRPC_POOL_CONNECT_RESPONSE_HEADER = 0;
			initblk(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 4, 0, 20L);
			int num3 = 0;
			byte* ptr2 = null;
			IntPtr intPtr = IntPtr.Zero;
			if (!this.m_pAsyncState.IsInvalid)
			{
				try
				{
					<Module>.MToUBytesSegment(auxiliaryOut, (int*)(&num3), &ptr2);
					byte* ptr3 = <Module>.MIDL_user_allocate(28UL);
					if (null == ptr3)
					{
						num = 1008;
					}
					else
					{
						tagRPC_POOL_CONNECT_RESPONSE_HEADER = flags;
						_GUID guid = <Module>.ToGUID(ref poolGuid);
						cpblk(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 8, ref guid, 16);
						*(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 4) = (int)maxPoolSize;
						tagRPC_BLOCK_HEADER = 24;
						*(ref tagRPC_BLOCK_HEADER + 2) = 2;
						num = <Module>.EcPackData<struct\u0020tagRPC_BLOCK_HEADER>(ptr3, 28, &num2, ref tagRPC_BLOCK_HEADER);
						if (num == null)
						{
							num = <Module>.EcPackData<struct\u0020tagRPC_POOL_CONNECT_RESPONSE_HEADER>(ptr3, 28, &num2, ref tagRPC_POOL_CONNECT_RESPONSE_HEADER);
							if (num == null)
							{
								IntPtr intPtr2 = this.m_pAsyncState.Detach();
								intPtr = intPtr2;
								if (IntPtr.Zero != intPtr2)
								{
									*(long*)this.m_pcpxh = contextHandle.ToPointer();
									*(int*)this.m_pcbOut = 28;
									*(long*)this.m_ppbOut = ptr3;
									*(int*)this.m_pcbAuxOut = num3;
									*(long*)this.m_ppbAuxOut = ptr2;
									ptr2 = null;
									<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
								}
								flag = true;
							}
						}
					}
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

		private unsafe void** m_pcpxh;

		private unsafe uint* m_pcbOut;

		private unsafe byte** m_ppbOut;
	}
}
