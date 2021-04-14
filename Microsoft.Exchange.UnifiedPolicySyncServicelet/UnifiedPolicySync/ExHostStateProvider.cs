using System;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Servicelets.UnifiedPolicySync
{
	public sealed class ExHostStateProvider : HostStateProvider
	{
		public override bool IsShuttingDown()
		{
			return this.shutDown == 1;
		}

		public override bool ShouldWait(out TimeSpan waitInterval)
		{
			waitInterval = TimeSpan.Zero;
			return false;
		}

		internal void SignalShutdown()
		{
			this.shutDown = 1;
		}

		private volatile int shutDown;
	}
}
