using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Nspi;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiQueryRowsDispatchTask : NspiStateDispatchTask
	{
		public NspiQueryRowsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiQueryRowsFlags flags, NspiState state, int[] mids, int rowCount, PropertyTag[] propertyTags) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.mids = mids;
			this.rowCount = rowCount;
			this.propertyTags = propertyTags;
		}

		public NspiStatus End(out NspiState state, out PropertyValue[][] rowset)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			state = this.returnState;
			rowset = this.returnRowset;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiQueryRows";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiQueryRowsFlags, int>((long)base.ContextHandle, "{0} params: flags={1}, rowCount={2}", this.TaskName, this.flags, this.rowCount);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			PropRowSet mapiRowset = null;
			base.NspiContextCallWrapper("QueryRows", () => this.Context.QueryRows(this.flags, this.NspiState, this.mids, this.rowCount, mapiPropTags, out mapiRowset));
			if (base.Status == NspiStatus.Success)
			{
				this.returnState = base.NspiState;
				this.returnRowset = ConvertHelper.ConvertFromMapiPropRowSet(mapiRowset, MarshalHelper.GetString8CodePage(base.NspiState));
				base.TraceNspiState();
				NspiDispatchTask.NspiTracer.TraceDebug<int>((long)base.ContextHandle, "Rows returned: {0}", (mapiRowset == null) ? 0 : mapiRowset.Rows.Count);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiQueryRowsDispatchTask>(this);
		}

		private const string Name = "NspiQueryRows";

		private readonly NspiQueryRowsFlags flags;

		private readonly int[] mids;

		private readonly int rowCount;

		private readonly PropertyTag[] propertyTags;

		private NspiState returnState;

		private PropertyValue[][] returnRowset;
	}
}
