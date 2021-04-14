using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RelocationStateRequested : byte
	{
		None,
		InitializationFinished = 10,
		SynchronizationFinishedFullSync = 25,
		SynchronizationFinishedDeltaSync = 40,
		LockdownFinishedFinalDeltaSync = 55,
		LockdownSwitchedGLS = 60,
		RetiredUpdatedSourceForest = 65,
		RetiredUpdatedTargetForest = 70,
		Cleanup = 90
	}
}
