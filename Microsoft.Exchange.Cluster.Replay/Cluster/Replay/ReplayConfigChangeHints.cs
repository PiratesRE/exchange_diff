using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum ReplayConfigChangeHints
	{
		None,
		PeriodicFullScan,
		DbCopyAdded,
		DbCopyRemoved,
		AmAttemptCopyLastLogs,
		AmPreMountCallback,
		AmPreMountCallbackRI,
		DbSeeder,
		DbResumeBefore,
		DbResumeAfter,
		DbSuspendBefore,
		DbSuspendAfter,
		RunConfigUpdaterRpc,
		AmMultiNodeReplicaNotifier,
		MoveDatabasePathConfigChanged,
		DbSyncSuspendResumeStateBefore,
		AmStoreServiceStartDetected,
		AmPreMountCallbackLogStreamReset,
		LogReplayerHitLogCorruption,
		LogCopierSkippingPastLog,
		IncReseedRedirtiedDatabase,
		IncReseedCompleted,
		DisableReplayLag,
		EnableReplayLag,
		AutoReseed
	}
}
