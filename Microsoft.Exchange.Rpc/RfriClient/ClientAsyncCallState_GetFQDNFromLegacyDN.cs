using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc.RfriClient
{
	internal class ClientAsyncCallState_GetFQDNFromLegacyDN : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_szServerDN != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_szServerDN);
				this.m_szServerDN = IntPtr.Zero;
			}
			if (this.m_pszServerFQDN != IntPtr.Zero)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszServerFQDN);
				IntPtr intPtr2 = intPtr;
				if (intPtr != IntPtr.Zero)
				{
					<Module>.MIDL_user_free(intPtr2.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pszServerFQDN);
				this.m_pszServerFQDN = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_GetFQDNFromLegacyDN(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr bindingHandle, RfriGetFQDNFromLegacyDNFlags flags, string serverDn) : base("GetFQDNFromLegacyDN", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = bindingHandle;
				this.m_flags = flags;
				this.m_szServerDN = IntPtr.Zero;
				this.m_pszServerFQDN = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr szServerDN = Marshal.StringToHGlobalAnsi(serverDn);
					this.m_szServerDN = szServerDN;
					IntPtr pszServerFQDN = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pszServerFQDN = pszServerFQDN;
					Marshal.WriteIntPtr(this.m_pszServerFQDN, IntPtr.Zero);
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

		private void ~ClientAsyncCallState_GetFQDNFromLegacyDN()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			void* ptr = this.m_szServerDN.ToPointer();
			void* ptr2 = ptr;
			if (*(sbyte*)ptr != 0)
			{
				do
				{
					ptr2 = (void*)((byte*)ptr2 + 1L);
				}
				while (*(sbyte*)ptr2 != 0);
			}
			long num = (long)(ptr2 - ptr);
			<Module>.cli_RfrGetFQDNFromLegacyDN((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), this.m_flags, (uint)(num + 1L), (byte*)this.m_szServerDN.ToPointer(), (byte**)this.m_pszServerFQDN.ToPointer());
		}

		public RfriStatus End(out string serverFqdn)
		{
			RfriStatus result;
			try
			{
				RfriStatus rfriStatus = (RfriStatus)base.CheckCompletion();
				serverFqdn = null;
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszServerFQDN);
				if (intPtr != IntPtr.Zero)
				{
					serverFqdn = Marshal.PtrToStringAnsi(intPtr);
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

		private RfriGetFQDNFromLegacyDNFlags m_flags;

		private IntPtr m_szServerDN;

		private IntPtr m_pszServerFQDN;
	}
}
