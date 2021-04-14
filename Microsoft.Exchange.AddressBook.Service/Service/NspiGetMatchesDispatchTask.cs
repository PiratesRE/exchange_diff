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
	internal sealed class NspiGetMatchesDispatchTask : NspiStateDispatchTask
	{
		public NspiGetMatchesDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiGetMatchesFlags flags, NspiState state, int[] mids, int interfaceOptions, Restriction restriction, IntPtr pPropName, int maxRows, PropertyTag[] propertyTags) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.interfaceOptions = interfaceOptions;
			this.restriction = restriction;
			this.maxRows = maxRows;
			this.propertyTags = propertyTags;
			if (this.maxRows < 0 || this.maxRows > 100000)
			{
				this.maxRows = 100000;
			}
		}

		public NspiStatus End(out NspiState state, out int[] mids, out PropertyValue[][] rowset)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			state = this.returnState;
			mids = this.returnMids;
			rowset = this.returnRowset;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiGetMatches";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, interfaceOptions={2}, maxRows={3}", new object[]
			{
				this.TaskName,
				this.flags,
				this.interfaceOptions,
				this.maxRows
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			Restriction mapiRestriction = null;
			if (this.restriction != null)
			{
				mapiRestriction = ConvertHelper.ConvertToMapiRestriction(this.restriction);
				if (mapiRestriction == null)
				{
					throw new NspiException(NspiStatus.TooComplex, "Restriction too complex");
				}
			}
			int[] midList = null;
			PropRowSet mapiRowset = null;
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			base.NspiContextCallWrapper("GetMatches", () => this.Context.GetMatches(this.NspiState, mapiRestriction, this.maxRows, mapiPropTags, out midList, out mapiRowset));
			if (base.Status == NspiStatus.Success)
			{
				this.returnState = base.NspiState;
				this.returnMids = midList;
				this.returnRowset = ConvertHelper.ConvertFromMapiPropRowSet(mapiRowset, MarshalHelper.GetString8CodePage(base.NspiState));
				base.TraceNspiState();
				NspiDispatchTask.NspiTracer.TraceDebug<int>((long)base.ContextHandle, "Rows returned: {0}", (mapiRowset == null) ? 0 : mapiRowset.Rows.Count);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetMatchesDispatchTask>(this);
		}

		private const string Name = "NspiGetMatches";

		private const int MaxRows = 100000;

		private readonly NspiGetMatchesFlags flags;

		private readonly int interfaceOptions;

		private readonly Restriction restriction;

		private readonly int maxRows;

		private readonly PropertyTag[] propertyTags;

		private NspiState returnState;

		private int[] returnMids;

		private PropertyValue[][] returnRowset;
	}
}
