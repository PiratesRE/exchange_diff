using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum CopyStatus
	{
		[LocDescription(Strings.IDs.CopyStatusUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.CopyStatusFailed)]
		Failed = 3,
		[LocDescription(Strings.IDs.CopyStatusSeeding)]
		Seeding,
		[LocDescription(Strings.IDs.CopyStatusSuspended)]
		Suspended,
		[LocDescription(Strings.IDs.CopyStatusHealthy)]
		Healthy,
		[LocDescription(Strings.IDs.CopyStatusServiceDown)]
		ServiceDown,
		[LocDescription(Strings.IDs.CopyStatusInitializing)]
		Initializing,
		[LocDescription(Strings.IDs.CopyStatusResynchronizing)]
		Resynchronizing,
		[LocDescription(Strings.IDs.CopyStatusMounted)]
		Mounted = 11,
		[LocDescription(Strings.IDs.CopyStatusDismounted)]
		Dismounted,
		[LocDescription(Strings.IDs.CopyStatusMounting)]
		Mounting,
		[LocDescription(Strings.IDs.CopyStatusDismounting)]
		Dismounting,
		[LocDescription(Strings.IDs.CopyStatusDisconnectedAndHealthy)]
		DisconnectedAndHealthy,
		[LocDescription(Strings.IDs.CopyStatusFailedAndSuspended)]
		FailedAndSuspended,
		[LocDescription(Strings.IDs.CopyStatusDisconnectedAndResynchronizing)]
		DisconnectedAndResynchronizing,
		[LocDescription(Strings.IDs.CopyStatusNonExchangeReplication)]
		NonExchangeReplication,
		[LocDescription(Strings.IDs.CopyStatusSeedingSource)]
		SeedingSource,
		[LocDescription(Strings.IDs.CopyStatusMisconfigured)]
		Misconfigured
	}
}
