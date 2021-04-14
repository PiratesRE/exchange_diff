using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiCompareDNTsDispatchTask : NspiStateDispatchTask
	{
		public NspiCompareDNTsDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiCompareDNTsFlags flags, NspiState state, int mid1, int mid2) : base(asyncCallback, asyncState, protocolRequestInfo, context, state)
		{
			this.flags = flags;
			this.mid1 = mid1;
			this.mid2 = mid2;
		}

		public NspiStatus End(out int result)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			result = this.result;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiCompareDNTs";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, mid1={2}, mid2={3}", new object[]
			{
				this.TaskName,
				this.flags,
				this.mid1,
				this.mid2
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			int localResult = 0;
			base.NspiContextCallWrapper("CompareMids", () => this.Context.CompareMids(this.NspiState, this.mid1, this.mid2, out localResult));
			if (base.Status == NspiStatus.Success)
			{
				this.result = localResult;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiCompareDNTsDispatchTask>(this);
		}

		private const string Name = "NspiCompareDNTs";

		private readonly NspiCompareDNTsFlags flags;

		private readonly int mid1;

		private readonly int mid2;

		private int result;
	}
}
