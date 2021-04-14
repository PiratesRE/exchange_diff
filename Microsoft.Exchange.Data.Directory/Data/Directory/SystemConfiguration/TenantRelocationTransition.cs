using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum TenantRelocationTransition : byte
	{
		NotStartedToSync,
		SyncToLockdown,
		LockdownToLockdownFinalSync,
		LockdownToGLSSwitch,
		LockdownToLockdownSwitchedGLS,
		LockdownToSync,
		LockdownToRetired,
		RetiredToSync,
		SyncToDeltaSync,
		DeltaSyncToSync
	}
}
