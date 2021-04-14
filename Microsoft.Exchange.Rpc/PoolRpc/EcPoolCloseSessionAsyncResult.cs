using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	internal sealed class EcPoolCloseSessionAsyncResult : RpcAsyncResult
	{
		public unsafe EcPoolCloseSessionAsyncResult(AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
			try
			{
				EcPoolCloseSessionAsyncState* ptr = <Module>.@new(112UL);
				EcPoolCloseSessionAsyncState* ptr2;
				if (ptr != null)
				{
					initblk(ptr, 0, 112L);
					ptr2 = ptr;
				}
				else
				{
					ptr2 = null;
				}
				this.m_pAsyncState = ptr2;
				if (null == ptr2)
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

		private unsafe void ~EcPoolCloseSessionAsyncResult()
		{
			<Module>.delete((void*)this.m_pAsyncState);
			this.m_pAsyncState = null;
		}

		public unsafe int Complete()
		{
			if (null == this.m_pAsyncState)
			{
				throw new InvalidOperationException();
			}
			int result;
			try
			{
				int num = 0;
				int num2 = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.m_pAsyncState, (void*)(&num));
				if (num2 != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num2, "EcPoolCloseSession, RpcAsyncCompleteCall");
				}
				result = num;
			}
			finally
			{
				<Module>.delete((void*)this.m_pAsyncState);
				this.m_pAsyncState = null;
			}
			return result;
		}

		public unsafe EcPoolCloseSessionAsyncState* NativeState()
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
					this.~EcPoolCloseSessionAsyncResult();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private unsafe EcPoolCloseSessionAsyncState* m_pAsyncState;
	}
}
