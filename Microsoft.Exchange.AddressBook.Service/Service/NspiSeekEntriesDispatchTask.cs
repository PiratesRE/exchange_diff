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
	internal sealed class NspiSeekEntriesDispatchTask : NspiStateDispatchTask
	{
		public NspiSeekEntriesDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] restriction, PropertyTag[] propertyTags) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.target = target;
			this.restriction = restriction;
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
				return "NspiSeekEntries";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiSeekEntriesFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			if (this.target == null)
			{
				NspiDispatchTask.NspiTracer.TraceError((long)base.ContextHandle, "Target is null");
				throw new NspiException(NspiStatus.GeneralFailure, "Target is null");
			}
			PropValue mapiPropValue = ConvertHelper.ConvertToMapiPropValue(this.target.Value);
			PropTag[] mapiPropTags = ConvertHelper.ConvertToMapiPropTags(this.propertyTags);
			PropRowSet mapiRowset = null;
			base.NspiContextCallWrapper("SeekEntries", () => this.Context.SeekEntries(this.NspiState, mapiPropValue, this.restriction, mapiPropTags, out mapiRowset));
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
			return DisposeTracker.Get<NspiSeekEntriesDispatchTask>(this);
		}

		private const string Name = "NspiSeekEntries";

		private readonly NspiSeekEntriesFlags flags;

		private readonly PropertyValue? target;

		private readonly int[] restriction;

		private readonly PropertyTag[] propertyTags;

		private NspiState returnState;

		private PropertyValue[][] returnRowset;
	}
}
