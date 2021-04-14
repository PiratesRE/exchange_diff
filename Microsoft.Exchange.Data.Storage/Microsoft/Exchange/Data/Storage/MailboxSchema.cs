using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSchema : StoreObjectSchema
	{
		public new static MailboxSchema Instance
		{
			get
			{
				if (MailboxSchema.instance == null)
				{
					MailboxSchema.instance = new MailboxSchema();
				}
				return MailboxSchema.instance;
			}
		}

		[Autoload]
		internal static readonly StorePropertyDefinition Id = InternalSchema.MailboxId;

		public static readonly PropertyTagPropertyDefinition ImapSubscribeList = InternalSchema.ImapSubscribeList;

		public static readonly PropertyTagPropertyDefinition UserName = InternalSchema.UserName;

		public static readonly PropertyTagPropertyDefinition MailboxOofStateEx = InternalSchema.MailboxOofStateEx;

		public static readonly PropertyTagPropertyDefinition MailboxOofStateUserChangeTime = InternalSchema.MailboxOofStateUserChangeTime;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition UserOofSettingsItemId = InternalSchema.UserOofSettingsItemId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition AdditionalRenEntryIds = InternalSchema.AdditionalRenEntryIds;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition QuotaStorageWarning = InternalSchema.QuotaStorageWarning;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition StorageQuotaLimit = InternalSchema.StorageQuotaLimit;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition PersistableTenantPartitionHint = InternalSchema.PersistableTenantPartitionHint;

		public static readonly PropertyTagPropertyDefinition IsContentIndexingEnabled = InternalSchema.IsContentIndexingEnabled;

		public static readonly StorePropertyDefinition UnifiedMessagingOptions = InternalSchema.UnifiedMessagingOptions;

		public static readonly StorePropertyDefinition OfficeCommunicatorOptions = InternalSchema.OfficeCommunicatorOptions;

		public static readonly PropertyTagPropertyDefinition InternetMdns = InternalSchema.InternetMdns;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition QuotaProhibitReceive = InternalSchema.ProhibitReceiveQuota;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition QuotaProhibitSend = InternalSchema.ProhibitSendQuota;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition QuotaUsed = InternalSchema.Size;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition QuotaUsedExtended = InternalSchema.ExtendedSize;

		public static readonly PropertyTagPropertyDefinition DumpsterQuotaUsedExtended = InternalSchema.ExtendedDumpsterSize;

		public static readonly PropertyTagPropertyDefinition MaxUserMessageSize = InternalSchema.MaxSubmitMessageSize;

		public static readonly StorePropertyDefinition MaxMessageSize = InternalSchema.MaxMessageSize;

		public static readonly PropertyTagPropertyDefinition SendReadNotifications = InternalSchema.SendReadNotifications;

		public static readonly PropertyTagPropertyDefinition MailboxMiscFlags = InternalSchema.MailboxMiscFlags;

		public static readonly PropertyTagPropertyDefinition MailboxGuid = InternalSchema.MailboxGuid;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition MailboxNumber = InternalSchema.MailboxNumber;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition LocaleId = InternalSchema.LocaleId;

		[Autoload]
		public static readonly StorePropertyDefinition LastDelegatedAuditTime = InternalSchema.LastDelegatedAuditTime;

		[Autoload]
		public static readonly StorePropertyDefinition LastExternalAuditTime = InternalSchema.LastExternalAuditTime;

		[Autoload]
		public static readonly StorePropertyDefinition LastNonOwnerAuditTime = InternalSchema.LastNonOwnerAuditTime;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition FinderEntryId = InternalSchema.FinderEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition CommonViewsEntryId = InternalSchema.CommonViewsEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition DeferredActionFolderEntryId = InternalSchema.DeferredActionFolderEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition LegacyScheduleFolderEntryId = InternalSchema.LegacyScheduleFolderEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition LegacyShortcutsFolderEntryId = InternalSchema.LegacyShortcutsFolderEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition LegacyViewsFolderEntryId = InternalSchema.LegacyViewsFolderEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition DeletedItemsEntryId = InternalSchema.DeletedItemsEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition SentItemsEntryId = InternalSchema.SentItemsEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition OutboxEntryId = InternalSchema.OutboxEntryId;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition LogonRightsOnMailbox = InternalSchema.LogonRightsOnMailbox;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition IsMailboxLocalized = InternalSchema.IsMailboxLocalized;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition UserPhotoCacheId = InternalSchema.UserPhotoCacheId;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition UserPhotoPreviewCacheId = InternalSchema.UserPhotoPreviewCacheId;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition InferenceClientActivityFlags = InternalSchema.InferenceClientActivityFlags;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition InferenceTrainedModelVersionBreadCrumb = InternalSchema.InferenceTrainedModelVersionBreadCrumb;

		public static readonly PropertyTagPropertyDefinition ControlDataForCalendarRepairAssistant = InternalSchema.ControlDataForCalendarRepairAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForSharingPolicyAssistant = InternalSchema.ControlDataForSharingPolicyAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForElcAssistant = InternalSchema.ControlDataForElcAssistant;

		public static readonly PropertyTagPropertyDefinition ElcLastRunTotalProcessingTime = InternalSchema.ElcLastRunTotalProcessingTime;

		public static readonly PropertyTagPropertyDefinition ElcLastRunSubAssistantProcessingTime = InternalSchema.ElcLastRunSubAssistantProcessingTime;

		public static readonly PropertyTagPropertyDefinition ElcLastRunUpdatedFolderCount = InternalSchema.ElcLastRunUpdatedFolderCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedFolderCount = InternalSchema.ElcLastRunTaggedFolderCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunUpdatedItemCount = InternalSchema.ElcLastRunUpdatedItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedWithArchiveItemCount = InternalSchema.ElcLastRunTaggedWithArchiveItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedWithExpiryItemCount = InternalSchema.ElcLastRunTaggedWithExpiryItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunDeletedFromRootItemCount = InternalSchema.ElcLastRunDeletedFromRootItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunDeletedFromDumpsterItemCount = InternalSchema.ElcLastRunDeletedFromDumpsterItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunArchivedFromRootItemCount = InternalSchema.ElcLastRunArchivedFromRootItemCount;

		public static readonly PropertyTagPropertyDefinition ElcLastRunArchivedFromDumpsterItemCount = InternalSchema.ElcLastRunArchivedFromDumpsterItemCount;

		public static readonly PropertyTagPropertyDefinition ELCLastSuccessTimestamp = InternalSchema.ELCLastSuccessTimestamp;

		public static readonly PropertyTagPropertyDefinition ControlDataForSearchIndexRepairAssistant = InternalSchema.ControlDataForSearchIndexRepairAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForTopNWordsAssistant = InternalSchema.ControlDataForTopNWordsAssistant;

		public static readonly PropertyTagPropertyDefinition IsTopNEnabled = InternalSchema.IsTopNEnabled;

		public static readonly PropertyTagPropertyDefinition ControlDataForJunkEmailAssistant = InternalSchema.ControlDataForJunkEmailAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForCalendarSyncAssistant = InternalSchema.ControlDataForCalendarSyncAssistant;

		public static readonly PropertyTagPropertyDefinition ExternalSharingCalendarSubscriptionCount = InternalSchema.ExternalSharingCalendarSubscriptionCount;

		public static readonly PropertyTagPropertyDefinition ConsumerSharingCalendarSubscriptionCount = InternalSchema.ConsumerSharingCalendarSubscriptionCount;

		public static readonly PropertyTagPropertyDefinition ControlDataForUMReportingAssistant = InternalSchema.ControlDataForUMReportingAssistant;

		public static readonly PropertyTagPropertyDefinition HasUMReportData = InternalSchema.HasUMReportData;

		public static readonly PropertyTagPropertyDefinition ControlDataForInferenceTrainingAssistant = InternalSchema.ControlDataForInferenceTrainingAssistant;

		public static readonly PropertyTagPropertyDefinition InferenceEnabled = InternalSchema.InferenceEnabled;

		public static readonly PropertyTagPropertyDefinition ControlDataForDirectoryProcessorAssistant = InternalSchema.ControlDataForDirectoryProcessorAssistant;

		public static readonly PropertyTagPropertyDefinition NeedsDirectoryProcessor = InternalSchema.NeedsDirectoryProcessor;

		public static readonly PropertyTagPropertyDefinition ControlDataForOABGeneratorAssistant = InternalSchema.ControlDataForOABGeneratorAssistant;

		public static readonly PropertyTagPropertyDefinition InternetCalendarSubscriptionCount = InternalSchema.InternetCalendarSubscriptionCount;

		public static readonly PropertyTagPropertyDefinition ExternalSharingContactSubscriptionCount = InternalSchema.ExternalSharingContactSubscriptionCount;

		public static readonly PropertyTagPropertyDefinition JunkEmailSafeListDirty = InternalSchema.JunkEmailSafeListDirty;

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedId = InternalSchema.LastSharingPolicyAppliedId;

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedHash = InternalSchema.LastSharingPolicyAppliedHash;

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedTime = InternalSchema.LastSharingPolicyAppliedTime;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition OofScheduleStart = InternalSchema.OofScheduleStart;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition OofScheduleEnd = InternalSchema.OofScheduleEnd;

		public static readonly PropertyTagPropertyDefinition RetentionQueryInfo = InternalSchema.RetentionQueryInfo;

		public static readonly PropertyTagPropertyDefinition ControlDataForPublicFolderAssistant = InternalSchema.ControlDataForPublicFolderAssistant;

		public static readonly PropertyTagPropertyDefinition IsMarkedMailbox = InternalSchema.IsMarkedMailbox;

		public static readonly PropertyTagPropertyDefinition MailboxLastProcessedTimestamp = InternalSchema.MailboxLastProcessedTimestamp;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition MailboxType = InternalSchema.MailboxType;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition MailboxTypeDetail = InternalSchema.MailboxTypeDetail;

		public static readonly PropertyTagPropertyDefinition ContactLinking = InternalSchema.ContactLinking;

		public static readonly PropertyTagPropertyDefinition ContactSaveVersion = InternalSchema.ContactSaveVersion;

		public static readonly PropertyTagPropertyDefinition PushNotificationSubscriptionType = InternalSchema.PushNotificationSubscriptionType;

		public static readonly PropertyTagPropertyDefinition NotificationBrokerSubscriptions = InternalSchema.NotificationBrokerSubscriptions;

		public static readonly PropertyTagPropertyDefinition ControlDataForInferenceDataCollectionAssistant = InternalSchema.ControlDataForInferenceDataCollectionAssistant;

		public static readonly PropertyTagPropertyDefinition InferenceDataCollectionProcessingState = InternalSchema.InferenceDataCollectionProcessingState;

		public static readonly PropertyTagPropertyDefinition SiteMailboxInternalState = InternalSchema.SiteMailboxInternalState;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition ExtendedRuleSizeLimit = InternalSchema.ExtendedRuleSizeLimit;

		public static readonly PropertyTagPropertyDefinition ControlDataForSiteMailboxAssistant = InternalSchema.ControlDataForSiteMailboxAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForPeopleRelevanceAssistant = InternalSchema.ControlDataForPeopleRelevanceAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForSharePointSignalStoreAssistant = InternalSchema.ControlDataForSharePointSignalStoreAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForGroupMailboxAssistant = InternalSchema.ControlDataForGroupMailboxAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForMailboxAssociationReplicationAssistant = InternalSchema.ControlDataForMailboxAssociationReplicationAssistant;

		public static readonly PropertyTagPropertyDefinition ControlDataForPeopleCentricTriageAssistant = InternalSchema.ControlDataForPeopleCentricTriageAssistant;

		public static readonly PropertyTagPropertyDefinition MailboxAssociationNextReplicationTime = InternalSchema.MailboxAssociationNextReplicationTime;

		public static readonly PropertyTagPropertyDefinition MailboxAssociationProcessingFlags = InternalSchema.MailboxAssociationProcessingFlags;

		public static readonly PropertyTagPropertyDefinition GroupMailboxPermissionsVersion = InternalSchema.GroupMailboxPermissionsVersion;

		public static readonly PropertyTagPropertyDefinition GroupMailboxGeneratedPhotoSignature = InternalSchema.GroupMailboxGeneratedPhotoSignature;

		public static readonly PropertyTagPropertyDefinition GroupMailboxGeneratedPhotoVersion = InternalSchema.GroupMailboxGeneratedPhotoVersion;

		public static readonly PropertyTagPropertyDefinition GroupMailboxExchangeResourcesPublishedVersion = InternalSchema.GroupMailboxExchangeResourcesPublishedVersion;

		public static readonly PropertyTagPropertyDefinition ItemCount = InternalSchema.ItemCount;

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastContentCount = InternalSchema.InferenceTrainingLastContentCount;

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastAttemptTimestamp = InternalSchema.InferenceTrainingLastAttemptTimestamp;

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastSuccessTimestamp = InternalSchema.InferenceTrainingLastSuccessTimestamp;

		public static readonly PropertyTagPropertyDefinition InferenceTruthLoggingLastAttemptTimestamp = InternalSchema.InferenceTruthLoggingLastAttemptTimestamp;

		public static readonly PropertyTagPropertyDefinition InferenceTruthLoggingLastSuccessTimestamp = InternalSchema.InferenceTruthLoggingLastSuccessTimestamp;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition InferenceUserCapabilityFlags = InternalSchema.InferenceUserCapabilityFlags;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceUserClassificationReady = InternalSchema.InferenceUserClassificationReady;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceUserUIReady = InternalSchema.InferenceUserUIReady;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceClassificationEnabled = InternalSchema.InferenceClassificationEnabled;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceClutterEnabled = InternalSchema.InferenceClutterEnabled;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceHasBeenClutterInvited = InternalSchema.InferenceHasBeenClutterInvited;

		[Autoload]
		public static readonly StorePropertyDefinition MailboxOofState = new MailboxOofStateProperty();

		public static readonly PropertyTagPropertyDefinition InTransitStatus = InternalSchema.InTransitStatus;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition ItemsPendingUpgrade = InternalSchema.ItemsPendingUpgrade;

		[Autoload]
		public static readonly PropertyTagPropertyDefinition LastLogonTime = InternalSchema.LastLogonTime;

		private static MailboxSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceOLKUserActivityLoggingEnabled = InternalSchema.InferenceOLKUserActivityLoggingEnabled;
	}
}
