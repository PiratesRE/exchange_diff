using System;

namespace Microsoft.Exchange.Management.Deployment
{
	public enum UpgradePhase
	{
		UpdateConfiguration = 1,
		UpdateMailboxes,
		Cleanup
	}
}
