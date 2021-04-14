using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Rpc.Cluster
{
	public enum ContentIndexStatusType
	{
		[LocDescription(CoreStrings.IDs.ContentIndexStatusUnknown)]
		Unknown,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusHealthy)]
		Healthy,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusCrawling)]
		Crawling,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusFailed)]
		Failed,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusSeeding)]
		Seeding,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusFailedAndSuspended)]
		FailedAndSuspended,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusSuspended)]
		Suspended,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusDisabled)]
		Disabled,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusAutoSuspended)]
		AutoSuspended,
		[LocDescription(CoreStrings.IDs.ContentIndexStatusHealthyAndUpgrading)]
		HealthyAndUpgrading
	}
}
