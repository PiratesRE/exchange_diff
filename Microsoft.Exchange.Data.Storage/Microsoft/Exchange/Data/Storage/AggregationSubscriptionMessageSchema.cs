using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationSubscriptionMessageSchema : MessageItemSchema
	{
		public new static AggregationSubscriptionMessageSchema Instance
		{
			get
			{
				if (AggregationSubscriptionMessageSchema.instance == null)
				{
					AggregationSubscriptionMessageSchema.instance = new AggregationSubscriptionMessageSchema();
				}
				return AggregationSubscriptionMessageSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition SharingInitiatorName = InternalSchema.SharingInitiatorName;

		public static readonly StorePropertyDefinition SharingInitiatorSmtp = InternalSchema.SharingInitiatorSmtp;

		public static readonly StorePropertyDefinition SharingRemoteUser = InternalSchema.SharingRemoteUser;

		public static readonly StorePropertyDefinition SharingRemotePass = InternalSchema.SharingRemotePass;

		public static readonly StorePropertyDefinition SharingLastSuccessSyncTime = InternalSchema.SharingLastSuccessSyncTime;

		public static readonly StorePropertyDefinition SharingSyncRange = InternalSchema.SharingSyncRange;

		public static readonly StorePropertyDefinition SharingAggregationStatus = InternalSchema.SharingAggregationStatus;

		public static readonly StorePropertyDefinition SharingWlidAuthPolicy = InternalSchema.SharingWlidAuthPolicy;

		public static readonly StorePropertyDefinition SharingWlidUserPuid = InternalSchema.SharingWlidUserPuid;

		public static readonly StorePropertyDefinition SharingWlidAuthToken = InternalSchema.SharingWlidAuthToken;

		public static readonly StorePropertyDefinition SharingWlidAuthTokenExpireTime = InternalSchema.SharingWlidAuthTokenExpireTime;

		public static readonly StorePropertyDefinition SharingMinSyncPollInterval = InternalSchema.SharingMinSyncPollInterval;

		public static readonly StorePropertyDefinition SharingMinSettingPollInterval = InternalSchema.SharingMinSettingPollInterval;

		public static readonly StorePropertyDefinition SharingSyncMultiplier = InternalSchema.SharingSyncMultiplier;

		public static readonly StorePropertyDefinition SharingMaxObjectsInSync = InternalSchema.SharingMaxObjectsInSync;

		public static readonly StorePropertyDefinition SharingMaxNumberOfEmails = InternalSchema.SharingMaxNumberOfEmails;

		public static readonly StorePropertyDefinition SharingMaxNumberOfFolders = InternalSchema.SharingMaxNumberOfFolders;

		public static readonly StorePropertyDefinition SharingMaxAttachments = InternalSchema.SharingMaxAttachments;

		public static readonly StorePropertyDefinition SharingMaxMessageSize = InternalSchema.SharingMaxMessageSize;

		public static readonly StorePropertyDefinition SharingMaxRecipients = InternalSchema.SharingMaxRecipients;

		public static readonly StorePropertyDefinition SharingMigrationState = InternalSchema.SharingMigrationState;

		public static readonly StorePropertyDefinition SharingAggregationType = InternalSchema.SharingAggregationType;

		public static readonly StorePropertyDefinition SharingPoisonCallstack = InternalSchema.SharingPoisonCallstack;

		public static readonly StorePropertyDefinition SharingSubscriptionConfiguration = InternalSchema.SharingSubscriptionConfiguration;

		public static readonly StorePropertyDefinition SharingAggregationProtocolVersion = InternalSchema.SharingAggregationProtocolVersion;

		public static readonly StorePropertyDefinition SharingAggregationProtocolName = InternalSchema.SharingAggregationProtocolName;

		public static readonly StorePropertyDefinition SharingSubscriptionName = InternalSchema.SharingSubscriptionName;

		public static readonly StorePropertyDefinition SharingSubscriptionsCache = InternalSchema.SharingSubscriptionsCache;

		public static readonly StorePropertyDefinition SharingSubscriptionCreationType = InternalSchema.SharingSubscriptionCreationType;

		public static readonly StorePropertyDefinition SharingSubscriptionSyncPhase = InternalSchema.SharingSubscriptionSyncPhase;

		public static readonly StorePropertyDefinition SharingSubscriptionExclusionFolders = InternalSchema.SharingSubscriptionExclusionFolders;

		public static readonly StorePropertyDefinition SharingSendAsVerificationEmailState = InternalSchema.SharingSendAsVerificationEmailState;

		public static readonly StorePropertyDefinition SharingSendAsVerificationMessageId = InternalSchema.SharingSendAsVerificationMessageId;

		public static readonly StorePropertyDefinition SharingSendAsVerificationTimestamp = InternalSchema.SharingSendAsVerificationTimestamp;

		public static readonly StorePropertyDefinition SharingSubscriptionEvents = InternalSchema.SharingSubscriptionEvents;

		public static readonly StorePropertyDefinition SharingSubscriptionItemsSynced = InternalSchema.SharingSubscriptionItemsSynced;

		public static readonly StorePropertyDefinition SharingSubscriptionItemsSkipped = InternalSchema.SharingSubscriptionItemsSkipped;

		public static readonly StorePropertyDefinition SharingSubscriptionTotalItemsInSourceMailbox = InternalSchema.SharingSubscriptionTotalItemsInSourceMailbox;

		public static readonly StorePropertyDefinition SharingSubscriptionTotalSizeOfSourceMailbox = InternalSchema.SharingSubscriptionTotalSizeOfSourceMailbox;

		public static readonly StorePropertyDefinition SharingImapPathPrefix = InternalSchema.SharingImapPathPrefix;

		public static readonly StorePropertyDefinition SharingAdjustedLastSuccessfulSyncTime = InternalSchema.SharingAdjustedLastSuccessfulSyncTime;

		public static readonly StorePropertyDefinition SharingLastSyncNowRequest = InternalSchema.SharingLastSyncNowRequest;

		public static readonly StorePropertyDefinition SharingOutageDetectionDiagnostics = InternalSchema.SharingOutageDetectionDiagnostics;

		public static readonly StorePropertyDefinition SharingEncryptedAccessToken = InternalSchema.SharingEncryptedAccessToken;

		public static readonly StorePropertyDefinition SharingAppId = InternalSchema.SharingAppId;

		public static readonly StorePropertyDefinition SharingUserId = InternalSchema.SharingUserId;

		public static readonly StorePropertyDefinition SharingEncryptedAccessTokenSecret = InternalSchema.SharingEncryptedAccessTokenSecret;

		public static readonly StorePropertyDefinition SharingInitialSyncInRecoveryMode = InternalSchema.SharingInitialSyncInRecoveryMode;

		private static AggregationSubscriptionMessageSchema instance = null;
	}
}
