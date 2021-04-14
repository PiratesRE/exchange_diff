using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CmdletExecutionPool
	{
		public CmdletExecutionPool(LoadBalanceAnchorContext anchorContext)
		{
			this.anchorContext = anchorContext;
			this.runspaces = new ConcurrentQueue<IAnchorRunspaceProxy>();
		}

		public bool HasRunspacesAvailable
		{
			get
			{
				return this.checkedOutRunspaceCount < this.anchorContext.Settings.MaximumNumberOfRunspaces;
			}
		}

		public RunspaceReservation AcquireRunspace()
		{
			if (!this.HasRunspacesAvailable)
			{
				throw new CmdletPoolExhaustedException();
			}
			Interlocked.Increment(ref this.checkedOutRunspaceCount);
			IAnchorRunspaceProxy runspace;
			if (!this.runspaces.TryDequeue(out runspace))
			{
				runspace = this.GetRunspace();
			}
			return new RunspaceReservation(this, runspace);
		}

		public void ReturnRunspace(IAnchorRunspaceProxy runspace)
		{
			Interlocked.Decrement(ref this.checkedOutRunspaceCount);
			this.runspaces.Enqueue(runspace);
		}

		protected virtual IAnchorRunspaceProxy GetRunspace()
		{
			IAnchorRunspaceProxy result;
			using (OperationTracker.Create(this.anchorContext.Logger, "Creating a new runspace.", new object[0]))
			{
				result = AnchorRunspaceProxy.CreateRunspaceForDatacenterAdmin(this.anchorContext, "Mailbox Load Balance");
			}
			return result;
		}

		private readonly LoadBalanceAnchorContext anchorContext;

		private ConcurrentQueue<IAnchorRunspaceProxy> runspaces;

		private int checkedOutRunspaceCount;
	}
}
