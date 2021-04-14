using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriGetFQDNFromLegacyDNDispatchTask : RfriDispatchTask
	{
		public RfriGetFQDNFromLegacyDNDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriContext context, RfriGetFQDNFromLegacyDNFlags flags, string serverDn) : base(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context)
		{
			this.flags = flags;
			this.serverDn = serverDn;
		}

		public RfriStatus End(out string serverFqdn)
		{
			base.CheckDisposed();
			base.CheckCompletion();
			serverFqdn = this.returnServerFqdn;
			return base.Status;
		}

		protected override string TaskName
		{
			get
			{
				return "RfriGetFQDNFromLegacyDN";
			}
		}

		protected override void InternalDebugTrace()
		{
			RfriDispatchTask.ReferralTracer.TraceDebug<string, RfriGetFQDNFromLegacyDNFlags, string>((long)base.ContextHandle, "{0} params: flags={1}, serverDn={2}", this.TaskName, this.flags, this.serverDn);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			string localServerFqdn = null;
			base.RfriContextCallWrapper("GetFQDNFromLegacyDN", () => this.Context.GetFQDNFromLegacyDN(this.serverDn, out localServerFqdn));
			if (base.Status == RfriStatus.Success)
			{
				this.returnServerFqdn = localServerFqdn;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetFQDNFromLegacyDNDispatchTask>(this);
		}

		private const string Name = "RfriGetFQDNFromLegacyDN";

		private readonly RfriGetFQDNFromLegacyDNFlags flags;

		private readonly string serverDn;

		private string returnServerFqdn = string.Empty;
	}
}
