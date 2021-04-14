using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncWorkerData
	{
		bool IsValid { get; }

		bool IsMirrored { get; }

		bool IsMigration { get; }

		bool IsPartnerProtocol { get; }

		Guid SubscriptionGuid { get; }

		AggregationSubscriptionType SubscriptionType { get; }

		AggregationType AggregationType { get; }

		string IncomingServerName { get; }

		int IncomingServerPort { get; }

		bool InitialSyncInRecoveryMode { get; }

		string Domain { get; }

		string Name { get; }

		int? EnumeratedItemsLimitPerConnection { get; }

		bool Inactive { get; }

		DateTime CreationTime { get; }

		string UserExchangeMailboxSmtpAddress { get; set; }

		string UserLegacyDN { get; }

		StoreObjectId SubscriptionMessageId { get; set; }

		FolderSupport FolderSupport { get; }

		ItemSupport ItemSupport { get; }

		SyncQuirks SyncQuirks { get; }

		SyncPhase SyncPhase { get; set; }

		AggregationStatus Status { get; set; }

		DetailedAggregationStatus DetailedAggregationStatus { get; set; }

		DateTime? LastSyncTime { get; set; }

		DateTime? LastSuccessfulSyncTime { get; set; }

		DateTime AdjustedLastSuccessfulSyncTime { get; set; }

		bool IsInitialSyncDone { get; }

		bool WasInitialSyncDone { get; }

		long ItemsSynced { get; }

		long ItemsSkipped { get; }

		DateTime? LastSyncNowRequestTime { get; set; }

		string Diagnostics { get; set; }

		string OutageDetectionDiagnostics { get; set; }

		string PoisonCallstack { set; }

		long? TotalItemsInSourceMailbox { get; set; }

		long? TotalSizeOfSourceMailbox { get; set; }

		AggregationSubscriptionIdentity SubscriptionIdentity { get; }

		string UserExchangeMailboxDisplayName { get; set; }

		void UpdateItemStatistics(long itemsSynced, long itemsSkipped);

		bool ShouldFolderBeExcluded(string folderName, char folderSeparator);

		void AppendOutageDetectionDiagnostics(string machineName, Guid databaseGuid, TimeSpan configuredOutageDetectionThreshold, TimeSpan observedOutageDuration);

		void SetToMessageObject(MessageItem message);
	}
}
