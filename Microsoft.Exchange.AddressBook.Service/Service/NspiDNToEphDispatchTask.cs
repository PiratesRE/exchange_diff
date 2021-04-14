using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class NspiDNToEphDispatchTask : NspiDispatchTask
	{
		public NspiDNToEphDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiDNToEphFlags flags, string[] legacyDNs) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.flags = flags;
			this.legacyDNs = legacyDNs;
		}

		public NspiStatus End(out int[] mids)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			mids = this.returnMids;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "NspiDNToEph";
			}
		}

		protected override void InternalDebugTrace()
		{
			NspiDispatchTask.NspiTracer.TraceDebug<string, NspiDNToEphFlags>((long)base.ContextHandle, "{0} params: flags={1}", this.TaskName, this.flags);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			if (this.legacyDNs != null && this.legacyDNs.Length > 100000)
			{
				throw new NspiException(NspiStatus.TooBig, "Too many legacyDN values");
			}
			int[] localMids = null;
			base.NspiContextCallWrapper("DNToEph", () => this.Context.DNToEph(this.legacyDNs, out localMids));
			if (base.Status == NspiStatus.Success)
			{
				this.returnMids = localMids;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiDNToEphDispatchTask>(this);
		}

		private const string Name = "NspiDNToEph";

		private const int MaxLegacyDNs = 100000;

		private readonly NspiDNToEphFlags flags;

		private readonly string[] legacyDNs;

		private int[] returnMids;
	}
}
