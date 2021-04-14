using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Flags]
	internal enum DatabaseCopyActionFlags : uint
	{
		Replication = 1U,
		Activation = 2U,
		ActiveCopy = 4U,
		SyncSuspendResume = 8U,
		SkipSettingResumeAutoReseedState = 16U
	}
}
