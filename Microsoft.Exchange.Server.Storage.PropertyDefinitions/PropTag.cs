using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public static class PropTag
	{
		public static class Mailbox
		{
			public static readonly StorePropTag DeleteAfterSubmit = new StorePropTag(3585, PropertyType.Boolean, new StorePropInfo("DeleteAfterSubmit", 3585, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MessageSize = new StorePropTag(3592, PropertyType.Int64, new StorePropInfo("MessageSize", 3592, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag MessageSize32 = new StorePropTag(3592, PropertyType.Int32, new StorePropInfo("MessageSize32", 3592, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag SentMailEntryId = new StorePropTag(3594, PropertyType.Binary, new StorePropInfo("SentMailEntryId", 3594, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag HighestFolderInternetId = new StorePropTag(3619, PropertyType.Int32, new StorePropInfo("HighestFolderInternetId", 3619, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag NTSecurityDescriptor = new StorePropTag(3623, PropertyType.Binary, new StorePropInfo("NTSecurityDescriptor", 3623, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag CISearchEnabled = new StorePropTag(3676, PropertyType.Boolean, new StorePropInfo("CISearchEnabled", 3676, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag ExtendedRuleSizeLimit = new StorePropTag(3739, PropertyType.Int32, new StorePropInfo("ExtendedRuleSizeLimit", 3739, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag Access = new StorePropTag(4084, PropertyType.Int32, new StorePropInfo("Access", 4084, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag MappingSignature = new StorePropTag(4088, PropertyType.Binary, new StorePropInfo("MappingSignature", 4088, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag StoreRecordKey = new StorePropTag(4090, PropertyType.Binary, new StorePropInfo("StoreRecordKey", 4090, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag StoreEntryId = new StorePropTag(4091, PropertyType.Binary, new StorePropInfo("StoreEntryId", 4091, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag EmailAddress = new StorePropTag(12291, PropertyType.Unicode, new StorePropInfo("EmailAddress", 12291, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag Comment = new StorePropTag(12292, PropertyType.Unicode, new StorePropInfo("Comment", 12292, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag CreationTime = new StorePropTag(12295, PropertyType.SysTime, new StorePropInfo("CreationTime", 12295, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag LastModificationTime = new StorePropTag(12296, PropertyType.SysTime, new StorePropInfo("LastModificationTime", 12296, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ResourceFlags = new StorePropTag(12297, PropertyType.Int32, new StorePropInfo("ResourceFlags", 12297, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MessageTableTotalPages = new StorePropTag(13313, PropertyType.Int32, new StorePropInfo("MessageTableTotalPages", 13313, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MessageTableAvailablePages = new StorePropTag(13314, PropertyType.Int32, new StorePropInfo("MessageTableAvailablePages", 13314, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag OtherTablesTotalPages = new StorePropTag(13315, PropertyType.Int32, new StorePropInfo("OtherTablesTotalPages", 13315, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag OtherTablesAvailablePages = new StorePropTag(13316, PropertyType.Int32, new StorePropInfo("OtherTablesAvailablePages", 13316, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag AttachmentTableTotalPages = new StorePropTag(13317, PropertyType.Int32, new StorePropInfo("AttachmentTableTotalPages", 13317, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag AttachmentTableAvailablePages = new StorePropTag(13318, PropertyType.Int32, new StorePropInfo("AttachmentTableAvailablePages", 13318, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxTypeVersion = new StorePropTag(13319, PropertyType.Int32, new StorePropInfo("MailboxTypeVersion", 13319, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxPartitionMailboxGuids = new StorePropTag(13320, PropertyType.MVGuid, new StorePropInfo("MailboxPartitionMailboxGuids", 13320, PropertyType.MVGuid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag StoreSupportMask = new StorePropTag(13325, PropertyType.Int32, new StorePropInfo("StoreSupportMask", 13325, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag StoreState = new StorePropTag(13326, PropertyType.Int32, new StorePropInfo("StoreState", 13326, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMSubtreeSearchKey = new StorePropTag(13328, PropertyType.Binary, new StorePropInfo("IPMSubtreeSearchKey", 13328, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMOutboxSearchKey = new StorePropTag(13329, PropertyType.Binary, new StorePropInfo("IPMOutboxSearchKey", 13329, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMWastebasketSearchKey = new StorePropTag(13330, PropertyType.Binary, new StorePropInfo("IPMWastebasketSearchKey", 13330, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMSentmailSearchKey = new StorePropTag(13331, PropertyType.Binary, new StorePropInfo("IPMSentmailSearchKey", 13331, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MdbProvider = new StorePropTag(13332, PropertyType.Binary, new StorePropInfo("MdbProvider", 13332, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ReceiveFolderSettings = new StorePropTag(13333, PropertyType.Object, new StorePropInfo("ReceiveFolderSettings", 13333, PropertyType.Object, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LocalDirectoryEntryID = new StorePropTag(13334, PropertyType.Binary, new StorePropInfo("LocalDirectoryEntryID", 13334, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForCalendarRepairAssistant = new StorePropTag(13344, PropertyType.Binary, new StorePropInfo("ControlDataForCalendarRepairAssistant", 13344, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForSharingPolicyAssistant = new StorePropTag(13345, PropertyType.Binary, new StorePropInfo("ControlDataForSharingPolicyAssistant", 13345, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForElcAssistant = new StorePropTag(13346, PropertyType.Binary, new StorePropInfo("ControlDataForElcAssistant", 13346, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForTopNWordsAssistant = new StorePropTag(13347, PropertyType.Binary, new StorePropInfo("ControlDataForTopNWordsAssistant", 13347, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForJunkEmailAssistant = new StorePropTag(13348, PropertyType.Binary, new StorePropInfo("ControlDataForJunkEmailAssistant", 13348, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForCalendarSyncAssistant = new StorePropTag(13349, PropertyType.Binary, new StorePropInfo("ControlDataForCalendarSyncAssistant", 13349, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ExternalSharingCalendarSubscriptionCount = new StorePropTag(13350, PropertyType.Int32, new StorePropInfo("ExternalSharingCalendarSubscriptionCount", 13350, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForUMReportingAssistant = new StorePropTag(13351, PropertyType.Binary, new StorePropInfo("ControlDataForUMReportingAssistant", 13351, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag HasUMReportData = new StorePropTag(13352, PropertyType.Boolean, new StorePropInfo("HasUMReportData", 13352, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InternetCalendarSubscriptionCount = new StorePropTag(13353, PropertyType.Int32, new StorePropInfo("InternetCalendarSubscriptionCount", 13353, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ExternalSharingContactSubscriptionCount = new StorePropTag(13354, PropertyType.Int32, new StorePropInfo("ExternalSharingContactSubscriptionCount", 13354, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag JunkEmailSafeListDirty = new StorePropTag(13355, PropertyType.Int32, new StorePropInfo("JunkEmailSafeListDirty", 13355, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IsTopNEnabled = new StorePropTag(13356, PropertyType.Boolean, new StorePropInfo("IsTopNEnabled", 13356, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LastSharingPolicyAppliedId = new StorePropTag(13357, PropertyType.Binary, new StorePropInfo("LastSharingPolicyAppliedId", 13357, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LastSharingPolicyAppliedHash = new StorePropTag(13358, PropertyType.Binary, new StorePropInfo("LastSharingPolicyAppliedHash", 13358, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LastSharingPolicyAppliedTime = new StorePropTag(13359, PropertyType.SysTime, new StorePropInfo("LastSharingPolicyAppliedTime", 13359, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OofScheduleStart = new StorePropTag(13360, PropertyType.SysTime, new StorePropInfo("OofScheduleStart", 13360, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OofScheduleEnd = new StorePropTag(13361, PropertyType.SysTime, new StorePropInfo("OofScheduleEnd", 13361, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForDirectoryProcessorAssistant = new StorePropTag(13362, PropertyType.Binary, new StorePropInfo("ControlDataForDirectoryProcessorAssistant", 13362, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag NeedsDirectoryProcessor = new StorePropTag(13363, PropertyType.Boolean, new StorePropInfo("NeedsDirectoryProcessor", 13363, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag RetentionQueryIds = new StorePropTag(13364, PropertyType.MVUnicode, new StorePropInfo("RetentionQueryIds", 13364, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag RetentionQueryInfo = new StorePropTag(13365, PropertyType.Int64, new StorePropInfo("RetentionQueryInfo", 13365, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForPublicFolderAssistant = new StorePropTag(13367, PropertyType.Binary, new StorePropInfo("ControlDataForPublicFolderAssistant", 13367, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForInferenceTrainingAssistant = new StorePropTag(13368, PropertyType.Binary, new StorePropInfo("ControlDataForInferenceTrainingAssistant", 13368, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceEnabled = new StorePropTag(13369, PropertyType.Boolean, new StorePropInfo("InferenceEnabled", 13369, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForContactLinkingAssistant = new StorePropTag(13370, PropertyType.Binary, new StorePropInfo("ControlDataForContactLinkingAssistant", 13370, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ContactLinking = new StorePropTag(13371, PropertyType.Int32, new StorePropInfo("ContactLinking", 13371, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForOABGeneratorAssistant = new StorePropTag(13372, PropertyType.Binary, new StorePropInfo("ControlDataForOABGeneratorAssistant", 13372, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ContactSaveVersion = new StorePropTag(13373, PropertyType.Int32, new StorePropInfo("ContactSaveVersion", 13373, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForOrgContactsSyncAssistant = new StorePropTag(13374, PropertyType.Binary, new StorePropInfo("ControlDataForOrgContactsSyncAssistant", 13374, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OrgContactsSyncTimestamp = new StorePropTag(13375, PropertyType.SysTime, new StorePropInfo("OrgContactsSyncTimestamp", 13375, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag PushNotificationSubscriptionType = new StorePropTag(13376, PropertyType.Binary, new StorePropInfo("PushNotificationSubscriptionType", 13376, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OrgContactsSyncADWatermark = new StorePropTag(13377, PropertyType.SysTime, new StorePropInfo("OrgContactsSyncADWatermark", 13377, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForInferenceDataCollectionAssistant = new StorePropTag(13378, PropertyType.Binary, new StorePropInfo("ControlDataForInferenceDataCollectionAssistant", 13378, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceDataCollectionProcessingState = new StorePropTag(13379, PropertyType.Binary, new StorePropInfo("InferenceDataCollectionProcessingState", 13379, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForPeopleRelevanceAssistant = new StorePropTag(13380, PropertyType.Binary, new StorePropInfo("ControlDataForPeopleRelevanceAssistant", 13380, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SiteMailboxInternalState = new StorePropTag(13381, PropertyType.Int32, new StorePropInfo("SiteMailboxInternalState", 13381, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForSiteMailboxAssistant = new StorePropTag(13382, PropertyType.Binary, new StorePropInfo("ControlDataForSiteMailboxAssistant", 13382, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTrainingLastContentCount = new StorePropTag(13383, PropertyType.Int32, new StorePropInfo("InferenceTrainingLastContentCount", 13383, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTrainingLastAttemptTimestamp = new StorePropTag(13384, PropertyType.SysTime, new StorePropInfo("InferenceTrainingLastAttemptTimestamp", 13384, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTrainingLastSuccessTimestamp = new StorePropTag(13385, PropertyType.SysTime, new StorePropInfo("InferenceTrainingLastSuccessTimestamp", 13385, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceUserCapabilityFlags = new StorePropTag(13386, PropertyType.Int32, new StorePropInfo("InferenceUserCapabilityFlags", 13386, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForMailboxAssociationReplicationAssistant = new StorePropTag(13387, PropertyType.Binary, new StorePropInfo("ControlDataForMailboxAssociationReplicationAssistant", 13387, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxAssociationNextReplicationTime = new StorePropTag(13388, PropertyType.SysTime, new StorePropInfo("MailboxAssociationNextReplicationTime", 13388, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxAssociationProcessingFlags = new StorePropTag(13389, PropertyType.Int32, new StorePropInfo("MailboxAssociationProcessingFlags", 13389, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForSharePointSignalStoreAssistant = new StorePropTag(13390, PropertyType.Binary, new StorePropInfo("ControlDataForSharePointSignalStoreAssistant", 13390, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForPeopleCentricTriageAssistant = new StorePropTag(13391, PropertyType.Binary, new StorePropInfo("ControlDataForPeopleCentricTriageAssistant", 13391, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag NotificationBrokerSubscriptions = new StorePropTag(13392, PropertyType.Int32, new StorePropInfo("NotificationBrokerSubscriptions", 13392, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag GroupMailboxPermissionsVersion = new StorePropTag(13393, PropertyType.Int32, new StorePropInfo("GroupMailboxPermissionsVersion", 13393, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunTotalProcessingTime = new StorePropTag(13394, PropertyType.Int64, new StorePropInfo("ElcLastRunTotalProcessingTime", 13394, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunSubAssistantProcessingTime = new StorePropTag(13395, PropertyType.Int64, new StorePropInfo("ElcLastRunSubAssistantProcessingTime", 13395, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunUpdatedFolderCount = new StorePropTag(13396, PropertyType.Int64, new StorePropInfo("ElcLastRunUpdatedFolderCount", 13396, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunTaggedFolderCount = new StorePropTag(13397, PropertyType.Int64, new StorePropInfo("ElcLastRunTaggedFolderCount", 13397, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunUpdatedItemCount = new StorePropTag(13398, PropertyType.Int64, new StorePropInfo("ElcLastRunUpdatedItemCount", 13398, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunTaggedWithArchiveItemCount = new StorePropTag(13399, PropertyType.Int64, new StorePropInfo("ElcLastRunTaggedWithArchiveItemCount", 13399, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunTaggedWithExpiryItemCount = new StorePropTag(13400, PropertyType.Int64, new StorePropInfo("ElcLastRunTaggedWithExpiryItemCount", 13400, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunDeletedFromRootItemCount = new StorePropTag(13401, PropertyType.Int64, new StorePropInfo("ElcLastRunDeletedFromRootItemCount", 13401, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunDeletedFromDumpsterItemCount = new StorePropTag(13402, PropertyType.Int64, new StorePropInfo("ElcLastRunDeletedFromDumpsterItemCount", 13402, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunArchivedFromRootItemCount = new StorePropTag(13403, PropertyType.Int64, new StorePropInfo("ElcLastRunArchivedFromRootItemCount", 13403, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ElcLastRunArchivedFromDumpsterItemCount = new StorePropTag(13404, PropertyType.Int64, new StorePropInfo("ElcLastRunArchivedFromDumpsterItemCount", 13404, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ScheduledISIntegLastFinished = new StorePropTag(13405, PropertyType.SysTime, new StorePropInfo("ScheduledISIntegLastFinished", 13405, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForSearchIndexRepairAssistant = new StorePropTag(13406, PropertyType.Binary, new StorePropInfo("ControlDataForSearchIndexRepairAssistant", 13406, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ELCLastSuccessTimestamp = new StorePropTag(13407, PropertyType.SysTime, new StorePropInfo("ELCLastSuccessTimestamp", 13407, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTruthLoggingLastAttemptTimestamp = new StorePropTag(13409, PropertyType.SysTime, new StorePropInfo("InferenceTruthLoggingLastAttemptTimestamp", 13409, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTruthLoggingLastSuccessTimestamp = new StorePropTag(13410, PropertyType.SysTime, new StorePropInfo("InferenceTruthLoggingLastSuccessTimestamp", 13410, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ControlDataForGroupMailboxAssistant = new StorePropTag(13411, PropertyType.Binary, new StorePropInfo("ControlDataForGroupMailboxAssistant", 13411, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ItemsPendingUpgrade = new StorePropTag(13412, PropertyType.Int32, new StorePropInfo("ItemsPendingUpgrade", 13412, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ConsumerSharingCalendarSubscriptionCount = new StorePropTag(13413, PropertyType.Int32, new StorePropInfo("ConsumerSharingCalendarSubscriptionCount", 13413, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag GroupMailboxGeneratedPhotoVersion = new StorePropTag(13414, PropertyType.Int32, new StorePropInfo("GroupMailboxGeneratedPhotoVersion", 13414, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag GroupMailboxGeneratedPhotoSignature = new StorePropTag(13415, PropertyType.Binary, new StorePropInfo("GroupMailboxGeneratedPhotoSignature", 13415, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag GroupMailboxExchangeResourcesPublishedVersion = new StorePropTag(13416, PropertyType.Int32, new StorePropInfo("GroupMailboxExchangeResourcesPublishedVersion", 13416, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ValidFolderMask = new StorePropTag(13791, PropertyType.Int32, new StorePropInfo("ValidFolderMask", 13791, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMSubtreeEntryId = new StorePropTag(13792, PropertyType.Binary, new StorePropInfo("IPMSubtreeEntryId", 13792, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMOutboxEntryId = new StorePropTag(13794, PropertyType.Binary, new StorePropInfo("IPMOutboxEntryId", 13794, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMWastebasketEntryId = new StorePropTag(13795, PropertyType.Binary, new StorePropInfo("IPMWastebasketEntryId", 13795, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMSentmailEntryId = new StorePropTag(13796, PropertyType.Binary, new StorePropInfo("IPMSentmailEntryId", 13796, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMViewsEntryId = new StorePropTag(13797, PropertyType.Binary, new StorePropInfo("IPMViewsEntryId", 13797, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag UnsearchableItems = new StorePropTag(13822, PropertyType.Binary, new StorePropInfo("UnsearchableItems", 13822, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag IPMFinderEntryId = new StorePropTag(13824, PropertyType.Binary, new StorePropInfo("IPMFinderEntryId", 13824, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ContentCount = new StorePropTag(13826, PropertyType.Int32, new StorePropInfo("ContentCount", 13826, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag ContentCountInt64 = new StorePropTag(13826, PropertyType.Int64, new StorePropInfo("ContentCountInt64", 13826, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag Search = new StorePropTag(13831, PropertyType.Object, new StorePropInfo("Search", 13831, PropertyType.Object, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AssociatedContentCount = new StorePropTag(13847, PropertyType.Int32, new StorePropInfo("AssociatedContentCount", 13847, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag AssociatedContentCountInt64 = new StorePropTag(13847, PropertyType.Int64, new StorePropInfo("AssociatedContentCountInt64", 13847, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag AdditionalRENEntryIds = new StorePropTag(14040, PropertyType.MVBinary, new StorePropInfo("AdditionalRENEntryIds", 14040, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SimpleDisplayName = new StorePropTag(14847, PropertyType.Unicode, new StorePropInfo("SimpleDisplayName", 14847, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag TestBlobProperty = new StorePropTag(15616, PropertyType.Int64, new StorePropInfo("TestBlobProperty", 15616, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ScheduledISIntegCorruptionCount = new StorePropTag(15773, PropertyType.Int32, new StorePropInfo("ScheduledISIntegCorruptionCount", 15773, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag ScheduledISIntegExecutionTime = new StorePropTag(15774, PropertyType.Int32, new StorePropInfo("ScheduledISIntegExecutionTime", 15774, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxPartitionNumber = new StorePropTag(15775, PropertyType.Int32, new StorePropInfo("MailboxPartitionNumber", 15775, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxTypeDetail = new StorePropTag(15782, PropertyType.Int32, new StorePropInfo("MailboxTypeDetail", 15782, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag InternalTenantHint = new StorePropTag(15783, PropertyType.Binary, new StorePropInfo("InternalTenantHint", 15783, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag TenantHint = new StorePropTag(15790, PropertyType.Binary, new StorePropInfo("TenantHint", 15790, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MaintenanceId = new StorePropTag(15803, PropertyType.Guid, new StorePropInfo("MaintenanceId", 15803, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxType = new StorePropTag(15804, PropertyType.Int32, new StorePropInfo("MailboxType", 15804, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag ACLData = new StorePropTag(16352, PropertyType.Binary, new StorePropInfo("ACLData", 16352, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag DesignInProgress = new StorePropTag(16356, PropertyType.Boolean, new StorePropInfo("DesignInProgress", 16356, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag StorageQuotaLimit = new StorePropTag(16373, PropertyType.Int32, new StorePropInfo("StorageQuotaLimit", 16373, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag RulesSize = new StorePropTag(16383, PropertyType.Int32, new StorePropInfo("RulesSize", 16383, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 3)), ObjectType.Mailbox);

			public static readonly StorePropTag IMAPSubscribeList = new StorePropTag(26102, PropertyType.MVUnicode, new StorePropInfo("IMAPSubscribeList", 26102, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InTransitState = new StorePropTag(26136, PropertyType.Boolean, new StorePropInfo("InTransitState", 26136, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag InTransitStatus = new StorePropTag(26136, PropertyType.Int32, new StorePropInfo("InTransitStatus", 26136, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag UserEntryId = new StorePropTag(26137, PropertyType.Binary, new StorePropInfo("UserEntryId", 26137, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag UserName = new StorePropTag(26138, PropertyType.Unicode, new StorePropInfo("UserName", 26138, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxOwnerEntryId = new StorePropTag(26139, PropertyType.Binary, new StorePropInfo("MailboxOwnerEntryId", 26139, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxOwnerName = new StorePropTag(26140, PropertyType.Unicode, new StorePropInfo("MailboxOwnerName", 26140, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag OofState = new StorePropTag(26141, PropertyType.Boolean, new StorePropInfo("OofState", 26141, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag TestLineSpeed = new StorePropTag(26155, PropertyType.Binary, new StorePropInfo("TestLineSpeed", 26155, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SerializedReplidGuidMap = new StorePropTag(26168, PropertyType.Binary, new StorePropInfo("SerializedReplidGuidMap", 26168, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedMsgCount = new StorePropTag(26176, PropertyType.Int32, new StorePropInfo("DeletedMsgCount", 26176, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedMsgCountInt64 = new StorePropTag(26176, PropertyType.Int64, new StorePropInfo("DeletedMsgCountInt64", 26176, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedAssocMsgCount = new StorePropTag(26179, PropertyType.Int32, new StorePropInfo("DeletedAssocMsgCount", 26179, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedAssocMsgCountInt64 = new StorePropTag(26179, PropertyType.Int64, new StorePropInfo("DeletedAssocMsgCountInt64", 26179, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag HasNamedProperties = new StorePropTag(26186, PropertyType.Boolean, new StorePropInfo("HasNamedProperties", 26186, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag ActiveUserEntryId = new StorePropTag(26194, PropertyType.Binary, new StorePropInfo("ActiveUserEntryId", 26194, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ProhibitReceiveQuota = new StorePropTag(26218, PropertyType.Int32, new StorePropInfo("ProhibitReceiveQuota", 26218, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MaxSubmitMessageSize = new StorePropTag(26221, PropertyType.Int32, new StorePropInfo("MaxSubmitMessageSize", 26221, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag ProhibitSendQuota = new StorePropTag(26222, PropertyType.Int32, new StorePropInfo("ProhibitSendQuota", 26222, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedOn = new StorePropTag(26255, PropertyType.SysTime, new StorePropInfo("DeletedOn", 26255, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxDatabaseVersion = new StorePropTag(26266, PropertyType.Int32, new StorePropInfo("MailboxDatabaseVersion", 26266, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedMessageSize = new StorePropTag(26267, PropertyType.Int64, new StorePropInfo("DeletedMessageSize", 26267, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedMessageSize32 = new StorePropTag(26267, PropertyType.Int32, new StorePropInfo("DeletedMessageSize32", 26267, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedNormalMessageSize = new StorePropTag(26268, PropertyType.Int64, new StorePropInfo("DeletedNormalMessageSize", 26268, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedNormalMessageSize32 = new StorePropTag(26268, PropertyType.Int32, new StorePropInfo("DeletedNormalMessageSize32", 26268, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedAssociatedMessageSize = new StorePropTag(26269, PropertyType.Int64, new StorePropInfo("DeletedAssociatedMessageSize", 26269, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag DeletedAssociatedMessageSize32 = new StorePropTag(26269, PropertyType.Int32, new StorePropInfo("DeletedAssociatedMessageSize32", 26269, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag NTUsername = new StorePropTag(26272, PropertyType.Unicode, new StorePropInfo("NTUsername", 26272, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag NTUserSid = new StorePropTag(26272, PropertyType.Binary, new StorePropInfo("NTUserSid", 26272, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LocaleId = new StorePropTag(26273, PropertyType.Int32, new StorePropInfo("LocaleId", 26273, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LastLogonTime = new StorePropTag(26274, PropertyType.SysTime, new StorePropInfo("LastLogonTime", 26274, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4)), ObjectType.Mailbox);

			public static readonly StorePropTag LastLogoffTime = new StorePropTag(26275, PropertyType.SysTime, new StorePropInfo("LastLogoffTime", 26275, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4)), ObjectType.Mailbox);

			public static readonly StorePropTag StorageLimitInformation = new StorePropTag(26276, PropertyType.Int32, new StorePropInfo("StorageLimitInformation", 26276, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InternetMdns = new StorePropTag(26277, PropertyType.Boolean, new StorePropInfo("InternetMdns", 26277, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxStatus = new StorePropTag(26277, PropertyType.Int16, new StorePropInfo("MailboxStatus", 26277, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxFlags = new StorePropTag(26279, PropertyType.Int32, new StorePropInfo("MailboxFlags", 26279, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag PreservingMailboxSignature = new StorePropTag(26280, PropertyType.Boolean, new StorePropInfo("PreservingMailboxSignature", 26280, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MRSPreservingMailboxSignature = new StorePropTag(26281, PropertyType.Boolean, new StorePropInfo("MRSPreservingMailboxSignature", 26281, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxMessagesPerFolderCountWarningQuota = new StorePropTag(26283, PropertyType.Int32, new StorePropInfo("MailboxMessagesPerFolderCountWarningQuota", 26283, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxMessagesPerFolderCountReceiveQuota = new StorePropTag(26284, PropertyType.Int32, new StorePropInfo("MailboxMessagesPerFolderCountReceiveQuota", 26284, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag DumpsterMessagesPerFolderCountWarningQuota = new StorePropTag(26285, PropertyType.Int32, new StorePropInfo("DumpsterMessagesPerFolderCountWarningQuota", 26285, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag DumpsterMessagesPerFolderCountReceiveQuota = new StorePropTag(26286, PropertyType.Int32, new StorePropInfo("DumpsterMessagesPerFolderCountReceiveQuota", 26286, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag FolderHierarchyChildrenCountWarningQuota = new StorePropTag(26287, PropertyType.Int32, new StorePropInfo("FolderHierarchyChildrenCountWarningQuota", 26287, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag FolderHierarchyChildrenCountReceiveQuota = new StorePropTag(26288, PropertyType.Int32, new StorePropInfo("FolderHierarchyChildrenCountReceiveQuota", 26288, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag FolderHierarchyDepthWarningQuota = new StorePropTag(26289, PropertyType.Int32, new StorePropInfo("FolderHierarchyDepthWarningQuota", 26289, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag FolderHierarchyDepthReceiveQuota = new StorePropTag(26290, PropertyType.Int32, new StorePropInfo("FolderHierarchyDepthReceiveQuota", 26290, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag NormalMessageSize = new StorePropTag(26291, PropertyType.Int64, new StorePropInfo("NormalMessageSize", 26291, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag NormalMessageSize32 = new StorePropTag(26291, PropertyType.Int32, new StorePropInfo("NormalMessageSize32", 26291, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag AssociatedMessageSize = new StorePropTag(26292, PropertyType.Int64, new StorePropInfo("AssociatedMessageSize", 26292, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag AssociatedMessageSize32 = new StorePropTag(26292, PropertyType.Int32, new StorePropInfo("AssociatedMessageSize32", 26292, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag FoldersCountWarningQuota = new StorePropTag(26293, PropertyType.Int32, new StorePropInfo("FoldersCountWarningQuota", 26293, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag FoldersCountReceiveQuota = new StorePropTag(26294, PropertyType.Int32, new StorePropInfo("FoldersCountReceiveQuota", 26294, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag NamedPropertiesCountQuota = new StorePropTag(26295, PropertyType.Int32, new StorePropInfo("NamedPropertiesCountQuota", 26295, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5)), ObjectType.Mailbox);

			public static readonly StorePropTag CodePageId = new StorePropTag(26307, PropertyType.Int32, new StorePropInfo("CodePageId", 26307, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag RetentionAgeLimit = new StorePropTag(26308, PropertyType.Int32, new StorePropInfo("RetentionAgeLimit", 26308, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag UserDisplayName = new StorePropTag(26315, PropertyType.Unicode, new StorePropInfo("UserDisplayName", 26315, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SortLocaleId = new StorePropTag(26373, PropertyType.Int32, new StorePropInfo("SortLocaleId", 26373, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxDSGuid = new StorePropTag(26375, PropertyType.Binary, new StorePropInfo("MailboxDSGuid", 26375, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxDSGuidGuid = new StorePropTag(26375, PropertyType.Guid, new StorePropInfo("MailboxDSGuidGuid", 26375, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag DateDiscoveredAbsentInDS = new StorePropTag(26376, PropertyType.SysTime, new StorePropInfo("DateDiscoveredAbsentInDS", 26376, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag UnifiedMailboxGuidGuid = new StorePropTag(26376, PropertyType.Guid, new StorePropInfo("UnifiedMailboxGuidGuid", 26376, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag QuotaWarningThreshold = new StorePropTag(26401, PropertyType.Int32, new StorePropInfo("QuotaWarningThreshold", 26401, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag QuotaSendThreshold = new StorePropTag(26402, PropertyType.Int32, new StorePropInfo("QuotaSendThreshold", 26402, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag QuotaReceiveThreshold = new StorePropTag(26403, PropertyType.Int32, new StorePropInfo("QuotaReceiveThreshold", 26403, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag PropertyGroupMappingId = new StorePropTag(26420, PropertyType.Int32, new StorePropInfo("PropertyGroupMappingId", 26420, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SentMailSvrEID = new StorePropTag(26432, PropertyType.SvrEid, new StorePropInfo("SentMailSvrEID", 26432, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SentMailSvrEIDBin = new StorePropTag(26432, PropertyType.Binary, new StorePropInfo("SentMailSvrEIDBin", 26432, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LocalIdNext = new StorePropTag(26465, PropertyType.Binary, new StorePropInfo("LocalIdNext", 26465, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag RootFid = new StorePropTag(26468, PropertyType.Int64, new StorePropInfo("RootFid", 26468, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag FIDC = new StorePropTag(26470, PropertyType.Binary, new StorePropInfo("FIDC", 26470, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MdbDSGuid = new StorePropTag(26474, PropertyType.Binary, new StorePropInfo("MdbDSGuid", 26474, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxOwnerDN = new StorePropTag(26475, PropertyType.Unicode, new StorePropInfo("MailboxOwnerDN", 26475, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MapiEntryIdGuid = new StorePropTag(26476, PropertyType.Binary, new StorePropInfo("MapiEntryIdGuid", 26476, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag Localized = new StorePropTag(26477, PropertyType.Boolean, new StorePropInfo("Localized", 26477, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag LCID = new StorePropTag(26478, PropertyType.Int32, new StorePropInfo("LCID", 26478, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AltRecipientDN = new StorePropTag(26479, PropertyType.Unicode, new StorePropInfo("AltRecipientDN", 26479, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag NoLocalDelivery = new StorePropTag(26480, PropertyType.Boolean, new StorePropInfo("NoLocalDelivery", 26480, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag DeliveryContentLength = new StorePropTag(26481, PropertyType.Int32, new StorePropInfo("DeliveryContentLength", 26481, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AutoReply = new StorePropTag(26482, PropertyType.Boolean, new StorePropInfo("AutoReply", 26482, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxOwnerDisplayName = new StorePropTag(26483, PropertyType.Unicode, new StorePropInfo("MailboxOwnerDisplayName", 26483, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxLastUpdated = new StorePropTag(26484, PropertyType.SysTime, new StorePropInfo("MailboxLastUpdated", 26484, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AdminSurName = new StorePropTag(26485, PropertyType.Unicode, new StorePropInfo("AdminSurName", 26485, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AdminGivenName = new StorePropTag(26486, PropertyType.Unicode, new StorePropInfo("AdminGivenName", 26486, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ActiveSearchCount = new StorePropTag(26487, PropertyType.Int32, new StorePropInfo("ActiveSearchCount", 26487, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag AdminNickname = new StorePropTag(26488, PropertyType.Unicode, new StorePropInfo("AdminNickname", 26488, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag QuotaStyle = new StorePropTag(26489, PropertyType.Int32, new StorePropInfo("QuotaStyle", 26489, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OverQuotaLimit = new StorePropTag(26490, PropertyType.Int32, new StorePropInfo("OverQuotaLimit", 26490, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag StorageQuota = new StorePropTag(26491, PropertyType.Int32, new StorePropInfo("StorageQuota", 26491, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag SubmitContentLength = new StorePropTag(26492, PropertyType.Int32, new StorePropInfo("SubmitContentLength", 26492, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ReservedIdCounterRangeUpperLimit = new StorePropTag(26494, PropertyType.Int64, new StorePropInfo("ReservedIdCounterRangeUpperLimit", 26494, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ReservedCnCounterRangeUpperLimit = new StorePropTag(26495, PropertyType.Int64, new StorePropInfo("ReservedCnCounterRangeUpperLimit", 26495, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag FolderIdsetIn = new StorePropTag(26514, PropertyType.Binary, new StorePropInfo("FolderIdsetIn", 26514, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag CnsetIn = new StorePropTag(26516, PropertyType.Binary, new StorePropInfo("CnsetIn", 26516, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag ShutoffQuota = new StorePropTag(26628, PropertyType.Int32, new StorePropInfo("ShutoffQuota", 26628, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxMiscFlags = new StorePropTag(26630, PropertyType.Int32, new StorePropInfo("MailboxMiscFlags", 26630, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 10)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxInCreation = new StorePropTag(26635, PropertyType.Boolean, new StorePropInfo("MailboxInCreation", 26635, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag ObjectClassFlags = new StorePropTag(26637, PropertyType.Int32, new StorePropInfo("ObjectClassFlags", 26637, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OOFStateEx = new StorePropTag(26640, PropertyType.Int32, new StorePropInfo("OOFStateEx", 26640, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag OofStateUserChangeTime = new StorePropTag(26643, PropertyType.SysTime, new StorePropInfo("OofStateUserChangeTime", 26643, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag UserOofSettingsItemId = new StorePropTag(26644, PropertyType.Binary, new StorePropInfo("UserOofSettingsItemId", 26644, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxQuarantined = new StorePropTag(26650, PropertyType.Boolean, new StorePropInfo("MailboxQuarantined", 26650, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxQuarantineDescription = new StorePropTag(26651, PropertyType.Unicode, new StorePropInfo("MailboxQuarantineDescription", 26651, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxQuarantineLastCrash = new StorePropTag(26652, PropertyType.SysTime, new StorePropInfo("MailboxQuarantineLastCrash", 26652, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxQuarantineEnd = new StorePropTag(26653, PropertyType.SysTime, new StorePropInfo("MailboxQuarantineEnd", 26653, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.Mailbox);

			public static readonly StorePropTag MaxMessageSize = new StorePropTag(26669, PropertyType.Int32, new StorePropInfo("MaxMessageSize", 26669, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceClientActivityFlags = new StorePropTag(26676, PropertyType.Int32, new StorePropInfo("InferenceClientActivityFlags", 26676, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceOWAUserActivityLoggingEnabledDeprecated = new StorePropTag(26677, PropertyType.Boolean, new StorePropInfo("InferenceOWAUserActivityLoggingEnabledDeprecated", 26677, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceOLKUserActivityLoggingEnabled = new StorePropTag(26678, PropertyType.Boolean, new StorePropInfo("InferenceOLKUserActivityLoggingEnabled", 26678, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag InferenceTrainedModelVersionBreadCrumb = new StorePropTag(26739, PropertyType.Binary, new StorePropInfo("InferenceTrainedModelVersionBreadCrumb", 26739, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag UserPhotoCacheId = new StorePropTag(31770, PropertyType.Int32, new StorePropInfo("UserPhotoCacheId", 31770, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag UserPhotoPreviewCacheId = new StorePropTag(31771, PropertyType.Int32, new StorePropInfo("UserPhotoPreviewCacheId", 31771, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Mailbox);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[]
			{
				PropTag.Mailbox.ScheduledISIntegCorruptionCount,
				PropTag.Mailbox.ScheduledISIntegExecutionTime,
				PropTag.Mailbox.MailboxPartitionNumber,
				PropTag.Mailbox.InternalTenantHint,
				PropTag.Mailbox.MaintenanceId
			};

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[]
			{
				PropTag.Mailbox.ScheduledISIntegCorruptionCount,
				PropTag.Mailbox.ScheduledISIntegExecutionTime,
				PropTag.Mailbox.MailboxPartitionNumber,
				PropTag.Mailbox.InternalTenantHint,
				PropTag.Mailbox.MaintenanceId,
				PropTag.Mailbox.RulesSize
			};

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.Mailbox.MessageSize,
				PropTag.Mailbox.MessageSize32,
				PropTag.Mailbox.HighestFolderInternetId,
				PropTag.Mailbox.CISearchEnabled,
				PropTag.Mailbox.Access,
				PropTag.Mailbox.DisplayName,
				PropTag.Mailbox.EmailAddress,
				PropTag.Mailbox.Comment,
				PropTag.Mailbox.CreationTime,
				PropTag.Mailbox.MessageTableTotalPages,
				PropTag.Mailbox.MessageTableAvailablePages,
				PropTag.Mailbox.OtherTablesTotalPages,
				PropTag.Mailbox.OtherTablesAvailablePages,
				PropTag.Mailbox.AttachmentTableTotalPages,
				PropTag.Mailbox.AttachmentTableAvailablePages,
				PropTag.Mailbox.MailboxTypeVersion,
				PropTag.Mailbox.MailboxPartitionMailboxGuids,
				PropTag.Mailbox.StoreState,
				PropTag.Mailbox.LocalDirectoryEntryID,
				PropTag.Mailbox.ContentCount,
				PropTag.Mailbox.ContentCountInt64,
				PropTag.Mailbox.AssociatedContentCount,
				PropTag.Mailbox.AssociatedContentCountInt64,
				PropTag.Mailbox.SimpleDisplayName,
				PropTag.Mailbox.ScheduledISIntegCorruptionCount,
				PropTag.Mailbox.ScheduledISIntegExecutionTime,
				PropTag.Mailbox.MailboxPartitionNumber,
				PropTag.Mailbox.MailboxTypeDetail,
				PropTag.Mailbox.InternalTenantHint,
				PropTag.Mailbox.TenantHint,
				PropTag.Mailbox.MaintenanceId,
				PropTag.Mailbox.MailboxType,
				PropTag.Mailbox.StorageQuotaLimit,
				PropTag.Mailbox.RulesSize,
				PropTag.Mailbox.InTransitState,
				PropTag.Mailbox.InTransitStatus,
				PropTag.Mailbox.UserEntryId,
				PropTag.Mailbox.UserName,
				PropTag.Mailbox.MailboxOwnerEntryId,
				PropTag.Mailbox.MailboxOwnerName,
				PropTag.Mailbox.SerializedReplidGuidMap,
				PropTag.Mailbox.DeletedMsgCount,
				PropTag.Mailbox.DeletedMsgCountInt64,
				PropTag.Mailbox.DeletedAssocMsgCount,
				PropTag.Mailbox.DeletedAssocMsgCountInt64,
				PropTag.Mailbox.HasNamedProperties,
				PropTag.Mailbox.ProhibitReceiveQuota,
				PropTag.Mailbox.MaxSubmitMessageSize,
				PropTag.Mailbox.ProhibitSendQuota,
				PropTag.Mailbox.DeletedOn,
				PropTag.Mailbox.DeletedMessageSize,
				PropTag.Mailbox.DeletedMessageSize32,
				PropTag.Mailbox.DeletedNormalMessageSize,
				PropTag.Mailbox.DeletedNormalMessageSize32,
				PropTag.Mailbox.DeletedAssociatedMessageSize,
				PropTag.Mailbox.DeletedAssociatedMessageSize32,
				PropTag.Mailbox.LastLogonTime,
				PropTag.Mailbox.LastLogoffTime,
				PropTag.Mailbox.InternetMdns,
				PropTag.Mailbox.MailboxStatus,
				PropTag.Mailbox.PreservingMailboxSignature,
				PropTag.Mailbox.MRSPreservingMailboxSignature,
				PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyDepthWarningQuota,
				PropTag.Mailbox.FolderHierarchyDepthReceiveQuota,
				PropTag.Mailbox.NormalMessageSize,
				PropTag.Mailbox.NormalMessageSize32,
				PropTag.Mailbox.AssociatedMessageSize,
				PropTag.Mailbox.AssociatedMessageSize32,
				PropTag.Mailbox.FoldersCountWarningQuota,
				PropTag.Mailbox.FoldersCountReceiveQuota,
				PropTag.Mailbox.NamedPropertiesCountQuota,
				PropTag.Mailbox.CodePageId,
				PropTag.Mailbox.MailboxDSGuid,
				PropTag.Mailbox.MailboxDSGuidGuid,
				PropTag.Mailbox.DateDiscoveredAbsentInDS,
				PropTag.Mailbox.UnifiedMailboxGuidGuid,
				PropTag.Mailbox.RootFid,
				PropTag.Mailbox.MdbDSGuid,
				PropTag.Mailbox.MailboxOwnerDN,
				PropTag.Mailbox.MailboxOwnerDisplayName,
				PropTag.Mailbox.FolderIdsetIn,
				PropTag.Mailbox.CnsetIn,
				PropTag.Mailbox.MailboxMiscFlags,
				PropTag.Mailbox.MailboxNum
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[]
			{
				PropTag.Mailbox.LastLogonTime,
				PropTag.Mailbox.LastLogoffTime
			};

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[]
			{
				PropTag.Mailbox.LocalDirectoryEntryID,
				PropTag.Mailbox.InTransitStatus,
				PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyDepthWarningQuota,
				PropTag.Mailbox.FolderHierarchyDepthReceiveQuota,
				PropTag.Mailbox.FoldersCountWarningQuota,
				PropTag.Mailbox.FoldersCountReceiveQuota,
				PropTag.Mailbox.NamedPropertiesCountQuota
			};

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[]
			{
				PropTag.Mailbox.MessageSize,
				PropTag.Mailbox.MessageSize32,
				PropTag.Mailbox.Access,
				PropTag.Mailbox.MailboxPartitionMailboxGuids,
				PropTag.Mailbox.StoreState,
				PropTag.Mailbox.ContentCount,
				PropTag.Mailbox.ContentCountInt64,
				PropTag.Mailbox.AssociatedContentCount,
				PropTag.Mailbox.AssociatedContentCountInt64,
				PropTag.Mailbox.SerializedReplidGuidMap,
				PropTag.Mailbox.DeletedMsgCount,
				PropTag.Mailbox.DeletedMsgCountInt64,
				PropTag.Mailbox.DeletedAssocMsgCount,
				PropTag.Mailbox.DeletedAssocMsgCountInt64,
				PropTag.Mailbox.HasNamedProperties,
				PropTag.Mailbox.DeletedOn,
				PropTag.Mailbox.DeletedMessageSize,
				PropTag.Mailbox.DeletedMessageSize32,
				PropTag.Mailbox.DeletedNormalMessageSize,
				PropTag.Mailbox.DeletedNormalMessageSize32,
				PropTag.Mailbox.DeletedAssociatedMessageSize,
				PropTag.Mailbox.DeletedAssociatedMessageSize32,
				PropTag.Mailbox.NormalMessageSize,
				PropTag.Mailbox.NormalMessageSize32,
				PropTag.Mailbox.AssociatedMessageSize,
				PropTag.Mailbox.AssociatedMessageSize32,
				PropTag.Mailbox.MailboxMiscFlags
			};

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Mailbox.DeleteAfterSubmit,
				PropTag.Mailbox.MessageSize,
				PropTag.Mailbox.MessageSize32,
				PropTag.Mailbox.SentMailEntryId,
				PropTag.Mailbox.HighestFolderInternetId,
				PropTag.Mailbox.NTSecurityDescriptor,
				PropTag.Mailbox.CISearchEnabled,
				PropTag.Mailbox.ExtendedRuleSizeLimit,
				PropTag.Mailbox.Access,
				PropTag.Mailbox.MappingSignature,
				PropTag.Mailbox.StoreRecordKey,
				PropTag.Mailbox.StoreEntryId,
				PropTag.Mailbox.DisplayName,
				PropTag.Mailbox.EmailAddress,
				PropTag.Mailbox.Comment,
				PropTag.Mailbox.CreationTime,
				PropTag.Mailbox.LastModificationTime,
				PropTag.Mailbox.ResourceFlags,
				PropTag.Mailbox.MessageTableTotalPages,
				PropTag.Mailbox.MessageTableAvailablePages,
				PropTag.Mailbox.OtherTablesTotalPages,
				PropTag.Mailbox.OtherTablesAvailablePages,
				PropTag.Mailbox.AttachmentTableTotalPages,
				PropTag.Mailbox.AttachmentTableAvailablePages,
				PropTag.Mailbox.MailboxTypeVersion,
				PropTag.Mailbox.MailboxPartitionMailboxGuids,
				PropTag.Mailbox.StoreSupportMask,
				PropTag.Mailbox.StoreState,
				PropTag.Mailbox.IPMSubtreeSearchKey,
				PropTag.Mailbox.IPMOutboxSearchKey,
				PropTag.Mailbox.IPMWastebasketSearchKey,
				PropTag.Mailbox.IPMSentmailSearchKey,
				PropTag.Mailbox.MdbProvider,
				PropTag.Mailbox.ReceiveFolderSettings,
				PropTag.Mailbox.LocalDirectoryEntryID,
				PropTag.Mailbox.ControlDataForCalendarRepairAssistant,
				PropTag.Mailbox.ControlDataForSharingPolicyAssistant,
				PropTag.Mailbox.ControlDataForElcAssistant,
				PropTag.Mailbox.ControlDataForTopNWordsAssistant,
				PropTag.Mailbox.ControlDataForJunkEmailAssistant,
				PropTag.Mailbox.ControlDataForCalendarSyncAssistant,
				PropTag.Mailbox.ExternalSharingCalendarSubscriptionCount,
				PropTag.Mailbox.ControlDataForUMReportingAssistant,
				PropTag.Mailbox.HasUMReportData,
				PropTag.Mailbox.InternetCalendarSubscriptionCount,
				PropTag.Mailbox.ExternalSharingContactSubscriptionCount,
				PropTag.Mailbox.JunkEmailSafeListDirty,
				PropTag.Mailbox.IsTopNEnabled,
				PropTag.Mailbox.LastSharingPolicyAppliedId,
				PropTag.Mailbox.LastSharingPolicyAppliedHash,
				PropTag.Mailbox.LastSharingPolicyAppliedTime,
				PropTag.Mailbox.OofScheduleStart,
				PropTag.Mailbox.OofScheduleEnd,
				PropTag.Mailbox.ControlDataForDirectoryProcessorAssistant,
				PropTag.Mailbox.NeedsDirectoryProcessor,
				PropTag.Mailbox.RetentionQueryIds,
				PropTag.Mailbox.RetentionQueryInfo,
				PropTag.Mailbox.ControlDataForPublicFolderAssistant,
				PropTag.Mailbox.ControlDataForInferenceTrainingAssistant,
				PropTag.Mailbox.InferenceEnabled,
				PropTag.Mailbox.ControlDataForContactLinkingAssistant,
				PropTag.Mailbox.ContactLinking,
				PropTag.Mailbox.ControlDataForOABGeneratorAssistant,
				PropTag.Mailbox.ContactSaveVersion,
				PropTag.Mailbox.ControlDataForOrgContactsSyncAssistant,
				PropTag.Mailbox.OrgContactsSyncTimestamp,
				PropTag.Mailbox.PushNotificationSubscriptionType,
				PropTag.Mailbox.OrgContactsSyncADWatermark,
				PropTag.Mailbox.ControlDataForInferenceDataCollectionAssistant,
				PropTag.Mailbox.InferenceDataCollectionProcessingState,
				PropTag.Mailbox.ControlDataForPeopleRelevanceAssistant,
				PropTag.Mailbox.SiteMailboxInternalState,
				PropTag.Mailbox.ControlDataForSiteMailboxAssistant,
				PropTag.Mailbox.InferenceTrainingLastContentCount,
				PropTag.Mailbox.InferenceTrainingLastAttemptTimestamp,
				PropTag.Mailbox.InferenceTrainingLastSuccessTimestamp,
				PropTag.Mailbox.InferenceUserCapabilityFlags,
				PropTag.Mailbox.ControlDataForMailboxAssociationReplicationAssistant,
				PropTag.Mailbox.MailboxAssociationNextReplicationTime,
				PropTag.Mailbox.MailboxAssociationProcessingFlags,
				PropTag.Mailbox.ControlDataForSharePointSignalStoreAssistant,
				PropTag.Mailbox.ControlDataForPeopleCentricTriageAssistant,
				PropTag.Mailbox.NotificationBrokerSubscriptions,
				PropTag.Mailbox.GroupMailboxPermissionsVersion,
				PropTag.Mailbox.ElcLastRunTotalProcessingTime,
				PropTag.Mailbox.ElcLastRunSubAssistantProcessingTime,
				PropTag.Mailbox.ElcLastRunUpdatedFolderCount,
				PropTag.Mailbox.ElcLastRunTaggedFolderCount,
				PropTag.Mailbox.ElcLastRunUpdatedItemCount,
				PropTag.Mailbox.ElcLastRunTaggedWithArchiveItemCount,
				PropTag.Mailbox.ElcLastRunTaggedWithExpiryItemCount,
				PropTag.Mailbox.ElcLastRunDeletedFromRootItemCount,
				PropTag.Mailbox.ElcLastRunDeletedFromDumpsterItemCount,
				PropTag.Mailbox.ElcLastRunArchivedFromRootItemCount,
				PropTag.Mailbox.ElcLastRunArchivedFromDumpsterItemCount,
				PropTag.Mailbox.ScheduledISIntegLastFinished,
				PropTag.Mailbox.ControlDataForSearchIndexRepairAssistant,
				PropTag.Mailbox.ELCLastSuccessTimestamp,
				PropTag.Mailbox.InferenceTruthLoggingLastAttemptTimestamp,
				PropTag.Mailbox.InferenceTruthLoggingLastSuccessTimestamp,
				PropTag.Mailbox.ControlDataForGroupMailboxAssistant,
				PropTag.Mailbox.ItemsPendingUpgrade,
				PropTag.Mailbox.ConsumerSharingCalendarSubscriptionCount,
				PropTag.Mailbox.GroupMailboxGeneratedPhotoVersion,
				PropTag.Mailbox.GroupMailboxGeneratedPhotoSignature,
				PropTag.Mailbox.GroupMailboxExchangeResourcesPublishedVersion,
				PropTag.Mailbox.ValidFolderMask,
				PropTag.Mailbox.IPMSubtreeEntryId,
				PropTag.Mailbox.IPMOutboxEntryId,
				PropTag.Mailbox.IPMWastebasketEntryId,
				PropTag.Mailbox.IPMSentmailEntryId,
				PropTag.Mailbox.IPMViewsEntryId,
				PropTag.Mailbox.UnsearchableItems,
				PropTag.Mailbox.IPMFinderEntryId,
				PropTag.Mailbox.ContentCount,
				PropTag.Mailbox.ContentCountInt64,
				PropTag.Mailbox.Search,
				PropTag.Mailbox.AssociatedContentCount,
				PropTag.Mailbox.AssociatedContentCountInt64,
				PropTag.Mailbox.AdditionalRENEntryIds,
				PropTag.Mailbox.SimpleDisplayName,
				PropTag.Mailbox.TestBlobProperty,
				PropTag.Mailbox.ScheduledISIntegCorruptionCount,
				PropTag.Mailbox.ScheduledISIntegExecutionTime,
				PropTag.Mailbox.MailboxPartitionNumber,
				PropTag.Mailbox.MailboxTypeDetail,
				PropTag.Mailbox.InternalTenantHint,
				PropTag.Mailbox.TenantHint,
				PropTag.Mailbox.MaintenanceId,
				PropTag.Mailbox.MailboxType,
				PropTag.Mailbox.ACLData,
				PropTag.Mailbox.DesignInProgress,
				PropTag.Mailbox.StorageQuotaLimit,
				PropTag.Mailbox.RulesSize,
				PropTag.Mailbox.IMAPSubscribeList,
				PropTag.Mailbox.InTransitState,
				PropTag.Mailbox.InTransitStatus,
				PropTag.Mailbox.UserEntryId,
				PropTag.Mailbox.UserName,
				PropTag.Mailbox.MailboxOwnerEntryId,
				PropTag.Mailbox.MailboxOwnerName,
				PropTag.Mailbox.OofState,
				PropTag.Mailbox.TestLineSpeed,
				PropTag.Mailbox.SerializedReplidGuidMap,
				PropTag.Mailbox.DeletedMsgCount,
				PropTag.Mailbox.DeletedMsgCountInt64,
				PropTag.Mailbox.DeletedAssocMsgCount,
				PropTag.Mailbox.DeletedAssocMsgCountInt64,
				PropTag.Mailbox.HasNamedProperties,
				PropTag.Mailbox.ActiveUserEntryId,
				PropTag.Mailbox.ProhibitReceiveQuota,
				PropTag.Mailbox.MaxSubmitMessageSize,
				PropTag.Mailbox.ProhibitSendQuota,
				PropTag.Mailbox.DeletedOn,
				PropTag.Mailbox.MailboxDatabaseVersion,
				PropTag.Mailbox.DeletedMessageSize,
				PropTag.Mailbox.DeletedMessageSize32,
				PropTag.Mailbox.DeletedNormalMessageSize,
				PropTag.Mailbox.DeletedNormalMessageSize32,
				PropTag.Mailbox.DeletedAssociatedMessageSize,
				PropTag.Mailbox.DeletedAssociatedMessageSize32,
				PropTag.Mailbox.NTUsername,
				PropTag.Mailbox.NTUserSid,
				PropTag.Mailbox.LocaleId,
				PropTag.Mailbox.LastLogonTime,
				PropTag.Mailbox.LastLogoffTime,
				PropTag.Mailbox.StorageLimitInformation,
				PropTag.Mailbox.InternetMdns,
				PropTag.Mailbox.MailboxStatus,
				PropTag.Mailbox.MailboxFlags,
				PropTag.Mailbox.PreservingMailboxSignature,
				PropTag.Mailbox.MRSPreservingMailboxSignature,
				PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota,
				PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota,
				PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota,
				PropTag.Mailbox.FolderHierarchyDepthWarningQuota,
				PropTag.Mailbox.FolderHierarchyDepthReceiveQuota,
				PropTag.Mailbox.NormalMessageSize,
				PropTag.Mailbox.NormalMessageSize32,
				PropTag.Mailbox.AssociatedMessageSize,
				PropTag.Mailbox.AssociatedMessageSize32,
				PropTag.Mailbox.FoldersCountWarningQuota,
				PropTag.Mailbox.FoldersCountReceiveQuota,
				PropTag.Mailbox.NamedPropertiesCountQuota,
				PropTag.Mailbox.CodePageId,
				PropTag.Mailbox.RetentionAgeLimit,
				PropTag.Mailbox.UserDisplayName,
				PropTag.Mailbox.SortLocaleId,
				PropTag.Mailbox.MailboxDSGuid,
				PropTag.Mailbox.MailboxDSGuidGuid,
				PropTag.Mailbox.DateDiscoveredAbsentInDS,
				PropTag.Mailbox.UnifiedMailboxGuidGuid,
				PropTag.Mailbox.QuotaWarningThreshold,
				PropTag.Mailbox.QuotaSendThreshold,
				PropTag.Mailbox.QuotaReceiveThreshold,
				PropTag.Mailbox.PropertyGroupMappingId,
				PropTag.Mailbox.SentMailSvrEID,
				PropTag.Mailbox.SentMailSvrEIDBin,
				PropTag.Mailbox.LocalIdNext,
				PropTag.Mailbox.RootFid,
				PropTag.Mailbox.FIDC,
				PropTag.Mailbox.MdbDSGuid,
				PropTag.Mailbox.MailboxOwnerDN,
				PropTag.Mailbox.MapiEntryIdGuid,
				PropTag.Mailbox.Localized,
				PropTag.Mailbox.LCID,
				PropTag.Mailbox.AltRecipientDN,
				PropTag.Mailbox.NoLocalDelivery,
				PropTag.Mailbox.DeliveryContentLength,
				PropTag.Mailbox.AutoReply,
				PropTag.Mailbox.MailboxOwnerDisplayName,
				PropTag.Mailbox.MailboxLastUpdated,
				PropTag.Mailbox.AdminSurName,
				PropTag.Mailbox.AdminGivenName,
				PropTag.Mailbox.ActiveSearchCount,
				PropTag.Mailbox.AdminNickname,
				PropTag.Mailbox.QuotaStyle,
				PropTag.Mailbox.OverQuotaLimit,
				PropTag.Mailbox.StorageQuota,
				PropTag.Mailbox.SubmitContentLength,
				PropTag.Mailbox.ReservedIdCounterRangeUpperLimit,
				PropTag.Mailbox.ReservedCnCounterRangeUpperLimit,
				PropTag.Mailbox.FolderIdsetIn,
				PropTag.Mailbox.CnsetIn,
				PropTag.Mailbox.ShutoffQuota,
				PropTag.Mailbox.MailboxMiscFlags,
				PropTag.Mailbox.MailboxInCreation,
				PropTag.Mailbox.ObjectClassFlags,
				PropTag.Mailbox.OOFStateEx,
				PropTag.Mailbox.OofStateUserChangeTime,
				PropTag.Mailbox.UserOofSettingsItemId,
				PropTag.Mailbox.MailboxQuarantined,
				PropTag.Mailbox.MailboxQuarantineDescription,
				PropTag.Mailbox.MailboxQuarantineLastCrash,
				PropTag.Mailbox.MailboxQuarantineEnd,
				PropTag.Mailbox.MailboxNum,
				PropTag.Mailbox.MaxMessageSize,
				PropTag.Mailbox.InferenceClientActivityFlags,
				PropTag.Mailbox.InferenceOWAUserActivityLoggingEnabledDeprecated,
				PropTag.Mailbox.InferenceOLKUserActivityLoggingEnabled,
				PropTag.Mailbox.InferenceTrainedModelVersionBreadCrumb,
				PropTag.Mailbox.UserPhotoCacheId,
				PropTag.Mailbox.UserPhotoPreviewCacheId
			};
		}

		public static class Folder
		{
			public static readonly StorePropTag MessageClass = new StorePropTag(26, PropertyType.Unicode, new StorePropInfo("MessageClass", 26, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MessageSize = new StorePropTag(3592, PropertyType.Int64, new StorePropInfo("MessageSize", 3592, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MessageSize32 = new StorePropTag(3592, PropertyType.Int32, new StorePropInfo("MessageSize32", 3592, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ParentEntryId = new StorePropTag(3593, PropertyType.Binary, new StorePropInfo("ParentEntryId", 3593, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ParentEntryIdSvrEid = new StorePropTag(3593, PropertyType.SvrEid, new StorePropInfo("ParentEntryIdSvrEid", 3593, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SentMailEntryId = new StorePropTag(3594, PropertyType.Binary, new StorePropInfo("SentMailEntryId", 3594, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MessageDownloadTime = new StorePropTag(3608, PropertyType.Int32, new StorePropInfo("MessageDownloadTime", 3608, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderInternetId = new StorePropTag(3619, PropertyType.Int32, new StorePropInfo("FolderInternetId", 3619, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NTSecurityDescriptor = new StorePropTag(3623, PropertyType.Binary, new StorePropInfo("NTSecurityDescriptor", 3623, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AclTableAndSecurityDescriptor = new StorePropTag(3647, PropertyType.Binary, new StorePropInfo("AclTableAndSecurityDescriptor", 3647, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9, 17)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CreatorSID = new StorePropTag(3672, PropertyType.Binary, new StorePropInfo("CreatorSID", 3672, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 11)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastModifierSid = new StorePropTag(3673, PropertyType.Binary, new StorePropInfo("LastModifierSid", 3673, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Catalog = new StorePropTag(3675, PropertyType.Binary, new StorePropInfo("Catalog", 3675, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CISearchEnabled = new StorePropTag(3676, PropertyType.Boolean, new StorePropInfo("CISearchEnabled", 3676, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CINotificationEnabled = new StorePropTag(3677, PropertyType.Boolean, new StorePropInfo("CINotificationEnabled", 3677, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MaxIndices = new StorePropTag(3678, PropertyType.Int32, new StorePropInfo("MaxIndices", 3678, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SourceFid = new StorePropTag(3679, PropertyType.Int64, new StorePropInfo("SourceFid", 3679, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFContactsGuid = new StorePropTag(3680, PropertyType.Binary, new StorePropInfo("PFContactsGuid", 3680, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SubfolderCount = new StorePropTag(3683, PropertyType.Int32, new StorePropInfo("SubfolderCount", 3683, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedSubfolderCt = new StorePropTag(3684, PropertyType.Int32, new StorePropInfo("DeletedSubfolderCt", 3684, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MaxCachedViews = new StorePropTag(3688, PropertyType.Int32, new StorePropInfo("MaxCachedViews", 3688, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NTSecurityDescriptorAsXML = new StorePropTag(3690, PropertyType.Unicode, new StorePropInfo("NTSecurityDescriptorAsXML", 3690, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdminNTSecurityDescriptorAsXML = new StorePropTag(3691, PropertyType.Unicode, new StorePropInfo("AdminNTSecurityDescriptorAsXML", 3691, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CreatorSidAsXML = new StorePropTag(3692, PropertyType.Unicode, new StorePropInfo("CreatorSidAsXML", 3692, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastModifierSidAsXML = new StorePropTag(3693, PropertyType.Unicode, new StorePropInfo("LastModifierSidAsXML", 3693, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MergeMidsetDeleted = new StorePropTag(3706, PropertyType.Binary, new StorePropInfo("MergeMidsetDeleted", 3706, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReserveRangeOfIDs = new StorePropTag(3707, PropertyType.Binary, new StorePropInfo("ReserveRangeOfIDs", 3707, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FreeBusyNTSD = new StorePropTag(3840, PropertyType.Binary, new StorePropInfo("FreeBusyNTSD", 3840, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Access = new StorePropTag(4084, PropertyType.Int32, new StorePropInfo("Access", 4084, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag InstanceKey = new StorePropTag(4086, PropertyType.Binary, new StorePropInfo("InstanceKey", 4086, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag InstanceKeySvrEid = new StorePropTag(4086, PropertyType.SvrEid, new StorePropInfo("InstanceKeySvrEid", 4086, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AccessLevel = new StorePropTag(4087, PropertyType.Int32, new StorePropInfo("AccessLevel", 4087, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MappingSignature = new StorePropTag(4088, PropertyType.Binary, new StorePropInfo("MappingSignature", 4088, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecordKey = new StorePropTag(4089, PropertyType.Binary, new StorePropInfo("RecordKey", 4089, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecordKeySvrEid = new StorePropTag(4089, PropertyType.SvrEid, new StorePropInfo("RecordKeySvrEid", 4089, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StoreRecordKey = new StorePropTag(4090, PropertyType.Binary, new StorePropInfo("StoreRecordKey", 4090, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StoreEntryId = new StorePropTag(4091, PropertyType.Binary, new StorePropInfo("StoreEntryId", 4091, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ObjectType = new StorePropTag(4094, PropertyType.Int32, new StorePropInfo("ObjectType", 4094, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EntryId = new StorePropTag(4095, PropertyType.Binary, new StorePropInfo("EntryId", 4095, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EntryIdSvrEid = new StorePropTag(4095, PropertyType.SvrEid, new StorePropInfo("EntryIdSvrEid", 4095, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag URLCompName = new StorePropTag(4339, PropertyType.Unicode, new StorePropInfo("URLCompName", 4339, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttrHidden = new StorePropTag(4340, PropertyType.Boolean, new StorePropInfo("AttrHidden", 4340, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttrSystem = new StorePropTag(4341, PropertyType.Boolean, new StorePropInfo("AttrSystem", 4341, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttrReadOnly = new StorePropTag(4342, PropertyType.Boolean, new StorePropInfo("AttrReadOnly", 4342, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EmailAddress = new StorePropTag(12291, PropertyType.Unicode, new StorePropInfo("EmailAddress", 12291, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Comment = new StorePropTag(12292, PropertyType.Unicode, new StorePropInfo("Comment", 12292, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Depth = new StorePropTag(12293, PropertyType.Int32, new StorePropInfo("Depth", 12293, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CreationTime = new StorePropTag(12295, PropertyType.SysTime, new StorePropInfo("CreationTime", 12295, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 11)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastModificationTime = new StorePropTag(12296, PropertyType.SysTime, new StorePropInfo("LastModificationTime", 12296, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 11)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StoreSupportMask = new StorePropTag(13325, PropertyType.Int32, new StorePropInfo("StoreSupportMask", 13325, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMWastebasketEntryId = new StorePropTag(13795, PropertyType.Binary, new StorePropInfo("IPMWastebasketEntryId", 13795, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMCommonViewsEntryId = new StorePropTag(13798, PropertyType.Binary, new StorePropInfo("IPMCommonViewsEntryId", 13798, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMConversationsEntryId = new StorePropTag(13804, PropertyType.Binary, new StorePropInfo("IPMConversationsEntryId", 13804, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMAllItemsEntryId = new StorePropTag(13806, PropertyType.Binary, new StorePropInfo("IPMAllItemsEntryId", 13806, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMSharingEntryId = new StorePropTag(13807, PropertyType.Binary, new StorePropInfo("IPMSharingEntryId", 13807, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdminDataEntryId = new StorePropTag(13821, PropertyType.Binary, new StorePropInfo("AdminDataEntryId", 13821, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderType = new StorePropTag(13825, PropertyType.Int32, new StorePropInfo("FolderType", 13825, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContentCount = new StorePropTag(13826, PropertyType.Int32, new StorePropInfo("ContentCount", 13826, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContentCountInt64 = new StorePropTag(13826, PropertyType.Int64, new StorePropInfo("ContentCountInt64", 13826, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag UnreadCount = new StorePropTag(13827, PropertyType.Int32, new StorePropInfo("UnreadCount", 13827, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag UnreadCountInt64 = new StorePropTag(13827, PropertyType.Int64, new StorePropInfo("UnreadCountInt64", 13827, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Subfolders = new StorePropTag(13834, PropertyType.Boolean, new StorePropInfo("Subfolders", 13834, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderStatus = new StorePropTag(13835, PropertyType.Int32, new StorePropInfo("FolderStatus", 13835, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContentsSortOrder = new StorePropTag(13837, PropertyType.MVInt32, new StorePropInfo("ContentsSortOrder", 13837, PropertyType.MVInt32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContainerHierarchy = new StorePropTag(13838, PropertyType.Object, new StorePropInfo("ContainerHierarchy", 13838, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContainerContents = new StorePropTag(13839, PropertyType.Object, new StorePropInfo("ContainerContents", 13839, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderAssociatedContents = new StorePropTag(13840, PropertyType.Object, new StorePropInfo("FolderAssociatedContents", 13840, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContainerClass = new StorePropTag(13843, PropertyType.Unicode, new StorePropInfo("ContainerClass", 13843, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContainerModifyVersion = new StorePropTag(13844, PropertyType.Int64, new StorePropInfo("ContainerModifyVersion", 13844, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DefaultViewEntryId = new StorePropTag(13846, PropertyType.Binary, new StorePropInfo("DefaultViewEntryId", 13846, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssociatedContentCount = new StorePropTag(13847, PropertyType.Int32, new StorePropInfo("AssociatedContentCount", 13847, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssociatedContentCountInt64 = new StorePropTag(13847, PropertyType.Int64, new StorePropInfo("AssociatedContentCountInt64", 13847, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PackedNamedProps = new StorePropTag(13852, PropertyType.Binary, new StorePropInfo("PackedNamedProps", 13852, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AllowAgeOut = new StorePropTag(13855, PropertyType.Boolean, new StorePropInfo("AllowAgeOut", 13855, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchFolderMsgCount = new StorePropTag(13892, PropertyType.Int32, new StorePropInfo("SearchFolderMsgCount", 13892, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PartOfContentIndexing = new StorePropTag(13893, PropertyType.Boolean, new StorePropInfo("PartOfContentIndexing", 13893, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag OwnerLogonUserConfigurationCache = new StorePropTag(13894, PropertyType.Binary, new StorePropInfo("OwnerLogonUserConfigurationCache", 13894, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchFolderAgeOutTimeout = new StorePropTag(13895, PropertyType.Int32, new StorePropInfo("SearchFolderAgeOutTimeout", 13895, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchFolderPopulationResult = new StorePropTag(13896, PropertyType.Int32, new StorePropInfo("SearchFolderPopulationResult", 13896, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchFolderPopulationDiagnostics = new StorePropTag(13897, PropertyType.Binary, new StorePropInfo("SearchFolderPopulationDiagnostics", 13897, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ConversationTopicHashEntries = new StorePropTag(13920, PropertyType.Binary, new StorePropInfo("ConversationTopicHashEntries", 13920, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContentAggregationFlags = new StorePropTag(13967, PropertyType.Int32, new StorePropInfo("ContentAggregationFlags", 13967, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag TransportRulesSnapshot = new StorePropTag(13968, PropertyType.Binary, new StorePropInfo("TransportRulesSnapshot", 13968, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 6, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag TransportRulesSnapshotId = new StorePropTag(13969, PropertyType.Guid, new StorePropInfo("TransportRulesSnapshotId", 13969, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 6, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CurrentIPMWasteBasketContainerEntryId = new StorePropTag(14031, PropertyType.Binary, new StorePropInfo("CurrentIPMWasteBasketContainerEntryId", 14031, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMAppointmentEntryId = new StorePropTag(14032, PropertyType.Binary, new StorePropInfo("IPMAppointmentEntryId", 14032, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMContactEntryId = new StorePropTag(14033, PropertyType.Binary, new StorePropInfo("IPMContactEntryId", 14033, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMJournalEntryId = new StorePropTag(14034, PropertyType.Binary, new StorePropInfo("IPMJournalEntryId", 14034, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMNoteEntryId = new StorePropTag(14035, PropertyType.Binary, new StorePropInfo("IPMNoteEntryId", 14035, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMTaskEntryId = new StorePropTag(14036, PropertyType.Binary, new StorePropInfo("IPMTaskEntryId", 14036, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag REMOnlineEntryId = new StorePropTag(14037, PropertyType.Binary, new StorePropInfo("REMOnlineEntryId", 14037, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMOfflineEntryId = new StorePropTag(14038, PropertyType.Binary, new StorePropInfo("IPMOfflineEntryId", 14038, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMDraftsEntryId = new StorePropTag(14039, PropertyType.Binary, new StorePropInfo("IPMDraftsEntryId", 14039, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdditionalRENEntryIds = new StorePropTag(14040, PropertyType.MVBinary, new StorePropInfo("AdditionalRENEntryIds", 14040, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdditionalRENEntryIdsExtended = new StorePropTag(14041, PropertyType.Binary, new StorePropInfo("AdditionalRENEntryIdsExtended", 14041, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdditionalRENEntryIdsExtendedMV = new StorePropTag(14041, PropertyType.MVBinary, new StorePropInfo("AdditionalRENEntryIdsExtendedMV", 14041, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ExtendedFolderFlags = new StorePropTag(14042, PropertyType.Binary, new StorePropInfo("ExtendedFolderFlags", 14042, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContainerTimestamp = new StorePropTag(14043, PropertyType.SysTime, new StorePropInfo("ContainerTimestamp", 14043, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag INetUnread = new StorePropTag(14045, PropertyType.Int32, new StorePropInfo("INetUnread", 14045, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NetFolderFlags = new StorePropTag(14046, PropertyType.Int32, new StorePropInfo("NetFolderFlags", 14046, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderWebViewInfo = new StorePropTag(14047, PropertyType.Binary, new StorePropInfo("FolderWebViewInfo", 14047, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderWebViewInfoExtended = new StorePropTag(14048, PropertyType.Binary, new StorePropInfo("FolderWebViewInfoExtended", 14048, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderViewFlags = new StorePropTag(14049, PropertyType.Int32, new StorePropInfo("FolderViewFlags", 14049, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FreeBusyEntryIds = new StorePropTag(14052, PropertyType.MVBinary, new StorePropInfo("FreeBusyEntryIds", 14052, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DefaultPostMsgClass = new StorePropTag(14053, PropertyType.Unicode, new StorePropInfo("DefaultPostMsgClass", 14053, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DefaultPostDisplayName = new StorePropTag(14054, PropertyType.Unicode, new StorePropInfo("DefaultPostDisplayName", 14054, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderViewList = new StorePropTag(14059, PropertyType.Binary, new StorePropInfo("FolderViewList", 14059, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AgingPeriod = new StorePropTag(14060, PropertyType.Int32, new StorePropInfo("AgingPeriod", 14060, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AgingGranularity = new StorePropTag(14062, PropertyType.Int32, new StorePropInfo("AgingGranularity", 14062, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DefaultFoldersLocaleId = new StorePropTag(14064, PropertyType.Int32, new StorePropInfo("DefaultFoldersLocaleId", 14064, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag InternalAccess = new StorePropTag(14065, PropertyType.Boolean, new StorePropInfo("InternalAccess", 14065, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SyncEventSuppressGuid = new StorePropTag(14464, PropertyType.Binary, new StorePropInfo("SyncEventSuppressGuid", 14464, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DisplayType = new StorePropTag(14592, PropertyType.Int32, new StorePropInfo("DisplayType", 14592, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag TestBlobProperty = new StorePropTag(15616, PropertyType.Int64, new StorePropInfo("TestBlobProperty", 15616, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdminSecurityDescriptor = new StorePropTag(15649, PropertyType.Binary, new StorePropInfo("AdminSecurityDescriptor", 15649, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Win32NTSecurityDescriptor = new StorePropTag(15650, PropertyType.Binary, new StorePropInfo("Win32NTSecurityDescriptor", 15650, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NonWin32ACL = new StorePropTag(15651, PropertyType.Boolean, new StorePropInfo("NonWin32ACL", 15651, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ItemLevelACL = new StorePropTag(15652, PropertyType.Boolean, new StorePropInfo("ItemLevelACL", 15652, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ICSGid = new StorePropTag(15662, PropertyType.Binary, new StorePropInfo("ICSGid", 15662, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SystemFolderFlags = new StorePropTag(15663, PropertyType.Int32, new StorePropInfo("SystemFolderFlags", 15663, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MaterializedRestrictionSearchRoot = new StorePropTag(15772, PropertyType.Binary, new StorePropInfo("MaterializedRestrictionSearchRoot", 15772, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MailboxPartitionNumber = new StorePropTag(15775, PropertyType.Int32, new StorePropInfo("MailboxPartitionNumber", 15775, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MailboxNumberInternal = new StorePropTag(15776, PropertyType.Int32, new StorePropInfo("MailboxNumberInternal", 15776, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag QueryCriteriaInternal = new StorePropTag(15777, PropertyType.Binary, new StorePropInfo("QueryCriteriaInternal", 15777, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastQuotaNotificationTime = new StorePropTag(15778, PropertyType.SysTime, new StorePropInfo("LastQuotaNotificationTime", 15778, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PropertyPromotionInProgressHiddenItems = new StorePropTag(15779, PropertyType.Boolean, new StorePropInfo("PropertyPromotionInProgressHiddenItems", 15779, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PropertyPromotionInProgressNormalItems = new StorePropTag(15780, PropertyType.Boolean, new StorePropInfo("PropertyPromotionInProgressNormalItems", 15780, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag VirtualUnreadMessageCount = new StorePropTag(15787, PropertyType.Int64, new StorePropInfo("VirtualUnreadMessageCount", 15787, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag InternalChangeKey = new StorePropTag(15806, PropertyType.Binary, new StorePropInfo("InternalChangeKey", 15806, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag InternalSourceKey = new StorePropTag(15807, PropertyType.Binary, new StorePropInfo("InternalSourceKey", 15807, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				16
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CorrelationId = new StorePropTag(15825, PropertyType.Guid, new StorePropInfo("CorrelationId", 15825, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastConflict = new StorePropTag(16329, PropertyType.Binary, new StorePropInfo("LastConflict", 16329, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NTSDModificationTime = new StorePropTag(16342, PropertyType.SysTime, new StorePropInfo("NTSDModificationTime", 16342, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ACLDataChecksum = new StorePropTag(16343, PropertyType.Int32, new StorePropInfo("ACLDataChecksum", 16343, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ACLData = new StorePropTag(16352, PropertyType.Binary, new StorePropInfo("ACLData", 16352, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ACLTable = new StorePropTag(16352, PropertyType.Object, new StorePropInfo("ACLTable", 16352, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RulesData = new StorePropTag(16353, PropertyType.Binary, new StorePropInfo("RulesData", 16353, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RulesTable = new StorePropTag(16353, PropertyType.Object, new StorePropInfo("RulesTable", 16353, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag OofHistory = new StorePropTag(16355, PropertyType.Binary, new StorePropInfo("OofHistory", 16355, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(17)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DesignInProgress = new StorePropTag(16356, PropertyType.Boolean, new StorePropInfo("DesignInProgress", 16356, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SecureOrigination = new StorePropTag(16357, PropertyType.Boolean, new StorePropInfo("SecureOrigination", 16357, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PublishInAddressBook = new StorePropTag(16358, PropertyType.Boolean, new StorePropInfo("PublishInAddressBook", 16358, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ResolveMethod = new StorePropTag(16359, PropertyType.Int32, new StorePropInfo("ResolveMethod", 16359, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AddressBookDisplayName = new StorePropTag(16360, PropertyType.Unicode, new StorePropInfo("AddressBookDisplayName", 16360, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EFormsLocaleId = new StorePropTag(16361, PropertyType.Int32, new StorePropInfo("EFormsLocaleId", 16361, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ExtendedACLData = new StorePropTag(16382, PropertyType.Binary, new StorePropInfo("ExtendedACLData", 16382, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RulesSize = new StorePropTag(16383, PropertyType.Int32, new StorePropInfo("RulesSize", 16383, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NewAttach = new StorePropTag(16384, PropertyType.Int32, new StorePropInfo("NewAttach", 16384, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartEmbed = new StorePropTag(16385, PropertyType.Int32, new StorePropInfo("StartEmbed", 16385, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndEmbed = new StorePropTag(16386, PropertyType.Int32, new StorePropInfo("EndEmbed", 16386, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartRecip = new StorePropTag(16387, PropertyType.Int32, new StorePropInfo("StartRecip", 16387, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndRecip = new StorePropTag(16388, PropertyType.Int32, new StorePropInfo("EndRecip", 16388, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndCcRecip = new StorePropTag(16389, PropertyType.Int32, new StorePropInfo("EndCcRecip", 16389, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndBccRecip = new StorePropTag(16390, PropertyType.Int32, new StorePropInfo("EndBccRecip", 16390, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndP1Recip = new StorePropTag(16391, PropertyType.Int32, new StorePropInfo("EndP1Recip", 16391, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DNPrefix = new StorePropTag(16392, PropertyType.Unicode, new StorePropInfo("DNPrefix", 16392, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartTopFolder = new StorePropTag(16393, PropertyType.Int32, new StorePropInfo("StartTopFolder", 16393, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartSubFolder = new StorePropTag(16394, PropertyType.Int32, new StorePropInfo("StartSubFolder", 16394, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndFolder = new StorePropTag(16395, PropertyType.Int32, new StorePropInfo("EndFolder", 16395, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartMessage = new StorePropTag(16396, PropertyType.Int32, new StorePropInfo("StartMessage", 16396, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndMessage = new StorePropTag(16397, PropertyType.Int32, new StorePropInfo("EndMessage", 16397, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EndAttach = new StorePropTag(16398, PropertyType.Int32, new StorePropInfo("EndAttach", 16398, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag EcWarning = new StorePropTag(16399, PropertyType.Int32, new StorePropInfo("EcWarning", 16399, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StartFAIMessage = new StorePropTag(16400, PropertyType.Int32, new StorePropInfo("StartFAIMessage", 16400, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NewFXFolder = new StorePropTag(16401, PropertyType.Binary, new StorePropInfo("NewFXFolder", 16401, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncChange = new StorePropTag(16402, PropertyType.Int32, new StorePropInfo("IncrSyncChange", 16402, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncDelete = new StorePropTag(16403, PropertyType.Int32, new StorePropInfo("IncrSyncDelete", 16403, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncEnd = new StorePropTag(16404, PropertyType.Int32, new StorePropInfo("IncrSyncEnd", 16404, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncMessage = new StorePropTag(16405, PropertyType.Int32, new StorePropInfo("IncrSyncMessage", 16405, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FastTransferDelProp = new StorePropTag(16406, PropertyType.Int32, new StorePropInfo("FastTransferDelProp", 16406, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IdsetGiven = new StorePropTag(16407, PropertyType.Binary, new StorePropInfo("IdsetGiven", 16407, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IdsetGivenInt32 = new StorePropTag(16407, PropertyType.Int32, new StorePropInfo("IdsetGivenInt32", 16407, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FastTransferErrorInfo = new StorePropTag(16408, PropertyType.Int32, new StorePropInfo("FastTransferErrorInfo", 16408, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SoftDeletes = new StorePropTag(16417, PropertyType.Binary, new StorePropInfo("SoftDeletes", 16417, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IdsetRead = new StorePropTag(16429, PropertyType.Binary, new StorePropInfo("IdsetRead", 16429, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IdsetUnread = new StorePropTag(16430, PropertyType.Binary, new StorePropInfo("IdsetUnread", 16430, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncRead = new StorePropTag(16431, PropertyType.Int32, new StorePropInfo("IncrSyncRead", 16431, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncStateBegin = new StorePropTag(16442, PropertyType.Int32, new StorePropInfo("IncrSyncStateBegin", 16442, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncStateEnd = new StorePropTag(16443, PropertyType.Int32, new StorePropInfo("IncrSyncStateEnd", 16443, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncImailStream = new StorePropTag(16444, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream", 16444, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncImailStreamContinue = new StorePropTag(16486, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamContinue", 16486, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncImailStreamCancel = new StorePropTag(16487, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamCancel", 16487, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncImailStream2Continue = new StorePropTag(16497, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream2Continue", 16497, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncProgressMode = new StorePropTag(16500, PropertyType.Boolean, new StorePropInfo("IncrSyncProgressMode", 16500, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SyncProgressPerMsg = new StorePropTag(16501, PropertyType.Boolean, new StorePropInfo("SyncProgressPerMsg", 16501, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncMsgPartial = new StorePropTag(16506, PropertyType.Int32, new StorePropInfo("IncrSyncMsgPartial", 16506, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncGroupInfo = new StorePropTag(16507, PropertyType.Int32, new StorePropInfo("IncrSyncGroupInfo", 16507, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncGroupId = new StorePropTag(16508, PropertyType.Int32, new StorePropInfo("IncrSyncGroupId", 16508, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IncrSyncChangePartial = new StorePropTag(16509, PropertyType.Int32, new StorePropInfo("IncrSyncChangePartial", 16509, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HierRev = new StorePropTag(16514, PropertyType.SysTime, new StorePropInfo("HierRev", 16514, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 5, 9, 11)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SourceKey = new StorePropTag(26080, PropertyType.Binary, new StorePropInfo("SourceKey", 26080, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ParentSourceKey = new StorePropTag(26081, PropertyType.Binary, new StorePropInfo("ParentSourceKey", 26081, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ChangeKey = new StorePropTag(26082, PropertyType.Binary, new StorePropInfo("ChangeKey", 26082, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PredecessorChangeList = new StorePropTag(26083, PropertyType.Binary, new StorePropInfo("PredecessorChangeList", 26083, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PreventMsgCreate = new StorePropTag(26100, PropertyType.Boolean, new StorePropInfo("PreventMsgCreate", 26100, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LISSD = new StorePropTag(26105, PropertyType.Binary, new StorePropInfo("LISSD", 26105, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FavoritesDefaultName = new StorePropTag(26165, PropertyType.Unicode, new StorePropInfo("FavoritesDefaultName", 26165, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderChildCount = new StorePropTag(26168, PropertyType.Int32, new StorePropInfo("FolderChildCount", 26168, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderChildCountInt64 = new StorePropTag(26168, PropertyType.Int64, new StorePropInfo("FolderChildCountInt64", 26168, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Rights = new StorePropTag(26169, PropertyType.Int32, new StorePropInfo("Rights", 26169, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HasRules = new StorePropTag(26170, PropertyType.Boolean, new StorePropInfo("HasRules", 26170, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AddressBookEntryId = new StorePropTag(26171, PropertyType.Binary, new StorePropInfo("AddressBookEntryId", 26171, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HierarchyChangeNumber = new StorePropTag(26174, PropertyType.Int32, new StorePropInfo("HierarchyChangeNumber", 26174, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 16)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HasModeratorRules = new StorePropTag(26175, PropertyType.Boolean, new StorePropInfo("HasModeratorRules", 26175, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ModeratorRuleCount = new StorePropTag(26175, PropertyType.Int32, new StorePropInfo("ModeratorRuleCount", 26175, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedMsgCount = new StorePropTag(26176, PropertyType.Int32, new StorePropInfo("DeletedMsgCount", 26176, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedMsgCountInt64 = new StorePropTag(26176, PropertyType.Int64, new StorePropInfo("DeletedMsgCountInt64", 26176, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedFolderCount = new StorePropTag(26177, PropertyType.Int32, new StorePropInfo("DeletedFolderCount", 26177, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedAssocMsgCount = new StorePropTag(26179, PropertyType.Int32, new StorePropInfo("DeletedAssocMsgCount", 26179, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedAssocMsgCountInt64 = new StorePropTag(26179, PropertyType.Int64, new StorePropInfo("DeletedAssocMsgCountInt64", 26179, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PromotedProperties = new StorePropTag(26181, PropertyType.Binary, new StorePropInfo("PromotedProperties", 26181, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HiddenPromotedProperties = new StorePropTag(26182, PropertyType.Binary, new StorePropInfo("HiddenPromotedProperties", 26182, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LinkedSiteAuthorityUrl = new StorePropTag(26183, PropertyType.Unicode, new StorePropInfo("LinkedSiteAuthorityUrl", 26183, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag HasNamedProperties = new StorePropTag(26186, PropertyType.Boolean, new StorePropInfo("HasNamedProperties", 26186, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FidMid = new StorePropTag(26188, PropertyType.Binary, new StorePropInfo("FidMid", 26188, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ICSChangeKey = new StorePropTag(26197, PropertyType.Binary, new StorePropInfo("ICSChangeKey", 26197, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SetPropsCondition = new StorePropTag(26199, PropertyType.Binary, new StorePropInfo("SetPropsCondition", 26199, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedOn = new StorePropTag(26255, PropertyType.SysTime, new StorePropInfo("DeletedOn", 26255, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicationStyle = new StorePropTag(26256, PropertyType.Int32, new StorePropInfo("ReplicationStyle", 26256, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicationTIB = new StorePropTag(26257, PropertyType.Binary, new StorePropInfo("ReplicationTIB", 26257, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicationMsgPriority = new StorePropTag(26258, PropertyType.Int32, new StorePropInfo("ReplicationMsgPriority", 26258, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicaList = new StorePropTag(26264, PropertyType.Binary, new StorePropInfo("ReplicaList", 26264, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(17)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag OverallAgeLimit = new StorePropTag(26265, PropertyType.Int32, new StorePropInfo("OverallAgeLimit", 26265, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedMessageSize = new StorePropTag(26267, PropertyType.Int64, new StorePropInfo("DeletedMessageSize", 26267, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedMessageSize32 = new StorePropTag(26267, PropertyType.Int32, new StorePropInfo("DeletedMessageSize32", 26267, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedNormalMessageSize = new StorePropTag(26268, PropertyType.Int64, new StorePropInfo("DeletedNormalMessageSize", 26268, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedNormalMessageSize32 = new StorePropTag(26268, PropertyType.Int32, new StorePropInfo("DeletedNormalMessageSize32", 26268, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedAssociatedMessageSize = new StorePropTag(26269, PropertyType.Int64, new StorePropInfo("DeletedAssociatedMessageSize", 26269, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedAssociatedMessageSize32 = new StorePropTag(26269, PropertyType.Int32, new StorePropInfo("DeletedAssociatedMessageSize32", 26269, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SecureInSite = new StorePropTag(26270, PropertyType.Boolean, new StorePropInfo("SecureInSite", 26270, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderFlags = new StorePropTag(26280, PropertyType.Int32, new StorePropInfo("FolderFlags", 26280, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastAccessTime = new StorePropTag(26281, PropertyType.SysTime, new StorePropInfo("LastAccessTime", 26281, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NormalMsgWithAttachCount = new StorePropTag(26285, PropertyType.Int32, new StorePropInfo("NormalMsgWithAttachCount", 26285, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NormalMsgWithAttachCountInt64 = new StorePropTag(26285, PropertyType.Int64, new StorePropInfo("NormalMsgWithAttachCountInt64", 26285, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssocMsgWithAttachCount = new StorePropTag(26286, PropertyType.Int32, new StorePropInfo("AssocMsgWithAttachCount", 26286, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssocMsgWithAttachCountInt64 = new StorePropTag(26286, PropertyType.Int64, new StorePropInfo("AssocMsgWithAttachCountInt64", 26286, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecipientOnNormalMsgCount = new StorePropTag(26287, PropertyType.Int32, new StorePropInfo("RecipientOnNormalMsgCount", 26287, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecipientOnNormalMsgCountInt64 = new StorePropTag(26287, PropertyType.Int64, new StorePropInfo("RecipientOnNormalMsgCountInt64", 26287, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecipientOnAssocMsgCount = new StorePropTag(26288, PropertyType.Int32, new StorePropInfo("RecipientOnAssocMsgCount", 26288, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecipientOnAssocMsgCountInt64 = new StorePropTag(26288, PropertyType.Int64, new StorePropInfo("RecipientOnAssocMsgCountInt64", 26288, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttachOnNormalMsgCt = new StorePropTag(26289, PropertyType.Int32, new StorePropInfo("AttachOnNormalMsgCt", 26289, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttachOnNormalMsgCtInt64 = new StorePropTag(26289, PropertyType.Int64, new StorePropInfo("AttachOnNormalMsgCtInt64", 26289, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttachOnAssocMsgCt = new StorePropTag(26290, PropertyType.Int32, new StorePropInfo("AttachOnAssocMsgCt", 26290, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AttachOnAssocMsgCtInt64 = new StorePropTag(26290, PropertyType.Int64, new StorePropInfo("AttachOnAssocMsgCtInt64", 26290, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NormalMessageSize = new StorePropTag(26291, PropertyType.Int64, new StorePropInfo("NormalMessageSize", 26291, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NormalMessageSize32 = new StorePropTag(26291, PropertyType.Int32, new StorePropInfo("NormalMessageSize32", 26291, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssociatedMessageSize = new StorePropTag(26292, PropertyType.Int64, new StorePropInfo("AssociatedMessageSize", 26292, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AssociatedMessageSize32 = new StorePropTag(26292, PropertyType.Int32, new StorePropInfo("AssociatedMessageSize32", 26292, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderPathName = new StorePropTag(26293, PropertyType.Unicode, new StorePropInfo("FolderPathName", 26293, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag OwnerCount = new StorePropTag(26294, PropertyType.Int32, new StorePropInfo("OwnerCount", 26294, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ContactCount = new StorePropTag(26295, PropertyType.Int32, new StorePropInfo("ContactCount", 26295, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RetentionAgeLimit = new StorePropTag(26308, PropertyType.Int32, new StorePropInfo("RetentionAgeLimit", 26308, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DisablePerUserRead = new StorePropTag(26309, PropertyType.Boolean, new StorePropInfo("DisablePerUserRead", 26309, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ServerDN = new StorePropTag(26336, PropertyType.Unicode, new StorePropInfo("ServerDN", 26336, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag BackfillRanking = new StorePropTag(26337, PropertyType.Int32, new StorePropInfo("BackfillRanking", 26337, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastTransmissionTime = new StorePropTag(26338, PropertyType.Int32, new StorePropInfo("LastTransmissionTime", 26338, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StatusSendTime = new StorePropTag(26339, PropertyType.SysTime, new StorePropInfo("StatusSendTime", 26339, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag BackfillEntryCount = new StorePropTag(26340, PropertyType.Int32, new StorePropInfo("BackfillEntryCount", 26340, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NextBroadcastTime = new StorePropTag(26341, PropertyType.SysTime, new StorePropInfo("NextBroadcastTime", 26341, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag NextBackfillTime = new StorePropTag(26342, PropertyType.SysTime, new StorePropInfo("NextBackfillTime", 26342, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastCNBroadcast = new StorePropTag(26343, PropertyType.Binary, new StorePropInfo("LastCNBroadcast", 26343, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastShortCNBroadcast = new StorePropTag(26356, PropertyType.Binary, new StorePropInfo("LastShortCNBroadcast", 26356, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AverageTransmissionTime = new StorePropTag(26363, PropertyType.SysTime, new StorePropInfo("AverageTransmissionTime", 26363, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicationStatus = new StorePropTag(26364, PropertyType.Int64, new StorePropInfo("ReplicationStatus", 26364, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastDataReceivalTime = new StorePropTag(26365, PropertyType.SysTime, new StorePropInfo("LastDataReceivalTime", 26365, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AdminDisplayName = new StorePropTag(26366, PropertyType.Unicode, new StorePropInfo("AdminDisplayName", 26366, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag URLName = new StorePropTag(26375, PropertyType.Unicode, new StorePropInfo("URLName", 26375, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LocalCommitTime = new StorePropTag(26377, PropertyType.SysTime, new StorePropInfo("LocalCommitTime", 26377, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LocalCommitTimeMax = new StorePropTag(26378, PropertyType.SysTime, new StorePropInfo("LocalCommitTimeMax", 26378, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedCountTotal = new StorePropTag(26379, PropertyType.Int32, new StorePropInfo("DeletedCountTotal", 26379, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedCountTotalInt64 = new StorePropTag(26379, PropertyType.Int64, new StorePropInfo("DeletedCountTotalInt64", 26379, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ScopeFIDs = new StorePropTag(26386, PropertyType.Binary, new StorePropInfo("ScopeFIDs", 26386, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFAdminDescription = new StorePropTag(26391, PropertyType.Unicode, new StorePropInfo("PFAdminDescription", 26391, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFProxy = new StorePropTag(26397, PropertyType.Binary, new StorePropInfo("PFProxy", 26397, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFPlatinumHomeMdb = new StorePropTag(26398, PropertyType.Boolean, new StorePropInfo("PFPlatinumHomeMdb", 26398, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFProxyRequired = new StorePropTag(26399, PropertyType.Boolean, new StorePropInfo("PFProxyRequired", 26399, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFOverHardQuotaLimit = new StorePropTag(26401, PropertyType.Int32, new StorePropInfo("PFOverHardQuotaLimit", 26401, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFMsgSizeLimit = new StorePropTag(26402, PropertyType.Int32, new StorePropInfo("PFMsgSizeLimit", 26402, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PFDisallowMdbWideExpiry = new StorePropTag(26403, PropertyType.Boolean, new StorePropInfo("PFDisallowMdbWideExpiry", 26403, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderAdminFlags = new StorePropTag(26413, PropertyType.Int32, new StorePropInfo("FolderAdminFlags", 26413, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ProvisionedFID = new StorePropTag(26415, PropertyType.Int64, new StorePropInfo("ProvisionedFID", 26415, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ELCFolderSize = new StorePropTag(26416, PropertyType.Int64, new StorePropInfo("ELCFolderSize", 26416, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ELCFolderQuota = new StorePropTag(26417, PropertyType.Int32, new StorePropInfo("ELCFolderQuota", 26417, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ELCPolicyId = new StorePropTag(26418, PropertyType.Unicode, new StorePropInfo("ELCPolicyId", 26418, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ELCPolicyComment = new StorePropTag(26419, PropertyType.Unicode, new StorePropInfo("ELCPolicyComment", 26419, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PropertyGroupMappingId = new StorePropTag(26420, PropertyType.Int32, new StorePropInfo("PropertyGroupMappingId", 26420, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag Fid = new StorePropTag(26440, PropertyType.Int64, new StorePropInfo("Fid", 26440, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FidBin = new StorePropTag(26440, PropertyType.Binary, new StorePropInfo("FidBin", 26440, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ParentFid = new StorePropTag(26441, PropertyType.Int64, new StorePropInfo("ParentFid", 26441, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ParentFidBin = new StorePropTag(26441, PropertyType.Binary, new StorePropInfo("ParentFidBin", 26441, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ArticleNumNext = new StorePropTag(26449, PropertyType.Int32, new StorePropInfo("ArticleNumNext", 26449, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ImapLastArticleId = new StorePropTag(26450, PropertyType.Int32, new StorePropInfo("ImapLastArticleId", 26450, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnExport = new StorePropTag(26457, PropertyType.Binary, new StorePropInfo("CnExport", 26457, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PclExport = new StorePropTag(26458, PropertyType.Binary, new StorePropInfo("PclExport", 26458, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnMvExport = new StorePropTag(26459, PropertyType.Binary, new StorePropInfo("CnMvExport", 26459, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidsetDeletedExport = new StorePropTag(26460, PropertyType.Binary, new StorePropInfo("MidsetDeletedExport", 26460, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ArticleNumMic = new StorePropTag(26461, PropertyType.Int32, new StorePropInfo("ArticleNumMic", 26461, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ArticleNumMost = new StorePropTag(26462, PropertyType.Int32, new StorePropInfo("ArticleNumMost", 26462, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RulesSync = new StorePropTag(26464, PropertyType.Int32, new StorePropInfo("RulesSync", 26464, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicaListR = new StorePropTag(26465, PropertyType.Binary, new StorePropInfo("ReplicaListR", 26465, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicaListRC = new StorePropTag(26466, PropertyType.Binary, new StorePropInfo("ReplicaListRC", 26466, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ReplicaListRBUG = new StorePropTag(26467, PropertyType.Binary, new StorePropInfo("ReplicaListRBUG", 26467, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RootFid = new StorePropTag(26468, PropertyType.Int64, new StorePropInfo("RootFid", 26468, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SoftDeleted = new StorePropTag(26480, PropertyType.Boolean, new StorePropInfo("SoftDeleted", 26480, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag QuotaStyle = new StorePropTag(26489, PropertyType.Int32, new StorePropInfo("QuotaStyle", 26489, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag StorageQuota = new StorePropTag(26491, PropertyType.Int32, new StorePropInfo("StorageQuota", 26491, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 5, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderPropTagArray = new StorePropTag(26494, PropertyType.Binary, new StorePropInfo("FolderPropTagArray", 26494, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MsgFolderPropTagArray = new StorePropTag(26495, PropertyType.Binary, new StorePropInfo("MsgFolderPropTagArray", 26495, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SetReceiveCount = new StorePropTag(26496, PropertyType.Int32, new StorePropInfo("SetReceiveCount", 26496, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SubmittedCount = new StorePropTag(26498, PropertyType.Int32, new StorePropInfo("SubmittedCount", 26498, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CreatorToken = new StorePropTag(26499, PropertyType.Binary, new StorePropInfo("CreatorToken", 26499, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchState = new StorePropTag(26499, PropertyType.Int32, new StorePropInfo("SearchState", 26499, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchRestriction = new StorePropTag(26500, PropertyType.Binary, new StorePropInfo("SearchRestriction", 26500, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchFIDs = new StorePropTag(26501, PropertyType.Binary, new StorePropInfo("SearchFIDs", 26501, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag RecursiveSearchFIDs = new StorePropTag(26502, PropertyType.Binary, new StorePropInfo("RecursiveSearchFIDs", 26502, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchBacklinks = new StorePropTag(26503, PropertyType.Binary, new StorePropInfo("SearchBacklinks", 26503, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CategFIDs = new StorePropTag(26506, PropertyType.Binary, new StorePropInfo("CategFIDs", 26506, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderCDN = new StorePropTag(26509, PropertyType.Binary, new StorePropInfo("FolderCDN", 26509, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidSegmentStart = new StorePropTag(26513, PropertyType.Int64, new StorePropInfo("MidSegmentStart", 26513, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidsetDeleted = new StorePropTag(26514, PropertyType.Binary, new StorePropInfo("MidsetDeleted", 26514, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidsetExpired = new StorePropTag(26515, PropertyType.Binary, new StorePropInfo("MidsetExpired", 26515, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnsetIn = new StorePropTag(26516, PropertyType.Binary, new StorePropInfo("CnsetIn", 26516, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnsetSeen = new StorePropTag(26518, PropertyType.Binary, new StorePropInfo("CnsetSeen", 26518, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidsetTombstones = new StorePropTag(26520, PropertyType.Binary, new StorePropInfo("MidsetTombstones", 26520, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag GWFolder = new StorePropTag(26522, PropertyType.Boolean, new StorePropInfo("GWFolder", 26522, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IPMFolder = new StorePropTag(26523, PropertyType.Boolean, new StorePropInfo("IPMFolder", 26523, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PublicFolderPath = new StorePropTag(26524, PropertyType.Unicode, new StorePropInfo("PublicFolderPath", 26524, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidSegmentIndex = new StorePropTag(26527, PropertyType.Int16, new StorePropInfo("MidSegmentIndex", 26527, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MidSegmentSize = new StorePropTag(26528, PropertyType.Int16, new StorePropInfo("MidSegmentSize", 26528, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnSegmentStart = new StorePropTag(26529, PropertyType.Int16, new StorePropInfo("CnSegmentStart", 26529, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnSegmentIndex = new StorePropTag(26530, PropertyType.Int16, new StorePropInfo("CnSegmentIndex", 26530, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnSegmentSize = new StorePropTag(26531, PropertyType.Int16, new StorePropInfo("CnSegmentSize", 26531, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ChangeNumber = new StorePropTag(26532, PropertyType.Int64, new StorePropInfo("ChangeNumber", 26532, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ChangeNumberBin = new StorePropTag(26532, PropertyType.Binary, new StorePropInfo("ChangeNumberBin", 26532, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag PCL = new StorePropTag(26533, PropertyType.Binary, new StorePropInfo("PCL", 26533, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnMv = new StorePropTag(26534, PropertyType.MVInt64, new StorePropInfo("CnMv", 26534, PropertyType.MVInt64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag FolderTreeRootFID = new StorePropTag(26535, PropertyType.Int64, new StorePropInfo("FolderTreeRootFID", 26535, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SourceEntryId = new StorePropTag(26536, PropertyType.Binary, new StorePropInfo("SourceEntryId", 26536, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag AnonymousRights = new StorePropTag(26564, PropertyType.Int16, new StorePropInfo("AnonymousRights", 26564, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SearchGUID = new StorePropTag(26574, PropertyType.Binary, new StorePropInfo("SearchGUID", 26574, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnsetRead = new StorePropTag(26578, PropertyType.Binary, new StorePropInfo("CnsetRead", 26578, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag CnsetSeenFAI = new StorePropTag(26586, PropertyType.Binary, new StorePropInfo("CnsetSeenFAI", 26586, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 16)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag IdSetDeleted = new StorePropTag(26597, PropertyType.Binary, new StorePropInfo("IdSetDeleted", 26597, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ModifiedCount = new StorePropTag(26614, PropertyType.Int32, new StorePropInfo("ModifiedCount", 26614, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag DeletedState = new StorePropTag(26615, PropertyType.Int32, new StorePropInfo("DeletedState", 26615, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ptagMsgHeaderTableFID = new StorePropTag(26638, PropertyType.Int64, new StorePropInfo("ptagMsgHeaderTableFID", 26638, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastUserAccessTime = new StorePropTag(26672, PropertyType.SysTime, new StorePropInfo("LastUserAccessTime", 26672, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag LastUserModificationTime = new StorePropTag(26673, PropertyType.SysTime, new StorePropInfo("LastUserModificationTime", 26673, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SyncCustomState = new StorePropTag(31746, PropertyType.Binary, new StorePropInfo("SyncCustomState", 31746, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SyncFolderChangeKey = new StorePropTag(31748, PropertyType.Binary, new StorePropInfo("SyncFolderChangeKey", 31748, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag SyncFolderLastModificationTime = new StorePropTag(31749, PropertyType.SysTime, new StorePropInfo("SyncFolderLastModificationTime", 31749, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag ptagSyncState = new StorePropTag(31754, PropertyType.Binary, new StorePropInfo("ptagSyncState", 31754, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[]
			{
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.ExtendedACLData
			};

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[]
			{
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.MappingSignature,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.StoreRecordKey,
				PropTag.Folder.StoreEntryId,
				PropTag.Folder.ObjectType,
				PropTag.Folder.EntryId,
				PropTag.Folder.EntryIdSvrEid,
				PropTag.Folder.StoreSupportMask,
				PropTag.Folder.DisplayType,
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.ExtendedACLData,
				PropTag.Folder.MidsetDeleted
			};

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[]
			{
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.MappingSignature,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.StoreRecordKey,
				PropTag.Folder.StoreEntryId,
				PropTag.Folder.ObjectType,
				PropTag.Folder.EntryId,
				PropTag.Folder.EntryIdSvrEid,
				PropTag.Folder.StoreSupportMask,
				PropTag.Folder.DisplayType,
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.ExtendedACLData,
				PropTag.Folder.MidsetDeleted
			};

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.Folder.MessageSize,
				PropTag.Folder.MessageSize32,
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.FolderInternetId,
				PropTag.Folder.CreatorSID,
				PropTag.Folder.LastModifierSid,
				PropTag.Folder.SourceFid,
				PropTag.Folder.SubfolderCount,
				PropTag.Folder.DeletedSubfolderCt,
				PropTag.Folder.ReserveRangeOfIDs,
				PropTag.Folder.Access,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.AccessLevel,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.EntryId,
				PropTag.Folder.EntryIdSvrEid,
				PropTag.Folder.Depth,
				PropTag.Folder.CreationTime,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.IPMWastebasketEntryId,
				PropTag.Folder.FolderType,
				PropTag.Folder.ContentCount,
				PropTag.Folder.ContentCountInt64,
				PropTag.Folder.UnreadCount,
				PropTag.Folder.UnreadCountInt64,
				PropTag.Folder.Subfolders,
				PropTag.Folder.ContainerHierarchy,
				PropTag.Folder.ContainerContents,
				PropTag.Folder.FolderAssociatedContents,
				PropTag.Folder.AssociatedContentCount,
				PropTag.Folder.AssociatedContentCountInt64,
				PropTag.Folder.TransportRulesSnapshot,
				PropTag.Folder.TransportRulesSnapshotId,
				PropTag.Folder.CurrentIPMWasteBasketContainerEntryId,
				PropTag.Folder.InternalAccess,
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.CorrelationId,
				PropTag.Folder.LastConflict,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.RulesData,
				PropTag.Folder.RulesTable,
				PropTag.Folder.ExtendedACLData,
				PropTag.Folder.NewAttach,
				PropTag.Folder.StartEmbed,
				PropTag.Folder.EndEmbed,
				PropTag.Folder.StartRecip,
				PropTag.Folder.EndRecip,
				PropTag.Folder.EndCcRecip,
				PropTag.Folder.EndBccRecip,
				PropTag.Folder.EndP1Recip,
				PropTag.Folder.DNPrefix,
				PropTag.Folder.StartTopFolder,
				PropTag.Folder.StartSubFolder,
				PropTag.Folder.EndFolder,
				PropTag.Folder.StartMessage,
				PropTag.Folder.EndMessage,
				PropTag.Folder.EndAttach,
				PropTag.Folder.EcWarning,
				PropTag.Folder.StartFAIMessage,
				PropTag.Folder.NewFXFolder,
				PropTag.Folder.IncrSyncChange,
				PropTag.Folder.IncrSyncDelete,
				PropTag.Folder.IncrSyncEnd,
				PropTag.Folder.IncrSyncMessage,
				PropTag.Folder.FastTransferDelProp,
				PropTag.Folder.IdsetGiven,
				PropTag.Folder.IdsetGivenInt32,
				PropTag.Folder.FastTransferErrorInfo,
				PropTag.Folder.SoftDeletes,
				PropTag.Folder.IdsetRead,
				PropTag.Folder.IdsetUnread,
				PropTag.Folder.IncrSyncRead,
				PropTag.Folder.IncrSyncStateBegin,
				PropTag.Folder.IncrSyncStateEnd,
				PropTag.Folder.IncrSyncImailStream,
				PropTag.Folder.IncrSyncImailStreamContinue,
				PropTag.Folder.IncrSyncImailStreamCancel,
				PropTag.Folder.IncrSyncImailStream2Continue,
				PropTag.Folder.IncrSyncProgressMode,
				PropTag.Folder.SyncProgressPerMsg,
				PropTag.Folder.IncrSyncMsgPartial,
				PropTag.Folder.IncrSyncGroupInfo,
				PropTag.Folder.IncrSyncGroupId,
				PropTag.Folder.IncrSyncChangePartial,
				PropTag.Folder.HierRev,
				PropTag.Folder.SourceKey,
				PropTag.Folder.ParentSourceKey,
				PropTag.Folder.ChangeKey,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.FolderChildCount,
				PropTag.Folder.FolderChildCountInt64,
				PropTag.Folder.Rights,
				PropTag.Folder.HierarchyChangeNumber,
				PropTag.Folder.HasModeratorRules,
				PropTag.Folder.ModeratorRuleCount,
				PropTag.Folder.DeletedMsgCount,
				PropTag.Folder.DeletedMsgCountInt64,
				PropTag.Folder.DeletedFolderCount,
				PropTag.Folder.DeletedAssocMsgCount,
				PropTag.Folder.DeletedAssocMsgCountInt64,
				PropTag.Folder.PromotedProperties,
				PropTag.Folder.HiddenPromotedProperties,
				PropTag.Folder.HasNamedProperties,
				PropTag.Folder.ICSChangeKey,
				PropTag.Folder.DeletedOn,
				PropTag.Folder.DeletedMessageSize,
				PropTag.Folder.DeletedMessageSize32,
				PropTag.Folder.DeletedNormalMessageSize,
				PropTag.Folder.DeletedNormalMessageSize32,
				PropTag.Folder.DeletedAssociatedMessageSize,
				PropTag.Folder.DeletedAssociatedMessageSize32,
				PropTag.Folder.FolderFlags,
				PropTag.Folder.NormalMsgWithAttachCount,
				PropTag.Folder.NormalMsgWithAttachCountInt64,
				PropTag.Folder.AssocMsgWithAttachCount,
				PropTag.Folder.AssocMsgWithAttachCountInt64,
				PropTag.Folder.RecipientOnNormalMsgCount,
				PropTag.Folder.RecipientOnNormalMsgCountInt64,
				PropTag.Folder.RecipientOnAssocMsgCount,
				PropTag.Folder.RecipientOnAssocMsgCountInt64,
				PropTag.Folder.AttachOnNormalMsgCt,
				PropTag.Folder.AttachOnNormalMsgCtInt64,
				PropTag.Folder.AttachOnAssocMsgCt,
				PropTag.Folder.AttachOnAssocMsgCtInt64,
				PropTag.Folder.NormalMessageSize,
				PropTag.Folder.NormalMessageSize32,
				PropTag.Folder.AssociatedMessageSize,
				PropTag.Folder.AssociatedMessageSize32,
				PropTag.Folder.FolderPathName,
				PropTag.Folder.LocalCommitTime,
				PropTag.Folder.LocalCommitTimeMax,
				PropTag.Folder.DeletedCountTotal,
				PropTag.Folder.DeletedCountTotalInt64,
				PropTag.Folder.ScopeFIDs,
				PropTag.Folder.PFOverHardQuotaLimit,
				PropTag.Folder.PFMsgSizeLimit,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.ELCFolderQuota,
				PropTag.Folder.ELCPolicyId,
				PropTag.Folder.ELCPolicyComment,
				PropTag.Folder.PropertyGroupMappingId,
				PropTag.Folder.Fid,
				PropTag.Folder.FidBin,
				PropTag.Folder.ParentFid,
				PropTag.Folder.ParentFidBin,
				PropTag.Folder.ArticleNumNext,
				PropTag.Folder.CnExport,
				PropTag.Folder.PclExport,
				PropTag.Folder.CnMvExport,
				PropTag.Folder.MidsetDeletedExport,
				PropTag.Folder.ArticleNumMic,
				PropTag.Folder.ArticleNumMost,
				PropTag.Folder.RulesSync,
				PropTag.Folder.ReplicaListR,
				PropTag.Folder.ReplicaListRC,
				PropTag.Folder.ReplicaListRBUG,
				PropTag.Folder.RootFid,
				PropTag.Folder.SoftDeleted,
				PropTag.Folder.QuotaStyle,
				PropTag.Folder.StorageQuota,
				PropTag.Folder.FolderPropTagArray,
				PropTag.Folder.MsgFolderPropTagArray,
				PropTag.Folder.SetReceiveCount,
				PropTag.Folder.SubmittedCount,
				PropTag.Folder.CreatorToken,
				PropTag.Folder.SearchState,
				PropTag.Folder.SearchRestriction,
				PropTag.Folder.SearchFIDs,
				PropTag.Folder.RecursiveSearchFIDs,
				PropTag.Folder.SearchBacklinks,
				PropTag.Folder.CategFIDs,
				PropTag.Folder.FolderCDN,
				PropTag.Folder.MidSegmentStart,
				PropTag.Folder.MidsetDeleted,
				PropTag.Folder.MidsetExpired,
				PropTag.Folder.CnsetIn,
				PropTag.Folder.CnsetSeen,
				PropTag.Folder.MidsetTombstones,
				PropTag.Folder.GWFolder,
				PropTag.Folder.IPMFolder,
				PropTag.Folder.PublicFolderPath,
				PropTag.Folder.MidSegmentIndex,
				PropTag.Folder.MidSegmentSize,
				PropTag.Folder.CnSegmentStart,
				PropTag.Folder.CnSegmentIndex,
				PropTag.Folder.CnSegmentSize,
				PropTag.Folder.ChangeNumber,
				PropTag.Folder.ChangeNumberBin,
				PropTag.Folder.PCL,
				PropTag.Folder.CnMv,
				PropTag.Folder.FolderTreeRootFID,
				PropTag.Folder.SourceEntryId,
				PropTag.Folder.AnonymousRights,
				PropTag.Folder.SearchGUID,
				PropTag.Folder.CnsetRead,
				PropTag.Folder.CnsetSeenFAI,
				PropTag.Folder.IdSetDeleted,
				PropTag.Folder.ModifiedCount,
				PropTag.Folder.DeletedState,
				PropTag.Folder.MailboxNum,
				PropTag.Folder.LastUserAccessTime,
				PropTag.Folder.LastUserModificationTime
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[]
			{
				PropTag.Folder.CreatorSID,
				PropTag.Folder.LastModifierSid,
				PropTag.Folder.CreationTime,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.IPMWastebasketEntryId,
				PropTag.Folder.CurrentIPMWasteBasketContainerEntryId,
				PropTag.Folder.HierRev,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.HierarchyChangeNumber,
				PropTag.Folder.LocalCommitTime,
				PropTag.Folder.LocalCommitTimeMax,
				PropTag.Folder.ArticleNumNext,
				PropTag.Folder.CnExport,
				PropTag.Folder.PclExport,
				PropTag.Folder.CnMvExport,
				PropTag.Folder.MidsetDeletedExport,
				PropTag.Folder.MidsetDeleted,
				PropTag.Folder.ChangeNumber,
				PropTag.Folder.PCL,
				PropTag.Folder.CnMv,
				PropTag.Folder.SearchGUID,
				PropTag.Folder.LastUserAccessTime,
				PropTag.Folder.LastUserModificationTime
			};

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[]
			{
				PropTag.Folder.HierRev,
				PropTag.Folder.PFOverHardQuotaLimit,
				PropTag.Folder.PFMsgSizeLimit,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.ELCFolderQuota,
				PropTag.Folder.ELCPolicyId,
				PropTag.Folder.ELCPolicyComment,
				PropTag.Folder.QuotaStyle,
				PropTag.Folder.StorageQuota
			};

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[]
			{
				PropTag.Folder.TransportRulesSnapshot,
				PropTag.Folder.TransportRulesSnapshotId
			};

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[]
			{
				PropTag.Folder.MessageSize,
				PropTag.Folder.MessageSize32,
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.FolderInternetId,
				PropTag.Folder.NTSecurityDescriptor,
				PropTag.Folder.AclTableAndSecurityDescriptor,
				PropTag.Folder.CreatorSID,
				PropTag.Folder.LastModifierSid,
				PropTag.Folder.SourceFid,
				PropTag.Folder.SubfolderCount,
				PropTag.Folder.DeletedSubfolderCt,
				PropTag.Folder.ReserveRangeOfIDs,
				PropTag.Folder.FreeBusyNTSD,
				PropTag.Folder.Access,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.AccessLevel,
				PropTag.Folder.MappingSignature,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.StoreRecordKey,
				PropTag.Folder.StoreEntryId,
				PropTag.Folder.ObjectType,
				PropTag.Folder.EntryId,
				PropTag.Folder.EntryIdSvrEid,
				PropTag.Folder.Depth,
				PropTag.Folder.CreationTime,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.StoreSupportMask,
				PropTag.Folder.IPMWastebasketEntryId,
				PropTag.Folder.FolderType,
				PropTag.Folder.ContentCount,
				PropTag.Folder.ContentCountInt64,
				PropTag.Folder.UnreadCount,
				PropTag.Folder.UnreadCountInt64,
				PropTag.Folder.Subfolders,
				PropTag.Folder.ContainerHierarchy,
				PropTag.Folder.ContainerContents,
				PropTag.Folder.FolderAssociatedContents,
				PropTag.Folder.AssociatedContentCount,
				PropTag.Folder.AssociatedContentCountInt64,
				PropTag.Folder.TransportRulesSnapshot,
				PropTag.Folder.TransportRulesSnapshotId,
				PropTag.Folder.CurrentIPMWasteBasketContainerEntryId,
				PropTag.Folder.InternalAccess,
				PropTag.Folder.DisplayType,
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.CorrelationId,
				PropTag.Folder.LastConflict,
				PropTag.Folder.NTSDModificationTime,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.RulesData,
				PropTag.Folder.RulesTable,
				PropTag.Folder.ExtendedACLData,
				PropTag.Folder.NewAttach,
				PropTag.Folder.StartEmbed,
				PropTag.Folder.EndEmbed,
				PropTag.Folder.StartRecip,
				PropTag.Folder.EndRecip,
				PropTag.Folder.EndCcRecip,
				PropTag.Folder.EndBccRecip,
				PropTag.Folder.EndP1Recip,
				PropTag.Folder.DNPrefix,
				PropTag.Folder.StartTopFolder,
				PropTag.Folder.StartSubFolder,
				PropTag.Folder.EndFolder,
				PropTag.Folder.StartMessage,
				PropTag.Folder.EndMessage,
				PropTag.Folder.EndAttach,
				PropTag.Folder.EcWarning,
				PropTag.Folder.StartFAIMessage,
				PropTag.Folder.NewFXFolder,
				PropTag.Folder.IncrSyncChange,
				PropTag.Folder.IncrSyncDelete,
				PropTag.Folder.IncrSyncEnd,
				PropTag.Folder.IncrSyncMessage,
				PropTag.Folder.FastTransferDelProp,
				PropTag.Folder.IdsetGiven,
				PropTag.Folder.IdsetGivenInt32,
				PropTag.Folder.FastTransferErrorInfo,
				PropTag.Folder.SoftDeletes,
				PropTag.Folder.IdsetRead,
				PropTag.Folder.IdsetUnread,
				PropTag.Folder.IncrSyncRead,
				PropTag.Folder.IncrSyncStateBegin,
				PropTag.Folder.IncrSyncStateEnd,
				PropTag.Folder.IncrSyncImailStream,
				PropTag.Folder.IncrSyncImailStreamContinue,
				PropTag.Folder.IncrSyncImailStreamCancel,
				PropTag.Folder.IncrSyncImailStream2Continue,
				PropTag.Folder.IncrSyncProgressMode,
				PropTag.Folder.SyncProgressPerMsg,
				PropTag.Folder.IncrSyncMsgPartial,
				PropTag.Folder.IncrSyncGroupInfo,
				PropTag.Folder.IncrSyncGroupId,
				PropTag.Folder.IncrSyncChangePartial,
				PropTag.Folder.HierRev,
				PropTag.Folder.SourceKey,
				PropTag.Folder.ParentSourceKey,
				PropTag.Folder.ChangeKey,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.FolderChildCount,
				PropTag.Folder.FolderChildCountInt64,
				PropTag.Folder.Rights,
				PropTag.Folder.HierarchyChangeNumber,
				PropTag.Folder.HasModeratorRules,
				PropTag.Folder.ModeratorRuleCount,
				PropTag.Folder.DeletedMsgCount,
				PropTag.Folder.DeletedMsgCountInt64,
				PropTag.Folder.DeletedFolderCount,
				PropTag.Folder.DeletedAssocMsgCount,
				PropTag.Folder.DeletedAssocMsgCountInt64,
				PropTag.Folder.PromotedProperties,
				PropTag.Folder.HiddenPromotedProperties,
				PropTag.Folder.HasNamedProperties,
				PropTag.Folder.ICSChangeKey,
				PropTag.Folder.DeletedOn,
				PropTag.Folder.DeletedMessageSize,
				PropTag.Folder.DeletedMessageSize32,
				PropTag.Folder.DeletedNormalMessageSize,
				PropTag.Folder.DeletedNormalMessageSize32,
				PropTag.Folder.DeletedAssociatedMessageSize,
				PropTag.Folder.DeletedAssociatedMessageSize32,
				PropTag.Folder.FolderFlags,
				PropTag.Folder.NormalMsgWithAttachCount,
				PropTag.Folder.NormalMsgWithAttachCountInt64,
				PropTag.Folder.AssocMsgWithAttachCount,
				PropTag.Folder.AssocMsgWithAttachCountInt64,
				PropTag.Folder.RecipientOnNormalMsgCount,
				PropTag.Folder.RecipientOnNormalMsgCountInt64,
				PropTag.Folder.RecipientOnAssocMsgCount,
				PropTag.Folder.RecipientOnAssocMsgCountInt64,
				PropTag.Folder.AttachOnNormalMsgCt,
				PropTag.Folder.AttachOnNormalMsgCtInt64,
				PropTag.Folder.AttachOnAssocMsgCt,
				PropTag.Folder.AttachOnAssocMsgCtInt64,
				PropTag.Folder.NormalMessageSize,
				PropTag.Folder.NormalMessageSize32,
				PropTag.Folder.AssociatedMessageSize,
				PropTag.Folder.AssociatedMessageSize32,
				PropTag.Folder.FolderPathName,
				PropTag.Folder.LocalCommitTime,
				PropTag.Folder.LocalCommitTimeMax,
				PropTag.Folder.DeletedCountTotal,
				PropTag.Folder.DeletedCountTotalInt64,
				PropTag.Folder.ScopeFIDs,
				PropTag.Folder.PFOverHardQuotaLimit,
				PropTag.Folder.PFMsgSizeLimit,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.ELCFolderQuota,
				PropTag.Folder.ELCPolicyId,
				PropTag.Folder.ELCPolicyComment,
				PropTag.Folder.Fid,
				PropTag.Folder.FidBin,
				PropTag.Folder.ParentFid,
				PropTag.Folder.ParentFidBin,
				PropTag.Folder.ArticleNumNext,
				PropTag.Folder.CnExport,
				PropTag.Folder.PclExport,
				PropTag.Folder.CnMvExport,
				PropTag.Folder.MidsetDeletedExport,
				PropTag.Folder.RulesSync,
				PropTag.Folder.RootFid,
				PropTag.Folder.QuotaStyle,
				PropTag.Folder.StorageQuota,
				PropTag.Folder.FolderPropTagArray,
				PropTag.Folder.MsgFolderPropTagArray,
				PropTag.Folder.SubmittedCount,
				PropTag.Folder.CreatorToken,
				PropTag.Folder.SearchState,
				PropTag.Folder.MidsetDeleted,
				PropTag.Folder.CnsetIn,
				PropTag.Folder.CnsetSeen,
				PropTag.Folder.IPMFolder,
				PropTag.Folder.ChangeNumber,
				PropTag.Folder.ChangeNumberBin,
				PropTag.Folder.PCL,
				PropTag.Folder.CnMv,
				PropTag.Folder.SearchGUID,
				PropTag.Folder.CnsetRead,
				PropTag.Folder.CnsetSeenFAI,
				PropTag.Folder.IdSetDeleted,
				PropTag.Folder.MailboxNum,
				PropTag.Folder.LastUserAccessTime,
				PropTag.Folder.LastUserModificationTime
			};

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[]
			{
				PropTag.Folder.MessageSize,
				PropTag.Folder.MessageSize32,
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.FolderInternetId,
				PropTag.Folder.SourceFid,
				PropTag.Folder.SubfolderCount,
				PropTag.Folder.DeletedSubfolderCt,
				PropTag.Folder.Access,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.AccessLevel,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.Depth,
				PropTag.Folder.FolderType,
				PropTag.Folder.ContentCount,
				PropTag.Folder.ContentCountInt64,
				PropTag.Folder.UnreadCount,
				PropTag.Folder.UnreadCountInt64,
				PropTag.Folder.Subfolders,
				PropTag.Folder.AssociatedContentCount,
				PropTag.Folder.AssociatedContentCountInt64,
				PropTag.Folder.CorrelationId,
				PropTag.Folder.PublishInAddressBook,
				PropTag.Folder.ResolveMethod,
				PropTag.Folder.AddressBookDisplayName,
				PropTag.Folder.SourceKey,
				PropTag.Folder.ParentSourceKey,
				PropTag.Folder.ChangeKey,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.LISSD,
				PropTag.Folder.FolderChildCount,
				PropTag.Folder.FolderChildCountInt64,
				PropTag.Folder.Rights,
				PropTag.Folder.HasModeratorRules,
				PropTag.Folder.DeletedMsgCount,
				PropTag.Folder.DeletedMsgCountInt64,
				PropTag.Folder.DeletedFolderCount,
				PropTag.Folder.DeletedAssocMsgCount,
				PropTag.Folder.DeletedAssocMsgCountInt64,
				PropTag.Folder.HasNamedProperties,
				PropTag.Folder.DeletedOn,
				PropTag.Folder.DeletedMessageSize,
				PropTag.Folder.DeletedMessageSize32,
				PropTag.Folder.DeletedNormalMessageSize,
				PropTag.Folder.DeletedNormalMessageSize32,
				PropTag.Folder.DeletedAssociatedMessageSize,
				PropTag.Folder.DeletedAssociatedMessageSize32,
				PropTag.Folder.FolderFlags,
				PropTag.Folder.NormalMsgWithAttachCount,
				PropTag.Folder.NormalMsgWithAttachCountInt64,
				PropTag.Folder.AssocMsgWithAttachCount,
				PropTag.Folder.AssocMsgWithAttachCountInt64,
				PropTag.Folder.RecipientOnNormalMsgCount,
				PropTag.Folder.RecipientOnNormalMsgCountInt64,
				PropTag.Folder.RecipientOnAssocMsgCount,
				PropTag.Folder.RecipientOnAssocMsgCountInt64,
				PropTag.Folder.AttachOnNormalMsgCt,
				PropTag.Folder.AttachOnNormalMsgCtInt64,
				PropTag.Folder.AttachOnAssocMsgCt,
				PropTag.Folder.AttachOnAssocMsgCtInt64,
				PropTag.Folder.NormalMessageSize,
				PropTag.Folder.NormalMessageSize32,
				PropTag.Folder.AssociatedMessageSize,
				PropTag.Folder.AssociatedMessageSize32,
				PropTag.Folder.FolderPathName,
				PropTag.Folder.LocalCommitTime,
				PropTag.Folder.LocalCommitTimeMax,
				PropTag.Folder.DeletedCountTotal,
				PropTag.Folder.DeletedCountTotalInt64,
				PropTag.Folder.RootFid,
				PropTag.Folder.SubmittedCount,
				PropTag.Folder.SearchState
			};

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[]
			{
				PropTag.Folder.CreatorSID,
				PropTag.Folder.CreationTime,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.HierRev
			};

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[]
			{
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.HierarchyChangeNumber,
				PropTag.Folder.CnsetIn,
				PropTag.Folder.CnsetSeen,
				PropTag.Folder.CnsetSeenFAI
			};

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[]
			{
				PropTag.Folder.AclTableAndSecurityDescriptor,
				PropTag.Folder.OofHistory,
				PropTag.Folder.ReplicaList
			};

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Folder.MessageClass,
				PropTag.Folder.MessageSize,
				PropTag.Folder.MessageSize32,
				PropTag.Folder.ParentEntryId,
				PropTag.Folder.ParentEntryIdSvrEid,
				PropTag.Folder.SentMailEntryId,
				PropTag.Folder.MessageDownloadTime,
				PropTag.Folder.FolderInternetId,
				PropTag.Folder.NTSecurityDescriptor,
				PropTag.Folder.AclTableAndSecurityDescriptor,
				PropTag.Folder.CreatorSID,
				PropTag.Folder.LastModifierSid,
				PropTag.Folder.Catalog,
				PropTag.Folder.CISearchEnabled,
				PropTag.Folder.CINotificationEnabled,
				PropTag.Folder.MaxIndices,
				PropTag.Folder.SourceFid,
				PropTag.Folder.PFContactsGuid,
				PropTag.Folder.SubfolderCount,
				PropTag.Folder.DeletedSubfolderCt,
				PropTag.Folder.MaxCachedViews,
				PropTag.Folder.NTSecurityDescriptorAsXML,
				PropTag.Folder.AdminNTSecurityDescriptorAsXML,
				PropTag.Folder.CreatorSidAsXML,
				PropTag.Folder.LastModifierSidAsXML,
				PropTag.Folder.MergeMidsetDeleted,
				PropTag.Folder.ReserveRangeOfIDs,
				PropTag.Folder.FreeBusyNTSD,
				PropTag.Folder.Access,
				PropTag.Folder.InstanceKey,
				PropTag.Folder.InstanceKeySvrEid,
				PropTag.Folder.AccessLevel,
				PropTag.Folder.MappingSignature,
				PropTag.Folder.RecordKey,
				PropTag.Folder.RecordKeySvrEid,
				PropTag.Folder.StoreRecordKey,
				PropTag.Folder.StoreEntryId,
				PropTag.Folder.ObjectType,
				PropTag.Folder.EntryId,
				PropTag.Folder.EntryIdSvrEid,
				PropTag.Folder.URLCompName,
				PropTag.Folder.AttrHidden,
				PropTag.Folder.AttrSystem,
				PropTag.Folder.AttrReadOnly,
				PropTag.Folder.DisplayName,
				PropTag.Folder.EmailAddress,
				PropTag.Folder.Comment,
				PropTag.Folder.Depth,
				PropTag.Folder.CreationTime,
				PropTag.Folder.LastModificationTime,
				PropTag.Folder.StoreSupportMask,
				PropTag.Folder.IPMWastebasketEntryId,
				PropTag.Folder.IPMCommonViewsEntryId,
				PropTag.Folder.IPMConversationsEntryId,
				PropTag.Folder.IPMAllItemsEntryId,
				PropTag.Folder.IPMSharingEntryId,
				PropTag.Folder.AdminDataEntryId,
				PropTag.Folder.FolderType,
				PropTag.Folder.ContentCount,
				PropTag.Folder.ContentCountInt64,
				PropTag.Folder.UnreadCount,
				PropTag.Folder.UnreadCountInt64,
				PropTag.Folder.Subfolders,
				PropTag.Folder.FolderStatus,
				PropTag.Folder.ContentsSortOrder,
				PropTag.Folder.ContainerHierarchy,
				PropTag.Folder.ContainerContents,
				PropTag.Folder.FolderAssociatedContents,
				PropTag.Folder.ContainerClass,
				PropTag.Folder.ContainerModifyVersion,
				PropTag.Folder.DefaultViewEntryId,
				PropTag.Folder.AssociatedContentCount,
				PropTag.Folder.AssociatedContentCountInt64,
				PropTag.Folder.PackedNamedProps,
				PropTag.Folder.AllowAgeOut,
				PropTag.Folder.SearchFolderMsgCount,
				PropTag.Folder.PartOfContentIndexing,
				PropTag.Folder.OwnerLogonUserConfigurationCache,
				PropTag.Folder.SearchFolderAgeOutTimeout,
				PropTag.Folder.SearchFolderPopulationResult,
				PropTag.Folder.SearchFolderPopulationDiagnostics,
				PropTag.Folder.ConversationTopicHashEntries,
				PropTag.Folder.ContentAggregationFlags,
				PropTag.Folder.TransportRulesSnapshot,
				PropTag.Folder.TransportRulesSnapshotId,
				PropTag.Folder.CurrentIPMWasteBasketContainerEntryId,
				PropTag.Folder.IPMAppointmentEntryId,
				PropTag.Folder.IPMContactEntryId,
				PropTag.Folder.IPMJournalEntryId,
				PropTag.Folder.IPMNoteEntryId,
				PropTag.Folder.IPMTaskEntryId,
				PropTag.Folder.REMOnlineEntryId,
				PropTag.Folder.IPMOfflineEntryId,
				PropTag.Folder.IPMDraftsEntryId,
				PropTag.Folder.AdditionalRENEntryIds,
				PropTag.Folder.AdditionalRENEntryIdsExtended,
				PropTag.Folder.AdditionalRENEntryIdsExtendedMV,
				PropTag.Folder.ExtendedFolderFlags,
				PropTag.Folder.ContainerTimestamp,
				PropTag.Folder.INetUnread,
				PropTag.Folder.NetFolderFlags,
				PropTag.Folder.FolderWebViewInfo,
				PropTag.Folder.FolderWebViewInfoExtended,
				PropTag.Folder.FolderViewFlags,
				PropTag.Folder.FreeBusyEntryIds,
				PropTag.Folder.DefaultPostMsgClass,
				PropTag.Folder.DefaultPostDisplayName,
				PropTag.Folder.FolderViewList,
				PropTag.Folder.AgingPeriod,
				PropTag.Folder.AgingGranularity,
				PropTag.Folder.DefaultFoldersLocaleId,
				PropTag.Folder.InternalAccess,
				PropTag.Folder.SyncEventSuppressGuid,
				PropTag.Folder.DisplayType,
				PropTag.Folder.TestBlobProperty,
				PropTag.Folder.AdminSecurityDescriptor,
				PropTag.Folder.Win32NTSecurityDescriptor,
				PropTag.Folder.NonWin32ACL,
				PropTag.Folder.ItemLevelACL,
				PropTag.Folder.ICSGid,
				PropTag.Folder.SystemFolderFlags,
				PropTag.Folder.MaterializedRestrictionSearchRoot,
				PropTag.Folder.MailboxPartitionNumber,
				PropTag.Folder.MailboxNumberInternal,
				PropTag.Folder.QueryCriteriaInternal,
				PropTag.Folder.LastQuotaNotificationTime,
				PropTag.Folder.PropertyPromotionInProgressHiddenItems,
				PropTag.Folder.PropertyPromotionInProgressNormalItems,
				PropTag.Folder.VirtualUnreadMessageCount,
				PropTag.Folder.InternalChangeKey,
				PropTag.Folder.InternalSourceKey,
				PropTag.Folder.CorrelationId,
				PropTag.Folder.LastConflict,
				PropTag.Folder.NTSDModificationTime,
				PropTag.Folder.ACLDataChecksum,
				PropTag.Folder.ACLData,
				PropTag.Folder.ACLTable,
				PropTag.Folder.RulesData,
				PropTag.Folder.RulesTable,
				PropTag.Folder.OofHistory,
				PropTag.Folder.DesignInProgress,
				PropTag.Folder.SecureOrigination,
				PropTag.Folder.PublishInAddressBook,
				PropTag.Folder.ResolveMethod,
				PropTag.Folder.AddressBookDisplayName,
				PropTag.Folder.EFormsLocaleId,
				PropTag.Folder.ExtendedACLData,
				PropTag.Folder.RulesSize,
				PropTag.Folder.NewAttach,
				PropTag.Folder.StartEmbed,
				PropTag.Folder.EndEmbed,
				PropTag.Folder.StartRecip,
				PropTag.Folder.EndRecip,
				PropTag.Folder.EndCcRecip,
				PropTag.Folder.EndBccRecip,
				PropTag.Folder.EndP1Recip,
				PropTag.Folder.DNPrefix,
				PropTag.Folder.StartTopFolder,
				PropTag.Folder.StartSubFolder,
				PropTag.Folder.EndFolder,
				PropTag.Folder.StartMessage,
				PropTag.Folder.EndMessage,
				PropTag.Folder.EndAttach,
				PropTag.Folder.EcWarning,
				PropTag.Folder.StartFAIMessage,
				PropTag.Folder.NewFXFolder,
				PropTag.Folder.IncrSyncChange,
				PropTag.Folder.IncrSyncDelete,
				PropTag.Folder.IncrSyncEnd,
				PropTag.Folder.IncrSyncMessage,
				PropTag.Folder.FastTransferDelProp,
				PropTag.Folder.IdsetGiven,
				PropTag.Folder.IdsetGivenInt32,
				PropTag.Folder.FastTransferErrorInfo,
				PropTag.Folder.SoftDeletes,
				PropTag.Folder.IdsetRead,
				PropTag.Folder.IdsetUnread,
				PropTag.Folder.IncrSyncRead,
				PropTag.Folder.IncrSyncStateBegin,
				PropTag.Folder.IncrSyncStateEnd,
				PropTag.Folder.IncrSyncImailStream,
				PropTag.Folder.IncrSyncImailStreamContinue,
				PropTag.Folder.IncrSyncImailStreamCancel,
				PropTag.Folder.IncrSyncImailStream2Continue,
				PropTag.Folder.IncrSyncProgressMode,
				PropTag.Folder.SyncProgressPerMsg,
				PropTag.Folder.IncrSyncMsgPartial,
				PropTag.Folder.IncrSyncGroupInfo,
				PropTag.Folder.IncrSyncGroupId,
				PropTag.Folder.IncrSyncChangePartial,
				PropTag.Folder.HierRev,
				PropTag.Folder.SourceKey,
				PropTag.Folder.ParentSourceKey,
				PropTag.Folder.ChangeKey,
				PropTag.Folder.PredecessorChangeList,
				PropTag.Folder.PreventMsgCreate,
				PropTag.Folder.LISSD,
				PropTag.Folder.FavoritesDefaultName,
				PropTag.Folder.FolderChildCount,
				PropTag.Folder.FolderChildCountInt64,
				PropTag.Folder.Rights,
				PropTag.Folder.HasRules,
				PropTag.Folder.AddressBookEntryId,
				PropTag.Folder.HierarchyChangeNumber,
				PropTag.Folder.HasModeratorRules,
				PropTag.Folder.ModeratorRuleCount,
				PropTag.Folder.DeletedMsgCount,
				PropTag.Folder.DeletedMsgCountInt64,
				PropTag.Folder.DeletedFolderCount,
				PropTag.Folder.DeletedAssocMsgCount,
				PropTag.Folder.DeletedAssocMsgCountInt64,
				PropTag.Folder.PromotedProperties,
				PropTag.Folder.HiddenPromotedProperties,
				PropTag.Folder.LinkedSiteAuthorityUrl,
				PropTag.Folder.HasNamedProperties,
				PropTag.Folder.FidMid,
				PropTag.Folder.ICSChangeKey,
				PropTag.Folder.SetPropsCondition,
				PropTag.Folder.DeletedOn,
				PropTag.Folder.ReplicationStyle,
				PropTag.Folder.ReplicationTIB,
				PropTag.Folder.ReplicationMsgPriority,
				PropTag.Folder.ReplicaList,
				PropTag.Folder.OverallAgeLimit,
				PropTag.Folder.DeletedMessageSize,
				PropTag.Folder.DeletedMessageSize32,
				PropTag.Folder.DeletedNormalMessageSize,
				PropTag.Folder.DeletedNormalMessageSize32,
				PropTag.Folder.DeletedAssociatedMessageSize,
				PropTag.Folder.DeletedAssociatedMessageSize32,
				PropTag.Folder.SecureInSite,
				PropTag.Folder.FolderFlags,
				PropTag.Folder.LastAccessTime,
				PropTag.Folder.NormalMsgWithAttachCount,
				PropTag.Folder.NormalMsgWithAttachCountInt64,
				PropTag.Folder.AssocMsgWithAttachCount,
				PropTag.Folder.AssocMsgWithAttachCountInt64,
				PropTag.Folder.RecipientOnNormalMsgCount,
				PropTag.Folder.RecipientOnNormalMsgCountInt64,
				PropTag.Folder.RecipientOnAssocMsgCount,
				PropTag.Folder.RecipientOnAssocMsgCountInt64,
				PropTag.Folder.AttachOnNormalMsgCt,
				PropTag.Folder.AttachOnNormalMsgCtInt64,
				PropTag.Folder.AttachOnAssocMsgCt,
				PropTag.Folder.AttachOnAssocMsgCtInt64,
				PropTag.Folder.NormalMessageSize,
				PropTag.Folder.NormalMessageSize32,
				PropTag.Folder.AssociatedMessageSize,
				PropTag.Folder.AssociatedMessageSize32,
				PropTag.Folder.FolderPathName,
				PropTag.Folder.OwnerCount,
				PropTag.Folder.ContactCount,
				PropTag.Folder.RetentionAgeLimit,
				PropTag.Folder.DisablePerUserRead,
				PropTag.Folder.ServerDN,
				PropTag.Folder.BackfillRanking,
				PropTag.Folder.LastTransmissionTime,
				PropTag.Folder.StatusSendTime,
				PropTag.Folder.BackfillEntryCount,
				PropTag.Folder.NextBroadcastTime,
				PropTag.Folder.NextBackfillTime,
				PropTag.Folder.LastCNBroadcast,
				PropTag.Folder.LastShortCNBroadcast,
				PropTag.Folder.AverageTransmissionTime,
				PropTag.Folder.ReplicationStatus,
				PropTag.Folder.LastDataReceivalTime,
				PropTag.Folder.AdminDisplayName,
				PropTag.Folder.URLName,
				PropTag.Folder.LocalCommitTime,
				PropTag.Folder.LocalCommitTimeMax,
				PropTag.Folder.DeletedCountTotal,
				PropTag.Folder.DeletedCountTotalInt64,
				PropTag.Folder.ScopeFIDs,
				PropTag.Folder.PFAdminDescription,
				PropTag.Folder.PFProxy,
				PropTag.Folder.PFPlatinumHomeMdb,
				PropTag.Folder.PFProxyRequired,
				PropTag.Folder.PFOverHardQuotaLimit,
				PropTag.Folder.PFMsgSizeLimit,
				PropTag.Folder.PFDisallowMdbWideExpiry,
				PropTag.Folder.FolderAdminFlags,
				PropTag.Folder.ProvisionedFID,
				PropTag.Folder.ELCFolderSize,
				PropTag.Folder.ELCFolderQuota,
				PropTag.Folder.ELCPolicyId,
				PropTag.Folder.ELCPolicyComment,
				PropTag.Folder.PropertyGroupMappingId,
				PropTag.Folder.Fid,
				PropTag.Folder.FidBin,
				PropTag.Folder.ParentFid,
				PropTag.Folder.ParentFidBin,
				PropTag.Folder.ArticleNumNext,
				PropTag.Folder.ImapLastArticleId,
				PropTag.Folder.CnExport,
				PropTag.Folder.PclExport,
				PropTag.Folder.CnMvExport,
				PropTag.Folder.MidsetDeletedExport,
				PropTag.Folder.ArticleNumMic,
				PropTag.Folder.ArticleNumMost,
				PropTag.Folder.RulesSync,
				PropTag.Folder.ReplicaListR,
				PropTag.Folder.ReplicaListRC,
				PropTag.Folder.ReplicaListRBUG,
				PropTag.Folder.RootFid,
				PropTag.Folder.SoftDeleted,
				PropTag.Folder.QuotaStyle,
				PropTag.Folder.StorageQuota,
				PropTag.Folder.FolderPropTagArray,
				PropTag.Folder.MsgFolderPropTagArray,
				PropTag.Folder.SetReceiveCount,
				PropTag.Folder.SubmittedCount,
				PropTag.Folder.CreatorToken,
				PropTag.Folder.SearchState,
				PropTag.Folder.SearchRestriction,
				PropTag.Folder.SearchFIDs,
				PropTag.Folder.RecursiveSearchFIDs,
				PropTag.Folder.SearchBacklinks,
				PropTag.Folder.CategFIDs,
				PropTag.Folder.FolderCDN,
				PropTag.Folder.MidSegmentStart,
				PropTag.Folder.MidsetDeleted,
				PropTag.Folder.MidsetExpired,
				PropTag.Folder.CnsetIn,
				PropTag.Folder.CnsetSeen,
				PropTag.Folder.MidsetTombstones,
				PropTag.Folder.GWFolder,
				PropTag.Folder.IPMFolder,
				PropTag.Folder.PublicFolderPath,
				PropTag.Folder.MidSegmentIndex,
				PropTag.Folder.MidSegmentSize,
				PropTag.Folder.CnSegmentStart,
				PropTag.Folder.CnSegmentIndex,
				PropTag.Folder.CnSegmentSize,
				PropTag.Folder.ChangeNumber,
				PropTag.Folder.ChangeNumberBin,
				PropTag.Folder.PCL,
				PropTag.Folder.CnMv,
				PropTag.Folder.FolderTreeRootFID,
				PropTag.Folder.SourceEntryId,
				PropTag.Folder.AnonymousRights,
				PropTag.Folder.SearchGUID,
				PropTag.Folder.CnsetRead,
				PropTag.Folder.CnsetSeenFAI,
				PropTag.Folder.IdSetDeleted,
				PropTag.Folder.ModifiedCount,
				PropTag.Folder.DeletedState,
				PropTag.Folder.ptagMsgHeaderTableFID,
				PropTag.Folder.MailboxNum,
				PropTag.Folder.LastUserAccessTime,
				PropTag.Folder.LastUserModificationTime,
				PropTag.Folder.SyncCustomState,
				PropTag.Folder.SyncFolderChangeKey,
				PropTag.Folder.SyncFolderLastModificationTime,
				PropTag.Folder.ptagSyncState
			};
		}

		public static class Message
		{
			public static readonly StorePropTag AcknowledgementMode = new StorePropTag(1, PropertyType.Int32, new StorePropInfo("AcknowledgementMode", 1, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TestTest = new StorePropTag(1, PropertyType.Binary, new StorePropInfo("TestTest", 1, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AlternateRecipientAllowed = new StorePropTag(2, PropertyType.Boolean, new StorePropInfo("AlternateRecipientAllowed", 2, PropertyType.Boolean, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AuthorizingUsers = new StorePropTag(3, PropertyType.Binary, new StorePropInfo("AuthorizingUsers", 3, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AutoForwardComment = new StorePropTag(4, PropertyType.Unicode, new StorePropInfo("AutoForwardComment", 4, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AutoForwarded = new StorePropTag(5, PropertyType.Boolean, new StorePropInfo("AutoForwarded", 5, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentConfidentialityAlgorithmId = new StorePropTag(6, PropertyType.Binary, new StorePropInfo("ContentConfidentialityAlgorithmId", 6, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentCorrelator = new StorePropTag(7, PropertyType.Binary, new StorePropInfo("ContentCorrelator", 7, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentIdentifier = new StorePropTag(8, PropertyType.Unicode, new StorePropInfo("ContentIdentifier", 8, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentLength = new StorePropTag(9, PropertyType.Int32, new StorePropInfo("ContentLength", 9, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentReturnRequested = new StorePropTag(10, PropertyType.Boolean, new StorePropInfo("ContentReturnRequested", 10, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationKey = new StorePropTag(11, PropertyType.Binary, new StorePropInfo("ConversationKey", 11, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversionEits = new StorePropTag(12, PropertyType.Binary, new StorePropInfo("ConversionEits", 12, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversionWithLossProhibited = new StorePropTag(13, PropertyType.Boolean, new StorePropInfo("ConversionWithLossProhibited", 13, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConvertedEits = new StorePropTag(14, PropertyType.Binary, new StorePropInfo("ConvertedEits", 14, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeferredDeliveryTime = new StorePropTag(15, PropertyType.SysTime, new StorePropInfo("DeferredDeliveryTime", 15, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeliverTime = new StorePropTag(16, PropertyType.SysTime, new StorePropInfo("DeliverTime", 16, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DiscardReason = new StorePropTag(17, PropertyType.Int32, new StorePropInfo("DiscardReason", 17, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisclosureOfRecipients = new StorePropTag(18, PropertyType.Boolean, new StorePropInfo("DisclosureOfRecipients", 18, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DLExpansionHistory = new StorePropTag(19, PropertyType.Binary, new StorePropInfo("DLExpansionHistory", 19, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DLExpansionProhibited = new StorePropTag(20, PropertyType.Boolean, new StorePropInfo("DLExpansionProhibited", 20, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExpiryTime = new StorePropTag(21, PropertyType.SysTime, new StorePropInfo("ExpiryTime", 21, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ImplicitConversionProhibited = new StorePropTag(22, PropertyType.Boolean, new StorePropInfo("ImplicitConversionProhibited", 22, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Importance = new StorePropTag(23, PropertyType.Int32, new StorePropInfo("Importance", 23, PropertyType.Int32, StorePropInfo.Flags.None, 2341871806232657924UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IPMID = new StorePropTag(24, PropertyType.Binary, new StorePropInfo("IPMID", 24, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LatestDeliveryTime = new StorePropTag(25, PropertyType.SysTime, new StorePropInfo("LatestDeliveryTime", 25, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageClass = new StorePropTag(26, PropertyType.Unicode, new StorePropInfo("MessageClass", 26, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageDeliveryId = new StorePropTag(27, PropertyType.Binary, new StorePropInfo("MessageDeliveryId", 27, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageSecurityLabel = new StorePropTag(30, PropertyType.Binary, new StorePropInfo("MessageSecurityLabel", 30, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ObsoletedIPMS = new StorePropTag(31, PropertyType.Binary, new StorePropInfo("ObsoletedIPMS", 31, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginallyIntendedRecipientName = new StorePropTag(32, PropertyType.Binary, new StorePropInfo("OriginallyIntendedRecipientName", 32, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalEITS = new StorePropTag(33, PropertyType.Binary, new StorePropInfo("OriginalEITS", 33, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorCertificate = new StorePropTag(34, PropertyType.Binary, new StorePropInfo("OriginatorCertificate", 34, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeliveryReportRequested = new StorePropTag(35, PropertyType.Boolean, new StorePropInfo("DeliveryReportRequested", 35, PropertyType.Boolean, StorePropInfo.Flags.None, 4UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorReturnAddress = new StorePropTag(36, PropertyType.Binary, new StorePropInfo("OriginatorReturnAddress", 36, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(18)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentKey = new StorePropTag(37, PropertyType.Binary, new StorePropInfo("ParentKey", 37, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Priority = new StorePropTag(38, PropertyType.Int32, new StorePropInfo("Priority", 38, PropertyType.Int32, StorePropInfo.Flags.None, 4UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginCheck = new StorePropTag(39, PropertyType.Binary, new StorePropInfo("OriginCheck", 39, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProofOfSubmissionRequested = new StorePropTag(40, PropertyType.Boolean, new StorePropInfo("ProofOfSubmissionRequested", 40, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptRequested = new StorePropTag(41, PropertyType.Boolean, new StorePropInfo("ReadReceiptRequested", 41, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceiptTime = new StorePropTag(42, PropertyType.SysTime, new StorePropInfo("ReceiptTime", 42, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientReassignmentProhibited = new StorePropTag(43, PropertyType.Boolean, new StorePropInfo("RecipientReassignmentProhibited", 43, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RedirectionHistory = new StorePropTag(44, PropertyType.Binary, new StorePropInfo("RedirectionHistory", 44, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RelatedIPMS = new StorePropTag(45, PropertyType.Binary, new StorePropInfo("RelatedIPMS", 45, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSensitivity = new StorePropTag(46, PropertyType.Int32, new StorePropInfo("OriginalSensitivity", 46, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Languages = new StorePropTag(47, PropertyType.Unicode, new StorePropInfo("Languages", 47, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyTime = new StorePropTag(48, PropertyType.SysTime, new StorePropInfo("ReplyTime", 48, PropertyType.SysTime, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportTag = new StorePropTag(49, PropertyType.Binary, new StorePropInfo("ReportTag", 49, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportTime = new StorePropTag(50, PropertyType.SysTime, new StorePropInfo("ReportTime", 50, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReturnedIPM = new StorePropTag(51, PropertyType.Boolean, new StorePropInfo("ReturnedIPM", 51, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Security = new StorePropTag(52, PropertyType.Int32, new StorePropInfo("Security", 52, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncompleteCopy = new StorePropTag(53, PropertyType.Boolean, new StorePropInfo("IncompleteCopy", 53, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Sensitivity = new StorePropTag(54, PropertyType.Int32, new StorePropInfo("Sensitivity", 54, PropertyType.Int32, StorePropInfo.Flags.None, 4UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Subject = new StorePropTag(55, PropertyType.Unicode, new StorePropInfo("Subject", 55, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(1, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SubjectIPM = new StorePropTag(56, PropertyType.Binary, new StorePropInfo("SubjectIPM", 56, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ClientSubmitTime = new StorePropTag(57, PropertyType.SysTime, new StorePropInfo("ClientSubmitTime", 57, PropertyType.SysTime, StorePropInfo.Flags.None, 2305843009213693953UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportName = new StorePropTag(58, PropertyType.Unicode, new StorePropInfo("ReportName", 58, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSearchKey = new StorePropTag(59, PropertyType.Binary, new StorePropInfo("SentRepresentingSearchKey", 59, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag X400ContentType = new StorePropTag(60, PropertyType.Binary, new StorePropInfo("X400ContentType", 60, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SubjectPrefix = new StorePropTag(61, PropertyType.Unicode, new StorePropInfo("SubjectPrefix", 61, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(1)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NonReceiptReason = new StorePropTag(62, PropertyType.Int32, new StorePropInfo("NonReceiptReason", 62, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByEntryId = new StorePropTag(63, PropertyType.Binary, new StorePropInfo("ReceivedByEntryId", 63, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByName = new StorePropTag(64, PropertyType.Unicode, new StorePropInfo("ReceivedByName", 64, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingEntryId = new StorePropTag(65, PropertyType.Binary, new StorePropInfo("SentRepresentingEntryId", 65, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingName = new StorePropTag(66, PropertyType.Unicode, new StorePropInfo("SentRepresentingName", 66, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingEntryId = new StorePropTag(67, PropertyType.Binary, new StorePropInfo("ReceivedRepresentingEntryId", 67, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingName = new StorePropTag(68, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingName", 68, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportEntryId = new StorePropTag(69, PropertyType.Binary, new StorePropInfo("ReportEntryId", 69, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptEntryId = new StorePropTag(70, PropertyType.Binary, new StorePropInfo("ReadReceiptEntryId", 70, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageSubmissionId = new StorePropTag(71, PropertyType.Binary, new StorePropInfo("MessageSubmissionId", 71, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProviderSubmitTime = new StorePropTag(72, PropertyType.SysTime, new StorePropInfo("ProviderSubmitTime", 72, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSubject = new StorePropTag(73, PropertyType.Unicode, new StorePropInfo("OriginalSubject", 73, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DiscVal = new StorePropTag(74, PropertyType.Boolean, new StorePropInfo("DiscVal", 74, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalMessageClass = new StorePropTag(75, PropertyType.Unicode, new StorePropInfo("OriginalMessageClass", 75, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorEntryId = new StorePropTag(76, PropertyType.Binary, new StorePropInfo("OriginalAuthorEntryId", 76, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorName = new StorePropTag(77, PropertyType.Unicode, new StorePropInfo("OriginalAuthorName", 77, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSubmitTime = new StorePropTag(78, PropertyType.SysTime, new StorePropInfo("OriginalSubmitTime", 78, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyRecipientEntries = new StorePropTag(79, PropertyType.Binary, new StorePropInfo("ReplyRecipientEntries", 79, PropertyType.Binary, StorePropInfo.Flags.None, 16UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyRecipientNames = new StorePropTag(80, PropertyType.Unicode, new StorePropInfo("ReplyRecipientNames", 80, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedBySearchKey = new StorePropTag(81, PropertyType.Binary, new StorePropInfo("ReceivedBySearchKey", 81, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingSearchKey = new StorePropTag(82, PropertyType.Binary, new StorePropInfo("ReceivedRepresentingSearchKey", 82, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSearchKey = new StorePropTag(83, PropertyType.Binary, new StorePropInfo("ReadReceiptSearchKey", 83, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportSearchKey = new StorePropTag(84, PropertyType.Binary, new StorePropInfo("ReportSearchKey", 84, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDeliveryTime = new StorePropTag(85, PropertyType.SysTime, new StorePropInfo("OriginalDeliveryTime", 85, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSearchKey = new StorePropTag(86, PropertyType.Binary, new StorePropInfo("OriginalAuthorSearchKey", 86, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageToMe = new StorePropTag(87, PropertyType.Boolean, new StorePropInfo("MessageToMe", 87, PropertyType.Boolean, StorePropInfo.Flags.None, 1UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageCCMe = new StorePropTag(88, PropertyType.Boolean, new StorePropInfo("MessageCCMe", 88, PropertyType.Boolean, StorePropInfo.Flags.None, 1UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageRecipMe = new StorePropTag(89, PropertyType.Boolean, new StorePropInfo("MessageRecipMe", 89, PropertyType.Boolean, StorePropInfo.Flags.None, 1UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderName = new StorePropTag(90, PropertyType.Unicode, new StorePropInfo("OriginalSenderName", 90, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderEntryId = new StorePropTag(91, PropertyType.Binary, new StorePropInfo("OriginalSenderEntryId", 91, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSearchKey = new StorePropTag(92, PropertyType.Binary, new StorePropInfo("OriginalSenderSearchKey", 92, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingName = new StorePropTag(93, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingName", 93, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingEntryId = new StorePropTag(94, PropertyType.Binary, new StorePropInfo("OriginalSentRepresentingEntryId", 94, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSearchKey = new StorePropTag(95, PropertyType.Binary, new StorePropInfo("OriginalSentRepresentingSearchKey", 95, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartDate = new StorePropTag(96, PropertyType.SysTime, new StorePropInfo("StartDate", 96, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndDate = new StorePropTag(97, PropertyType.SysTime, new StorePropInfo("EndDate", 97, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OwnerApptId = new StorePropTag(98, PropertyType.Int32, new StorePropInfo("OwnerApptId", 98, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ResponseRequested = new StorePropTag(99, PropertyType.Boolean, new StorePropInfo("ResponseRequested", 99, PropertyType.Boolean, StorePropInfo.Flags.None, 4096UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingAddressType = new StorePropTag(100, PropertyType.Unicode, new StorePropInfo("SentRepresentingAddressType", 100, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingEmailAddress = new StorePropTag(101, PropertyType.Unicode, new StorePropInfo("SentRepresentingEmailAddress", 101, PropertyType.Unicode, StorePropInfo.Flags.None, 128UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderAddressType = new StorePropTag(102, PropertyType.Unicode, new StorePropInfo("OriginalSenderAddressType", 102, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderEmailAddress = new StorePropTag(103, PropertyType.Unicode, new StorePropInfo("OriginalSenderEmailAddress", 103, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingAddressType = new StorePropTag(104, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingAddressType", 104, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingEmailAddress = new StorePropTag(105, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingEmailAddress", 105, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationTopic = new StorePropTag(112, PropertyType.Unicode, new StorePropInfo("ConversationTopic", 112, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationIndex = new StorePropTag(113, PropertyType.Binary, new StorePropInfo("ConversationIndex", 113, PropertyType.Binary, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDisplayBcc = new StorePropTag(114, PropertyType.Unicode, new StorePropInfo("OriginalDisplayBcc", 114, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDisplayCc = new StorePropTag(115, PropertyType.Unicode, new StorePropInfo("OriginalDisplayCc", 115, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDisplayTo = new StorePropTag(116, PropertyType.Unicode, new StorePropInfo("OriginalDisplayTo", 116, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByAddressType = new StorePropTag(117, PropertyType.Unicode, new StorePropInfo("ReceivedByAddressType", 117, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByEmailAddress = new StorePropTag(118, PropertyType.Unicode, new StorePropInfo("ReceivedByEmailAddress", 118, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingAddressType = new StorePropTag(119, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingAddressType", 119, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingEmailAddress = new StorePropTag(120, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingEmailAddress", 120, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorAddressType = new StorePropTag(121, PropertyType.Unicode, new StorePropInfo("OriginalAuthorAddressType", 121, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorEmailAddress = new StorePropTag(122, PropertyType.Unicode, new StorePropInfo("OriginalAuthorEmailAddress", 122, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginallyIntendedRecipientAddressType = new StorePropTag(124, PropertyType.Unicode, new StorePropInfo("OriginallyIntendedRecipientAddressType", 124, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TransportMessageHeaders = new StorePropTag(125, PropertyType.Unicode, new StorePropInfo("TransportMessageHeaders", 125, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Delegation = new StorePropTag(126, PropertyType.Binary, new StorePropInfo("Delegation", 126, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDisposition = new StorePropTag(128, PropertyType.Unicode, new StorePropInfo("ReportDisposition", 128, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDispositionMode = new StorePropTag(129, PropertyType.Unicode, new StorePropInfo("ReportDispositionMode", 129, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportOriginalSender = new StorePropTag(130, PropertyType.Unicode, new StorePropInfo("ReportOriginalSender", 130, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDispositionToNames = new StorePropTag(131, PropertyType.Unicode, new StorePropInfo("ReportDispositionToNames", 131, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDispositionToEmailAddress = new StorePropTag(132, PropertyType.Unicode, new StorePropInfo("ReportDispositionToEmailAddress", 132, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDispositionOptions = new StorePropTag(133, PropertyType.Unicode, new StorePropInfo("ReportDispositionOptions", 133, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RichContent = new StorePropTag(134, PropertyType.Int16, new StorePropInfo("RichContent", 134, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AdministratorEMail = new StorePropTag(256, PropertyType.MVUnicode, new StorePropInfo("AdministratorEMail", 256, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentIntegrityCheck = new StorePropTag(3072, PropertyType.Binary, new StorePropInfo("ContentIntegrityCheck", 3072, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExplicitConversion = new StorePropTag(3073, PropertyType.Int32, new StorePropInfo("ExplicitConversion", 3073, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReturnRequested = new StorePropTag(3074, PropertyType.Boolean, new StorePropInfo("ReturnRequested", 3074, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageToken = new StorePropTag(3075, PropertyType.Binary, new StorePropInfo("MessageToken", 3075, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NDRReasonCode = new StorePropTag(3076, PropertyType.Int32, new StorePropInfo("NDRReasonCode", 3076, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NDRDiagCode = new StorePropTag(3077, PropertyType.Int32, new StorePropInfo("NDRDiagCode", 3077, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NonReceiptNotificationRequested = new StorePropTag(3078, PropertyType.Boolean, new StorePropInfo("NonReceiptNotificationRequested", 3078, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeliveryPoint = new StorePropTag(3079, PropertyType.Int32, new StorePropInfo("DeliveryPoint", 3079, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NonDeliveryReportRequested = new StorePropTag(3080, PropertyType.Boolean, new StorePropInfo("NonDeliveryReportRequested", 3080, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorRequestedAlterateRecipient = new StorePropTag(3081, PropertyType.Binary, new StorePropInfo("OriginatorRequestedAlterateRecipient", 3081, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalDeliveryBureauFaxDelivery = new StorePropTag(3082, PropertyType.Boolean, new StorePropInfo("PhysicalDeliveryBureauFaxDelivery", 3082, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalDeliveryMode = new StorePropTag(3083, PropertyType.Int32, new StorePropInfo("PhysicalDeliveryMode", 3083, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalDeliveryReportRequest = new StorePropTag(3084, PropertyType.Int32, new StorePropInfo("PhysicalDeliveryReportRequest", 3084, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalForwardingAddress = new StorePropTag(3085, PropertyType.Binary, new StorePropInfo("PhysicalForwardingAddress", 3085, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalForwardingAddressRequested = new StorePropTag(3086, PropertyType.Boolean, new StorePropInfo("PhysicalForwardingAddressRequested", 3086, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PhysicalForwardingProhibited = new StorePropTag(3087, PropertyType.Boolean, new StorePropInfo("PhysicalForwardingProhibited", 3087, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProofOfDelivery = new StorePropTag(3089, PropertyType.Binary, new StorePropInfo("ProofOfDelivery", 3089, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProofOfDeliveryRequested = new StorePropTag(3090, PropertyType.Boolean, new StorePropInfo("ProofOfDeliveryRequested", 3090, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientCertificate = new StorePropTag(3091, PropertyType.Binary, new StorePropInfo("RecipientCertificate", 3091, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientNumberForAdvice = new StorePropTag(3092, PropertyType.Unicode, new StorePropInfo("RecipientNumberForAdvice", 3092, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientType = new StorePropTag(3093, PropertyType.Int32, new StorePropInfo("RecipientType", 3093, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RegisteredMailType = new StorePropTag(3094, PropertyType.Int32, new StorePropInfo("RegisteredMailType", 3094, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyRequested = new StorePropTag(3095, PropertyType.Boolean, new StorePropInfo("ReplyRequested", 3095, PropertyType.Boolean, StorePropInfo.Flags.None, 4096UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RequestedDeliveryMethod = new StorePropTag(3096, PropertyType.Int32, new StorePropInfo("RequestedDeliveryMethod", 3096, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderEntryId = new StorePropTag(3097, PropertyType.Binary, new StorePropInfo("SenderEntryId", 3097, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderName = new StorePropTag(3098, PropertyType.Unicode, new StorePropInfo("SenderName", 3098, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694080UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SupplementaryInfo = new StorePropTag(3099, PropertyType.Unicode, new StorePropInfo("SupplementaryInfo", 3099, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TypeOfMTSUser = new StorePropTag(3100, PropertyType.Int32, new StorePropInfo("TypeOfMTSUser", 3100, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSearchKey = new StorePropTag(3101, PropertyType.Binary, new StorePropInfo("SenderSearchKey", 3101, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderAddressType = new StorePropTag(3102, PropertyType.Unicode, new StorePropInfo("SenderAddressType", 3102, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderEmailAddress = new StorePropTag(3103, PropertyType.Unicode, new StorePropInfo("SenderEmailAddress", 3103, PropertyType.Unicode, StorePropInfo.Flags.None, 128UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParticipantSID = new StorePropTag(3108, PropertyType.Binary, new StorePropInfo("ParticipantSID", 3108, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParticipantGuid = new StorePropTag(3109, PropertyType.Binary, new StorePropInfo("ParticipantGuid", 3109, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ToGroupExpansionRecipients = new StorePropTag(3110, PropertyType.Unicode, new StorePropInfo("ToGroupExpansionRecipients", 3110, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CcGroupExpansionRecipients = new StorePropTag(3111, PropertyType.Unicode, new StorePropInfo("CcGroupExpansionRecipients", 3111, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BccGroupExpansionRecipients = new StorePropTag(3112, PropertyType.Unicode, new StorePropInfo("BccGroupExpansionRecipients", 3112, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CurrentVersion = new StorePropTag(3584, PropertyType.Int64, new StorePropInfo("CurrentVersion", 3584, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeleteAfterSubmit = new StorePropTag(3585, PropertyType.Boolean, new StorePropInfo("DeleteAfterSubmit", 3585, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayBcc = new StorePropTag(3586, PropertyType.Unicode, new StorePropInfo("DisplayBcc", 3586, PropertyType.Unicode, StorePropInfo.Flags.None, 16UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayCc = new StorePropTag(3587, PropertyType.Unicode, new StorePropInfo("DisplayCc", 3587, PropertyType.Unicode, StorePropInfo.Flags.None, 16UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayTo = new StorePropTag(3588, PropertyType.Unicode, new StorePropInfo("DisplayTo", 3588, PropertyType.Unicode, StorePropInfo.Flags.None, 36028797018963984UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentDisplay = new StorePropTag(3589, PropertyType.Unicode, new StorePropInfo("ParentDisplay", 3589, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageDeliveryTime = new StorePropTag(3590, PropertyType.SysTime, new StorePropInfo("MessageDeliveryTime", 3590, PropertyType.SysTime, StorePropInfo.Flags.None, 2341871806232657921UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageFlags = new StorePropTag(3591, PropertyType.Int32, new StorePropInfo("MessageFlags", 3591, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageSize = new StorePropTag(3592, PropertyType.Int64, new StorePropInfo("MessageSize", 3592, PropertyType.Int64, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageSize32 = new StorePropTag(3592, PropertyType.Int32, new StorePropInfo("MessageSize32", 3592, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentEntryId = new StorePropTag(3593, PropertyType.Binary, new StorePropInfo("ParentEntryId", 3593, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14,
				18
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentEntryIdSvrEid = new StorePropTag(3593, PropertyType.SvrEid, new StorePropInfo("ParentEntryIdSvrEid", 3593, PropertyType.SvrEid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentMailEntryId = new StorePropTag(3594, PropertyType.Binary, new StorePropInfo("SentMailEntryId", 3594, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Correlate = new StorePropTag(3596, PropertyType.Boolean, new StorePropInfo("Correlate", 3596, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CorrelateMTSID = new StorePropTag(3597, PropertyType.Binary, new StorePropInfo("CorrelateMTSID", 3597, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DiscreteValues = new StorePropTag(3598, PropertyType.Boolean, new StorePropInfo("DiscreteValues", 3598, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Responsibility = new StorePropTag(3599, PropertyType.Boolean, new StorePropInfo("Responsibility", 3599, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SpoolerStatus = new StorePropTag(3600, PropertyType.Int32, new StorePropInfo("SpoolerStatus", 3600, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TransportStatus = new StorePropTag(3601, PropertyType.Int32, new StorePropInfo("TransportStatus", 3601, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageRecipients = new StorePropTag(3602, PropertyType.Object, new StorePropInfo("MessageRecipients", 3602, PropertyType.Object, StorePropInfo.Flags.None, 2305843009213693968UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageRecipientsMVBin = new StorePropTag(3602, PropertyType.MVBinary, new StorePropInfo("MessageRecipientsMVBin", 3602, PropertyType.MVBinary, StorePropInfo.Flags.None, 2305843009213693968UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageAttachments = new StorePropTag(3603, PropertyType.Object, new StorePropInfo("MessageAttachments", 3603, PropertyType.Object, StorePropInfo.Flags.None, 3458764513820540960UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ItemSubobjectsBin = new StorePropTag(3603, PropertyType.Binary, new StorePropInfo("ItemSubobjectsBin", 3603, PropertyType.Binary, StorePropInfo.Flags.None, 3458764513820540960UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SubmitFlags = new StorePropTag(3604, PropertyType.Int32, new StorePropInfo("SubmitFlags", 3604, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientStatus = new StorePropTag(3605, PropertyType.Int32, new StorePropInfo("RecipientStatus", 3605, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TransportKey = new StorePropTag(3606, PropertyType.Int32, new StorePropInfo("TransportKey", 3606, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MsgStatus = new StorePropTag(3607, PropertyType.Int32, new StorePropInfo("MsgStatus", 3607, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreationVersion = new StorePropTag(3609, PropertyType.Int64, new StorePropInfo("CreationVersion", 3609, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ModifyVersion = new StorePropTag(3610, PropertyType.Int64, new StorePropInfo("ModifyVersion", 3610, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HasAttach = new StorePropTag(3611, PropertyType.Boolean, new StorePropInfo("HasAttach", 3611, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyCRC = new StorePropTag(3612, PropertyType.Int32, new StorePropInfo("BodyCRC", 3612, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NormalizedSubject = new StorePropTag(3613, PropertyType.Unicode, new StorePropInfo("NormalizedSubject", 3613, PropertyType.Unicode, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(1)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RTFInSync = new StorePropTag(3615, PropertyType.Boolean, new StorePropInfo("RTFInSync", 3615, PropertyType.Boolean, StorePropInfo.Flags.None, 3458764513820540936UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Preprocess = new StorePropTag(3618, PropertyType.Boolean, new StorePropInfo("Preprocess", 3618, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetArticleNumber = new StorePropTag(3619, PropertyType.Int32, new StorePropInfo("InternetArticleNumber", 3619, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatingMTACertificate = new StorePropTag(3621, PropertyType.Binary, new StorePropInfo("OriginatingMTACertificate", 3621, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProofOfSubmission = new StorePropTag(3622, PropertyType.Binary, new StorePropInfo("ProofOfSubmission", 3622, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NTSecurityDescriptor = new StorePropTag(3623, PropertyType.Binary, new StorePropInfo("NTSecurityDescriptor", 3623, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PrimarySendAccount = new StorePropTag(3624, PropertyType.Unicode, new StorePropInfo("PrimarySendAccount", 3624, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NextSendAccount = new StorePropTag(3625, PropertyType.Unicode, new StorePropInfo("NextSendAccount", 3625, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TodoItemFlags = new StorePropTag(3627, PropertyType.Int32, new StorePropInfo("TodoItemFlags", 3627, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SwappedTODOStore = new StorePropTag(3628, PropertyType.Binary, new StorePropInfo("SwappedTODOStore", 3628, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SwappedTODOData = new StorePropTag(3629, PropertyType.Binary, new StorePropInfo("SwappedTODOData", 3629, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IMAPId = new StorePropTag(3631, PropertyType.Int32, new StorePropInfo("IMAPId", 3631, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSourceServerVersion = new StorePropTag(3633, PropertyType.Int16, new StorePropInfo("OriginalSourceServerVersion", 3633, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplFlags = new StorePropTag(3640, PropertyType.Int32, new StorePropInfo("ReplFlags", 3640, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageDeepAttachments = new StorePropTag(3642, PropertyType.Object, new StorePropInfo("MessageDeepAttachments", 3642, PropertyType.Object, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderGuid = new StorePropTag(3648, PropertyType.Binary, new StorePropInfo("SenderGuid", 3648, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingGuid = new StorePropTag(3649, PropertyType.Binary, new StorePropInfo("SentRepresentingGuid", 3649, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderGuid = new StorePropTag(3650, PropertyType.Binary, new StorePropInfo("OriginalSenderGuid", 3650, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingGuid = new StorePropTag(3651, PropertyType.Binary, new StorePropInfo("OriginalSentRepresentingGuid", 3651, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptGuid = new StorePropTag(3652, PropertyType.Binary, new StorePropInfo("ReadReceiptGuid", 3652, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportGuid = new StorePropTag(3653, PropertyType.Binary, new StorePropInfo("ReportGuid", 3653, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorGuid = new StorePropTag(3654, PropertyType.Binary, new StorePropInfo("OriginatorGuid", 3654, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationGuid = new StorePropTag(3655, PropertyType.Binary, new StorePropInfo("ReportDestinationGuid", 3655, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorGuid = new StorePropTag(3656, PropertyType.Binary, new StorePropInfo("OriginalAuthorGuid", 3656, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByGuid = new StorePropTag(3657, PropertyType.Binary, new StorePropInfo("ReceivedByGuid", 3657, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingGuid = new StorePropTag(3658, PropertyType.Binary, new StorePropInfo("ReceivedRepresentingGuid", 3658, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorGuid = new StorePropTag(3659, PropertyType.Binary, new StorePropInfo("CreatorGuid", 3659, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierGuid = new StorePropTag(3660, PropertyType.Binary, new StorePropInfo("LastModifierGuid", 3660, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSID = new StorePropTag(3661, PropertyType.Binary, new StorePropInfo("SenderSID", 3661, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSID = new StorePropTag(3662, PropertyType.Binary, new StorePropInfo("SentRepresentingSID", 3662, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSid = new StorePropTag(3663, PropertyType.Binary, new StorePropInfo("OriginalSenderSid", 3663, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSid = new StorePropTag(3664, PropertyType.Binary, new StorePropInfo("OriginalSentRepresentingSid", 3664, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSid = new StorePropTag(3665, PropertyType.Binary, new StorePropInfo("ReadReceiptSid", 3665, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportSid = new StorePropTag(3666, PropertyType.Binary, new StorePropInfo("ReportSid", 3666, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorSid = new StorePropTag(3667, PropertyType.Binary, new StorePropInfo("OriginatorSid", 3667, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationSid = new StorePropTag(3668, PropertyType.Binary, new StorePropInfo("ReportDestinationSid", 3668, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSid = new StorePropTag(3669, PropertyType.Binary, new StorePropInfo("OriginalAuthorSid", 3669, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdBySid = new StorePropTag(3670, PropertyType.Binary, new StorePropInfo("RcvdBySid", 3670, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdRepresentingSid = new StorePropTag(3671, PropertyType.Binary, new StorePropInfo("RcvdRepresentingSid", 3671, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorSID = new StorePropTag(3672, PropertyType.Binary, new StorePropInfo("CreatorSID", 3672, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierSid = new StorePropTag(3673, PropertyType.Binary, new StorePropInfo("LastModifierSid", 3673, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientCAI = new StorePropTag(3674, PropertyType.Binary, new StorePropInfo("RecipientCAI", 3674, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationCreatorSID = new StorePropTag(3675, PropertyType.Binary, new StorePropInfo("ConversationCreatorSID", 3675, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag URLCompNamePostfix = new StorePropTag(3681, PropertyType.Int32, new StorePropInfo("URLCompNamePostfix", 3681, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag URLCompNameSet = new StorePropTag(3682, PropertyType.Boolean, new StorePropInfo("URLCompNameSet", 3682, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Read = new StorePropTag(3689, PropertyType.Boolean, new StorePropInfo("Read", 3689, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, new PropertyCategories(1, 2)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorSidAsXML = new StorePropTag(3692, PropertyType.Unicode, new StorePropInfo("CreatorSidAsXML", 3692, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierSidAsXML = new StorePropTag(3693, PropertyType.Unicode, new StorePropInfo("LastModifierSidAsXML", 3693, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSIDAsXML = new StorePropTag(3694, PropertyType.Unicode, new StorePropInfo("SenderSIDAsXML", 3694, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSidAsXML = new StorePropTag(3695, PropertyType.Unicode, new StorePropInfo("SentRepresentingSidAsXML", 3695, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSIDAsXML = new StorePropTag(3696, PropertyType.Unicode, new StorePropInfo("OriginalSenderSIDAsXML", 3696, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSIDAsXML = new StorePropTag(3697, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingSIDAsXML", 3697, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSIDAsXML = new StorePropTag(3698, PropertyType.Unicode, new StorePropInfo("ReadReceiptSIDAsXML", 3698, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportSIDAsXML = new StorePropTag(3699, PropertyType.Unicode, new StorePropInfo("ReportSIDAsXML", 3699, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorSidAsXML = new StorePropTag(3700, PropertyType.Unicode, new StorePropInfo("OriginatorSidAsXML", 3700, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationSIDAsXML = new StorePropTag(3701, PropertyType.Unicode, new StorePropInfo("ReportDestinationSIDAsXML", 3701, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSIDAsXML = new StorePropTag(3702, PropertyType.Unicode, new StorePropInfo("OriginalAuthorSIDAsXML", 3702, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedBySIDAsXML = new StorePropTag(3703, PropertyType.Unicode, new StorePropInfo("ReceivedBySIDAsXML", 3703, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepersentingSIDAsXML = new StorePropTag(3704, PropertyType.Unicode, new StorePropInfo("ReceivedRepersentingSIDAsXML", 3704, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TrustSender = new StorePropTag(3705, PropertyType.Int32, new StorePropInfo("TrustSender", 3705, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSMTPAddress = new StorePropTag(3721, PropertyType.Unicode, new StorePropInfo("SenderSMTPAddress", 3721, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSMTPAddress = new StorePropTag(3722, PropertyType.Unicode, new StorePropInfo("SentRepresentingSMTPAddress", 3722, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSMTPAddress = new StorePropTag(3723, PropertyType.Unicode, new StorePropInfo("OriginalSenderSMTPAddress", 3723, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSMTPAddress = new StorePropTag(3724, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingSMTPAddress", 3724, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSMTPAddress = new StorePropTag(3725, PropertyType.Unicode, new StorePropInfo("ReadReceiptSMTPAddress", 3725, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportSMTPAddress = new StorePropTag(3726, PropertyType.Unicode, new StorePropInfo("ReportSMTPAddress", 3726, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorSMTPAddress = new StorePropTag(3727, PropertyType.Unicode, new StorePropInfo("OriginatorSMTPAddress", 3727, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationSMTPAddress = new StorePropTag(3728, PropertyType.Unicode, new StorePropInfo("ReportDestinationSMTPAddress", 3728, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSMTPAddress = new StorePropTag(3729, PropertyType.Unicode, new StorePropInfo("OriginalAuthorSMTPAddress", 3729, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedBySMTPAddress = new StorePropTag(3730, PropertyType.Unicode, new StorePropInfo("ReceivedBySMTPAddress", 3730, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingSMTPAddress = new StorePropTag(3731, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingSMTPAddress", 3731, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorSMTPAddress = new StorePropTag(3732, PropertyType.Unicode, new StorePropInfo("CreatorSMTPAddress", 3732, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierSMTPAddress = new StorePropTag(3733, PropertyType.Unicode, new StorePropInfo("LastModifierSMTPAddress", 3733, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VirusScannerStamp = new StorePropTag(3734, PropertyType.Binary, new StorePropInfo("VirusScannerStamp", 3734, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 6, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VirusTransportStamp = new StorePropTag(3734, PropertyType.MVUnicode, new StorePropInfo("VirusTransportStamp", 3734, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AddrTo = new StorePropTag(3735, PropertyType.Unicode, new StorePropInfo("AddrTo", 3735, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AddrCc = new StorePropTag(3736, PropertyType.Unicode, new StorePropInfo("AddrCc", 3736, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExtendedRuleActions = new StorePropTag(3737, PropertyType.Binary, new StorePropInfo("ExtendedRuleActions", 3737, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExtendedRuleCondition = new StorePropTag(3738, PropertyType.Binary, new StorePropInfo("ExtendedRuleCondition", 3738, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EntourageSentHistory = new StorePropTag(3743, PropertyType.MVUnicode, new StorePropInfo("EntourageSentHistory", 3743, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProofInProgress = new StorePropTag(3746, PropertyType.Int32, new StorePropInfo("ProofInProgress", 3746, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAttachmentsOLK = new StorePropTag(3749, PropertyType.Unicode, new StorePropInfo("SearchAttachmentsOLK", 3749, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipEmailTo = new StorePropTag(3750, PropertyType.Unicode, new StorePropInfo("SearchRecipEmailTo", 3750, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipEmailCc = new StorePropTag(3751, PropertyType.Unicode, new StorePropInfo("SearchRecipEmailCc", 3751, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipEmailBcc = new StorePropTag(3752, PropertyType.Unicode, new StorePropInfo("SearchRecipEmailBcc", 3752, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SFGAOFlags = new StorePropTag(3754, PropertyType.Int32, new StorePropInfo("SFGAOFlags", 3754, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchFullTextSubject = new StorePropTag(3756, PropertyType.Unicode, new StorePropInfo("SearchFullTextSubject", 3756, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchFullTextBody = new StorePropTag(3757, PropertyType.Unicode, new StorePropInfo("SearchFullTextBody", 3757, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FullTextConversationIndex = new StorePropTag(3758, PropertyType.Unicode, new StorePropInfo("FullTextConversationIndex", 3758, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAllIndexedProps = new StorePropTag(3759, PropertyType.Unicode, new StorePropInfo("SearchAllIndexedProps", 3759, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipients = new StorePropTag(3761, PropertyType.Unicode, new StorePropInfo("SearchRecipients", 3761, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipientsTo = new StorePropTag(3762, PropertyType.Unicode, new StorePropInfo("SearchRecipientsTo", 3762, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipientsCc = new StorePropTag(3763, PropertyType.Unicode, new StorePropInfo("SearchRecipientsCc", 3763, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchRecipientsBcc = new StorePropTag(3764, PropertyType.Unicode, new StorePropInfo("SearchRecipientsBcc", 3764, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAccountTo = new StorePropTag(3765, PropertyType.Unicode, new StorePropInfo("SearchAccountTo", 3765, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAccountCc = new StorePropTag(3766, PropertyType.Unicode, new StorePropInfo("SearchAccountCc", 3766, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAccountBcc = new StorePropTag(3767, PropertyType.Unicode, new StorePropInfo("SearchAccountBcc", 3767, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchEmailAddressTo = new StorePropTag(3768, PropertyType.Unicode, new StorePropInfo("SearchEmailAddressTo", 3768, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchEmailAddressCc = new StorePropTag(3769, PropertyType.Unicode, new StorePropInfo("SearchEmailAddressCc", 3769, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchEmailAddressBcc = new StorePropTag(3770, PropertyType.Unicode, new StorePropInfo("SearchEmailAddressBcc", 3770, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchSmtpAddressTo = new StorePropTag(3771, PropertyType.Unicode, new StorePropInfo("SearchSmtpAddressTo", 3771, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchSmtpAddressCc = new StorePropTag(3772, PropertyType.Unicode, new StorePropInfo("SearchSmtpAddressCc", 3772, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchSmtpAddressBcc = new StorePropTag(3773, PropertyType.Unicode, new StorePropInfo("SearchSmtpAddressBcc", 3773, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchSender = new StorePropTag(3774, PropertyType.Unicode, new StorePropInfo("SearchSender", 3774, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsIRMMessage = new StorePropTag(3789, PropertyType.Boolean, new StorePropInfo("IsIRMMessage", 3789, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchIsPartiallyIndexed = new StorePropTag(3790, PropertyType.Boolean, new StorePropInfo("SearchIsPartiallyIndexed", 3790, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RenewTime = new StorePropTag(3841, PropertyType.SysTime, new StorePropInfo("RenewTime", 3841, PropertyType.SysTime, StorePropInfo.Flags.None, 2341871806232657921UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeliveryOrRenewTime = new StorePropTag(3842, PropertyType.SysTime, new StorePropInfo("DeliveryOrRenewTime", 3842, PropertyType.SysTime, StorePropInfo.Flags.None, 2341871806232657921UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationFamilyId = new StorePropTag(3843, PropertyType.Binary, new StorePropInfo("ConversationFamilyId", 3843, PropertyType.Binary, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LikeCount = new StorePropTag(3844, PropertyType.Int32, new StorePropInfo("LikeCount", 3844, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RichContentDeprecated = new StorePropTag(3845, PropertyType.Int16, new StorePropInfo("RichContentDeprecated", 3845, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PeopleCentricConversationId = new StorePropTag(3846, PropertyType.Int32, new StorePropInfo("PeopleCentricConversationId", 3846, PropertyType.Int32, StorePropInfo.Flags.None, 9259400833873739776UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DiscoveryAnnotation = new StorePropTag(3854, PropertyType.Unicode, new StorePropInfo("DiscoveryAnnotation", 3854, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Access = new StorePropTag(4084, PropertyType.Int32, new StorePropInfo("Access", 4084, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RowType = new StorePropTag(4085, PropertyType.Int32, new StorePropInfo("RowType", 4085, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InstanceKey = new StorePropTag(4086, PropertyType.Binary, new StorePropInfo("InstanceKey", 4086, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InstanceKeySvrEid = new StorePropTag(4086, PropertyType.SvrEid, new StorePropInfo("InstanceKeySvrEid", 4086, PropertyType.SvrEid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AccessLevel = new StorePropTag(4087, PropertyType.Int32, new StorePropInfo("AccessLevel", 4087, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MappingSignature = new StorePropTag(4088, PropertyType.Binary, new StorePropInfo("MappingSignature", 4088, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecordKey = new StorePropTag(4089, PropertyType.Binary, new StorePropInfo("RecordKey", 4089, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecordKeySvrEid = new StorePropTag(4089, PropertyType.SvrEid, new StorePropInfo("RecordKeySvrEid", 4089, PropertyType.SvrEid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StoreRecordKey = new StorePropTag(4090, PropertyType.Binary, new StorePropInfo("StoreRecordKey", 4090, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StoreEntryId = new StorePropTag(4091, PropertyType.Binary, new StorePropInfo("StoreEntryId", 4091, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MiniIcon = new StorePropTag(4092, PropertyType.Binary, new StorePropInfo("MiniIcon", 4092, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Icon = new StorePropTag(4093, PropertyType.Binary, new StorePropInfo("Icon", 4093, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ObjectType = new StorePropTag(4094, PropertyType.Int32, new StorePropInfo("ObjectType", 4094, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EntryId = new StorePropTag(4095, PropertyType.Binary, new StorePropInfo("EntryId", 4095, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EntryIdSvrEid = new StorePropTag(4095, PropertyType.SvrEid, new StorePropInfo("EntryIdSvrEid", 4095, PropertyType.SvrEid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyUnicode = new StorePropTag(4096, PropertyType.Unicode, new StorePropInfo("BodyUnicode", 4096, PropertyType.Unicode, StorePropInfo.Flags.Private, 3458764513820540936UL, new PropertyCategories(12, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportText = new StorePropTag(4097, PropertyType.Unicode, new StorePropInfo("ReportText", 4097, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorAndDLExpansionHistory = new StorePropTag(4098, PropertyType.Binary, new StorePropInfo("OriginatorAndDLExpansionHistory", 4098, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportingDLName = new StorePropTag(4099, PropertyType.Binary, new StorePropInfo("ReportingDLName", 4099, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportingMTACertificate = new StorePropTag(4100, PropertyType.Binary, new StorePropInfo("ReportingMTACertificate", 4100, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfSyncBodyCrc = new StorePropTag(4102, PropertyType.Int32, new StorePropInfo("RtfSyncBodyCrc", 4102, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfSyncBodyCount = new StorePropTag(4103, PropertyType.Int32, new StorePropInfo("RtfSyncBodyCount", 4103, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfSyncBodyTag = new StorePropTag(4104, PropertyType.Unicode, new StorePropInfo("RtfSyncBodyTag", 4104, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfCompressed = new StorePropTag(4105, PropertyType.Binary, new StorePropInfo("RtfCompressed", 4105, PropertyType.Binary, StorePropInfo.Flags.Private, 3458764513820540936UL, new PropertyCategories(12, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AlternateBestBody = new StorePropTag(4106, PropertyType.Binary, new StorePropInfo("AlternateBestBody", 4106, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfSyncPrefixCount = new StorePropTag(4112, PropertyType.Int32, new StorePropInfo("RtfSyncPrefixCount", 4112, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RtfSyncTrailingCount = new StorePropTag(4113, PropertyType.Int32, new StorePropInfo("RtfSyncTrailingCount", 4113, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginallyIntendedRecipientEntryId = new StorePropTag(4114, PropertyType.Binary, new StorePropInfo("OriginallyIntendedRecipientEntryId", 4114, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyHtml = new StorePropTag(4115, PropertyType.Binary, new StorePropInfo("BodyHtml", 4115, PropertyType.Binary, StorePropInfo.Flags.Private, 3458764513820540936UL, new PropertyCategories(12, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyHtmlUnicode = new StorePropTag(4115, PropertyType.Unicode, new StorePropInfo("BodyHtmlUnicode", 4115, PropertyType.Unicode, StorePropInfo.Flags.Private, 3458764513820540936UL, new PropertyCategories(12, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyContentLocation = new StorePropTag(4116, PropertyType.Unicode, new StorePropInfo("BodyContentLocation", 4116, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyContentId = new StorePropTag(4117, PropertyType.Unicode, new StorePropInfo("BodyContentId", 4117, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NativeBodyInfo = new StorePropTag(4118, PropertyType.Int32, new StorePropInfo("NativeBodyInfo", 4118, PropertyType.Int32, StorePropInfo.Flags.None, 3458764513820540936UL, new PropertyCategories(3, 9, 10, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NativeBodyType = new StorePropTag(4118, PropertyType.Int16, new StorePropInfo("NativeBodyType", 4118, PropertyType.Int16, StorePropInfo.Flags.None, 3458764513820540936UL, new PropertyCategories(3, 9, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NativeBody = new StorePropTag(4118, PropertyType.Binary, new StorePropInfo("NativeBody", 4118, PropertyType.Binary, StorePropInfo.Flags.Private, 3458764513820540936UL, new PropertyCategories(3, 9, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AnnotationToken = new StorePropTag(4119, PropertyType.Binary, new StorePropInfo("AnnotationToken", 4119, PropertyType.Binary, StorePropInfo.Flags.Private, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetApproved = new StorePropTag(4144, PropertyType.Unicode, new StorePropInfo("InternetApproved", 4144, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetFollowupTo = new StorePropTag(4147, PropertyType.Unicode, new StorePropInfo("InternetFollowupTo", 4147, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetMessageId = new StorePropTag(4149, PropertyType.Unicode, new StorePropInfo("InternetMessageId", 4149, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InetNewsgroups = new StorePropTag(4150, PropertyType.Unicode, new StorePropInfo("InetNewsgroups", 4150, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetReferences = new StorePropTag(4153, PropertyType.Unicode, new StorePropInfo("InternetReferences", 4153, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PostReplyFolderEntries = new StorePropTag(4157, PropertyType.Binary, new StorePropInfo("PostReplyFolderEntries", 4157, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NNTPXRef = new StorePropTag(4160, PropertyType.Unicode, new StorePropInfo("NNTPXRef", 4160, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InReplyToId = new StorePropTag(4162, PropertyType.Unicode, new StorePropInfo("InReplyToId", 4162, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalInternetMessageId = new StorePropTag(4166, PropertyType.Unicode, new StorePropInfo("OriginalInternetMessageId", 4166, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IconIndex = new StorePropTag(4224, PropertyType.Int32, new StorePropInfo("IconIndex", 4224, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastVerbExecuted = new StorePropTag(4225, PropertyType.Int32, new StorePropInfo("LastVerbExecuted", 4225, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastVerbExecutionTime = new StorePropTag(4226, PropertyType.SysTime, new StorePropInfo("LastVerbExecutionTime", 4226, PropertyType.SysTime, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Relevance = new StorePropTag(4228, PropertyType.Int32, new StorePropInfo("Relevance", 4228, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FlagStatus = new StorePropTag(4240, PropertyType.Int32, new StorePropInfo("FlagStatus", 4240, PropertyType.Int32, StorePropInfo.Flags.None, 2305843009213693954UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FlagCompleteTime = new StorePropTag(4241, PropertyType.SysTime, new StorePropInfo("FlagCompleteTime", 4241, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormatPT = new StorePropTag(4242, PropertyType.Int32, new StorePropInfo("FormatPT", 4242, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FollowupIcon = new StorePropTag(4245, PropertyType.Int32, new StorePropInfo("FollowupIcon", 4245, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BlockStatus = new StorePropTag(4246, PropertyType.Int32, new StorePropInfo("BlockStatus", 4246, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ItemTempFlags = new StorePropTag(4247, PropertyType.Int32, new StorePropInfo("ItemTempFlags", 4247, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SMTPTempTblData = new StorePropTag(4288, PropertyType.Binary, new StorePropInfo("SMTPTempTblData", 4288, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SMTPTempTblData2 = new StorePropTag(4289, PropertyType.Int32, new StorePropInfo("SMTPTempTblData2", 4289, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SMTPTempTblData3 = new StorePropTag(4290, PropertyType.Binary, new StorePropInfo("SMTPTempTblData3", 4290, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DAVSubmitData = new StorePropTag(4294, PropertyType.Unicode, new StorePropInfo("DAVSubmitData", 4294, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ImapCachedMsgSize = new StorePropTag(4336, PropertyType.Binary, new StorePropInfo("ImapCachedMsgSize", 4336, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(1)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisableFullFidelity = new StorePropTag(4338, PropertyType.Boolean, new StorePropInfo("DisableFullFidelity", 4338, PropertyType.Boolean, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag URLCompName = new StorePropTag(4339, PropertyType.Unicode, new StorePropInfo("URLCompName", 4339, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AttrHidden = new StorePropTag(4340, PropertyType.Boolean, new StorePropInfo("AttrHidden", 4340, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AttrSystem = new StorePropTag(4341, PropertyType.Boolean, new StorePropInfo("AttrSystem", 4341, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AttrReadOnly = new StorePropTag(4342, PropertyType.Boolean, new StorePropInfo("AttrReadOnly", 4342, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PredictedActions = new StorePropTag(4612, PropertyType.MVInt16, new StorePropInfo("PredictedActions", 4612, PropertyType.MVInt16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GroupingActions = new StorePropTag(4613, PropertyType.MVInt16, new StorePropInfo("GroupingActions", 4613, PropertyType.MVInt16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PredictedActionsSummary = new StorePropTag(4614, PropertyType.Int32, new StorePropInfo("PredictedActionsSummary", 4614, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PredictedActionsThresholds = new StorePropTag(4615, PropertyType.Binary, new StorePropInfo("PredictedActionsThresholds", 4615, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsClutter = new StorePropTag(4615, PropertyType.Boolean, new StorePropInfo("IsClutter", 4615, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InferencePredictedReplyForwardReasons = new StorePropTag(4616, PropertyType.Binary, new StorePropInfo("InferencePredictedReplyForwardReasons", 4616, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InferencePredictedDeleteReasons = new StorePropTag(4617, PropertyType.Binary, new StorePropInfo("InferencePredictedDeleteReasons", 4617, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InferencePredictedIgnoreReasons = new StorePropTag(4618, PropertyType.Binary, new StorePropInfo("InferencePredictedIgnoreReasons", 4618, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDeliveryFolderInfo = new StorePropTag(4619, PropertyType.Binary, new StorePropInfo("OriginalDeliveryFolderInfo", 4619, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RowId = new StorePropTag(12288, PropertyType.Int32, new StorePropInfo("RowId", 12288, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 128UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AddressType = new StorePropTag(12290, PropertyType.Unicode, new StorePropInfo("AddressType", 12290, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EmailAddress = new StorePropTag(12291, PropertyType.Unicode, new StorePropInfo("EmailAddress", 12291, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Comment = new StorePropTag(12292, PropertyType.Unicode, new StorePropInfo("Comment", 12292, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Depth = new StorePropTag(12293, PropertyType.Int32, new StorePropInfo("Depth", 12293, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreationTime = new StorePropTag(12295, PropertyType.SysTime, new StorePropInfo("CreationTime", 12295, PropertyType.SysTime, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModificationTime = new StorePropTag(12296, PropertyType.SysTime, new StorePropInfo("LastModificationTime", 12296, PropertyType.SysTime, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchKey = new StorePropTag(12299, PropertyType.Binary, new StorePropInfo("SearchKey", 12299, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchKeySvrEid = new StorePropTag(12299, PropertyType.SvrEid, new StorePropInfo("SearchKeySvrEid", 12299, PropertyType.SvrEid, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TargetEntryId = new StorePropTag(12304, PropertyType.Binary, new StorePropInfo("TargetEntryId", 12304, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationId = new StorePropTag(12307, PropertyType.Binary, new StorePropInfo("ConversationId", 12307, PropertyType.Binary, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BodyTag = new StorePropTag(12308, PropertyType.Binary, new StorePropInfo("BodyTag", 12308, PropertyType.Binary, StorePropInfo.Flags.None, 4194304UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationIndexTrackingObsolete = new StorePropTag(12309, PropertyType.Int64, new StorePropInfo("ConversationIndexTrackingObsolete", 12309, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationIndexTracking = new StorePropTag(12310, PropertyType.Boolean, new StorePropInfo("ConversationIndexTracking", 12310, PropertyType.Boolean, StorePropInfo.Flags.None, 2341871806232658048UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ArchiveTag = new StorePropTag(12312, PropertyType.Binary, new StorePropInfo("ArchiveTag", 12312, PropertyType.Binary, StorePropInfo.Flags.None, 576460752304472064UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PolicyTag = new StorePropTag(12313, PropertyType.Binary, new StorePropInfo("PolicyTag", 12313, PropertyType.Binary, StorePropInfo.Flags.None, 2882303761518166016UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RetentionPeriod = new StorePropTag(12314, PropertyType.Int32, new StorePropInfo("RetentionPeriod", 12314, PropertyType.Int32, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartDateEtc = new StorePropTag(12315, PropertyType.Binary, new StorePropInfo("StartDateEtc", 12315, PropertyType.Binary, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RetentionDate = new StorePropTag(12316, PropertyType.SysTime, new StorePropInfo("RetentionDate", 12316, PropertyType.SysTime, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RetentionFlags = new StorePropTag(12317, PropertyType.Int32, new StorePropInfo("RetentionFlags", 12317, PropertyType.Int32, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ArchivePeriod = new StorePropTag(12318, PropertyType.Int32, new StorePropInfo("ArchivePeriod", 12318, PropertyType.Int32, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ArchiveDate = new StorePropTag(12319, PropertyType.SysTime, new StorePropInfo("ArchiveDate", 12319, PropertyType.SysTime, StorePropInfo.Flags.None, 288230376153808896UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormVersion = new StorePropTag(13057, PropertyType.Unicode, new StorePropInfo("FormVersion", 13057, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormCLSID = new StorePropTag(13058, PropertyType.Guid, new StorePropInfo("FormCLSID", 13058, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormContactName = new StorePropTag(13059, PropertyType.Unicode, new StorePropInfo("FormContactName", 13059, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormCategory = new StorePropTag(13060, PropertyType.Unicode, new StorePropInfo("FormCategory", 13060, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormCategorySub = new StorePropTag(13061, PropertyType.Unicode, new StorePropInfo("FormCategorySub", 13061, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormHidden = new StorePropTag(13063, PropertyType.Boolean, new StorePropInfo("FormHidden", 13063, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormDesignerName = new StorePropTag(13064, PropertyType.Unicode, new StorePropInfo("FormDesignerName", 13064, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormDesignerGuid = new StorePropTag(13065, PropertyType.Guid, new StorePropInfo("FormDesignerGuid", 13065, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FormMessageBehavior = new StorePropTag(13066, PropertyType.Int32, new StorePropInfo("FormMessageBehavior", 13066, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StoreSupportMask = new StorePropTag(13325, PropertyType.Int32, new StorePropInfo("StoreSupportMask", 13325, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MdbProvider = new StorePropTag(13332, PropertyType.Binary, new StorePropInfo("MdbProvider", 13332, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EventEmailReminderTimer = new StorePropTag(13408, PropertyType.SysTime, new StorePropInfo("EventEmailReminderTimer", 13408, PropertyType.SysTime, StorePropInfo.Flags.None, 9232379236109516800UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentCount = new StorePropTag(13826, PropertyType.Int32, new StorePropInfo("ContentCount", 13826, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UnreadCount = new StorePropTag(13827, PropertyType.Int32, new StorePropInfo("UnreadCount", 13827, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UnreadCountInt64 = new StorePropTag(13827, PropertyType.Int64, new StorePropInfo("UnreadCountInt64", 13827, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DetailsTable = new StorePropTag(13829, PropertyType.Object, new StorePropInfo("DetailsTable", 13829, PropertyType.Object, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AppointmentColorName = new StorePropTag(14044, PropertyType.Binary, new StorePropInfo("AppointmentColorName", 14044, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentId = new StorePropTag(14083, PropertyType.Unicode, new StorePropInfo("ContentId", 14083, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MimeUrl = new StorePropTag(14087, PropertyType.Unicode, new StorePropInfo("MimeUrl", 14087, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayType = new StorePropTag(14592, PropertyType.Int32, new StorePropInfo("DisplayType", 14592, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SmtpAddress = new StorePropTag(14846, PropertyType.Unicode, new StorePropInfo("SmtpAddress", 14846, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SimpleDisplayName = new StorePropTag(14847, PropertyType.Unicode, new StorePropInfo("SimpleDisplayName", 14847, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Account = new StorePropTag(14848, PropertyType.Unicode, new StorePropInfo("Account", 14848, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AlternateRecipient = new StorePropTag(14849, PropertyType.Binary, new StorePropInfo("AlternateRecipient", 14849, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CallbackTelephoneNumber = new StorePropTag(14850, PropertyType.Unicode, new StorePropInfo("CallbackTelephoneNumber", 14850, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversionProhibited = new StorePropTag(14851, PropertyType.Boolean, new StorePropInfo("ConversionProhibited", 14851, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Generation = new StorePropTag(14853, PropertyType.Unicode, new StorePropInfo("Generation", 14853, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GivenName = new StorePropTag(14854, PropertyType.Unicode, new StorePropInfo("GivenName", 14854, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GovernmentIDNumber = new StorePropTag(14855, PropertyType.Unicode, new StorePropInfo("GovernmentIDNumber", 14855, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BusinessTelephoneNumber = new StorePropTag(14856, PropertyType.Unicode, new StorePropInfo("BusinessTelephoneNumber", 14856, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeTelephoneNumber = new StorePropTag(14857, PropertyType.Unicode, new StorePropInfo("HomeTelephoneNumber", 14857, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Initials = new StorePropTag(14858, PropertyType.Unicode, new StorePropInfo("Initials", 14858, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Keyword = new StorePropTag(14859, PropertyType.Unicode, new StorePropInfo("Keyword", 14859, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Language = new StorePropTag(14860, PropertyType.Unicode, new StorePropInfo("Language", 14860, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Location = new StorePropTag(14861, PropertyType.Unicode, new StorePropInfo("Location", 14861, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailPermission = new StorePropTag(14862, PropertyType.Boolean, new StorePropInfo("MailPermission", 14862, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MHSCommonName = new StorePropTag(14863, PropertyType.Unicode, new StorePropInfo("MHSCommonName", 14863, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OrganizationalIDNumber = new StorePropTag(14864, PropertyType.Unicode, new StorePropInfo("OrganizationalIDNumber", 14864, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SurName = new StorePropTag(14865, PropertyType.Unicode, new StorePropInfo("SurName", 14865, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalEntryId = new StorePropTag(14866, PropertyType.Binary, new StorePropInfo("OriginalEntryId", 14866, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalDisplayName = new StorePropTag(14867, PropertyType.Unicode, new StorePropInfo("OriginalDisplayName", 14867, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSearchKey = new StorePropTag(14868, PropertyType.Binary, new StorePropInfo("OriginalSearchKey", 14868, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PostalAddress = new StorePropTag(14869, PropertyType.Unicode, new StorePropInfo("PostalAddress", 14869, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CompanyName = new StorePropTag(14870, PropertyType.Unicode, new StorePropInfo("CompanyName", 14870, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Title = new StorePropTag(14871, PropertyType.Unicode, new StorePropInfo("Title", 14871, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DepartmentName = new StorePropTag(14872, PropertyType.Unicode, new StorePropInfo("DepartmentName", 14872, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OfficeLocation = new StorePropTag(14873, PropertyType.Unicode, new StorePropInfo("OfficeLocation", 14873, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PrimaryTelephoneNumber = new StorePropTag(14874, PropertyType.Unicode, new StorePropInfo("PrimaryTelephoneNumber", 14874, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Business2TelephoneNumber = new StorePropTag(14875, PropertyType.Unicode, new StorePropInfo("Business2TelephoneNumber", 14875, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Business2TelephoneNumberMv = new StorePropTag(14875, PropertyType.MVUnicode, new StorePropInfo("Business2TelephoneNumberMv", 14875, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MobileTelephoneNumber = new StorePropTag(14876, PropertyType.Unicode, new StorePropInfo("MobileTelephoneNumber", 14876, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RadioTelephoneNumber = new StorePropTag(14877, PropertyType.Unicode, new StorePropInfo("RadioTelephoneNumber", 14877, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CarTelephoneNumber = new StorePropTag(14878, PropertyType.Unicode, new StorePropInfo("CarTelephoneNumber", 14878, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherTelephoneNumber = new StorePropTag(14879, PropertyType.Unicode, new StorePropInfo("OtherTelephoneNumber", 14879, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TransmitableDisplayName = new StorePropTag(14880, PropertyType.Unicode, new StorePropInfo("TransmitableDisplayName", 14880, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PagerTelephoneNumber = new StorePropTag(14881, PropertyType.Unicode, new StorePropInfo("PagerTelephoneNumber", 14881, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserCertificate = new StorePropTag(14882, PropertyType.Binary, new StorePropInfo("UserCertificate", 14882, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PrimaryFaxNumber = new StorePropTag(14883, PropertyType.Unicode, new StorePropInfo("PrimaryFaxNumber", 14883, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BusinessFaxNumber = new StorePropTag(14884, PropertyType.Unicode, new StorePropInfo("BusinessFaxNumber", 14884, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeFaxNumber = new StorePropTag(14885, PropertyType.Unicode, new StorePropInfo("HomeFaxNumber", 14885, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Country = new StorePropTag(14886, PropertyType.Unicode, new StorePropInfo("Country", 14886, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Locality = new StorePropTag(14887, PropertyType.Unicode, new StorePropInfo("Locality", 14887, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StateOrProvince = new StorePropTag(14888, PropertyType.Unicode, new StorePropInfo("StateOrProvince", 14888, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StreetAddress = new StorePropTag(14889, PropertyType.Unicode, new StorePropInfo("StreetAddress", 14889, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PostalCode = new StorePropTag(14890, PropertyType.Unicode, new StorePropInfo("PostalCode", 14890, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PostOfficeBox = new StorePropTag(14891, PropertyType.Unicode, new StorePropInfo("PostOfficeBox", 14891, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TelexNumber = new StorePropTag(14892, PropertyType.Unicode, new StorePropInfo("TelexNumber", 14892, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ISDNNumber = new StorePropTag(14893, PropertyType.Unicode, new StorePropInfo("ISDNNumber", 14893, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AssistantTelephoneNumber = new StorePropTag(14894, PropertyType.Unicode, new StorePropInfo("AssistantTelephoneNumber", 14894, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Home2TelephoneNumber = new StorePropTag(14895, PropertyType.Unicode, new StorePropInfo("Home2TelephoneNumber", 14895, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Home2TelephoneNumberMv = new StorePropTag(14895, PropertyType.MVUnicode, new StorePropInfo("Home2TelephoneNumberMv", 14895, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Assistant = new StorePropTag(14896, PropertyType.Unicode, new StorePropInfo("Assistant", 14896, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SendRichInfo = new StorePropTag(14912, PropertyType.Boolean, new StorePropInfo("SendRichInfo", 14912, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WeddingAnniversary = new StorePropTag(14913, PropertyType.SysTime, new StorePropInfo("WeddingAnniversary", 14913, PropertyType.SysTime, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Birthday = new StorePropTag(14914, PropertyType.SysTime, new StorePropInfo("Birthday", 14914, PropertyType.SysTime, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Hobbies = new StorePropTag(14915, PropertyType.Unicode, new StorePropInfo("Hobbies", 14915, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MiddleName = new StorePropTag(14916, PropertyType.Unicode, new StorePropInfo("MiddleName", 14916, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DisplayNamePrefix = new StorePropTag(14917, PropertyType.Unicode, new StorePropInfo("DisplayNamePrefix", 14917, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Profession = new StorePropTag(14918, PropertyType.Unicode, new StorePropInfo("Profession", 14918, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReferredByName = new StorePropTag(14919, PropertyType.Unicode, new StorePropInfo("ReferredByName", 14919, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SpouseName = new StorePropTag(14920, PropertyType.Unicode, new StorePropInfo("SpouseName", 14920, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ComputerNetworkName = new StorePropTag(14921, PropertyType.Unicode, new StorePropInfo("ComputerNetworkName", 14921, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CustomerId = new StorePropTag(14922, PropertyType.Unicode, new StorePropInfo("CustomerId", 14922, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TTYTDDPhoneNumber = new StorePropTag(14923, PropertyType.Unicode, new StorePropInfo("TTYTDDPhoneNumber", 14923, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FTPSite = new StorePropTag(14924, PropertyType.Unicode, new StorePropInfo("FTPSite", 14924, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Gender = new StorePropTag(14925, PropertyType.Int16, new StorePropInfo("Gender", 14925, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ManagerName = new StorePropTag(14926, PropertyType.Unicode, new StorePropInfo("ManagerName", 14926, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NickName = new StorePropTag(14927, PropertyType.Unicode, new StorePropInfo("NickName", 14927, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonalHomePage = new StorePropTag(14928, PropertyType.Unicode, new StorePropInfo("PersonalHomePage", 14928, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag BusinessHomePage = new StorePropTag(14929, PropertyType.Unicode, new StorePropInfo("BusinessHomePage", 14929, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContactVersion = new StorePropTag(14930, PropertyType.Guid, new StorePropInfo("ContactVersion", 14930, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContactEntryIds = new StorePropTag(14931, PropertyType.MVBinary, new StorePropInfo("ContactEntryIds", 14931, PropertyType.MVBinary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContactAddressTypes = new StorePropTag(14932, PropertyType.MVUnicode, new StorePropInfo("ContactAddressTypes", 14932, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContactDefaultAddressIndex = new StorePropTag(14933, PropertyType.Int32, new StorePropInfo("ContactDefaultAddressIndex", 14933, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContactEmailAddress = new StorePropTag(14934, PropertyType.MVUnicode, new StorePropInfo("ContactEmailAddress", 14934, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CompanyMainPhoneNumber = new StorePropTag(14935, PropertyType.Unicode, new StorePropInfo("CompanyMainPhoneNumber", 14935, PropertyType.Unicode, StorePropInfo.Flags.None, 2305843009213694208UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ChildrensNames = new StorePropTag(14936, PropertyType.MVUnicode, new StorePropInfo("ChildrensNames", 14936, PropertyType.MVUnicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressCity = new StorePropTag(14937, PropertyType.Unicode, new StorePropInfo("HomeAddressCity", 14937, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressCountry = new StorePropTag(14938, PropertyType.Unicode, new StorePropInfo("HomeAddressCountry", 14938, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressPostalCode = new StorePropTag(14939, PropertyType.Unicode, new StorePropInfo("HomeAddressPostalCode", 14939, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressStateOrProvince = new StorePropTag(14940, PropertyType.Unicode, new StorePropInfo("HomeAddressStateOrProvince", 14940, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressStreet = new StorePropTag(14941, PropertyType.Unicode, new StorePropInfo("HomeAddressStreet", 14941, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HomeAddressPostOfficeBox = new StorePropTag(14942, PropertyType.Unicode, new StorePropInfo("HomeAddressPostOfficeBox", 14942, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressCity = new StorePropTag(14943, PropertyType.Unicode, new StorePropInfo("OtherAddressCity", 14943, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressCountry = new StorePropTag(14944, PropertyType.Unicode, new StorePropInfo("OtherAddressCountry", 14944, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressPostalCode = new StorePropTag(14945, PropertyType.Unicode, new StorePropInfo("OtherAddressPostalCode", 14945, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressStateOrProvince = new StorePropTag(14946, PropertyType.Unicode, new StorePropInfo("OtherAddressStateOrProvince", 14946, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressStreet = new StorePropTag(14947, PropertyType.Unicode, new StorePropInfo("OtherAddressStreet", 14947, PropertyType.Unicode, StorePropInfo.Flags.None, 256UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OtherAddressPostOfficeBox = new StorePropTag(14948, PropertyType.Unicode, new StorePropInfo("OtherAddressPostOfficeBox", 14948, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserX509CertificateABSearchPath = new StorePropTag(14960, PropertyType.MVBinary, new StorePropInfo("UserX509CertificateABSearchPath", 14960, PropertyType.MVBinary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SendInternetEncoding = new StorePropTag(14961, PropertyType.Int32, new StorePropInfo("SendInternetEncoding", 14961, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PartnerNetworkId = new StorePropTag(14966, PropertyType.Unicode, new StorePropInfo("PartnerNetworkId", 14966, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PartnerNetworkUserId = new StorePropTag(14967, PropertyType.Unicode, new StorePropInfo("PartnerNetworkUserId", 14967, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PartnerNetworkThumbnailPhotoUrl = new StorePropTag(14968, PropertyType.Unicode, new StorePropInfo("PartnerNetworkThumbnailPhotoUrl", 14968, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PartnerNetworkProfilePhotoUrl = new StorePropTag(14969, PropertyType.Unicode, new StorePropInfo("PartnerNetworkProfilePhotoUrl", 14969, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PartnerNetworkContactType = new StorePropTag(14970, PropertyType.Unicode, new StorePropInfo("PartnerNetworkContactType", 14970, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RelevanceScore = new StorePropTag(14971, PropertyType.Int32, new StorePropInfo("RelevanceScore", 14971, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsDistributionListContact = new StorePropTag(14972, PropertyType.Boolean, new StorePropInfo("IsDistributionListContact", 14972, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsPromotedContact = new StorePropTag(14973, PropertyType.Boolean, new StorePropInfo("IsPromotedContact", 14973, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OrgUnitName = new StorePropTag(15358, PropertyType.Unicode, new StorePropInfo("OrgUnitName", 15358, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OrganizationName = new StorePropTag(15359, PropertyType.Unicode, new StorePropInfo("OrganizationName", 15359, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag TestBlobProperty = new StorePropTag(15616, PropertyType.Int64, new StorePropInfo("TestBlobProperty", 15616, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FilteringHooks = new StorePropTag(15624, PropertyType.Binary, new StorePropInfo("FilteringHooks", 15624, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailboxPartitionNumber = new StorePropTag(15775, PropertyType.Int32, new StorePropInfo("MailboxPartitionNumber", 15775, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailboxNumberInternal = new StorePropTag(15776, PropertyType.Int32, new StorePropInfo("MailboxNumberInternal", 15776, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VirtualParentDisplay = new StorePropTag(15781, PropertyType.Unicode, new StorePropInfo("VirtualParentDisplay", 15781, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternalConversationIndexTracking = new StorePropTag(15784, PropertyType.Boolean, new StorePropInfo("InternalConversationIndexTracking", 15784, PropertyType.Boolean, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternalConversationIndex = new StorePropTag(15785, PropertyType.Binary, new StorePropInfo("InternalConversationIndex", 15785, PropertyType.Binary, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationItemConversationId = new StorePropTag(15786, PropertyType.Binary, new StorePropInfo("ConversationItemConversationId", 15786, PropertyType.Binary, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VirtualUnreadMessageCount = new StorePropTag(15787, PropertyType.Int64, new StorePropInfo("VirtualUnreadMessageCount", 15787, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VirtualIsRead = new StorePropTag(15788, PropertyType.Boolean, new StorePropInfo("VirtualIsRead", 15788, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsReadColumn = new StorePropTag(15789, PropertyType.Boolean, new StorePropInfo("IsReadColumn", 15789, PropertyType.Boolean, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Internal9ByteChangeNumber = new StorePropTag(15791, PropertyType.Binary, new StorePropInfo("Internal9ByteChangeNumber", 15791, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Internal9ByteReadCnNew = new StorePropTag(15792, PropertyType.Binary, new StorePropInfo("Internal9ByteReadCnNew", 15792, PropertyType.Binary, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderLevelStub1 = new StorePropTag(15793, PropertyType.Boolean, new StorePropInfo("CategoryHeaderLevelStub1", 15793, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderLevelStub2 = new StorePropTag(15794, PropertyType.Boolean, new StorePropInfo("CategoryHeaderLevelStub2", 15794, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderLevelStub3 = new StorePropTag(15795, PropertyType.Boolean, new StorePropInfo("CategoryHeaderLevelStub3", 15795, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderAggregateProp0 = new StorePropTag(15796, PropertyType.Binary, new StorePropInfo("CategoryHeaderAggregateProp0", 15796, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderAggregateProp1 = new StorePropTag(15797, PropertyType.Binary, new StorePropInfo("CategoryHeaderAggregateProp1", 15797, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderAggregateProp2 = new StorePropTag(15798, PropertyType.Binary, new StorePropInfo("CategoryHeaderAggregateProp2", 15798, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategoryHeaderAggregateProp3 = new StorePropTag(15799, PropertyType.Binary, new StorePropInfo("CategoryHeaderAggregateProp3", 15799, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageFlagsActual = new StorePropTag(15805, PropertyType.Int32, new StorePropInfo("MessageFlagsActual", 15805, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternalChangeKey = new StorePropTag(15806, PropertyType.Binary, new StorePropInfo("InternalChangeKey", 15806, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternalSourceKey = new StorePropTag(15807, PropertyType.Binary, new StorePropInfo("InternalSourceKey", 15807, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				0,
				1,
				2,
				3,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HeaderFolderEntryId = new StorePropTag(15882, PropertyType.Binary, new StorePropInfo("HeaderFolderEntryId", 15882, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RemoteProgress = new StorePropTag(15883, PropertyType.Int32, new StorePropInfo("RemoteProgress", 15883, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RemoteProgressText = new StorePropTag(15884, PropertyType.Unicode, new StorePropInfo("RemoteProgressText", 15884, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VID = new StorePropTag(16264, PropertyType.Int64, new StorePropInfo("VID", 16264, PropertyType.Int64, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GVid = new StorePropTag(16265, PropertyType.Binary, new StorePropInfo("GVid", 16265, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GDID = new StorePropTag(16266, PropertyType.Binary, new StorePropInfo("GDID", 16266, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag XVid = new StorePropTag(16277, PropertyType.Binary, new StorePropInfo("XVid", 16277, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag GDefVid = new StorePropTag(16278, PropertyType.Binary, new StorePropInfo("GDefVid", 16278, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PrimaryMailboxOverQuota = new StorePropTag(16322, PropertyType.Boolean, new StorePropInfo("PrimaryMailboxOverQuota", 16322, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternalPostReply = new StorePropTag(16341, PropertyType.Binary, new StorePropInfo("InternalPostReply", 16341, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PreviewUnread = new StorePropTag(16344, PropertyType.Unicode, new StorePropInfo("PreviewUnread", 16344, PropertyType.Unicode, StorePropInfo.Flags.None, 2UL, new PropertyCategories(3, 9, 10, 15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Preview = new StorePropTag(16345, PropertyType.Unicode, new StorePropInfo("Preview", 16345, PropertyType.Unicode, StorePropInfo.Flags.Private, 2UL, new PropertyCategories(15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetCPID = new StorePropTag(16350, PropertyType.Int32, new StorePropInfo("InternetCPID", 16350, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AutoResponseSuppress = new StorePropTag(16351, PropertyType.Int32, new StorePropInfo("AutoResponseSuppress", 16351, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HasDAMs = new StorePropTag(16362, PropertyType.Boolean, new StorePropInfo("HasDAMs", 16362, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeferredSendNumber = new StorePropTag(16363, PropertyType.Int32, new StorePropInfo("DeferredSendNumber", 16363, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeferredSendUnits = new StorePropTag(16364, PropertyType.Int32, new StorePropInfo("DeferredSendUnits", 16364, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExpiryNumber = new StorePropTag(16365, PropertyType.Int32, new StorePropInfo("ExpiryNumber", 16365, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExpiryUnits = new StorePropTag(16366, PropertyType.Int32, new StorePropInfo("ExpiryUnits", 16366, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeferredSendTime = new StorePropTag(16367, PropertyType.SysTime, new StorePropInfo("DeferredSendTime", 16367, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageLocaleId = new StorePropTag(16369, PropertyType.Int32, new StorePropInfo("MessageLocaleId", 16369, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleTriggerHistory = new StorePropTag(16370, PropertyType.Binary, new StorePropInfo("RuleTriggerHistory", 16370, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MoveToStoreEid = new StorePropTag(16371, PropertyType.Binary, new StorePropInfo("MoveToStoreEid", 16371, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MoveToFolderEid = new StorePropTag(16372, PropertyType.Binary, new StorePropInfo("MoveToFolderEid", 16372, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StorageQuotaLimit = new StorePropTag(16373, PropertyType.Int32, new StorePropInfo("StorageQuotaLimit", 16373, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ExcessStorageUsed = new StorePropTag(16374, PropertyType.Int32, new StorePropInfo("ExcessStorageUsed", 16374, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ServerGeneratingQuotaMsg = new StorePropTag(16375, PropertyType.Unicode, new StorePropInfo("ServerGeneratingQuotaMsg", 16375, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorName = new StorePropTag(16376, PropertyType.Unicode, new StorePropInfo("CreatorName", 16376, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorEntryId = new StorePropTag(16377, PropertyType.Binary, new StorePropInfo("CreatorEntryId", 16377, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierName = new StorePropTag(16378, PropertyType.Unicode, new StorePropInfo("LastModifierName", 16378, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 5, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierEntryId = new StorePropTag(16379, PropertyType.Binary, new StorePropInfo("LastModifierEntryId", 16379, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageCodePage = new StorePropTag(16381, PropertyType.Int32, new StorePropInfo("MessageCodePage", 16381, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag QuotaType = new StorePropTag(16382, PropertyType.Int32, new StorePropInfo("QuotaType", 16382, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IsPublicFolderQuotaMessage = new StorePropTag(16383, PropertyType.Boolean, new StorePropInfo("IsPublicFolderQuotaMessage", 16383, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NewAttach = new StorePropTag(16384, PropertyType.Int32, new StorePropInfo("NewAttach", 16384, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartEmbed = new StorePropTag(16385, PropertyType.Int32, new StorePropInfo("StartEmbed", 16385, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndEmbed = new StorePropTag(16386, PropertyType.Int32, new StorePropInfo("EndEmbed", 16386, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartRecip = new StorePropTag(16387, PropertyType.Int32, new StorePropInfo("StartRecip", 16387, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndRecip = new StorePropTag(16388, PropertyType.Int32, new StorePropInfo("EndRecip", 16388, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndCcRecip = new StorePropTag(16389, PropertyType.Int32, new StorePropInfo("EndCcRecip", 16389, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndBccRecip = new StorePropTag(16390, PropertyType.Int32, new StorePropInfo("EndBccRecip", 16390, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndP1Recip = new StorePropTag(16391, PropertyType.Int32, new StorePropInfo("EndP1Recip", 16391, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DNPrefix = new StorePropTag(16392, PropertyType.Unicode, new StorePropInfo("DNPrefix", 16392, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartTopFolder = new StorePropTag(16393, PropertyType.Int32, new StorePropInfo("StartTopFolder", 16393, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartSubFolder = new StorePropTag(16394, PropertyType.Int32, new StorePropInfo("StartSubFolder", 16394, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndFolder = new StorePropTag(16395, PropertyType.Int32, new StorePropInfo("EndFolder", 16395, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartMessage = new StorePropTag(16396, PropertyType.Int32, new StorePropInfo("StartMessage", 16396, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndMessage = new StorePropTag(16397, PropertyType.Int32, new StorePropInfo("EndMessage", 16397, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EndAttach = new StorePropTag(16398, PropertyType.Int32, new StorePropInfo("EndAttach", 16398, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag EcWarning = new StorePropTag(16399, PropertyType.Int32, new StorePropInfo("EcWarning", 16399, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StartFAIMessage = new StorePropTag(16400, PropertyType.Int32, new StorePropInfo("StartFAIMessage", 16400, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NewFXFolder = new StorePropTag(16401, PropertyType.Binary, new StorePropInfo("NewFXFolder", 16401, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncChange = new StorePropTag(16402, PropertyType.Int32, new StorePropInfo("IncrSyncChange", 16402, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncDelete = new StorePropTag(16403, PropertyType.Int32, new StorePropInfo("IncrSyncDelete", 16403, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncEnd = new StorePropTag(16404, PropertyType.Int32, new StorePropInfo("IncrSyncEnd", 16404, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncMessage = new StorePropTag(16405, PropertyType.Int32, new StorePropInfo("IncrSyncMessage", 16405, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FastTransferDelProp = new StorePropTag(16406, PropertyType.Int32, new StorePropInfo("FastTransferDelProp", 16406, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IdsetGiven = new StorePropTag(16407, PropertyType.Binary, new StorePropInfo("IdsetGiven", 16407, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IdsetGivenInt32 = new StorePropTag(16407, PropertyType.Int32, new StorePropInfo("IdsetGivenInt32", 16407, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FastTransferErrorInfo = new StorePropTag(16408, PropertyType.Int32, new StorePropInfo("FastTransferErrorInfo", 16408, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderFlags = new StorePropTag(16409, PropertyType.Int32, new StorePropInfo("SenderFlags", 16409, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingFlags = new StorePropTag(16410, PropertyType.Int32, new StorePropInfo("SentRepresentingFlags", 16410, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdByFlags = new StorePropTag(16411, PropertyType.Int32, new StorePropInfo("RcvdByFlags", 16411, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdRepresentingFlags = new StorePropTag(16412, PropertyType.Int32, new StorePropInfo("RcvdRepresentingFlags", 16412, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderFlags = new StorePropTag(16413, PropertyType.Int32, new StorePropInfo("OriginalSenderFlags", 16413, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingFlags = new StorePropTag(16414, PropertyType.Int32, new StorePropInfo("OriginalSentRepresentingFlags", 16414, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportFlags = new StorePropTag(16415, PropertyType.Int32, new StorePropInfo("ReportFlags", 16415, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptFlags = new StorePropTag(16416, PropertyType.Int32, new StorePropInfo("ReadReceiptFlags", 16416, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SoftDeletes = new StorePropTag(16417, PropertyType.Binary, new StorePropInfo("SoftDeletes", 16417, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorAddressType = new StorePropTag(16418, PropertyType.Unicode, new StorePropInfo("CreatorAddressType", 16418, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorEmailAddr = new StorePropTag(16419, PropertyType.Unicode, new StorePropInfo("CreatorEmailAddr", 16419, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierAddressType = new StorePropTag(16420, PropertyType.Unicode, new StorePropInfo("LastModifierAddressType", 16420, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierEmailAddr = new StorePropTag(16421, PropertyType.Unicode, new StorePropInfo("LastModifierEmailAddr", 16421, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportAddressType = new StorePropTag(16422, PropertyType.Unicode, new StorePropInfo("ReportAddressType", 16422, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportEmailAddress = new StorePropTag(16423, PropertyType.Unicode, new StorePropInfo("ReportEmailAddress", 16423, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDisplayName = new StorePropTag(16424, PropertyType.Unicode, new StorePropInfo("ReportDisplayName", 16424, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptAddressType = new StorePropTag(16425, PropertyType.Unicode, new StorePropInfo("ReadReceiptAddressType", 16425, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptEmailAddress = new StorePropTag(16426, PropertyType.Unicode, new StorePropInfo("ReadReceiptEmailAddress", 16426, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptDisplayName = new StorePropTag(16427, PropertyType.Unicode, new StorePropInfo("ReadReceiptDisplayName", 16427, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IdsetRead = new StorePropTag(16429, PropertyType.Binary, new StorePropInfo("IdsetRead", 16429, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IdsetUnread = new StorePropTag(16430, PropertyType.Binary, new StorePropInfo("IdsetUnread", 16430, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncRead = new StorePropTag(16431, PropertyType.Int32, new StorePropInfo("IncrSyncRead", 16431, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSimpleDisplayName = new StorePropTag(16432, PropertyType.Unicode, new StorePropInfo("SenderSimpleDisplayName", 16432, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSimpleDisplayName = new StorePropTag(16433, PropertyType.Unicode, new StorePropInfo("SentRepresentingSimpleDisplayName", 16433, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSimpleDisplayName = new StorePropTag(16434, PropertyType.Unicode, new StorePropInfo("OriginalSenderSimpleDisplayName", 16434, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSimpleDisplayName = new StorePropTag(16435, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingSimpleDisplayName", 16435, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedBySimpleDisplayName = new StorePropTag(16436, PropertyType.Unicode, new StorePropInfo("ReceivedBySimpleDisplayName", 16436, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingSimpleDisplayName = new StorePropTag(16437, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingSimpleDisplayName", 16437, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSimpleDisplayName = new StorePropTag(16438, PropertyType.Unicode, new StorePropInfo("ReadReceiptSimpleDisplayName", 16438, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportSimpleDisplayName = new StorePropTag(16439, PropertyType.Unicode, new StorePropInfo("ReportSimpleDisplayName", 16439, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorSimpleDisplayName = new StorePropTag(16440, PropertyType.Unicode, new StorePropInfo("CreatorSimpleDisplayName", 16440, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierSimpleDisplayName = new StorePropTag(16441, PropertyType.Unicode, new StorePropInfo("LastModifierSimpleDisplayName", 16441, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncStateBegin = new StorePropTag(16442, PropertyType.Int32, new StorePropInfo("IncrSyncStateBegin", 16442, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncStateEnd = new StorePropTag(16443, PropertyType.Int32, new StorePropInfo("IncrSyncStateEnd", 16443, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncImailStream = new StorePropTag(16444, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream", 16444, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderOrgAddressType = new StorePropTag(16447, PropertyType.Unicode, new StorePropInfo("SenderOrgAddressType", 16447, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderOrgEmailAddr = new StorePropTag(16448, PropertyType.Unicode, new StorePropInfo("SenderOrgEmailAddr", 16448, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingOrgAddressType = new StorePropTag(16449, PropertyType.Unicode, new StorePropInfo("SentRepresentingOrgAddressType", 16449, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingOrgEmailAddr = new StorePropTag(16450, PropertyType.Unicode, new StorePropInfo("SentRepresentingOrgEmailAddr", 16450, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderOrgAddressType = new StorePropTag(16451, PropertyType.Unicode, new StorePropInfo("OriginalSenderOrgAddressType", 16451, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderOrgEmailAddr = new StorePropTag(16452, PropertyType.Unicode, new StorePropInfo("OriginalSenderOrgEmailAddr", 16452, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingOrgAddressType = new StorePropTag(16453, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingOrgAddressType", 16453, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingOrgEmailAddr = new StorePropTag(16454, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingOrgEmailAddr", 16454, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdByOrgAddressType = new StorePropTag(16455, PropertyType.Unicode, new StorePropInfo("RcvdByOrgAddressType", 16455, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdByOrgEmailAddr = new StorePropTag(16456, PropertyType.Unicode, new StorePropInfo("RcvdByOrgEmailAddr", 16456, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdRepresentingOrgAddressType = new StorePropTag(16457, PropertyType.Unicode, new StorePropInfo("RcvdRepresentingOrgAddressType", 16457, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RcvdRepresentingOrgEmailAddr = new StorePropTag(16458, PropertyType.Unicode, new StorePropInfo("RcvdRepresentingOrgEmailAddr", 16458, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptOrgAddressType = new StorePropTag(16459, PropertyType.Unicode, new StorePropInfo("ReadReceiptOrgAddressType", 16459, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptOrgEmailAddr = new StorePropTag(16460, PropertyType.Unicode, new StorePropInfo("ReadReceiptOrgEmailAddr", 16460, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportOrgAddressType = new StorePropTag(16461, PropertyType.Unicode, new StorePropInfo("ReportOrgAddressType", 16461, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportOrgEmailAddr = new StorePropTag(16462, PropertyType.Unicode, new StorePropInfo("ReportOrgEmailAddr", 16462, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorOrgAddressType = new StorePropTag(16463, PropertyType.Unicode, new StorePropInfo("CreatorOrgAddressType", 16463, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorOrgEmailAddr = new StorePropTag(16464, PropertyType.Unicode, new StorePropInfo("CreatorOrgEmailAddr", 16464, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierOrgAddressType = new StorePropTag(16465, PropertyType.Unicode, new StorePropInfo("LastModifierOrgAddressType", 16465, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierOrgEmailAddr = new StorePropTag(16466, PropertyType.Unicode, new StorePropInfo("LastModifierOrgEmailAddr", 16466, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorOrgAddressType = new StorePropTag(16467, PropertyType.Unicode, new StorePropInfo("OriginatorOrgAddressType", 16467, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorOrgEmailAddr = new StorePropTag(16468, PropertyType.Unicode, new StorePropInfo("OriginatorOrgEmailAddr", 16468, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationOrgEmailType = new StorePropTag(16469, PropertyType.Unicode, new StorePropInfo("ReportDestinationOrgEmailType", 16469, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationOrgEmailAddr = new StorePropTag(16470, PropertyType.Unicode, new StorePropInfo("ReportDestinationOrgEmailAddr", 16470, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorOrgAddressType = new StorePropTag(16471, PropertyType.Unicode, new StorePropInfo("OriginalAuthorOrgAddressType", 16471, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorOrgEmailAddr = new StorePropTag(16472, PropertyType.Unicode, new StorePropInfo("OriginalAuthorOrgEmailAddr", 16472, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorFlags = new StorePropTag(16473, PropertyType.Int32, new StorePropInfo("CreatorFlags", 16473, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierFlags = new StorePropTag(16474, PropertyType.Int32, new StorePropInfo("LastModifierFlags", 16474, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 7, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorFlags = new StorePropTag(16475, PropertyType.Int32, new StorePropInfo("OriginatorFlags", 16475, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationFlags = new StorePropTag(16476, PropertyType.Int32, new StorePropInfo("ReportDestinationFlags", 16476, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorFlags = new StorePropTag(16477, PropertyType.Int32, new StorePropInfo("OriginalAuthorFlags", 16477, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorSimpleDisplayName = new StorePropTag(16478, PropertyType.Unicode, new StorePropInfo("OriginatorSimpleDisplayName", 16478, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationSimpleDisplayName = new StorePropTag(16479, PropertyType.Unicode, new StorePropInfo("ReportDestinationSimpleDisplayName", 16479, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSimpleDispName = new StorePropTag(16480, PropertyType.Unicode, new StorePropInfo("OriginalAuthorSimpleDispName", 16480, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorSearchKey = new StorePropTag(16481, PropertyType.Binary, new StorePropInfo("OriginatorSearchKey", 16481, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationAddressType = new StorePropTag(16482, PropertyType.Unicode, new StorePropInfo("ReportDestinationAddressType", 16482, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationEmailAddress = new StorePropTag(16483, PropertyType.Unicode, new StorePropInfo("ReportDestinationEmailAddress", 16483, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationSearchKey = new StorePropTag(16484, PropertyType.Binary, new StorePropInfo("ReportDestinationSearchKey", 16484, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncImailStreamContinue = new StorePropTag(16486, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamContinue", 16486, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncImailStreamCancel = new StorePropTag(16487, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamCancel", 16487, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncImailStream2Continue = new StorePropTag(16497, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream2Continue", 16497, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncProgressMode = new StorePropTag(16500, PropertyType.Boolean, new StorePropInfo("IncrSyncProgressMode", 16500, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SyncProgressPerMsg = new StorePropTag(16501, PropertyType.Boolean, new StorePropInfo("SyncProgressPerMsg", 16501, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentFilterSCL = new StorePropTag(16502, PropertyType.Int32, new StorePropInfo("ContentFilterSCL", 16502, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncMsgPartial = new StorePropTag(16506, PropertyType.Int32, new StorePropInfo("IncrSyncMsgPartial", 16506, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncGroupInfo = new StorePropTag(16507, PropertyType.Int32, new StorePropInfo("IncrSyncGroupInfo", 16507, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncGroupId = new StorePropTag(16508, PropertyType.Int32, new StorePropInfo("IncrSyncGroupId", 16508, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IncrSyncChangePartial = new StorePropTag(16509, PropertyType.Int32, new StorePropInfo("IncrSyncChangePartial", 16509, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ContentFilterPCL = new StorePropTag(16516, PropertyType.Int32, new StorePropInfo("ContentFilterPCL", 16516, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeliverAsRead = new StorePropTag(22534, PropertyType.Boolean, new StorePropInfo("DeliverAsRead", 22534, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InetMailOverrideFormat = new StorePropTag(22786, PropertyType.Int32, new StorePropInfo("InetMailOverrideFormat", 22786, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageEditorFormat = new StorePropTag(22793, PropertyType.Int32, new StorePropInfo("MessageEditorFormat", 22793, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderSMTPAddressXSO = new StorePropTag(23809, PropertyType.Unicode, new StorePropInfo("SenderSMTPAddressXSO", 23809, PropertyType.Unicode, StorePropInfo.Flags.None, 11565243843087433728UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingSMTPAddressXSO = new StorePropTag(23810, PropertyType.Unicode, new StorePropInfo("SentRepresentingSMTPAddressXSO", 23810, PropertyType.Unicode, StorePropInfo.Flags.None, 11529215046068469760UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderSMTPAddressXSO = new StorePropTag(23811, PropertyType.Unicode, new StorePropInfo("OriginalSenderSMTPAddressXSO", 23811, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingSMTPAddressXSO = new StorePropTag(23812, PropertyType.Unicode, new StorePropInfo("OriginalSentRepresentingSMTPAddressXSO", 23812, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptSMTPAddressXSO = new StorePropTag(23813, PropertyType.Unicode, new StorePropInfo("ReadReceiptSMTPAddressXSO", 23813, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorSMTPAddressXSO = new StorePropTag(23814, PropertyType.Unicode, new StorePropInfo("OriginalAuthorSMTPAddressXSO", 23814, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedBySMTPAddressXSO = new StorePropTag(23815, PropertyType.Unicode, new StorePropInfo("ReceivedBySMTPAddressXSO", 23815, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingSMTPAddressXSO = new StorePropTag(23816, PropertyType.Unicode, new StorePropInfo("ReceivedRepresentingSMTPAddressXSO", 23816, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientOrder = new StorePropTag(24543, PropertyType.Int32, new StorePropInfo("RecipientOrder", 24543, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientSipUri = new StorePropTag(24549, PropertyType.Unicode, new StorePropInfo("RecipientSipUri", 24549, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientDisplayName = new StorePropTag(24566, PropertyType.Unicode, new StorePropInfo("RecipientDisplayName", 24566, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientEntryId = new StorePropTag(24567, PropertyType.Binary, new StorePropInfo("RecipientEntryId", 24567, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientFlags = new StorePropTag(24573, PropertyType.Int32, new StorePropInfo("RecipientFlags", 24573, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientTrackStatus = new StorePropTag(24575, PropertyType.Int32, new StorePropInfo("RecipientTrackStatus", 24575, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DotStuffState = new StorePropTag(24577, PropertyType.Unicode, new StorePropInfo("DotStuffState", 24577, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetMessageIdHash = new StorePropTag(25088, PropertyType.Int32, new StorePropInfo("InternetMessageIdHash", 25088, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationTopicHash = new StorePropTag(25089, PropertyType.Int32, new StorePropInfo("ConversationTopicHash", 25089, PropertyType.Int32, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MimeSkeleton = new StorePropTag(25840, PropertyType.Binary, new StorePropInfo("MimeSkeleton", 25840, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyTemplateId = new StorePropTag(26050, PropertyType.Binary, new StorePropInfo("ReplyTemplateId", 26050, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SecureSubmitFlags = new StorePropTag(26054, PropertyType.Int32, new StorePropInfo("SecureSubmitFlags", 26054, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SourceKey = new StorePropTag(26080, PropertyType.Binary, new StorePropInfo("SourceKey", 26080, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentSourceKey = new StorePropTag(26081, PropertyType.Binary, new StorePropInfo("ParentSourceKey", 26081, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ChangeKey = new StorePropTag(26082, PropertyType.Binary, new StorePropInfo("ChangeKey", 26082, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(2, 3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PredecessorChangeList = new StorePropTag(26083, PropertyType.Binary, new StorePropInfo("PredecessorChangeList", 26083, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(2, 3, 4, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgState = new StorePropTag(26089, PropertyType.Int32, new StorePropInfo("RuleMsgState", 26089, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgUserFlags = new StorePropTag(26090, PropertyType.Int32, new StorePropInfo("RuleMsgUserFlags", 26090, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgProvider = new StorePropTag(26091, PropertyType.Unicode, new StorePropInfo("RuleMsgProvider", 26091, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgName = new StorePropTag(26092, PropertyType.Unicode, new StorePropInfo("RuleMsgName", 26092, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgLevel = new StorePropTag(26093, PropertyType.Int32, new StorePropInfo("RuleMsgLevel", 26093, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgProviderData = new StorePropTag(26094, PropertyType.Binary, new StorePropInfo("RuleMsgProviderData", 26094, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgActions = new StorePropTag(26095, PropertyType.Binary, new StorePropInfo("RuleMsgActions", 26095, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgCondition = new StorePropTag(26096, PropertyType.Binary, new StorePropInfo("RuleMsgCondition", 26096, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgConditionLCID = new StorePropTag(26097, PropertyType.Int32, new StorePropInfo("RuleMsgConditionLCID", 26097, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgVersion = new StorePropTag(26098, PropertyType.Int16, new StorePropInfo("RuleMsgVersion", 26098, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgSequence = new StorePropTag(26099, PropertyType.Int32, new StorePropInfo("RuleMsgSequence", 26099, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LISSD = new StorePropTag(26105, PropertyType.Binary, new StorePropInfo("LISSD", 26105, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplicaServer = new StorePropTag(26180, PropertyType.Unicode, new StorePropInfo("ReplicaServer", 26180, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DAMOriginalEntryId = new StorePropTag(26182, PropertyType.Binary, new StorePropInfo("DAMOriginalEntryId", 26182, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag HasNamedProperties = new StorePropTag(26186, PropertyType.Boolean, new StorePropInfo("HasNamedProperties", 26186, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FidMid = new StorePropTag(26188, PropertyType.Binary, new StorePropInfo("FidMid", 26188, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InternetContent = new StorePropTag(26201, PropertyType.Binary, new StorePropInfo("InternetContent", 26201, PropertyType.Binary, StorePropInfo.Flags.None, 1152921504606848000UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorName = new StorePropTag(26203, PropertyType.Unicode, new StorePropInfo("OriginatorName", 26203, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorEmailAddress = new StorePropTag(26204, PropertyType.Unicode, new StorePropInfo("OriginatorEmailAddress", 26204, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorAddressType = new StorePropTag(26205, PropertyType.Unicode, new StorePropInfo("OriginatorAddressType", 26205, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorEntryId = new StorePropTag(26206, PropertyType.Binary, new StorePropInfo("OriginatorEntryId", 26206, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RecipientNumber = new StorePropTag(26210, PropertyType.Int32, new StorePropInfo("RecipientNumber", 26210, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationName = new StorePropTag(26212, PropertyType.Unicode, new StorePropInfo("ReportDestinationName", 26212, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationEntryId = new StorePropTag(26213, PropertyType.Binary, new StorePropInfo("ReportDestinationEntryId", 26213, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProhibitReceiveQuota = new StorePropTag(26218, PropertyType.Int32, new StorePropInfo("ProhibitReceiveQuota", 26218, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SearchAttachments = new StorePropTag(26221, PropertyType.Unicode, new StorePropInfo("SearchAttachments", 26221, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ProhibitSendQuota = new StorePropTag(26222, PropertyType.Int32, new StorePropInfo("ProhibitSendQuota", 26222, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SubmittedByAdmin = new StorePropTag(26223, PropertyType.Boolean, new StorePropInfo("SubmittedByAdmin", 26223, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 4, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LongTermEntryIdFromTable = new StorePropTag(26224, PropertyType.Binary, new StorePropInfo("LongTermEntryIdFromTable", 26224, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleIds = new StorePropTag(26229, PropertyType.Binary, new StorePropInfo("RuleIds", 26229, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgConditionOld = new StorePropTag(26233, PropertyType.Binary, new StorePropInfo("RuleMsgConditionOld", 26233, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RuleMsgActionsOld = new StorePropTag(26240, PropertyType.Binary, new StorePropInfo("RuleMsgActionsOld", 26240, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DeletedOn = new StorePropTag(26255, PropertyType.SysTime, new StorePropInfo("DeletedOn", 26255, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CodePageId = new StorePropTag(26307, PropertyType.Int32, new StorePropInfo("CodePageId", 26307, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserDN = new StorePropTag(26314, PropertyType.Unicode, new StorePropInfo("UserDN", 26314, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailboxDSGuidGuid = new StorePropTag(26375, PropertyType.Guid, new StorePropInfo("MailboxDSGuidGuid", 26375, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag URLName = new StorePropTag(26375, PropertyType.Unicode, new StorePropInfo("URLName", 26375, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LocalCommitTime = new StorePropTag(26377, PropertyType.SysTime, new StorePropInfo("LocalCommitTime", 26377, PropertyType.SysTime, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AutoReset = new StorePropTag(26380, PropertyType.MVGuid, new StorePropInfo("AutoReset", 26380, PropertyType.MVGuid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ELCAutoCopyTag = new StorePropTag(26390, PropertyType.Binary, new StorePropInfo("ELCAutoCopyTag", 26390, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ELCMoveDate = new StorePropTag(26391, PropertyType.Binary, new StorePropInfo("ELCMoveDate", 26391, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PropGroupInfo = new StorePropTag(26430, PropertyType.Binary, new StorePropInfo("PropGroupInfo", 26430, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PropertyGroupChangeMask = new StorePropTag(26430, PropertyType.Int32, new StorePropInfo("PropertyGroupChangeMask", 26430, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadCnNewExport = new StorePropTag(26431, PropertyType.Binary, new StorePropInfo("ReadCnNewExport", 26431, PropertyType.Binary, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentMailSvrEID = new StorePropTag(26432, PropertyType.SvrEid, new StorePropInfo("SentMailSvrEID", 26432, PropertyType.SvrEid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentMailSvrEIDBin = new StorePropTag(26432, PropertyType.Binary, new StorePropInfo("SentMailSvrEIDBin", 26432, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LocallyDelivered = new StorePropTag(26437, PropertyType.Binary, new StorePropInfo("LocallyDelivered", 26437, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MimeSize = new StorePropTag(26438, PropertyType.Int64, new StorePropInfo("MimeSize", 26438, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MimeSize32 = new StorePropTag(26438, PropertyType.Int32, new StorePropInfo("MimeSize32", 26438, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FileSize = new StorePropTag(26439, PropertyType.Int64, new StorePropInfo("FileSize", 26439, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FileSize32 = new StorePropTag(26439, PropertyType.Int32, new StorePropInfo("FileSize32", 26439, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Fid = new StorePropTag(26440, PropertyType.Int64, new StorePropInfo("Fid", 26440, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FidBin = new StorePropTag(26440, PropertyType.Binary, new StorePropInfo("FidBin", 26440, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentFid = new StorePropTag(26441, PropertyType.Int64, new StorePropInfo("ParentFid", 26441, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Mid = new StorePropTag(26442, PropertyType.Int64, new StorePropInfo("Mid", 26442, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MidBin = new StorePropTag(26442, PropertyType.Binary, new StorePropInfo("MidBin", 26442, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CategID = new StorePropTag(26443, PropertyType.Int64, new StorePropInfo("CategID", 26443, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ParentCategID = new StorePropTag(26444, PropertyType.Int64, new StorePropInfo("ParentCategID", 26444, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InstanceId = new StorePropTag(26445, PropertyType.Int64, new StorePropInfo("InstanceId", 26445, PropertyType.Int64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag InstanceNum = new StorePropTag(26446, PropertyType.Int32, new StorePropInfo("InstanceNum", 26446, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ChangeType = new StorePropTag(26448, PropertyType.Int16, new StorePropInfo("ChangeType", 26448, PropertyType.Int16, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag RequiresRefResolve = new StorePropTag(26449, PropertyType.Boolean, new StorePropInfo("RequiresRefResolve", 26449, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LTID = new StorePropTag(26456, PropertyType.Binary, new StorePropInfo("LTID", 26456, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9, 10, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnExport = new StorePropTag(26457, PropertyType.Binary, new StorePropInfo("CnExport", 26457, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PclExport = new StorePropTag(26458, PropertyType.Binary, new StorePropInfo("PclExport", 26458, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnMvExport = new StorePropTag(26459, PropertyType.Binary, new StorePropInfo("CnMvExport", 26459, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(1, 3, 4, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailboxGuid = new StorePropTag(26476, PropertyType.Binary, new StorePropInfo("MailboxGuid", 26476, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MapiEntryIdGuidGuid = new StorePropTag(26476, PropertyType.Guid, new StorePropInfo("MapiEntryIdGuidGuid", 26476, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ImapCachedBodystructure = new StorePropTag(26477, PropertyType.Binary, new StorePropInfo("ImapCachedBodystructure", 26477, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag StorageQuota = new StorePropTag(26491, PropertyType.Int32, new StorePropInfo("StorageQuota", 26491, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnsetIn = new StorePropTag(26516, PropertyType.Binary, new StorePropInfo("CnsetIn", 26516, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				10,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnsetSeen = new StorePropTag(26518, PropertyType.Binary, new StorePropInfo("CnsetSeen", 26518, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ChangeNumber = new StorePropTag(26532, PropertyType.Int64, new StorePropInfo("ChangeNumber", 26532, PropertyType.Int64, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ChangeNumberBin = new StorePropTag(26532, PropertyType.Binary, new StorePropInfo("ChangeNumberBin", 26532, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PCL = new StorePropTag(26533, PropertyType.Binary, new StorePropInfo("PCL", 26533, PropertyType.Binary, StorePropInfo.Flags.None, 1UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnMv = new StorePropTag(26534, PropertyType.MVInt64, new StorePropInfo("CnMv", 26534, PropertyType.MVInt64, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SourceEntryId = new StorePropTag(26536, PropertyType.Binary, new StorePropInfo("SourceEntryId", 26536, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailFlags = new StorePropTag(26537, PropertyType.Int16, new StorePropInfo("MailFlags", 26537, PropertyType.Int16, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Associated = new StorePropTag(26538, PropertyType.Boolean, new StorePropInfo("Associated", 26538, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SubmitResponsibility = new StorePropTag(26539, PropertyType.Int32, new StorePropInfo("SubmitResponsibility", 26539, PropertyType.Int32, StorePropInfo.Flags.None, 2UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SharedReceiptHandling = new StorePropTag(26541, PropertyType.Boolean, new StorePropInfo("SharedReceiptHandling", 26541, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Inid = new StorePropTag(26544, PropertyType.Binary, new StorePropInfo("Inid", 26544, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MessageAttachList = new StorePropTag(26547, PropertyType.Binary, new StorePropInfo("MessageAttachList", 26547, PropertyType.Binary, StorePropInfo.Flags.None, 32UL, new PropertyCategories(1, 2, 3, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderCAI = new StorePropTag(26549, PropertyType.Binary, new StorePropInfo("SenderCAI", 26549, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SentRepresentingCAI = new StorePropTag(26550, PropertyType.Binary, new StorePropInfo("SentRepresentingCAI", 26550, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSenderCAI = new StorePropTag(26551, PropertyType.Binary, new StorePropInfo("OriginalSenderCAI", 26551, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalSentRepresentingCAI = new StorePropTag(26552, PropertyType.Binary, new StorePropInfo("OriginalSentRepresentingCAI", 26552, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedByCAI = new StorePropTag(26553, PropertyType.Binary, new StorePropInfo("ReceivedByCAI", 26553, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReceivedRepresentingCAI = new StorePropTag(26554, PropertyType.Binary, new StorePropInfo("ReceivedRepresentingCAI", 26554, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadReceiptCAI = new StorePropTag(26555, PropertyType.Binary, new StorePropInfo("ReadReceiptCAI", 26555, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportCAI = new StorePropTag(26556, PropertyType.Binary, new StorePropInfo("ReportCAI", 26556, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CreatorCAI = new StorePropTag(26557, PropertyType.Binary, new StorePropInfo("CreatorCAI", 26557, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LastModifierCAI = new StorePropTag(26558, PropertyType.Binary, new StorePropInfo("LastModifierCAI", 26558, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnsetRead = new StorePropTag(26578, PropertyType.Binary, new StorePropInfo("CnsetRead", 26578, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag CnsetSeenFAI = new StorePropTag(26586, PropertyType.Binary, new StorePropInfo("CnsetSeenFAI", 26586, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag IdSetDeleted = new StorePropTag(26597, PropertyType.Binary, new StorePropInfo("IdSetDeleted", 26597, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginatorCAI = new StorePropTag(26616, PropertyType.Binary, new StorePropInfo("OriginatorCAI", 26616, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReportDestinationCAI = new StorePropTag(26617, PropertyType.Binary, new StorePropInfo("ReportDestinationCAI", 26617, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14,
				18
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OriginalAuthorCAI = new StorePropTag(26618, PropertyType.Binary, new StorePropInfo("OriginalAuthorCAI", 26618, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				9,
				13,
				14,
				18
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadCnNew = new StorePropTag(26622, PropertyType.Int64, new StorePropInfo("ReadCnNew", 26622, PropertyType.Int64, StorePropInfo.Flags.None, 2UL, new PropertyCategories(new int[]
			{
				1,
				2,
				3,
				4,
				9,
				14
			})), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReadCnNewBin = new StorePropTag(26622, PropertyType.Binary, new StorePropInfo("ReadCnNewBin", 26622, PropertyType.Binary, StorePropInfo.Flags.None, 2UL, new PropertyCategories(1, 2, 3, 9, 14)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SenderTelephoneNumber = new StorePropTag(26626, PropertyType.Unicode, new StorePropInfo("SenderTelephoneNumber", 26626, PropertyType.Unicode, StorePropInfo.Flags.None, 128UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag VoiceMessageAttachmentOrder = new StorePropTag(26629, PropertyType.Unicode, new StorePropInfo("VoiceMessageAttachmentOrder", 26629, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag DocumentId = new StorePropTag(26645, PropertyType.Int32, new StorePropInfo("DocumentId", 26645, PropertyType.Int32, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationIdHash = new StorePropTag(26663, PropertyType.Int32, new StorePropInfo("ConversationIdHash", 26663, PropertyType.Int32, StorePropInfo.Flags.None, 2341871806232658048UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationDocumentId = new StorePropTag(26662, PropertyType.Int32, new StorePropInfo("ConversationDocumentId", 26662, PropertyType.Int32, StorePropInfo.Flags.None, 1UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag LocalDirectoryBlob = new StorePropTag(26664, PropertyType.Binary, new StorePropInfo("LocalDirectoryBlob", 26664, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ViewStyle = new StorePropTag(26676, PropertyType.Int32, new StorePropInfo("ViewStyle", 26676, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FreebusyEMA = new StorePropTag(26697, PropertyType.Unicode, new StorePropInfo("FreebusyEMA", 26697, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WunderbarLinkEntryID = new StorePropTag(26700, PropertyType.Binary, new StorePropInfo("WunderbarLinkEntryID", 26700, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WunderbarLinkStoreEntryId = new StorePropTag(26702, PropertyType.Binary, new StorePropInfo("WunderbarLinkStoreEntryId", 26702, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SchdInfoFreebusyMerged = new StorePropTag(26704, PropertyType.MVBinary, new StorePropInfo("SchdInfoFreebusyMerged", 26704, PropertyType.MVBinary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WunderbarLinkGroupClsId = new StorePropTag(26704, PropertyType.Binary, new StorePropInfo("WunderbarLinkGroupClsId", 26704, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WunderbarLinkGroupName = new StorePropTag(26705, PropertyType.Unicode, new StorePropInfo("WunderbarLinkGroupName", 26705, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag WunderbarLinkSection = new StorePropTag(26706, PropertyType.Int32, new StorePropInfo("WunderbarLinkSection", 26706, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NavigationNodeCalendarColor = new StorePropTag(26707, PropertyType.Int32, new StorePropInfo("NavigationNodeCalendarColor", 26707, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NavigationNodeAddressbookEntryId = new StorePropTag(26708, PropertyType.Binary, new StorePropInfo("NavigationNodeAddressbookEntryId", 26708, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingDeleteItems = new StorePropTag(26709, PropertyType.Int32, new StorePropInfo("AgingDeleteItems", 26709, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingFileName9AndPrev = new StorePropTag(26710, PropertyType.Unicode, new StorePropInfo("AgingFileName9AndPrev", 26710, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingAgeFolder = new StorePropTag(26711, PropertyType.Boolean, new StorePropInfo("AgingAgeFolder", 26711, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingDontAgeMe = new StorePropTag(26712, PropertyType.Boolean, new StorePropInfo("AgingDontAgeMe", 26712, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingFileNameAfter9 = new StorePropTag(26713, PropertyType.Unicode, new StorePropInfo("AgingFileNameAfter9", 26713, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingWhenDeletedOnServer = new StorePropTag(26715, PropertyType.Boolean, new StorePropInfo("AgingWhenDeletedOnServer", 26715, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag AgingWaitUntilExpired = new StorePropTag(26716, PropertyType.Boolean, new StorePropInfo("AgingWaitUntilExpired", 26716, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvFrom = new StorePropTag(26752, PropertyType.MVUnicode, new StorePropInfo("ConversationMvFrom", 26752, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvFromMailboxWide = new StorePropTag(26753, PropertyType.MVUnicode, new StorePropInfo("ConversationMvFromMailboxWide", 26753, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvTo = new StorePropTag(26754, PropertyType.MVUnicode, new StorePropInfo("ConversationMvTo", 26754, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvToMailboxWide = new StorePropTag(26755, PropertyType.MVUnicode, new StorePropInfo("ConversationMvToMailboxWide", 26755, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageDeliveryTime = new StorePropTag(26756, PropertyType.SysTime, new StorePropInfo("ConversationMessageDeliveryTime", 26756, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageDeliveryTimeMailboxWide = new StorePropTag(26757, PropertyType.SysTime, new StorePropInfo("ConversationMessageDeliveryTimeMailboxWide", 26757, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationCategories = new StorePropTag(26758, PropertyType.MVUnicode, new StorePropInfo("ConversationCategories", 26758, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationCategoriesMailboxWide = new StorePropTag(26759, PropertyType.MVUnicode, new StorePropInfo("ConversationCategoriesMailboxWide", 26759, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationFlagStatus = new StorePropTag(26760, PropertyType.Int32, new StorePropInfo("ConversationFlagStatus", 26760, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationFlagStatusMailboxWide = new StorePropTag(26761, PropertyType.Int32, new StorePropInfo("ConversationFlagStatusMailboxWide", 26761, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationFlagCompleteTime = new StorePropTag(26762, PropertyType.SysTime, new StorePropInfo("ConversationFlagCompleteTime", 26762, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationFlagCompleteTimeMailboxWide = new StorePropTag(26763, PropertyType.SysTime, new StorePropInfo("ConversationFlagCompleteTimeMailboxWide", 26763, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasAttach = new StorePropTag(26764, PropertyType.Boolean, new StorePropInfo("ConversationHasAttach", 26764, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasAttachMailboxWide = new StorePropTag(26765, PropertyType.Boolean, new StorePropInfo("ConversationHasAttachMailboxWide", 26765, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationContentCount = new StorePropTag(26766, PropertyType.Int32, new StorePropInfo("ConversationContentCount", 26766, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationContentCountMailboxWide = new StorePropTag(26767, PropertyType.Int32, new StorePropInfo("ConversationContentCountMailboxWide", 26767, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationContentUnread = new StorePropTag(26768, PropertyType.Int32, new StorePropInfo("ConversationContentUnread", 26768, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationContentUnreadMailboxWide = new StorePropTag(26769, PropertyType.Int32, new StorePropInfo("ConversationContentUnreadMailboxWide", 26769, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageSize = new StorePropTag(26770, PropertyType.Int32, new StorePropInfo("ConversationMessageSize", 26770, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageSizeMailboxWide = new StorePropTag(26771, PropertyType.Int32, new StorePropInfo("ConversationMessageSizeMailboxWide", 26771, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageClasses = new StorePropTag(26772, PropertyType.MVUnicode, new StorePropInfo("ConversationMessageClasses", 26772, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageClassesMailboxWide = new StorePropTag(26773, PropertyType.MVUnicode, new StorePropInfo("ConversationMessageClassesMailboxWide", 26773, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationReplyForwardState = new StorePropTag(26774, PropertyType.Int32, new StorePropInfo("ConversationReplyForwardState", 26774, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationReplyForwardStateMailboxWide = new StorePropTag(26775, PropertyType.Int32, new StorePropInfo("ConversationReplyForwardStateMailboxWide", 26775, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationImportance = new StorePropTag(26776, PropertyType.Int32, new StorePropInfo("ConversationImportance", 26776, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationImportanceMailboxWide = new StorePropTag(26777, PropertyType.Int32, new StorePropInfo("ConversationImportanceMailboxWide", 26777, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvFromUnread = new StorePropTag(26778, PropertyType.MVUnicode, new StorePropInfo("ConversationMvFromUnread", 26778, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvFromUnreadMailboxWide = new StorePropTag(26779, PropertyType.MVUnicode, new StorePropInfo("ConversationMvFromUnreadMailboxWide", 26779, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvItemIds = new StorePropTag(26784, PropertyType.MVBinary, new StorePropInfo("ConversationMvItemIds", 26784, PropertyType.MVBinary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMvItemIdsMailboxWide = new StorePropTag(26785, PropertyType.MVBinary, new StorePropInfo("ConversationMvItemIdsMailboxWide", 26785, PropertyType.MVBinary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasIrm = new StorePropTag(26786, PropertyType.Boolean, new StorePropInfo("ConversationHasIrm", 26786, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasIrmMailboxWide = new StorePropTag(26787, PropertyType.Boolean, new StorePropInfo("ConversationHasIrmMailboxWide", 26787, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonCompanyNameMailboxWide = new StorePropTag(26788, PropertyType.Unicode, new StorePropInfo("PersonCompanyNameMailboxWide", 26788, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonDisplayNameMailboxWide = new StorePropTag(26789, PropertyType.Unicode, new StorePropInfo("PersonDisplayNameMailboxWide", 26789, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonGivenNameMailboxWide = new StorePropTag(26790, PropertyType.Unicode, new StorePropInfo("PersonGivenNameMailboxWide", 26790, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonSurnameMailboxWide = new StorePropTag(26791, PropertyType.Unicode, new StorePropInfo("PersonSurnameMailboxWide", 26791, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonPhotoContactEntryIdMailboxWide = new StorePropTag(26792, PropertyType.Binary, new StorePropInfo("PersonPhotoContactEntryIdMailboxWide", 26792, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredImportanceInternal = new StorePropTag(26794, PropertyType.Real64, new StorePropInfo("ConversationInferredImportanceInternal", 26794, PropertyType.Real64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredImportanceOverride = new StorePropTag(26795, PropertyType.Int32, new StorePropInfo("ConversationInferredImportanceOverride", 26795, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredUnimportanceInternal = new StorePropTag(26796, PropertyType.Real64, new StorePropInfo("ConversationInferredUnimportanceInternal", 26796, PropertyType.Real64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredImportanceInternalMailboxWide = new StorePropTag(26797, PropertyType.Real64, new StorePropInfo("ConversationInferredImportanceInternalMailboxWide", 26797, PropertyType.Real64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredImportanceOverrideMailboxWide = new StorePropTag(26798, PropertyType.Int32, new StorePropInfo("ConversationInferredImportanceOverrideMailboxWide", 26798, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInferredUnimportanceInternalMailboxWide = new StorePropTag(26799, PropertyType.Real64, new StorePropInfo("ConversationInferredUnimportanceInternalMailboxWide", 26799, PropertyType.Real64, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonFileAsMailboxWide = new StorePropTag(26800, PropertyType.Unicode, new StorePropInfo("PersonFileAsMailboxWide", 26800, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonRelevanceScoreMailboxWide = new StorePropTag(26801, PropertyType.Int32, new StorePropInfo("PersonRelevanceScoreMailboxWide", 26801, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonIsDistributionListMailboxWide = new StorePropTag(26802, PropertyType.Boolean, new StorePropInfo("PersonIsDistributionListMailboxWide", 26802, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonHomeCityMailboxWide = new StorePropTag(26803, PropertyType.Unicode, new StorePropInfo("PersonHomeCityMailboxWide", 26803, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonCreationTimeMailboxWide = new StorePropTag(26804, PropertyType.SysTime, new StorePropInfo("PersonCreationTimeMailboxWide", 26804, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonGALLinkIDMailboxWide = new StorePropTag(26807, PropertyType.Guid, new StorePropInfo("PersonGALLinkIDMailboxWide", 26807, PropertyType.Guid, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonMvEmailAddressMailboxWide = new StorePropTag(26810, PropertyType.MVUnicode, new StorePropInfo("PersonMvEmailAddressMailboxWide", 26810, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonMvEmailDisplayNameMailboxWide = new StorePropTag(26811, PropertyType.MVUnicode, new StorePropInfo("PersonMvEmailDisplayNameMailboxWide", 26811, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonMvEmailRoutingTypeMailboxWide = new StorePropTag(26812, PropertyType.MVUnicode, new StorePropInfo("PersonMvEmailRoutingTypeMailboxWide", 26812, PropertyType.MVUnicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonImAddressMailboxWide = new StorePropTag(26813, PropertyType.Unicode, new StorePropInfo("PersonImAddressMailboxWide", 26813, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonWorkCityMailboxWide = new StorePropTag(26814, PropertyType.Unicode, new StorePropInfo("PersonWorkCityMailboxWide", 26814, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonDisplayNameFirstLastMailboxWide = new StorePropTag(26815, PropertyType.Unicode, new StorePropInfo("PersonDisplayNameFirstLastMailboxWide", 26815, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag PersonDisplayNameLastFirstMailboxWide = new StorePropTag(26816, PropertyType.Unicode, new StorePropInfo("PersonDisplayNameLastFirstMailboxWide", 26816, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationGroupingActions = new StorePropTag(26814, PropertyType.MVInt16, new StorePropInfo("ConversationGroupingActions", 26814, PropertyType.MVInt16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationGroupingActionsMailboxWide = new StorePropTag(26815, PropertyType.MVInt16, new StorePropInfo("ConversationGroupingActionsMailboxWide", 26815, PropertyType.MVInt16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationPredictedActionsSummary = new StorePropTag(26816, PropertyType.Int32, new StorePropInfo("ConversationPredictedActionsSummary", 26816, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationPredictedActionsSummaryMailboxWide = new StorePropTag(26817, PropertyType.Int32, new StorePropInfo("ConversationPredictedActionsSummaryMailboxWide", 26817, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasClutter = new StorePropTag(26818, PropertyType.Boolean, new StorePropInfo("ConversationHasClutter", 26818, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationHasClutterMailboxWide = new StorePropTag(26819, PropertyType.Boolean, new StorePropInfo("ConversationHasClutterMailboxWide", 26819, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationLastMemberDocumentId = new StorePropTag(26880, PropertyType.Int32, new StorePropInfo("ConversationLastMemberDocumentId", 26880, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationPreview = new StorePropTag(26881, PropertyType.Unicode, new StorePropInfo("ConversationPreview", 26881, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationLastMemberDocumentIdMailboxWide = new StorePropTag(26882, PropertyType.Int32, new StorePropInfo("ConversationLastMemberDocumentIdMailboxWide", 26882, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationInitialMemberDocumentId = new StorePropTag(26883, PropertyType.Int32, new StorePropInfo("ConversationInitialMemberDocumentId", 26883, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMemberDocumentIds = new StorePropTag(26884, PropertyType.MVInt32, new StorePropInfo("ConversationMemberDocumentIds", 26884, PropertyType.MVInt32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageDeliveryOrRenewTimeMailboxWide = new StorePropTag(26885, PropertyType.SysTime, new StorePropInfo("ConversationMessageDeliveryOrRenewTimeMailboxWide", 26885, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FamilyId = new StorePropTag(26886, PropertyType.Binary, new StorePropInfo("FamilyId", 26886, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageRichContentMailboxWide = new StorePropTag(26887, PropertyType.MVInt16, new StorePropInfo("ConversationMessageRichContentMailboxWide", 26887, PropertyType.MVInt16, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationPreviewMailboxWide = new StorePropTag(26888, PropertyType.Unicode, new StorePropInfo("ConversationPreviewMailboxWide", 26888, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationMessageDeliveryOrRenewTime = new StorePropTag(26889, PropertyType.SysTime, new StorePropInfo("ConversationMessageDeliveryOrRenewTime", 26889, PropertyType.SysTime, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ConversationWorkingSetSourcePartition = new StorePropTag(26890, PropertyType.Unicode, new StorePropInfo("ConversationWorkingSetSourcePartition", 26890, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag NDRFromName = new StorePropTag(26885, PropertyType.Unicode, new StorePropInfo("NDRFromName", 26885, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SecurityFlags = new StorePropTag(28161, PropertyType.Int32, new StorePropInfo("SecurityFlags", 28161, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SecurityReceiptRequestProcessed = new StorePropTag(28164, PropertyType.Boolean, new StorePropInfo("SecurityReceiptRequestProcessed", 28164, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FavoriteDisplayName = new StorePropTag(31744, PropertyType.Unicode, new StorePropInfo("FavoriteDisplayName", 31744, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FavoriteDisplayAlias = new StorePropTag(31745, PropertyType.Unicode, new StorePropInfo("FavoriteDisplayAlias", 31745, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FavPublicSourceKey = new StorePropTag(31746, PropertyType.Binary, new StorePropInfo("FavPublicSourceKey", 31746, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag SyncFolderSourceKey = new StorePropTag(31747, PropertyType.Binary, new StorePropInfo("SyncFolderSourceKey", 31747, PropertyType.Binary, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserConfigurationDataType = new StorePropTag(31750, PropertyType.Int32, new StorePropInfo("UserConfigurationDataType", 31750, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserConfigurationXmlStream = new StorePropTag(31752, PropertyType.Binary, new StorePropInfo("UserConfigurationXmlStream", 31752, PropertyType.Binary, StorePropInfo.Flags.Private, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag UserConfigurationStream = new StorePropTag(31753, PropertyType.Binary, new StorePropInfo("UserConfigurationStream", 31753, PropertyType.Binary, StorePropInfo.Flags.Private, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag ReplyFwdStatus = new StorePropTag(31755, PropertyType.Unicode, new StorePropInfo("ReplyFwdStatus", 31755, PropertyType.Unicode, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag OscSyncEnabledOnServer = new StorePropTag(31780, PropertyType.Boolean, new StorePropInfo("OscSyncEnabledOnServer", 31780, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag Processed = new StorePropTag(32001, PropertyType.Boolean, new StorePropInfo("Processed", 32001, PropertyType.Boolean, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag FavLevelMask = new StorePropTag(32003, PropertyType.Int32, new StorePropInfo("FavLevelMask", 32003, PropertyType.Int32, StorePropInfo.Flags.None, 9223372036854775808UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[]
			{
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey
			};

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[]
			{
				PropTag.Message.Subject,
				PropTag.Message.SubjectPrefix,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.SentMailEntryId,
				PropTag.Message.MsgStatus,
				PropTag.Message.NormalizedSubject,
				PropTag.Message.Read,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.MappingSignature,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.StoreRecordKey,
				PropTag.Message.StoreEntryId,
				PropTag.Message.ObjectType,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.ImapCachedMsgSize,
				PropTag.Message.StoreSupportMask,
				PropTag.Message.MdbProvider,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.InternalPostReply,
				PropTag.Message.ReplicaServer,
				PropTag.Message.LongTermEntryIdFromTable,
				PropTag.Message.DeletedOn,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.LocallyDelivered,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.RequiresRefResolve,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.ImapCachedBodystructure,
				PropTag.Message.StorageQuota,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.SourceEntryId,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SharedReceiptHandling,
				PropTag.Message.Inid,
				PropTag.Message.MessageAttachList,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin,
				PropTag.Message.DocumentId
			};

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[]
			{
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.MsgStatus,
				PropTag.Message.Read,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.MappingSignature,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.StoreRecordKey,
				PropTag.Message.StoreEntryId,
				PropTag.Message.ObjectType,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.StoreSupportMask,
				PropTag.Message.MdbProvider,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.InternalPostReply,
				PropTag.Message.SourceKey,
				PropTag.Message.ChangeKey,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.ReplicaServer,
				PropTag.Message.LongTermEntryIdFromTable,
				PropTag.Message.DeletedOn,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.ImapCachedBodystructure,
				PropTag.Message.StorageQuota,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.SourceEntryId,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SharedReceiptHandling,
				PropTag.Message.Inid,
				PropTag.Message.MessageAttachList,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin,
				PropTag.Message.DocumentId
			};

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.Message.DisplayBcc,
				PropTag.Message.DisplayCc,
				PropTag.Message.DisplayTo,
				PropTag.Message.ParentDisplay,
				PropTag.Message.MessageSize,
				PropTag.Message.MessageSize32,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.MessageRecipients,
				PropTag.Message.MessageRecipientsMVBin,
				PropTag.Message.MessageAttachments,
				PropTag.Message.ItemSubobjectsBin,
				PropTag.Message.SubmitFlags,
				PropTag.Message.HasAttach,
				PropTag.Message.InternetArticleNumber,
				PropTag.Message.IMAPId,
				PropTag.Message.MessageDeepAttachments,
				PropTag.Message.CreatorGuid,
				PropTag.Message.LastModifierGuid,
				PropTag.Message.CreatorSID,
				PropTag.Message.LastModifierSid,
				PropTag.Message.VirusScannerStamp,
				PropTag.Message.VirusTransportStamp,
				PropTag.Message.SearchAttachmentsOLK,
				PropTag.Message.SearchRecipEmailTo,
				PropTag.Message.SearchRecipEmailCc,
				PropTag.Message.SearchRecipEmailBcc,
				PropTag.Message.SearchFullTextSubject,
				PropTag.Message.SearchFullTextBody,
				PropTag.Message.SearchAllIndexedProps,
				PropTag.Message.SearchRecipients,
				PropTag.Message.SearchRecipientsTo,
				PropTag.Message.SearchRecipientsCc,
				PropTag.Message.SearchRecipientsBcc,
				PropTag.Message.SearchAccountTo,
				PropTag.Message.SearchAccountCc,
				PropTag.Message.SearchAccountBcc,
				PropTag.Message.SearchEmailAddressTo,
				PropTag.Message.SearchEmailAddressCc,
				PropTag.Message.SearchEmailAddressBcc,
				PropTag.Message.SearchSmtpAddressTo,
				PropTag.Message.SearchSmtpAddressCc,
				PropTag.Message.SearchSmtpAddressBcc,
				PropTag.Message.SearchSender,
				PropTag.Message.SearchIsPartiallyIndexed,
				PropTag.Message.Access,
				PropTag.Message.RowType,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.AccessLevel,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.NativeBodyInfo,
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody,
				PropTag.Message.Depth,
				PropTag.Message.CreationTime,
				PropTag.Message.LastModificationTime,
				PropTag.Message.ConversationId,
				PropTag.Message.ContentCount,
				PropTag.Message.UnreadCount,
				PropTag.Message.UnreadCountInt64,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.PreviewUnread,
				PropTag.Message.CreatorName,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.LastModifierName,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.NewAttach,
				PropTag.Message.StartEmbed,
				PropTag.Message.EndEmbed,
				PropTag.Message.StartRecip,
				PropTag.Message.EndRecip,
				PropTag.Message.EndCcRecip,
				PropTag.Message.EndBccRecip,
				PropTag.Message.EndP1Recip,
				PropTag.Message.DNPrefix,
				PropTag.Message.StartTopFolder,
				PropTag.Message.StartSubFolder,
				PropTag.Message.EndFolder,
				PropTag.Message.StartMessage,
				PropTag.Message.EndMessage,
				PropTag.Message.EndAttach,
				PropTag.Message.EcWarning,
				PropTag.Message.StartFAIMessage,
				PropTag.Message.NewFXFolder,
				PropTag.Message.IncrSyncChange,
				PropTag.Message.IncrSyncDelete,
				PropTag.Message.IncrSyncEnd,
				PropTag.Message.IncrSyncMessage,
				PropTag.Message.FastTransferDelProp,
				PropTag.Message.IdsetGiven,
				PropTag.Message.IdsetGivenInt32,
				PropTag.Message.FastTransferErrorInfo,
				PropTag.Message.SoftDeletes,
				PropTag.Message.CreatorAddressType,
				PropTag.Message.CreatorEmailAddr,
				PropTag.Message.LastModifierAddressType,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.IdsetRead,
				PropTag.Message.IdsetUnread,
				PropTag.Message.IncrSyncRead,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.IncrSyncStateBegin,
				PropTag.Message.IncrSyncStateEnd,
				PropTag.Message.IncrSyncImailStream,
				PropTag.Message.CreatorOrgAddressType,
				PropTag.Message.CreatorOrgEmailAddr,
				PropTag.Message.LastModifierOrgAddressType,
				PropTag.Message.LastModifierOrgEmailAddr,
				PropTag.Message.CreatorFlags,
				PropTag.Message.LastModifierFlags,
				PropTag.Message.IncrSyncImailStreamContinue,
				PropTag.Message.IncrSyncImailStreamCancel,
				PropTag.Message.IncrSyncImailStream2Continue,
				PropTag.Message.IncrSyncProgressMode,
				PropTag.Message.SyncProgressPerMsg,
				PropTag.Message.IncrSyncMsgPartial,
				PropTag.Message.IncrSyncGroupInfo,
				PropTag.Message.IncrSyncGroupId,
				PropTag.Message.IncrSyncChangePartial,
				PropTag.Message.InternetMessageIdHash,
				PropTag.Message.ConversationTopicHash,
				PropTag.Message.SourceKey,
				PropTag.Message.ParentSourceKey,
				PropTag.Message.ChangeKey,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.HasNamedProperties,
				PropTag.Message.SearchAttachments,
				PropTag.Message.SubmittedByAdmin,
				PropTag.Message.DeletedOn,
				PropTag.Message.MailboxDSGuidGuid,
				PropTag.Message.URLName,
				PropTag.Message.LocalCommitTime,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.ParentFid,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.CategID,
				PropTag.Message.ParentCategID,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.ChangeType,
				PropTag.Message.LTID,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.ImapCachedBodystructure,
				PropTag.Message.StorageQuota,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.SourceEntryId,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SharedReceiptHandling,
				PropTag.Message.Inid,
				PropTag.Message.MessageAttachList,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin,
				PropTag.Message.DocumentId,
				PropTag.Message.MailboxNum,
				PropTag.Message.ConversationIdHash,
				PropTag.Message.ConversationDocumentId
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[]
			{
				PropTag.Message.InternetArticleNumber,
				PropTag.Message.IMAPId,
				PropTag.Message.CreatorGuid,
				PropTag.Message.LastModifierGuid,
				PropTag.Message.CreatorSID,
				PropTag.Message.LastModifierSid,
				PropTag.Message.CreationTime,
				PropTag.Message.LastModificationTime,
				PropTag.Message.CreatorName,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.LastModifierName,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.CreatorAddressType,
				PropTag.Message.CreatorEmailAddr,
				PropTag.Message.LastModifierAddressType,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.CreatorOrgAddressType,
				PropTag.Message.CreatorOrgEmailAddr,
				PropTag.Message.LastModifierOrgAddressType,
				PropTag.Message.LastModifierOrgEmailAddr,
				PropTag.Message.CreatorFlags,
				PropTag.Message.LastModifierFlags,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.SubmittedByAdmin,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.ChangeNumber,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.MailFlags,
				PropTag.Message.ReadCnNew
			};

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[]
			{
				PropTag.Message.LastModifierName
			};

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[]
			{
				PropTag.Message.VirusScannerStamp
			};

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[]
			{
				PropTag.Message.LastModifierGuid,
				PropTag.Message.LastModifierSid,
				PropTag.Message.LastModificationTime,
				PropTag.Message.LastModifierName,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.LastModifierAddressType,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.LastModifierOrgAddressType,
				PropTag.Message.LastModifierOrgEmailAddr,
				PropTag.Message.LastModifierFlags
			};

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[]
			{
				PropTag.Message.Subject,
				PropTag.Message.DisplayBcc,
				PropTag.Message.DisplayCc,
				PropTag.Message.DisplayTo,
				PropTag.Message.ParentDisplay,
				PropTag.Message.MessageSize,
				PropTag.Message.MessageSize32,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.MessageRecipients,
				PropTag.Message.MessageRecipientsMVBin,
				PropTag.Message.MessageAttachments,
				PropTag.Message.ItemSubobjectsBin,
				PropTag.Message.SubmitFlags,
				PropTag.Message.MsgStatus,
				PropTag.Message.HasAttach,
				PropTag.Message.InternetArticleNumber,
				PropTag.Message.IMAPId,
				PropTag.Message.MessageDeepAttachments,
				PropTag.Message.CreatorGuid,
				PropTag.Message.LastModifierGuid,
				PropTag.Message.CreatorSID,
				PropTag.Message.LastModifierSid,
				PropTag.Message.VirusScannerStamp,
				PropTag.Message.VirusTransportStamp,
				PropTag.Message.SearchAttachmentsOLK,
				PropTag.Message.SearchRecipEmailTo,
				PropTag.Message.SearchRecipEmailCc,
				PropTag.Message.SearchRecipEmailBcc,
				PropTag.Message.SearchFullTextSubject,
				PropTag.Message.SearchFullTextBody,
				PropTag.Message.SearchAllIndexedProps,
				PropTag.Message.SearchRecipients,
				PropTag.Message.SearchRecipientsTo,
				PropTag.Message.SearchRecipientsCc,
				PropTag.Message.SearchRecipientsBcc,
				PropTag.Message.SearchAccountTo,
				PropTag.Message.SearchAccountCc,
				PropTag.Message.SearchAccountBcc,
				PropTag.Message.SearchEmailAddressTo,
				PropTag.Message.SearchEmailAddressCc,
				PropTag.Message.SearchEmailAddressBcc,
				PropTag.Message.SearchSmtpAddressTo,
				PropTag.Message.SearchSmtpAddressCc,
				PropTag.Message.SearchSmtpAddressBcc,
				PropTag.Message.SearchSender,
				PropTag.Message.SearchIsPartiallyIndexed,
				PropTag.Message.Access,
				PropTag.Message.RowType,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.AccessLevel,
				PropTag.Message.MappingSignature,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.StoreRecordKey,
				PropTag.Message.StoreEntryId,
				PropTag.Message.ObjectType,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.NativeBodyInfo,
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody,
				PropTag.Message.Depth,
				PropTag.Message.CreationTime,
				PropTag.Message.LastModificationTime,
				PropTag.Message.ConversationId,
				PropTag.Message.StoreSupportMask,
				PropTag.Message.MdbProvider,
				PropTag.Message.EventEmailReminderTimer,
				PropTag.Message.ContentCount,
				PropTag.Message.UnreadCount,
				PropTag.Message.UnreadCountInt64,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.PreviewUnread,
				PropTag.Message.CreatorName,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.LastModifierName,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.NewAttach,
				PropTag.Message.StartEmbed,
				PropTag.Message.EndEmbed,
				PropTag.Message.StartRecip,
				PropTag.Message.EndRecip,
				PropTag.Message.EndCcRecip,
				PropTag.Message.EndBccRecip,
				PropTag.Message.EndP1Recip,
				PropTag.Message.DNPrefix,
				PropTag.Message.StartTopFolder,
				PropTag.Message.StartSubFolder,
				PropTag.Message.EndFolder,
				PropTag.Message.StartMessage,
				PropTag.Message.EndMessage,
				PropTag.Message.EndAttach,
				PropTag.Message.EcWarning,
				PropTag.Message.StartFAIMessage,
				PropTag.Message.NewFXFolder,
				PropTag.Message.IncrSyncChange,
				PropTag.Message.IncrSyncDelete,
				PropTag.Message.IncrSyncEnd,
				PropTag.Message.IncrSyncMessage,
				PropTag.Message.FastTransferDelProp,
				PropTag.Message.IdsetGiven,
				PropTag.Message.IdsetGivenInt32,
				PropTag.Message.FastTransferErrorInfo,
				PropTag.Message.SoftDeletes,
				PropTag.Message.CreatorAddressType,
				PropTag.Message.CreatorEmailAddr,
				PropTag.Message.LastModifierAddressType,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.IdsetRead,
				PropTag.Message.IdsetUnread,
				PropTag.Message.IncrSyncRead,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.IncrSyncStateBegin,
				PropTag.Message.IncrSyncStateEnd,
				PropTag.Message.IncrSyncImailStream,
				PropTag.Message.CreatorOrgAddressType,
				PropTag.Message.CreatorOrgEmailAddr,
				PropTag.Message.LastModifierOrgAddressType,
				PropTag.Message.LastModifierOrgEmailAddr,
				PropTag.Message.CreatorFlags,
				PropTag.Message.LastModifierFlags,
				PropTag.Message.IncrSyncImailStreamContinue,
				PropTag.Message.IncrSyncImailStreamCancel,
				PropTag.Message.IncrSyncImailStream2Continue,
				PropTag.Message.IncrSyncProgressMode,
				PropTag.Message.SyncProgressPerMsg,
				PropTag.Message.IncrSyncMsgPartial,
				PropTag.Message.IncrSyncGroupInfo,
				PropTag.Message.IncrSyncGroupId,
				PropTag.Message.IncrSyncChangePartial,
				PropTag.Message.InternetMessageIdHash,
				PropTag.Message.ConversationTopicHash,
				PropTag.Message.SourceKey,
				PropTag.Message.ParentSourceKey,
				PropTag.Message.ChangeKey,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.ReplicaServer,
				PropTag.Message.HasNamedProperties,
				PropTag.Message.SearchAttachments,
				PropTag.Message.SubmittedByAdmin,
				PropTag.Message.LongTermEntryIdFromTable,
				PropTag.Message.DeletedOn,
				PropTag.Message.MailboxDSGuidGuid,
				PropTag.Message.URLName,
				PropTag.Message.LocalCommitTime,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.CategID,
				PropTag.Message.ParentCategID,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.ChangeType,
				PropTag.Message.LTID,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin,
				PropTag.Message.DocumentId,
				PropTag.Message.MailboxNum,
				PropTag.Message.ConversationIdHash,
				PropTag.Message.ConversationDocumentId
			};

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[]
			{
				PropTag.Message.MessageToMe,
				PropTag.Message.MessageCCMe,
				PropTag.Message.MessageRecipMe,
				PropTag.Message.DisplayBcc,
				PropTag.Message.DisplayCc,
				PropTag.Message.DisplayTo,
				PropTag.Message.ParentDisplay,
				PropTag.Message.MessageFlags,
				PropTag.Message.MessageSize,
				PropTag.Message.MessageSize32,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.SubmitFlags,
				PropTag.Message.HasAttach,
				PropTag.Message.FullTextConversationIndex,
				PropTag.Message.Access,
				PropTag.Message.RowType,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.AccessLevel,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.NativeBodyInfo,
				PropTag.Message.NNTPXRef,
				PropTag.Message.Depth,
				PropTag.Message.ConversationId,
				PropTag.Message.ContentCount,
				PropTag.Message.UnreadCount,
				PropTag.Message.GVid,
				PropTag.Message.GDID,
				PropTag.Message.XVid,
				PropTag.Message.GDefVid,
				PropTag.Message.PreviewUnread,
				PropTag.Message.HasDAMs,
				PropTag.Message.DeferredSendNumber,
				PropTag.Message.InternetMessageIdHash,
				PropTag.Message.ConversationTopicHash,
				PropTag.Message.SourceKey,
				PropTag.Message.ParentSourceKey,
				PropTag.Message.ChangeKey,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.LISSD,
				PropTag.Message.HasNamedProperties,
				PropTag.Message.DeletedOn,
				PropTag.Message.LocalCommitTime,
				PropTag.Message.CategID,
				PropTag.Message.ParentCategID,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.ChangeType,
				PropTag.Message.LTID,
				PropTag.Message.CnsetIn,
				PropTag.Message.ConversationIdHash
			};

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[]
			{
				PropTag.Message.BodyUnicode,
				PropTag.Message.RtfCompressed,
				PropTag.Message.BodyHtml,
				PropTag.Message.BodyHtmlUnicode
			};

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[]
			{
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI
			};

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[]
			{
				PropTag.Message.DisplayBcc,
				PropTag.Message.DisplayCc,
				PropTag.Message.DisplayTo,
				PropTag.Message.MessageSize,
				PropTag.Message.MessageSize32,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.HasAttach,
				PropTag.Message.Access,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.StoreRecordKey,
				PropTag.Message.StoreEntryId,
				PropTag.Message.ObjectType,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.InetNewsgroups,
				PropTag.Message.SMTPTempTblData,
				PropTag.Message.SMTPTempTblData2,
				PropTag.Message.SMTPTempTblData3,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.MimeSkeleton,
				PropTag.Message.ReplicaServer,
				PropTag.Message.DAMOriginalEntryId,
				PropTag.Message.HasNamedProperties,
				PropTag.Message.FidMid,
				PropTag.Message.InternetContent,
				PropTag.Message.OriginatorName,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.OriginatorAddressType,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.RecipientNumber,
				PropTag.Message.ReportDestinationName,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ProhibitReceiveQuota,
				PropTag.Message.SearchAttachments,
				PropTag.Message.ProhibitSendQuota,
				PropTag.Message.SubmittedByAdmin,
				PropTag.Message.LongTermEntryIdFromTable,
				PropTag.Message.RuleIds,
				PropTag.Message.RuleMsgConditionOld,
				PropTag.Message.RuleMsgActionsOld,
				PropTag.Message.DeletedOn,
				PropTag.Message.CodePageId,
				PropTag.Message.UserDN,
				PropTag.Message.MailboxDSGuidGuid,
				PropTag.Message.URLName,
				PropTag.Message.LocalCommitTime,
				PropTag.Message.AutoReset,
				PropTag.Message.ELCAutoCopyTag,
				PropTag.Message.ELCMoveDate,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.SentMailSvrEID,
				PropTag.Message.SentMailSvrEIDBin,
				PropTag.Message.LocallyDelivered,
				PropTag.Message.MimeSize,
				PropTag.Message.MimeSize32,
				PropTag.Message.FileSize,
				PropTag.Message.FileSize32,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.ParentFid,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.CategID,
				PropTag.Message.ParentCategID,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.ChangeType,
				PropTag.Message.RequiresRefResolve,
				PropTag.Message.LTID,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.ImapCachedBodystructure,
				PropTag.Message.StorageQuota,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.SourceEntryId,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SharedReceiptHandling,
				PropTag.Message.Inid,
				PropTag.Message.MessageAttachList,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin
			};

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[]
			{
				PropTag.Message.BodyUnicode,
				PropTag.Message.RtfCompressed,
				PropTag.Message.BodyHtml,
				PropTag.Message.BodyHtmlUnicode,
				PropTag.Message.NativeBodyInfo,
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody,
				PropTag.Message.PreviewUnread,
				PropTag.Message.Preview
			};

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[]
			{
				PropTag.Message.AcknowledgementMode,
				PropTag.Message.TestTest,
				PropTag.Message.AuthorizingUsers,
				PropTag.Message.AutoForwarded,
				PropTag.Message.ContentConfidentialityAlgorithmId,
				PropTag.Message.ContentCorrelator,
				PropTag.Message.ContentIdentifier,
				PropTag.Message.ContentLength,
				PropTag.Message.ContentReturnRequested,
				PropTag.Message.ConversationKey,
				PropTag.Message.ConversionEits,
				PropTag.Message.ConversionWithLossProhibited,
				PropTag.Message.ConvertedEits,
				PropTag.Message.DeferredDeliveryTime,
				PropTag.Message.DeliverTime,
				PropTag.Message.DiscardReason,
				PropTag.Message.DisclosureOfRecipients,
				PropTag.Message.DLExpansionHistory,
				PropTag.Message.DLExpansionProhibited,
				PropTag.Message.ExpiryTime,
				PropTag.Message.ImplicitConversionProhibited,
				PropTag.Message.Importance,
				PropTag.Message.ObsoletedIPMS,
				PropTag.Message.OriginallyIntendedRecipientName,
				PropTag.Message.OriginalEITS,
				PropTag.Message.OriginatorReturnAddress,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI
			};

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Message.AcknowledgementMode,
				PropTag.Message.TestTest,
				PropTag.Message.AlternateRecipientAllowed,
				PropTag.Message.AuthorizingUsers,
				PropTag.Message.AutoForwardComment,
				PropTag.Message.AutoForwarded,
				PropTag.Message.ContentConfidentialityAlgorithmId,
				PropTag.Message.ContentCorrelator,
				PropTag.Message.ContentIdentifier,
				PropTag.Message.ContentLength,
				PropTag.Message.ContentReturnRequested,
				PropTag.Message.ConversationKey,
				PropTag.Message.ConversionEits,
				PropTag.Message.ConversionWithLossProhibited,
				PropTag.Message.ConvertedEits,
				PropTag.Message.DeferredDeliveryTime,
				PropTag.Message.DeliverTime,
				PropTag.Message.DiscardReason,
				PropTag.Message.DisclosureOfRecipients,
				PropTag.Message.DLExpansionHistory,
				PropTag.Message.DLExpansionProhibited,
				PropTag.Message.ExpiryTime,
				PropTag.Message.ImplicitConversionProhibited,
				PropTag.Message.Importance,
				PropTag.Message.IPMID,
				PropTag.Message.LatestDeliveryTime,
				PropTag.Message.MessageClass,
				PropTag.Message.MessageDeliveryId,
				PropTag.Message.MessageSecurityLabel,
				PropTag.Message.ObsoletedIPMS,
				PropTag.Message.OriginallyIntendedRecipientName,
				PropTag.Message.OriginalEITS,
				PropTag.Message.OriginatorCertificate,
				PropTag.Message.DeliveryReportRequested,
				PropTag.Message.OriginatorReturnAddress,
				PropTag.Message.ParentKey,
				PropTag.Message.Priority,
				PropTag.Message.OriginCheck,
				PropTag.Message.ProofOfSubmissionRequested,
				PropTag.Message.ReadReceiptRequested,
				PropTag.Message.ReceiptTime,
				PropTag.Message.RecipientReassignmentProhibited,
				PropTag.Message.RedirectionHistory,
				PropTag.Message.RelatedIPMS,
				PropTag.Message.OriginalSensitivity,
				PropTag.Message.Languages,
				PropTag.Message.ReplyTime,
				PropTag.Message.ReportTag,
				PropTag.Message.ReportTime,
				PropTag.Message.ReturnedIPM,
				PropTag.Message.Security,
				PropTag.Message.IncompleteCopy,
				PropTag.Message.Sensitivity,
				PropTag.Message.Subject,
				PropTag.Message.SubjectIPM,
				PropTag.Message.ClientSubmitTime,
				PropTag.Message.ReportName,
				PropTag.Message.SentRepresentingSearchKey,
				PropTag.Message.X400ContentType,
				PropTag.Message.SubjectPrefix,
				PropTag.Message.NonReceiptReason,
				PropTag.Message.ReceivedByEntryId,
				PropTag.Message.ReceivedByName,
				PropTag.Message.SentRepresentingEntryId,
				PropTag.Message.SentRepresentingName,
				PropTag.Message.ReceivedRepresentingEntryId,
				PropTag.Message.ReceivedRepresentingName,
				PropTag.Message.ReportEntryId,
				PropTag.Message.ReadReceiptEntryId,
				PropTag.Message.MessageSubmissionId,
				PropTag.Message.ProviderSubmitTime,
				PropTag.Message.OriginalSubject,
				PropTag.Message.DiscVal,
				PropTag.Message.OriginalMessageClass,
				PropTag.Message.OriginalAuthorEntryId,
				PropTag.Message.OriginalAuthorName,
				PropTag.Message.OriginalSubmitTime,
				PropTag.Message.ReplyRecipientEntries,
				PropTag.Message.ReplyRecipientNames,
				PropTag.Message.ReceivedBySearchKey,
				PropTag.Message.ReceivedRepresentingSearchKey,
				PropTag.Message.ReadReceiptSearchKey,
				PropTag.Message.ReportSearchKey,
				PropTag.Message.OriginalDeliveryTime,
				PropTag.Message.OriginalAuthorSearchKey,
				PropTag.Message.MessageToMe,
				PropTag.Message.MessageCCMe,
				PropTag.Message.MessageRecipMe,
				PropTag.Message.OriginalSenderName,
				PropTag.Message.OriginalSenderEntryId,
				PropTag.Message.OriginalSenderSearchKey,
				PropTag.Message.OriginalSentRepresentingName,
				PropTag.Message.OriginalSentRepresentingEntryId,
				PropTag.Message.OriginalSentRepresentingSearchKey,
				PropTag.Message.StartDate,
				PropTag.Message.EndDate,
				PropTag.Message.OwnerApptId,
				PropTag.Message.ResponseRequested,
				PropTag.Message.SentRepresentingAddressType,
				PropTag.Message.SentRepresentingEmailAddress,
				PropTag.Message.OriginalSenderAddressType,
				PropTag.Message.OriginalSenderEmailAddress,
				PropTag.Message.OriginalSentRepresentingAddressType,
				PropTag.Message.OriginalSentRepresentingEmailAddress,
				PropTag.Message.ConversationTopic,
				PropTag.Message.ConversationIndex,
				PropTag.Message.OriginalDisplayBcc,
				PropTag.Message.OriginalDisplayCc,
				PropTag.Message.OriginalDisplayTo,
				PropTag.Message.ReceivedByAddressType,
				PropTag.Message.ReceivedByEmailAddress,
				PropTag.Message.ReceivedRepresentingAddressType,
				PropTag.Message.ReceivedRepresentingEmailAddress,
				PropTag.Message.OriginalAuthorAddressType,
				PropTag.Message.OriginalAuthorEmailAddress,
				PropTag.Message.OriginallyIntendedRecipientAddressType,
				PropTag.Message.TransportMessageHeaders,
				PropTag.Message.Delegation,
				PropTag.Message.ReportDisposition,
				PropTag.Message.ReportDispositionMode,
				PropTag.Message.ReportOriginalSender,
				PropTag.Message.ReportDispositionToNames,
				PropTag.Message.ReportDispositionToEmailAddress,
				PropTag.Message.ReportDispositionOptions,
				PropTag.Message.RichContent,
				PropTag.Message.AdministratorEMail,
				PropTag.Message.ContentIntegrityCheck,
				PropTag.Message.ExplicitConversion,
				PropTag.Message.ReturnRequested,
				PropTag.Message.MessageToken,
				PropTag.Message.NDRReasonCode,
				PropTag.Message.NDRDiagCode,
				PropTag.Message.NonReceiptNotificationRequested,
				PropTag.Message.DeliveryPoint,
				PropTag.Message.NonDeliveryReportRequested,
				PropTag.Message.OriginatorRequestedAlterateRecipient,
				PropTag.Message.PhysicalDeliveryBureauFaxDelivery,
				PropTag.Message.PhysicalDeliveryMode,
				PropTag.Message.PhysicalDeliveryReportRequest,
				PropTag.Message.PhysicalForwardingAddress,
				PropTag.Message.PhysicalForwardingAddressRequested,
				PropTag.Message.PhysicalForwardingProhibited,
				PropTag.Message.ProofOfDelivery,
				PropTag.Message.ProofOfDeliveryRequested,
				PropTag.Message.RecipientCertificate,
				PropTag.Message.RecipientNumberForAdvice,
				PropTag.Message.RecipientType,
				PropTag.Message.RegisteredMailType,
				PropTag.Message.ReplyRequested,
				PropTag.Message.RequestedDeliveryMethod,
				PropTag.Message.SenderEntryId,
				PropTag.Message.SenderName,
				PropTag.Message.SupplementaryInfo,
				PropTag.Message.TypeOfMTSUser,
				PropTag.Message.SenderSearchKey,
				PropTag.Message.SenderAddressType,
				PropTag.Message.SenderEmailAddress,
				PropTag.Message.ParticipantSID,
				PropTag.Message.ParticipantGuid,
				PropTag.Message.ToGroupExpansionRecipients,
				PropTag.Message.CcGroupExpansionRecipients,
				PropTag.Message.BccGroupExpansionRecipients,
				PropTag.Message.CurrentVersion,
				PropTag.Message.DeleteAfterSubmit,
				PropTag.Message.DisplayBcc,
				PropTag.Message.DisplayCc,
				PropTag.Message.DisplayTo,
				PropTag.Message.ParentDisplay,
				PropTag.Message.MessageDeliveryTime,
				PropTag.Message.MessageFlags,
				PropTag.Message.MessageSize,
				PropTag.Message.MessageSize32,
				PropTag.Message.ParentEntryId,
				PropTag.Message.ParentEntryIdSvrEid,
				PropTag.Message.SentMailEntryId,
				PropTag.Message.Correlate,
				PropTag.Message.CorrelateMTSID,
				PropTag.Message.DiscreteValues,
				PropTag.Message.Responsibility,
				PropTag.Message.SpoolerStatus,
				PropTag.Message.TransportStatus,
				PropTag.Message.MessageRecipients,
				PropTag.Message.MessageRecipientsMVBin,
				PropTag.Message.MessageAttachments,
				PropTag.Message.ItemSubobjectsBin,
				PropTag.Message.SubmitFlags,
				PropTag.Message.RecipientStatus,
				PropTag.Message.TransportKey,
				PropTag.Message.MsgStatus,
				PropTag.Message.CreationVersion,
				PropTag.Message.ModifyVersion,
				PropTag.Message.HasAttach,
				PropTag.Message.BodyCRC,
				PropTag.Message.NormalizedSubject,
				PropTag.Message.RTFInSync,
				PropTag.Message.Preprocess,
				PropTag.Message.InternetArticleNumber,
				PropTag.Message.OriginatingMTACertificate,
				PropTag.Message.ProofOfSubmission,
				PropTag.Message.NTSecurityDescriptor,
				PropTag.Message.PrimarySendAccount,
				PropTag.Message.NextSendAccount,
				PropTag.Message.TodoItemFlags,
				PropTag.Message.SwappedTODOStore,
				PropTag.Message.SwappedTODOData,
				PropTag.Message.IMAPId,
				PropTag.Message.OriginalSourceServerVersion,
				PropTag.Message.ReplFlags,
				PropTag.Message.MessageDeepAttachments,
				PropTag.Message.SenderGuid,
				PropTag.Message.SentRepresentingGuid,
				PropTag.Message.OriginalSenderGuid,
				PropTag.Message.OriginalSentRepresentingGuid,
				PropTag.Message.ReadReceiptGuid,
				PropTag.Message.ReportGuid,
				PropTag.Message.OriginatorGuid,
				PropTag.Message.ReportDestinationGuid,
				PropTag.Message.OriginalAuthorGuid,
				PropTag.Message.ReceivedByGuid,
				PropTag.Message.ReceivedRepresentingGuid,
				PropTag.Message.CreatorGuid,
				PropTag.Message.LastModifierGuid,
				PropTag.Message.SenderSID,
				PropTag.Message.SentRepresentingSID,
				PropTag.Message.OriginalSenderSid,
				PropTag.Message.OriginalSentRepresentingSid,
				PropTag.Message.ReadReceiptSid,
				PropTag.Message.ReportSid,
				PropTag.Message.OriginatorSid,
				PropTag.Message.ReportDestinationSid,
				PropTag.Message.OriginalAuthorSid,
				PropTag.Message.RcvdBySid,
				PropTag.Message.RcvdRepresentingSid,
				PropTag.Message.CreatorSID,
				PropTag.Message.LastModifierSid,
				PropTag.Message.RecipientCAI,
				PropTag.Message.ConversationCreatorSID,
				PropTag.Message.URLCompNamePostfix,
				PropTag.Message.URLCompNameSet,
				PropTag.Message.Read,
				PropTag.Message.CreatorSidAsXML,
				PropTag.Message.LastModifierSidAsXML,
				PropTag.Message.SenderSIDAsXML,
				PropTag.Message.SentRepresentingSidAsXML,
				PropTag.Message.OriginalSenderSIDAsXML,
				PropTag.Message.OriginalSentRepresentingSIDAsXML,
				PropTag.Message.ReadReceiptSIDAsXML,
				PropTag.Message.ReportSIDAsXML,
				PropTag.Message.OriginatorSidAsXML,
				PropTag.Message.ReportDestinationSIDAsXML,
				PropTag.Message.OriginalAuthorSIDAsXML,
				PropTag.Message.ReceivedBySIDAsXML,
				PropTag.Message.ReceivedRepersentingSIDAsXML,
				PropTag.Message.TrustSender,
				PropTag.Message.SenderSMTPAddress,
				PropTag.Message.SentRepresentingSMTPAddress,
				PropTag.Message.OriginalSenderSMTPAddress,
				PropTag.Message.OriginalSentRepresentingSMTPAddress,
				PropTag.Message.ReadReceiptSMTPAddress,
				PropTag.Message.ReportSMTPAddress,
				PropTag.Message.OriginatorSMTPAddress,
				PropTag.Message.ReportDestinationSMTPAddress,
				PropTag.Message.OriginalAuthorSMTPAddress,
				PropTag.Message.ReceivedBySMTPAddress,
				PropTag.Message.ReceivedRepresentingSMTPAddress,
				PropTag.Message.CreatorSMTPAddress,
				PropTag.Message.LastModifierSMTPAddress,
				PropTag.Message.VirusScannerStamp,
				PropTag.Message.VirusTransportStamp,
				PropTag.Message.AddrTo,
				PropTag.Message.AddrCc,
				PropTag.Message.ExtendedRuleActions,
				PropTag.Message.ExtendedRuleCondition,
				PropTag.Message.EntourageSentHistory,
				PropTag.Message.ProofInProgress,
				PropTag.Message.SearchAttachmentsOLK,
				PropTag.Message.SearchRecipEmailTo,
				PropTag.Message.SearchRecipEmailCc,
				PropTag.Message.SearchRecipEmailBcc,
				PropTag.Message.SFGAOFlags,
				PropTag.Message.SearchFullTextSubject,
				PropTag.Message.SearchFullTextBody,
				PropTag.Message.FullTextConversationIndex,
				PropTag.Message.SearchAllIndexedProps,
				PropTag.Message.SearchRecipients,
				PropTag.Message.SearchRecipientsTo,
				PropTag.Message.SearchRecipientsCc,
				PropTag.Message.SearchRecipientsBcc,
				PropTag.Message.SearchAccountTo,
				PropTag.Message.SearchAccountCc,
				PropTag.Message.SearchAccountBcc,
				PropTag.Message.SearchEmailAddressTo,
				PropTag.Message.SearchEmailAddressCc,
				PropTag.Message.SearchEmailAddressBcc,
				PropTag.Message.SearchSmtpAddressTo,
				PropTag.Message.SearchSmtpAddressCc,
				PropTag.Message.SearchSmtpAddressBcc,
				PropTag.Message.SearchSender,
				PropTag.Message.IsIRMMessage,
				PropTag.Message.SearchIsPartiallyIndexed,
				PropTag.Message.RenewTime,
				PropTag.Message.DeliveryOrRenewTime,
				PropTag.Message.ConversationFamilyId,
				PropTag.Message.LikeCount,
				PropTag.Message.RichContentDeprecated,
				PropTag.Message.PeopleCentricConversationId,
				PropTag.Message.DiscoveryAnnotation,
				PropTag.Message.Access,
				PropTag.Message.RowType,
				PropTag.Message.InstanceKey,
				PropTag.Message.InstanceKeySvrEid,
				PropTag.Message.AccessLevel,
				PropTag.Message.MappingSignature,
				PropTag.Message.RecordKey,
				PropTag.Message.RecordKeySvrEid,
				PropTag.Message.StoreRecordKey,
				PropTag.Message.StoreEntryId,
				PropTag.Message.MiniIcon,
				PropTag.Message.Icon,
				PropTag.Message.ObjectType,
				PropTag.Message.EntryId,
				PropTag.Message.EntryIdSvrEid,
				PropTag.Message.BodyUnicode,
				PropTag.Message.ReportText,
				PropTag.Message.OriginatorAndDLExpansionHistory,
				PropTag.Message.ReportingDLName,
				PropTag.Message.ReportingMTACertificate,
				PropTag.Message.RtfSyncBodyCrc,
				PropTag.Message.RtfSyncBodyCount,
				PropTag.Message.RtfSyncBodyTag,
				PropTag.Message.RtfCompressed,
				PropTag.Message.AlternateBestBody,
				PropTag.Message.RtfSyncPrefixCount,
				PropTag.Message.RtfSyncTrailingCount,
				PropTag.Message.OriginallyIntendedRecipientEntryId,
				PropTag.Message.BodyHtml,
				PropTag.Message.BodyHtmlUnicode,
				PropTag.Message.BodyContentLocation,
				PropTag.Message.BodyContentId,
				PropTag.Message.NativeBodyInfo,
				PropTag.Message.NativeBodyType,
				PropTag.Message.NativeBody,
				PropTag.Message.AnnotationToken,
				PropTag.Message.InternetApproved,
				PropTag.Message.InternetFollowupTo,
				PropTag.Message.InternetMessageId,
				PropTag.Message.InetNewsgroups,
				PropTag.Message.InternetReferences,
				PropTag.Message.PostReplyFolderEntries,
				PropTag.Message.NNTPXRef,
				PropTag.Message.InReplyToId,
				PropTag.Message.OriginalInternetMessageId,
				PropTag.Message.IconIndex,
				PropTag.Message.LastVerbExecuted,
				PropTag.Message.LastVerbExecutionTime,
				PropTag.Message.Relevance,
				PropTag.Message.FlagStatus,
				PropTag.Message.FlagCompleteTime,
				PropTag.Message.FormatPT,
				PropTag.Message.FollowupIcon,
				PropTag.Message.BlockStatus,
				PropTag.Message.ItemTempFlags,
				PropTag.Message.SMTPTempTblData,
				PropTag.Message.SMTPTempTblData2,
				PropTag.Message.SMTPTempTblData3,
				PropTag.Message.DAVSubmitData,
				PropTag.Message.ImapCachedMsgSize,
				PropTag.Message.DisableFullFidelity,
				PropTag.Message.URLCompName,
				PropTag.Message.AttrHidden,
				PropTag.Message.AttrSystem,
				PropTag.Message.AttrReadOnly,
				PropTag.Message.PredictedActions,
				PropTag.Message.GroupingActions,
				PropTag.Message.PredictedActionsSummary,
				PropTag.Message.PredictedActionsThresholds,
				PropTag.Message.IsClutter,
				PropTag.Message.InferencePredictedReplyForwardReasons,
				PropTag.Message.InferencePredictedDeleteReasons,
				PropTag.Message.InferencePredictedIgnoreReasons,
				PropTag.Message.OriginalDeliveryFolderInfo,
				PropTag.Message.RowId,
				PropTag.Message.DisplayName,
				PropTag.Message.AddressType,
				PropTag.Message.EmailAddress,
				PropTag.Message.Comment,
				PropTag.Message.Depth,
				PropTag.Message.CreationTime,
				PropTag.Message.LastModificationTime,
				PropTag.Message.SearchKey,
				PropTag.Message.SearchKeySvrEid,
				PropTag.Message.TargetEntryId,
				PropTag.Message.ConversationId,
				PropTag.Message.BodyTag,
				PropTag.Message.ConversationIndexTrackingObsolete,
				PropTag.Message.ConversationIndexTracking,
				PropTag.Message.ArchiveTag,
				PropTag.Message.PolicyTag,
				PropTag.Message.RetentionPeriod,
				PropTag.Message.StartDateEtc,
				PropTag.Message.RetentionDate,
				PropTag.Message.RetentionFlags,
				PropTag.Message.ArchivePeriod,
				PropTag.Message.ArchiveDate,
				PropTag.Message.FormVersion,
				PropTag.Message.FormCLSID,
				PropTag.Message.FormContactName,
				PropTag.Message.FormCategory,
				PropTag.Message.FormCategorySub,
				PropTag.Message.FormHidden,
				PropTag.Message.FormDesignerName,
				PropTag.Message.FormDesignerGuid,
				PropTag.Message.FormMessageBehavior,
				PropTag.Message.StoreSupportMask,
				PropTag.Message.MdbProvider,
				PropTag.Message.EventEmailReminderTimer,
				PropTag.Message.ContentCount,
				PropTag.Message.UnreadCount,
				PropTag.Message.UnreadCountInt64,
				PropTag.Message.DetailsTable,
				PropTag.Message.AppointmentColorName,
				PropTag.Message.ContentId,
				PropTag.Message.MimeUrl,
				PropTag.Message.DisplayType,
				PropTag.Message.SmtpAddress,
				PropTag.Message.SimpleDisplayName,
				PropTag.Message.Account,
				PropTag.Message.AlternateRecipient,
				PropTag.Message.CallbackTelephoneNumber,
				PropTag.Message.ConversionProhibited,
				PropTag.Message.Generation,
				PropTag.Message.GivenName,
				PropTag.Message.GovernmentIDNumber,
				PropTag.Message.BusinessTelephoneNumber,
				PropTag.Message.HomeTelephoneNumber,
				PropTag.Message.Initials,
				PropTag.Message.Keyword,
				PropTag.Message.Language,
				PropTag.Message.Location,
				PropTag.Message.MailPermission,
				PropTag.Message.MHSCommonName,
				PropTag.Message.OrganizationalIDNumber,
				PropTag.Message.SurName,
				PropTag.Message.OriginalEntryId,
				PropTag.Message.OriginalDisplayName,
				PropTag.Message.OriginalSearchKey,
				PropTag.Message.PostalAddress,
				PropTag.Message.CompanyName,
				PropTag.Message.Title,
				PropTag.Message.DepartmentName,
				PropTag.Message.OfficeLocation,
				PropTag.Message.PrimaryTelephoneNumber,
				PropTag.Message.Business2TelephoneNumber,
				PropTag.Message.Business2TelephoneNumberMv,
				PropTag.Message.MobileTelephoneNumber,
				PropTag.Message.RadioTelephoneNumber,
				PropTag.Message.CarTelephoneNumber,
				PropTag.Message.OtherTelephoneNumber,
				PropTag.Message.TransmitableDisplayName,
				PropTag.Message.PagerTelephoneNumber,
				PropTag.Message.UserCertificate,
				PropTag.Message.PrimaryFaxNumber,
				PropTag.Message.BusinessFaxNumber,
				PropTag.Message.HomeFaxNumber,
				PropTag.Message.Country,
				PropTag.Message.Locality,
				PropTag.Message.StateOrProvince,
				PropTag.Message.StreetAddress,
				PropTag.Message.PostalCode,
				PropTag.Message.PostOfficeBox,
				PropTag.Message.TelexNumber,
				PropTag.Message.ISDNNumber,
				PropTag.Message.AssistantTelephoneNumber,
				PropTag.Message.Home2TelephoneNumber,
				PropTag.Message.Home2TelephoneNumberMv,
				PropTag.Message.Assistant,
				PropTag.Message.SendRichInfo,
				PropTag.Message.WeddingAnniversary,
				PropTag.Message.Birthday,
				PropTag.Message.Hobbies,
				PropTag.Message.MiddleName,
				PropTag.Message.DisplayNamePrefix,
				PropTag.Message.Profession,
				PropTag.Message.ReferredByName,
				PropTag.Message.SpouseName,
				PropTag.Message.ComputerNetworkName,
				PropTag.Message.CustomerId,
				PropTag.Message.TTYTDDPhoneNumber,
				PropTag.Message.FTPSite,
				PropTag.Message.Gender,
				PropTag.Message.ManagerName,
				PropTag.Message.NickName,
				PropTag.Message.PersonalHomePage,
				PropTag.Message.BusinessHomePage,
				PropTag.Message.ContactVersion,
				PropTag.Message.ContactEntryIds,
				PropTag.Message.ContactAddressTypes,
				PropTag.Message.ContactDefaultAddressIndex,
				PropTag.Message.ContactEmailAddress,
				PropTag.Message.CompanyMainPhoneNumber,
				PropTag.Message.ChildrensNames,
				PropTag.Message.HomeAddressCity,
				PropTag.Message.HomeAddressCountry,
				PropTag.Message.HomeAddressPostalCode,
				PropTag.Message.HomeAddressStateOrProvince,
				PropTag.Message.HomeAddressStreet,
				PropTag.Message.HomeAddressPostOfficeBox,
				PropTag.Message.OtherAddressCity,
				PropTag.Message.OtherAddressCountry,
				PropTag.Message.OtherAddressPostalCode,
				PropTag.Message.OtherAddressStateOrProvince,
				PropTag.Message.OtherAddressStreet,
				PropTag.Message.OtherAddressPostOfficeBox,
				PropTag.Message.UserX509CertificateABSearchPath,
				PropTag.Message.SendInternetEncoding,
				PropTag.Message.PartnerNetworkId,
				PropTag.Message.PartnerNetworkUserId,
				PropTag.Message.PartnerNetworkThumbnailPhotoUrl,
				PropTag.Message.PartnerNetworkProfilePhotoUrl,
				PropTag.Message.PartnerNetworkContactType,
				PropTag.Message.RelevanceScore,
				PropTag.Message.IsDistributionListContact,
				PropTag.Message.IsPromotedContact,
				PropTag.Message.OrgUnitName,
				PropTag.Message.OrganizationName,
				PropTag.Message.TestBlobProperty,
				PropTag.Message.FilteringHooks,
				PropTag.Message.MailboxPartitionNumber,
				PropTag.Message.MailboxNumberInternal,
				PropTag.Message.VirtualParentDisplay,
				PropTag.Message.InternalConversationIndexTracking,
				PropTag.Message.InternalConversationIndex,
				PropTag.Message.ConversationItemConversationId,
				PropTag.Message.VirtualUnreadMessageCount,
				PropTag.Message.VirtualIsRead,
				PropTag.Message.IsReadColumn,
				PropTag.Message.Internal9ByteChangeNumber,
				PropTag.Message.Internal9ByteReadCnNew,
				PropTag.Message.CategoryHeaderLevelStub1,
				PropTag.Message.CategoryHeaderLevelStub2,
				PropTag.Message.CategoryHeaderLevelStub3,
				PropTag.Message.CategoryHeaderAggregateProp0,
				PropTag.Message.CategoryHeaderAggregateProp1,
				PropTag.Message.CategoryHeaderAggregateProp2,
				PropTag.Message.CategoryHeaderAggregateProp3,
				PropTag.Message.MessageFlagsActual,
				PropTag.Message.InternalChangeKey,
				PropTag.Message.InternalSourceKey,
				PropTag.Message.HeaderFolderEntryId,
				PropTag.Message.RemoteProgress,
				PropTag.Message.RemoteProgressText,
				PropTag.Message.VID,
				PropTag.Message.GVid,
				PropTag.Message.GDID,
				PropTag.Message.XVid,
				PropTag.Message.GDefVid,
				PropTag.Message.PrimaryMailboxOverQuota,
				PropTag.Message.InternalPostReply,
				PropTag.Message.PreviewUnread,
				PropTag.Message.Preview,
				PropTag.Message.InternetCPID,
				PropTag.Message.AutoResponseSuppress,
				PropTag.Message.HasDAMs,
				PropTag.Message.DeferredSendNumber,
				PropTag.Message.DeferredSendUnits,
				PropTag.Message.ExpiryNumber,
				PropTag.Message.ExpiryUnits,
				PropTag.Message.DeferredSendTime,
				PropTag.Message.MessageLocaleId,
				PropTag.Message.RuleTriggerHistory,
				PropTag.Message.MoveToStoreEid,
				PropTag.Message.MoveToFolderEid,
				PropTag.Message.StorageQuotaLimit,
				PropTag.Message.ExcessStorageUsed,
				PropTag.Message.ServerGeneratingQuotaMsg,
				PropTag.Message.CreatorName,
				PropTag.Message.CreatorEntryId,
				PropTag.Message.LastModifierName,
				PropTag.Message.LastModifierEntryId,
				PropTag.Message.MessageCodePage,
				PropTag.Message.QuotaType,
				PropTag.Message.IsPublicFolderQuotaMessage,
				PropTag.Message.NewAttach,
				PropTag.Message.StartEmbed,
				PropTag.Message.EndEmbed,
				PropTag.Message.StartRecip,
				PropTag.Message.EndRecip,
				PropTag.Message.EndCcRecip,
				PropTag.Message.EndBccRecip,
				PropTag.Message.EndP1Recip,
				PropTag.Message.DNPrefix,
				PropTag.Message.StartTopFolder,
				PropTag.Message.StartSubFolder,
				PropTag.Message.EndFolder,
				PropTag.Message.StartMessage,
				PropTag.Message.EndMessage,
				PropTag.Message.EndAttach,
				PropTag.Message.EcWarning,
				PropTag.Message.StartFAIMessage,
				PropTag.Message.NewFXFolder,
				PropTag.Message.IncrSyncChange,
				PropTag.Message.IncrSyncDelete,
				PropTag.Message.IncrSyncEnd,
				PropTag.Message.IncrSyncMessage,
				PropTag.Message.FastTransferDelProp,
				PropTag.Message.IdsetGiven,
				PropTag.Message.IdsetGivenInt32,
				PropTag.Message.FastTransferErrorInfo,
				PropTag.Message.SenderFlags,
				PropTag.Message.SentRepresentingFlags,
				PropTag.Message.RcvdByFlags,
				PropTag.Message.RcvdRepresentingFlags,
				PropTag.Message.OriginalSenderFlags,
				PropTag.Message.OriginalSentRepresentingFlags,
				PropTag.Message.ReportFlags,
				PropTag.Message.ReadReceiptFlags,
				PropTag.Message.SoftDeletes,
				PropTag.Message.CreatorAddressType,
				PropTag.Message.CreatorEmailAddr,
				PropTag.Message.LastModifierAddressType,
				PropTag.Message.LastModifierEmailAddr,
				PropTag.Message.ReportAddressType,
				PropTag.Message.ReportEmailAddress,
				PropTag.Message.ReportDisplayName,
				PropTag.Message.ReadReceiptAddressType,
				PropTag.Message.ReadReceiptEmailAddress,
				PropTag.Message.ReadReceiptDisplayName,
				PropTag.Message.IdsetRead,
				PropTag.Message.IdsetUnread,
				PropTag.Message.IncrSyncRead,
				PropTag.Message.SenderSimpleDisplayName,
				PropTag.Message.SentRepresentingSimpleDisplayName,
				PropTag.Message.OriginalSenderSimpleDisplayName,
				PropTag.Message.OriginalSentRepresentingSimpleDisplayName,
				PropTag.Message.ReceivedBySimpleDisplayName,
				PropTag.Message.ReceivedRepresentingSimpleDisplayName,
				PropTag.Message.ReadReceiptSimpleDisplayName,
				PropTag.Message.ReportSimpleDisplayName,
				PropTag.Message.CreatorSimpleDisplayName,
				PropTag.Message.LastModifierSimpleDisplayName,
				PropTag.Message.IncrSyncStateBegin,
				PropTag.Message.IncrSyncStateEnd,
				PropTag.Message.IncrSyncImailStream,
				PropTag.Message.SenderOrgAddressType,
				PropTag.Message.SenderOrgEmailAddr,
				PropTag.Message.SentRepresentingOrgAddressType,
				PropTag.Message.SentRepresentingOrgEmailAddr,
				PropTag.Message.OriginalSenderOrgAddressType,
				PropTag.Message.OriginalSenderOrgEmailAddr,
				PropTag.Message.OriginalSentRepresentingOrgAddressType,
				PropTag.Message.OriginalSentRepresentingOrgEmailAddr,
				PropTag.Message.RcvdByOrgAddressType,
				PropTag.Message.RcvdByOrgEmailAddr,
				PropTag.Message.RcvdRepresentingOrgAddressType,
				PropTag.Message.RcvdRepresentingOrgEmailAddr,
				PropTag.Message.ReadReceiptOrgAddressType,
				PropTag.Message.ReadReceiptOrgEmailAddr,
				PropTag.Message.ReportOrgAddressType,
				PropTag.Message.ReportOrgEmailAddr,
				PropTag.Message.CreatorOrgAddressType,
				PropTag.Message.CreatorOrgEmailAddr,
				PropTag.Message.LastModifierOrgAddressType,
				PropTag.Message.LastModifierOrgEmailAddr,
				PropTag.Message.OriginatorOrgAddressType,
				PropTag.Message.OriginatorOrgEmailAddr,
				PropTag.Message.ReportDestinationOrgEmailType,
				PropTag.Message.ReportDestinationOrgEmailAddr,
				PropTag.Message.OriginalAuthorOrgAddressType,
				PropTag.Message.OriginalAuthorOrgEmailAddr,
				PropTag.Message.CreatorFlags,
				PropTag.Message.LastModifierFlags,
				PropTag.Message.OriginatorFlags,
				PropTag.Message.ReportDestinationFlags,
				PropTag.Message.OriginalAuthorFlags,
				PropTag.Message.OriginatorSimpleDisplayName,
				PropTag.Message.ReportDestinationSimpleDisplayName,
				PropTag.Message.OriginalAuthorSimpleDispName,
				PropTag.Message.OriginatorSearchKey,
				PropTag.Message.ReportDestinationAddressType,
				PropTag.Message.ReportDestinationEmailAddress,
				PropTag.Message.ReportDestinationSearchKey,
				PropTag.Message.IncrSyncImailStreamContinue,
				PropTag.Message.IncrSyncImailStreamCancel,
				PropTag.Message.IncrSyncImailStream2Continue,
				PropTag.Message.IncrSyncProgressMode,
				PropTag.Message.SyncProgressPerMsg,
				PropTag.Message.ContentFilterSCL,
				PropTag.Message.IncrSyncMsgPartial,
				PropTag.Message.IncrSyncGroupInfo,
				PropTag.Message.IncrSyncGroupId,
				PropTag.Message.IncrSyncChangePartial,
				PropTag.Message.ContentFilterPCL,
				PropTag.Message.DeliverAsRead,
				PropTag.Message.InetMailOverrideFormat,
				PropTag.Message.MessageEditorFormat,
				PropTag.Message.SenderSMTPAddressXSO,
				PropTag.Message.SentRepresentingSMTPAddressXSO,
				PropTag.Message.OriginalSenderSMTPAddressXSO,
				PropTag.Message.OriginalSentRepresentingSMTPAddressXSO,
				PropTag.Message.ReadReceiptSMTPAddressXSO,
				PropTag.Message.OriginalAuthorSMTPAddressXSO,
				PropTag.Message.ReceivedBySMTPAddressXSO,
				PropTag.Message.ReceivedRepresentingSMTPAddressXSO,
				PropTag.Message.RecipientOrder,
				PropTag.Message.RecipientSipUri,
				PropTag.Message.RecipientDisplayName,
				PropTag.Message.RecipientEntryId,
				PropTag.Message.RecipientFlags,
				PropTag.Message.RecipientTrackStatus,
				PropTag.Message.DotStuffState,
				PropTag.Message.InternetMessageIdHash,
				PropTag.Message.ConversationTopicHash,
				PropTag.Message.MimeSkeleton,
				PropTag.Message.ReplyTemplateId,
				PropTag.Message.SecureSubmitFlags,
				PropTag.Message.SourceKey,
				PropTag.Message.ParentSourceKey,
				PropTag.Message.ChangeKey,
				PropTag.Message.PredecessorChangeList,
				PropTag.Message.RuleMsgState,
				PropTag.Message.RuleMsgUserFlags,
				PropTag.Message.RuleMsgProvider,
				PropTag.Message.RuleMsgName,
				PropTag.Message.RuleMsgLevel,
				PropTag.Message.RuleMsgProviderData,
				PropTag.Message.RuleMsgActions,
				PropTag.Message.RuleMsgCondition,
				PropTag.Message.RuleMsgConditionLCID,
				PropTag.Message.RuleMsgVersion,
				PropTag.Message.RuleMsgSequence,
				PropTag.Message.LISSD,
				PropTag.Message.ReplicaServer,
				PropTag.Message.DAMOriginalEntryId,
				PropTag.Message.HasNamedProperties,
				PropTag.Message.FidMid,
				PropTag.Message.InternetContent,
				PropTag.Message.OriginatorName,
				PropTag.Message.OriginatorEmailAddress,
				PropTag.Message.OriginatorAddressType,
				PropTag.Message.OriginatorEntryId,
				PropTag.Message.RecipientNumber,
				PropTag.Message.ReportDestinationName,
				PropTag.Message.ReportDestinationEntryId,
				PropTag.Message.ProhibitReceiveQuota,
				PropTag.Message.SearchAttachments,
				PropTag.Message.ProhibitSendQuota,
				PropTag.Message.SubmittedByAdmin,
				PropTag.Message.LongTermEntryIdFromTable,
				PropTag.Message.RuleIds,
				PropTag.Message.RuleMsgConditionOld,
				PropTag.Message.RuleMsgActionsOld,
				PropTag.Message.DeletedOn,
				PropTag.Message.CodePageId,
				PropTag.Message.UserDN,
				PropTag.Message.MailboxDSGuidGuid,
				PropTag.Message.URLName,
				PropTag.Message.LocalCommitTime,
				PropTag.Message.AutoReset,
				PropTag.Message.ELCAutoCopyTag,
				PropTag.Message.ELCMoveDate,
				PropTag.Message.PropGroupInfo,
				PropTag.Message.PropertyGroupChangeMask,
				PropTag.Message.ReadCnNewExport,
				PropTag.Message.SentMailSvrEID,
				PropTag.Message.SentMailSvrEIDBin,
				PropTag.Message.LocallyDelivered,
				PropTag.Message.MimeSize,
				PropTag.Message.MimeSize32,
				PropTag.Message.FileSize,
				PropTag.Message.FileSize32,
				PropTag.Message.Fid,
				PropTag.Message.FidBin,
				PropTag.Message.ParentFid,
				PropTag.Message.Mid,
				PropTag.Message.MidBin,
				PropTag.Message.CategID,
				PropTag.Message.ParentCategID,
				PropTag.Message.InstanceId,
				PropTag.Message.InstanceNum,
				PropTag.Message.ChangeType,
				PropTag.Message.RequiresRefResolve,
				PropTag.Message.LTID,
				PropTag.Message.CnExport,
				PropTag.Message.PclExport,
				PropTag.Message.CnMvExport,
				PropTag.Message.MailboxGuid,
				PropTag.Message.MapiEntryIdGuidGuid,
				PropTag.Message.ImapCachedBodystructure,
				PropTag.Message.StorageQuota,
				PropTag.Message.CnsetIn,
				PropTag.Message.CnsetSeen,
				PropTag.Message.ChangeNumber,
				PropTag.Message.ChangeNumberBin,
				PropTag.Message.PCL,
				PropTag.Message.CnMv,
				PropTag.Message.SourceEntryId,
				PropTag.Message.MailFlags,
				PropTag.Message.Associated,
				PropTag.Message.SubmitResponsibility,
				PropTag.Message.SharedReceiptHandling,
				PropTag.Message.Inid,
				PropTag.Message.MessageAttachList,
				PropTag.Message.SenderCAI,
				PropTag.Message.SentRepresentingCAI,
				PropTag.Message.OriginalSenderCAI,
				PropTag.Message.OriginalSentRepresentingCAI,
				PropTag.Message.ReceivedByCAI,
				PropTag.Message.ReceivedRepresentingCAI,
				PropTag.Message.ReadReceiptCAI,
				PropTag.Message.ReportCAI,
				PropTag.Message.CreatorCAI,
				PropTag.Message.LastModifierCAI,
				PropTag.Message.CnsetRead,
				PropTag.Message.CnsetSeenFAI,
				PropTag.Message.IdSetDeleted,
				PropTag.Message.OriginatorCAI,
				PropTag.Message.ReportDestinationCAI,
				PropTag.Message.OriginalAuthorCAI,
				PropTag.Message.ReadCnNew,
				PropTag.Message.ReadCnNewBin,
				PropTag.Message.SenderTelephoneNumber,
				PropTag.Message.VoiceMessageAttachmentOrder,
				PropTag.Message.DocumentId,
				PropTag.Message.MailboxNum,
				PropTag.Message.ConversationIdHash,
				PropTag.Message.ConversationDocumentId,
				PropTag.Message.LocalDirectoryBlob,
				PropTag.Message.ViewStyle,
				PropTag.Message.FreebusyEMA,
				PropTag.Message.WunderbarLinkEntryID,
				PropTag.Message.WunderbarLinkStoreEntryId,
				PropTag.Message.SchdInfoFreebusyMerged,
				PropTag.Message.WunderbarLinkGroupClsId,
				PropTag.Message.WunderbarLinkGroupName,
				PropTag.Message.WunderbarLinkSection,
				PropTag.Message.NavigationNodeCalendarColor,
				PropTag.Message.NavigationNodeAddressbookEntryId,
				PropTag.Message.AgingDeleteItems,
				PropTag.Message.AgingFileName9AndPrev,
				PropTag.Message.AgingAgeFolder,
				PropTag.Message.AgingDontAgeMe,
				PropTag.Message.AgingFileNameAfter9,
				PropTag.Message.AgingWhenDeletedOnServer,
				PropTag.Message.AgingWaitUntilExpired,
				PropTag.Message.ConversationMvFrom,
				PropTag.Message.ConversationMvFromMailboxWide,
				PropTag.Message.ConversationMvTo,
				PropTag.Message.ConversationMvToMailboxWide,
				PropTag.Message.ConversationMessageDeliveryTime,
				PropTag.Message.ConversationMessageDeliveryTimeMailboxWide,
				PropTag.Message.ConversationCategories,
				PropTag.Message.ConversationCategoriesMailboxWide,
				PropTag.Message.ConversationFlagStatus,
				PropTag.Message.ConversationFlagStatusMailboxWide,
				PropTag.Message.ConversationFlagCompleteTime,
				PropTag.Message.ConversationFlagCompleteTimeMailboxWide,
				PropTag.Message.ConversationHasAttach,
				PropTag.Message.ConversationHasAttachMailboxWide,
				PropTag.Message.ConversationContentCount,
				PropTag.Message.ConversationContentCountMailboxWide,
				PropTag.Message.ConversationContentUnread,
				PropTag.Message.ConversationContentUnreadMailboxWide,
				PropTag.Message.ConversationMessageSize,
				PropTag.Message.ConversationMessageSizeMailboxWide,
				PropTag.Message.ConversationMessageClasses,
				PropTag.Message.ConversationMessageClassesMailboxWide,
				PropTag.Message.ConversationReplyForwardState,
				PropTag.Message.ConversationReplyForwardStateMailboxWide,
				PropTag.Message.ConversationImportance,
				PropTag.Message.ConversationImportanceMailboxWide,
				PropTag.Message.ConversationMvFromUnread,
				PropTag.Message.ConversationMvFromUnreadMailboxWide,
				PropTag.Message.ConversationMvItemIds,
				PropTag.Message.ConversationMvItemIdsMailboxWide,
				PropTag.Message.ConversationHasIrm,
				PropTag.Message.ConversationHasIrmMailboxWide,
				PropTag.Message.PersonCompanyNameMailboxWide,
				PropTag.Message.PersonDisplayNameMailboxWide,
				PropTag.Message.PersonGivenNameMailboxWide,
				PropTag.Message.PersonSurnameMailboxWide,
				PropTag.Message.PersonPhotoContactEntryIdMailboxWide,
				PropTag.Message.ConversationInferredImportanceInternal,
				PropTag.Message.ConversationInferredImportanceOverride,
				PropTag.Message.ConversationInferredUnimportanceInternal,
				PropTag.Message.ConversationInferredImportanceInternalMailboxWide,
				PropTag.Message.ConversationInferredImportanceOverrideMailboxWide,
				PropTag.Message.ConversationInferredUnimportanceInternalMailboxWide,
				PropTag.Message.PersonFileAsMailboxWide,
				PropTag.Message.PersonRelevanceScoreMailboxWide,
				PropTag.Message.PersonIsDistributionListMailboxWide,
				PropTag.Message.PersonHomeCityMailboxWide,
				PropTag.Message.PersonCreationTimeMailboxWide,
				PropTag.Message.PersonGALLinkIDMailboxWide,
				PropTag.Message.PersonMvEmailAddressMailboxWide,
				PropTag.Message.PersonMvEmailDisplayNameMailboxWide,
				PropTag.Message.PersonMvEmailRoutingTypeMailboxWide,
				PropTag.Message.PersonImAddressMailboxWide,
				PropTag.Message.PersonWorkCityMailboxWide,
				PropTag.Message.PersonDisplayNameFirstLastMailboxWide,
				PropTag.Message.PersonDisplayNameLastFirstMailboxWide,
				PropTag.Message.ConversationGroupingActions,
				PropTag.Message.ConversationGroupingActionsMailboxWide,
				PropTag.Message.ConversationPredictedActionsSummary,
				PropTag.Message.ConversationPredictedActionsSummaryMailboxWide,
				PropTag.Message.ConversationHasClutter,
				PropTag.Message.ConversationHasClutterMailboxWide,
				PropTag.Message.ConversationLastMemberDocumentId,
				PropTag.Message.ConversationPreview,
				PropTag.Message.ConversationLastMemberDocumentIdMailboxWide,
				PropTag.Message.ConversationInitialMemberDocumentId,
				PropTag.Message.ConversationMemberDocumentIds,
				PropTag.Message.ConversationMessageDeliveryOrRenewTimeMailboxWide,
				PropTag.Message.FamilyId,
				PropTag.Message.ConversationMessageRichContentMailboxWide,
				PropTag.Message.ConversationPreviewMailboxWide,
				PropTag.Message.ConversationMessageDeliveryOrRenewTime,
				PropTag.Message.ConversationWorkingSetSourcePartition,
				PropTag.Message.NDRFromName,
				PropTag.Message.SecurityFlags,
				PropTag.Message.SecurityReceiptRequestProcessed,
				PropTag.Message.FavoriteDisplayName,
				PropTag.Message.FavoriteDisplayAlias,
				PropTag.Message.FavPublicSourceKey,
				PropTag.Message.SyncFolderSourceKey,
				PropTag.Message.UserConfigurationDataType,
				PropTag.Message.UserConfigurationXmlStream,
				PropTag.Message.UserConfigurationStream,
				PropTag.Message.ReplyFwdStatus,
				PropTag.Message.OscSyncEnabledOnServer,
				PropTag.Message.Processed,
				PropTag.Message.FavLevelMask
			};
		}

		public static class Attachment
		{
			public static readonly StorePropTag TNEFCorrelationKey = new StorePropTag(127, PropertyType.Binary, new StorePropInfo("TNEFCorrelationKey", 127, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag PhysicalRenditionAttributes = new StorePropTag(3088, PropertyType.Binary, new StorePropInfo("PhysicalRenditionAttributes", 3088, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ItemSubobjectsBin = new StorePropTag(3603, PropertyType.Binary, new StorePropInfo("ItemSubobjectsBin", 3603, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachSize = new StorePropTag(3616, PropertyType.Int32, new StorePropInfo("AttachSize", 3616, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachSizeInt64 = new StorePropTag(3616, PropertyType.Int64, new StorePropInfo("AttachSizeInt64", 3616, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachNum = new StorePropTag(3617, PropertyType.Int32, new StorePropInfo("AttachNum", 3617, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CreatorSID = new StorePropTag(3672, PropertyType.Binary, new StorePropInfo("CreatorSID", 3672, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag LastModifierSid = new StorePropTag(3673, PropertyType.Binary, new StorePropInfo("LastModifierSid", 3673, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag VirusScannerStamp = new StorePropTag(3734, PropertyType.Binary, new StorePropInfo("VirusScannerStamp", 3734, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 6, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag VirusTransportStamp = new StorePropTag(3734, PropertyType.MVUnicode, new StorePropInfo("VirusTransportStamp", 3734, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AccessLevel = new StorePropTag(4087, PropertyType.Int32, new StorePropInfo("AccessLevel", 4087, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MappingSignature = new StorePropTag(4088, PropertyType.Binary, new StorePropInfo("MappingSignature", 4088, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag RecordKey = new StorePropTag(4089, PropertyType.Binary, new StorePropInfo("RecordKey", 4089, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ObjectType = new StorePropTag(4094, PropertyType.Int32, new StorePropInfo("ObjectType", 4094, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag Comment = new StorePropTag(12292, PropertyType.Unicode, new StorePropInfo("Comment", 12292, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CreationTime = new StorePropTag(12295, PropertyType.SysTime, new StorePropInfo("CreationTime", 12295, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag LastModificationTime = new StorePropTag(12296, PropertyType.SysTime, new StorePropInfo("LastModificationTime", 12296, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentX400Parameters = new StorePropTag(14080, PropertyType.Binary, new StorePropInfo("AttachmentX400Parameters", 14080, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag Content = new StorePropTag(14081, PropertyType.Binary, new StorePropInfo("Content", 14081, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ContentObj = new StorePropTag(14081, PropertyType.Object, new StorePropInfo("ContentObj", 14081, PropertyType.Object, StorePropInfo.Flags.None, 0UL, new PropertyCategories(15)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentEncoding = new StorePropTag(14082, PropertyType.Binary, new StorePropInfo("AttachmentEncoding", 14082, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ContentId = new StorePropTag(14083, PropertyType.Unicode, new StorePropInfo("ContentId", 14083, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ContentType = new StorePropTag(14084, PropertyType.Unicode, new StorePropInfo("ContentType", 14084, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachMethod = new StorePropTag(14085, PropertyType.Int32, new StorePropInfo("AttachMethod", 14085, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MimeUrl = new StorePropTag(14087, PropertyType.Unicode, new StorePropInfo("MimeUrl", 14087, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentPathName = new StorePropTag(14088, PropertyType.Unicode, new StorePropInfo("AttachmentPathName", 14088, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachRendering = new StorePropTag(14089, PropertyType.Binary, new StorePropInfo("AttachRendering", 14089, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachTag = new StorePropTag(14090, PropertyType.Binary, new StorePropInfo("AttachTag", 14090, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag RenderingPosition = new StorePropTag(14091, PropertyType.Int32, new StorePropInfo("RenderingPosition", 14091, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachTransportName = new StorePropTag(14092, PropertyType.Unicode, new StorePropInfo("AttachTransportName", 14092, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentLongPathName = new StorePropTag(14093, PropertyType.Unicode, new StorePropInfo("AttachmentLongPathName", 14093, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentMimeTag = new StorePropTag(14094, PropertyType.Unicode, new StorePropInfo("AttachmentMimeTag", 14094, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachAdditionalInfo = new StorePropTag(14095, PropertyType.Binary, new StorePropInfo("AttachAdditionalInfo", 14095, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentMimeSequence = new StorePropTag(14096, PropertyType.Int32, new StorePropInfo("AttachmentMimeSequence", 14096, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachContentBase = new StorePropTag(14097, PropertyType.Unicode, new StorePropInfo("AttachContentBase", 14097, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachContentId = new StorePropTag(14098, PropertyType.Unicode, new StorePropInfo("AttachContentId", 14098, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachContentLocation = new StorePropTag(14099, PropertyType.Unicode, new StorePropInfo("AttachContentLocation", 14099, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentFlags = new StorePropTag(14100, PropertyType.Int32, new StorePropInfo("AttachmentFlags", 14100, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachDisposition = new StorePropTag(14102, PropertyType.Unicode, new StorePropInfo("AttachDisposition", 14102, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachPayloadProviderGuidString = new StorePropTag(14105, PropertyType.Unicode, new StorePropInfo("AttachPayloadProviderGuidString", 14105, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachPayloadClass = new StorePropTag(14106, PropertyType.Unicode, new StorePropInfo("AttachPayloadClass", 14106, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag TextAttachmentCharset = new StorePropTag(14107, PropertyType.Unicode, new StorePropInfo("TextAttachmentCharset", 14107, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag Language = new StorePropTag(14860, PropertyType.Unicode, new StorePropInfo("Language", 14860, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag TestBlobProperty = new StorePropTag(15616, PropertyType.Int64, new StorePropInfo("TestBlobProperty", 15616, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MailboxPartitionNumber = new StorePropTag(15775, PropertyType.Int32, new StorePropInfo("MailboxPartitionNumber", 15775, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MailboxNumberInternal = new StorePropTag(15776, PropertyType.Int32, new StorePropInfo("MailboxNumberInternal", 15776, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(0, 1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentId = new StorePropTag(16264, PropertyType.Int64, new StorePropInfo("AttachmentId", 16264, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentIdBin = new StorePropTag(16264, PropertyType.Binary, new StorePropInfo("AttachmentIdBin", 16264, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ReplicaChangeNumber = new StorePropTag(16328, PropertyType.Binary, new StorePropInfo("ReplicaChangeNumber", 16328, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag NewAttach = new StorePropTag(16384, PropertyType.Int32, new StorePropInfo("NewAttach", 16384, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartEmbed = new StorePropTag(16385, PropertyType.Int32, new StorePropInfo("StartEmbed", 16385, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndEmbed = new StorePropTag(16386, PropertyType.Int32, new StorePropInfo("EndEmbed", 16386, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartRecip = new StorePropTag(16387, PropertyType.Int32, new StorePropInfo("StartRecip", 16387, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndRecip = new StorePropTag(16388, PropertyType.Int32, new StorePropInfo("EndRecip", 16388, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndCcRecip = new StorePropTag(16389, PropertyType.Int32, new StorePropInfo("EndCcRecip", 16389, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndBccRecip = new StorePropTag(16390, PropertyType.Int32, new StorePropInfo("EndBccRecip", 16390, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndP1Recip = new StorePropTag(16391, PropertyType.Int32, new StorePropInfo("EndP1Recip", 16391, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag DNPrefix = new StorePropTag(16392, PropertyType.Unicode, new StorePropInfo("DNPrefix", 16392, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartTopFolder = new StorePropTag(16393, PropertyType.Int32, new StorePropInfo("StartTopFolder", 16393, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartSubFolder = new StorePropTag(16394, PropertyType.Int32, new StorePropInfo("StartSubFolder", 16394, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndFolder = new StorePropTag(16395, PropertyType.Int32, new StorePropInfo("EndFolder", 16395, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartMessage = new StorePropTag(16396, PropertyType.Int32, new StorePropInfo("StartMessage", 16396, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndMessage = new StorePropTag(16397, PropertyType.Int32, new StorePropInfo("EndMessage", 16397, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EndAttach = new StorePropTag(16398, PropertyType.Int32, new StorePropInfo("EndAttach", 16398, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag EcWarning = new StorePropTag(16399, PropertyType.Int32, new StorePropInfo("EcWarning", 16399, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag StartFAIMessage = new StorePropTag(16400, PropertyType.Int32, new StorePropInfo("StartFAIMessage", 16400, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag NewFXFolder = new StorePropTag(16401, PropertyType.Binary, new StorePropInfo("NewFXFolder", 16401, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncChange = new StorePropTag(16402, PropertyType.Int32, new StorePropInfo("IncrSyncChange", 16402, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncDelete = new StorePropTag(16403, PropertyType.Int32, new StorePropInfo("IncrSyncDelete", 16403, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncEnd = new StorePropTag(16404, PropertyType.Int32, new StorePropInfo("IncrSyncEnd", 16404, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncMessage = new StorePropTag(16405, PropertyType.Int32, new StorePropInfo("IncrSyncMessage", 16405, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag FastTransferDelProp = new StorePropTag(16406, PropertyType.Int32, new StorePropInfo("FastTransferDelProp", 16406, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IdsetGiven = new StorePropTag(16407, PropertyType.Binary, new StorePropInfo("IdsetGiven", 16407, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IdsetGivenInt32 = new StorePropTag(16407, PropertyType.Int32, new StorePropInfo("IdsetGivenInt32", 16407, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag FastTransferErrorInfo = new StorePropTag(16408, PropertyType.Int32, new StorePropInfo("FastTransferErrorInfo", 16408, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag SoftDeletes = new StorePropTag(16417, PropertyType.Binary, new StorePropInfo("SoftDeletes", 16417, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IdsetRead = new StorePropTag(16429, PropertyType.Binary, new StorePropInfo("IdsetRead", 16429, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IdsetUnread = new StorePropTag(16430, PropertyType.Binary, new StorePropInfo("IdsetUnread", 16430, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncRead = new StorePropTag(16431, PropertyType.Int32, new StorePropInfo("IncrSyncRead", 16431, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncStateBegin = new StorePropTag(16442, PropertyType.Int32, new StorePropInfo("IncrSyncStateBegin", 16442, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncStateEnd = new StorePropTag(16443, PropertyType.Int32, new StorePropInfo("IncrSyncStateEnd", 16443, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncImailStream = new StorePropTag(16444, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream", 16444, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncImailStreamContinue = new StorePropTag(16486, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamContinue", 16486, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncImailStreamCancel = new StorePropTag(16487, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamCancel", 16487, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncImailStream2Continue = new StorePropTag(16497, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream2Continue", 16497, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncProgressMode = new StorePropTag(16500, PropertyType.Boolean, new StorePropInfo("IncrSyncProgressMode", 16500, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag SyncProgressPerMsg = new StorePropTag(16501, PropertyType.Boolean, new StorePropInfo("SyncProgressPerMsg", 16501, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncMsgPartial = new StorePropTag(16506, PropertyType.Int32, new StorePropInfo("IncrSyncMsgPartial", 16506, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncGroupInfo = new StorePropTag(16507, PropertyType.Int32, new StorePropInfo("IncrSyncGroupInfo", 16507, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncGroupId = new StorePropTag(16508, PropertyType.Int32, new StorePropInfo("IncrSyncGroupId", 16508, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IncrSyncChangePartial = new StorePropTag(16509, PropertyType.Int32, new StorePropInfo("IncrSyncChangePartial", 16509, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag HasNamedProperties = new StorePropTag(26186, PropertyType.Boolean, new StorePropInfo("HasNamedProperties", 26186, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CodePageId = new StorePropTag(26307, PropertyType.Int32, new StorePropInfo("CodePageId", 26307, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag URLName = new StorePropTag(26375, PropertyType.Unicode, new StorePropInfo("URLName", 26375, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MimeSize = new StorePropTag(26438, PropertyType.Int64, new StorePropInfo("MimeSize", 26438, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MimeSize32 = new StorePropTag(26438, PropertyType.Int32, new StorePropInfo("MimeSize32", 26438, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag FileSize = new StorePropTag(26439, PropertyType.Int64, new StorePropInfo("FileSize", 26439, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag FileSize32 = new StorePropTag(26439, PropertyType.Int32, new StorePropInfo("FileSize32", 26439, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag Mid = new StorePropTag(26442, PropertyType.Int64, new StorePropInfo("Mid", 26442, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MidBin = new StorePropTag(26442, PropertyType.Binary, new StorePropInfo("MidBin", 26442, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag LTID = new StorePropTag(26456, PropertyType.Binary, new StorePropInfo("LTID", 26456, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9, 10)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CnsetSeen = new StorePropTag(26518, PropertyType.Binary, new StorePropInfo("CnsetSeen", 26518, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag Inid = new StorePropTag(26544, PropertyType.Binary, new StorePropInfo("Inid", 26544, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CnsetRead = new StorePropTag(26578, PropertyType.Binary, new StorePropInfo("CnsetRead", 26578, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag CnsetSeenFAI = new StorePropTag(26586, PropertyType.Binary, new StorePropInfo("CnsetSeenFAI", 26586, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag IdSetDeleted = new StorePropTag(26597, PropertyType.Binary, new StorePropInfo("IdSetDeleted", 26597, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(1, 2, 3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachEXCLIVersion = new StorePropTag(26889, PropertyType.Int32, new StorePropInfo("AttachEXCLIVersion", 26889, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag HasDlpDetectedAttachmentClassifications = new StorePropTag(32760, PropertyType.Unicode, new StorePropInfo("HasDlpDetectedAttachmentClassifications", 32760, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag SExceptionReplaceTime = new StorePropTag(32761, PropertyType.SysTime, new StorePropInfo("SExceptionReplaceTime", 32761, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentLinkId = new StorePropTag(32762, PropertyType.Int32, new StorePropInfo("AttachmentLinkId", 32762, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ExceptionStartTime = new StorePropTag(32763, PropertyType.AppTime, new StorePropInfo("ExceptionStartTime", 32763, PropertyType.AppTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag ExceptionEndTime = new StorePropTag(32764, PropertyType.AppTime, new StorePropInfo("ExceptionEndTime", 32764, PropertyType.AppTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentFlags2 = new StorePropTag(32765, PropertyType.Int32, new StorePropInfo("AttachmentFlags2", 32765, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentHidden = new StorePropTag(32766, PropertyType.Boolean, new StorePropInfo("AttachmentHidden", 32766, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag AttachmentContactPhoto = new StorePropTag(32767, PropertyType.Boolean, new StorePropInfo("AttachmentContactPhoto", 32767, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Attachment);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[]
			{
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal
			};

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[]
			{
				PropTag.Attachment.MappingSignature,
				PropTag.Attachment.ObjectType,
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal,
				PropTag.Attachment.CnsetSeen,
				PropTag.Attachment.Inid,
				PropTag.Attachment.CnsetRead,
				PropTag.Attachment.CnsetSeenFAI,
				PropTag.Attachment.IdSetDeleted
			};

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[]
			{
				PropTag.Attachment.MappingSignature,
				PropTag.Attachment.ObjectType,
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal,
				PropTag.Attachment.CnsetSeen,
				PropTag.Attachment.Inid,
				PropTag.Attachment.CnsetRead,
				PropTag.Attachment.CnsetSeenFAI,
				PropTag.Attachment.IdSetDeleted
			};

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.Attachment.ItemSubobjectsBin,
				PropTag.Attachment.AttachSize,
				PropTag.Attachment.AttachSizeInt64,
				PropTag.Attachment.AttachNum,
				PropTag.Attachment.VirusScannerStamp,
				PropTag.Attachment.VirusTransportStamp,
				PropTag.Attachment.AccessLevel,
				PropTag.Attachment.RecordKey,
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal,
				PropTag.Attachment.AttachmentId,
				PropTag.Attachment.AttachmentIdBin,
				PropTag.Attachment.ReplicaChangeNumber,
				PropTag.Attachment.NewAttach,
				PropTag.Attachment.StartEmbed,
				PropTag.Attachment.EndEmbed,
				PropTag.Attachment.StartRecip,
				PropTag.Attachment.EndRecip,
				PropTag.Attachment.EndCcRecip,
				PropTag.Attachment.EndBccRecip,
				PropTag.Attachment.EndP1Recip,
				PropTag.Attachment.DNPrefix,
				PropTag.Attachment.StartTopFolder,
				PropTag.Attachment.StartSubFolder,
				PropTag.Attachment.EndFolder,
				PropTag.Attachment.StartMessage,
				PropTag.Attachment.EndMessage,
				PropTag.Attachment.EndAttach,
				PropTag.Attachment.EcWarning,
				PropTag.Attachment.StartFAIMessage,
				PropTag.Attachment.NewFXFolder,
				PropTag.Attachment.IncrSyncChange,
				PropTag.Attachment.IncrSyncDelete,
				PropTag.Attachment.IncrSyncEnd,
				PropTag.Attachment.IncrSyncMessage,
				PropTag.Attachment.FastTransferDelProp,
				PropTag.Attachment.IdsetGiven,
				PropTag.Attachment.IdsetGivenInt32,
				PropTag.Attachment.FastTransferErrorInfo,
				PropTag.Attachment.SoftDeletes,
				PropTag.Attachment.IdsetRead,
				PropTag.Attachment.IdsetUnread,
				PropTag.Attachment.IncrSyncRead,
				PropTag.Attachment.IncrSyncStateBegin,
				PropTag.Attachment.IncrSyncStateEnd,
				PropTag.Attachment.IncrSyncImailStream,
				PropTag.Attachment.IncrSyncImailStreamContinue,
				PropTag.Attachment.IncrSyncImailStreamCancel,
				PropTag.Attachment.IncrSyncImailStream2Continue,
				PropTag.Attachment.IncrSyncProgressMode,
				PropTag.Attachment.SyncProgressPerMsg,
				PropTag.Attachment.IncrSyncMsgPartial,
				PropTag.Attachment.IncrSyncGroupInfo,
				PropTag.Attachment.IncrSyncGroupId,
				PropTag.Attachment.IncrSyncChangePartial,
				PropTag.Attachment.HasNamedProperties,
				PropTag.Attachment.FileSize,
				PropTag.Attachment.FileSize32,
				PropTag.Attachment.Mid,
				PropTag.Attachment.MidBin,
				PropTag.Attachment.LTID,
				PropTag.Attachment.CnsetSeen,
				PropTag.Attachment.Inid,
				PropTag.Attachment.CnsetRead,
				PropTag.Attachment.CnsetSeenFAI,
				PropTag.Attachment.IdSetDeleted,
				PropTag.Attachment.MailboxNum
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[]
			{
				PropTag.Attachment.VirusScannerStamp
			};

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[]
			{
				PropTag.Attachment.ItemSubobjectsBin,
				PropTag.Attachment.AttachSize,
				PropTag.Attachment.AttachSizeInt64,
				PropTag.Attachment.AttachNum,
				PropTag.Attachment.VirusScannerStamp,
				PropTag.Attachment.VirusTransportStamp,
				PropTag.Attachment.AccessLevel,
				PropTag.Attachment.MappingSignature,
				PropTag.Attachment.RecordKey,
				PropTag.Attachment.ObjectType,
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal,
				PropTag.Attachment.AttachmentId,
				PropTag.Attachment.AttachmentIdBin,
				PropTag.Attachment.ReplicaChangeNumber,
				PropTag.Attachment.NewAttach,
				PropTag.Attachment.StartEmbed,
				PropTag.Attachment.EndEmbed,
				PropTag.Attachment.StartRecip,
				PropTag.Attachment.EndRecip,
				PropTag.Attachment.EndCcRecip,
				PropTag.Attachment.EndBccRecip,
				PropTag.Attachment.EndP1Recip,
				PropTag.Attachment.DNPrefix,
				PropTag.Attachment.StartTopFolder,
				PropTag.Attachment.StartSubFolder,
				PropTag.Attachment.EndFolder,
				PropTag.Attachment.StartMessage,
				PropTag.Attachment.EndMessage,
				PropTag.Attachment.EndAttach,
				PropTag.Attachment.EcWarning,
				PropTag.Attachment.StartFAIMessage,
				PropTag.Attachment.NewFXFolder,
				PropTag.Attachment.IncrSyncChange,
				PropTag.Attachment.IncrSyncDelete,
				PropTag.Attachment.IncrSyncEnd,
				PropTag.Attachment.IncrSyncMessage,
				PropTag.Attachment.FastTransferDelProp,
				PropTag.Attachment.IdsetGiven,
				PropTag.Attachment.IdsetGivenInt32,
				PropTag.Attachment.FastTransferErrorInfo,
				PropTag.Attachment.SoftDeletes,
				PropTag.Attachment.IdsetRead,
				PropTag.Attachment.IdsetUnread,
				PropTag.Attachment.IncrSyncRead,
				PropTag.Attachment.IncrSyncStateBegin,
				PropTag.Attachment.IncrSyncStateEnd,
				PropTag.Attachment.IncrSyncImailStream,
				PropTag.Attachment.IncrSyncImailStreamContinue,
				PropTag.Attachment.IncrSyncImailStreamCancel,
				PropTag.Attachment.IncrSyncImailStream2Continue,
				PropTag.Attachment.IncrSyncProgressMode,
				PropTag.Attachment.SyncProgressPerMsg,
				PropTag.Attachment.IncrSyncMsgPartial,
				PropTag.Attachment.IncrSyncGroupInfo,
				PropTag.Attachment.IncrSyncGroupId,
				PropTag.Attachment.IncrSyncChangePartial,
				PropTag.Attachment.HasNamedProperties,
				PropTag.Attachment.FileSize,
				PropTag.Attachment.FileSize32,
				PropTag.Attachment.Mid,
				PropTag.Attachment.MidBin,
				PropTag.Attachment.LTID,
				PropTag.Attachment.CnsetSeen,
				PropTag.Attachment.CnsetRead,
				PropTag.Attachment.CnsetSeenFAI,
				PropTag.Attachment.IdSetDeleted,
				PropTag.Attachment.MailboxNum
			};

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[]
			{
				PropTag.Attachment.AttachSize,
				PropTag.Attachment.AttachSizeInt64,
				PropTag.Attachment.AttachNum,
				PropTag.Attachment.AccessLevel,
				PropTag.Attachment.HasNamedProperties,
				PropTag.Attachment.FileSize,
				PropTag.Attachment.FileSize32,
				PropTag.Attachment.LTID
			};

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[]
			{
				PropTag.Attachment.Content,
				PropTag.Attachment.ContentObj
			};

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Attachment.TNEFCorrelationKey,
				PropTag.Attachment.PhysicalRenditionAttributes,
				PropTag.Attachment.ItemSubobjectsBin,
				PropTag.Attachment.AttachSize,
				PropTag.Attachment.AttachSizeInt64,
				PropTag.Attachment.AttachNum,
				PropTag.Attachment.CreatorSID,
				PropTag.Attachment.LastModifierSid,
				PropTag.Attachment.VirusScannerStamp,
				PropTag.Attachment.VirusTransportStamp,
				PropTag.Attachment.AccessLevel,
				PropTag.Attachment.MappingSignature,
				PropTag.Attachment.RecordKey,
				PropTag.Attachment.ObjectType,
				PropTag.Attachment.DisplayName,
				PropTag.Attachment.Comment,
				PropTag.Attachment.CreationTime,
				PropTag.Attachment.LastModificationTime,
				PropTag.Attachment.AttachmentX400Parameters,
				PropTag.Attachment.Content,
				PropTag.Attachment.ContentObj,
				PropTag.Attachment.AttachmentEncoding,
				PropTag.Attachment.ContentId,
				PropTag.Attachment.ContentType,
				PropTag.Attachment.AttachMethod,
				PropTag.Attachment.MimeUrl,
				PropTag.Attachment.AttachmentPathName,
				PropTag.Attachment.AttachRendering,
				PropTag.Attachment.AttachTag,
				PropTag.Attachment.RenderingPosition,
				PropTag.Attachment.AttachTransportName,
				PropTag.Attachment.AttachmentLongPathName,
				PropTag.Attachment.AttachmentMimeTag,
				PropTag.Attachment.AttachAdditionalInfo,
				PropTag.Attachment.AttachmentMimeSequence,
				PropTag.Attachment.AttachContentBase,
				PropTag.Attachment.AttachContentId,
				PropTag.Attachment.AttachContentLocation,
				PropTag.Attachment.AttachmentFlags,
				PropTag.Attachment.AttachDisposition,
				PropTag.Attachment.AttachPayloadProviderGuidString,
				PropTag.Attachment.AttachPayloadClass,
				PropTag.Attachment.TextAttachmentCharset,
				PropTag.Attachment.Language,
				PropTag.Attachment.TestBlobProperty,
				PropTag.Attachment.MailboxPartitionNumber,
				PropTag.Attachment.MailboxNumberInternal,
				PropTag.Attachment.AttachmentId,
				PropTag.Attachment.AttachmentIdBin,
				PropTag.Attachment.ReplicaChangeNumber,
				PropTag.Attachment.NewAttach,
				PropTag.Attachment.StartEmbed,
				PropTag.Attachment.EndEmbed,
				PropTag.Attachment.StartRecip,
				PropTag.Attachment.EndRecip,
				PropTag.Attachment.EndCcRecip,
				PropTag.Attachment.EndBccRecip,
				PropTag.Attachment.EndP1Recip,
				PropTag.Attachment.DNPrefix,
				PropTag.Attachment.StartTopFolder,
				PropTag.Attachment.StartSubFolder,
				PropTag.Attachment.EndFolder,
				PropTag.Attachment.StartMessage,
				PropTag.Attachment.EndMessage,
				PropTag.Attachment.EndAttach,
				PropTag.Attachment.EcWarning,
				PropTag.Attachment.StartFAIMessage,
				PropTag.Attachment.NewFXFolder,
				PropTag.Attachment.IncrSyncChange,
				PropTag.Attachment.IncrSyncDelete,
				PropTag.Attachment.IncrSyncEnd,
				PropTag.Attachment.IncrSyncMessage,
				PropTag.Attachment.FastTransferDelProp,
				PropTag.Attachment.IdsetGiven,
				PropTag.Attachment.IdsetGivenInt32,
				PropTag.Attachment.FastTransferErrorInfo,
				PropTag.Attachment.SoftDeletes,
				PropTag.Attachment.IdsetRead,
				PropTag.Attachment.IdsetUnread,
				PropTag.Attachment.IncrSyncRead,
				PropTag.Attachment.IncrSyncStateBegin,
				PropTag.Attachment.IncrSyncStateEnd,
				PropTag.Attachment.IncrSyncImailStream,
				PropTag.Attachment.IncrSyncImailStreamContinue,
				PropTag.Attachment.IncrSyncImailStreamCancel,
				PropTag.Attachment.IncrSyncImailStream2Continue,
				PropTag.Attachment.IncrSyncProgressMode,
				PropTag.Attachment.SyncProgressPerMsg,
				PropTag.Attachment.IncrSyncMsgPartial,
				PropTag.Attachment.IncrSyncGroupInfo,
				PropTag.Attachment.IncrSyncGroupId,
				PropTag.Attachment.IncrSyncChangePartial,
				PropTag.Attachment.HasNamedProperties,
				PropTag.Attachment.CodePageId,
				PropTag.Attachment.URLName,
				PropTag.Attachment.MimeSize,
				PropTag.Attachment.MimeSize32,
				PropTag.Attachment.FileSize,
				PropTag.Attachment.FileSize32,
				PropTag.Attachment.Mid,
				PropTag.Attachment.MidBin,
				PropTag.Attachment.LTID,
				PropTag.Attachment.CnsetSeen,
				PropTag.Attachment.Inid,
				PropTag.Attachment.CnsetRead,
				PropTag.Attachment.CnsetSeenFAI,
				PropTag.Attachment.IdSetDeleted,
				PropTag.Attachment.MailboxNum,
				PropTag.Attachment.AttachEXCLIVersion,
				PropTag.Attachment.HasDlpDetectedAttachmentClassifications,
				PropTag.Attachment.SExceptionReplaceTime,
				PropTag.Attachment.AttachmentLinkId,
				PropTag.Attachment.ExceptionStartTime,
				PropTag.Attachment.ExceptionEndTime,
				PropTag.Attachment.AttachmentFlags2,
				PropTag.Attachment.AttachmentHidden,
				PropTag.Attachment.AttachmentContactPhoto
			};
		}

		public static class Recipient
		{
			public static readonly StorePropTag DeliveryReportRequested = new StorePropTag(35, PropertyType.Boolean, new StorePropInfo("DeliveryReportRequested", 35, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ReadReceiptRequested = new StorePropTag(41, PropertyType.Boolean, new StorePropInfo("ReadReceiptRequested", 41, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ReportTime = new StorePropTag(50, PropertyType.SysTime, new StorePropInfo("ReportTime", 50, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DiscVal = new StorePropTag(74, PropertyType.Boolean, new StorePropInfo("DiscVal", 74, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ExplicitConversion = new StorePropTag(3073, PropertyType.Int32, new StorePropInfo("ExplicitConversion", 3073, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NDRReasonCode = new StorePropTag(3076, PropertyType.Int32, new StorePropInfo("NDRReasonCode", 3076, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NDRDiagCode = new StorePropTag(3077, PropertyType.Int32, new StorePropInfo("NDRDiagCode", 3077, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NonReceiptNotificationRequested = new StorePropTag(3078, PropertyType.Boolean, new StorePropInfo("NonReceiptNotificationRequested", 3078, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NonDeliveryReportRequested = new StorePropTag(3080, PropertyType.Boolean, new StorePropInfo("NonDeliveryReportRequested", 3080, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OriginatorRequestedAlterateRecipient = new StorePropTag(3081, PropertyType.Binary, new StorePropInfo("OriginatorRequestedAlterateRecipient", 3081, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PhysicalDeliveryMode = new StorePropTag(3083, PropertyType.Int32, new StorePropInfo("PhysicalDeliveryMode", 3083, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PhysicalDeliveryReportRequest = new StorePropTag(3084, PropertyType.Int32, new StorePropInfo("PhysicalDeliveryReportRequest", 3084, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PhysicalForwardingAddress = new StorePropTag(3085, PropertyType.Binary, new StorePropInfo("PhysicalForwardingAddress", 3085, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PhysicalForwardingAddressRequested = new StorePropTag(3086, PropertyType.Boolean, new StorePropInfo("PhysicalForwardingAddressRequested", 3086, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PhysicalForwardingProhibited = new StorePropTag(3087, PropertyType.Boolean, new StorePropInfo("PhysicalForwardingProhibited", 3087, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ProofOfDelivery = new StorePropTag(3089, PropertyType.Binary, new StorePropInfo("ProofOfDelivery", 3089, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ProofOfDeliveryRequested = new StorePropTag(3090, PropertyType.Boolean, new StorePropInfo("ProofOfDeliveryRequested", 3090, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientCertificate = new StorePropTag(3091, PropertyType.Binary, new StorePropInfo("RecipientCertificate", 3091, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientNumberForAdvice = new StorePropTag(3092, PropertyType.Unicode, new StorePropInfo("RecipientNumberForAdvice", 3092, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientType = new StorePropTag(3093, PropertyType.Int32, new StorePropInfo("RecipientType", 3093, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag TypeOfMTSUser = new StorePropTag(3100, PropertyType.Int32, new StorePropInfo("TypeOfMTSUser", 3100, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DiscreteValues = new StorePropTag(3598, PropertyType.Boolean, new StorePropInfo("DiscreteValues", 3598, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Responsibility = new StorePropTag(3599, PropertyType.Boolean, new StorePropInfo("Responsibility", 3599, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientStatus = new StorePropTag(3605, PropertyType.Int32, new StorePropInfo("RecipientStatus", 3605, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag InstanceKey = new StorePropTag(4086, PropertyType.Binary, new StorePropInfo("InstanceKey", 4086, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag AccessLevel = new StorePropTag(4087, PropertyType.Int32, new StorePropInfo("AccessLevel", 4087, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecordKey = new StorePropTag(4089, PropertyType.Binary, new StorePropInfo("RecordKey", 4089, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecordKeySvrEid = new StorePropTag(4089, PropertyType.SvrEid, new StorePropInfo("RecordKeySvrEid", 4089, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ObjectType = new StorePropTag(4094, PropertyType.Int32, new StorePropInfo("ObjectType", 4094, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EntryId = new StorePropTag(4095, PropertyType.Binary, new StorePropInfo("EntryId", 4095, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EntryIdSvrEid = new StorePropTag(4095, PropertyType.SvrEid, new StorePropInfo("EntryIdSvrEid", 4095, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RowId = new StorePropTag(12288, PropertyType.Int32, new StorePropInfo("RowId", 12288, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag AddressType = new StorePropTag(12290, PropertyType.Unicode, new StorePropInfo("AddressType", 12290, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EmailAddress = new StorePropTag(12291, PropertyType.Unicode, new StorePropInfo("EmailAddress", 12291, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Comment = new StorePropTag(12292, PropertyType.Unicode, new StorePropInfo("Comment", 12292, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag LastModificationTime = new StorePropTag(12296, PropertyType.SysTime, new StorePropInfo("LastModificationTime", 12296, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SearchKey = new StorePropTag(12299, PropertyType.Binary, new StorePropInfo("SearchKey", 12299, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SearchKeySvrEid = new StorePropTag(12299, PropertyType.SvrEid, new StorePropInfo("SearchKeySvrEid", 12299, PropertyType.SvrEid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DetailsTable = new StorePropTag(13829, PropertyType.Object, new StorePropInfo("DetailsTable", 13829, PropertyType.Object, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DisplayType = new StorePropTag(14592, PropertyType.Int32, new StorePropInfo("DisplayType", 14592, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SmtpAddress = new StorePropTag(14846, PropertyType.Unicode, new StorePropInfo("SmtpAddress", 14846, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SimpleDisplayName = new StorePropTag(14847, PropertyType.Unicode, new StorePropInfo("SimpleDisplayName", 14847, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Account = new StorePropTag(14848, PropertyType.Unicode, new StorePropInfo("Account", 14848, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag AlternateRecipient = new StorePropTag(14849, PropertyType.Binary, new StorePropInfo("AlternateRecipient", 14849, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CallbackTelephoneNumber = new StorePropTag(14850, PropertyType.Unicode, new StorePropInfo("CallbackTelephoneNumber", 14850, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Generation = new StorePropTag(14853, PropertyType.Unicode, new StorePropInfo("Generation", 14853, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag GivenName = new StorePropTag(14854, PropertyType.Unicode, new StorePropInfo("GivenName", 14854, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag GovernmentIDNumber = new StorePropTag(14855, PropertyType.Unicode, new StorePropInfo("GovernmentIDNumber", 14855, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag BusinessTelephoneNumber = new StorePropTag(14856, PropertyType.Unicode, new StorePropInfo("BusinessTelephoneNumber", 14856, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeTelephoneNumber = new StorePropTag(14857, PropertyType.Unicode, new StorePropInfo("HomeTelephoneNumber", 14857, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Initials = new StorePropTag(14858, PropertyType.Unicode, new StorePropInfo("Initials", 14858, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Keyword = new StorePropTag(14859, PropertyType.Unicode, new StorePropInfo("Keyword", 14859, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Language = new StorePropTag(14860, PropertyType.Unicode, new StorePropInfo("Language", 14860, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Location = new StorePropTag(14861, PropertyType.Unicode, new StorePropInfo("Location", 14861, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag MailPermission = new StorePropTag(14862, PropertyType.Boolean, new StorePropInfo("MailPermission", 14862, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OrganizationalIDNumber = new StorePropTag(14864, PropertyType.Unicode, new StorePropInfo("OrganizationalIDNumber", 14864, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SurName = new StorePropTag(14865, PropertyType.Unicode, new StorePropInfo("SurName", 14865, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OriginalEntryId = new StorePropTag(14866, PropertyType.Binary, new StorePropInfo("OriginalEntryId", 14866, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OriginalDisplayName = new StorePropTag(14867, PropertyType.Unicode, new StorePropInfo("OriginalDisplayName", 14867, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OriginalSearchKey = new StorePropTag(14868, PropertyType.Binary, new StorePropInfo("OriginalSearchKey", 14868, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PostalAddress = new StorePropTag(14869, PropertyType.Unicode, new StorePropInfo("PostalAddress", 14869, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CompanyName = new StorePropTag(14870, PropertyType.Unicode, new StorePropInfo("CompanyName", 14870, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Title = new StorePropTag(14871, PropertyType.Unicode, new StorePropInfo("Title", 14871, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DepartmentName = new StorePropTag(14872, PropertyType.Unicode, new StorePropInfo("DepartmentName", 14872, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OfficeLocation = new StorePropTag(14873, PropertyType.Unicode, new StorePropInfo("OfficeLocation", 14873, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PrimaryTelephoneNumber = new StorePropTag(14874, PropertyType.Unicode, new StorePropInfo("PrimaryTelephoneNumber", 14874, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Business2TelephoneNumber = new StorePropTag(14875, PropertyType.Unicode, new StorePropInfo("Business2TelephoneNumber", 14875, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Business2TelephoneNumberMv = new StorePropTag(14875, PropertyType.MVUnicode, new StorePropInfo("Business2TelephoneNumberMv", 14875, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag MobileTelephoneNumber = new StorePropTag(14876, PropertyType.Unicode, new StorePropInfo("MobileTelephoneNumber", 14876, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RadioTelephoneNumber = new StorePropTag(14877, PropertyType.Unicode, new StorePropInfo("RadioTelephoneNumber", 14877, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CarTelephoneNumber = new StorePropTag(14878, PropertyType.Unicode, new StorePropInfo("CarTelephoneNumber", 14878, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherTelephoneNumber = new StorePropTag(14879, PropertyType.Unicode, new StorePropInfo("OtherTelephoneNumber", 14879, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag TransmitableDisplayName = new StorePropTag(14880, PropertyType.Unicode, new StorePropInfo("TransmitableDisplayName", 14880, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PagerTelephoneNumber = new StorePropTag(14881, PropertyType.Unicode, new StorePropInfo("PagerTelephoneNumber", 14881, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag UserCertificate = new StorePropTag(14882, PropertyType.Binary, new StorePropInfo("UserCertificate", 14882, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PrimaryFaxNumber = new StorePropTag(14883, PropertyType.Unicode, new StorePropInfo("PrimaryFaxNumber", 14883, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag BusinessFaxNumber = new StorePropTag(14884, PropertyType.Unicode, new StorePropInfo("BusinessFaxNumber", 14884, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeFaxNumber = new StorePropTag(14885, PropertyType.Unicode, new StorePropInfo("HomeFaxNumber", 14885, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Country = new StorePropTag(14886, PropertyType.Unicode, new StorePropInfo("Country", 14886, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Locality = new StorePropTag(14887, PropertyType.Unicode, new StorePropInfo("Locality", 14887, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StateOrProvince = new StorePropTag(14888, PropertyType.Unicode, new StorePropInfo("StateOrProvince", 14888, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StreetAddress = new StorePropTag(14889, PropertyType.Unicode, new StorePropInfo("StreetAddress", 14889, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PostalCode = new StorePropTag(14890, PropertyType.Unicode, new StorePropInfo("PostalCode", 14890, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PostOfficeBox = new StorePropTag(14891, PropertyType.Unicode, new StorePropInfo("PostOfficeBox", 14891, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag TelexNumber = new StorePropTag(14892, PropertyType.Unicode, new StorePropInfo("TelexNumber", 14892, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ISDNNumber = new StorePropTag(14893, PropertyType.Unicode, new StorePropInfo("ISDNNumber", 14893, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag AssistantTelephoneNumber = new StorePropTag(14894, PropertyType.Unicode, new StorePropInfo("AssistantTelephoneNumber", 14894, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Home2TelephoneNumber = new StorePropTag(14895, PropertyType.Unicode, new StorePropInfo("Home2TelephoneNumber", 14895, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Home2TelephoneNumberMv = new StorePropTag(14895, PropertyType.MVUnicode, new StorePropInfo("Home2TelephoneNumberMv", 14895, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Assistant = new StorePropTag(14896, PropertyType.Unicode, new StorePropInfo("Assistant", 14896, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SendRichInfo = new StorePropTag(14912, PropertyType.Boolean, new StorePropInfo("SendRichInfo", 14912, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag WeddingAnniversary = new StorePropTag(14913, PropertyType.SysTime, new StorePropInfo("WeddingAnniversary", 14913, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Birthday = new StorePropTag(14914, PropertyType.SysTime, new StorePropInfo("Birthday", 14914, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Hobbies = new StorePropTag(14915, PropertyType.Unicode, new StorePropInfo("Hobbies", 14915, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag MiddleName = new StorePropTag(14916, PropertyType.Unicode, new StorePropInfo("MiddleName", 14916, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DisplayNamePrefix = new StorePropTag(14917, PropertyType.Unicode, new StorePropInfo("DisplayNamePrefix", 14917, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Profession = new StorePropTag(14918, PropertyType.Unicode, new StorePropInfo("Profession", 14918, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ReferredByName = new StorePropTag(14919, PropertyType.Unicode, new StorePropInfo("ReferredByName", 14919, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SpouseName = new StorePropTag(14920, PropertyType.Unicode, new StorePropInfo("SpouseName", 14920, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ComputerNetworkName = new StorePropTag(14921, PropertyType.Unicode, new StorePropInfo("ComputerNetworkName", 14921, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CustomerId = new StorePropTag(14922, PropertyType.Unicode, new StorePropInfo("CustomerId", 14922, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag TTYTDDPhoneNumber = new StorePropTag(14923, PropertyType.Unicode, new StorePropInfo("TTYTDDPhoneNumber", 14923, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag FTPSite = new StorePropTag(14924, PropertyType.Unicode, new StorePropInfo("FTPSite", 14924, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag Gender = new StorePropTag(14925, PropertyType.Int16, new StorePropInfo("Gender", 14925, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ManagerName = new StorePropTag(14926, PropertyType.Unicode, new StorePropInfo("ManagerName", 14926, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NickName = new StorePropTag(14927, PropertyType.Unicode, new StorePropInfo("NickName", 14927, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PersonalHomePage = new StorePropTag(14928, PropertyType.Unicode, new StorePropInfo("PersonalHomePage", 14928, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag BusinessHomePage = new StorePropTag(14929, PropertyType.Unicode, new StorePropInfo("BusinessHomePage", 14929, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ContactVersion = new StorePropTag(14930, PropertyType.Guid, new StorePropInfo("ContactVersion", 14930, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ContactEntryIds = new StorePropTag(14931, PropertyType.MVBinary, new StorePropInfo("ContactEntryIds", 14931, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ContactAddressTypes = new StorePropTag(14932, PropertyType.MVUnicode, new StorePropInfo("ContactAddressTypes", 14932, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ContactDefaultAddressIndex = new StorePropTag(14933, PropertyType.Int32, new StorePropInfo("ContactDefaultAddressIndex", 14933, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ContactEmailAddress = new StorePropTag(14934, PropertyType.MVUnicode, new StorePropInfo("ContactEmailAddress", 14934, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CompanyMainPhoneNumber = new StorePropTag(14935, PropertyType.Unicode, new StorePropInfo("CompanyMainPhoneNumber", 14935, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag ChildrensNames = new StorePropTag(14936, PropertyType.MVUnicode, new StorePropInfo("ChildrensNames", 14936, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressCity = new StorePropTag(14937, PropertyType.Unicode, new StorePropInfo("HomeAddressCity", 14937, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressCountry = new StorePropTag(14938, PropertyType.Unicode, new StorePropInfo("HomeAddressCountry", 14938, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressPostalCode = new StorePropTag(14939, PropertyType.Unicode, new StorePropInfo("HomeAddressPostalCode", 14939, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressStateOrProvince = new StorePropTag(14940, PropertyType.Unicode, new StorePropInfo("HomeAddressStateOrProvince", 14940, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressStreet = new StorePropTag(14941, PropertyType.Unicode, new StorePropInfo("HomeAddressStreet", 14941, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag HomeAddressPostOfficeBox = new StorePropTag(14942, PropertyType.Unicode, new StorePropInfo("HomeAddressPostOfficeBox", 14942, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressCity = new StorePropTag(14943, PropertyType.Unicode, new StorePropInfo("OtherAddressCity", 14943, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressCountry = new StorePropTag(14944, PropertyType.Unicode, new StorePropInfo("OtherAddressCountry", 14944, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressPostalCode = new StorePropTag(14945, PropertyType.Unicode, new StorePropInfo("OtherAddressPostalCode", 14945, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressStateOrProvince = new StorePropTag(14946, PropertyType.Unicode, new StorePropInfo("OtherAddressStateOrProvince", 14946, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressStreet = new StorePropTag(14947, PropertyType.Unicode, new StorePropInfo("OtherAddressStreet", 14947, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OtherAddressPostOfficeBox = new StorePropTag(14948, PropertyType.Unicode, new StorePropInfo("OtherAddressPostOfficeBox", 14948, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag UserX509CertificateABSearchPath = new StorePropTag(14960, PropertyType.MVBinary, new StorePropInfo("UserX509CertificateABSearchPath", 14960, PropertyType.MVBinary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SendInternetEncoding = new StorePropTag(14961, PropertyType.Int32, new StorePropInfo("SendInternetEncoding", 14961, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PartnerNetworkId = new StorePropTag(14966, PropertyType.Unicode, new StorePropInfo("PartnerNetworkId", 14966, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PartnerNetworkUserId = new StorePropTag(14967, PropertyType.Unicode, new StorePropInfo("PartnerNetworkUserId", 14967, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PartnerNetworkThumbnailPhotoUrl = new StorePropTag(14968, PropertyType.Unicode, new StorePropInfo("PartnerNetworkThumbnailPhotoUrl", 14968, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PartnerNetworkProfilePhotoUrl = new StorePropTag(14969, PropertyType.Unicode, new StorePropInfo("PartnerNetworkProfilePhotoUrl", 14969, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag PartnerNetworkContactType = new StorePropTag(14970, PropertyType.Unicode, new StorePropInfo("PartnerNetworkContactType", 14970, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RelevanceScore = new StorePropTag(14971, PropertyType.Int32, new StorePropInfo("RelevanceScore", 14971, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IsDistributionListContact = new StorePropTag(14972, PropertyType.Boolean, new StorePropInfo("IsDistributionListContact", 14972, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IsPromotedContact = new StorePropTag(14973, PropertyType.Boolean, new StorePropInfo("IsPromotedContact", 14973, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OrgUnitName = new StorePropTag(15358, PropertyType.Unicode, new StorePropInfo("OrgUnitName", 15358, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag OrganizationName = new StorePropTag(15359, PropertyType.Unicode, new StorePropInfo("OrganizationName", 15359, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag TestBlobProperty = new StorePropTag(15616, PropertyType.Int64, new StorePropInfo("TestBlobProperty", 15616, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NewAttach = new StorePropTag(16384, PropertyType.Int32, new StorePropInfo("NewAttach", 16384, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartEmbed = new StorePropTag(16385, PropertyType.Int32, new StorePropInfo("StartEmbed", 16385, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndEmbed = new StorePropTag(16386, PropertyType.Int32, new StorePropInfo("EndEmbed", 16386, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartRecip = new StorePropTag(16387, PropertyType.Int32, new StorePropInfo("StartRecip", 16387, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndRecip = new StorePropTag(16388, PropertyType.Int32, new StorePropInfo("EndRecip", 16388, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndCcRecip = new StorePropTag(16389, PropertyType.Int32, new StorePropInfo("EndCcRecip", 16389, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndBccRecip = new StorePropTag(16390, PropertyType.Int32, new StorePropInfo("EndBccRecip", 16390, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndP1Recip = new StorePropTag(16391, PropertyType.Int32, new StorePropInfo("EndP1Recip", 16391, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DNPrefix = new StorePropTag(16392, PropertyType.Unicode, new StorePropInfo("DNPrefix", 16392, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartTopFolder = new StorePropTag(16393, PropertyType.Int32, new StorePropInfo("StartTopFolder", 16393, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartSubFolder = new StorePropTag(16394, PropertyType.Int32, new StorePropInfo("StartSubFolder", 16394, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndFolder = new StorePropTag(16395, PropertyType.Int32, new StorePropInfo("EndFolder", 16395, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartMessage = new StorePropTag(16396, PropertyType.Int32, new StorePropInfo("StartMessage", 16396, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndMessage = new StorePropTag(16397, PropertyType.Int32, new StorePropInfo("EndMessage", 16397, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EndAttach = new StorePropTag(16398, PropertyType.Int32, new StorePropInfo("EndAttach", 16398, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag EcWarning = new StorePropTag(16399, PropertyType.Int32, new StorePropInfo("EcWarning", 16399, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag StartFAIMessage = new StorePropTag(16400, PropertyType.Int32, new StorePropInfo("StartFAIMessage", 16400, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag NewFXFolder = new StorePropTag(16401, PropertyType.Binary, new StorePropInfo("NewFXFolder", 16401, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncChange = new StorePropTag(16402, PropertyType.Int32, new StorePropInfo("IncrSyncChange", 16402, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncDelete = new StorePropTag(16403, PropertyType.Int32, new StorePropInfo("IncrSyncDelete", 16403, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncEnd = new StorePropTag(16404, PropertyType.Int32, new StorePropInfo("IncrSyncEnd", 16404, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncMessage = new StorePropTag(16405, PropertyType.Int32, new StorePropInfo("IncrSyncMessage", 16405, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag FastTransferDelProp = new StorePropTag(16406, PropertyType.Int32, new StorePropInfo("FastTransferDelProp", 16406, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IdsetGiven = new StorePropTag(16407, PropertyType.Binary, new StorePropInfo("IdsetGiven", 16407, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IdsetGivenInt32 = new StorePropTag(16407, PropertyType.Int32, new StorePropInfo("IdsetGivenInt32", 16407, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag FastTransferErrorInfo = new StorePropTag(16408, PropertyType.Int32, new StorePropInfo("FastTransferErrorInfo", 16408, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SoftDeletes = new StorePropTag(16417, PropertyType.Binary, new StorePropInfo("SoftDeletes", 16417, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IdsetRead = new StorePropTag(16429, PropertyType.Binary, new StorePropInfo("IdsetRead", 16429, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IdsetUnread = new StorePropTag(16430, PropertyType.Binary, new StorePropInfo("IdsetUnread", 16430, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncRead = new StorePropTag(16431, PropertyType.Int32, new StorePropInfo("IncrSyncRead", 16431, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncStateBegin = new StorePropTag(16442, PropertyType.Int32, new StorePropInfo("IncrSyncStateBegin", 16442, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncStateEnd = new StorePropTag(16443, PropertyType.Int32, new StorePropInfo("IncrSyncStateEnd", 16443, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncImailStream = new StorePropTag(16444, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream", 16444, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncImailStreamContinue = new StorePropTag(16486, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamContinue", 16486, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncImailStreamCancel = new StorePropTag(16487, PropertyType.Int32, new StorePropInfo("IncrSyncImailStreamCancel", 16487, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncImailStream2Continue = new StorePropTag(16497, PropertyType.Int32, new StorePropInfo("IncrSyncImailStream2Continue", 16497, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncProgressMode = new StorePropTag(16500, PropertyType.Boolean, new StorePropInfo("IncrSyncProgressMode", 16500, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SyncProgressPerMsg = new StorePropTag(16501, PropertyType.Boolean, new StorePropInfo("SyncProgressPerMsg", 16501, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncMsgPartial = new StorePropTag(16506, PropertyType.Int32, new StorePropInfo("IncrSyncMsgPartial", 16506, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncGroupInfo = new StorePropTag(16507, PropertyType.Int32, new StorePropInfo("IncrSyncGroupInfo", 16507, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncGroupId = new StorePropTag(16508, PropertyType.Int32, new StorePropInfo("IncrSyncGroupId", 16508, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IncrSyncChangePartial = new StorePropTag(16509, PropertyType.Int32, new StorePropInfo("IncrSyncChangePartial", 16509, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientOrder = new StorePropTag(24543, PropertyType.Int32, new StorePropInfo("RecipientOrder", 24543, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientSipUri = new StorePropTag(24549, PropertyType.Unicode, new StorePropInfo("RecipientSipUri", 24549, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientDisplayName = new StorePropTag(24566, PropertyType.Unicode, new StorePropInfo("RecipientDisplayName", 24566, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientEntryId = new StorePropTag(24567, PropertyType.Binary, new StorePropInfo("RecipientEntryId", 24567, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientFlags = new StorePropTag(24573, PropertyType.Int32, new StorePropInfo("RecipientFlags", 24573, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientTrackStatus = new StorePropTag(24575, PropertyType.Int32, new StorePropInfo("RecipientTrackStatus", 24575, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag DotStuffState = new StorePropTag(24577, PropertyType.Unicode, new StorePropInfo("DotStuffState", 24577, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag RecipientNumber = new StorePropTag(26210, PropertyType.Int32, new StorePropInfo("RecipientNumber", 26210, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag UserDN = new StorePropTag(26314, PropertyType.Unicode, new StorePropInfo("UserDN", 26314, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CnsetSeen = new StorePropTag(26518, PropertyType.Binary, new StorePropInfo("CnsetSeen", 26518, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag SourceEntryId = new StorePropTag(26536, PropertyType.Binary, new StorePropInfo("SourceEntryId", 26536, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CnsetRead = new StorePropTag(26578, PropertyType.Binary, new StorePropInfo("CnsetRead", 26578, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag CnsetSeenFAI = new StorePropTag(26586, PropertyType.Binary, new StorePropInfo("CnsetSeenFAI", 26586, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag IdSetDeleted = new StorePropTag(26597, PropertyType.Binary, new StorePropInfo("IdSetDeleted", 26597, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 9)), Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.Recipient.NewAttach,
				PropTag.Recipient.StartEmbed,
				PropTag.Recipient.EndEmbed,
				PropTag.Recipient.StartRecip,
				PropTag.Recipient.EndRecip,
				PropTag.Recipient.EndCcRecip,
				PropTag.Recipient.EndBccRecip,
				PropTag.Recipient.EndP1Recip,
				PropTag.Recipient.DNPrefix,
				PropTag.Recipient.StartTopFolder,
				PropTag.Recipient.StartSubFolder,
				PropTag.Recipient.EndFolder,
				PropTag.Recipient.StartMessage,
				PropTag.Recipient.EndMessage,
				PropTag.Recipient.EndAttach,
				PropTag.Recipient.EcWarning,
				PropTag.Recipient.StartFAIMessage,
				PropTag.Recipient.NewFXFolder,
				PropTag.Recipient.IncrSyncChange,
				PropTag.Recipient.IncrSyncDelete,
				PropTag.Recipient.IncrSyncEnd,
				PropTag.Recipient.IncrSyncMessage,
				PropTag.Recipient.FastTransferDelProp,
				PropTag.Recipient.IdsetGiven,
				PropTag.Recipient.IdsetGivenInt32,
				PropTag.Recipient.FastTransferErrorInfo,
				PropTag.Recipient.SoftDeletes,
				PropTag.Recipient.IdsetRead,
				PropTag.Recipient.IdsetUnread,
				PropTag.Recipient.IncrSyncRead,
				PropTag.Recipient.IncrSyncStateBegin,
				PropTag.Recipient.IncrSyncStateEnd,
				PropTag.Recipient.IncrSyncImailStream,
				PropTag.Recipient.IncrSyncImailStreamContinue,
				PropTag.Recipient.IncrSyncImailStreamCancel,
				PropTag.Recipient.IncrSyncImailStream2Continue,
				PropTag.Recipient.IncrSyncProgressMode,
				PropTag.Recipient.SyncProgressPerMsg,
				PropTag.Recipient.IncrSyncMsgPartial,
				PropTag.Recipient.IncrSyncGroupInfo,
				PropTag.Recipient.IncrSyncGroupId,
				PropTag.Recipient.IncrSyncChangePartial,
				PropTag.Recipient.CnsetSeen,
				PropTag.Recipient.CnsetRead,
				PropTag.Recipient.CnsetSeenFAI,
				PropTag.Recipient.IdSetDeleted
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[]
			{
				PropTag.Recipient.RecordKey,
				PropTag.Recipient.RecordKeySvrEid,
				PropTag.Recipient.ObjectType,
				PropTag.Recipient.EntryId,
				PropTag.Recipient.EntryIdSvrEid,
				PropTag.Recipient.RowId,
				PropTag.Recipient.NewAttach,
				PropTag.Recipient.StartEmbed,
				PropTag.Recipient.EndEmbed,
				PropTag.Recipient.StartRecip,
				PropTag.Recipient.EndRecip,
				PropTag.Recipient.EndCcRecip,
				PropTag.Recipient.EndBccRecip,
				PropTag.Recipient.EndP1Recip,
				PropTag.Recipient.DNPrefix,
				PropTag.Recipient.StartTopFolder,
				PropTag.Recipient.StartSubFolder,
				PropTag.Recipient.EndFolder,
				PropTag.Recipient.StartMessage,
				PropTag.Recipient.EndMessage,
				PropTag.Recipient.EndAttach,
				PropTag.Recipient.EcWarning,
				PropTag.Recipient.StartFAIMessage,
				PropTag.Recipient.NewFXFolder,
				PropTag.Recipient.IncrSyncChange,
				PropTag.Recipient.IncrSyncDelete,
				PropTag.Recipient.IncrSyncEnd,
				PropTag.Recipient.IncrSyncMessage,
				PropTag.Recipient.FastTransferDelProp,
				PropTag.Recipient.IdsetGiven,
				PropTag.Recipient.IdsetGivenInt32,
				PropTag.Recipient.FastTransferErrorInfo,
				PropTag.Recipient.SoftDeletes,
				PropTag.Recipient.IdsetRead,
				PropTag.Recipient.IdsetUnread,
				PropTag.Recipient.IncrSyncRead,
				PropTag.Recipient.IncrSyncStateBegin,
				PropTag.Recipient.IncrSyncStateEnd,
				PropTag.Recipient.IncrSyncImailStream,
				PropTag.Recipient.IncrSyncImailStreamContinue,
				PropTag.Recipient.IncrSyncImailStreamCancel,
				PropTag.Recipient.IncrSyncImailStream2Continue,
				PropTag.Recipient.IncrSyncProgressMode,
				PropTag.Recipient.SyncProgressPerMsg,
				PropTag.Recipient.IncrSyncMsgPartial,
				PropTag.Recipient.IncrSyncGroupInfo,
				PropTag.Recipient.IncrSyncGroupId,
				PropTag.Recipient.IncrSyncChangePartial,
				PropTag.Recipient.CnsetSeen,
				PropTag.Recipient.CnsetRead,
				PropTag.Recipient.CnsetSeenFAI,
				PropTag.Recipient.IdSetDeleted
			};

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Recipient.DeliveryReportRequested,
				PropTag.Recipient.ReadReceiptRequested,
				PropTag.Recipient.ReportTime,
				PropTag.Recipient.DiscVal,
				PropTag.Recipient.ExplicitConversion,
				PropTag.Recipient.NDRReasonCode,
				PropTag.Recipient.NDRDiagCode,
				PropTag.Recipient.NonReceiptNotificationRequested,
				PropTag.Recipient.NonDeliveryReportRequested,
				PropTag.Recipient.OriginatorRequestedAlterateRecipient,
				PropTag.Recipient.PhysicalDeliveryMode,
				PropTag.Recipient.PhysicalDeliveryReportRequest,
				PropTag.Recipient.PhysicalForwardingAddress,
				PropTag.Recipient.PhysicalForwardingAddressRequested,
				PropTag.Recipient.PhysicalForwardingProhibited,
				PropTag.Recipient.ProofOfDelivery,
				PropTag.Recipient.ProofOfDeliveryRequested,
				PropTag.Recipient.RecipientCertificate,
				PropTag.Recipient.RecipientNumberForAdvice,
				PropTag.Recipient.RecipientType,
				PropTag.Recipient.TypeOfMTSUser,
				PropTag.Recipient.DiscreteValues,
				PropTag.Recipient.Responsibility,
				PropTag.Recipient.RecipientStatus,
				PropTag.Recipient.InstanceKey,
				PropTag.Recipient.AccessLevel,
				PropTag.Recipient.RecordKey,
				PropTag.Recipient.RecordKeySvrEid,
				PropTag.Recipient.ObjectType,
				PropTag.Recipient.EntryId,
				PropTag.Recipient.EntryIdSvrEid,
				PropTag.Recipient.RowId,
				PropTag.Recipient.DisplayName,
				PropTag.Recipient.AddressType,
				PropTag.Recipient.EmailAddress,
				PropTag.Recipient.Comment,
				PropTag.Recipient.LastModificationTime,
				PropTag.Recipient.SearchKey,
				PropTag.Recipient.SearchKeySvrEid,
				PropTag.Recipient.DetailsTable,
				PropTag.Recipient.DisplayType,
				PropTag.Recipient.SmtpAddress,
				PropTag.Recipient.SimpleDisplayName,
				PropTag.Recipient.Account,
				PropTag.Recipient.AlternateRecipient,
				PropTag.Recipient.CallbackTelephoneNumber,
				PropTag.Recipient.Generation,
				PropTag.Recipient.GivenName,
				PropTag.Recipient.GovernmentIDNumber,
				PropTag.Recipient.BusinessTelephoneNumber,
				PropTag.Recipient.HomeTelephoneNumber,
				PropTag.Recipient.Initials,
				PropTag.Recipient.Keyword,
				PropTag.Recipient.Language,
				PropTag.Recipient.Location,
				PropTag.Recipient.MailPermission,
				PropTag.Recipient.OrganizationalIDNumber,
				PropTag.Recipient.SurName,
				PropTag.Recipient.OriginalEntryId,
				PropTag.Recipient.OriginalDisplayName,
				PropTag.Recipient.OriginalSearchKey,
				PropTag.Recipient.PostalAddress,
				PropTag.Recipient.CompanyName,
				PropTag.Recipient.Title,
				PropTag.Recipient.DepartmentName,
				PropTag.Recipient.OfficeLocation,
				PropTag.Recipient.PrimaryTelephoneNumber,
				PropTag.Recipient.Business2TelephoneNumber,
				PropTag.Recipient.Business2TelephoneNumberMv,
				PropTag.Recipient.MobileTelephoneNumber,
				PropTag.Recipient.RadioTelephoneNumber,
				PropTag.Recipient.CarTelephoneNumber,
				PropTag.Recipient.OtherTelephoneNumber,
				PropTag.Recipient.TransmitableDisplayName,
				PropTag.Recipient.PagerTelephoneNumber,
				PropTag.Recipient.UserCertificate,
				PropTag.Recipient.PrimaryFaxNumber,
				PropTag.Recipient.BusinessFaxNumber,
				PropTag.Recipient.HomeFaxNumber,
				PropTag.Recipient.Country,
				PropTag.Recipient.Locality,
				PropTag.Recipient.StateOrProvince,
				PropTag.Recipient.StreetAddress,
				PropTag.Recipient.PostalCode,
				PropTag.Recipient.PostOfficeBox,
				PropTag.Recipient.TelexNumber,
				PropTag.Recipient.ISDNNumber,
				PropTag.Recipient.AssistantTelephoneNumber,
				PropTag.Recipient.Home2TelephoneNumber,
				PropTag.Recipient.Home2TelephoneNumberMv,
				PropTag.Recipient.Assistant,
				PropTag.Recipient.SendRichInfo,
				PropTag.Recipient.WeddingAnniversary,
				PropTag.Recipient.Birthday,
				PropTag.Recipient.Hobbies,
				PropTag.Recipient.MiddleName,
				PropTag.Recipient.DisplayNamePrefix,
				PropTag.Recipient.Profession,
				PropTag.Recipient.ReferredByName,
				PropTag.Recipient.SpouseName,
				PropTag.Recipient.ComputerNetworkName,
				PropTag.Recipient.CustomerId,
				PropTag.Recipient.TTYTDDPhoneNumber,
				PropTag.Recipient.FTPSite,
				PropTag.Recipient.Gender,
				PropTag.Recipient.ManagerName,
				PropTag.Recipient.NickName,
				PropTag.Recipient.PersonalHomePage,
				PropTag.Recipient.BusinessHomePage,
				PropTag.Recipient.ContactVersion,
				PropTag.Recipient.ContactEntryIds,
				PropTag.Recipient.ContactAddressTypes,
				PropTag.Recipient.ContactDefaultAddressIndex,
				PropTag.Recipient.ContactEmailAddress,
				PropTag.Recipient.CompanyMainPhoneNumber,
				PropTag.Recipient.ChildrensNames,
				PropTag.Recipient.HomeAddressCity,
				PropTag.Recipient.HomeAddressCountry,
				PropTag.Recipient.HomeAddressPostalCode,
				PropTag.Recipient.HomeAddressStateOrProvince,
				PropTag.Recipient.HomeAddressStreet,
				PropTag.Recipient.HomeAddressPostOfficeBox,
				PropTag.Recipient.OtherAddressCity,
				PropTag.Recipient.OtherAddressCountry,
				PropTag.Recipient.OtherAddressPostalCode,
				PropTag.Recipient.OtherAddressStateOrProvince,
				PropTag.Recipient.OtherAddressStreet,
				PropTag.Recipient.OtherAddressPostOfficeBox,
				PropTag.Recipient.UserX509CertificateABSearchPath,
				PropTag.Recipient.SendInternetEncoding,
				PropTag.Recipient.PartnerNetworkId,
				PropTag.Recipient.PartnerNetworkUserId,
				PropTag.Recipient.PartnerNetworkThumbnailPhotoUrl,
				PropTag.Recipient.PartnerNetworkProfilePhotoUrl,
				PropTag.Recipient.PartnerNetworkContactType,
				PropTag.Recipient.RelevanceScore,
				PropTag.Recipient.IsDistributionListContact,
				PropTag.Recipient.IsPromotedContact,
				PropTag.Recipient.OrgUnitName,
				PropTag.Recipient.OrganizationName,
				PropTag.Recipient.TestBlobProperty,
				PropTag.Recipient.NewAttach,
				PropTag.Recipient.StartEmbed,
				PropTag.Recipient.EndEmbed,
				PropTag.Recipient.StartRecip,
				PropTag.Recipient.EndRecip,
				PropTag.Recipient.EndCcRecip,
				PropTag.Recipient.EndBccRecip,
				PropTag.Recipient.EndP1Recip,
				PropTag.Recipient.DNPrefix,
				PropTag.Recipient.StartTopFolder,
				PropTag.Recipient.StartSubFolder,
				PropTag.Recipient.EndFolder,
				PropTag.Recipient.StartMessage,
				PropTag.Recipient.EndMessage,
				PropTag.Recipient.EndAttach,
				PropTag.Recipient.EcWarning,
				PropTag.Recipient.StartFAIMessage,
				PropTag.Recipient.NewFXFolder,
				PropTag.Recipient.IncrSyncChange,
				PropTag.Recipient.IncrSyncDelete,
				PropTag.Recipient.IncrSyncEnd,
				PropTag.Recipient.IncrSyncMessage,
				PropTag.Recipient.FastTransferDelProp,
				PropTag.Recipient.IdsetGiven,
				PropTag.Recipient.IdsetGivenInt32,
				PropTag.Recipient.FastTransferErrorInfo,
				PropTag.Recipient.SoftDeletes,
				PropTag.Recipient.IdsetRead,
				PropTag.Recipient.IdsetUnread,
				PropTag.Recipient.IncrSyncRead,
				PropTag.Recipient.IncrSyncStateBegin,
				PropTag.Recipient.IncrSyncStateEnd,
				PropTag.Recipient.IncrSyncImailStream,
				PropTag.Recipient.IncrSyncImailStreamContinue,
				PropTag.Recipient.IncrSyncImailStreamCancel,
				PropTag.Recipient.IncrSyncImailStream2Continue,
				PropTag.Recipient.IncrSyncProgressMode,
				PropTag.Recipient.SyncProgressPerMsg,
				PropTag.Recipient.IncrSyncMsgPartial,
				PropTag.Recipient.IncrSyncGroupInfo,
				PropTag.Recipient.IncrSyncGroupId,
				PropTag.Recipient.IncrSyncChangePartial,
				PropTag.Recipient.RecipientOrder,
				PropTag.Recipient.RecipientSipUri,
				PropTag.Recipient.RecipientDisplayName,
				PropTag.Recipient.RecipientEntryId,
				PropTag.Recipient.RecipientFlags,
				PropTag.Recipient.RecipientTrackStatus,
				PropTag.Recipient.DotStuffState,
				PropTag.Recipient.RecipientNumber,
				PropTag.Recipient.UserDN,
				PropTag.Recipient.CnsetSeen,
				PropTag.Recipient.SourceEntryId,
				PropTag.Recipient.CnsetRead,
				PropTag.Recipient.CnsetSeenFAI,
				PropTag.Recipient.IdSetDeleted
			};
		}

		public static class Event
		{
			public static readonly StorePropTag EventMailboxGuid = new StorePropTag(26474, PropertyType.Binary, new StorePropInfo("EventMailboxGuid", 26474, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventCounter = new StorePropTag(26631, PropertyType.Int64, new StorePropInfo("EventCounter", 26631, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventMask = new StorePropTag(26632, PropertyType.Int32, new StorePropInfo("EventMask", 26632, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFid = new StorePropTag(26633, PropertyType.Binary, new StorePropInfo("EventFid", 26633, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventMid = new StorePropTag(26634, PropertyType.Binary, new StorePropInfo("EventMid", 26634, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFidParent = new StorePropTag(26635, PropertyType.Binary, new StorePropInfo("EventFidParent", 26635, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFidOld = new StorePropTag(26636, PropertyType.Binary, new StorePropInfo("EventFidOld", 26636, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventMidOld = new StorePropTag(26637, PropertyType.Binary, new StorePropInfo("EventMidOld", 26637, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFidOldParent = new StorePropTag(26638, PropertyType.Binary, new StorePropInfo("EventFidOldParent", 26638, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventCreatedTime = new StorePropTag(26639, PropertyType.SysTime, new StorePropInfo("EventCreatedTime", 26639, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventMessageClass = new StorePropTag(26640, PropertyType.Unicode, new StorePropInfo("EventMessageClass", 26640, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventItemCount = new StorePropTag(26641, PropertyType.Int32, new StorePropInfo("EventItemCount", 26641, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFidRoot = new StorePropTag(26642, PropertyType.Binary, new StorePropInfo("EventFidRoot", 26642, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventUnreadCount = new StorePropTag(26643, PropertyType.Int32, new StorePropInfo("EventUnreadCount", 26643, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventTransacId = new StorePropTag(26644, PropertyType.Int32, new StorePropInfo("EventTransacId", 26644, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventFlags = new StorePropTag(26645, PropertyType.Int32, new StorePropInfo("EventFlags", 26645, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventExtendedFlags = new StorePropTag(26648, PropertyType.Int64, new StorePropInfo("EventExtendedFlags", 26648, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventClientType = new StorePropTag(26649, PropertyType.Int32, new StorePropInfo("EventClientType", 26649, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventSid = new StorePropTag(26650, PropertyType.Binary, new StorePropInfo("EventSid", 26650, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag EventDocId = new StorePropTag(26651, PropertyType.Int32, new StorePropInfo("EventDocId", 26651, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.Event);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.Event.EventMailboxGuid,
				PropTag.Event.EventCounter,
				PropTag.Event.EventMask,
				PropTag.Event.EventFid,
				PropTag.Event.EventMid,
				PropTag.Event.EventFidParent,
				PropTag.Event.EventFidOld,
				PropTag.Event.EventMidOld,
				PropTag.Event.EventFidOldParent,
				PropTag.Event.EventCreatedTime,
				PropTag.Event.EventMessageClass,
				PropTag.Event.EventItemCount,
				PropTag.Event.EventFidRoot,
				PropTag.Event.EventUnreadCount,
				PropTag.Event.EventTransacId,
				PropTag.Event.EventFlags,
				PropTag.Event.EventExtendedFlags,
				PropTag.Event.EventClientType,
				PropTag.Event.EventSid,
				PropTag.Event.EventDocId,
				PropTag.Event.MailboxNum
			};
		}

		public static class PermissionView
		{
			public static readonly StorePropTag EntryId = new StorePropTag(4095, PropertyType.Binary, new StorePropInfo("EntryId", 4095, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag MemberId = new StorePropTag(26225, PropertyType.Int64, new StorePropInfo("MemberId", 26225, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag MemberName = new StorePropTag(26226, PropertyType.Unicode, new StorePropInfo("MemberName", 26226, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag MemberRights = new StorePropTag(26227, PropertyType.Int32, new StorePropInfo("MemberRights", 26227, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag MemberSecurityIdentifier = new StorePropTag(26228, PropertyType.Binary, new StorePropInfo("MemberSecurityIdentifier", 26228, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag MemberIsGroup = new StorePropTag(26229, PropertyType.Boolean, new StorePropInfo("MemberIsGroup", 26229, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.PermissionView);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.PermissionView.EntryId,
				PropTag.PermissionView.MemberId,
				PropTag.PermissionView.MemberName,
				PropTag.PermissionView.MemberRights,
				PropTag.PermissionView.MemberSecurityIdentifier,
				PropTag.PermissionView.MemberIsGroup
			};
		}

		public static class ViewDefinition
		{
			public static readonly StorePropTag SortOrder = new StorePropTag(26465, PropertyType.Binary, new StorePropInfo("SortOrder", 26465, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag LCID = new StorePropTag(26478, PropertyType.Int32, new StorePropInfo("LCID", 26478, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag ViewCoveringPropertyTags = new StorePropTag(26610, PropertyType.MVInt32, new StorePropInfo("ViewCoveringPropertyTags", 26610, PropertyType.MVInt32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag ViewAccessTime = new StorePropTag(26611, PropertyType.SysTime, new StorePropInfo("ViewAccessTime", 26611, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag ICSViewFilter = new StorePropTag(26612, PropertyType.Boolean, new StorePropInfo("ICSViewFilter", 26612, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag SoftDeletedFilter = new StorePropTag(26649, PropertyType.Boolean, new StorePropInfo("SoftDeletedFilter", 26649, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag AssociatedFilter = new StorePropTag(26650, PropertyType.Boolean, new StorePropInfo("AssociatedFilter", 26650, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag ConversationsFilter = new StorePropTag(26651, PropertyType.Boolean, new StorePropInfo("ConversationsFilter", 26651, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag CategCount = new StorePropTag(26782, PropertyType.Int32, new StorePropInfo("CategCount", 26782, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ViewDefinition);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.ViewDefinition.SortOrder,
				PropTag.ViewDefinition.LCID,
				PropTag.ViewDefinition.ViewCoveringPropertyTags,
				PropTag.ViewDefinition.ViewAccessTime,
				PropTag.ViewDefinition.ICSViewFilter,
				PropTag.ViewDefinition.SoftDeletedFilter,
				PropTag.ViewDefinition.AssociatedFilter,
				PropTag.ViewDefinition.ConversationsFilter,
				PropTag.ViewDefinition.CategCount
			};
		}

		public static class RestrictionView
		{
			public static readonly StorePropTag EntryId = new StorePropTag(4095, PropertyType.Binary, new StorePropInfo("EntryId", 4095, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag ContentCount = new StorePropTag(13826, PropertyType.Int32, new StorePropInfo("ContentCount", 13826, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag UnreadCount = new StorePropTag(13827, PropertyType.Int32, new StorePropInfo("UnreadCount", 13827, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag LCIDRestriction = new StorePropTag(26504, PropertyType.Int32, new StorePropInfo("LCIDRestriction", 26504, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag ViewRestriction = new StorePropTag(26544, PropertyType.SRestriction, new StorePropInfo("ViewRestriction", 26544, PropertyType.SRestriction, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag ViewAccessTime = new StorePropTag(26611, PropertyType.SysTime, new StorePropInfo("ViewAccessTime", 26611, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.RestrictionView);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.RestrictionView.EntryId,
				PropTag.RestrictionView.DisplayName,
				PropTag.RestrictionView.ContentCount,
				PropTag.RestrictionView.UnreadCount,
				PropTag.RestrictionView.LCIDRestriction,
				PropTag.RestrictionView.ViewRestriction,
				PropTag.RestrictionView.ViewAccessTime
			};
		}

		public static class LocalDirectory
		{
			public static readonly StorePropTag LocalDirectoryEntryID = new StorePropTag(13334, PropertyType.Binary, new StorePropInfo("LocalDirectoryEntryID", 13334, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.LocalDirectory);

			public static readonly StorePropTag MemberName = new StorePropTag(26226, PropertyType.Unicode, new StorePropInfo("MemberName", 26226, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.LocalDirectory);

			public static readonly StorePropTag MemberEmail = new StorePropTag(26665, PropertyType.Unicode, new StorePropInfo("MemberEmail", 26665, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.LocalDirectory);

			public static readonly StorePropTag MemberExternalId = new StorePropTag(26666, PropertyType.Unicode, new StorePropInfo("MemberExternalId", 26666, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.LocalDirectory);

			public static readonly StorePropTag MemberSID = new StorePropTag(26667, PropertyType.Binary, new StorePropInfo("MemberSID", 26667, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.LocalDirectory);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.LocalDirectory.LocalDirectoryEntryID,
				PropTag.LocalDirectory.MemberName,
				PropTag.LocalDirectory.MemberEmail,
				PropTag.LocalDirectory.MemberExternalId,
				PropTag.LocalDirectory.MemberSID
			};
		}

		public static class ResourceDigest
		{
			public static readonly StorePropTag DisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("DisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag MailboxDSGuid = new StorePropTag(26375, PropertyType.Binary, new StorePropInfo("MailboxDSGuid", 26375, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag TimeInServer = new StorePropTag(26413, PropertyType.Int32, new StorePropInfo("TimeInServer", 26413, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag TimeInCpu = new StorePropTag(26414, PropertyType.Int32, new StorePropInfo("TimeInCpu", 26414, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag RopCount = new StorePropTag(26415, PropertyType.Int32, new StorePropInfo("RopCount", 26415, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag PageRead = new StorePropTag(26416, PropertyType.Int32, new StorePropInfo("PageRead", 26416, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag PagePreread = new StorePropTag(26417, PropertyType.Int32, new StorePropInfo("PagePreread", 26417, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag LogRecordCount = new StorePropTag(26418, PropertyType.Int32, new StorePropInfo("LogRecordCount", 26418, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag LogRecordBytes = new StorePropTag(26419, PropertyType.Int32, new StorePropInfo("LogRecordBytes", 26419, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag LdapReads = new StorePropTag(26420, PropertyType.Int32, new StorePropInfo("LdapReads", 26420, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag LdapSearches = new StorePropTag(26421, PropertyType.Int32, new StorePropInfo("LdapSearches", 26421, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag DigestCategory = new StorePropTag(26422, PropertyType.Unicode, new StorePropInfo("DigestCategory", 26422, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag SampleId = new StorePropTag(26423, PropertyType.Int32, new StorePropInfo("SampleId", 26423, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag SampleTime = new StorePropTag(26424, PropertyType.SysTime, new StorePropInfo("SampleTime", 26424, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag MailboxQuarantined = new StorePropTag(26650, PropertyType.Boolean, new StorePropInfo("MailboxQuarantined", 26650, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ResourceDigest);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.ResourceDigest.DisplayName,
				PropTag.ResourceDigest.MailboxDSGuid,
				PropTag.ResourceDigest.TimeInServer,
				PropTag.ResourceDigest.TimeInCpu,
				PropTag.ResourceDigest.RopCount,
				PropTag.ResourceDigest.PageRead,
				PropTag.ResourceDigest.PagePreread,
				PropTag.ResourceDigest.LogRecordCount,
				PropTag.ResourceDigest.LogRecordBytes,
				PropTag.ResourceDigest.LdapReads,
				PropTag.ResourceDigest.LdapSearches,
				PropTag.ResourceDigest.DigestCategory,
				PropTag.ResourceDigest.SampleId,
				PropTag.ResourceDigest.SampleTime,
				PropTag.ResourceDigest.MailboxQuarantined
			};
		}

		public static class IcsState
		{
			public static readonly StorePropTag IdsetGiven = new StorePropTag(16407, PropertyType.Binary, new StorePropInfo("IdsetGiven", 16407, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IcsState);

			public static readonly StorePropTag IdsetGivenInt32 = new StorePropTag(16407, PropertyType.Int32, new StorePropInfo("IdsetGivenInt32", 16407, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IcsState);

			public static readonly StorePropTag CnsetSeen = new StorePropTag(26518, PropertyType.Binary, new StorePropInfo("CnsetSeen", 26518, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IcsState);

			public static readonly StorePropTag CnsetRead = new StorePropTag(26578, PropertyType.Binary, new StorePropInfo("CnsetRead", 26578, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IcsState);

			public static readonly StorePropTag CnsetSeenFAI = new StorePropTag(26586, PropertyType.Binary, new StorePropInfo("CnsetSeenFAI", 26586, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IcsState);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.IcsState.IdsetGiven,
				PropTag.IcsState.IdsetGivenInt32,
				PropTag.IcsState.CnsetSeen,
				PropTag.IcsState.CnsetRead,
				PropTag.IcsState.CnsetSeenFAI
			};
		}

		public static class InferenceLog
		{
			public static readonly StorePropTag RowId = new StorePropTag(12288, PropertyType.Int32, new StorePropInfo("RowId", 12288, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag MailboxPartitionNumber = new StorePropTag(15775, PropertyType.Int32, new StorePropInfo("MailboxPartitionNumber", 15775, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag MailboxNumberInternal = new StorePropTag(15776, PropertyType.Int32, new StorePropInfo("MailboxNumberInternal", 15776, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag Mid = new StorePropTag(26442, PropertyType.Int64, new StorePropInfo("Mid", 26442, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag MailboxNum = new StorePropTag(26655, PropertyType.Int32, new StorePropInfo("MailboxNum", 26655, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceActivityId = new StorePropTag(26656, PropertyType.Int32, new StorePropInfo("InferenceActivityId", 26656, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceClientId = new StorePropTag(26657, PropertyType.Int32, new StorePropInfo("InferenceClientId", 26657, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceItemId = new StorePropTag(26658, PropertyType.Binary, new StorePropInfo("InferenceItemId", 26658, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceTimeStamp = new StorePropTag(26659, PropertyType.SysTime, new StorePropInfo("InferenceTimeStamp", 26659, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceWindowId = new StorePropTag(26660, PropertyType.Guid, new StorePropInfo("InferenceWindowId", 26660, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceSessionId = new StorePropTag(26661, PropertyType.Guid, new StorePropInfo("InferenceSessionId", 26661, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceFolderId = new StorePropTag(26662, PropertyType.Binary, new StorePropInfo("InferenceFolderId", 26662, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceOofEnabled = new StorePropTag(26663, PropertyType.Boolean, new StorePropInfo("InferenceOofEnabled", 26663, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceDeleteType = new StorePropTag(26664, PropertyType.Int32, new StorePropInfo("InferenceDeleteType", 26664, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceBrowser = new StorePropTag(26665, PropertyType.Unicode, new StorePropInfo("InferenceBrowser", 26665, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceLocaleId = new StorePropTag(26666, PropertyType.Int32, new StorePropInfo("InferenceLocaleId", 26666, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceLocation = new StorePropTag(26667, PropertyType.Unicode, new StorePropInfo("InferenceLocation", 26667, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceConversationId = new StorePropTag(26668, PropertyType.Binary, new StorePropInfo("InferenceConversationId", 26668, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceIpAddress = new StorePropTag(26669, PropertyType.Unicode, new StorePropInfo("InferenceIpAddress", 26669, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceTimeZone = new StorePropTag(26670, PropertyType.Unicode, new StorePropInfo("InferenceTimeZone", 26670, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceCategory = new StorePropTag(26671, PropertyType.Unicode, new StorePropInfo("InferenceCategory", 26671, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceAttachmentId = new StorePropTag(26672, PropertyType.Binary, new StorePropInfo("InferenceAttachmentId", 26672, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceGlobalObjectId = new StorePropTag(26673, PropertyType.Binary, new StorePropInfo("InferenceGlobalObjectId", 26673, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceModuleSelected = new StorePropTag(26674, PropertyType.Int32, new StorePropInfo("InferenceModuleSelected", 26674, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag InferenceLayoutType = new StorePropTag(26675, PropertyType.Unicode, new StorePropInfo("InferenceLayoutType", 26675, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.InferenceLog);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.InferenceLog.RowId,
				PropTag.InferenceLog.MailboxPartitionNumber,
				PropTag.InferenceLog.MailboxNumberInternal,
				PropTag.InferenceLog.Mid,
				PropTag.InferenceLog.MailboxNum,
				PropTag.InferenceLog.InferenceActivityId,
				PropTag.InferenceLog.InferenceClientId,
				PropTag.InferenceLog.InferenceItemId,
				PropTag.InferenceLog.InferenceTimeStamp,
				PropTag.InferenceLog.InferenceWindowId,
				PropTag.InferenceLog.InferenceSessionId,
				PropTag.InferenceLog.InferenceFolderId,
				PropTag.InferenceLog.InferenceOofEnabled,
				PropTag.InferenceLog.InferenceDeleteType,
				PropTag.InferenceLog.InferenceBrowser,
				PropTag.InferenceLog.InferenceLocaleId,
				PropTag.InferenceLog.InferenceLocation,
				PropTag.InferenceLog.InferenceConversationId,
				PropTag.InferenceLog.InferenceIpAddress,
				PropTag.InferenceLog.InferenceTimeZone,
				PropTag.InferenceLog.InferenceCategory,
				PropTag.InferenceLog.InferenceAttachmentId,
				PropTag.InferenceLog.InferenceGlobalObjectId,
				PropTag.InferenceLog.InferenceModuleSelected,
				PropTag.InferenceLog.InferenceLayoutType
			};
		}

		public static class ProcessInfo
		{
			public static readonly StorePropTag WorkerProcessId = new StorePropTag(26263, PropertyType.Int32, new StorePropInfo("WorkerProcessId", 26263, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ProcessInfo);

			public static readonly StorePropTag MinimumDatabaseSchemaVersion = new StorePropTag(26264, PropertyType.Int32, new StorePropInfo("MinimumDatabaseSchemaVersion", 26264, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ProcessInfo);

			public static readonly StorePropTag MaximumDatabaseSchemaVersion = new StorePropTag(26265, PropertyType.Int32, new StorePropInfo("MaximumDatabaseSchemaVersion", 26265, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ProcessInfo);

			public static readonly StorePropTag CurrentDatabaseSchemaVersion = new StorePropTag(26266, PropertyType.Int32, new StorePropInfo("CurrentDatabaseSchemaVersion", 26266, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ProcessInfo);

			public static readonly StorePropTag RequestedDatabaseSchemaVersion = new StorePropTag(26267, PropertyType.Int32, new StorePropInfo("RequestedDatabaseSchemaVersion", 26267, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.ProcessInfo);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.ProcessInfo.WorkerProcessId,
				PropTag.ProcessInfo.MinimumDatabaseSchemaVersion,
				PropTag.ProcessInfo.MaximumDatabaseSchemaVersion,
				PropTag.ProcessInfo.CurrentDatabaseSchemaVersion,
				PropTag.ProcessInfo.RequestedDatabaseSchemaVersion
			};
		}

		public static class FastTransferStream
		{
			public static readonly StorePropTag InstanceGuid = new StorePropTag(26653, PropertyType.Guid, new StorePropInfo("InstanceGuid", 26653, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.FastTransferStream);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.FastTransferStream.InstanceGuid
			};
		}

		public static class IsIntegJob
		{
			public static readonly StorePropTag IsIntegJobMailboxGuid = new StorePropTag(4096, PropertyType.Guid, new StorePropInfo("IsIntegJobMailboxGuid", 4096, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobGuid = new StorePropTag(4097, PropertyType.Guid, new StorePropInfo("IsIntegJobGuid", 4097, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobFlags = new StorePropTag(4098, PropertyType.Int32, new StorePropInfo("IsIntegJobFlags", 4098, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobTask = new StorePropTag(4099, PropertyType.Int32, new StorePropInfo("IsIntegJobTask", 4099, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobState = new StorePropTag(4100, PropertyType.Int16, new StorePropInfo("IsIntegJobState", 4100, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobCreationTime = new StorePropTag(4101, PropertyType.SysTime, new StorePropInfo("IsIntegJobCreationTime", 4101, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobCompletedTime = new StorePropTag(4102, PropertyType.SysTime, new StorePropInfo("IsIntegJobCompletedTime", 4102, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobLastExecutionTime = new StorePropTag(4103, PropertyType.SysTime, new StorePropInfo("IsIntegJobLastExecutionTime", 4103, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobCorruptionsDetected = new StorePropTag(4104, PropertyType.Int32, new StorePropInfo("IsIntegJobCorruptionsDetected", 4104, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobCorruptionsFixed = new StorePropTag(4105, PropertyType.Int32, new StorePropInfo("IsIntegJobCorruptionsFixed", 4105, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobRequestGuid = new StorePropTag(4106, PropertyType.Guid, new StorePropInfo("IsIntegJobRequestGuid", 4106, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobProgress = new StorePropTag(4107, PropertyType.Int16, new StorePropInfo("IsIntegJobProgress", 4107, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobCorruptions = new StorePropTag(4108, PropertyType.Binary, new StorePropInfo("IsIntegJobCorruptions", 4108, PropertyType.Binary, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobSource = new StorePropTag(4109, PropertyType.Int16, new StorePropInfo("IsIntegJobSource", 4109, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobPriority = new StorePropTag(4110, PropertyType.Int16, new StorePropInfo("IsIntegJobPriority", 4110, PropertyType.Int16, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobTimeInServer = new StorePropTag(4111, PropertyType.Real64, new StorePropInfo("IsIntegJobTimeInServer", 4111, PropertyType.Real64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobMailboxNumber = new StorePropTag(4112, PropertyType.Int32, new StorePropInfo("IsIntegJobMailboxNumber", 4112, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag IsIntegJobError = new StorePropTag(4113, PropertyType.Int32, new StorePropInfo("IsIntegJobError", 4113, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.IsIntegJob);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.IsIntegJob.IsIntegJobMailboxGuid,
				PropTag.IsIntegJob.IsIntegJobGuid,
				PropTag.IsIntegJob.IsIntegJobFlags,
				PropTag.IsIntegJob.IsIntegJobTask,
				PropTag.IsIntegJob.IsIntegJobState,
				PropTag.IsIntegJob.IsIntegJobCreationTime,
				PropTag.IsIntegJob.IsIntegJobCompletedTime,
				PropTag.IsIntegJob.IsIntegJobLastExecutionTime,
				PropTag.IsIntegJob.IsIntegJobCorruptionsDetected,
				PropTag.IsIntegJob.IsIntegJobCorruptionsFixed,
				PropTag.IsIntegJob.IsIntegJobRequestGuid,
				PropTag.IsIntegJob.IsIntegJobProgress,
				PropTag.IsIntegJob.IsIntegJobCorruptions,
				PropTag.IsIntegJob.IsIntegJobSource,
				PropTag.IsIntegJob.IsIntegJobPriority,
				PropTag.IsIntegJob.IsIntegJobTimeInServer,
				PropTag.IsIntegJob.IsIntegJobMailboxNumber,
				PropTag.IsIntegJob.IsIntegJobError
			};
		}

		public static class UserInfo
		{
			public static readonly StorePropTag UserInformationGuid = new StorePropTag(12288, PropertyType.Guid, new StorePropInfo("UserInformationGuid", 12288, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDisplayName = new StorePropTag(12289, PropertyType.Unicode, new StorePropInfo("UserInformationDisplayName", 12289, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCreationTime = new StorePropTag(12290, PropertyType.SysTime, new StorePropInfo("UserInformationCreationTime", 12290, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLastModificationTime = new StorePropTag(12291, PropertyType.SysTime, new StorePropInfo("UserInformationLastModificationTime", 12291, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationChangeNumber = new StorePropTag(12292, PropertyType.Int64, new StorePropInfo("UserInformationChangeNumber", 12292, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, new PropertyCategories(3, 4)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLastInteractiveLogonTime = new StorePropTag(12293, PropertyType.SysTime, new StorePropInfo("UserInformationLastInteractiveLogonTime", 12293, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationActiveSyncAllowedDeviceIDs = new StorePropTag(12294, PropertyType.MVUnicode, new StorePropInfo("UserInformationActiveSyncAllowedDeviceIDs", 12294, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationActiveSyncBlockedDeviceIDs = new StorePropTag(12295, PropertyType.MVUnicode, new StorePropInfo("UserInformationActiveSyncBlockedDeviceIDs", 12295, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationActiveSyncDebugLogging = new StorePropTag(12296, PropertyType.Int32, new StorePropInfo("UserInformationActiveSyncDebugLogging", 12296, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationActiveSyncEnabled = new StorePropTag(12297, PropertyType.Boolean, new StorePropInfo("UserInformationActiveSyncEnabled", 12297, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAdminDisplayName = new StorePropTag(12298, PropertyType.Unicode, new StorePropInfo("UserInformationAdminDisplayName", 12298, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAggregationSubscriptionCredential = new StorePropTag(12299, PropertyType.MVUnicode, new StorePropInfo("UserInformationAggregationSubscriptionCredential", 12299, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAllowArchiveAddressSync = new StorePropTag(12300, PropertyType.Boolean, new StorePropInfo("UserInformationAllowArchiveAddressSync", 12300, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAltitude = new StorePropTag(12301, PropertyType.Int32, new StorePropInfo("UserInformationAltitude", 12301, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAntispamBypassEnabled = new StorePropTag(12302, PropertyType.Boolean, new StorePropInfo("UserInformationAntispamBypassEnabled", 12302, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveDomain = new StorePropTag(12303, PropertyType.Unicode, new StorePropInfo("UserInformationArchiveDomain", 12303, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveGuid = new StorePropTag(12304, PropertyType.Guid, new StorePropInfo("UserInformationArchiveGuid", 12304, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveName = new StorePropTag(12305, PropertyType.MVUnicode, new StorePropInfo("UserInformationArchiveName", 12305, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveQuota = new StorePropTag(12306, PropertyType.Unicode, new StorePropInfo("UserInformationArchiveQuota", 12306, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveRelease = new StorePropTag(12307, PropertyType.Unicode, new StorePropInfo("UserInformationArchiveRelease", 12307, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveStatus = new StorePropTag(12308, PropertyType.Int32, new StorePropInfo("UserInformationArchiveStatus", 12308, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationArchiveWarningQuota = new StorePropTag(12309, PropertyType.Unicode, new StorePropInfo("UserInformationArchiveWarningQuota", 12309, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAssistantName = new StorePropTag(12310, PropertyType.Unicode, new StorePropInfo("UserInformationAssistantName", 12310, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationBirthdate = new StorePropTag(12311, PropertyType.SysTime, new StorePropInfo("UserInformationBirthdate", 12311, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationBypassNestedModerationEnabled = new StorePropTag(12312, PropertyType.Boolean, new StorePropInfo("UserInformationBypassNestedModerationEnabled", 12312, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationC = new StorePropTag(12313, PropertyType.Unicode, new StorePropInfo("UserInformationC", 12313, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCalendarLoggingQuota = new StorePropTag(12314, PropertyType.Unicode, new StorePropInfo("UserInformationCalendarLoggingQuota", 12314, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCalendarRepairDisabled = new StorePropTag(12315, PropertyType.Boolean, new StorePropInfo("UserInformationCalendarRepairDisabled", 12315, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCalendarVersionStoreDisabled = new StorePropTag(12316, PropertyType.Boolean, new StorePropInfo("UserInformationCalendarVersionStoreDisabled", 12316, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCity = new StorePropTag(12317, PropertyType.Unicode, new StorePropInfo("UserInformationCity", 12317, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCountry = new StorePropTag(12318, PropertyType.Unicode, new StorePropInfo("UserInformationCountry", 12318, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCountryCode = new StorePropTag(12319, PropertyType.Int32, new StorePropInfo("UserInformationCountryCode", 12319, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationCountryOrRegion = new StorePropTag(12320, PropertyType.Unicode, new StorePropInfo("UserInformationCountryOrRegion", 12320, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDefaultMailTip = new StorePropTag(12321, PropertyType.Unicode, new StorePropInfo("UserInformationDefaultMailTip", 12321, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDeliverToMailboxAndForward = new StorePropTag(12322, PropertyType.Boolean, new StorePropInfo("UserInformationDeliverToMailboxAndForward", 12322, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDescription = new StorePropTag(12323, PropertyType.MVUnicode, new StorePropInfo("UserInformationDescription", 12323, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDisabledArchiveGuid = new StorePropTag(12324, PropertyType.Guid, new StorePropInfo("UserInformationDisabledArchiveGuid", 12324, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationDowngradeHighPriorityMessagesEnabled = new StorePropTag(12325, PropertyType.Boolean, new StorePropInfo("UserInformationDowngradeHighPriorityMessagesEnabled", 12325, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationECPEnabled = new StorePropTag(12326, PropertyType.Boolean, new StorePropInfo("UserInformationECPEnabled", 12326, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEmailAddressPolicyEnabled = new StorePropTag(12327, PropertyType.Boolean, new StorePropInfo("UserInformationEmailAddressPolicyEnabled", 12327, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsAllowEntourage = new StorePropTag(12328, PropertyType.Boolean, new StorePropInfo("UserInformationEwsAllowEntourage", 12328, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsAllowMacOutlook = new StorePropTag(12329, PropertyType.Boolean, new StorePropInfo("UserInformationEwsAllowMacOutlook", 12329, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsAllowOutlook = new StorePropTag(12330, PropertyType.Boolean, new StorePropInfo("UserInformationEwsAllowOutlook", 12330, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsApplicationAccessPolicy = new StorePropTag(12331, PropertyType.Int32, new StorePropInfo("UserInformationEwsApplicationAccessPolicy", 12331, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsEnabled = new StorePropTag(12332, PropertyType.Int32, new StorePropInfo("UserInformationEwsEnabled", 12332, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsExceptions = new StorePropTag(12333, PropertyType.MVUnicode, new StorePropInfo("UserInformationEwsExceptions", 12333, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEwsWellKnownApplicationAccessPolicies = new StorePropTag(12334, PropertyType.MVUnicode, new StorePropInfo("UserInformationEwsWellKnownApplicationAccessPolicies", 12334, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationExchangeGuid = new StorePropTag(12335, PropertyType.Guid, new StorePropInfo("UserInformationExchangeGuid", 12335, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationExternalOofOptions = new StorePropTag(12336, PropertyType.Int32, new StorePropInfo("UserInformationExternalOofOptions", 12336, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationFirstName = new StorePropTag(12337, PropertyType.Unicode, new StorePropInfo("UserInformationFirstName", 12337, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationForwardingSmtpAddress = new StorePropTag(12338, PropertyType.Unicode, new StorePropInfo("UserInformationForwardingSmtpAddress", 12338, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationGender = new StorePropTag(12339, PropertyType.Unicode, new StorePropInfo("UserInformationGender", 12339, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationGenericForwardingAddress = new StorePropTag(12340, PropertyType.Unicode, new StorePropInfo("UserInformationGenericForwardingAddress", 12340, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationGeoCoordinates = new StorePropTag(12341, PropertyType.Unicode, new StorePropInfo("UserInformationGeoCoordinates", 12341, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationHABSeniorityIndex = new StorePropTag(12342, PropertyType.Int32, new StorePropInfo("UserInformationHABSeniorityIndex", 12342, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationHasActiveSyncDevicePartnership = new StorePropTag(12343, PropertyType.Boolean, new StorePropInfo("UserInformationHasActiveSyncDevicePartnership", 12343, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationHiddenFromAddressListsEnabled = new StorePropTag(12344, PropertyType.Boolean, new StorePropInfo("UserInformationHiddenFromAddressListsEnabled", 12344, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationHiddenFromAddressListsValue = new StorePropTag(12345, PropertyType.Boolean, new StorePropInfo("UserInformationHiddenFromAddressListsValue", 12345, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationHomePhone = new StorePropTag(12346, PropertyType.Unicode, new StorePropInfo("UserInformationHomePhone", 12346, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapEnabled = new StorePropTag(12347, PropertyType.Boolean, new StorePropInfo("UserInformationImapEnabled", 12347, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapEnableExactRFC822Size = new StorePropTag(12348, PropertyType.Boolean, new StorePropInfo("UserInformationImapEnableExactRFC822Size", 12348, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapForceICalForCalendarRetrievalOption = new StorePropTag(12349, PropertyType.Boolean, new StorePropInfo("UserInformationImapForceICalForCalendarRetrievalOption", 12349, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapMessagesRetrievalMimeFormat = new StorePropTag(12350, PropertyType.Int32, new StorePropInfo("UserInformationImapMessagesRetrievalMimeFormat", 12350, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapProtocolLoggingEnabled = new StorePropTag(12351, PropertyType.Int32, new StorePropInfo("UserInformationImapProtocolLoggingEnabled", 12351, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapSuppressReadReceipt = new StorePropTag(12352, PropertyType.Boolean, new StorePropInfo("UserInformationImapSuppressReadReceipt", 12352, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationImapUseProtocolDefaults = new StorePropTag(12353, PropertyType.Boolean, new StorePropInfo("UserInformationImapUseProtocolDefaults", 12353, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIncludeInGarbageCollection = new StorePropTag(12354, PropertyType.Boolean, new StorePropInfo("UserInformationIncludeInGarbageCollection", 12354, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationInitials = new StorePropTag(12355, PropertyType.Unicode, new StorePropInfo("UserInformationInitials", 12355, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationInPlaceHolds = new StorePropTag(12356, PropertyType.MVUnicode, new StorePropInfo("UserInformationInPlaceHolds", 12356, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationInternalOnly = new StorePropTag(12357, PropertyType.Boolean, new StorePropInfo("UserInformationInternalOnly", 12357, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationInternalUsageLocation = new StorePropTag(12358, PropertyType.Unicode, new StorePropInfo("UserInformationInternalUsageLocation", 12358, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationInternetEncoding = new StorePropTag(12359, PropertyType.Int32, new StorePropInfo("UserInformationInternetEncoding", 12359, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsCalculatedTargetAddress = new StorePropTag(12360, PropertyType.Boolean, new StorePropInfo("UserInformationIsCalculatedTargetAddress", 12360, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsExcludedFromServingHierarchy = new StorePropTag(12361, PropertyType.Boolean, new StorePropInfo("UserInformationIsExcludedFromServingHierarchy", 12361, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsHierarchyReady = new StorePropTag(12362, PropertyType.Boolean, new StorePropInfo("UserInformationIsHierarchyReady", 12362, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsInactiveMailbox = new StorePropTag(12363, PropertyType.Boolean, new StorePropInfo("UserInformationIsInactiveMailbox", 12363, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsSoftDeletedByDisable = new StorePropTag(12364, PropertyType.Boolean, new StorePropInfo("UserInformationIsSoftDeletedByDisable", 12364, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsSoftDeletedByRemove = new StorePropTag(12365, PropertyType.Boolean, new StorePropInfo("UserInformationIsSoftDeletedByRemove", 12365, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIssueWarningQuota = new StorePropTag(12366, PropertyType.Unicode, new StorePropInfo("UserInformationIssueWarningQuota", 12366, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationJournalArchiveAddress = new StorePropTag(12367, PropertyType.Unicode, new StorePropInfo("UserInformationJournalArchiveAddress", 12367, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLanguages = new StorePropTag(12368, PropertyType.MVUnicode, new StorePropInfo("UserInformationLanguages", 12368, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLastExchangeChangedTime = new StorePropTag(12369, PropertyType.SysTime, new StorePropInfo("UserInformationLastExchangeChangedTime", 12369, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLastName = new StorePropTag(12370, PropertyType.Unicode, new StorePropInfo("UserInformationLastName", 12370, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLatitude = new StorePropTag(12371, PropertyType.Int32, new StorePropInfo("UserInformationLatitude", 12371, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLEOEnabled = new StorePropTag(12372, PropertyType.Boolean, new StorePropInfo("UserInformationLEOEnabled", 12372, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLocaleID = new StorePropTag(12373, PropertyType.MVInt32, new StorePropInfo("UserInformationLocaleID", 12373, PropertyType.MVInt32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationLongitude = new StorePropTag(12374, PropertyType.Int32, new StorePropInfo("UserInformationLongitude", 12374, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMacAttachmentFormat = new StorePropTag(12375, PropertyType.Int32, new StorePropInfo("UserInformationMacAttachmentFormat", 12375, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailboxContainerGuid = new StorePropTag(12376, PropertyType.Guid, new StorePropInfo("UserInformationMailboxContainerGuid", 12376, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailboxMoveBatchName = new StorePropTag(12377, PropertyType.Unicode, new StorePropInfo("UserInformationMailboxMoveBatchName", 12377, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailboxMoveRemoteHostName = new StorePropTag(12378, PropertyType.Unicode, new StorePropInfo("UserInformationMailboxMoveRemoteHostName", 12378, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailboxMoveStatus = new StorePropTag(12379, PropertyType.Int32, new StorePropInfo("UserInformationMailboxMoveStatus", 12379, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailboxRelease = new StorePropTag(12380, PropertyType.Unicode, new StorePropInfo("UserInformationMailboxRelease", 12380, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMailTipTranslations = new StorePropTag(12381, PropertyType.MVUnicode, new StorePropInfo("UserInformationMailTipTranslations", 12381, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMAPIBlockOutlookNonCachedMode = new StorePropTag(12382, PropertyType.Boolean, new StorePropInfo("UserInformationMAPIBlockOutlookNonCachedMode", 12382, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMAPIBlockOutlookRpcHttp = new StorePropTag(12383, PropertyType.Boolean, new StorePropInfo("UserInformationMAPIBlockOutlookRpcHttp", 12383, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMAPIBlockOutlookVersions = new StorePropTag(12384, PropertyType.Unicode, new StorePropInfo("UserInformationMAPIBlockOutlookVersions", 12384, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMAPIEnabled = new StorePropTag(12385, PropertyType.Boolean, new StorePropInfo("UserInformationMAPIEnabled", 12385, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMapiRecipient = new StorePropTag(12386, PropertyType.Boolean, new StorePropInfo("UserInformationMapiRecipient", 12386, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMaxBlockedSenders = new StorePropTag(12387, PropertyType.Int32, new StorePropInfo("UserInformationMaxBlockedSenders", 12387, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMaxReceiveSize = new StorePropTag(12388, PropertyType.Unicode, new StorePropInfo("UserInformationMaxReceiveSize", 12388, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMaxSafeSenders = new StorePropTag(12389, PropertyType.Int32, new StorePropInfo("UserInformationMaxSafeSenders", 12389, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMaxSendSize = new StorePropTag(12390, PropertyType.Unicode, new StorePropInfo("UserInformationMaxSendSize", 12390, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMemberName = new StorePropTag(12391, PropertyType.Unicode, new StorePropInfo("UserInformationMemberName", 12391, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMessageBodyFormat = new StorePropTag(12392, PropertyType.Int32, new StorePropInfo("UserInformationMessageBodyFormat", 12392, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMessageFormat = new StorePropTag(12393, PropertyType.Int32, new StorePropInfo("UserInformationMessageFormat", 12393, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMessageTrackingReadStatusDisabled = new StorePropTag(12394, PropertyType.Boolean, new StorePropInfo("UserInformationMessageTrackingReadStatusDisabled", 12394, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMobileFeaturesEnabled = new StorePropTag(12395, PropertyType.Int32, new StorePropInfo("UserInformationMobileFeaturesEnabled", 12395, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMobilePhone = new StorePropTag(12396, PropertyType.Unicode, new StorePropInfo("UserInformationMobilePhone", 12396, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationModerationFlags = new StorePropTag(12397, PropertyType.Int32, new StorePropInfo("UserInformationModerationFlags", 12397, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationNotes = new StorePropTag(12398, PropertyType.Unicode, new StorePropInfo("UserInformationNotes", 12398, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOccupation = new StorePropTag(12399, PropertyType.Unicode, new StorePropInfo("UserInformationOccupation", 12399, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOpenDomainRoutingDisabled = new StorePropTag(12400, PropertyType.Boolean, new StorePropInfo("UserInformationOpenDomainRoutingDisabled", 12400, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOtherHomePhone = new StorePropTag(12401, PropertyType.MVUnicode, new StorePropInfo("UserInformationOtherHomePhone", 12401, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOtherMobile = new StorePropTag(12402, PropertyType.MVUnicode, new StorePropInfo("UserInformationOtherMobile", 12402, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOtherTelephone = new StorePropTag(12403, PropertyType.MVUnicode, new StorePropInfo("UserInformationOtherTelephone", 12403, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOWAEnabled = new StorePropTag(12404, PropertyType.Boolean, new StorePropInfo("UserInformationOWAEnabled", 12404, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOWAforDevicesEnabled = new StorePropTag(12405, PropertyType.Boolean, new StorePropInfo("UserInformationOWAforDevicesEnabled", 12405, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPager = new StorePropTag(12406, PropertyType.Unicode, new StorePropInfo("UserInformationPager", 12406, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPersistedCapabilities = new StorePropTag(12407, PropertyType.MVInt32, new StorePropInfo("UserInformationPersistedCapabilities", 12407, PropertyType.MVInt32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPhone = new StorePropTag(12408, PropertyType.Unicode, new StorePropInfo("UserInformationPhone", 12408, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPhoneProviderId = new StorePropTag(12409, PropertyType.Unicode, new StorePropInfo("UserInformationPhoneProviderId", 12409, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopEnabled = new StorePropTag(12410, PropertyType.Boolean, new StorePropInfo("UserInformationPopEnabled", 12410, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopEnableExactRFC822Size = new StorePropTag(12411, PropertyType.Boolean, new StorePropInfo("UserInformationPopEnableExactRFC822Size", 12411, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopForceICalForCalendarRetrievalOption = new StorePropTag(12412, PropertyType.Boolean, new StorePropInfo("UserInformationPopForceICalForCalendarRetrievalOption", 12412, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopMessagesRetrievalMimeFormat = new StorePropTag(12413, PropertyType.Int32, new StorePropInfo("UserInformationPopMessagesRetrievalMimeFormat", 12413, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopProtocolLoggingEnabled = new StorePropTag(12414, PropertyType.Int32, new StorePropInfo("UserInformationPopProtocolLoggingEnabled", 12414, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopSuppressReadReceipt = new StorePropTag(12415, PropertyType.Boolean, new StorePropInfo("UserInformationPopSuppressReadReceipt", 12415, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPopUseProtocolDefaults = new StorePropTag(12416, PropertyType.Boolean, new StorePropInfo("UserInformationPopUseProtocolDefaults", 12416, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPostalCode = new StorePropTag(12417, PropertyType.Unicode, new StorePropInfo("UserInformationPostalCode", 12417, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPostOfficeBox = new StorePropTag(12418, PropertyType.MVUnicode, new StorePropInfo("UserInformationPostOfficeBox", 12418, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPreviousExchangeGuid = new StorePropTag(12419, PropertyType.Guid, new StorePropInfo("UserInformationPreviousExchangeGuid", 12419, PropertyType.Guid, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationPreviousRecipientTypeDetails = new StorePropTag(12420, PropertyType.Int32, new StorePropInfo("UserInformationPreviousRecipientTypeDetails", 12420, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationProhibitSendQuota = new StorePropTag(12421, PropertyType.Unicode, new StorePropInfo("UserInformationProhibitSendQuota", 12421, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationProhibitSendReceiveQuota = new StorePropTag(12422, PropertyType.Unicode, new StorePropInfo("UserInformationProhibitSendReceiveQuota", 12422, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationQueryBaseDNRestrictionEnabled = new StorePropTag(12423, PropertyType.Boolean, new StorePropInfo("UserInformationQueryBaseDNRestrictionEnabled", 12423, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRecipientDisplayType = new StorePropTag(12424, PropertyType.Int32, new StorePropInfo("UserInformationRecipientDisplayType", 12424, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRecipientLimits = new StorePropTag(12425, PropertyType.Unicode, new StorePropInfo("UserInformationRecipientLimits", 12425, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRecipientSoftDeletedStatus = new StorePropTag(12426, PropertyType.Int32, new StorePropInfo("UserInformationRecipientSoftDeletedStatus", 12426, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRecoverableItemsQuota = new StorePropTag(12427, PropertyType.Unicode, new StorePropInfo("UserInformationRecoverableItemsQuota", 12427, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRecoverableItemsWarningQuota = new StorePropTag(12428, PropertyType.Unicode, new StorePropInfo("UserInformationRecoverableItemsWarningQuota", 12428, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRegion = new StorePropTag(12429, PropertyType.Unicode, new StorePropInfo("UserInformationRegion", 12429, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRemotePowerShellEnabled = new StorePropTag(12430, PropertyType.Boolean, new StorePropInfo("UserInformationRemotePowerShellEnabled", 12430, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRemoteRecipientType = new StorePropTag(12431, PropertyType.Int32, new StorePropInfo("UserInformationRemoteRecipientType", 12431, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRequireAllSendersAreAuthenticated = new StorePropTag(12432, PropertyType.Boolean, new StorePropInfo("UserInformationRequireAllSendersAreAuthenticated", 12432, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationResetPasswordOnNextLogon = new StorePropTag(12433, PropertyType.Boolean, new StorePropInfo("UserInformationResetPasswordOnNextLogon", 12433, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRetainDeletedItemsFor = new StorePropTag(12434, PropertyType.Int64, new StorePropInfo("UserInformationRetainDeletedItemsFor", 12434, PropertyType.Int64, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRetainDeletedItemsUntilBackup = new StorePropTag(12435, PropertyType.Boolean, new StorePropInfo("UserInformationRetainDeletedItemsUntilBackup", 12435, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationRulesQuota = new StorePropTag(12436, PropertyType.Unicode, new StorePropInfo("UserInformationRulesQuota", 12436, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationShouldUseDefaultRetentionPolicy = new StorePropTag(12437, PropertyType.Boolean, new StorePropInfo("UserInformationShouldUseDefaultRetentionPolicy", 12437, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationSimpleDisplayName = new StorePropTag(12438, PropertyType.Unicode, new StorePropInfo("UserInformationSimpleDisplayName", 12438, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationSingleItemRecoveryEnabled = new StorePropTag(12439, PropertyType.Boolean, new StorePropInfo("UserInformationSingleItemRecoveryEnabled", 12439, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationStateOrProvince = new StorePropTag(12440, PropertyType.Unicode, new StorePropInfo("UserInformationStateOrProvince", 12440, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationStreetAddress = new StorePropTag(12441, PropertyType.Unicode, new StorePropInfo("UserInformationStreetAddress", 12441, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationSubscriberAccessEnabled = new StorePropTag(12442, PropertyType.Boolean, new StorePropInfo("UserInformationSubscriberAccessEnabled", 12442, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationTextEncodedORAddress = new StorePropTag(12443, PropertyType.Unicode, new StorePropInfo("UserInformationTextEncodedORAddress", 12443, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationTextMessagingState = new StorePropTag(12444, PropertyType.MVUnicode, new StorePropInfo("UserInformationTextMessagingState", 12444, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationTimezone = new StorePropTag(12445, PropertyType.Unicode, new StorePropInfo("UserInformationTimezone", 12445, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUCSImListMigrationCompleted = new StorePropTag(12446, PropertyType.Boolean, new StorePropInfo("UserInformationUCSImListMigrationCompleted", 12446, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeDetails = new StorePropTag(12447, PropertyType.Unicode, new StorePropInfo("UserInformationUpgradeDetails", 12447, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeMessage = new StorePropTag(12448, PropertyType.Unicode, new StorePropInfo("UserInformationUpgradeMessage", 12448, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeRequest = new StorePropTag(12449, PropertyType.Int32, new StorePropInfo("UserInformationUpgradeRequest", 12449, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeStage = new StorePropTag(12450, PropertyType.Int32, new StorePropInfo("UserInformationUpgradeStage", 12450, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeStageTimeStamp = new StorePropTag(12451, PropertyType.SysTime, new StorePropInfo("UserInformationUpgradeStageTimeStamp", 12451, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUpgradeStatus = new StorePropTag(12452, PropertyType.Int32, new StorePropInfo("UserInformationUpgradeStatus", 12452, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUsageLocation = new StorePropTag(12453, PropertyType.Unicode, new StorePropInfo("UserInformationUsageLocation", 12453, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUseMapiRichTextFormat = new StorePropTag(12454, PropertyType.Int32, new StorePropInfo("UserInformationUseMapiRichTextFormat", 12454, PropertyType.Int32, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUsePreferMessageFormat = new StorePropTag(12455, PropertyType.Boolean, new StorePropInfo("UserInformationUsePreferMessageFormat", 12455, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationUseUCCAuditConfig = new StorePropTag(12456, PropertyType.Boolean, new StorePropInfo("UserInformationUseUCCAuditConfig", 12456, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationWebPage = new StorePropTag(12457, PropertyType.Unicode, new StorePropInfo("UserInformationWebPage", 12457, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationWhenMailboxCreated = new StorePropTag(12458, PropertyType.SysTime, new StorePropInfo("UserInformationWhenMailboxCreated", 12458, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationWhenSoftDeleted = new StorePropTag(12459, PropertyType.SysTime, new StorePropInfo("UserInformationWhenSoftDeleted", 12459, PropertyType.SysTime, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationBirthdayPrecision = new StorePropTag(12460, PropertyType.Unicode, new StorePropInfo("UserInformationBirthdayPrecision", 12460, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationNameVersion = new StorePropTag(12461, PropertyType.Unicode, new StorePropInfo("UserInformationNameVersion", 12461, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationOptInUser = new StorePropTag(12462, PropertyType.Boolean, new StorePropInfo("UserInformationOptInUser", 12462, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsMigratedConsumerMailbox = new StorePropTag(12463, PropertyType.Boolean, new StorePropInfo("UserInformationIsMigratedConsumerMailbox", 12463, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMigrationDryRun = new StorePropTag(12464, PropertyType.Boolean, new StorePropInfo("UserInformationMigrationDryRun", 12464, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationIsPremiumConsumerMailbox = new StorePropTag(12465, PropertyType.Boolean, new StorePropInfo("UserInformationIsPremiumConsumerMailbox", 12465, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationAlternateSupportEmailAddresses = new StorePropTag(12466, PropertyType.Unicode, new StorePropInfo("UserInformationAlternateSupportEmailAddresses", 12466, PropertyType.Unicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationEmailAddresses = new StorePropTag(12467, PropertyType.MVUnicode, new StorePropInfo("UserInformationEmailAddresses", 12467, PropertyType.MVUnicode, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMapiHttpEnabled = new StorePropTag(12502, PropertyType.Boolean, new StorePropInfo("UserInformationMapiHttpEnabled", 12502, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag UserInformationMAPIBlockOutlookExternalConnectivity = new StorePropTag(12503, PropertyType.Boolean, new StorePropInfo("UserInformationMAPIBlockOutlookExternalConnectivity", 12503, PropertyType.Boolean, StorePropInfo.Flags.None, 0UL, default(PropertyCategories)), ObjectType.UserInfo);

			public static readonly StorePropTag[] NoGetPropProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoGetPropListForFastTransferProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropRestrictedProperties = new StorePropTag[]
			{
				PropTag.UserInfo.UserInformationGuid,
				PropTag.UserInfo.UserInformationCreationTime,
				PropTag.UserInfo.UserInformationLastModificationTime,
				PropTag.UserInfo.UserInformationChangeNumber
			};

			public static readonly StorePropTag[] SetPropAllowedForMailboxMoveProperties = new StorePropTag[]
			{
				PropTag.UserInfo.UserInformationCreationTime,
				PropTag.UserInfo.UserInformationLastModificationTime,
				PropTag.UserInfo.UserInformationChangeNumber
			};

			public static readonly StorePropTag[] SetPropAllowedForAdminProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedForTransportProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SetPropAllowedOnEmbeddedMessageProperties = new StorePropTag[0];

			public static readonly StorePropTag[] FacebookProtectedPropertiesProperties = new StorePropTag[0];

			public static readonly StorePropTag[] NoCopyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ComputedProperties = new StorePropTag[0];

			public static readonly StorePropTag[] IgnoreSetErrorProperties = new StorePropTag[0];

			public static readonly StorePropTag[] MessageBodyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] CAIProperties = new StorePropTag[0];

			public static readonly StorePropTag[] ServerOnlySyncGroupPropertyProperties = new StorePropTag[0];

			public static readonly StorePropTag[] SensitiveProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotBumpChangeNumberProperties = new StorePropTag[0];

			public static readonly StorePropTag[] DoNotDeleteAtFXCopyToDestinationProperties = new StorePropTag[0];

			public static readonly StorePropTag[] TestProperties = new StorePropTag[0];

			public static readonly StorePropTag[] AllProperties = new StorePropTag[]
			{
				PropTag.UserInfo.UserInformationGuid,
				PropTag.UserInfo.UserInformationDisplayName,
				PropTag.UserInfo.UserInformationCreationTime,
				PropTag.UserInfo.UserInformationLastModificationTime,
				PropTag.UserInfo.UserInformationChangeNumber,
				PropTag.UserInfo.UserInformationLastInteractiveLogonTime,
				PropTag.UserInfo.UserInformationActiveSyncAllowedDeviceIDs,
				PropTag.UserInfo.UserInformationActiveSyncBlockedDeviceIDs,
				PropTag.UserInfo.UserInformationActiveSyncDebugLogging,
				PropTag.UserInfo.UserInformationActiveSyncEnabled,
				PropTag.UserInfo.UserInformationAdminDisplayName,
				PropTag.UserInfo.UserInformationAggregationSubscriptionCredential,
				PropTag.UserInfo.UserInformationAllowArchiveAddressSync,
				PropTag.UserInfo.UserInformationAltitude,
				PropTag.UserInfo.UserInformationAntispamBypassEnabled,
				PropTag.UserInfo.UserInformationArchiveDomain,
				PropTag.UserInfo.UserInformationArchiveGuid,
				PropTag.UserInfo.UserInformationArchiveName,
				PropTag.UserInfo.UserInformationArchiveQuota,
				PropTag.UserInfo.UserInformationArchiveRelease,
				PropTag.UserInfo.UserInformationArchiveStatus,
				PropTag.UserInfo.UserInformationArchiveWarningQuota,
				PropTag.UserInfo.UserInformationAssistantName,
				PropTag.UserInfo.UserInformationBirthdate,
				PropTag.UserInfo.UserInformationBypassNestedModerationEnabled,
				PropTag.UserInfo.UserInformationC,
				PropTag.UserInfo.UserInformationCalendarLoggingQuota,
				PropTag.UserInfo.UserInformationCalendarRepairDisabled,
				PropTag.UserInfo.UserInformationCalendarVersionStoreDisabled,
				PropTag.UserInfo.UserInformationCity,
				PropTag.UserInfo.UserInformationCountry,
				PropTag.UserInfo.UserInformationCountryCode,
				PropTag.UserInfo.UserInformationCountryOrRegion,
				PropTag.UserInfo.UserInformationDefaultMailTip,
				PropTag.UserInfo.UserInformationDeliverToMailboxAndForward,
				PropTag.UserInfo.UserInformationDescription,
				PropTag.UserInfo.UserInformationDisabledArchiveGuid,
				PropTag.UserInfo.UserInformationDowngradeHighPriorityMessagesEnabled,
				PropTag.UserInfo.UserInformationECPEnabled,
				PropTag.UserInfo.UserInformationEmailAddressPolicyEnabled,
				PropTag.UserInfo.UserInformationEwsAllowEntourage,
				PropTag.UserInfo.UserInformationEwsAllowMacOutlook,
				PropTag.UserInfo.UserInformationEwsAllowOutlook,
				PropTag.UserInfo.UserInformationEwsApplicationAccessPolicy,
				PropTag.UserInfo.UserInformationEwsEnabled,
				PropTag.UserInfo.UserInformationEwsExceptions,
				PropTag.UserInfo.UserInformationEwsWellKnownApplicationAccessPolicies,
				PropTag.UserInfo.UserInformationExchangeGuid,
				PropTag.UserInfo.UserInformationExternalOofOptions,
				PropTag.UserInfo.UserInformationFirstName,
				PropTag.UserInfo.UserInformationForwardingSmtpAddress,
				PropTag.UserInfo.UserInformationGender,
				PropTag.UserInfo.UserInformationGenericForwardingAddress,
				PropTag.UserInfo.UserInformationGeoCoordinates,
				PropTag.UserInfo.UserInformationHABSeniorityIndex,
				PropTag.UserInfo.UserInformationHasActiveSyncDevicePartnership,
				PropTag.UserInfo.UserInformationHiddenFromAddressListsEnabled,
				PropTag.UserInfo.UserInformationHiddenFromAddressListsValue,
				PropTag.UserInfo.UserInformationHomePhone,
				PropTag.UserInfo.UserInformationImapEnabled,
				PropTag.UserInfo.UserInformationImapEnableExactRFC822Size,
				PropTag.UserInfo.UserInformationImapForceICalForCalendarRetrievalOption,
				PropTag.UserInfo.UserInformationImapMessagesRetrievalMimeFormat,
				PropTag.UserInfo.UserInformationImapProtocolLoggingEnabled,
				PropTag.UserInfo.UserInformationImapSuppressReadReceipt,
				PropTag.UserInfo.UserInformationImapUseProtocolDefaults,
				PropTag.UserInfo.UserInformationIncludeInGarbageCollection,
				PropTag.UserInfo.UserInformationInitials,
				PropTag.UserInfo.UserInformationInPlaceHolds,
				PropTag.UserInfo.UserInformationInternalOnly,
				PropTag.UserInfo.UserInformationInternalUsageLocation,
				PropTag.UserInfo.UserInformationInternetEncoding,
				PropTag.UserInfo.UserInformationIsCalculatedTargetAddress,
				PropTag.UserInfo.UserInformationIsExcludedFromServingHierarchy,
				PropTag.UserInfo.UserInformationIsHierarchyReady,
				PropTag.UserInfo.UserInformationIsInactiveMailbox,
				PropTag.UserInfo.UserInformationIsSoftDeletedByDisable,
				PropTag.UserInfo.UserInformationIsSoftDeletedByRemove,
				PropTag.UserInfo.UserInformationIssueWarningQuota,
				PropTag.UserInfo.UserInformationJournalArchiveAddress,
				PropTag.UserInfo.UserInformationLanguages,
				PropTag.UserInfo.UserInformationLastExchangeChangedTime,
				PropTag.UserInfo.UserInformationLastName,
				PropTag.UserInfo.UserInformationLatitude,
				PropTag.UserInfo.UserInformationLEOEnabled,
				PropTag.UserInfo.UserInformationLocaleID,
				PropTag.UserInfo.UserInformationLongitude,
				PropTag.UserInfo.UserInformationMacAttachmentFormat,
				PropTag.UserInfo.UserInformationMailboxContainerGuid,
				PropTag.UserInfo.UserInformationMailboxMoveBatchName,
				PropTag.UserInfo.UserInformationMailboxMoveRemoteHostName,
				PropTag.UserInfo.UserInformationMailboxMoveStatus,
				PropTag.UserInfo.UserInformationMailboxRelease,
				PropTag.UserInfo.UserInformationMailTipTranslations,
				PropTag.UserInfo.UserInformationMAPIBlockOutlookNonCachedMode,
				PropTag.UserInfo.UserInformationMAPIBlockOutlookRpcHttp,
				PropTag.UserInfo.UserInformationMAPIBlockOutlookVersions,
				PropTag.UserInfo.UserInformationMAPIEnabled,
				PropTag.UserInfo.UserInformationMapiRecipient,
				PropTag.UserInfo.UserInformationMaxBlockedSenders,
				PropTag.UserInfo.UserInformationMaxReceiveSize,
				PropTag.UserInfo.UserInformationMaxSafeSenders,
				PropTag.UserInfo.UserInformationMaxSendSize,
				PropTag.UserInfo.UserInformationMemberName,
				PropTag.UserInfo.UserInformationMessageBodyFormat,
				PropTag.UserInfo.UserInformationMessageFormat,
				PropTag.UserInfo.UserInformationMessageTrackingReadStatusDisabled,
				PropTag.UserInfo.UserInformationMobileFeaturesEnabled,
				PropTag.UserInfo.UserInformationMobilePhone,
				PropTag.UserInfo.UserInformationModerationFlags,
				PropTag.UserInfo.UserInformationNotes,
				PropTag.UserInfo.UserInformationOccupation,
				PropTag.UserInfo.UserInformationOpenDomainRoutingDisabled,
				PropTag.UserInfo.UserInformationOtherHomePhone,
				PropTag.UserInfo.UserInformationOtherMobile,
				PropTag.UserInfo.UserInformationOtherTelephone,
				PropTag.UserInfo.UserInformationOWAEnabled,
				PropTag.UserInfo.UserInformationOWAforDevicesEnabled,
				PropTag.UserInfo.UserInformationPager,
				PropTag.UserInfo.UserInformationPersistedCapabilities,
				PropTag.UserInfo.UserInformationPhone,
				PropTag.UserInfo.UserInformationPhoneProviderId,
				PropTag.UserInfo.UserInformationPopEnabled,
				PropTag.UserInfo.UserInformationPopEnableExactRFC822Size,
				PropTag.UserInfo.UserInformationPopForceICalForCalendarRetrievalOption,
				PropTag.UserInfo.UserInformationPopMessagesRetrievalMimeFormat,
				PropTag.UserInfo.UserInformationPopProtocolLoggingEnabled,
				PropTag.UserInfo.UserInformationPopSuppressReadReceipt,
				PropTag.UserInfo.UserInformationPopUseProtocolDefaults,
				PropTag.UserInfo.UserInformationPostalCode,
				PropTag.UserInfo.UserInformationPostOfficeBox,
				PropTag.UserInfo.UserInformationPreviousExchangeGuid,
				PropTag.UserInfo.UserInformationPreviousRecipientTypeDetails,
				PropTag.UserInfo.UserInformationProhibitSendQuota,
				PropTag.UserInfo.UserInformationProhibitSendReceiveQuota,
				PropTag.UserInfo.UserInformationQueryBaseDNRestrictionEnabled,
				PropTag.UserInfo.UserInformationRecipientDisplayType,
				PropTag.UserInfo.UserInformationRecipientLimits,
				PropTag.UserInfo.UserInformationRecipientSoftDeletedStatus,
				PropTag.UserInfo.UserInformationRecoverableItemsQuota,
				PropTag.UserInfo.UserInformationRecoverableItemsWarningQuota,
				PropTag.UserInfo.UserInformationRegion,
				PropTag.UserInfo.UserInformationRemotePowerShellEnabled,
				PropTag.UserInfo.UserInformationRemoteRecipientType,
				PropTag.UserInfo.UserInformationRequireAllSendersAreAuthenticated,
				PropTag.UserInfo.UserInformationResetPasswordOnNextLogon,
				PropTag.UserInfo.UserInformationRetainDeletedItemsFor,
				PropTag.UserInfo.UserInformationRetainDeletedItemsUntilBackup,
				PropTag.UserInfo.UserInformationRulesQuota,
				PropTag.UserInfo.UserInformationShouldUseDefaultRetentionPolicy,
				PropTag.UserInfo.UserInformationSimpleDisplayName,
				PropTag.UserInfo.UserInformationSingleItemRecoveryEnabled,
				PropTag.UserInfo.UserInformationStateOrProvince,
				PropTag.UserInfo.UserInformationStreetAddress,
				PropTag.UserInfo.UserInformationSubscriberAccessEnabled,
				PropTag.UserInfo.UserInformationTextEncodedORAddress,
				PropTag.UserInfo.UserInformationTextMessagingState,
				PropTag.UserInfo.UserInformationTimezone,
				PropTag.UserInfo.UserInformationUCSImListMigrationCompleted,
				PropTag.UserInfo.UserInformationUpgradeDetails,
				PropTag.UserInfo.UserInformationUpgradeMessage,
				PropTag.UserInfo.UserInformationUpgradeRequest,
				PropTag.UserInfo.UserInformationUpgradeStage,
				PropTag.UserInfo.UserInformationUpgradeStageTimeStamp,
				PropTag.UserInfo.UserInformationUpgradeStatus,
				PropTag.UserInfo.UserInformationUsageLocation,
				PropTag.UserInfo.UserInformationUseMapiRichTextFormat,
				PropTag.UserInfo.UserInformationUsePreferMessageFormat,
				PropTag.UserInfo.UserInformationUseUCCAuditConfig,
				PropTag.UserInfo.UserInformationWebPage,
				PropTag.UserInfo.UserInformationWhenMailboxCreated,
				PropTag.UserInfo.UserInformationWhenSoftDeleted,
				PropTag.UserInfo.UserInformationBirthdayPrecision,
				PropTag.UserInfo.UserInformationNameVersion,
				PropTag.UserInfo.UserInformationOptInUser,
				PropTag.UserInfo.UserInformationIsMigratedConsumerMailbox,
				PropTag.UserInfo.UserInformationMigrationDryRun,
				PropTag.UserInfo.UserInformationIsPremiumConsumerMailbox,
				PropTag.UserInfo.UserInformationAlternateSupportEmailAddresses,
				PropTag.UserInfo.UserInformationEmailAddresses,
				PropTag.UserInfo.UserInformationMapiHttpEnabled,
				PropTag.UserInfo.UserInformationMAPIBlockOutlookExternalConnectivity
			};
		}
	}
}
