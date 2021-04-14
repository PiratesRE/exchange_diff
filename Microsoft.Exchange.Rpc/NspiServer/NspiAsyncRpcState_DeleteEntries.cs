using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_DeleteEntries : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_DeleteEntries,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiDeleteEntriesFlags flags, int mid, IntPtr pEntryIds)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.mid = mid;
			this.pEntryIds = pEntryIds;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiDeleteEntriesFlags.None;
			this.mid = 0;
			this.pEntryIds = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			byte[][] entryIds = NspiHelper.ConvertCountedEntryIdArrayFromNative(this.pEntryIds);
			base.AsyncDispatch.BeginDeleteEntries(null, this.contextHandle, this.flags, this.mid, entryIds, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			return (int)base.AsyncDispatch.EndDeleteEntries(asyncResult);
		}

		private IntPtr contextHandle;

		private NspiDeleteEntriesFlags flags;

		private int mid;

		private IntPtr pEntryIds;
	}
}
