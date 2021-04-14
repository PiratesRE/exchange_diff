using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeClient
{
	internal class ClientAsyncCallState_Disconnect : ClientAsyncCallState
	{
		private unsafe void Cleanup()
		{
			if (this.m_pExCXH != IntPtr.Zero)
			{
				if (Marshal.ReadIntPtr(this.m_pExCXH) != IntPtr.Zero)
				{
					<Module>.RpcSsDestroyClientContext((void**)this.m_pExCXH.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pExCXH);
				this.m_pExCXH = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Disconnect(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle) : base("Disconnect", asyncCallback, asyncState)
		{
			try
			{
				this.m_pExCXH = IntPtr.Zero;
				this.isContextHandleValid = true;
				bool flag = false;
				try
				{
					IntPtr pExCXH = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pExCXH = pExCXH;
					Marshal.WriteIntPtr(this.m_pExCXH, contextHandle);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~ClientAsyncCallState_Disconnect()
		{
			this.Cleanup();
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe override void InternalBegin()
		{
			int num;
			try
			{
				<Module>.cli_Async_EcDoDisconnect((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), (void**)this.m_pExCXH.ToPointer());
			}
			catch when (delegate
			{
				// Failed to create a 'catch-when' expression
				num = ((Marshal.GetExceptionCode() == 6) ? 1 : 0);
				endfilter(num != 0);
			})
			{
				this.isContextHandleValid = false;
				base.CompleteSynchronously(null);
			}
		}

		public int End(out IntPtr contextHandle)
		{
			int result;
			try
			{
				if (this.isContextHandleValid)
				{
					base.CheckCompletion();
					IntPtr intPtr = Marshal.ReadIntPtr(this.m_pExCXH);
					contextHandle = intPtr;
				}
				else
				{
					contextHandle = IntPtr.Zero;
				}
				result = 0;
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.Cleanup();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private IntPtr m_pExCXH;

		private bool isContextHandleValid;
	}
}
