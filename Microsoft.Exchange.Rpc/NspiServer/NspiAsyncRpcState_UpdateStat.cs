using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_UpdateStat : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_UpdateStat,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiUpdateStatFlags flags, IntPtr pState, IntPtr pDelta)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.pDelta = pDelta;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiUpdateStatFlags.None;
			this.pState = IntPtr.Zero;
			this.pDelta = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			bool deltaRequested = false;
			if (this.pDelta != IntPtr.Zero)
			{
				Marshal.WriteInt32(this.pDelta, 0);
				deltaRequested = true;
			}
			base.AsyncDispatch.BeginUpdateStat(null, this.contextHandle, this.flags, state, deltaRequested, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			NspiState nspiState = null;
			int? num = null;
			int result = (int)base.AsyncDispatch.EndUpdateStat(asyncResult, out nspiState, out num);
			if (this.pState != IntPtr.Zero && nspiState != null)
			{
				nspiState.MarshalToNative(this.pState);
			}
			if (num != null && this.pDelta != IntPtr.Zero)
			{
				Marshal.WriteInt32(this.pDelta, num.Value);
			}
			return result;
		}

		private IntPtr contextHandle;

		private NspiUpdateStatFlags flags;

		private IntPtr pState;

		private IntPtr pDelta;
	}
}
