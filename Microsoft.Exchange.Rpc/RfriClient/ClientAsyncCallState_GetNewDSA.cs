using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc.RfriClient
{
	internal class ClientAsyncCallState_GetNewDSA : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_szUserDN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_szUserDN);
				this.m_szUserDN = IntPtr.Zero;
			}
			if (this.m_pszServer != IntPtr.Zero)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszServer);
				IntPtr intPtr2 = intPtr;
				if (intPtr != IntPtr.Zero)
				{
					<Module>.MIDL_user_free(intPtr2.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pszServer);
				this.m_pszServer = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_GetNewDSA(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr bindingHandle, RfriGetNewDSAFlags flags, string userDn) : base("GetNewDSA", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = bindingHandle;
				this.m_flags = flags;
				this.m_szUserDN = IntPtr.Zero;
				this.m_pszServer = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr szUserDN = Marshal.StringToHGlobalAnsi(userDn);
					this.m_szUserDN = szUserDN;
					IntPtr pszServer = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pszServer = pszServer;
					Marshal.WriteIntPtr(this.m_pszServer, IntPtr.Zero);
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

		private void ~ClientAsyncCallState_GetNewDSA()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			<Module>.cli_RfrGetNewDSA((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), this.m_flags, (byte*)this.m_szUserDN.ToPointer(), null, (byte**)this.m_pszServer.ToPointer());
		}

		public RfriStatus End(out string server)
		{
			RfriStatus result;
			try
			{
				RfriStatus rfriStatus = (RfriStatus)base.CheckCompletion();
				server = null;
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszServer);
				if (intPtr != IntPtr.Zero)
				{
					server = Marshal.PtrToStringAnsi(intPtr);
				}
				result = rfriStatus;
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

		private IntPtr m_pRpcBindingHandle;

		private RfriGetNewDSAFlags m_flags;

		private IntPtr m_szUserDN;

		private IntPtr m_pszServer;
	}
}
