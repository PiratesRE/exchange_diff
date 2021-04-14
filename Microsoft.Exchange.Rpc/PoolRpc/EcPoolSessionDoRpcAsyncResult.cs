using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal sealed class EcPoolSessionDoRpcAsyncResult : RpcAsyncResult
	{
		public unsafe EcPoolSessionDoRpcAsyncResult(AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
			try
			{
				EcPoolSessionDoRpcAsyncState* ptr = <Module>.@new(144UL);
				EcPoolSessionDoRpcAsyncState* ptr2;
				try
				{
					if (ptr != null)
					{
						*(int*)(ptr + 112L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0;
						*(int*)(ptr + 116L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0;
						*(long*)(ptr + 120L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0L;
						*(int*)(ptr + 128L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0;
						*(long*)(ptr + 136L / (long)sizeof(EcPoolSessionDoRpcAsyncState)) = 0L;
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

		private unsafe void ~EcPoolSessionDoRpcAsyncResult()
		{
			EcPoolSessionDoRpcAsyncState* pAsyncState = this.m_pAsyncState;
			if (pAsyncState != null)
			{
				<Module>.EcPoolSessionDoRpcAsyncState.__delDtor(pAsyncState, 1U);
			}
			this.m_pAsyncState = null;
		}

		public unsafe int Complete(out uint flags, out ArraySegment<byte> response, out ArraySegment<byte> auxOut)
		{
			if (null == this.m_pAsyncState)
			{
				throw new InvalidOperationException();
			}
			byte[] array = null;
			byte[] array2 = null;
			int num = 0;
			int count = 0;
			int num2 = 0;
			try
			{
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(RpcBufferPool.GetBuffer(0));
				response = arraySegment;
				ArraySegment<byte> arraySegment2 = new ArraySegment<byte>(RpcBufferPool.GetBuffer(0));
				auxOut = arraySegment2;
				flags = 0U;
				int num3 = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.m_pAsyncState, (void*)(&num2));
				if (num3 != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num3, "EcPoolConnect, RpcAsyncCompleteCall");
				}
				EcPoolSessionDoRpcAsyncState* pAsyncState = this.m_pAsyncState;
				uint num4 = (uint)(*(int*)(pAsyncState + 128L / (long)sizeof(EcPoolSessionDoRpcAsyncState)));
				if (num4 > 0U)
				{
					array2 = <Module>.UToMLeasedBuffer((int)num4, *(long*)(pAsyncState + 136L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), ref count);
				}
				if (0 == num2)
				{
					pAsyncState = this.m_pAsyncState;
					uint num5 = (uint)(*(int*)(pAsyncState + 116L / (long)sizeof(EcPoolSessionDoRpcAsyncState)));
					if (num5 > 0U)
					{
						array = <Module>.UToMLeasedBuffer((int)num5, *(long*)(pAsyncState + 120L / (long)sizeof(EcPoolSessionDoRpcAsyncState)), ref num);
					}
					byte condition = (num > 0) ? 1 : 0;
					ExAssert.Assert(condition != 0, "The server returned an empty response buffer");
					flags = (uint)(*(int*)(this.m_pAsyncState + 112L / (long)sizeof(EcPoolSessionDoRpcAsyncState)));
				}
				if (array != null)
				{
					ArraySegment<byte> arraySegment3 = new ArraySegment<byte>(array, 0, num);
					response = arraySegment3;
					array = null;
				}
				if (array2 != null)
				{
					ArraySegment<byte> arraySegment4 = new ArraySegment<byte>(array2, 0, count);
					auxOut = arraySegment4;
					array2 = null;
				}
			}
			finally
			{
				EcPoolSessionDoRpcAsyncState* pAsyncState2 = this.m_pAsyncState;
				if (pAsyncState2 != null)
				{
					<Module>.EcPoolSessionDoRpcAsyncState.__delDtor(pAsyncState2, 1U);
				}
				this.m_pAsyncState = null;
				if (array != null)
				{
					RpcBufferPool.ReleaseBuffer(array);
				}
				if (array2 != null)
				{
					RpcBufferPool.ReleaseBuffer(array2);
				}
			}
			return num2;
		}

		public unsafe EcPoolSessionDoRpcAsyncState* NativeState()
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
					this.~EcPoolSessionDoRpcAsyncResult();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private unsafe EcPoolSessionDoRpcAsyncState* m_pAsyncState;
	}
}
