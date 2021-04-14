using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate
{
	[Flags]
	public enum GetItemEstimateStatus
	{
		Success = 1,
		InvalidCollection = 2050,
		SyncNotPrimed = 1027,
		InvalidSyncKey = 1028
	}
}
