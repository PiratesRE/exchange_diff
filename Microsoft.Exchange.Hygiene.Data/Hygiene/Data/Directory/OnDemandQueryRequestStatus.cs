using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal enum OnDemandQueryRequestStatus
	{
		NotStarted,
		InProgress,
		Success,
		UserCancel,
		Failed,
		Expired,
		OverTenantQuota,
		OverSystemQuota
	}
}
