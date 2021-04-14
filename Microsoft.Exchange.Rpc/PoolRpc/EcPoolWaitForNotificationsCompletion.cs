using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class EcPoolWaitForNotificationsCompletion : AsyncRpcCompletionBase, IPoolWaitForNotificationsCompletion
	{
		public unsafe EcPoolWaitForNotificationsCompletion(SafeRpcAsyncStateHandle pAsyncState, uint* pcbOut, byte** ppbOut)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = null;
			this.m_ppbAuxOut = null;
			this.m_pcbOut = pcbOut;
			this.m_ppbOut = ppbOut;
		}

		public unsafe virtual void CompleteAsyncCall(uint[] sessionHandles)
		{
			bool flag = false;
			bool flag2 = true;
			int num = 0;
			byte* ptr = null;
			int num2 = 0;
			tagRPC_BLOCK_HEADER tagRPC_BLOCK_HEADER = 0;
			initblk(ref tagRPC_BLOCK_HEADER + 2, 0, 2L);
			tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER = 0;
			initblk(ref tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER + 4, 0, 4L);
			IntPtr intPtr = IntPtr.Zero;
			if (!this.m_pAsyncState.IsInvalid)
			{
				try
				{
					tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER = 0;
					int num3;
					if (sessionHandles != null)
					{
						num3 = sessionHandles.Length;
					}
					else
					{
						num3 = 0;
					}
					*(ref tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER + 4) = num3;
					if (16381 < num3)
					{
						throw new ArgumentException("Too many session handles.", "sessionHandles");
					}
					*(ref tagRPC_BLOCK_HEADER + 2) = 5;
					tagRPC_BLOCK_HEADER = (uint)(((ulong)num3 + 2UL) * 4UL);
					int num4 = (int)(tagRPC_BLOCK_HEADER + 4UL);
					byte* ptr2 = <Module>.MIDL_user_allocate((ulong)num4);
					if (null == ptr2)
					{
						num = 1008;
					}
					else
					{
						num = <Module>.EcPackData<struct\u0020tagRPC_BLOCK_HEADER>(ptr2, num4, &num2, ref tagRPC_BLOCK_HEADER);
						if (num == null)
						{
							num = <Module>.EcPackData<struct\u0020tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER>(ptr2, num4, &num2, ref tagRPC_POOL_WAIT_FOR_NOTIFICATIONS_ASYNC_RESPONSE_HEADER);
							if (num == null)
							{
								if (sessionHandles != null && sessionHandles.Length > 0)
								{
									foreach (uint num5 in sessionHandles)
									{
										num = <Module>.EcPackData<unsigned\u0020long>(ptr2, num4, &num2, ref num5);
										if (num != null)
										{
											goto IL_138;
										}
									}
								}
								IntPtr intPtr2 = this.m_pAsyncState.Detach();
								intPtr = intPtr2;
								if (IntPtr.Zero != intPtr2)
								{
									*(int*)this.m_pcbOut = num4;
									*(long*)this.m_ppbOut = ptr2;
									<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
								}
								flag = true;
							}
						}
					}
					IL_138:
					flag2 = false;
				}
				finally
				{
					if (ptr != null)
					{
						<Module>.MIDL_user_free((void*)ptr);
					}
					if (flag2)
					{
						this.AbortAsyncCall(1726U);
					}
					else if (!flag)
					{
						int errorCode = (0 != num) ? num : -2147467259;
						this.FailAsyncCall(errorCode, RpcServerBase.EmptyArraySegment);
					}
				}
			}
		}

		private unsafe void** m_pcpxh;

		private unsafe uint* m_pcbOut;

		private unsafe byte** m_ppbOut;
	}
}
