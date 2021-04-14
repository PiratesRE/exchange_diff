using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_Bind : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_Bind,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr bindingHandle, NspiBindFlags flags, IntPtr pState, IntPtr pServerGuid, IntPtr pContextHandle)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.bindingHandle = bindingHandle;
			this.flags = flags;
			this.pState = pState;
			this.pServerGuid = pServerGuid;
			this.pContextHandle = pContextHandle;
		}

		public override void InternalReset()
		{
			this.bindingHandle = IntPtr.Zero;
			this.flags = NspiBindFlags.None;
			this.pState = IntPtr.Zero;
			this.pServerGuid = IntPtr.Zero;
			this.pContextHandle = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			if (this.pContextHandle != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.pContextHandle, IntPtr.Zero);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			ClientBinding clientBinding = new RpcClientBinding(this.bindingHandle, base.AsyncState);
			Guid? guid = null;
			if (this.pServerGuid != IntPtr.Zero)
			{
				Guid value = NspiHelper.ConvertGuidFromNative(this.pServerGuid);
				Guid? guid2 = new Guid?(value);
				guid = guid2;
			}
			base.AsyncDispatch.BeginBind(null, clientBinding, this.flags, state, guid, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			Guid? guid = null;
			IntPtr zero = IntPtr.Zero;
			int result = (int)base.AsyncDispatch.EndBind(asyncResult, out guid, out zero);
			if (this.pContextHandle != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(this.pContextHandle, zero);
			}
			if (guid != null && this.pServerGuid != IntPtr.Zero)
			{
				Marshal.StructureToPtr(guid.Value, this.pServerGuid, false);
			}
			return result;
		}

		private IntPtr bindingHandle;

		private NspiBindFlags flags;

		private IntPtr pState;

		private IntPtr pServerGuid;

		private IntPtr pContextHandle;
	}
}
