using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public enum ReplayLagPlayDownReason
	{
		[LocDescription(Strings.IDs.ReplayLagPlayDownReasonNone)]
		None,
		[LocDescription(Strings.IDs.ReplayLagPlayDownReasonLagDisabled)]
		LagDisabled,
		[LocDescription(Strings.IDs.ReplayLagPlayDownReasonNotEnoughFreeSpace)]
		NotEnoughFreeSpace,
		[LocDescription(Strings.IDs.ReplayLagPlayDownReasonLogsInRequiredRange)]
		LogsInRequiredRange
	}
}
