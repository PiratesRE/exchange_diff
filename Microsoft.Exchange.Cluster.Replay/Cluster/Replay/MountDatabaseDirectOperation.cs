using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum MountDatabaseDirectOperation
	{
		AmPreMountCallback = 1,
		RegistryReplicatorCopy,
		StoreMount,
		PreMountQueuedOpStart,
		PreMountQueuedOpExecution,
		PreventMountIfNecessary,
		ResumeActiveCopy,
		UpdateLastLogGenOnMount,
		GetRunningReplicaInstance,
		ConfirmLogReset,
		LowestGenerationInDirectory,
		HighestGenerationInDirectory,
		GenerationAvailableInDirectory,
		UpdateLastLogGeneratedInClusDB
	}
}
