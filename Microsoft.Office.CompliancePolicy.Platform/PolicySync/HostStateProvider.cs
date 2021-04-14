using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public class HostStateProvider
	{
		public virtual bool IsShuttingDown()
		{
			return false;
		}

		public virtual bool ShouldWait(out TimeSpan waitInterval)
		{
			waitInterval = TimeSpan.Zero;
			return false;
		}
	}
}
