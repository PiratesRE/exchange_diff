using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiDeleteEntriesDispatchTask : NspiDispatchTask
	{
		public NspiDeleteEntriesDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiDeleteEntriesFlags flags, int mid, byte[][] entryIds) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.mid = mid;
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
				return "NspiDeleteEntries";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiDeleteEntriesFlags, int>((long)base.ContextHandle, "{0} params: flags={1}, mid={2}", this.TaskName, this.flags, this.mid);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			base.NspiContextCallWrapper("DeleteEntries", () => NspiStatus.NotSupported);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiDeleteEntriesDispatchTask>(this);
		}

		private const string Name = "NspiDeleteEntries";

		private readonly NspiDeleteEntriesFlags flags;

		private readonly int mid;
	}
}
