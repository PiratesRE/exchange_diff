using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiUpdateStatDispatchTask : NspiStateDispatchTask
	{
		public NspiUpdateStatDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiUpdateStatFlags flags, NspiState state, bool deltaRequested) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.deltaRequested = deltaRequested;
		}

		public NspiStatus End(out NspiState state, out int? delta)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			state = this.returnState;
			delta = this.returnedDelta;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiUpdateStat";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiUpdateStatFlags, bool>((long)base.ContextHandle, "{0} params: flags={1}, deltaRequested={2}", this.TaskName, this.flags, this.deltaRequested);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			if (!this.deltaRequested)
			{
				base.NspiContextCallWrapper("UpdateStat", () => base.Context.UpdateStat(base.NspiState));
			}
			else
			{
				int localDelta = 0;
				base.NspiContextCallWrapper("UpdateStat", () => this.Context.UpdateStat(this.NspiState, out localDelta));
				if (base.Status == NspiStatus.Success)
				{
					this.returnedDelta = new int?(localDelta);
				}
			}
			if (base.Status == NspiStatus.Success)
			{
				this.returnState = base.NspiState;
				base.TraceNspiState();
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiUpdateStatDispatchTask>(this);
		}

		private const string Name = "NspiUpdateStat";

		private readonly NspiUpdateStatFlags flags;

		private readonly bool deltaRequested;

		private NspiState returnState;

		private int? returnedDelta = null;
	}
}
