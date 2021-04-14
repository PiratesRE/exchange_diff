using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum MountStatus
	{
		[LocDescription(Strings.IDs.MountStatusUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.MountStatusMounted)]
		Mounted,
		[LocDescription(Strings.IDs.MountStatusDismounted)]
		Dismounted,
		[LocDescription(Strings.IDs.MountStatusMounting)]
		Mounting,
		[LocDescription(Strings.IDs.MountStatusDismounting)]
		Dismounting
	}
}
