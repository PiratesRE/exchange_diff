using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc.RfriServer
{
	internal class RfriAsyncRpcState_GetFQDNFromLegacyDN : BaseAsyncRpcState<Microsoft::Exchange::Rpc::RfriServer::RfriAsyncRpcState_GetFQDNFromLegacyDN,Microsoft::Exchange::Rpc::RfriServer::RfriAsyncRpcServer,Microsoft::Exchange::Rpc::IRfriAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, RfriAsyncRpcServer asyncServer, IntPtr bindingHandle, RfriGetFQDNFromLegacyDNFlags flags, uint cbServerDn, IntPtr pServerDn, IntPtr ppServerFqdn)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.bindingHandle = bindingHandle;
			this.flags = flags;
			this.cbServerDn = cbServerDn;
			this.pServerDn = pServerDn;
			this.ppServerFqdn = ppServerFqdn;
			this.clientBinding = null;
		}

		public override void InternalReset()
		{
			this.bindingHandle = IntPtr.Zero;
			this.flags = RfriGetFQDNFromLegacyDNFlags.None;
			this.cbServerDn = 0U;
			this.pServerDn = IntPtr.Zero;
			this.ppServerFqdn = IntPtr.Zero;
			this.clientBinding = null;
		}

		public unsafe override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			string serverDn = null;
			if (this.ppServerFqdn != IntPtr.Zero)
			{
				*(long*)this.ppServerFqdn.ToPointer() = 0L;
			}
			if (this.pServerDn != IntPtr.Zero)
			{
				uint num = this.cbServerDn;
				if (num > 0U)
				{
					serverDn = Marshal.PtrToStringAnsi(this.pServerDn, (int)(num - 1U));
				}
			}
			RpcClientBinding rpcClientBinding = new RpcClientBinding(this.bindingHandle, base.AsyncState);
			this.clientBinding = rpcClientBinding;
			base.AsyncDispatch.BeginGetFQDNFromLegacyDN(null, rpcClientBinding, this.flags, serverDn, asyncCallback, this);
		}

		public unsafe override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			string @string = null;
			bool flag = false;
			byte* ptr = null;
			int result;
			try
			{
				@string = null;
				RfriStatus rfriStatus = base.AsyncDispatch.EndGetFQDNFromLegacyDN(asyncResult, out @string);
				if (rfriStatus == RfriStatus.Success && this.ppServerFqdn != IntPtr.Zero)
				{
					ptr = (byte*)<Module>.StringToUnmanagedMultiByte(@string, 0U);
					*(long*)this.ppServerFqdn.ToPointer() = ptr;
				}
				flag = true;
				result = (int)rfriStatus;
			}
			finally
			{
				if (!flag && ptr != null)
				{
					<Module>.FreeString((ushort*)ptr);
					if (this.ppServerFqdn != IntPtr.Zero)
					{
						*(long*)this.ppServerFqdn.ToPointer() = 0L;
					}
				}
			}
			return result;
		}

		private IntPtr bindingHandle;

		private RfriGetFQDNFromLegacyDNFlags flags;

		private uint cbServerDn;

		private IntPtr pServerDn;

		private IntPtr ppServerFqdn;

		private ClientBinding clientBinding;
	}
}
