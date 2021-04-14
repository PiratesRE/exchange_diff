using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UpgradeStage
	{
		None,
		SyncedWorkItem,
		StartPilotUpgrade,
		StartOrgUpgrade,
		MoveArbitration,
		MoveRegularUser,
		MoveCloudOnlyArchive,
		MoveRegularPilot,
		MoveCloudOnlyArchivePilot,
		CompleteOrgUpgrade
	}
}
