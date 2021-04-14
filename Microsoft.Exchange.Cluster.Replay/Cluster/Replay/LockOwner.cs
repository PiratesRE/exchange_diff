using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum LockOwner
	{
		[LocDescription(ReplayStrings.IDs.LockOwnerSuspend)]
		Suspend = 40,
		[LocDescription(ReplayStrings.IDs.LockOwnerAttemptCopyLastLogs)]
		AttemptCopyLastLogs = 30,
		[LocDescription(ReplayStrings.IDs.LockOwnerBackup)]
		Backup = 21,
		[LocDescription(ReplayStrings.IDs.LockOwnerComponent)]
		Component = 10,
		[LocDescription(ReplayStrings.IDs.LockOwnerConfigChecker)]
		ConfigChecker,
		[LocDescription(ReplayStrings.IDs.LockOwnerIdle)]
		Idle = 0
	}
}
