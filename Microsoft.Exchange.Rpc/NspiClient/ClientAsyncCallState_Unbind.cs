using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Rpc.NspiClient
{
	internal class ClientAsyncCallState_Unbind : ClientAsyncCallState
	{
		private unsafe void Cleanup()
		{
			if (this.m_pContextHandle != IntPtr.Zero)
			{
				if (Marshal.ReadIntPtr(this.m_pContextHandle) != IntPtr.Zero)
				{
					<Module>.RpcSsDestroyClientContext((void**)this.m_pContextHandle.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pContextHandle);
				this.m_pContextHandle = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Unbind(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr contextHandle, NspiUnbindFlags flags) : base("Unbind", asyncCallback, asyncState)
		{
			try
			{
				this.m_pContextHandle = IntPtr.Zero;
				this.m_flags = flags;
				this.isContextHandleValid = true;
				bool flag = false;
				try
				{
					IntPtr pContextHandle = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pContextHandle = pContextHandle;
					Marshal.WriteIntPtr(this.m_pContextHandle, contextHandle);
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

		private void ~ClientAsyncCallState_Unbind()
		{
			this.Cleanup();
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe override void InternalBegin()
		{
			int num;
			try
			{
				<Module>.cli_NspiUnbind((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), (void**)this.m_pContextHandle.ToPointer(), this.m_flags);
			}
			catch when (delegate
			{
				// Failed to create a 'catch-when' expression
				num = ((Marshal.GetExceptionCode() == 6) ? 1 : 0);
				endfilter(num != 0);
			})
			{
				this.isContextHandleValid = false;
				base.Completion();
			}
		}

		public NspiStatus End(out IntPtr contextHandle)
		{
			NspiStatus result;
			try
			{
				NspiStatus nspiStatus = NspiStatus.Success;
				if (this.isContextHandleValid)
				{
					nspiStatus = (NspiStatus)base.CheckCompletion();
					IntPtr intPtr = Marshal.ReadIntPtr(this.m_pContextHandle);
					contextHandle = intPtr;
				}
				else
				{
					contextHandle = IntPtr.Zero;
				}
				result = nspiStatus;
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

		private IntPtr m_pContextHandle;

		private NspiUnbindFlags m_flags;

		private bool isContextHandleValid;
	}
}
