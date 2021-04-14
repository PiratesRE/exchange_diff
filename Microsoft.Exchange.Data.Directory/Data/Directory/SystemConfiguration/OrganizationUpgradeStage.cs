using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum OrganizationUpgradeStage
	{
		StartUpgrade,
		UpgradeOrganizationMailboxes,
		UpgradeUserMailboxes,
		CompleteUpgrade
	}
}
