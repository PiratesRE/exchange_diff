using System;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiModLinkAttDispatchTask : NspiDispatchTask
	{
		public NspiModLinkAttDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiModLinkAttFlags flags, PropertyTag propertyTag, int mid, byte[][] rawEntryIds) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.propertyTag = propertyTag;
			this.mid = mid;
			this.rawEntryIds = rawEntryIds;
		}

		public NspiStatus End()
		{
			base.CheckDisposed();
			base.CheckCompletion();
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiModLinkAtt";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, propertyTag={2}, mid={3}", new object[]
			{
				this.TaskName,
				this.flags,
				this.propertyTag,
				this.mid
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			EntryId[] entryIds = ConvertHelper.ConvertToMapiEntryIds(this.rawEntryIds);
			PropTag mapiPropTag = (PropTag)this.propertyTag;
			base.NspiContextCallWrapper("ModLinkAtt", () => this.Context.ModLinkAtt(this.flags, mapiPropTag, this.mid, entryIds));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiModLinkAttDispatchTask>(this);
		}

		private const string Name = "NspiModLinkAtt";

		private readonly NspiModLinkAttFlags flags;

		private readonly PropertyTag propertyTag;

		private readonly int mid;

		private readonly byte[][] rawEntryIds;
	}
}
