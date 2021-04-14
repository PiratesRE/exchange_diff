using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum MoveStatus
	{
		[LocDescription(Strings.IDs.MoveStatusUnknown)]
		Unknown,
		[LocDescription(Strings.IDs.MoveStatusSucceeded)]
		Succeeded,
		[LocDescription(Strings.IDs.MoveStatusWarning)]
		Warning,
		[LocDescription(Strings.IDs.MoveStatusFailed)]
		Failed,
		[LocDescription(Strings.IDs.MoveStatusSkipped)]
		Skipped
	}
}
