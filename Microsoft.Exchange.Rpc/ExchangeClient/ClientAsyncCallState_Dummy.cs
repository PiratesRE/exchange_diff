using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ClientAsyncCallState_Dummy : ClientAsyncCallState
	{
		public ClientAsyncCallState_Dummy(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr pRpcBindingHandle) : base("Dummy", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = pRpcBindingHandle;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~ClientAsyncCallState_Dummy()
		{
		}

		public unsafe override void InternalBegin()
		{
			<Module>.cli_Async_EcDummyRpc((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer());
		}

		public int End()
		{
			return base.CheckCompletion();
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private IntPtr m_pRpcBindingHandle;
	}
}
