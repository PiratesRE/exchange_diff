using System;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaConfig : IQueueQuotaConfig
	{
		public QueueQuotaConfig(TransportAppConfig.FlowControlLogConfig flowControlLogConfig, TransportAppConfig.QueueConfig queueConfig)
		{
			this.EnforceQuota = TransportAppConfig.GetConfigBool("EnforceQueueQuota", VariantConfiguration.InvariantNoFlightingSnapshot.Transport.EnforceQueueQuota.Enabled);
			this.WarningRatio = TransportAppConfig.GetConfigDouble("QueueQuotaWarningRatio", 0.0, 1.0, 0.4);
			this.LowWatermarkRatio = TransportAppConfig.GetConfigDouble("QueueQuotaLowWatermarkRatio", 0.0, 1.0, 0.66);
			this.SubmissionQueueCapacity = TransportAppConfig.GetConfigInt("SubmissionQueueCapacity", 0, int.MaxValue, 10000);
			this.TotalQueueCapacity = TransportAppConfig.GetConfigInt("TotalQueueCapacity", 0, int.MaxValue, 100000);
			this.OrganizationQueueQuota = TransportAppConfig.GetConfigInt("OrganizationQueueQuota", 0, 100, 50);
			this.SafeTenantOrganizationQueueQuota = TransportAppConfig.GetConfigInt("SafeTenantOrganizationQueueQuota", 0, 100, 100);
			this.OutlookTenantOrganizationQueueQuota = TransportAppConfig.GetConfigInt("OutlookTenantOrganizationQueueQuota", 0, 500, 150);
			this.OutlookTenantSenderQueueQuota = TransportAppConfig.GetConfigInt("OutlookTenantSenderQueueQuota", 0, 100, 1);
			this.SenderQueueQuota = TransportAppConfig.GetConfigInt("SenderQueueQuota", 0, 100, 10);
			this.NullSenderQueueQuota = TransportAppConfig.GetConfigInt("NullSenderQueueQuota", 0, 100, 50);
			this.SenderTrackingThreshold = TransportAppConfig.GetConfigInt("QueueQuotaSenderTrackingThreshold", 0, int.MaxValue, 100);
			this.NumberOfOrganizationsLoggedInSummary = TransportAppConfig.GetConfigInt("NumberOfOrganizationsLoggedInQueueQuotaSummary", 0, int.MaxValue, 50);
			this.NumberOfSendersLoggedInSummary = TransportAppConfig.GetConfigInt("NumberOfSendersLoggedInQueueQuotaSummary", 0, int.MaxValue, 3);
			this.TrackerEntryLifeTime = TransportAppConfig.GetConfigTimeSpan("QueueQuotaTrackerEntryLifeTime", TimeSpan.Zero, TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(15.0));
			this.AccountForestEnabled = TransportAppConfig.GetConfigBool("QueueQuotaAccountForestEnabled", false);
			this.AccountForestQueueQuota = TransportAppConfig.GetConfigInt("QueueQuotaAccountForestQuota", 0, 100, 75);
			this.TrackSummaryLoggingInterval = flowControlLogConfig.SummaryLoggingInterval;
			this.TrackSummaryBucketLength = flowControlLogConfig.SummaryBucketLength;
			this.MaxSummaryLinesLogged = flowControlLogConfig.MaxSummaryLinesLogged;
			this.RecentPerfCounterTrackingInterval = queueConfig.RecentPerfCounterTrackingInterval;
			this.RecentPerfCounterTrackingBucketSize = queueConfig.RecentPerfCounterTrackingBucketSize;
		}

		public bool EnforceQuota { get; private set; }

		public double WarningRatio { get; private set; }

		public double LowWatermarkRatio { get; private set; }

		public int SubmissionQueueCapacity { get; private set; }

		public int TotalQueueCapacity { get; private set; }

		public int OrganizationQueueQuota { get; private set; }

		public int SafeTenantOrganizationQueueQuota { get; private set; }

		public int OutlookTenantOrganizationQueueQuota { get; private set; }

		public int OutlookTenantSenderQueueQuota { get; private set; }

		public int SenderQueueQuota { get; private set; }

		public int NullSenderQueueQuota { get; private set; }

		public int SenderTrackingThreshold { get; private set; }

		public int NumberOfOrganizationsLoggedInSummary { get; private set; }

		public int NumberOfSendersLoggedInSummary { get; private set; }

		public TimeSpan TrackerEntryLifeTime { get; private set; }

		public TimeSpan TrackSummaryLoggingInterval { get; private set; }

		public TimeSpan TrackSummaryBucketLength { get; private set; }

		public int MaxSummaryLinesLogged { get; private set; }

		public bool AccountForestEnabled { get; set; }

		public int AccountForestQueueQuota { get; set; }

		public TimeSpan RecentPerfCounterTrackingInterval { get; private set; }

		public TimeSpan RecentPerfCounterTrackingBucketSize { get; private set; }

		public static bool IsQueueQuotaEnabled()
		{
			return TransportAppConfig.GetConfigBool("QueueQuotaEnabled", false);
		}

		public static bool IsQueueQuotaWithMeteringEnabled()
		{
			return TransportAppConfig.GetConfigBool("QueueQuotaMeteringEnabled", true);
		}
	}
}
