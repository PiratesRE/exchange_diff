using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal sealed class RfriGetAddressBookUrlDispatchTask : RfriDispatchTask
	{
		public RfriGetAddressBookUrlDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriContext context, RfriGetAddressBookUrlFlags flags, string hostname, string userDn) : base(asyncCallback, asyncState, protocolRequestInfo, clientBinding, context)
		{
			this.flags = flags;
			this.hostname = hostname;
			this.userDn = userDn;
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
				return "RfriGetAddressBookUrlDispatch";
			}
		}

		protected override void InternalDebugTrace()
		{
			RfriDispatchTask.ReferralTracer.TraceDebug<string, RfriGetAddressBookUrlFlags, string>((long)base.ContextHandle, "{0} params: flags={1}, userDn={2}", this.TaskName, this.flags, this.userDn);
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			string localServerUrl = null;
			base.RfriContextCallWrapper("GetAddressBookUrl", () => this.Context.GetAddressBookUrl(this.hostname, this.userDn, out localServerUrl));
			if (base.Status == RfriStatus.Success)
			{
				this.returnServerUrl = localServerUrl;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetAddressBookUrlDispatchTask>(this);
		}

		private const string Name = "RfriGetAddressBookUrlDispatch";

		private readonly RfriGetAddressBookUrlFlags flags;

		private readonly string hostname;

		private readonly string userDn;

		private string returnServerUrl = string.Empty;
	}
}
