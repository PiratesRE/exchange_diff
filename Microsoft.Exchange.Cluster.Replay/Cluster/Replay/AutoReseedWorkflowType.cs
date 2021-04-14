using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum AutoReseedWorkflowType
	{
		FailedSuspendedCopyAutoReseed = 1,
		CatalogAutoReseed,
		FailedSuspendedCatalogRebuild,
		HealthyCopyCompletedSeed,
		FailedCopy,
		ManualReseed,
		ManualResume,
		MountNeverMountedActive
	}
}
