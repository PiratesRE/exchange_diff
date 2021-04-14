using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriGetNewDSADispatchTask : RfriDispatchTask
	{
		public RfriGetNewDSADispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriContext context, RfriGetNewDSAFlags flags, string userDn) : base(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context)
		{
			this.flags = flags;
			this.userDn = userDn;
		}

		public RfriStatus End(out string serverDn)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			serverDn = this.returnServerDn;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "RfriGetNewDSA";
			}
		}

		protected override void InternalDebugTrace()
		{
			RfriDispatchTask.ReferralTracer.TraceDebug<string, RfriGetNewDSAFlags, string>((long)base.ContextHandle, "{0} params: flags={1}, userDn={2}", this.TaskName, this.flags, this.userDn);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			string localServerDn = null;
			base.RfriContextCallWrapper("GetNewDSA", () => this.Context.GetNewDSA(this.userDn, out localServerDn));
			if (base.Status == RfriStatus.Success)
			{
				this.returnServerDn = localServerDn;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetNewDSADispatchTask>(this);
		}

		private const string Name = "RfriGetNewDSA";

		private readonly RfriGetNewDSAFlags flags;

		private readonly string userDn;

		private string returnServerDn = string.Empty;
	}
}
