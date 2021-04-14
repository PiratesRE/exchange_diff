using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal sealed class EcPoolCreateSessionAsyncResult : RpcAsyncResult
	{
		public unsafe EcPoolCreateSessionAsyncResult(AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
			try
			{
				EcPoolCreateSessionAsyncState* ptr = <Module>.@new(144UL);
				EcPoolCreateSessionAsyncState* ptr2;
				try
				{
					if (ptr != null)
					{
						*(int*)(ptr + 112L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 0;
						*(long*)(ptr + 120L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 0L;
						*(int*)(ptr + 128L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 0;
						*(long*)(ptr + 136L / (long)sizeof(EcPoolCreateSessionAsyncState)) = 0L;
						ptr2 = ptr;
					}
					else
					{
						ptr2 = 0L;
					}
				}
				catch
				{
					<Module>.delete((void*)ptr);
					throw;
				}
				this.m_pAsyncState = ptr2;
				if (0L == ptr2)
				{
					throw new OutOfMemoryException();
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private unsafe void ~EcPoolCreateSessionAsyncResult()
		{
			EcPoolCreateSessionAsyncState* pAsyncState = this.m_pAsyncState;
			if (pAsyncState != null)
			{
				<Module>.EcPoolCreateSessionAsyncState.__delDtor(pAsyncState, 1U);
			}
			this.m_pAsyncState = null;
		}

		public unsafe int Complete(out uint sessionHandle, out string displayName, out uint maximumPolls, out uint retryCount, out uint retryDelay, out uint timeStamp, out short[] serverVersion, out short[] bestVersion, out ArraySegment<byte> auxOut)
		{
			if (null == this.m_pAsyncState)
			{
				throw new InvalidOperationException();
			}
			byte[] array = null;
			int count = 0;
			int num = 0;
			try
			{
				sessionHandle = 0U;
				maximumPolls = 0U;
				retryCount = 0U;
				retryDelay = 0U;
				timeStamp = 0U;
				displayName = null;
				serverVersion = null;
				bestVersion = null;
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(RpcBufferPool.GetBuffer(0));
				auxOut = arraySegment;
				int num2 = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.m_pAsyncState, (void*)(&num));
				if (num2 != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "EcPoolCreateSession, RpcAsyncCompleteCall");
				}
				EcPoolCreateSessionAsyncState* pAsyncState = this.m_pAsyncState;
				uint num3 = (uint)(*(int*)(pAsyncState + 128L / (long)sizeof(EcPoolCreateSessionAsyncState)));
				if (num3 > 0U)
				{
					array = <Module>.UToMLeasedBuffer((int)num3, *(long*)(pAsyncState + 136L / (long)sizeof(EcPoolCreateSessionAsyncState)), ref count);
				}
				if (0 == num)
				{
					tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER = 0;
					initblk(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 4, 0, 32L);
					sbyte* value = null;
					pAsyncState = this.m_pAsyncState;
					num = this.EcUnpackSessionResponse(*(long*)(pAsyncState + 120L / (long)sizeof(EcPoolCreateSessionAsyncState)), *(int*)(pAsyncState + 112L / (long)sizeof(EcPoolCreateSessionAsyncState)), &tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER, &value);
					if (0 == num)
					{
						sessionHandle = tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER;
						maximumPolls = (uint)(*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 4));
						retryCount = (uint)(*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 8));
						retryDelay = (uint)(*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 12));
						timeStamp = (uint)(*(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 16));
						serverVersion = this.GetVersion(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 24);
						bestVersion = this.GetVersion(ref tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER + 30);
						IntPtr ptr = new IntPtr((void*)value);
						displayName = Marshal.PtrToStringAnsi(ptr);
					}
				}
				if (array != null)
				{
					ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(array, 0, count);
					auxOut = arraySegment2;
					array = null;
				}
			}
			finally
			{
				EcPoolCreateSessionAsyncState* pAsyncState2 = this.m_pAsyncState;
				if (pAsyncState2 != null)
				{
					<Module>.EcPoolCreateSessionAsyncState.__delDtor(pAsyncState2, 1U);
				}
				this.m_pAsyncState = null;
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
			}
			return num;
		}

		public unsafe EcPoolCreateSessionAsyncState* NativeState()
		{
			return this.m_pAsyncState;
		}

		private unsafe int EcUnpackSessionResponse(byte* pb, uint cb, tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER* pheader, sbyte** pszDisplayName)
		{
			int num = 0;
			tagRPC_BLOCK_HEADER tagRPC_BLOCK_HEADER = 0;
			initblk(ref tagRPC_BLOCK_HEADER + 2, 0, 2L);
			sbyte* ptr = null;
			sbyte* ptr2 = null;
			int num2;
			if (pb != null && cb > 0)
			{
				if (0 < cb)
				{
					do
					{
						num2 = <Module>.EcUnpackData<struct\u0020tagRPC_BLOCK_HEADER>(pb, cb, &num, &tagRPC_BLOCK_HEADER);
						if (num2 != null)
						{
							return num2;
						}
						if (cb - num < tagRPC_BLOCK_HEADER)
						{
							break;
						}
						if (4 == *(ref tagRPC_BLOCK_HEADER + 2))
						{
							goto IL_55;
						}
						num += tagRPC_BLOCK_HEADER;
					}
					while (num < cb);
					goto IL_99;
					IL_55:
					byte* ptr3 = num + pb;
					int num3 = 0;
					num2 = <Module>.EcUnpackData<struct\u0020tagRPC_POOL_CREATE_SESSION_RESPONSE_HEADER>(ptr3, tagRPC_BLOCK_HEADER, &num3, pheader);
					if (num2 != null)
					{
						return num2;
					}
					num2 = <Module>.EcUnpackData<char\u0020*>(ptr3, tagRPC_BLOCK_HEADER, &num3, &ptr);
					if (num2 != null)
					{
						return num2;
					}
					num2 = <Module>.EcUnpackData<char\u0020*>(ptr3, tagRPC_BLOCK_HEADER, &num3, &ptr2);
					if (num2 == null && pszDisplayName != null)
					{
						*(long*)pszDisplayName = ptr2;
						return num2;
					}
					return num2;
				}
				IL_99:
				num2 = 1206;
			}
			else
			{
				num2 = -2147221227;
			}
			return num2;
		}

		private unsafe short[] GetVersion(ushort* rgVersion)
		{
			return new short[]
			{
				(short)(*rgVersion),
				(short)rgVersion[2L / 2L],
				(short)rgVersion[4L / 2L]
			};
		}

		[HandleProcessCorruptedStateExceptions]
		protected sealed override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.~EcPoolCreateSessionAsyncResult();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private unsafe EcPoolCreateSessionAsyncState* m_pAsyncState;
	}
}
