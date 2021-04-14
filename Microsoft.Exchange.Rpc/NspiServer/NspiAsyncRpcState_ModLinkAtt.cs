using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc.NspiServer
{
	internal class NspiAsyncRpcState_ModLinkAtt : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcState_ModLinkAtt,Microsoft::Exchange::Rpc::NspiServer::NspiAsyncRpcServer,Microsoft::Exchange::Rpc::INspiAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NspiAsyncRpcServer asyncServer, IntPtr contextHandle, NspiModLinkAttFlags flags, int propTag, int mid, IntPtr pEntryIds)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flags = flags;
			this.propTag = propTag;
			this.mid = mid;
			this.pEntryIds = pEntryIds;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flags = NspiModLinkAttFlags.None;
			this.propTag = 0;
			this.mid = 0;
			this.pEntryIds = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			PropertyTag propertyTag = new PropertyTag((uint)this.propTag);
			byte[][] entryIds = NspiHelper.ConvertCountedEntryIdArrayFromNative(this.pEntryIds);
			base.AsyncDispatch.BeginModLinkAtt(null, this.contextHandle, this.flags, propertyTag, this.mid, entryIds, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			return (int)base.AsyncDispatch.EndModLinkAtt(asyncResult);
		}

		private IntPtr contextHandle;

		private NspiModLinkAttFlags flags;

		private int propTag;

		private int mid;

		private IntPtr pEntryIds;
	}
}
