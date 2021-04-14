using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_Unbind : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_Unbind,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr pContextHandle, NspiUnbindFlags flags)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.pContextHandle = pContextHandle;
			this.flags = flags;
		}

		public override void InternalReset()
		{
			this.pContextHandle = IntPtr.Zero;
			this.flags = NspiUnbindFlags.None;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			IntPtr contextHandle = IntPtr.Zero;
			if (this.pContextHandle != IntPtr.Zero)
			{
				contextHandle = Marshal.ReadIntPtr(this.pContextHandle);
			}
			base.AsyncDispatch.BeginUnbind(null, contextHandle, this.flags, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			IntPtr zero = IntPtr.Zero;
			int result = (int)base.AsyncDispatch.EndUnbind(asyncResult, out zero);
			if (this.pContextHandle != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.pContextHandle, zero);
			}
			return result;
		}

		private NspiUnbindFlags flags;

		private IntPtr pContextHandle;
	}
}
