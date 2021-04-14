using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaConfig
	{
		bool EnforceQuota { get; }

		double WarningRatio { get; }

		double LowWatermarkRatio { get; }

		int SubmissionQueueCapacity { get; }

		int TotalQueueCapacity { get; }

		int OrganizationQueueQuota { get; }

		int SafeTenantOrganizationQueueQuota { get; }

		int OutlookTenantOrganizationQueueQuota { get; }

		int OutlookTenantSenderQueueQuota { get; }

		int SenderQueueQuota { get; }

		int NullSenderQueueQuota { get; }

		int SenderTrackingThreshold { get; }

		int NumberOfOrganizationsLoggedInSummary { get; }

		int NumberOfSendersLoggedInSummary { get; }

		TimeSpan TrackerEntryLifeTime { get; }

		TimeSpan TrackSummaryLoggingInterval { get; }

		TimeSpan TrackSummaryBucketLength { get; }

		int MaxSummaryLinesLogged { get; }

		bool AccountForestEnabled { get; set; }

		int AccountForestQueueQuota { get; set; }
	}
}
