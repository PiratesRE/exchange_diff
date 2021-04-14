using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	internal class RunspaceReservation : DisposeTrackableBase
	{
		public RunspaceReservation(CmdletExecutionPool pool, IAnchorRunspaceProxy runspace)
		{
			this.Runspace = runspace;
			this.originatingPool = pool;
		}

		public IAnchorRunspaceProxy Runspace { get; private set; }

		protected override void InternalDispose(bool disposing)
		{
			this.originatingPool.ReturnRunspace(this.Runspace);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RunspaceReservation>(this);
		}

		private readonly CmdletExecutionPool originatingPool;
	}
}
