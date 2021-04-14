using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal class EcPoolCreateSessionCompletion : AsyncRpcCompletionBase, IPoolCreateSessionCompletion
	{
		public unsafe EcPoolCreateSessionCompletion(SafeRpcAsyncStateHandle pAsyncState, uint* pcbOut, byte** ppbOut, uint* pcbAuxOut, byte** ppbAuxOut, ushort* rgwBestVersion, ushort versionDelta)
		{
			this.m_pAsyncState = pAsyncState;
			this.m_pcbAuxOut = pcbAuxOut;
			this.m_ppbAuxOut = ppbAuxOut;
			this.m_pcbOut = pcbOut;
			this.m_ppbOut = ppbOut;
			this.m_rgwBestVersion_w0 = *rgwBestVersion;
			this.m_rgwBestVersion_w1 = *(rgwBestVersion + 2L);
			this.m_rgwBestVersion_w2 = *(rgwBestVersion + 4L);
			this.m_versionDelta = versionDelta;
		}

		public unsafe virtual void CompleteAsyncCall(uint sessionHandle, string displayName, uint maximumPolls, uint retryCount, uint retryDelay, ushort sessionId, ArraySegment<byte> auxiliaryOut)
		{
			bool flag = false;
			bool flag2 = true;
			int num = 0;
			byte* ptr = null;
			int num2 = 0;
			byte* ptr2 = null;
			IntPtr intPtr = IntPtr.Zero;
			int num3 = 0;
			IntPtr intPtr2 = IntPtr.Zero;
			sbyte* ptr3 = null;
			sbyte* ptr4 = null;
			tagRPC_BLOCK_HEADER tagRPC_BLOCK_HEADER = 0;
			initblk(ref tagRPC_BLOCK_HEADER + 2, 0, 2L);
			tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER = 0;
			initblk(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 4, 0, 32L);
			if (!this.m_pAsyncState.IsInvalid)
			{
				try
				{
					<Module>.MToUBytesSegment(auxiliaryOut, (int*)(&num2), &ptr2);
					intPtr2 = Marshal.StringToHGlobalAnsi(displayName);
					ptr4 = (sbyte*)intPtr2.ToPointer();
					tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER = sessionHandle;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 4) = (int)maximumPolls;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 8) = (int)retryCount;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 12) = (int)retryDelay;
					DateTime utcNow = DateTime.UtcNow;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 16) = (int)utcNow.ToFileTime();
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 20) = (int)sessionId;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 24) = 3840;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 26) = (short)34265;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 28) = (short)(this.m_versionDelta + 15);
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 30) = (short)this.m_rgwBestVersion_w0;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 32) = (short)this.m_rgwBestVersion_w1;
					*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 34) = (short)this.m_rgwBestVersion_w2;
					num = <Module>.EcPackData<char\u0020*>(null, 0, &num3, ref ptr3);
					if (num == null)
					{
						num = <Module>.EcPackData<char\u0020*>(null, 0, &num3, ref ptr4);
						if (num == null)
						{
							int num4 = (int)(num3 + 40L);
							ptr = <Module>.MIDL_user_allocate((ulong)num4);
							if (null == ptr)
							{
								num = 1008;
							}
							else
							{
								tagRPC_BLOCK_HEADER = num4 - 4;
								*(ref tagRPC_BLOCK_HEADER + 2) = 4;
								num3 = 0;
								num = <Module>.EcPackData<struct\u0020tagRPC_BLOCK_HEADER>(ptr, num4, &num3, ref tagRPC_BLOCK_HEADER);
								if (num == null)
								{
									num = <Module>.EcPackData<struct\u0020tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER>(ptr, num4, &num3, ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER);
									if (num == null)
									{
										num = <Module>.EcPackData<char\u0020*>(ptr, num4, &num3, ref ptr3);
										if (num == null)
										{
											num = <Module>.EcPackData<char\u0020*>(ptr, num4, &num3, ref ptr4);
											if (num == null)
											{
												IntPtr intPtr3 = this.m_pAsyncState.Detach();
												intPtr = intPtr3;
												if (IntPtr.Zero != intPtr3)
												{
													*(int*)this.m_pcbOut = num4;
													*(long*)this.m_ppbOut = ptr;
													ptr = null;
													*(int*)this.m_pcbAuxOut = num2;
													*(long*)this.m_ppbAuxOut = ptr2;
													ptr2 = null;
													<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr.ToPointer(), (void*)(&num));
												}
												flag = true;
											}
										}
									}
								}
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
					if (intPtr2 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr2);
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

		private unsafe uint* m_pcbOut;

		private unsafe byte** m_ppbOut;

		private ushort m_rgwBestVersion_w0;

		private ushort m_rgwBestVersion_w1;

		private ushort m_rgwBestVersion_w2;

		private ushort m_versionDelta;
	}
}
