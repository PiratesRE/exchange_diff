using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal sealed class EcPoolConnectAsyncResult : RpcAsyncResult
	{
		public unsafe EcPoolConnectAsyncResult(AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
			try
			{
				EcPoolConnectAsyncState* ptr = <Module>.@new(152UL);
				EcPoolConnectAsyncState* ptr2;
				try
				{
					if (ptr != null)
					{
						*(long*)(ptr + 112L / (long)sizeof(EcPoolConnectAsyncState)) = 0L;
						*(int*)(ptr + 120L / (long)sizeof(EcPoolConnectAsyncState)) = 0;
						*(long*)(ptr + 128L / (long)sizeof(EcPoolConnectAsyncState)) = 0L;
						*(int*)(ptr + 136L / (long)sizeof(EcPoolConnectAsyncState)) = 0;
						*(long*)(ptr + 144L / (long)sizeof(EcPoolConnectAsyncState)) = 0L;
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

		private unsafe void ~EcPoolConnectAsyncResult()
		{
			EcPoolConnectAsyncState* pAsyncState = this.m_pAsyncState;
			if (pAsyncState != null)
			{
				<Module>.EcPoolConnectAsyncState.__delDtor(pAsyncState, 1U);
			}
			this.m_pAsyncState = null;
		}

		public unsafe int Complete(out IntPtr contextHandle, out uint flags, out uint maxPoolSize, out Guid poolGuid, out ArraySegment<byte> auxOut)
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
				contextHandle = IntPtr.Zero;
				flags = 0U;
				maxPoolSize = 0U;
				poolGuid = Guid.Empty;
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(RpcBufferPool.GetBuffer(0));
				auxOut = arraySegment;
				int num2 = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.m_pAsyncState, (void*)(&num));
				if (num2 != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "EcPoolConnect, RpcAsyncCompleteCall");
				}
				EcPoolConnectAsyncState* pAsyncState = this.m_pAsyncState;
				uint num3 = (uint)(*(int*)(pAsyncState + 136L / (long)sizeof(EcPoolConnectAsyncState)));
				if (num3 > 0U)
				{
					array = <Module>.UToMLeasedBuffer((int)num3, *(long*)(pAsyncState + 144L / (long)sizeof(EcPoolConnectAsyncState)), ref count);
				}
				if (0 == num)
				{
					IntPtr intPtr = new IntPtr(*(long*)(this.m_pAsyncState + 112L / (long)sizeof(EcPoolConnectAsyncState)));
					contextHandle = intPtr;
					tagRPC_POOL_CONNECT_RESPONSE_HEADER tagRPC_POOL_CONNECT_RESPONSE_HEADER = 0;
					initblk(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 4, 0, 20L);
					int num4 = 0;
					pAsyncState = this.m_pAsyncState;
					num = <Module>.EcUnpackSingleRpcBlock<2,struct\u0020tagRPC_POOL_CONNECT_RESPONSE_HEADER>(*(long*)(pAsyncState + 128L / (long)sizeof(EcPoolConnectAsyncState)), *(int*)(pAsyncState + 120L / (long)sizeof(EcPoolConnectAsyncState)), &num4, &tagRPC_POOL_CONNECT_RESPONSE_HEADER);
					if (0 == num)
					{
						flags = tagRPC_POOL_CONNECT_RESPONSE_HEADER;
						maxPoolSize = (uint)(*(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 4));
						Guid guid = <Module>.FromGUID(ref tagRPC_POOL_CONNECT_RESPONSE_HEADER + 8);
						poolGuid = guid;
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
				EcPoolConnectAsyncState* pAsyncState2 = this.m_pAsyncState;
				if (pAsyncState2 != null)
				{
					<Module>.EcPoolConnectAsyncState.__delDtor(pAsyncState2, 1U);
				}
				this.m_pAsyncState = null;
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
			}
			return num;
		}

		public unsafe EcPoolConnectAsyncState* NativeState()
		{
			return this.m_pAsyncState;
		}

		[HandleProcessCorruptedStateExceptions]
		protected sealed override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.~EcPoolConnectAsyncResult();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private unsafe EcPoolConnectAsyncState* m_pAsyncState;
	}
}
