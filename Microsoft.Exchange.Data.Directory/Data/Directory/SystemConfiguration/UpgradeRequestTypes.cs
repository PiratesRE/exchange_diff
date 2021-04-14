using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UpgradeRequestTypes
	{
		None,
		TenantUpgrade,
		PrestageUpgrade,
		CancelPrestageUpgrade,
		PilotUpgrade,
		TenantUpgradeDryRun
	}
}
