using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationBatchMessageSchema
	{
		public const DefaultFolderType WorkingFolder = DefaultFolderType.Inbox;

		public const string InputCsvAttachmentName = "Request.csv";

		public const string ErrorsCsvAttachmentName = "Errors.csv";

		public static readonly char[] ListSeparator = new char[]
		{
			':'
		};

		public static readonly string FolderSeparator = "\r\n";

		public static readonly StorePropertyDefinition MigrationVersion = InternalSchema.MigrationVersion;

		public static readonly StorePropertyDefinition MigrationJobId = InternalSchema.MigrationJobId;

		public static readonly StorePropertyDefinition MigrationJobItemId = InternalSchema.MigrationJobItemId;

		public static readonly StorePropertyDefinition MigrationJobName = InternalSchema.MigrationJobName;

		public static readonly StorePropertyDefinition MigrationJobSubmittedBy = InternalSchema.MigrationJobSubmittedBy;

		public static readonly StorePropertyDefinition MigrationJobTotalRowCount = InternalSchema.MigrationJobTotalRowCount;

		public static readonly StorePropertyDefinition MigrationJobTotalItemCountLegacy = InternalSchema.MigrationJobTotalItemCountLegacy;

		public static readonly StorePropertyDefinition MigrationJobCountCache = InternalSchema.MigrationJobCountCache;

		public static readonly StorePropertyDefinition MigrationJobCountCacheFullScanTime = InternalSchema.MigrationJobCountCacheFullScanTime;

		public static readonly StorePropertyDefinition MigrationJobExcludedFolders = InternalSchema.MigrationJobExcludedFolders;

		public static readonly StorePropertyDefinition MigrationJobNotificationEmails = InternalSchema.MigrationJobNotificationEmails;

		public static readonly StorePropertyDefinition MigrationJobOriginalCreationTime = InternalSchema.MigrationJobOriginalCreationTime;

		public static readonly StorePropertyDefinition MigrationJobStartTime = InternalSchema.MigrationJobStartTime;

		public static readonly StorePropertyDefinition MigrationJobLastRestartTime = InternalSchema.MigrationJobLastRestartTime;

		public static readonly StorePropertyDefinition MigrationJobFinalizeTime = InternalSchema.MigrationJobFinalizeTime;

		public static readonly StorePropertyDefinition MigrationJobLastFinalizationAttempt = InternalSchema.MigrationJobLastFinalizationAttempt;

		public static readonly StorePropertyDefinition MigrationJobUserTimeZone = InternalSchema.MigrationJobUserTimeZone;

		public static readonly StorePropertyDefinition MigrationJobCancelledFlag = InternalSchema.MigrationJobCancelledFlag;

		public static readonly StorePropertyDefinition MigrationUserStatus = InternalSchema.MigrationJobItemStatus;

		public static readonly StorePropertyDefinition MigrationJobItemRecipientType = InternalSchema.MigrationJobItemRecipientType;

		public static readonly StorePropertyDefinition MigrationJobSuppressErrors = InternalSchema.MigrationJobSuppressErrors;

		public static readonly StorePropertyDefinition MigrationJobItemIdentifier = InternalSchema.MigrationJobItemIdentifier;

		public static readonly StorePropertyDefinition MigrationJobItemEncryptedIncomingPassword = InternalSchema.MigrationJobItemEncryptedIncomingPassword;

		public static readonly StorePropertyDefinition MigrationJobItemIncomingUsername = InternalSchema.MigrationJobItemIncomingUsername;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionMessageId = InternalSchema.MigrationJobItemSubscriptionMessageId;

		public static readonly StorePropertyDefinition MigrationJobItemMailboxServer = InternalSchema.MigrationJobItemMailboxServer;

		public static readonly StorePropertyDefinition MigrationJobItemMailboxId = InternalSchema.MigrationJobItemMailboxId;

		public static readonly StorePropertyDefinition MigrationJobItemMailboxDatabaseId = InternalSchema.MigrationJobItemMailboxDatabaseId;

		public static readonly StorePropertyDefinition MigrationJobItemStateLastUpdated = InternalSchema.MigrationJobItemStateLastUpdated;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionCreated = InternalSchema.MigrationJobItemSubscriptionCreated;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionLastChecked = InternalSchema.MigrationJobItemSubscriptionLastChecked;

		public static readonly StorePropertyDefinition MigrationJobItemMailboxLegacyDN = InternalSchema.MigrationJobItemMailboxLegacyDN;

		public static readonly StorePropertyDefinition MigrationJobItemRowIndex = InternalSchema.MigrationJobItemRowIndex;

		public static readonly StorePropertyDefinition MigrationJobItemLocalizedError = InternalSchema.MigrationJobItemLocalizedError;

		public static readonly StorePropertyDefinition MigrationJobInternalError = InternalSchema.MigrationJobInternalError;

		public static readonly StorePropertyDefinition MigrationJobInternalErrorTime = InternalSchema.MigrationJobInternalErrorTime;

		public static readonly StorePropertyDefinition MigrationJobRemoteServerHostName = InternalSchema.MigrationJobRemoteServerHostName;

		public static readonly StorePropertyDefinition MigrationJobRemoteServerPortNumber = InternalSchema.MigrationJobRemoteServerPortNumber;

		public static readonly StorePropertyDefinition MigrationJobRemoteServerAuth = InternalSchema.MigrationJobRemoteServerAuth;

		public static readonly StorePropertyDefinition MigrationJobRemoteServerSecurity = InternalSchema.MigrationJobRemoteServerSecurity;

		public static readonly StorePropertyDefinition MigrationJobMaxConcurrentMigrations = InternalSchema.MigrationJobMaxConcurrentMigrations;

		public static readonly StorePropertyDefinition MigrationJobAdminCulture = InternalSchema.MigrationJobAdminCulture;

		public static readonly StorePropertyDefinition MigrationCacheEntryMailboxLegacyDN = InternalSchema.MigrationCacheEntryMailboxLegacyDN;

		public static readonly StorePropertyDefinition MigrationCacheEntryTenantPartitionHint = InternalSchema.MigrationCacheEntryTenantPartitionHint;

		public static readonly StorePropertyDefinition MigrationSubmittedByUserAdminType = InternalSchema.MigrationSubmittedByUserAdminType;

		public static readonly StorePropertyDefinition MigrationCacheEntryLastUpdated = InternalSchema.MigrationCacheEntryLastUpdated;

		public static readonly StorePropertyDefinition MigrationUserRootFolder = InternalSchema.MigrationUserRootFolder;

		public static readonly StorePropertyDefinition MigrationType = InternalSchema.MigrationType;

		public static readonly StorePropertyDefinition MigrationJobWindowsLiveNetId = InternalSchema.MigrationJobWindowsLiveNetId;

		public static readonly StorePropertyDefinition MigrationJobCursorPosition = InternalSchema.MigrationJobCursorPosition;

		public static readonly StorePropertyDefinition MigrationJobItemWLSASigned = InternalSchema.MigrationJobItemWLSASigned;

		public static readonly StorePropertyDefinition MigrationJobOwnerId = InternalSchema.MigrationJobOwnerId;

		public static readonly StorePropertyDefinition MigrationJobDelegatedAdminOwnerId = InternalSchema.MigrationJobDelegatedAdminOwnerId;

		public static readonly StorePropertyDefinition MigrationJobExchangeHasAdminPrivilege = InternalSchema.MigrationJobHasAdminPrivilege;

		public static readonly StorePropertyDefinition MigrationJobExchangeHasAutodiscovery = InternalSchema.MigrationJobHasAutodiscovery;

		public static readonly StorePropertyDefinition MigrationJobExchangeEmailAddress = InternalSchema.MigrationJobEmailAddress;

		public static readonly StorePropertyDefinition MigrationJobExchangeAutodiscoverUrl = InternalSchema.MigrationJobRemoteAutodiscoverUrl;

		public static readonly StorePropertyDefinition MigrationJobExchangeRemoteServerHostName = InternalSchema.MigrationJobExchangeRemoteServerHostName;

		public static readonly StorePropertyDefinition MigrationJobExchangeRPCProxyServerHostName = InternalSchema.MigrationJobProxyServerHostName;

		public static readonly StorePropertyDefinition MigrationJobExchangeNSPIServerHostName = InternalSchema.MigrationJobRemoteNSPIServerHostName;

		public static readonly StorePropertyDefinition MigrationJobExchangeDomain = InternalSchema.MigrationJobRemoteDomain;

		public static readonly StorePropertyDefinition MigrationJobExchangeRemoteServerVersion = InternalSchema.MigrationJobRemoteServerVersion;

		public static readonly StorePropertyDefinition MigrationJobStatisticsEnabled = InternalSchema.MigrationJobStatisticsEnabled;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRemoteMailboxLegacyDN = InternalSchema.MigrationJobItemRemoteMailboxLegacyDN;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRemoteServerLegacyDN = InternalSchema.MigrationJobItemRemoteServerLegacyDN;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRemoteServerHostName = InternalSchema.MigrationJobItemExchangeRemoteServerHostName;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRPCProxyServerHostName = InternalSchema.MigrationJobItemProxyServerHostName;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeAutodiscoverUrl = InternalSchema.MigrationJobItemRemoteAutodiscoverUrl;

		public static readonly StorePropertyDefinition MigrationJobExchangeRemoteServerAuth = InternalSchema.MigrationJobExchangeRemoteServerAuth;

		public static readonly StorePropertyDefinition MigrationJobItemProvisioningData = InternalSchema.MigrationJobItemProvisioningData;

		public static readonly StorePropertyDefinition MigrationJobItemMRSId = InternalSchema.MigrationJobItemMRSId;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRecipientIndex = InternalSchema.MigrationJobItemExchangeRecipientIndex;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeRecipientProperties = InternalSchema.MigrationJobItemExchangeRecipientProperties;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeMsExchHomeServerName = InternalSchema.MigrationJobItemExchangeMsExchHomeServerName;

		public static readonly GuidIdPropertyDefinition MigrationJobItemItemsSynced = InternalSchema.MigrationJobItemItemsSynced;

		public static readonly GuidIdPropertyDefinition MigrationJobItemItemsSkipped = InternalSchema.MigrationJobItemItemsSkipped;

		public static readonly StorePropertyDefinition MigrationJobItemGroupMemberProvisioningState = InternalSchema.MigrationJobItemGroupMemberProvisioningState;

		public static readonly StorePropertyDefinition MigrationJobItemGroupMemberProvisioned = InternalSchema.MigrationJobItemGroupMemberProvisioned;

		public static readonly StorePropertyDefinition MigrationJobItemGroupMemberSkipped = InternalSchema.MigrationJobItemGroupMemberSkipped;

		public static readonly StorePropertyDefinition MigrationJobItemLastProvisionedMemberIndex = InternalSchema.MigrationJobItemLastProvisionedMemberIndex;

		public static readonly StorePropertyDefinition MigrationJobItemADObjectExists = InternalSchema.MigrationJobItemADObjectExists;

		public static readonly StorePropertyDefinition MigrationReportName = InternalSchema.MigrationReportName;

		public static readonly StorePropertyDefinition MigrationJobItemOwnerId = InternalSchema.MigrationJobItemOwnerId;

		public static readonly StorePropertyDefinition MigrationJobCancellationReason = InternalSchema.MigrationJobCancellationReason;

		public static readonly StorePropertyDefinition MigrationJobItemExchangeMbxEncryptedPassword = InternalSchema.MigrationJobItemExchangeMbxEncryptedPassword;

		public static readonly StorePropertyDefinition MigrationJobIsStaged = InternalSchema.MigrationJobIsStaged;

		public static readonly StorePropertyDefinition MigrationJobCheckWLSA = InternalSchema.MigrationJobCheckWLSA;

		public static readonly StorePropertyDefinition MigrationJobItemTransientErrorCount = InternalSchema.MigrationJobItemTransientErrorCount;

		public static readonly StorePropertyDefinition MigrationJobItemPreviousStatus = InternalSchema.MigrationJobItemPreviousStatus;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionId = InternalSchema.MigrationJobItemSubscriptionId;

		public static readonly StorePropertyDefinition MigrationJobPoisonCount = InternalSchema.MigrationJobPoisonCount;

		public static readonly StorePropertyDefinition MigrationReportType = InternalSchema.MigrationReportType;

		public static readonly StorePropertyDefinition MigrationSuccessReportUrl = InternalSchema.MigrationSuccessReportUrl;

		public static readonly StorePropertyDefinition MigrationErrorReportUrl = InternalSchema.MigrationErrorReportUrl;

		public static readonly StorePropertyDefinition MigrationJobTargetDomainName = InternalSchema.MigrationJobTargetDomainName;

		public static readonly StorePropertyDefinition MigrationJobItemForceChangePassword = InternalSchema.MigrationJobItemForceChangePassword;

		public static readonly StorePropertyDefinition MigrationJobItemLocalizedErrorID = InternalSchema.MigrationJobItemLocalizedErrorID;

		public static readonly StorePropertyDefinition MigrationJobItemStatusHistory = InternalSchema.MigrationJobItemStatusHistory;

		public static readonly StorePropertyDefinition MigrationJobItemIDSIdentityFlags = InternalSchema.MigrationJobItemIDSIdentityFlags;

		public static readonly StorePropertyDefinition MigrationJobItemLocalizedMessage = InternalSchema.MigrationJobItemLocalizedMessage;

		public static readonly StorePropertyDefinition MigrationJobItemLocalizedMessageID = InternalSchema.MigrationJobItemLocalizedMessageID;

		public static readonly StorePropertyDefinition MigrationSameStatusCount = InternalSchema.MigrationSameStatusCount;

		public static readonly StorePropertyDefinition MigrationTransitionTime = InternalSchema.MigrationTransitionTime;

		public static readonly StorePropertyDefinition MigrationDeltaSyncShouldSync = InternalSchema.MigrationDeltaSyncShouldSync;

		public static readonly StorePropertyDefinition MigrationPersistableDictionary = InternalSchema.MigrationPersistableDictionary;

		public static readonly StorePropertyDefinition MigrationRuntimeJobData = InternalSchema.MigrationRuntimeJobData;

		public static readonly StorePropertyDefinition MigrationReportSets = InternalSchema.MigrationReportSets;

		public static readonly StorePropertyDefinition MigrationDisableTime = InternalSchema.MigrationDisableTime;

		public static readonly StorePropertyDefinition MigrationProvisionedTime = InternalSchema.MigrationProvisionedTime;

		public static readonly StorePropertyDefinition MigrationLastSuccessfulSyncTime = InternalSchema.MigrationLastSuccessfulSyncTime;

		public static readonly StorePropertyDefinition MigrationJobSourceEndpoint = InternalSchema.MigrationJobSourceEndpoint;

		public static readonly StorePropertyDefinition MigrationJobTargetEndpoint = InternalSchema.MigrationJobTargetEndpoint;

		public static readonly StorePropertyDefinition MigrationJobDirection = InternalSchema.MigrationJobDirection;

		public static readonly PropertyDefinition MigrationJobSourcePublicFolderDatabase = InternalSchema.MigrationJobSourcePublicFolderDatabase;

		public static readonly PropertyDefinition MigrationJobTargetDatabase = InternalSchema.MigrationJobTargetDatabase;

		public static readonly PropertyDefinition MigrationJobTargetArchiveDatabase = InternalSchema.MigrationJobTargetArchiveDatabase;

		public static readonly PropertyDefinition MigrationJobBadItemLimit = InternalSchema.MigrationJobBadItemLimit;

		public static readonly PropertyDefinition MigrationJobLargeItemLimit = InternalSchema.MigrationJobLargeItemLimit;

		public static readonly PropertyDefinition MigrationJobPrimaryOnly = InternalSchema.MigrationJobPrimaryOnly;

		public static readonly PropertyDefinition MigrationJobArchiveOnly = InternalSchema.MigrationJobArchiveOnly;

		public static readonly PropertyDefinition MigrationJobTargetDeliveryDomain = InternalSchema.MigrationJobTargetDeliveryDomain;

		public static readonly PropertyDefinition MigrationSlotMaximumInitialSeedings = InternalSchema.MigrationSlotMaximumInitialSeedings;

		public static readonly PropertyDefinition MigrationSlotMaximumIncrementalSeedings = InternalSchema.MigrationSlotMaximumIncrementalSeedings;

		public static readonly PropertyDefinition MigrationJobSkipSteps = InternalSchema.MigrationJobSkipSteps;

		public static readonly StorePropertyDefinition MigrationJobItemSlotType = InternalSchema.MigrationJobItemSlotType;

		public static readonly StorePropertyDefinition MigrationJobItemSlotProviderId = InternalSchema.MigrationJobItemSlotProviderId;

		public static readonly PropertyDefinition MigrationFailureRecord = InternalSchema.MigrationFailureRecord;

		public static readonly PropertyDefinition MigrationJobIsRunning = InternalSchema.MigrationJobIsRunning;

		public static readonly StorePropertyDefinition MigrationSubscriptionSettingsLastModifiedTime = InternalSchema.MigrationSubscriptionSettingsLastModifiedTime;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionSettingsLastUpdatedTime = InternalSchema.MigrationJobItemSubscriptionSettingsLastUpdatedTime;

		public static readonly StorePropertyDefinition MigrationJobStartAfter = InternalSchema.MigrationJobStartAfter;

		public static readonly StorePropertyDefinition MigrationJobCompleteAfter = InternalSchema.MigrationJobCompleteAfter;

		public static readonly StorePropertyDefinition MigrationNextProcessTime = InternalSchema.MigrationNextProcessTime;

		public static readonly StorePropertyDefinition MigrationStatusDataFailureWatsonHash = InternalSchema.MigrationStatusDataFailureWatsonHash;

		public static readonly StorePropertyDefinition MigrationState = InternalSchema.MigrationState;

		public static readonly StorePropertyDefinition MigrationFlags = InternalSchema.MigrationFlags;

		public static readonly StorePropertyDefinition MigrationStage = InternalSchema.MigrationStage;

		public static readonly StorePropertyDefinition MigrationStep = InternalSchema.MigrationStep;

		public static readonly StorePropertyDefinition MigrationWorkflow = InternalSchema.MigrationWorkflow;

		public static readonly StorePropertyDefinition MigrationPSTFilePath = InternalSchema.MigrationPSTFilePath;

		public static readonly StorePropertyDefinition MigrationSourceRootFolder = InternalSchema.MigrationSourceRootFolder;

		public static readonly StorePropertyDefinition MigrationTargetRootFolder = InternalSchema.MigrationTargetRootFolder;

		public static readonly StorePropertyDefinition MigrationJobItemLocalMailboxIdentifier = InternalSchema.MigrationJobItemLocalMailboxIdentifier;

		public static readonly StorePropertyDefinition MigrationExchangeObjectId = InternalSchema.MigrationExchangeObjectId;

		public static readonly StorePropertyDefinition MigrationJobItemSubscriptionQueuedTime = InternalSchema.MigrationJobItemSubscriptionQueuedTime;

		public static readonly StorePropertyDefinition MigrationJobItemPuid = InternalSchema.MigrationJobItemPuid;

		public static readonly StorePropertyDefinition MigrationJobItemFirstName = InternalSchema.MigrationJobItemFirstName;

		public static readonly StorePropertyDefinition MigrationJobItemLastName = InternalSchema.MigrationJobItemLastName;

		public static readonly StorePropertyDefinition MigrationJobItemTimeZone = InternalSchema.MigrationJobItemTimeZone;

		public static readonly StorePropertyDefinition MigrationJobItemLocaleId = InternalSchema.MigrationJobItemLocaleId;

		public static readonly StorePropertyDefinition MigrationJobItemAliases = InternalSchema.MigrationJobItemAliases;

		public static readonly StorePropertyDefinition MigrationJobItemAccountSize = InternalSchema.MigrationJobItemAccountSize;

		public static readonly string MigrationJobClass = "IPM.MS-Exchange.MigrationJob";

		public static readonly string MigrationJobItemClass = "IPM.MS-Exchange.MigrationJobItem";

		public static readonly string MigrationCacheEntryClass = "IPM.MS-Exchange.MigrationCacheEntry";

		public static readonly string MigrationReportItemClass = "IPM.MS-Exchange.MigrationReportItem";
	}
}
