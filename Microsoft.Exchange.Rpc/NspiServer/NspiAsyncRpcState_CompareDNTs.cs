using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_CompareDNTs : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_CompareDNTs,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiCompareDNTsFlags flags, IntPtr pState, int mid1, int mid2, IntPtr pResult)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.pState = pState;
			this.mid1 = mid1;
			this.mid2 = mid2;
			this.pResult = pResult;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiCompareDNTsFlags.None;
			this.pState = IntPtr.Zero;
			this.mid1 = 0;
			this.mid2 = 0;
			this.pResult = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			NspiState state = null;
			if (this.pResult != IntPtr.Zero)
			{
				Marshal.WriteInt32(this.pResult, 0);
			}
			if (this.pState != IntPtr.Zero)
			{
				state = new NspiState(this.pState);
			}
			base.AsyncDispatch.BeginCompareDNTs(null, this.contextHandle, this.flags, state, this.mid1, this.mid2, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			int val = 0;
			int result = (int)base.AsyncDispatch.EndCompareDNTs(asyncResult, out val);
			if (this.pResult != IntPtr.Zero)
			{
				Marshal.WriteInt32(this.pResult, val);
			}
			return result;
		}

		private IntPtr contextHandle;

		private NspiCompareDNTsFlags flags;

		private IntPtr pState;

		private int mid1;

		private int mid2;

		private IntPtr pResult;
	}
}
