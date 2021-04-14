using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiResolveNamesDispatchTask : NspiStateDispatchTask
	{
		public NspiResolveNamesDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiResolveNamesFlags flags, NspiState state, PropertyTag[] propertyTags, byte[][] names) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.propertyTags = propertyTags;
			this.names = names;
		}

		public NspiStatus End(out int codePage, out int[] mids, out PropertyValue[][] rowset)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			codePage = this.returnCodePage;
			mids = this.returnMids;
			rowset = this.returnRowset;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiResolveNames";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiResolveNamesFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			PropRowSet mapiRowset = null;
			int[] midList = null;
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			base.NspiContextCallWrapper("ResolveNames", () => this.Context.ResolveNames(this.NspiState, mapiPropTags, this.names, out midList, out mapiRowset));
			if (base.Status == NspiStatus.Success)
			{
				this.returnCodePage = base.NspiState.CodePage;
				this.returnMids = midList;
				this.returnRowset = ConvertHelper.ConvertFromMapiPropRowSet(mapiRowset, this.returnCodePage);
			}
			NspiDispatchTask.NspiTracer.TraceDebug<int>((long)base.ContextHandle, "Rows returned: {0}", (mapiRowset == null) ? 0 : mapiRowset.Rows.Count);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiResolveNamesDispatchTask>(this);
		}

		private const string Name = "NspiResolveNames";

		private readonly NspiResolveNamesFlags flags;

		private readonly PropertyTag[] propertyTags;

		private readonly byte[][] names;

		private int returnCodePage;

		private int[] returnMids;

		private PropertyValue[][] returnRowset;
	}
}
