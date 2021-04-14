using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiResortRestrictionDispatchTask : NspiStateDispatchTask
	{
		public NspiResortRestrictionDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiResortRestrictionFlags flags, NspiState state, int[] mids) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.mids = mids;
		}

		public NspiStatus End(out NspiState state, out int[] mids)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			state = this.returnState;
			mids = this.returnMids;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiResortRestriction";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiResortRestrictionFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			int[] localMids = null;
			base.NspiContextCallWrapper("ResortRestriction", () => this.Context.ResortRestriction(this.NspiState, this.mids, out localMids));
			if (base.Status == NspiStatus.Success)
			{
				this.returnState = base.NspiState;
				this.returnMids = localMids;
				base.TraceNspiState();
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiResortRestrictionDispatchTask>(this);
		}

		private const string Name = "NspiResortRestriction";

		private readonly NspiResortRestrictionFlags flags;

		private readonly int[] mids;

		private NspiState returnState;

		private int[] returnMids;
	}
}
