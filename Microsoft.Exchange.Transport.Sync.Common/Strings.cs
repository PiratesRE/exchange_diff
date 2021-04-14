using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(995731409U, "IMAPUnsupportedVersionException");
			Strings.stringIDs.Add(147813322U, "NestedFoldersNotAllowedException");
			Strings.stringIDs.Add(1898862428U, "LeaveOnServerNotSupportedStatus");
			Strings.stringIDs.Add(1598126189U, "IMAPTrash");
			Strings.stringIDs.Add(1507388000U, "Pop3MirroredAccountNotPossibleException");
			Strings.stringIDs.Add(4121087442U, "RemoteMailboxQuotaWarningWithDisabledStatus");
			Strings.stringIDs.Add(3959479424U, "SubscriptionInvalidPasswordException");
			Strings.stringIDs.Add(1971329929U, "SubscriptionUpdateTransientException");
			Strings.stringIDs.Add(1075040750U, "ConnectionErrorWithDisabledStatus");
			Strings.stringIDs.Add(3599922107U, "ConnectedAccountsDetails");
			Strings.stringIDs.Add(3969501405U, "IMAPAllMail");
			Strings.stringIDs.Add(2030480939U, "MTOMParsingFailedException");
			Strings.stringIDs.Add(4079355374U, "LabsMailboxQuotaWarningWithDelayedDetailedStatus");
			Strings.stringIDs.Add(3801534216U, "AuthenticationErrorWithDelayedDetailedStatus");
			Strings.stringIDs.Add(1261870541U, "RemoteServerIsBackedOffException");
			Strings.stringIDs.Add(1401415251U, "LinkedInNonPromotableTransientException");
			Strings.stringIDs.Add(2432721285U, "RetryLaterException");
			Strings.stringIDs.Add(2971027178U, "ProviderExceptionDetailedStatus");
			Strings.stringIDs.Add(3410639299U, "MessageSizeLimitExceededException");
			Strings.stringIDs.Add(1298696584U, "CommunicationErrorWithDelayedStatus");
			Strings.stringIDs.Add(604756220U, "MailboxFailure");
			Strings.stringIDs.Add(3346793726U, "Pop3CapabilitiesNotSupportedException");
			Strings.stringIDs.Add(580763412U, "IMAPInvalidServerException");
			Strings.stringIDs.Add(3937165620U, "LeaveOnServerNotSupportedDetailedStatus");
			Strings.stringIDs.Add(3902086079U, "ConnectionClosedException");
			Strings.stringIDs.Add(2952144307U, "PoisonousRemoteServerException");
			Strings.stringIDs.Add(2394731155U, "SubscriptionInvalidVersionException");
			Strings.stringIDs.Add(3561039784U, "IMAPSentMail");
			Strings.stringIDs.Add(1756157035U, "InvalidVersionDetailedStatus");
			Strings.stringIDs.Add(3613930518U, "CommunicationErrorWithDisabledStatus");
			Strings.stringIDs.Add(3694666204U, "InProgressDetailedStatus");
			Strings.stringIDs.Add(517632853U, "AutoProvisionTestImap");
			Strings.stringIDs.Add(3137325842U, "DelayedStatus");
			Strings.stringIDs.Add(3444450370U, "ProviderExceptionStatus");
			Strings.stringIDs.Add(3530466102U, "CommunicationErrorWithDisabledDetailedStatus");
			Strings.stringIDs.Add(4176165696U, "RemoteServerIsSlowDisabledDetailedStatus");
			Strings.stringIDs.Add(2010701853U, "SendAsVerificationBottomBlock");
			Strings.stringIDs.Add(3292189374U, "AuthenticationErrorWithDisabledStatus");
			Strings.stringIDs.Add(591776832U, "ConnectionDownloadedLimitExceededException");
			Strings.stringIDs.Add(1477720531U, "MailboxOverQuotaException");
			Strings.stringIDs.Add(2149789026U, "InvalidServerResponseException");
			Strings.stringIDs.Add(2425965665U, "SyncStateSizeErrorDetailedStatus");
			Strings.stringIDs.Add(2492614440U, "RemoteAccountDoesNotExistDetailedStatus");
			Strings.stringIDs.Add(710927991U, "IMAPDrafts");
			Strings.stringIDs.Add(3842730909U, "MaxedOutSyncRelationshipsErrorWithDelayedDetailedStatus");
			Strings.stringIDs.Add(1812618676U, "IMAPInvalidItemException");
			Strings.stringIDs.Add(2019254494U, "SendAsVerificationSender");
			Strings.stringIDs.Add(2713854040U, "IMAPDraft");
			Strings.stringIDs.Add(882640929U, "IMAPSentMessages");
			Strings.stringIDs.Add(4155460975U, "TlsRemoteCertificateInvalid");
			Strings.stringIDs.Add(802159867U, "Pop3AuthErrorException");
			Strings.stringIDs.Add(2900222280U, "PoisonStatus");
			Strings.stringIDs.Add(2053678515U, "MessageIdGenerationTransientException");
			Strings.stringIDs.Add(3896029100U, "AutoProvisionTestAutoDiscover");
			Strings.stringIDs.Add(3699840920U, "Pop3TransientSystemAuthErrorException");
			Strings.stringIDs.Add(2759557964U, "SyncConflictException");
			Strings.stringIDs.Add(76218079U, "IMAPSentItems");
			Strings.stringIDs.Add(1404272750U, "SyncEngineSyncStorageProviderCreationException");
			Strings.stringIDs.Add(2942865760U, "Pop3TransientLoginDelayedAuthErrorException");
			Strings.stringIDs.Add(3228154330U, "IMAPDeletedMessages");
			Strings.stringIDs.Add(1124604668U, "RemoteMailboxQuotaWarningWithDelayedStatus");
			Strings.stringIDs.Add(1488733145U, "SyncStateSizeErrorStatus");
			Strings.stringIDs.Add(4092585897U, "MissingServerResponseException");
			Strings.stringIDs.Add(4257197704U, "LabsMailboxQuotaWarningWithDisabledStatus");
			Strings.stringIDs.Add(4287770443U, "Pop3TransientInUseAuthErrorException");
			Strings.stringIDs.Add(3000749982U, "Pop3DisabledResponseException");
			Strings.stringIDs.Add(233413355U, "SendAsVerificationSubject");
			Strings.stringIDs.Add(1726233450U, "IMAPAuthenticationException");
			Strings.stringIDs.Add(1413995526U, "FailedToGenerateVerificationEmail");
			Strings.stringIDs.Add(1235995580U, "RemoteMailboxQuotaWarningWithDelayedDetailedStatus");
			Strings.stringIDs.Add(715816254U, "AuthenticationErrorWithDisabledDetailedStatus");
			Strings.stringIDs.Add(1257586764U, "ConfigurationErrorStatus");
			Strings.stringIDs.Add(189434176U, "AuthenticationErrorWithDelayedStatus");
			Strings.stringIDs.Add(2936000979U, "MaxedOutSyncRelationshipsErrorBody");
			Strings.stringIDs.Add(2363855804U, "HotmailAccountVerificationFailedException");
			Strings.stringIDs.Add(1548922274U, "AutoProvisionResults");
			Strings.stringIDs.Add(2204337803U, "InvalidVersionStatus");
			Strings.stringIDs.Add(2984668342U, "IMAPDeletedItems");
			Strings.stringIDs.Add(601262870U, "TooManyFoldersStatus");
			Strings.stringIDs.Add(2617144415U, "SendAsVerificationSignatureTopPart");
			Strings.stringIDs.Add(2808236588U, "ConnectionErrorWithDelayedStatus");
			Strings.stringIDs.Add(3760020583U, "IMAPJunk");
			Strings.stringIDs.Add(974222160U, "RemoteAccountDoesNotExistStatus");
			Strings.stringIDs.Add(2167449654U, "TooManyFoldersDetailedStatus");
			Strings.stringIDs.Add(925456835U, "SuccessDetailedStatus");
			Strings.stringIDs.Add(2318825308U, "InProgressStatus");
			Strings.stringIDs.Add(3335416471U, "SubscriptionNotificationEmailBodyStartText");
			Strings.stringIDs.Add(3933215749U, "MaxedOutSyncRelationshipsErrorWithDisabledDetailedStatus");
			Strings.stringIDs.Add(2173734488U, "IMAPSpam");
			Strings.stringIDs.Add(1734582074U, "RemoteMailboxQuotaWarningWithDisabledDetailedStatus");
			Strings.stringIDs.Add(650547385U, "SendAsVerificationTopBlock");
			Strings.stringIDs.Add(3930645136U, "CommunicationErrorWithDelayedDetailedStatus");
			Strings.stringIDs.Add(4038775200U, "ContactCsvFileContainsNoKnownColumns");
			Strings.stringIDs.Add(82266192U, "LabsMailboxQuotaWarningWithDisabledDetailedStatus");
			Strings.stringIDs.Add(420875274U, "DelayedDetailedStatus");
			Strings.stringIDs.Add(3447698883U, "HttpResponseStreamNullException");
			Strings.stringIDs.Add(1059234861U, "RemoveSubscriptionStatus");
			Strings.stringIDs.Add(2325761520U, "DisabledDetailedStatus");
			Strings.stringIDs.Add(3091979858U, "InvalidAggregationSubscriptionIdentity");
			Strings.stringIDs.Add(1325089515U, "IMAPJunkEmail");
			Strings.stringIDs.Add(3269288896U, "DisabledStatus");
			Strings.stringIDs.Add(3779095989U, "MaxedOutSyncRelationshipsErrorWithDisabledStatus");
			Strings.stringIDs.Add(2754299458U, "FirstAppendToNotes");
			Strings.stringIDs.Add(3826985530U, "InvalidCsvFileFormat");
			Strings.stringIDs.Add(348158591U, "DeltaSyncServiceEndpointsLoadException");
			Strings.stringIDs.Add(2529513648U, "PoisonDetailedStatus");
			Strings.stringIDs.Add(2425850576U, "AutoProvisionTestHotmail");
			Strings.stringIDs.Add(1096710760U, "RemoteServerIsSlowStatus");
			Strings.stringIDs.Add(623751595U, "SubscriptionUpdatePermanentException");
			Strings.stringIDs.Add(603913161U, "PoisonousErrorBody");
			Strings.stringIDs.Add(2979796742U, "Pop3NonCompliantServerException");
			Strings.stringIDs.Add(1339731251U, "SuccessStatus");
			Strings.stringIDs.Add(3068969418U, "ConnectionErrorDetailedStatus");
			Strings.stringIDs.Add(3345637854U, "LabsMailboxQuotaWarningWithDelayedStatus");
			Strings.stringIDs.Add(2505373931U, "IMAPGmailNotSupportedException");
			Strings.stringIDs.Add(521865336U, "Pop3CannotConnectToServerException");
			Strings.stringIDs.Add(2222507634U, "RemoteServerIsSlowDelayedDetailedStatus");
			Strings.stringIDs.Add(2839399357U, "IMAPSent");
			Strings.stringIDs.Add(2049114804U, "StoreRestartedException");
			Strings.stringIDs.Add(3011192446U, "AccessTokenNullOrEmpty");
			Strings.stringIDs.Add(1745238072U, "Pop3LeaveOnServerNotPossibleException");
			Strings.stringIDs.Add(3743909068U, "AutoProvisionTestPop3");
			Strings.stringIDs.Add(3420783178U, "InvalidSyncEngineStateException");
			Strings.stringIDs.Add(3910384057U, "ContactCsvFileEmpty");
			Strings.stringIDs.Add(4000016521U, "FacebookNonPromotableTransientException");
			Strings.stringIDs.Add(305729253U, "MaxedOutSyncRelationshipsErrorWithDelayedStatus");
		}

		public static LocalizedString IMAPUnsupportedVersionException
		{
			get
			{
				return new LocalizedString("IMAPUnsupportedVersionException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NestedFoldersNotAllowedException
		{
			get
			{
				return new LocalizedString("NestedFoldersNotAllowedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedDetailedStatusHours(int hours)
		{
			return new LocalizedString("DelayedDetailedStatusHours", "Ex70F5A8", false, true, Strings.ResourceManager, new object[]
			{
				hours
			});
		}

		public static LocalizedString UnexpectedContentTypeException(string contentType)
		{
			return new LocalizedString("UnexpectedContentTypeException", "", false, false, Strings.ResourceManager, new object[]
			{
				contentType
			});
		}

		public static LocalizedString LeaveOnServerNotSupportedStatus
		{
			get
			{
				return new LocalizedString("LeaveOnServerNotSupportedStatus", "ExFEE8E9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPTrash
		{
			get
			{
				return new LocalizedString("IMAPTrash", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAsVerificationSalutation(string userDisplayName)
		{
			return new LocalizedString("SendAsVerificationSalutation", "Ex11593B", false, true, Strings.ResourceManager, new object[]
			{
				userDisplayName
			});
		}

		public static LocalizedString SubscriptionInconsistent(string name)
		{
			return new LocalizedString("SubscriptionInconsistent", "ExD2BAA0", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString Pop3MirroredAccountNotPossibleException
		{
			get
			{
				return new LocalizedString("Pop3MirroredAccountNotPossibleException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserAccessException(int statusCode)
		{
			return new LocalizedString("UserAccessException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString RemoteMailboxQuotaWarningWithDisabledStatus
		{
			get
			{
				return new LocalizedString("RemoteMailboxQuotaWarningWithDisabledStatus", "ExC85FB0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionInvalidPasswordException
		{
			get
			{
				return new LocalizedString("SubscriptionInvalidPasswordException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionUpdateTransientException
		{
			get
			{
				return new LocalizedString("SubscriptionUpdateTransientException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorWithDisabledStatus
		{
			get
			{
				return new LocalizedString("ConnectionErrorWithDisabledStatus", "Ex7A4C38", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LeaveOnServerNotSupportedErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("LeaveOnServerNotSupportedErrorBody", "Ex525E3E", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString ConnectedAccountsDetails
		{
			get
			{
				return new LocalizedString("ConnectedAccountsDetails", "Ex5DBE07", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteAccountDoesNotExistBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("RemoteAccountDoesNotExistBody", "Ex4CE140", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString ConnectionErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("ConnectionErrorBody", "Ex9D9D39", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString SyncTooSlowException(TimeSpan syncDurationThreshold)
		{
			return new LocalizedString("SyncTooSlowException", "", false, false, Strings.ResourceManager, new object[]
			{
				syncDurationThreshold
			});
		}

		public static LocalizedString ConnectionErrorBodyHour(int hour, string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("ConnectionErrorBodyHour", "ExAED56C", false, true, Strings.ResourceManager, new object[]
			{
				hour,
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString IMAPAllMail
		{
			get
			{
				return new LocalizedString("IMAPAllMail", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageDecompressionFailedException(string serverId)
		{
			return new LocalizedString("MessageDecompressionFailedException", "", false, false, Strings.ResourceManager, new object[]
			{
				serverId
			});
		}

		public static LocalizedString FailedSetAggregationSubscription(string name)
		{
			return new LocalizedString("FailedSetAggregationSubscription", "Ex43A139", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MTOMParsingFailedException
		{
			get
			{
				return new LocalizedString("MTOMParsingFailedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabsMailboxQuotaWarningWithDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("LabsMailboxQuotaWarningWithDelayedDetailedStatus", "ExCE1D75", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedDeleteAggregationSubscription(string name)
		{
			return new LocalizedString("FailedDeleteAggregationSubscription", "Ex901310", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString PartnerAuthenticationException(int statusCode)
		{
			return new LocalizedString("PartnerAuthenticationException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString AuthenticationErrorWithDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("AuthenticationErrorWithDelayedDetailedStatus", "Ex4CA188", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerIsBackedOffException
		{
			get
			{
				return new LocalizedString("RemoteServerIsBackedOffException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LinkedInNonPromotableTransientException
		{
			get
			{
				return new LocalizedString("LinkedInNonPromotableTransientException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetryLaterException
		{
			get
			{
				return new LocalizedString("RetryLaterException", "Ex82BE43", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProviderExceptionDetailedStatus
		{
			get
			{
				return new LocalizedString("ProviderExceptionDetailedStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageSizeLimitExceededException
		{
			get
			{
				return new LocalizedString("MessageSizeLimitExceededException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedCreateAggregationSubscription(string name)
		{
			return new LocalizedString("FailedCreateAggregationSubscription", "ExEB3294", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CommunicationErrorWithDelayedStatus
		{
			get
			{
				return new LocalizedString("CommunicationErrorWithDelayedStatus", "Ex780460", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxFailure
		{
			get
			{
				return new LocalizedString("MailboxFailure", "Ex9057FE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3CapabilitiesNotSupportedException
		{
			get
			{
				return new LocalizedString("Pop3CapabilitiesNotSupportedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPInvalidServerException
		{
			get
			{
				return new LocalizedString("IMAPInvalidServerException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LeaveOnServerNotSupportedDetailedStatus
		{
			get
			{
				return new LocalizedString("LeaveOnServerNotSupportedDetailedStatus", "Ex153CCE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionNotFound(string subscription)
		{
			return new LocalizedString("SubscriptionNotFound", "Ex7CE313", false, true, Strings.ResourceManager, new object[]
			{
				subscription
			});
		}

		public static LocalizedString ConnectionClosedException
		{
			get
			{
				return new LocalizedString("ConnectionClosedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxPermanentErrorSavingContact(int failedContactIndex, int contactsSaved)
		{
			return new LocalizedString("MailboxPermanentErrorSavingContact", "", false, false, Strings.ResourceManager, new object[]
			{
				failedContactIndex,
				contactsSaved
			});
		}

		public static LocalizedString PoisonousRemoteServerException
		{
			get
			{
				return new LocalizedString("PoisonousRemoteServerException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleNativeItemsHaveSameCloudIdException(string cloudId, Guid subscriptionGuid)
		{
			return new LocalizedString("MultipleNativeItemsHaveSameCloudIdException", "", false, false, Strings.ResourceManager, new object[]
			{
				cloudId,
				subscriptionGuid
			});
		}

		public static LocalizedString SubscriptionInvalidVersionException
		{
			get
			{
				return new LocalizedString("SubscriptionInvalidVersionException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxTransientExceptionSavingContact(int failedContactIndex, int contactsSaved)
		{
			return new LocalizedString("MailboxTransientExceptionSavingContact", "", false, false, Strings.ResourceManager, new object[]
			{
				failedContactIndex,
				contactsSaved
			});
		}

		public static LocalizedString IMAPSentMail
		{
			get
			{
				return new LocalizedString("IMAPSentMail", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidVersionDetailedStatus
		{
			get
			{
				return new LocalizedString("InvalidVersionDetailedStatus", "Ex39BACE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommunicationErrorWithDisabledStatus
		{
			get
			{
				return new LocalizedString("CommunicationErrorWithDisabledStatus", "Ex3100BA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InProgressDetailedStatus
		{
			get
			{
				return new LocalizedString("InProgressDetailedStatus", "Ex44EC73", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionTestImap
		{
			get
			{
				return new LocalizedString("AutoProvisionTestImap", "Ex80AAE3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorDetailedStatusHour(int hour)
		{
			return new LocalizedString("ConnectionErrorDetailedStatusHour", "Ex7B4143", false, true, Strings.ResourceManager, new object[]
			{
				hour
			});
		}

		public static LocalizedString DelayedStatus
		{
			get
			{
				return new LocalizedString("DelayedStatus", "ExD77583", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncPoisonItemFoundException(string syncPoisonItem, Guid subscriptionId)
		{
			return new LocalizedString("SyncPoisonItemFoundException", "", false, false, Strings.ResourceManager, new object[]
			{
				syncPoisonItem,
				subscriptionId
			});
		}

		public static LocalizedString ProviderExceptionStatus
		{
			get
			{
				return new LocalizedString("ProviderExceptionStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorBodyHours(int hours, string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("ConnectionErrorBodyHours", "Ex296F05", false, true, Strings.ResourceManager, new object[]
			{
				hours,
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString SubscriptionSyncException(string subscriptionName)
		{
			return new LocalizedString("SubscriptionSyncException", "", false, false, Strings.ResourceManager, new object[]
			{
				subscriptionName
			});
		}

		public static LocalizedString CommunicationErrorWithDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("CommunicationErrorWithDisabledDetailedStatus", "Ex087643", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerIsSlowDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("RemoteServerIsSlowDisabledDetailedStatus", "Ex1749DA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAsVerificationBottomBlock
		{
			get
			{
				return new LocalizedString("SendAsVerificationBottomBlock", "Ex619B79", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationErrorWithDisabledStatus
		{
			get
			{
				return new LocalizedString("AuthenticationErrorWithDisabledStatus", "Ex9BF3DC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptSubscriptionException(Guid guid)
		{
			return new LocalizedString("CorruptSubscriptionException", "", false, false, Strings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString DelayedDetailedStatusDay(int day)
		{
			return new LocalizedString("DelayedDetailedStatusDay", "ExD3990F", false, true, Strings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString ConnectionDownloadedLimitExceededException
		{
			get
			{
				return new LocalizedString("ConnectionDownloadedLimitExceededException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxOverQuotaException
		{
			get
			{
				return new LocalizedString("MailboxOverQuotaException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServerResponseException
		{
			get
			{
				return new LocalizedString("InvalidServerResponseException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStateSizeErrorDetailedStatus
		{
			get
			{
				return new LocalizedString("SyncStateSizeErrorDetailedStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteAccountDoesNotExistDetailedStatus
		{
			get
			{
				return new LocalizedString("RemoteAccountDoesNotExistDetailedStatus", "Ex506CFF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPDrafts
		{
			get
			{
				return new LocalizedString("IMAPDrafts", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxedOutSyncRelationshipsErrorWithDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("MaxedOutSyncRelationshipsErrorWithDelayedDetailedStatus", "Ex7E5344", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPInvalidItemException
		{
			get
			{
				return new LocalizedString("IMAPInvalidItemException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAsVerificationSender
		{
			get
			{
				return new LocalizedString("SendAsVerificationSender", "ExE337C1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuotaExceededSavingContact(int failedContactIndex, int contactsSaved)
		{
			return new LocalizedString("QuotaExceededSavingContact", "", false, false, Strings.ResourceManager, new object[]
			{
				failedContactIndex,
				contactsSaved
			});
		}

		public static LocalizedString IMAPDraft
		{
			get
			{
				return new LocalizedString("IMAPDraft", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPSentMessages
		{
			get
			{
				return new LocalizedString("IMAPSentMessages", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsRemoteCertificateInvalid
		{
			get
			{
				return new LocalizedString("TlsRemoteCertificateInvalid", "Ex87C115", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3AuthErrorException
		{
			get
			{
				return new LocalizedString("Pop3AuthErrorException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3ErrorResponseException(string command, string response)
		{
			return new LocalizedString("Pop3ErrorResponseException", "", false, false, Strings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString AutoProvisionStatus(string authority, string username, string security, string authentication)
		{
			return new LocalizedString("AutoProvisionStatus", "", false, false, Strings.ResourceManager, new object[]
			{
				authority,
				username,
				security,
				authentication
			});
		}

		public static LocalizedString InternalErrorSavingContact(int failedContactIndex, int contactsSaved)
		{
			return new LocalizedString("InternalErrorSavingContact", "", false, false, Strings.ResourceManager, new object[]
			{
				failedContactIndex,
				contactsSaved
			});
		}

		public static LocalizedString DeltaSyncServerException(int statusCode)
		{
			return new LocalizedString("DeltaSyncServerException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString PoisonStatus
		{
			get
			{
				return new LocalizedString("PoisonStatus", "Ex4671DD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageIdGenerationTransientException
		{
			get
			{
				return new LocalizedString("MessageIdGenerationTransientException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPException(string failureReason)
		{
			return new LocalizedString("IMAPException", "", false, false, Strings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString SubscriptionNotificationEmailSubject(string subscriptionEmailAddress)
		{
			return new LocalizedString("SubscriptionNotificationEmailSubject", "ExBFF38A", false, true, Strings.ResourceManager, new object[]
			{
				subscriptionEmailAddress
			});
		}

		public static LocalizedString AutoProvisionTestAutoDiscover
		{
			get
			{
				return new LocalizedString("AutoProvisionTestAutoDiscover", "ExC150D2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3TransientSystemAuthErrorException
		{
			get
			{
				return new LocalizedString("Pop3TransientSystemAuthErrorException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncUnhandledException(Type type)
		{
			return new LocalizedString("SyncUnhandledException", "", false, false, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString SyncConflictException
		{
			get
			{
				return new LocalizedString("SyncConflictException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorDetailedStatusDays(int days)
		{
			return new LocalizedString("ConnectionErrorDetailedStatusDays", "Ex605693", false, true, Strings.ResourceManager, new object[]
			{
				days
			});
		}

		public static LocalizedString IMAPSentItems
		{
			get
			{
				return new LocalizedString("IMAPSentItems", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3PermErrorResponseException(string command, string response)
		{
			return new LocalizedString("Pop3PermErrorResponseException", "", false, false, Strings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString DelayedDetailedStatusHour(int hour)
		{
			return new LocalizedString("DelayedDetailedStatusHour", "Ex4EAD4C", false, true, Strings.ResourceManager, new object[]
			{
				hour
			});
		}

		public static LocalizedString FailedDeletePeopleConnectSubscription(AggregationSubscriptionType subscriptionType)
		{
			return new LocalizedString("FailedDeletePeopleConnectSubscription", "", false, false, Strings.ResourceManager, new object[]
			{
				subscriptionType
			});
		}

		public static LocalizedString SyncEngineSyncStorageProviderCreationException
		{
			get
			{
				return new LocalizedString("SyncEngineSyncStorageProviderCreationException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3TransientLoginDelayedAuthErrorException
		{
			get
			{
				return new LocalizedString("Pop3TransientLoginDelayedAuthErrorException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TlsFailureException(string failureReason)
		{
			return new LocalizedString("TlsFailureException", "", false, false, Strings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString IMAPDeletedMessages
		{
			get
			{
				return new LocalizedString("IMAPDeletedMessages", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteMailboxQuotaWarningWithDelayedStatus
		{
			get
			{
				return new LocalizedString("RemoteMailboxQuotaWarningWithDelayedStatus", "Ex40402E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStateSizeErrorStatus
		{
			get
			{
				return new LocalizedString("SyncStateSizeErrorStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingServerResponseException
		{
			get
			{
				return new LocalizedString("MissingServerResponseException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerTooSlowException(string remoteServer, int port, TimeSpan actualLatency, TimeSpan expectedLatency)
		{
			return new LocalizedString("RemoteServerTooSlowException", "", false, false, Strings.ResourceManager, new object[]
			{
				remoteServer,
				port,
				actualLatency,
				expectedLatency
			});
		}

		public static LocalizedString Pop3BrokenResponseException(string command, string response)
		{
			return new LocalizedString("Pop3BrokenResponseException", "", false, false, Strings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString LabsMailboxQuotaWarningWithDisabledStatus
		{
			get
			{
				return new LocalizedString("LabsMailboxQuotaWarningWithDisabledStatus", "ExC59B39", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3TransientInUseAuthErrorException
		{
			get
			{
				return new LocalizedString("Pop3TransientInUseAuthErrorException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3DisabledResponseException
		{
			get
			{
				return new LocalizedString("Pop3DisabledResponseException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerIsSlowErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("RemoteServerIsSlowErrorBody", "ExF197B6", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString SendAsVerificationSubject
		{
			get
			{
				return new LocalizedString("SendAsVerificationSubject", "ExF655E3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPAuthenticationException
		{
			get
			{
				return new LocalizedString("IMAPAuthenticationException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGenerateVerificationEmail
		{
			get
			{
				return new LocalizedString("FailedToGenerateVerificationEmail", "ExD9B280", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommunicationErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("CommunicationErrorBody", "Ex89DF6D", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString RemoteMailboxQuotaWarningWithDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("RemoteMailboxQuotaWarningWithDelayedDetailedStatus", "Ex935EA1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationErrorWithDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("AuthenticationErrorWithDisabledDetailedStatus", "Ex690DF9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigurationErrorStatus
		{
			get
			{
				return new LocalizedString("ConfigurationErrorStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationErrorWithDelayedStatus
		{
			get
			{
				return new LocalizedString("AuthenticationErrorWithDelayedStatus", "ExD59937", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UncompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, ByteQuantifiedSize currentUncompressedSyncStateSize, ByteQuantifiedSize loadedSyncStateSizeLimit)
		{
			return new LocalizedString("UncompressedSyncStateSizeExceededException", "", false, false, Strings.ResourceManager, new object[]
			{
				syncStateId,
				subscriptionId,
				currentUncompressedSyncStateSize,
				loadedSyncStateSizeLimit
			});
		}

		public static LocalizedString MaxedOutSyncRelationshipsErrorBody
		{
			get
			{
				return new LocalizedString("MaxedOutSyncRelationshipsErrorBody", "Ex8C1104", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HotmailAccountVerificationFailedException
		{
			get
			{
				return new LocalizedString("HotmailAccountVerificationFailedException", "Ex0CE4D2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionResults
		{
			get
			{
				return new LocalizedString("AutoProvisionResults", "Ex4E3CEC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidVersionStatus
		{
			get
			{
				return new LocalizedString("InvalidVersionStatus", "Ex097C26", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPDeletedItems
		{
			get
			{
				return new LocalizedString("IMAPDeletedItems", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AuthenticationErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("AuthenticationErrorBody", "ExE642A6", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString RedundantPimSubscription(string emailAddress)
		{
			return new LocalizedString("RedundantPimSubscription", "Ex874294", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString TooManyFoldersStatus
		{
			get
			{
				return new LocalizedString("TooManyFoldersStatus", "Ex712E6D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SendAsVerificationSignatureTopPart
		{
			get
			{
				return new LocalizedString("SendAsVerificationSignatureTopPart", "ExA39B17", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorWithDelayedStatus
		{
			get
			{
				return new LocalizedString("ConnectionErrorWithDelayedStatus", "ExA90BC0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPJunk
		{
			get
			{
				return new LocalizedString("IMAPJunk", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStateSizeErrorBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("SyncStateSizeErrorBody", "", false, false, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString Pop3UnknownResponseException(string command, string response)
		{
			return new LocalizedString("Pop3UnknownResponseException", "", false, false, Strings.ResourceManager, new object[]
			{
				command,
				response
			});
		}

		public static LocalizedString CompressedSyncStateSizeExceededException(string syncStateId, Guid subscriptionId, StoragePermanentException e)
		{
			return new LocalizedString("CompressedSyncStateSizeExceededException", "", false, false, Strings.ResourceManager, new object[]
			{
				syncStateId,
				subscriptionId,
				e
			});
		}

		public static LocalizedString DelayedDetailedStatusDays(int days)
		{
			return new LocalizedString("DelayedDetailedStatusDays", "ExE90DC4", false, true, Strings.ResourceManager, new object[]
			{
				days
			});
		}

		public static LocalizedString RemoteAccountDoesNotExistStatus
		{
			get
			{
				return new LocalizedString("RemoteAccountDoesNotExistStatus", "ExB18D81", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorBodyDays(int days, string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("ConnectionErrorBodyDays", "Ex83D02B", false, true, Strings.ResourceManager, new object[]
			{
				days,
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString TlsFailureErrorOccurred(SecurityStatus securityStatus)
		{
			return new LocalizedString("TlsFailureErrorOccurred", "Ex1A9BAC", false, true, Strings.ResourceManager, new object[]
			{
				securityStatus
			});
		}

		public static LocalizedString TooManyFoldersDetailedStatus
		{
			get
			{
				return new LocalizedString("TooManyFoldersDetailedStatus", "Ex9142EC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuccessDetailedStatus
		{
			get
			{
				return new LocalizedString("SuccessDetailedStatus", "Ex0B254E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownDeltaSyncException(int statusCode)
		{
			return new LocalizedString("UnknownDeltaSyncException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString InProgressStatus
		{
			get
			{
				return new LocalizedString("InProgressStatus", "Ex6D4483", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionNotificationEmailBodyStartText
		{
			get
			{
				return new LocalizedString("SubscriptionNotificationEmailBodyStartText", "ExE34D33", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestFormatException(int statusCode)
		{
			return new LocalizedString("RequestFormatException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString MaxedOutSyncRelationshipsErrorWithDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("MaxedOutSyncRelationshipsErrorWithDisabledDetailedStatus", "ExAE3E49", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnresolveableFolderNameException(string folderName)
		{
			return new LocalizedString("UnresolveableFolderNameException", "", false, false, Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString RedundantAccountSubscription(string username, string server)
		{
			return new LocalizedString("RedundantAccountSubscription", "ExA964A4", false, true, Strings.ResourceManager, new object[]
			{
				username,
				server
			});
		}

		public static LocalizedString IMAPSpam
		{
			get
			{
				return new LocalizedString("IMAPSpam", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteMailboxQuotaWarningWithDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("RemoteMailboxQuotaWarningWithDisabledDetailedStatus", "Ex9FBFAF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestSettingsException(int statusCode)
		{
			return new LocalizedString("RequestSettingsException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString SendAsVerificationTopBlock
		{
			get
			{
				return new LocalizedString("SendAsVerificationTopBlock", "Ex11E78B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorBodyDay(int day, string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("ConnectionErrorBodyDay", "Ex8E098F", false, true, Strings.ResourceManager, new object[]
			{
				day,
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString SubscriptionNumberExceedLimit(int number)
		{
			return new LocalizedString("SubscriptionNumberExceedLimit", "Ex69D7B2", false, true, Strings.ResourceManager, new object[]
			{
				number
			});
		}

		public static LocalizedString CommunicationErrorWithDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("CommunicationErrorWithDelayedDetailedStatus", "Ex5A8691", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataOutOfSyncException(int statusCode)
		{
			return new LocalizedString("DataOutOfSyncException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString ContactCsvFileContainsNoKnownColumns
		{
			get
			{
				return new LocalizedString("ContactCsvFileContainsNoKnownColumns", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabsMailboxQuotaWarningWithDisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("LabsMailboxQuotaWarningWithDisabledDetailedStatus", "ExB10331", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("DelayedDetailedStatus", "ExDDC385", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HttpResponseStreamNullException
		{
			get
			{
				return new LocalizedString("HttpResponseStreamNullException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveSubscriptionStatus
		{
			get
			{
				return new LocalizedString("RemoveSubscriptionStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisabledDetailedStatus
		{
			get
			{
				return new LocalizedString("DisabledDetailedStatus", "ExE5870F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAggregationSubscriptionIdentity
		{
			get
			{
				return new LocalizedString("InvalidAggregationSubscriptionIdentity", "Ex66A82E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPJunkEmail
		{
			get
			{
				return new LocalizedString("IMAPJunkEmail", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisabledStatus
		{
			get
			{
				return new LocalizedString("DisabledStatus", "ExE1EB04", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxedOutSyncRelationshipsErrorWithDisabledStatus
		{
			get
			{
				return new LocalizedString("MaxedOutSyncRelationshipsErrorWithDisabledStatus", "ExB49C16", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstAppendToNotes
		{
			get
			{
				return new LocalizedString("FirstAppendToNotes", "Ex95492E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCsvFileFormat
		{
			get
			{
				return new LocalizedString("InvalidCsvFileFormat", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyFoldersException(int maxNumber)
		{
			return new LocalizedString("TooManyFoldersException", "", false, false, Strings.ResourceManager, new object[]
			{
				maxNumber
			});
		}

		public static LocalizedString SubscriptionNameAlreadyExists(string name)
		{
			return new LocalizedString("SubscriptionNameAlreadyExists", "ExB14F4B", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DeltaSyncServiceEndpointsLoadException
		{
			get
			{
				return new LocalizedString("DeltaSyncServiceEndpointsLoadException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonDetailedStatus
		{
			get
			{
				return new LocalizedString("PoisonDetailedStatus", "ExC70BEB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoProvisionTestHotmail
		{
			get
			{
				return new LocalizedString("AutoProvisionTestHotmail", "Ex8CD58D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyFoldersErrorBody(int maxFolders, string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("TooManyFoldersErrorBody", "Ex6DF77D", false, true, Strings.ResourceManager, new object[]
			{
				maxFolders,
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString RemoteServerIsSlowStatus
		{
			get
			{
				return new LocalizedString("RemoteServerIsSlowStatus", "Ex501EDD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionUpdatePermanentException
		{
			get
			{
				return new LocalizedString("SubscriptionUpdatePermanentException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonousErrorBody
		{
			get
			{
				return new LocalizedString("PoisonousErrorBody", "ExEF3233", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3NonCompliantServerException
		{
			get
			{
				return new LocalizedString("Pop3NonCompliantServerException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuccessStatus
		{
			get
			{
				return new LocalizedString("SuccessStatus", "Ex3072EC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorDetailedStatus
		{
			get
			{
				return new LocalizedString("ConnectionErrorDetailedStatus", "Ex385FD7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabsMailboxQuotaWarningWithDelayedStatus
		{
			get
			{
				return new LocalizedString("LabsMailboxQuotaWarningWithDelayedStatus", "ExBFDF43", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IMAPGmailNotSupportedException
		{
			get
			{
				return new LocalizedString("IMAPGmailNotSupportedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Pop3CannotConnectToServerException
		{
			get
			{
				return new LocalizedString("Pop3CannotConnectToServerException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerIsSlowDelayedDetailedStatus
		{
			get
			{
				return new LocalizedString("RemoteServerIsSlowDelayedDetailedStatus", "Ex13382A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabsMailboxQuoteWarningBody(string connectedAccountsDetailsLinkedText)
		{
			return new LocalizedString("LabsMailboxQuoteWarningBody", "ExE5EEFF", false, true, Strings.ResourceManager, new object[]
			{
				connectedAccountsDetailsLinkedText
			});
		}

		public static LocalizedString ConnectionErrorDetailedStatusDay(int day)
		{
			return new LocalizedString("ConnectionErrorDetailedStatusDay", "Ex4D4EB0", false, true, Strings.ResourceManager, new object[]
			{
				day
			});
		}

		public static LocalizedString IMAPSent
		{
			get
			{
				return new LocalizedString("IMAPSent", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreRestartedException
		{
			get
			{
				return new LocalizedString("StoreRestartedException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessTokenNullOrEmpty
		{
			get
			{
				return new LocalizedString("AccessTokenNullOrEmpty", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionErrorDetailedStatusHours(int hours)
		{
			return new LocalizedString("ConnectionErrorDetailedStatusHours", "Ex757E95", false, true, Strings.ResourceManager, new object[]
			{
				hours
			});
		}

		public static LocalizedString Pop3LeaveOnServerNotPossibleException
		{
			get
			{
				return new LocalizedString("Pop3LeaveOnServerNotPossibleException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncPropertyValidationException(string property, string value)
		{
			return new LocalizedString("SyncPropertyValidationException", "", false, false, Strings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString AutoProvisionTestPop3
		{
			get
			{
				return new LocalizedString("AutoProvisionTestPop3", "ExDF5D32", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSyncEngineStateException
		{
			get
			{
				return new LocalizedString("InvalidSyncEngineStateException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStoreUnhealthyExceptionInfo(Guid databaseGuid, int backOff)
		{
			return new LocalizedString("SyncStoreUnhealthyExceptionInfo", "", false, false, Strings.ResourceManager, new object[]
			{
				databaseGuid,
				backOff
			});
		}

		public static LocalizedString ContactCsvFileEmpty
		{
			get
			{
				return new LocalizedString("ContactCsvFileEmpty", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestContentException(int statusCode)
		{
			return new LocalizedString("RequestContentException", "", false, false, Strings.ResourceManager, new object[]
			{
				statusCode
			});
		}

		public static LocalizedString FacebookNonPromotableTransientException
		{
			get
			{
				return new LocalizedString("FacebookNonPromotableTransientException", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxedOutSyncRelationshipsErrorWithDelayedStatus
		{
			get
			{
				return new LocalizedString("MaxedOutSyncRelationshipsErrorWithDelayedStatus", "Ex044E5F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactCsvFileTooLarge(int maxContacts)
		{
			return new LocalizedString("ContactCsvFileTooLarge", "", false, false, Strings.ResourceManager, new object[]
			{
				maxContacts
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(126);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.Sync.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			IMAPUnsupportedVersionException = 995731409U,
			NestedFoldersNotAllowedException = 147813322U,
			LeaveOnServerNotSupportedStatus = 1898862428U,
			IMAPTrash = 1598126189U,
			Pop3MirroredAccountNotPossibleException = 1507388000U,
			RemoteMailboxQuotaWarningWithDisabledStatus = 4121087442U,
			SubscriptionInvalidPasswordException = 3959479424U,
			SubscriptionUpdateTransientException = 1971329929U,
			ConnectionErrorWithDisabledStatus = 1075040750U,
			ConnectedAccountsDetails = 3599922107U,
			IMAPAllMail = 3969501405U,
			MTOMParsingFailedException = 2030480939U,
			LabsMailboxQuotaWarningWithDelayedDetailedStatus = 4079355374U,
			AuthenticationErrorWithDelayedDetailedStatus = 3801534216U,
			RemoteServerIsBackedOffException = 1261870541U,
			LinkedInNonPromotableTransientException = 1401415251U,
			RetryLaterException = 2432721285U,
			ProviderExceptionDetailedStatus = 2971027178U,
			MessageSizeLimitExceededException = 3410639299U,
			CommunicationErrorWithDelayedStatus = 1298696584U,
			MailboxFailure = 604756220U,
			Pop3CapabilitiesNotSupportedException = 3346793726U,
			IMAPInvalidServerException = 580763412U,
			LeaveOnServerNotSupportedDetailedStatus = 3937165620U,
			ConnectionClosedException = 3902086079U,
			PoisonousRemoteServerException = 2952144307U,
			SubscriptionInvalidVersionException = 2394731155U,
			IMAPSentMail = 3561039784U,
			InvalidVersionDetailedStatus = 1756157035U,
			CommunicationErrorWithDisabledStatus = 3613930518U,
			InProgressDetailedStatus = 3694666204U,
			AutoProvisionTestImap = 517632853U,
			DelayedStatus = 3137325842U,
			ProviderExceptionStatus = 3444450370U,
			CommunicationErrorWithDisabledDetailedStatus = 3530466102U,
			RemoteServerIsSlowDisabledDetailedStatus = 4176165696U,
			SendAsVerificationBottomBlock = 2010701853U,
			AuthenticationErrorWithDisabledStatus = 3292189374U,
			ConnectionDownloadedLimitExceededException = 591776832U,
			MailboxOverQuotaException = 1477720531U,
			InvalidServerResponseException = 2149789026U,
			SyncStateSizeErrorDetailedStatus = 2425965665U,
			RemoteAccountDoesNotExistDetailedStatus = 2492614440U,
			IMAPDrafts = 710927991U,
			MaxedOutSyncRelationshipsErrorWithDelayedDetailedStatus = 3842730909U,
			IMAPInvalidItemException = 1812618676U,
			SendAsVerificationSender = 2019254494U,
			IMAPDraft = 2713854040U,
			IMAPSentMessages = 882640929U,
			TlsRemoteCertificateInvalid = 4155460975U,
			Pop3AuthErrorException = 802159867U,
			PoisonStatus = 2900222280U,
			MessageIdGenerationTransientException = 2053678515U,
			AutoProvisionTestAutoDiscover = 3896029100U,
			Pop3TransientSystemAuthErrorException = 3699840920U,
			SyncConflictException = 2759557964U,
			IMAPSentItems = 76218079U,
			SyncEngineSyncStorageProviderCreationException = 1404272750U,
			Pop3TransientLoginDelayedAuthErrorException = 2942865760U,
			IMAPDeletedMessages = 3228154330U,
			RemoteMailboxQuotaWarningWithDelayedStatus = 1124604668U,
			SyncStateSizeErrorStatus = 1488733145U,
			MissingServerResponseException = 4092585897U,
			LabsMailboxQuotaWarningWithDisabledStatus = 4257197704U,
			Pop3TransientInUseAuthErrorException = 4287770443U,
			Pop3DisabledResponseException = 3000749982U,
			SendAsVerificationSubject = 233413355U,
			IMAPAuthenticationException = 1726233450U,
			FailedToGenerateVerificationEmail = 1413995526U,
			RemoteMailboxQuotaWarningWithDelayedDetailedStatus = 1235995580U,
			AuthenticationErrorWithDisabledDetailedStatus = 715816254U,
			ConfigurationErrorStatus = 1257586764U,
			AuthenticationErrorWithDelayedStatus = 189434176U,
			MaxedOutSyncRelationshipsErrorBody = 2936000979U,
			HotmailAccountVerificationFailedException = 2363855804U,
			AutoProvisionResults = 1548922274U,
			InvalidVersionStatus = 2204337803U,
			IMAPDeletedItems = 2984668342U,
			TooManyFoldersStatus = 601262870U,
			SendAsVerificationSignatureTopPart = 2617144415U,
			ConnectionErrorWithDelayedStatus = 2808236588U,
			IMAPJunk = 3760020583U,
			RemoteAccountDoesNotExistStatus = 974222160U,
			TooManyFoldersDetailedStatus = 2167449654U,
			SuccessDetailedStatus = 925456835U,
			InProgressStatus = 2318825308U,
			SubscriptionNotificationEmailBodyStartText = 3335416471U,
			MaxedOutSyncRelationshipsErrorWithDisabledDetailedStatus = 3933215749U,
			IMAPSpam = 2173734488U,
			RemoteMailboxQuotaWarningWithDisabledDetailedStatus = 1734582074U,
			SendAsVerificationTopBlock = 650547385U,
			CommunicationErrorWithDelayedDetailedStatus = 3930645136U,
			ContactCsvFileContainsNoKnownColumns = 4038775200U,
			LabsMailboxQuotaWarningWithDisabledDetailedStatus = 82266192U,
			DelayedDetailedStatus = 420875274U,
			HttpResponseStreamNullException = 3447698883U,
			RemoveSubscriptionStatus = 1059234861U,
			DisabledDetailedStatus = 2325761520U,
			InvalidAggregationSubscriptionIdentity = 3091979858U,
			IMAPJunkEmail = 1325089515U,
			DisabledStatus = 3269288896U,
			MaxedOutSyncRelationshipsErrorWithDisabledStatus = 3779095989U,
			FirstAppendToNotes = 2754299458U,
			InvalidCsvFileFormat = 3826985530U,
			DeltaSyncServiceEndpointsLoadException = 348158591U,
			PoisonDetailedStatus = 2529513648U,
			AutoProvisionTestHotmail = 2425850576U,
			RemoteServerIsSlowStatus = 1096710760U,
			SubscriptionUpdatePermanentException = 623751595U,
			PoisonousErrorBody = 603913161U,
			Pop3NonCompliantServerException = 2979796742U,
			SuccessStatus = 1339731251U,
			ConnectionErrorDetailedStatus = 3068969418U,
			LabsMailboxQuotaWarningWithDelayedStatus = 3345637854U,
			IMAPGmailNotSupportedException = 2505373931U,
			Pop3CannotConnectToServerException = 521865336U,
			RemoteServerIsSlowDelayedDetailedStatus = 2222507634U,
			IMAPSent = 2839399357U,
			StoreRestartedException = 2049114804U,
			AccessTokenNullOrEmpty = 3011192446U,
			Pop3LeaveOnServerNotPossibleException = 1745238072U,
			AutoProvisionTestPop3 = 3743909068U,
			InvalidSyncEngineStateException = 3420783178U,
			ContactCsvFileEmpty = 3910384057U,
			FacebookNonPromotableTransientException = 4000016521U,
			MaxedOutSyncRelationshipsErrorWithDelayedStatus = 305729253U
		}

		private enum ParamIDs
		{
			DelayedDetailedStatusHours,
			UnexpectedContentTypeException,
			SendAsVerificationSalutation,
			SubscriptionInconsistent,
			UserAccessException,
			LeaveOnServerNotSupportedErrorBody,
			RemoteAccountDoesNotExistBody,
			ConnectionErrorBody,
			SyncTooSlowException,
			ConnectionErrorBodyHour,
			MessageDecompressionFailedException,
			FailedSetAggregationSubscription,
			FailedDeleteAggregationSubscription,
			PartnerAuthenticationException,
			FailedCreateAggregationSubscription,
			SubscriptionNotFound,
			MailboxPermanentErrorSavingContact,
			MultipleNativeItemsHaveSameCloudIdException,
			MailboxTransientExceptionSavingContact,
			ConnectionErrorDetailedStatusHour,
			SyncPoisonItemFoundException,
			ConnectionErrorBodyHours,
			SubscriptionSyncException,
			CorruptSubscriptionException,
			DelayedDetailedStatusDay,
			QuotaExceededSavingContact,
			Pop3ErrorResponseException,
			AutoProvisionStatus,
			InternalErrorSavingContact,
			DeltaSyncServerException,
			IMAPException,
			SubscriptionNotificationEmailSubject,
			SyncUnhandledException,
			ConnectionErrorDetailedStatusDays,
			Pop3PermErrorResponseException,
			DelayedDetailedStatusHour,
			FailedDeletePeopleConnectSubscription,
			TlsFailureException,
			RemoteServerTooSlowException,
			Pop3BrokenResponseException,
			RemoteServerIsSlowErrorBody,
			CommunicationErrorBody,
			UncompressedSyncStateSizeExceededException,
			AuthenticationErrorBody,
			RedundantPimSubscription,
			SyncStateSizeErrorBody,
			Pop3UnknownResponseException,
			CompressedSyncStateSizeExceededException,
			DelayedDetailedStatusDays,
			ConnectionErrorBodyDays,
			TlsFailureErrorOccurred,
			UnknownDeltaSyncException,
			RequestFormatException,
			UnresolveableFolderNameException,
			RedundantAccountSubscription,
			RequestSettingsException,
			ConnectionErrorBodyDay,
			SubscriptionNumberExceedLimit,
			DataOutOfSyncException,
			TooManyFoldersException,
			SubscriptionNameAlreadyExists,
			TooManyFoldersErrorBody,
			LabsMailboxQuoteWarningBody,
			ConnectionErrorDetailedStatusDay,
			ConnectionErrorDetailedStatusHours,
			SyncPropertyValidationException,
			SyncStoreUnhealthyExceptionInfo,
			RequestContentException,
			ContactCsvFileTooLarge
		}
	}
}
