using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RelocationStatusDetails : byte
	{
		NotStarted,
		InitializationStarted = 5,
		InitializationFinished = 10,
		SynchronizationStartedFullSync = 15,
		SynchronizationFinishedFullSync = 25,
		SynchronizationStartedDeltaSync = 30,
		SynchronizationFinishedDeltaSync = 40,
		LockdownStarted = 45,
		LockdownStartedFinalDeltaSync = 50,
		LockdownFinishedFinalDeltaSync = 55,
		LockdownSwitchedGLS = 60,
		RetiredUpdatedSourceForest = 65,
		RetiredUpdatedTargetForest = 70,
		Arriving = 75,
		Active = 80,
		Cleanup = 90
	}
}
