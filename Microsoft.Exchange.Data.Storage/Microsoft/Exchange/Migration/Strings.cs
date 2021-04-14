using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(86638814U, "RemoteMailboxQuotaWarningStatus");
			Strings.stringIDs.Add(1530080577U, "CompletingMigrationJobCannotBeAppendedTo");
			Strings.stringIDs.Add(2718293401U, "MigrationReportJobInitialSyncComplete");
			Strings.stringIDs.Add(2060675488U, "LabelRunTime");
			Strings.stringIDs.Add(149572871U, "CorruptedMigrationBatchCannotBeStopped");
			Strings.stringIDs.Add(1676389812U, "MigrationExchangeRpcConnectionFailure");
			Strings.stringIDs.Add(1899202903U, "MigrationJobCannotBeRemoved");
			Strings.stringIDs.Add(3099209002U, "CannotSpecifyUnicodeInCredentials");
			Strings.stringIDs.Add(1798653784U, "MigrationCancelledByUserRequest");
			Strings.stringIDs.Add(4872333U, "FailureDuringRemoval");
			Strings.stringIDs.Add(223688197U, "FinalizationErrorSummaryRetryMessage");
			Strings.stringIDs.Add(3674744977U, "MigrationJobAlreadyStopping");
			Strings.stringIDs.Add(2530769896U, "MigrationJobCannotBeStopped");
			Strings.stringIDs.Add(1168096436U, "SubscriptionNotFound");
			Strings.stringIDs.Add(3851331319U, "ContactsMigration");
			Strings.stringIDs.Add(4089501710U, "EmailMigration");
			Strings.stringIDs.Add(3003515263U, "UserProvisioningInternalError");
			Strings.stringIDs.Add(1509794908U, "CompletedMigrationJobCannotBeStartedMultiBatch");
			Strings.stringIDs.Add(1898862428U, "LeaveOnServerNotSupportedStatus");
			Strings.stringIDs.Add(2871031082U, "MigrationLocalDatabasesNotFound");
			Strings.stringIDs.Add(829120357U, "OnlyOnePublicFolderBatchIsAllowedError");
			Strings.stringIDs.Add(1144436407U, "LabelFileName");
			Strings.stringIDs.Add(3073036977U, "MigrationTempMissingDatabase");
			Strings.stringIDs.Add(1781379657U, "MigrationDataCorruptionError");
			Strings.stringIDs.Add(1300976335U, "CutoverAndStagedBatchesCannotCoexistError");
			Strings.stringIDs.Add(2350676710U, "MaximumNumberOfBatchesReached");
			Strings.stringIDs.Add(4040662282U, "PublicFolderMailboxesNotProvisionedError");
			Strings.stringIDs.Add(2999872780U, "CannotSpecifyUnicodeUserIdPasswordWithBasicAuth");
			Strings.stringIDs.Add(853242704U, "CompletingMigrationJobCannotBeStarted");
			Strings.stringIDs.Add(514394488U, "MigrationJobAlreadyStarted");
			Strings.stringIDs.Add(850869622U, "LabelTotalMailboxes");
			Strings.stringIDs.Add(2694765716U, "MigrationJobAlreadyStopped");
			Strings.stringIDs.Add(391393381U, "MigrationGenericError");
			Strings.stringIDs.Add(3393024142U, "CommunicationErrorStatus");
			Strings.stringIDs.Add(3351583406U, "ErrorReportLink");
			Strings.stringIDs.Add(2548932531U, "LabelStartDateTime");
			Strings.stringIDs.Add(1956114709U, "MigrationJobDoesNotSupportAppendingUserCSV");
			Strings.stringIDs.Add(934722052U, "SubscriptionRpcThresholdExceeded");
			Strings.stringIDs.Add(2142750819U, "CorruptedMigrationBatchCannotBeCompleted");
			Strings.stringIDs.Add(437021588U, "RemovedMigrationJobCannotBeStarted");
			Strings.stringIDs.Add(2530286416U, "ErrorCouldNotDeserializeConnectionSettings");
			Strings.stringIDs.Add(1754366117U, "UnknownTimespan");
			Strings.stringIDs.Add(1419436392U, "LabsMailboxQuotaWarningStatus");
			Strings.stringIDs.Add(2571873410U, "ConnectionErrorStatus");
			Strings.stringIDs.Add(1951127083U, "StatisticsReportLink");
			Strings.stringIDs.Add(571725646U, "RemoteServerIsSlow");
			Strings.stringIDs.Add(672478732U, "ErrorMigrationMailboxMissingOrInvalid");
			Strings.stringIDs.Add(1978952169U, "CompletedMigrationJobCannotBeStarted");
			Strings.stringIDs.Add(2036241881U, "MigrationReportFailed");
			Strings.stringIDs.Add(1271432035U, "MigrationCancelledDueToInternalError");
			Strings.stringIDs.Add(1786933629U, "PublicFolderMigrationBatchCannotBeCompletedWithErrors");
			Strings.stringIDs.Add(85986654U, "MigrationJobCannotRetryCompletion");
			Strings.stringIDs.Add(3662584967U, "MissingRequiredSubscriptionId");
			Strings.stringIDs.Add(1905833821U, "IMAPPathPrefixInvalidStatus");
			Strings.stringIDs.Add(601262870U, "TooManyFoldersStatus");
			Strings.stringIDs.Add(1066087601U, "LabelSubmittedByUser");
			Strings.stringIDs.Add(1842619565U, "CSVFileTooLarge");
			Strings.stringIDs.Add(1189164420U, "RemovedMigrationJobCannotBeModified");
			Strings.stringIDs.Add(4257873367U, "LabelCompleted");
			Strings.stringIDs.Add(1047974326U, "MigrationPublicFolderWireUpFailed");
			Strings.stringIDs.Add(1386196812U, "LabelCouldntMigrate");
			Strings.stringIDs.Add(3955323767U, "MigrationTenantPermissionFailure");
			Strings.stringIDs.Add(1043780694U, "UnknownMigrationBatchError");
			Strings.stringIDs.Add(1332982264U, "MigrationReportJobComplete");
			Strings.stringIDs.Add(367906109U, "UnableToDisableSubscription");
			Strings.stringIDs.Add(1279919936U, "OnlyOneCutoverBatchIsAllowedError");
			Strings.stringIDs.Add(2536192239U, "CouldNotUpdateSubscriptionSettingsWithoutBatch");
			Strings.stringIDs.Add(3327693399U, "CorruptedMigrationBatchCannotBeStarted");
			Strings.stringIDs.Add(617690654U, "ProvisioningThrottledBack");
			Strings.stringIDs.Add(4177113737U, "CompletedMigrationJobCannotBeModified");
			Strings.stringIDs.Add(3751987975U, "CorruptedMigrationBatchCannotBeModified");
			Strings.stringIDs.Add(5503809U, "LabelTotalRows");
			Strings.stringIDs.Add(1136304310U, "LabelSynced");
			Strings.stringIDs.Add(1088630310U, "CouldNotDiscoverNSPISettings");
			Strings.stringIDs.Add(16533286U, "SyncingMigrationJobCannotBeAppendedTo");
			Strings.stringIDs.Add(4038100652U, "MigrationExchangeCredentialFailure");
			Strings.stringIDs.Add(1756157035U, "InvalidVersionDetailedStatus");
			Strings.stringIDs.Add(2104158819U, "MigrationReportJobAutoStarted");
			Strings.stringIDs.Add(320046086U, "RemovingMigrationUserBatchMustBeIdle");
			Strings.stringIDs.Add(759640474U, "NoDataMigrated");
			Strings.stringIDs.Add(2605498839U, "SubscriptionDisabledSinceFinalized");
			Strings.stringIDs.Add(2047379006U, "AuthenticationErrorStatus");
			Strings.stringIDs.Add(986156097U, "MigrationReportJobTransientError");
			Strings.stringIDs.Add(1214466260U, "MigrationJobAlreadyHasPendingCSV");
			Strings.stringIDs.Add(3504597832U, "ErrorConnectionSettingsNotSerialized");
			Strings.stringIDs.Add(1236042521U, "PasswordPreviouslySet");
			Strings.stringIDs.Add(3020735710U, "MigrationJobAlreadyQueued");
			Strings.stringIDs.Add(2529513648U, "PoisonDetailedStatus");
			Strings.stringIDs.Add(1500792389U, "NoPartialMigrationSummaryMessage");
			Strings.stringIDs.Add(3968870632U, "StoppingMigrationJobCannotBeStarted");
			Strings.stringIDs.Add(1142848800U, "FailedToReadPublicFoldersOnPremise");
			Strings.stringIDs.Add(1731044986U, "MailboxNotFoundSubscriptionStatus");
			Strings.stringIDs.Add(1197149433U, "ReadPublicFoldersOnTargetInternalError");
			Strings.stringIDs.Add(3334133771U, "ExternallySuspendedFailure");
			Strings.stringIDs.Add(4060495101U, "SyncStateSizeError");
			Strings.stringIDs.Add(1246705638U, "LabelLogMailFooter");
			Strings.stringIDs.Add(4122602291U, "CorruptedSubscriptionStatus");
			Strings.stringIDs.Add(295927301U, "SignInMightBeRequired");
			Strings.stringIDs.Add(4162391104U, "MigrationTempMissingMigrationMailbox");
			Strings.stringIDs.Add(501292080U, "MigrationJobCannotBeCompleted");
			Strings.stringIDs.Add(2595351182U, "UnknownMigrationError");
			Strings.stringIDs.Add(1980396424U, "CompleteMigrationBatchNotSupported");
			Strings.stringIDs.Add(2353197456U, "ConfigAccessRuntimeError");
		}

		public static LocalizedString RemoteMailboxQuotaWarningStatus
		{
			get
			{
				return new LocalizedString("RemoteMailboxQuotaWarningStatus", "Ex434D8F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompletingMigrationJobCannotBeAppendedTo
		{
			get
			{
				return new LocalizedString("CompletingMigrationJobCannotBeAppendedTo", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchCompletionReportMailErrorHeader(string batchName)
		{
			return new LocalizedString("MigrationBatchCompletionReportMailErrorHeader", "ExA4F7F4", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString MigrationReportJobItemCorrupted(string user)
		{
			return new LocalizedString("MigrationReportJobItemCorrupted", "", false, false, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString UnsupportedSourceRecipientTypeError(string type)
		{
			return new LocalizedString("UnsupportedSourceRecipientTypeError", "Ex3271B6", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString MigrationJobCannotBeDeletedWithPendingItems(int count)
		{
			return new LocalizedString("MigrationJobCannotBeDeletedWithPendingItems", "", false, false, Strings.ResourceManager, new object[]
			{
				count
			});
		}

		public static LocalizedString UnsupportedAdminCulture(string culture)
		{
			return new LocalizedString("UnsupportedAdminCulture", "", false, false, Strings.ResourceManager, new object[]
			{
				culture
			});
		}

		public static LocalizedString MigrationProcessorInvalidation(string processor, string jobname)
		{
			return new LocalizedString("MigrationProcessorInvalidation", "", false, false, Strings.ResourceManager, new object[]
			{
				processor,
				jobname
			});
		}

		public static LocalizedString MigrationReportJobInitialSyncComplete
		{
			get
			{
				return new LocalizedString("MigrationReportJobInitialSyncComplete", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobItemNotFound(string identity)
		{
			return new LocalizedString("MigrationJobItemNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString LabelRunTime
		{
			get
			{
				return new LocalizedString("LabelRunTime", "Ex6C0D2C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFeatureNotSupported(string feature)
		{
			return new LocalizedString("MigrationFeatureNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				feature
			});
		}

		public static LocalizedString CorruptedMigrationBatchCannotBeStopped
		{
			get
			{
				return new LocalizedString("CorruptedMigrationBatchCannotBeStopped", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationJobNotFound", "ExBDDC58", false, true, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString MigrationExchangeRpcConnectionFailure
		{
			get
			{
				return new LocalizedString("MigrationExchangeRpcConnectionFailure", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobCannotBeRemoved
		{
			get
			{
				return new LocalizedString("MigrationJobCannotBeRemoved", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNspiNotSupportedForEndpointType(MigrationType type)
		{
			return new LocalizedString("ErrorNspiNotSupportedForEndpointType", "", false, false, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString UserDuplicateInCSV(string alias)
		{
			return new LocalizedString("UserDuplicateInCSV", "Ex75BB6D", false, true, Strings.ResourceManager, new object[]
			{
				alias
			});
		}

		public static LocalizedString MigrationOrganizationNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationOrganizationNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString CannotSpecifyUnicodeInCredentials
		{
			get
			{
				return new LocalizedString("CannotSpecifyUnicodeInCredentials", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportRepaired(string user, string action)
		{
			return new LocalizedString("MigrationReportRepaired", "", false, false, Strings.ResourceManager, new object[]
			{
				user,
				action
			});
		}

		public static LocalizedString LabelAutoRetry(int attemptsRemaining)
		{
			return new LocalizedString("LabelAutoRetry", "", false, false, Strings.ResourceManager, new object[]
			{
				attemptsRemaining
			});
		}

		public static LocalizedString MigrationCancelledByUserRequest
		{
			get
			{
				return new LocalizedString("MigrationCancelledByUserRequest", "ExA58CCA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailureDuringRemoval
		{
			get
			{
				return new LocalizedString("FailureDuringRemoval", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FinalizationErrorSummaryRetryMessage
		{
			get
			{
				return new LocalizedString("FinalizationErrorSummaryRetryMessage", "ExC05D2F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotLoadMigrationPersistedItem(string itemId)
		{
			return new LocalizedString("CouldNotLoadMigrationPersistedItem", "", false, false, Strings.ResourceManager, new object[]
			{
				itemId
			});
		}

		public static LocalizedString MigrationReportJobItemRetried(string user)
		{
			return new LocalizedString("MigrationReportJobItemRetried", "", false, false, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString MigrationItemLastUpdatedInTheFuture(string time)
		{
			return new LocalizedString("MigrationItemLastUpdatedInTheFuture", "", false, false, Strings.ResourceManager, new object[]
			{
				time
			});
		}

		public static LocalizedString MigrationJobAlreadyStopping
		{
			get
			{
				return new LocalizedString("MigrationJobAlreadyStopping", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobCannotBeStopped
		{
			get
			{
				return new LocalizedString("MigrationJobCannotBeStopped", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUpdateRecipientSource(string targetSmtpAddress)
		{
			return new LocalizedString("FailedToUpdateRecipientSource", "ExE49A98", false, true, Strings.ResourceManager, new object[]
			{
				targetSmtpAddress
			});
		}

		public static LocalizedString MigrationReportJobCreated(string userName, MigrationType migrationType)
		{
			return new LocalizedString("MigrationReportJobCreated", "", false, false, Strings.ResourceManager, new object[]
			{
				userName,
				migrationType
			});
		}

		public static LocalizedString MigrationSubscriptionCreationFailed(string user)
		{
			return new LocalizedString("MigrationSubscriptionCreationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString SubscriptionNotFound
		{
			get
			{
				return new LocalizedString("SubscriptionNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportNspiFailed(string context, string status)
		{
			return new LocalizedString("MigrationReportNspiFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				context,
				status
			});
		}

		public static LocalizedString ContactsMigration
		{
			get
			{
				return new LocalizedString("ContactsMigration", "Ex8D84CD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedSubscriptionStatus(string status)
		{
			return new LocalizedString("UnexpectedSubscriptionStatus", "", false, false, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString EmailMigration
		{
			get
			{
				return new LocalizedString("EmailMigration", "ExEB071C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationRpcFailed(string result)
		{
			return new LocalizedString("MigrationRpcFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				result
			});
		}

		public static LocalizedString UserProvisioningInternalError
		{
			get
			{
				return new LocalizedString("UserProvisioningInternalError", "ExF29C1F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchCompletionReportMailHeader(string batchName)
		{
			return new LocalizedString("MigrationBatchCompletionReportMailHeader", "Ex37C091", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString MigrationReportNspiRfrFailed(string context, string status)
		{
			return new LocalizedString("MigrationReportNspiRfrFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				context,
				status
			});
		}

		public static LocalizedString CompletedMigrationJobCannotBeStartedMultiBatch
		{
			get
			{
				return new LocalizedString("CompletedMigrationJobCannotBeStartedMultiBatch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationVersionMismatch(long version, long expectedVersion)
		{
			return new LocalizedString("MigrationVersionMismatch", "", false, false, Strings.ResourceManager, new object[]
			{
				version,
				expectedVersion
			});
		}

		public static LocalizedString MigrationBatchCancelledBySystem(string batchName)
		{
			return new LocalizedString("MigrationBatchCancelledBySystem", "Ex1516F4", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString BatchCancelledByUser(string fileName)
		{
			return new LocalizedString("BatchCancelledByUser", "Ex59EB12", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString LeaveOnServerNotSupportedStatus
		{
			get
			{
				return new LocalizedString("LeaveOnServerNotSupportedStatus", "ExE8E9AF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRootFolderMappingInCSVError(int rowIndex, string folderPath, string identifier, string hierarchyMailboxName)
		{
			return new LocalizedString("InvalidRootFolderMappingInCSVError", "", false, false, Strings.ResourceManager, new object[]
			{
				rowIndex,
				folderPath,
				identifier,
				hierarchyMailboxName
			});
		}

		public static LocalizedString DuplicateFolderInCSVError(int rowIndex, string folderPath, string identifier)
		{
			return new LocalizedString("DuplicateFolderInCSVError", "", false, false, Strings.ResourceManager, new object[]
			{
				rowIndex,
				folderPath,
				identifier
			});
		}

		public static LocalizedString MigrationLocalDatabasesNotFound
		{
			get
			{
				return new LocalizedString("MigrationLocalDatabasesNotFound", "ExC4DB1F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyTransientFailures(string batchIdentity)
		{
			return new LocalizedString("ErrorTooManyTransientFailures", "", false, false, Strings.ResourceManager, new object[]
			{
				batchIdentity
			});
		}

		public static LocalizedString OnlyOnePublicFolderBatchIsAllowedError
		{
			get
			{
				return new LocalizedString("OnlyOnePublicFolderBatchIsAllowedError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelFileName
		{
			get
			{
				return new LocalizedString("LabelFileName", "Ex57FED3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTempMissingDatabase
		{
			get
			{
				return new LocalizedString("MigrationTempMissingDatabase", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationDataCorruptionError
		{
			get
			{
				return new LocalizedString("MigrationDataCorruptionError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CutoverAndStagedBatchesCannotCoexistError
		{
			get
			{
				return new LocalizedString("CutoverAndStagedBatchesCannotCoexistError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunTimeFormatHours(int hours, int minutes)
		{
			return new LocalizedString("RunTimeFormatHours", "ExC7DA67", false, true, Strings.ResourceManager, new object[]
			{
				hours,
				minutes
			});
		}

		public static LocalizedString MaximumNumberOfBatchesReached
		{
			get
			{
				return new LocalizedString("MaximumNumberOfBatchesReached", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderMailboxesNotProvisionedError
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxesNotProvisionedError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobItemRemovedInternal(string identity)
		{
			return new LocalizedString("MigrationReportJobItemRemovedInternal", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString RecipientDoesNotExistAtSource(string email)
		{
			return new LocalizedString("RecipientDoesNotExistAtSource", "Ex4F0A6F", false, true, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString CannotSpecifyUnicodeUserIdPasswordWithBasicAuth
		{
			get
			{
				return new LocalizedString("CannotSpecifyUnicodeUserIdPasswordWithBasicAuth", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobItemCreatedInternal(string identity)
		{
			return new LocalizedString("MigrationReportJobItemCreatedInternal", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString CompletingMigrationJobCannotBeStarted
		{
			get
			{
				return new LocalizedString("CompletingMigrationJobCannotBeStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobAlreadyStarted
		{
			get
			{
				return new LocalizedString("MigrationJobAlreadyStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSummaryMessageSingular(int errorCount)
		{
			return new LocalizedString("ErrorSummaryMessageSingular", "ExC5BA76", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString MigrationReportJobItemWithError(string user, LocalizedString message)
		{
			return new LocalizedString("MigrationReportJobItemWithError", "", false, false, Strings.ResourceManager, new object[]
			{
				user,
				message
			});
		}

		public static LocalizedString ErrorUserAlreadyBeingMigrated(string identity)
		{
			return new LocalizedString("ErrorUserAlreadyBeingMigrated", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MigrationBatchReportMailHeader(string batchName)
		{
			return new LocalizedString("MigrationBatchReportMailHeader", "", false, false, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString DetailedAggregationStatus(string code)
		{
			return new LocalizedString("DetailedAggregationStatus", "", false, false, Strings.ResourceManager, new object[]
			{
				code
			});
		}

		public static LocalizedString LabelTotalMailboxes
		{
			get
			{
				return new LocalizedString("LabelTotalMailboxes", "Ex4D931B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobAlreadyStopped
		{
			get
			{
				return new LocalizedString("MigrationJobAlreadyStopped", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationGenericError
		{
			get
			{
				return new LocalizedString("MigrationGenericError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedTargetRecipientTypeError(string type)
		{
			return new LocalizedString("UnsupportedTargetRecipientTypeError", "ExFB3FDA", false, true, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString CommunicationErrorStatus
		{
			get
			{
				return new LocalizedString("CommunicationErrorStatus", "Ex0866B8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReportLink
		{
			get
			{
				return new LocalizedString("ErrorReportLink", "Ex779419", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelStartDateTime
		{
			get
			{
				return new LocalizedString("LabelStartDateTime", "Ex797C8C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserMissingOrWithoutRBAC(string username)
		{
			return new LocalizedString("ErrorUserMissingOrWithoutRBAC", "", false, false, Strings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString PartialMigrationSummaryMessagePlural(int partialMigrationCount)
		{
			return new LocalizedString("PartialMigrationSummaryMessagePlural", "Ex63E0A5", false, true, Strings.ResourceManager, new object[]
			{
				partialMigrationCount
			});
		}

		public static LocalizedString MigrationReportJobItemRemoved(string username, string identity)
		{
			return new LocalizedString("MigrationReportJobItemRemoved", "", false, false, Strings.ResourceManager, new object[]
			{
				username,
				identity
			});
		}

		public static LocalizedString MigrationJobDoesNotSupportAppendingUserCSV
		{
			get
			{
				return new LocalizedString("MigrationJobDoesNotSupportAppendingUserCSV", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionRpcThresholdExceeded
		{
			get
			{
				return new LocalizedString("SubscriptionRpcThresholdExceeded", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotUpgradeMigrationVersion(string explanation)
		{
			return new LocalizedString("CannotUpgradeMigrationVersion", "", false, false, Strings.ResourceManager, new object[]
			{
				explanation
			});
		}

		public static LocalizedString AutoDiscoverInternalError(LocalizedString details)
		{
			return new LocalizedString("AutoDiscoverInternalError", "", false, false, Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString MigrationUserAlreadyExistsInDifferentType(string batchName, string batchType)
		{
			return new LocalizedString("MigrationUserAlreadyExistsInDifferentType", "", false, false, Strings.ResourceManager, new object[]
			{
				batchName,
				batchType
			});
		}

		public static LocalizedString CorruptedMigrationBatchCannotBeCompleted
		{
			get
			{
				return new LocalizedString("CorruptedMigrationBatchCannotBeCompleted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemovedMigrationJobCannotBeStarted
		{
			get
			{
				return new LocalizedString("RemovedMigrationJobCannotBeStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCouldNotDeserializeConnectionSettings
		{
			get
			{
				return new LocalizedString("ErrorCouldNotDeserializeConnectionSettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownTimespan
		{
			get
			{
				return new LocalizedString("UnknownTimespan", "Ex20DFAB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabsMailboxQuotaWarningStatus
		{
			get
			{
				return new LocalizedString("LabsMailboxQuotaWarningStatus", "Ex953087", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobModifiedByUser(string userName)
		{
			return new LocalizedString("MigrationReportJobModifiedByUser", "", false, false, Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString ErrorInvalidRecipientType(string actual, string expected)
		{
			return new LocalizedString("ErrorInvalidRecipientType", "", false, false, Strings.ResourceManager, new object[]
			{
				actual,
				expected
			});
		}

		public static LocalizedString ConnectionErrorStatus
		{
			get
			{
				return new LocalizedString("ConnectionErrorStatus", "Ex23333F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserAlreadyRemoved(string user)
		{
			return new LocalizedString("MigrationUserAlreadyRemoved", "", false, false, Strings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString StatisticsReportLink
		{
			get
			{
				return new LocalizedString("StatisticsReportLink", "ExBC8D8F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerIsSlow
		{
			get
			{
				return new LocalizedString("RemoteServerIsSlow", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMigrationMailboxMissingOrInvalid
		{
			get
			{
				return new LocalizedString("ErrorMigrationMailboxMissingOrInvalid", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompletedMigrationJobCannotBeStarted
		{
			get
			{
				return new LocalizedString("CompletedMigrationJobCannotBeStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FinalizationErrorSummaryMessageSingular(int errorCount)
		{
			return new LocalizedString("FinalizationErrorSummaryMessageSingular", "Ex72CD80", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString ErrorParsingCSV(int rowIndex, string errorMessage)
		{
			return new LocalizedString("ErrorParsingCSV", "", false, false, Strings.ResourceManager, new object[]
			{
				rowIndex,
				errorMessage
			});
		}

		public static LocalizedString MigrationConfigString(string maxnumbatches, string features)
		{
			return new LocalizedString("MigrationConfigString", "", false, false, Strings.ResourceManager, new object[]
			{
				maxnumbatches,
				features
			});
		}

		public static LocalizedString MigrationReportFailed
		{
			get
			{
				return new LocalizedString("MigrationReportFailed", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSummaryMessagePlural(int errorCount)
		{
			return new LocalizedString("ErrorSummaryMessagePlural", "Ex1A34B0", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString ValueNotProvidedForColumn(string columnName)
		{
			return new LocalizedString("ValueNotProvidedForColumn", "ExFA1E73", false, true, Strings.ResourceManager, new object[]
			{
				columnName
			});
		}

		public static LocalizedString UserAlreadyMigratedWithAlternateEmail(string previous)
		{
			return new LocalizedString("UserAlreadyMigratedWithAlternateEmail", "", false, false, Strings.ResourceManager, new object[]
			{
				previous
			});
		}

		public static LocalizedString MigrationCancelledDueToInternalError
		{
			get
			{
				return new LocalizedString("MigrationCancelledDueToInternalError", "ExA70D62", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFinalizationReportMailErrorHeader(string batchName)
		{
			return new LocalizedString("MigrationFinalizationReportMailErrorHeader", "Ex1A8B79", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString PublicFolderMigrationBatchCannotBeCompletedWithErrors
		{
			get
			{
				return new LocalizedString("PublicFolderMigrationBatchCannotBeCompletedWithErrors", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAutoDiscoveryNotSupported(MigrationType type)
		{
			return new LocalizedString("ErrorAutoDiscoveryNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString MigrationJobCannotRetryCompletion
		{
			get
			{
				return new LocalizedString("MigrationJobCannotRetryCompletion", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingRequiredSubscriptionId
		{
			get
			{
				return new LocalizedString("MissingRequiredSubscriptionId", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunTimeFormatMinutes(int minutes)
		{
			return new LocalizedString("RunTimeFormatMinutes", "ExF67C7F", false, true, Strings.ResourceManager, new object[]
			{
				minutes
			});
		}

		public static LocalizedString RunTimeFormatDays(int days, int hours, int minutes)
		{
			return new LocalizedString("RunTimeFormatDays", "ExE5DB04", false, true, Strings.ResourceManager, new object[]
			{
				days,
				hours,
				minutes
			});
		}

		public static LocalizedString ErrorMissingExpectedCapability(string user, string capability)
		{
			return new LocalizedString("ErrorMissingExpectedCapability", "", false, false, Strings.ResourceManager, new object[]
			{
				user,
				capability
			});
		}

		public static LocalizedString IMAPPathPrefixInvalidStatus
		{
			get
			{
				return new LocalizedString("IMAPPathPrefixInvalidStatus", "ExCBABF6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportSetString(string creationTime, string successUrl, string errorUrl)
		{
			return new LocalizedString("MigrationReportSetString", "", false, false, Strings.ResourceManager, new object[]
			{
				creationTime,
				successUrl,
				errorUrl
			});
		}

		public static LocalizedString MigrationEndpointAlreadyExistsError(string endpointIdentity)
		{
			return new LocalizedString("MigrationEndpointAlreadyExistsError", "", false, false, Strings.ResourceManager, new object[]
			{
				endpointIdentity
			});
		}

		public static LocalizedString TooManyFoldersStatus
		{
			get
			{
				return new LocalizedString("TooManyFoldersStatus", "Ex2E6D50", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelSubmittedByUser
		{
			get
			{
				return new LocalizedString("LabelSubmittedByUser", "ExFB88A4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchReportMailErrorHeader(string batchName)
		{
			return new LocalizedString("MigrationBatchReportMailErrorHeader", "", false, false, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString CouldNotAddExchangeSnapIn(string snapInName)
		{
			return new LocalizedString("CouldNotAddExchangeSnapIn", "", false, false, Strings.ResourceManager, new object[]
			{
				snapInName
			});
		}

		public static LocalizedString ErrorNoEndpointSupportForMigrationType(MigrationType migrationType)
		{
			return new LocalizedString("ErrorNoEndpointSupportForMigrationType", "", false, false, Strings.ResourceManager, new object[]
			{
				migrationType
			});
		}

		public static LocalizedString CSVFileTooLarge
		{
			get
			{
				return new LocalizedString("CSVFileTooLarge", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemovedMigrationJobCannotBeModified
		{
			get
			{
				return new LocalizedString("RemovedMigrationJobCannotBeModified", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationRecipientNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationRecipientNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString ErrorCannotRemoveUserWithoutBatch(string userName)
		{
			return new LocalizedString("ErrorCannotRemoveUserWithoutBatch", "", false, false, Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString LabelCompleted
		{
			get
			{
				return new LocalizedString("LabelCompleted", "Ex70D639", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemoveEndpointWithAssociatedBatches(string endpointId, string batches)
		{
			return new LocalizedString("ErrorCannotRemoveEndpointWithAssociatedBatches", "", false, false, Strings.ResourceManager, new object[]
			{
				endpointId,
				batches
			});
		}

		public static LocalizedString MigrationPublicFolderWireUpFailed
		{
			get
			{
				return new LocalizedString("MigrationPublicFolderWireUpFailed", "ExB111AD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserAlreadyMigrated(string alias)
		{
			return new LocalizedString("UserAlreadyMigrated", "Ex2F7769", false, true, Strings.ResourceManager, new object[]
			{
				alias
			});
		}

		public static LocalizedString LabelCouldntMigrate
		{
			get
			{
				return new LocalizedString("LabelCouldntMigrate", "ExB6A237", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobItemFailed(string user, LocalizedString message)
		{
			return new LocalizedString("MigrationReportJobItemFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				user,
				message
			});
		}

		public static LocalizedString MigrationTenantPermissionFailure
		{
			get
			{
				return new LocalizedString("MigrationTenantPermissionFailure", "Ex5A26A0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownMigrationBatchError
		{
			get
			{
				return new LocalizedString("UnknownMigrationBatchError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobComplete
		{
			get
			{
				return new LocalizedString("MigrationReportJobComplete", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserMovedToAnotherBatch(string batchName)
		{
			return new LocalizedString("MigrationUserMovedToAnotherBatch", "", false, false, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString UnableToDisableSubscription
		{
			get
			{
				return new LocalizedString("UnableToDisableSubscription", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobAutomaticallyRestarting(int numOfFailures, int retryCount, int maxRetries)
		{
			return new LocalizedString("MigrationReportJobAutomaticallyRestarting", "", false, false, Strings.ResourceManager, new object[]
			{
				numOfFailures,
				retryCount,
				maxRetries
			});
		}

		public static LocalizedString ConfigKeyAccessRuntimeError(string keyname)
		{
			return new LocalizedString("ConfigKeyAccessRuntimeError", "", false, false, Strings.ResourceManager, new object[]
			{
				keyname
			});
		}

		public static LocalizedString OnlyOneCutoverBatchIsAllowedError
		{
			get
			{
				return new LocalizedString("OnlyOneCutoverBatchIsAllowedError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotUpdateSubscriptionSettingsWithoutBatch
		{
			get
			{
				return new LocalizedString("CouldNotUpdateSubscriptionSettingsWithoutBatch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMigrationSlotCapacityExceeded(Unlimited<int> availableCapacity, int requestedCapacity)
		{
			return new LocalizedString("ErrorMigrationSlotCapacityExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				availableCapacity,
				requestedCapacity
			});
		}

		public static LocalizedString CorruptedMigrationBatchCannotBeStarted
		{
			get
			{
				return new LocalizedString("CorruptedMigrationBatchCannotBeStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorConnectionTimeout(string remoteHost, TimeSpan timeout)
		{
			return new LocalizedString("ErrorConnectionTimeout", "", false, false, Strings.ResourceManager, new object[]
			{
				remoteHost,
				timeout
			});
		}

		public static LocalizedString CouldNotDetermineExchangeRemoteSettings(string serverName)
		{
			return new LocalizedString("CouldNotDetermineExchangeRemoteSettings", "", false, false, Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString FinalizationErrorSummaryMessagePlural(int errorCount)
		{
			return new LocalizedString("FinalizationErrorSummaryMessagePlural", "Ex39E5BE", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString ProvisioningThrottledBack
		{
			get
			{
				return new LocalizedString("ProvisioningThrottledBack", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompletedMigrationJobCannotBeModified
		{
			get
			{
				return new LocalizedString("CompletedMigrationJobCannotBeModified", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptedMigrationBatchCannotBeModified
		{
			get
			{
				return new LocalizedString("CorruptedMigrationBatchCannotBeModified", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelTotalRows
		{
			get
			{
				return new LocalizedString("LabelTotalRows", "Ex808717", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelSynced
		{
			get
			{
				return new LocalizedString("LabelSynced", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotDiscoverNSPISettings
		{
			get
			{
				return new LocalizedString("CouldNotDiscoverNSPISettings", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FeatureCannotBeDisabled(string feature)
		{
			return new LocalizedString("FeatureCannotBeDisabled", "", false, false, Strings.ResourceManager, new object[]
			{
				feature
			});
		}

		public static LocalizedString SyncingMigrationJobCannotBeAppendedTo
		{
			get
			{
				return new LocalizedString("SyncingMigrationJobCannotBeAppendedTo", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationExchangeCredentialFailure
		{
			get
			{
				return new LocalizedString("MigrationExchangeCredentialFailure", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaximumConcurrentMigrationLimitExceeded(string endpointValue, string limitValue, string migrationType)
		{
			return new LocalizedString("ErrorMaximumConcurrentMigrationLimitExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				endpointValue,
				limitValue,
				migrationType
			});
		}

		public static LocalizedString UserDuplicateInOtherBatch(string alias, string batchName)
		{
			return new LocalizedString("UserDuplicateInOtherBatch", "", false, false, Strings.ResourceManager, new object[]
			{
				alias,
				batchName
			});
		}

		public static LocalizedString MultipleMigrationJobItems(string email)
		{
			return new LocalizedString("MultipleMigrationJobItems", "", false, false, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString InvalidVersionDetailedStatus
		{
			get
			{
				return new LocalizedString("InvalidVersionDetailedStatus", "ExBACE1A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobStarted(string userName)
		{
			return new LocalizedString("MigrationReportJobStarted", "", false, false, Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString MigrationReportJobAutoStarted
		{
			get
			{
				return new LocalizedString("MigrationReportJobAutoStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobRemoved(string username)
		{
			return new LocalizedString("MigrationReportJobRemoved", "", false, false, Strings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString RemovingMigrationUserBatchMustBeIdle
		{
			get
			{
				return new LocalizedString("RemovingMigrationUserBatchMustBeIdle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BatchCompletionReportMailHeader(string fileName)
		{
			return new LocalizedString("BatchCompletionReportMailHeader", "ExC678DF", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString MigrationObjectNotFoundInADError(string legDN, string server)
		{
			return new LocalizedString("MigrationObjectNotFoundInADError", "", false, false, Strings.ResourceManager, new object[]
			{
				legDN,
				server
			});
		}

		public static LocalizedString NoDataMigrated
		{
			get
			{
				return new LocalizedString("NoDataMigrated", "Ex0AEE83", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorConnectionFailed(string remoteHost)
		{
			return new LocalizedString("ErrorConnectionFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				remoteHost
			});
		}

		public static LocalizedString UserDuplicateOrphanedFromBatch(string alias)
		{
			return new LocalizedString("UserDuplicateOrphanedFromBatch", "", false, false, Strings.ResourceManager, new object[]
			{
				alias
			});
		}

		public static LocalizedString PartialMigrationSummaryMessageSingular(int partialMigrationCount)
		{
			return new LocalizedString("PartialMigrationSummaryMessageSingular", "Ex3C7464", false, true, Strings.ResourceManager, new object[]
			{
				partialMigrationCount
			});
		}

		public static LocalizedString AutoDiscoverConfigurationError(LocalizedString details)
		{
			return new LocalizedString("AutoDiscoverConfigurationError", "", false, false, Strings.ResourceManager, new object[]
			{
				details
			});
		}

		public static LocalizedString ErrorCouldNotCreateCredentials(string username)
		{
			return new LocalizedString("ErrorCouldNotCreateCredentials", "", false, false, Strings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString ErrorUnknownConnectionSettingsType(string root)
		{
			return new LocalizedString("ErrorUnknownConnectionSettingsType", "", false, false, Strings.ResourceManager, new object[]
			{
				root
			});
		}

		public static LocalizedString SubscriptionDisabledSinceFinalized
		{
			get
			{
				return new LocalizedString("SubscriptionDisabledSinceFinalized", "ExB1EBC1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAuthenticationMethodNotSupported(string authenticationMethod, string protocol, string validValues)
		{
			return new LocalizedString("ErrorAuthenticationMethodNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				authenticationMethod,
				protocol,
				validValues
			});
		}

		public static LocalizedString AuthenticationErrorStatus
		{
			get
			{
				return new LocalizedString("AuthenticationErrorStatus", "Ex2DE54F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientInfoInvalidAtSource(string email)
		{
			return new LocalizedString("RecipientInfoInvalidAtSource", "", false, false, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ErrorMissingExchangeGuid(string identity)
		{
			return new LocalizedString("ErrorMissingExchangeGuid", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ErrorCouldNotEncryptPassword(string username)
		{
			return new LocalizedString("ErrorCouldNotEncryptPassword", "", false, false, Strings.ResourceManager, new object[]
			{
				username
			});
		}

		public static LocalizedString MigrationReportJobTransientError
		{
			get
			{
				return new LocalizedString("MigrationReportJobTransientError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedRecipientTypeForProtocol(string type, string protocol)
		{
			return new LocalizedString("ErrorUnsupportedRecipientTypeForProtocol", "", false, false, Strings.ResourceManager, new object[]
			{
				type,
				protocol
			});
		}

		public static LocalizedString ErrorUnexpectedMigrationType(string discoveredType, string expectedType)
		{
			return new LocalizedString("ErrorUnexpectedMigrationType", "", false, false, Strings.ResourceManager, new object[]
			{
				discoveredType,
				expectedType
			});
		}

		public static LocalizedString MigrationJobAlreadyHasPendingCSV
		{
			get
			{
				return new LocalizedString("MigrationJobAlreadyHasPendingCSV", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorConnectionSettingsNotSerialized
		{
			get
			{
				return new LocalizedString("ErrorConnectionSettingsNotSerialized", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BatchCancelledBySystem(string fileName)
		{
			return new LocalizedString("BatchCancelledBySystem", "Ex2B0B04", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString PasswordPreviouslySet
		{
			get
			{
				return new LocalizedString("PasswordPreviouslySet", "Ex849B35", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobAlreadyQueued
		{
			get
			{
				return new LocalizedString("MigrationJobAlreadyQueued", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PoisonDetailedStatus
		{
			get
			{
				return new LocalizedString("PoisonDetailedStatus", "Ex0BEB90", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoPartialMigrationSummaryMessage
		{
			get
			{
				return new LocalizedString("NoPartialMigrationSummaryMessage", "Ex22EA1D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoppingMigrationJobCannotBeStarted
		{
			get
			{
				return new LocalizedString("StoppingMigrationJobCannotBeStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchCancelledByUser(string batchName)
		{
			return new LocalizedString("MigrationBatchCancelledByUser", "Ex56E109", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString RpcRequestFailed(string requestType, string serverName)
		{
			return new LocalizedString("RpcRequestFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				requestType,
				serverName
			});
		}

		public static LocalizedString ErrorMigrationJobNotFound(Guid identity)
		{
			return new LocalizedString("ErrorMigrationJobNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString FailedToReadPublicFoldersOnPremise
		{
			get
			{
				return new LocalizedString("FailedToReadPublicFoldersOnPremise", "Ex88CD84", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFinalizationReportMailHeader(string batchName)
		{
			return new LocalizedString("MigrationFinalizationReportMailHeader", "Ex6EAFCA", false, true, Strings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString MailboxNotFoundSubscriptionStatus
		{
			get
			{
				return new LocalizedString("MailboxNotFoundSubscriptionStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReadPublicFoldersOnTargetInternalError
		{
			get
			{
				return new LocalizedString("ReadPublicFoldersOnTargetInternalError", "Ex1CE50E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternallySuspendedFailure
		{
			get
			{
				return new LocalizedString("ExternallySuspendedFailure", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStateSizeError
		{
			get
			{
				return new LocalizedString("SyncStateSizeError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportJobCompletedByUser(string userName)
		{
			return new LocalizedString("MigrationReportJobCompletedByUser", "", false, false, Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString MoacWarningMessage(string url)
		{
			return new LocalizedString("MoacWarningMessage", "Ex62298B", false, true, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString LabelLogMailFooter
		{
			get
			{
				return new LocalizedString("LabelLogMailFooter", "Ex32026C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingRootFolderMappingInCSVError(string hierarchyMailboxName)
		{
			return new LocalizedString("MissingRootFolderMappingInCSVError", "", false, false, Strings.ResourceManager, new object[]
			{
				hierarchyMailboxName
			});
		}

		public static LocalizedString CorruptedSubscriptionStatus
		{
			get
			{
				return new LocalizedString("CorruptedSubscriptionStatus", "Ex6F410E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceEmailAddressNotUnique(string smtpAddress)
		{
			return new LocalizedString("SourceEmailAddressNotUnique", "", false, false, Strings.ResourceManager, new object[]
			{
				smtpAddress
			});
		}

		public static LocalizedString SignInMightBeRequired
		{
			get
			{
				return new LocalizedString("SignInMightBeRequired", "Ex29EFE6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTempMissingMigrationMailbox
		{
			get
			{
				return new LocalizedString("MigrationTempMissingMigrationMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobCannotBeCompleted
		{
			get
			{
				return new LocalizedString("MigrationJobCannotBeCompleted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownMigrationError
		{
			get
			{
				return new LocalizedString("UnknownMigrationError", "Ex4B5361", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidValueProvidedForColumn(string columnName, string value)
		{
			return new LocalizedString("InvalidValueProvidedForColumn", "ExE97005", false, true, Strings.ResourceManager, new object[]
			{
				columnName,
				value
			});
		}

		public static LocalizedString SyncTimeOutFailure(string timespan)
		{
			return new LocalizedString("SyncTimeOutFailure", "Ex529E25", false, true, Strings.ResourceManager, new object[]
			{
				timespan
			});
		}

		public static LocalizedString CompleteMigrationBatchNotSupported
		{
			get
			{
				return new LocalizedString("CompleteMigrationBatchNotSupported", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfigAccessRuntimeError
		{
			get
			{
				return new LocalizedString("ConfigAccessRuntimeError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BatchCompletionReportMailErrorHeader(string fileName)
		{
			return new LocalizedString("BatchCompletionReportMailErrorHeader", "ExCFE22C", false, true, Strings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString DisplayNameFormat(string firstname, string lastname)
		{
			return new LocalizedString("DisplayNameFormat", "Ex520F15", false, true, Strings.ResourceManager, new object[]
			{
				firstname,
				lastname
			});
		}

		public static LocalizedString UnsupportedMigrationTypeError(MigrationType type)
		{
			return new LocalizedString("UnsupportedMigrationTypeError", "", false, false, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(103);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Migration.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			RemoteMailboxQuotaWarningStatus = 86638814U,
			CompletingMigrationJobCannotBeAppendedTo = 1530080577U,
			MigrationReportJobInitialSyncComplete = 2718293401U,
			LabelRunTime = 2060675488U,
			CorruptedMigrationBatchCannotBeStopped = 149572871U,
			MigrationExchangeRpcConnectionFailure = 1676389812U,
			MigrationJobCannotBeRemoved = 1899202903U,
			CannotSpecifyUnicodeInCredentials = 3099209002U,
			MigrationCancelledByUserRequest = 1798653784U,
			FailureDuringRemoval = 4872333U,
			FinalizationErrorSummaryRetryMessage = 223688197U,
			MigrationJobAlreadyStopping = 3674744977U,
			MigrationJobCannotBeStopped = 2530769896U,
			SubscriptionNotFound = 1168096436U,
			ContactsMigration = 3851331319U,
			EmailMigration = 4089501710U,
			UserProvisioningInternalError = 3003515263U,
			CompletedMigrationJobCannotBeStartedMultiBatch = 1509794908U,
			LeaveOnServerNotSupportedStatus = 1898862428U,
			MigrationLocalDatabasesNotFound = 2871031082U,
			OnlyOnePublicFolderBatchIsAllowedError = 829120357U,
			LabelFileName = 1144436407U,
			MigrationTempMissingDatabase = 3073036977U,
			MigrationDataCorruptionError = 1781379657U,
			CutoverAndStagedBatchesCannotCoexistError = 1300976335U,
			MaximumNumberOfBatchesReached = 2350676710U,
			PublicFolderMailboxesNotProvisionedError = 4040662282U,
			CannotSpecifyUnicodeUserIdPasswordWithBasicAuth = 2999872780U,
			CompletingMigrationJobCannotBeStarted = 853242704U,
			MigrationJobAlreadyStarted = 514394488U,
			LabelTotalMailboxes = 850869622U,
			MigrationJobAlreadyStopped = 2694765716U,
			MigrationGenericError = 391393381U,
			CommunicationErrorStatus = 3393024142U,
			ErrorReportLink = 3351583406U,
			LabelStartDateTime = 2548932531U,
			MigrationJobDoesNotSupportAppendingUserCSV = 1956114709U,
			SubscriptionRpcThresholdExceeded = 934722052U,
			CorruptedMigrationBatchCannotBeCompleted = 2142750819U,
			RemovedMigrationJobCannotBeStarted = 437021588U,
			ErrorCouldNotDeserializeConnectionSettings = 2530286416U,
			UnknownTimespan = 1754366117U,
			LabsMailboxQuotaWarningStatus = 1419436392U,
			ConnectionErrorStatus = 2571873410U,
			StatisticsReportLink = 1951127083U,
			RemoteServerIsSlow = 571725646U,
			ErrorMigrationMailboxMissingOrInvalid = 672478732U,
			CompletedMigrationJobCannotBeStarted = 1978952169U,
			MigrationReportFailed = 2036241881U,
			MigrationCancelledDueToInternalError = 1271432035U,
			PublicFolderMigrationBatchCannotBeCompletedWithErrors = 1786933629U,
			MigrationJobCannotRetryCompletion = 85986654U,
			MissingRequiredSubscriptionId = 3662584967U,
			IMAPPathPrefixInvalidStatus = 1905833821U,
			TooManyFoldersStatus = 601262870U,
			LabelSubmittedByUser = 1066087601U,
			CSVFileTooLarge = 1842619565U,
			RemovedMigrationJobCannotBeModified = 1189164420U,
			LabelCompleted = 4257873367U,
			MigrationPublicFolderWireUpFailed = 1047974326U,
			LabelCouldntMigrate = 1386196812U,
			MigrationTenantPermissionFailure = 3955323767U,
			UnknownMigrationBatchError = 1043780694U,
			MigrationReportJobComplete = 1332982264U,
			UnableToDisableSubscription = 367906109U,
			OnlyOneCutoverBatchIsAllowedError = 1279919936U,
			CouldNotUpdateSubscriptionSettingsWithoutBatch = 2536192239U,
			CorruptedMigrationBatchCannotBeStarted = 3327693399U,
			ProvisioningThrottledBack = 617690654U,
			CompletedMigrationJobCannotBeModified = 4177113737U,
			CorruptedMigrationBatchCannotBeModified = 3751987975U,
			LabelTotalRows = 5503809U,
			LabelSynced = 1136304310U,
			CouldNotDiscoverNSPISettings = 1088630310U,
			SyncingMigrationJobCannotBeAppendedTo = 16533286U,
			MigrationExchangeCredentialFailure = 4038100652U,
			InvalidVersionDetailedStatus = 1756157035U,
			MigrationReportJobAutoStarted = 2104158819U,
			RemovingMigrationUserBatchMustBeIdle = 320046086U,
			NoDataMigrated = 759640474U,
			SubscriptionDisabledSinceFinalized = 2605498839U,
			AuthenticationErrorStatus = 2047379006U,
			MigrationReportJobTransientError = 986156097U,
			MigrationJobAlreadyHasPendingCSV = 1214466260U,
			ErrorConnectionSettingsNotSerialized = 3504597832U,
			PasswordPreviouslySet = 1236042521U,
			MigrationJobAlreadyQueued = 3020735710U,
			PoisonDetailedStatus = 2529513648U,
			NoPartialMigrationSummaryMessage = 1500792389U,
			StoppingMigrationJobCannotBeStarted = 3968870632U,
			FailedToReadPublicFoldersOnPremise = 1142848800U,
			MailboxNotFoundSubscriptionStatus = 1731044986U,
			ReadPublicFoldersOnTargetInternalError = 1197149433U,
			ExternallySuspendedFailure = 3334133771U,
			SyncStateSizeError = 4060495101U,
			LabelLogMailFooter = 1246705638U,
			CorruptedSubscriptionStatus = 4122602291U,
			SignInMightBeRequired = 295927301U,
			MigrationTempMissingMigrationMailbox = 4162391104U,
			MigrationJobCannotBeCompleted = 501292080U,
			UnknownMigrationError = 2595351182U,
			CompleteMigrationBatchNotSupported = 1980396424U,
			ConfigAccessRuntimeError = 2353197456U
		}

		private enum ParamIDs
		{
			MigrationBatchCompletionReportMailErrorHeader,
			MigrationReportJobItemCorrupted,
			UnsupportedSourceRecipientTypeError,
			MigrationJobCannotBeDeletedWithPendingItems,
			UnsupportedAdminCulture,
			MigrationProcessorInvalidation,
			MigrationJobItemNotFound,
			MigrationFeatureNotSupported,
			MigrationJobNotFound,
			ErrorNspiNotSupportedForEndpointType,
			UserDuplicateInCSV,
			MigrationOrganizationNotFound,
			MigrationReportRepaired,
			LabelAutoRetry,
			CouldNotLoadMigrationPersistedItem,
			MigrationReportJobItemRetried,
			MigrationItemLastUpdatedInTheFuture,
			FailedToUpdateRecipientSource,
			MigrationReportJobCreated,
			MigrationSubscriptionCreationFailed,
			MigrationReportNspiFailed,
			UnexpectedSubscriptionStatus,
			MigrationRpcFailed,
			MigrationBatchCompletionReportMailHeader,
			MigrationReportNspiRfrFailed,
			MigrationVersionMismatch,
			MigrationBatchCancelledBySystem,
			BatchCancelledByUser,
			InvalidRootFolderMappingInCSVError,
			DuplicateFolderInCSVError,
			ErrorTooManyTransientFailures,
			RunTimeFormatHours,
			MigrationReportJobItemRemovedInternal,
			RecipientDoesNotExistAtSource,
			MigrationReportJobItemCreatedInternal,
			ErrorSummaryMessageSingular,
			MigrationReportJobItemWithError,
			ErrorUserAlreadyBeingMigrated,
			MigrationBatchReportMailHeader,
			DetailedAggregationStatus,
			UnsupportedTargetRecipientTypeError,
			ErrorUserMissingOrWithoutRBAC,
			PartialMigrationSummaryMessagePlural,
			MigrationReportJobItemRemoved,
			CannotUpgradeMigrationVersion,
			AutoDiscoverInternalError,
			MigrationUserAlreadyExistsInDifferentType,
			MigrationReportJobModifiedByUser,
			ErrorInvalidRecipientType,
			MigrationUserAlreadyRemoved,
			FinalizationErrorSummaryMessageSingular,
			ErrorParsingCSV,
			MigrationConfigString,
			ErrorSummaryMessagePlural,
			ValueNotProvidedForColumn,
			UserAlreadyMigratedWithAlternateEmail,
			MigrationFinalizationReportMailErrorHeader,
			ErrorAutoDiscoveryNotSupported,
			RunTimeFormatMinutes,
			RunTimeFormatDays,
			ErrorMissingExpectedCapability,
			MigrationReportSetString,
			MigrationEndpointAlreadyExistsError,
			MigrationBatchReportMailErrorHeader,
			CouldNotAddExchangeSnapIn,
			ErrorNoEndpointSupportForMigrationType,
			MigrationRecipientNotFound,
			ErrorCannotRemoveUserWithoutBatch,
			ErrorCannotRemoveEndpointWithAssociatedBatches,
			UserAlreadyMigrated,
			MigrationReportJobItemFailed,
			MigrationUserMovedToAnotherBatch,
			MigrationReportJobAutomaticallyRestarting,
			ConfigKeyAccessRuntimeError,
			ErrorMigrationSlotCapacityExceeded,
			ErrorConnectionTimeout,
			CouldNotDetermineExchangeRemoteSettings,
			FinalizationErrorSummaryMessagePlural,
			FeatureCannotBeDisabled,
			ErrorMaximumConcurrentMigrationLimitExceeded,
			UserDuplicateInOtherBatch,
			MultipleMigrationJobItems,
			MigrationReportJobStarted,
			MigrationReportJobRemoved,
			BatchCompletionReportMailHeader,
			MigrationObjectNotFoundInADError,
			ErrorConnectionFailed,
			UserDuplicateOrphanedFromBatch,
			PartialMigrationSummaryMessageSingular,
			AutoDiscoverConfigurationError,
			ErrorCouldNotCreateCredentials,
			ErrorUnknownConnectionSettingsType,
			ErrorAuthenticationMethodNotSupported,
			RecipientInfoInvalidAtSource,
			ErrorMissingExchangeGuid,
			ErrorCouldNotEncryptPassword,
			ErrorUnsupportedRecipientTypeForProtocol,
			ErrorUnexpectedMigrationType,
			BatchCancelledBySystem,
			MigrationBatchCancelledByUser,
			RpcRequestFailed,
			ErrorMigrationJobNotFound,
			MigrationFinalizationReportMailHeader,
			MigrationReportJobCompletedByUser,
			MoacWarningMessage,
			MissingRootFolderMappingInCSVError,
			SourceEmailAddressNotUnique,
			InvalidValueProvidedForColumn,
			SyncTimeOutFailure,
			BatchCompletionReportMailErrorHeader,
			DisplayNameFormat,
			UnsupportedMigrationTypeError
		}
	}
}
