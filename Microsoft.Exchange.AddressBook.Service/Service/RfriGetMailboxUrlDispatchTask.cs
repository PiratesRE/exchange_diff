using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriGetMailboxUrlDispatchTask : RfriDispatchTask
	{
		public RfriGetMailboxUrlDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriContext context, RfriGetMailboxUrlFlags flags, string hostname, string serverDn) : base(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context)
		{
			this.flags = flags;
			this.hostname = hostname;
			this.serverDn = serverDn;
		}

		public RfriStatus End(out string serverUrl)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			serverUrl = this.returnServerUrl;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "RfriGetMailboxUrlDispatch";
			}
		}

		protected override void InternalDebugTrace()
		{
			RfriDispatchTask.ReferralTracer.TraceDebug((long)base.ContextHandle, "{0} params: flags={1}, hostname={2}, serverDn={3}", new object[]
			{
				this.TaskName,
				this.flags,
				this.hostname,
				this.serverDn
			});
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			string localServerUrl = null;
			base.RfriContextCallWrapper("GetMailboxUrl", () => this.Context.GetMailboxUrl(this.hostname, this.serverDn, out localServerUrl));
			if (base.Status == RfriStatus.Success)
			{
				this.returnServerUrl = localServerUrl;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetMailboxUrlDispatchTask>(this);
		}

		private const string Name = "RfriGetMailboxUrlDispatch";

		private readonly RfriGetMailboxUrlFlags flags;

		private readonly string hostname;

		private readonly string serverDn;

		private string returnServerUrl = string.Empty;
	}
}
