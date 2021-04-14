using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc.RfriServer
{
	internal class RfriAsyncRpcState_GetNewDSA : BaseAsyncRpcState<Microsoft::Exchange::Rpc::RfriServer::RfriAsyncRpcState_GetNewDSA,Microsoft::Exchange::Rpc::RfriServer::RfriAsyncRpcServer,Microsoft::Exchange::Rpc::IRfriAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, RfriAsyncRpcServer asyncServer, IntPtr bindingHandle, RfriGetNewDSAFlags flags, IntPtr pUserDn, IntPtr ppUnused, IntPtr ppServer)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.bindingHandle = bindingHandle;
			this.flags = flags;
			this.pUserDn = pUserDn;
			this.ppUnused = ppUnused;
			this.ppServer = ppServer;
			this.clientBinding = null;
		}

		public override void InternalReset()
		{
			this.bindingHandle = IntPtr.Zero;
			this.flags = RfriGetNewDSAFlags.None;
			this.pUserDn = IntPtr.Zero;
			this.ppUnused = IntPtr.Zero;
			this.ppServer = IntPtr.Zero;
			this.clientBinding = null;
		}

		public unsafe override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			string userDn = null;
			if (this.ppServer != IntPtr.Zero)
			{
				*(long*)this.ppServer.ToPointer() = 0L;
			}
			if (this.pUserDn != IntPtr.Zero)
			{
				userDn = Marshal.PtrToStringAnsi(this.pUserDn);
			}
			RpcClientBinding rpcClientBinding = new RpcClientBinding(this.bindingHandle, base.AsyncState);
			this.clientBinding = rpcClientBinding;
			base.AsyncDispatch.BeginGetNewDSA(null, rpcClientBinding, this.flags, userDn, asyncCallback, this);
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
				RfriStatus rfriStatus = base.AsyncDispatch.EndGetNewDSA(asyncResult, out @string);
				if (rfriStatus == RfriStatus.Success && this.ppServer != IntPtr.Zero)
				{
					ptr = (byte*)<Module>.StringToUnmanagedMultiByte(@string, 0U);
					*(long*)this.ppServer.ToPointer() = ptr;
				}
				flag = true;
				result = (int)rfriStatus;
			}
			finally
			{
				if (!flag && ptr != null)
				{
					<Module>.FreeString((ushort*)ptr);
					if (this.ppServer != IntPtr.Zero)
					{
						*(long*)this.ppServer.ToPointer() = 0L;
					}
				}
			}
			return result;
		}

		private IntPtr bindingHandle;

		private IntPtr pUserDn;

		private RfriGetNewDSAFlags flags;

		private IntPtr ppUnused;

		private IntPtr ppServer;

		private ClientBinding clientBinding;
	}
}
