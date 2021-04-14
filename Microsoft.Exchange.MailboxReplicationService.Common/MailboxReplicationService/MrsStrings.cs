using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MrsStrings
	{
		static MrsStrings()
		{
			MrsStrings.stringIDs.Add(1424238063U, "InvalidRequestJob");
			MrsStrings.stringIDs.Add(392888166U, "WorkloadTypeOnboarding");
			MrsStrings.stringIDs.Add(3088101088U, "ReportMessagesCopied");
			MrsStrings.stringIDs.Add(3176193428U, "DestMailboxAlreadyBeingMoved");
			MrsStrings.stringIDs.Add(3671912657U, "ReportDestinationSDCannotBeRead");
			MrsStrings.stringIDs.Add(3429098023U, "ServiceIsStopping");
			MrsStrings.stringIDs.Add(1833949493U, "PSTPathMustBeAFile");
			MrsStrings.stringIDs.Add(3825549875U, "ReportMovedMailboxAlreadyMorphedToMailUser");
			MrsStrings.stringIDs.Add(3369317625U, "UnableToReadAD");
			MrsStrings.stringIDs.Add(1476056932U, "MoveRestartDueToContainerMailboxesChanged");
			MrsStrings.stringIDs.Add(3728110147U, "ReportCopyPerUserReadUnreadDataStarted");
			MrsStrings.stringIDs.Add(1704413971U, "JobCannotBeRehomedWhenInProgress");
			MrsStrings.stringIDs.Add(2750851982U, "FolderHierarchyIsInconsistent");
			MrsStrings.stringIDs.Add(3945110826U, "ReportMoveRestartedDueToSignatureChange");
			MrsStrings.stringIDs.Add(3089757325U, "ErrorCannotPreventCompletionForCompletingMove");
			MrsStrings.stringIDs.Add(4185133072U, "WorkloadTypeOffboarding");
			MrsStrings.stringIDs.Add(979363606U, "MRSAlreadyConfigured");
			MrsStrings.stringIDs.Add(135363966U, "ReportTargetPublicFolderDeploymentUnlocked");
			MrsStrings.stringIDs.Add(815599557U, "ReportRequestCancelPostponed");
			MrsStrings.stringIDs.Add(1388232024U, "JobHasBeenRelinquishedDueToHAStall");
			MrsStrings.stringIDs.Add(738613247U, "RequestPriorityLow");
			MrsStrings.stringIDs.Add(1580421706U, "MoveRequestDirectionPull");
			MrsStrings.stringIDs.Add(119144040U, "UnableToApplyFolderHierarchyChanges");
			MrsStrings.stringIDs.Add(3837251784U, "PickupStatusDisabled");
			MrsStrings.stringIDs.Add(2288253100U, "RemoteResource");
			MrsStrings.stringIDs.Add(2018114904U, "MoveRestartedDueToSignatureChange");
			MrsStrings.stringIDs.Add(3406658852U, "FolderHierarchyContainsNoRoots");
			MrsStrings.stringIDs.Add(2360217149U, "JobHasBeenRelinquishedDueToCIStall");
			MrsStrings.stringIDs.Add(2713483251U, "ContentIndexing");
			MrsStrings.stringIDs.Add(3235245142U, "TooManyLargeItems");
			MrsStrings.stringIDs.Add(2888971502U, "CouldNotConnectToTargetMailbox");
			MrsStrings.stringIDs.Add(1886860910U, "PSTIOException");
			MrsStrings.stringIDs.Add(2240969000U, "RequestPriorityNormal");
			MrsStrings.stringIDs.Add(268267125U, "SmtpServerInfoMissing");
			MrsStrings.stringIDs.Add(4282140830U, "NoPublicFolderMailboxFoundInSource");
			MrsStrings.stringIDs.Add(3551669574U, "WorkloadTypeRemotePstExport");
			MrsStrings.stringIDs.Add(3345143506U, "FastTransferArgumentError");
			MrsStrings.stringIDs.Add(3435030616U, "PickupStatusCompletedJob");
			MrsStrings.stringIDs.Add(856796896U, "ReportJobProcessingDisabled");
			MrsStrings.stringIDs.Add(1247936583U, "ImproperTypeForThisIdParameter");
			MrsStrings.stringIDs.Add(2006276195U, "MoveRequestMissingInfoDelete");
			MrsStrings.stringIDs.Add(3798869933U, "ReportRelinquishingJobDueToServiceStop");
			MrsStrings.stringIDs.Add(1827127136U, "PickupStatusCorruptJob");
			MrsStrings.stringIDs.Add(4161853231U, "RequestHasBeenRelinquishedDueToBadHealthOfBackendServers");
			MrsStrings.stringIDs.Add(2260777903U, "MoveRequestMissingInfoSave");
			MrsStrings.stringIDs.Add(3708951374U, "RestartingMove");
			MrsStrings.stringIDs.Add(3631041786U, "ErrorWhileUpdatingMovedMailbox");
			MrsStrings.stringIDs.Add(1320326778U, "MoveRequestValidationFailed");
			MrsStrings.stringIDs.Add(199165212U, "MustProvideValidSessionForFindingRequests");
			MrsStrings.stringIDs.Add(4003924173U, "TooManyMissingItems");
			MrsStrings.stringIDs.Add(4230582602U, "UpdateFolderFailed");
			MrsStrings.stringIDs.Add(990473213U, "OfflinePublicFolderMigrationNotSupported");
			MrsStrings.stringIDs.Add(2906979272U, "TaskCanceled");
			MrsStrings.stringIDs.Add(2323682581U, "SourceMailboxAlreadyBeingMoved");
			MrsStrings.stringIDs.Add(938866594U, "MoveJobDeserializationFailed");
			MrsStrings.stringIDs.Add(3959740005U, "MoveRequestNotFoundInQueue");
			MrsStrings.stringIDs.Add(2697508630U, "JobHasBeenCanceled");
			MrsStrings.stringIDs.Add(1085885350U, "ReportRequestStarted");
			MrsStrings.stringIDs.Add(90093709U, "ErrorDownlevelClientsNotSupported");
			MrsStrings.stringIDs.Add(2295784267U, "DataExportTimeout");
			MrsStrings.stringIDs.Add(2935411904U, "TargetMailboxConnectionWasLost");
			MrsStrings.stringIDs.Add(2247143358U, "JobHasBeenRelinquishedDueToDatabaseFailover");
			MrsStrings.stringIDs.Add(3942653727U, "PublicFolderMailboxesNotProvisionedForMigration");
			MrsStrings.stringIDs.Add(3540791304U, "RequestPriorityHigher");
			MrsStrings.stringIDs.Add(1147283780U, "JobHasBeenRelinquishedDueToHAOrCIStalls");
			MrsStrings.stringIDs.Add(1573145718U, "ReportRequestCanceled");
			MrsStrings.stringIDs.Add(4082292636U, "InvalidProxyOperationOrder");
			MrsStrings.stringIDs.Add(2286820319U, "ReportRequestOfflineMovePostponed");
			MrsStrings.stringIDs.Add(3759213604U, "MailboxIsBeingMoved");
			MrsStrings.stringIDs.Add(3604568438U, "NoSuchRequestInSpecifiedIndex");
			MrsStrings.stringIDs.Add(859003787U, "InitializedWithInvalidObjectId");
			MrsStrings.stringIDs.Add(215436927U, "ReportCopyPerUserReadUnreadDataCompleted");
			MrsStrings.stringIDs.Add(699419898U, "ReportSessionStatisticsUpdated");
			MrsStrings.stringIDs.Add(2708825588U, "ReportRelinquishingJobDueToServerThrottling");
			MrsStrings.stringIDs.Add(3162969353U, "MRSNotConfigured");
			MrsStrings.stringIDs.Add(2244475911U, "MailboxRootFolderNotFound");
			MrsStrings.stringIDs.Add(540485114U, "WorkloadTypeLoadBalancing");
			MrsStrings.stringIDs.Add(1035843159U, "JobIsQuarantined");
			MrsStrings.stringIDs.Add(2538826952U, "ReportSourceSDCannotBeRead");
			MrsStrings.stringIDs.Add(2850410499U, "ReportMoveRequestIsNoLongerSticky");
			MrsStrings.stringIDs.Add(1606949857U, "ClusterNotFound");
			MrsStrings.stringIDs.Add(833630790U, "MoveRestartDueToIsIntegCheck");
			MrsStrings.stringIDs.Add(952727642U, "ReportJobIsStillStalled");
			MrsStrings.stringIDs.Add(4023361926U, "WorkloadTypeRemotePstIngestion");
			MrsStrings.stringIDs.Add(472899259U, "ReportPrimaryMservEntryPointsToExo");
			MrsStrings.stringIDs.Add(2750716986U, "ValidationADUserIsNotBeingMoved");
			MrsStrings.stringIDs.Add(702077469U, "PostMoveStateIsUncertain");
			MrsStrings.stringIDs.Add(901032809U, "RequestPriorityHigh");
			MrsStrings.stringIDs.Add(888446530U, "SourceContainer");
			MrsStrings.stringIDs.Add(3595573419U, "WorkloadTypeTenantUpgrade");
			MrsStrings.stringIDs.Add(3655041298U, "EasMissingMessageCategory");
			MrsStrings.stringIDs.Add(336070114U, "JobHasBeenRelinquished");
			MrsStrings.stringIDs.Add(2930966715U, "RecoverySyncNotImplemented");
			MrsStrings.stringIDs.Add(1123448769U, "ErrorTooManyCleanupRetries");
			MrsStrings.stringIDs.Add(3238364772U, "ReportFinalSyncStarted");
			MrsStrings.stringIDs.Add(1078342972U, "ReportJobExitedStalledByThrottlingState");
			MrsStrings.stringIDs.Add(1302133406U, "MustProvideNonEmptyStringForIdentity");
			MrsStrings.stringIDs.Add(2181380613U, "ReportRelinquishingJobDueToNeedForRehome");
			MrsStrings.stringIDs.Add(2761303157U, "NotEnoughInformationSupplied");
			MrsStrings.stringIDs.Add(4239850164U, "NoDataContext");
			MrsStrings.stringIDs.Add(2023474786U, "ReportMoveCompleted");
			MrsStrings.stringIDs.Add(4145210966U, "UnableToDeleteMoveRequestMessage");
			MrsStrings.stringIDs.Add(4239846426U, "DestinationFolderHierarchyInconsistent");
			MrsStrings.stringIDs.Add(4095394241U, "NotEnoughInformationToFindMoveRequest");
			MrsStrings.stringIDs.Add(949547255U, "TaskSchedulerStopped");
			MrsStrings.stringIDs.Add(1292011054U, "ReportRelinquishingJobDueToCIStall");
			MrsStrings.stringIDs.Add(45208631U, "WriteCpu");
			MrsStrings.stringIDs.Add(73509179U, "ReportHomeMdbPointsToTarget");
			MrsStrings.stringIDs.Add(672463457U, "CorruptRestrictionData");
			MrsStrings.stringIDs.Add(1194778775U, "ReportIncrementalMoveRestartDueToGlobalCounterRangeDepletion");
			MrsStrings.stringIDs.Add(3109918443U, "ReadRpc");
			MrsStrings.stringIDs.Add(1212084898U, "WorkloadTypeLocal");
			MrsStrings.stringIDs.Add(3906020551U, "MoveRequestDirectionPush");
			MrsStrings.stringIDs.Add(3213196515U, "SourceFolderHierarchyInconsistent");
			MrsStrings.stringIDs.Add(2170397003U, "RequestPriorityHighest");
			MrsStrings.stringIDs.Add(1787602764U, "ErrorFinalizationIsBlocked");
			MrsStrings.stringIDs.Add(1410610418U, "MoveRequestTypeCrossOrg");
			MrsStrings.stringIDs.Add(1554654695U, "MrsProxyServiceIsDisabled");
			MrsStrings.stringIDs.Add(2497488330U, "ReportMoveCanceled");
			MrsStrings.stringIDs.Add(1153630962U, "ErrorCannotPreventCompletionForOfflineMove");
			MrsStrings.stringIDs.Add(3994387752U, "RequestHasBeenPostponedDueToBadHealthOfBackendServers2");
			MrsStrings.stringIDs.Add(1608330969U, "ValidationNoCorrespondingIndexEntries");
			MrsStrings.stringIDs.Add(1728063749U, "InvalidSyncStateData");
			MrsStrings.stringIDs.Add(874970588U, "ReportMoveRestartedDueToSourceCorruption");
			MrsStrings.stringIDs.Add(3603681089U, "JobHasBeenAutoSuspended");
			MrsStrings.stringIDs.Add(3843865613U, "InputDataIsInvalid");
			MrsStrings.stringIDs.Add(3287770946U, "ReportJobExitedStalledState");
			MrsStrings.stringIDs.Add(1094055921U, "ActionNotSupported");
			MrsStrings.stringIDs.Add(1690707778U, "ReportTargetAuxFolderContentMailboxGuidUpdated");
			MrsStrings.stringIDs.Add(1049832713U, "ReportStoreMailboxHasFinalized");
			MrsStrings.stringIDs.Add(1157208427U, "ErrorReservationExpired");
			MrsStrings.stringIDs.Add(3856642209U, "ErrorImplicitSplit");
			MrsStrings.stringIDs.Add(1609645991U, "ReportRelinquishingJob");
			MrsStrings.stringIDs.Add(2756637510U, "CouldNotConnectToSourceMailbox");
			MrsStrings.stringIDs.Add(2671521794U, "NoFoldersIncluded");
			MrsStrings.stringIDs.Add(780406631U, "ReportSuspendingJob");
			MrsStrings.stringIDs.Add(1228469352U, "WriteRpc");
			MrsStrings.stringIDs.Add(2309811282U, "NotConnected");
			MrsStrings.stringIDs.Add(4081437335U, "MdbReplication");
			MrsStrings.stringIDs.Add(1499253095U, "ReportRulesWillNotBeCopied");
			MrsStrings.stringIDs.Add(772970447U, "ReportSkippingUpdateSourceMailbox");
			MrsStrings.stringIDs.Add(2051604558U, "ErrorEmptyMailboxGuid");
			MrsStrings.stringIDs.Add(4227707103U, "ReportArchiveAlreadyUpdated");
			MrsStrings.stringIDs.Add(3167108423U, "JobHasBeenSynced");
			MrsStrings.stringIDs.Add(300694364U, "JobHasBeenRelinquishedDueToLongRun");
			MrsStrings.stringIDs.Add(2739554404U, "ReportRelinquishingJobDueToHAOrCIStalling");
			MrsStrings.stringIDs.Add(3709264734U, "Mailbox");
			MrsStrings.stringIDs.Add(2181899002U, "FolderHierarchyIsInconsistentTemporarily");
			MrsStrings.stringIDs.Add(1729678064U, "RequestPriorityEmergency");
			MrsStrings.stringIDs.Add(3018124355U, "ReportRelinquishingJobDueToHAStall");
			MrsStrings.stringIDs.Add(2466192281U, "CorruptSyncState");
			MrsStrings.stringIDs.Add(1574793031U, "ReportTargetPublicFolderContentMailboxGuidUpdated");
			MrsStrings.stringIDs.Add(551973766U, "NoMRSAvailable");
			MrsStrings.stringIDs.Add(3329536416U, "RequestPriorityLower");
			MrsStrings.stringIDs.Add(2648214014U, "ReportAutoSuspendingJob");
			MrsStrings.stringIDs.Add(123753966U, "ReportCalendarFolderFaiSaveFailed");
			MrsStrings.stringIDs.Add(2000650826U, "MoveIsPreventedFromFinalization");
			MrsStrings.stringIDs.Add(615074259U, "ReportMoveAlreadyFinished");
			MrsStrings.stringIDs.Add(3151307821U, "RehomeRequestFailure");
			MrsStrings.stringIDs.Add(2161771148U, "RequestIsStalledByHigherPriorityJobs");
			MrsStrings.stringIDs.Add(951459652U, "WorkloadTypeEmergency");
			MrsStrings.stringIDs.Add(1237434822U, "ReportCalendarFolderSaveFailed");
			MrsStrings.stringIDs.Add(2993893239U, "MRSProxyConnectionNotThrottledError");
			MrsStrings.stringIDs.Add(3133742332U, "ReportWaitingForMailboxDataReplication");
			MrsStrings.stringIDs.Add(1628850222U, "ReportDatabaseFailedOver");
			MrsStrings.stringIDs.Add(1268966663U, "FolderIsMissingInMerge");
			MrsStrings.stringIDs.Add(2846264340U, "Unknown");
			MrsStrings.stringIDs.Add(681951690U, "WorkloadTypeSyncAggregation");
			MrsStrings.stringIDs.Add(1347847794U, "ReportTargetUserIsNotMailEnabledUser");
			MrsStrings.stringIDs.Add(676706684U, "ReportRequestIsNoLongerSticky");
			MrsStrings.stringIDs.Add(3237229570U, "MoveRequestTypeIntraOrg");
			MrsStrings.stringIDs.Add(3107299645U, "ValidationMoveRequestNotDeserialized");
			MrsStrings.stringIDs.Add(2605163142U, "ReportMoveStarted");
			MrsStrings.stringIDs.Add(1655277830U, "ReportPostMoveCleanupStarted");
			MrsStrings.stringIDs.Add(704248557U, "InternalAccessFolderCreationIsNotSupported");
			MrsStrings.stringIDs.Add(1464484042U, "ReportRequestCompleted");
			MrsStrings.stringIDs.Add(3109918916U, "ReadCpu");
			MrsStrings.stringIDs.Add(202150486U, "TargetContainer");
			MrsStrings.stringIDs.Add(4026185433U, "RequestPriorityLowest");
			MrsStrings.stringIDs.Add(630728405U, "WorkloadTypeNone");
			MrsStrings.stringIDs.Add(4156705784U, "UnableToObtainServersInLocalSite");
			MrsStrings.stringIDs.Add(3403189953U, "WrongUserObjectFound");
			MrsStrings.stringIDs.Add(3829274068U, "GetIdsFromNamesCalledOnDestination");
			MrsStrings.stringIDs.Add(1551771611U, "DataExportCanceled");
			MrsStrings.stringIDs.Add(3775277574U, "TooManyBadItems");
			MrsStrings.stringIDs.Add(1220106367U, "ReportVerifyingMailboxContents");
		}

		public static LocalizedString EasFolderSyncFailed(string errorMessage)
		{
			return new LocalizedString("EasFolderSyncFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString InvalidRequestJob
		{
			get
			{
				return new LocalizedString("InvalidRequestJob", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToOpenMailbox(string serverName)
		{
			return new LocalizedString("UnableToOpenMailbox", "Ex72E5B4", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString WorkloadTypeOnboarding
		{
			get
			{
				return new LocalizedString("WorkloadTypeOnboarding", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMessagesCopied
		{
			get
			{
				return new LocalizedString("ReportMessagesCopied", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("ServerError", "Ex25D79D", false, true, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString DestMailboxAlreadyBeingMoved
		{
			get
			{
				return new LocalizedString("DestMailboxAlreadyBeingMoved", "ExF7DBE2", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSyncStateNull(Guid requestGuid)
		{
			return new LocalizedString("ReportSyncStateNull", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid
			});
		}

		public static LocalizedString RecipientArchiveGuidMismatch(string recipient, Guid recipientArchiveGuid, Guid targetArchiveGuid)
		{
			return new LocalizedString("RecipientArchiveGuidMismatch", "Ex71CCCD", false, true, MrsStrings.ResourceManager, new object[]
			{
				recipient,
				recipientArchiveGuid,
				targetArchiveGuid
			});
		}

		public static LocalizedString ReportDestinationMailboxResetSucceeded(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportDestinationMailboxResetSucceeded", "ExF5A6D0", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ReportMovedMailboxUpdated(string domainController)
		{
			return new LocalizedString("ReportMovedMailboxUpdated", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				domainController
			});
		}

		public static LocalizedString ReportDestinationSDCannotBeRead
		{
			get
			{
				return new LocalizedString("ReportDestinationSDCannotBeRead", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SystemMailboxNotFound(string systemMailboxName)
		{
			return new LocalizedString("SystemMailboxNotFound", "Ex501017", false, true, MrsStrings.ResourceManager, new object[]
			{
				systemMailboxName
			});
		}

		public static LocalizedString ServiceIsStopping
		{
			get
			{
				return new LocalizedString("ServiceIsStopping", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRemovingTargetMailboxDueToOfflineMoveFailure(LocalizedString mbxId)
		{
			return new LocalizedString("ReportRemovingTargetMailboxDueToOfflineMoveFailure", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxId
			});
		}

		public static LocalizedString TimeoutError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("TimeoutError", "ExAA9BE9", false, true, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString ReportMailboxBeforeFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportMailboxBeforeFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString PSTPathMustBeAFile
		{
			get
			{
				return new LocalizedString("PSTPathMustBeAFile", "Ex22ECEF", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveHasBeenSynced(Guid mbxGuid, DateTime nextIncremental)
		{
			return new LocalizedString("MoveHasBeenSynced", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid,
				nextIncremental
			});
		}

		public static LocalizedString MoveCancelFailedForAlreadyCompletedMove(Guid mbxGuid)
		{
			return new LocalizedString("MoveCancelFailedForAlreadyCompletedMove", "ExBA867E", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString RPCHTTPPublicFoldersId(string legDN)
		{
			return new LocalizedString("RPCHTTPPublicFoldersId", "ExC426EA", false, true, MrsStrings.ResourceManager, new object[]
			{
				legDN
			});
		}

		public static LocalizedString ReportThrottles(string mdbThrottle, string cpuThrottle, string mdbReplicationThrottle, string contentIndexingThrottle, string unknownThrottle)
		{
			return new LocalizedString("ReportThrottles", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mdbThrottle,
				cpuThrottle,
				mdbReplicationThrottle,
				contentIndexingThrottle,
				unknownThrottle
			});
		}

		public static LocalizedString ReportMailboxInfoAfterMove(string mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxInfoAfterMove", "ExDC727C", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ReportMovedMailboxAlreadyMorphedToMailUser
		{
			get
			{
				return new LocalizedString("ReportMovedMailboxAlreadyMorphedToMailUser", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToReadAD
		{
			get
			{
				return new LocalizedString("UnableToReadAD", "Ex994787", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRestartDueToContainerMailboxesChanged
		{
			get
			{
				return new LocalizedString("MoveRestartDueToContainerMailboxesChanged", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCopyPerUserReadUnreadDataStarted
		{
			get
			{
				return new LocalizedString("ReportCopyPerUserReadUnreadDataStarted", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSourceMailboxBeforeFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportSourceMailboxBeforeFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString JobCannotBeRehomedWhenInProgress
		{
			get
			{
				return new LocalizedString("JobCannotBeRehomedWhenInProgress", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RulesDataContext(string rulesStr)
		{
			return new LocalizedString("RulesDataContext", "Ex62EFB6", false, true, MrsStrings.ResourceManager, new object[]
			{
				rulesStr
			});
		}

		public static LocalizedString UnableToGetPSTProps(string filePath)
		{
			return new LocalizedString("UnableToGetPSTProps", "ExF6CDBC", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString ReportFailedToDisconnectFromSource2(string errorType)
		{
			return new LocalizedString("ReportFailedToDisconnectFromSource2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString ReportJobHasBeenRelinquishedDueToServerBusy(DateTime pickupTime)
		{
			return new LocalizedString("ReportJobHasBeenRelinquishedDueToServerBusy", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString ReportDestinationMailboxClearSyncStateFailed(string errorType, LocalizedString errorMsg, string trace, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxClearSyncStateFailed", "Ex088797", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString MoveCompleteFailedForAlreadyFailedMove(Guid mbxGuid)
		{
			return new LocalizedString("MoveCompleteFailedForAlreadyFailedMove", "Ex2C160B", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString DatabaseCouldNotBeMapped(string databaseName)
		{
			return new LocalizedString("DatabaseCouldNotBeMapped", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ReportCopyFolderPropertyProgress(ulong folderCount)
		{
			return new LocalizedString("ReportCopyFolderPropertyProgress", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderCount
			});
		}

		public static LocalizedString ReportUnableToLoadDestinationUser(string errorType)
		{
			return new LocalizedString("ReportUnableToLoadDestinationUser", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString NotImplemented(string methodName)
		{
			return new LocalizedString("NotImplemented", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				methodName
			});
		}

		public static LocalizedString ReportFolderCreationProgress(int foldersCreated, LocalizedString physicalMailboxId)
		{
			return new LocalizedString("ReportFolderCreationProgress", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				foldersCreated,
				physicalMailboxId
			});
		}

		public static LocalizedString PropValuesDataContext(string propValuesStr)
		{
			return new LocalizedString("PropValuesDataContext", "Ex467613", false, true, MrsStrings.ResourceManager, new object[]
			{
				propValuesStr
			});
		}

		public static LocalizedString ReportMailboxInfoBeforeMoveLoc(LocalizedString mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxInfoBeforeMoveLoc", "ExE09E72", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ReportMoveRequestCreated(string userName)
		{
			return new LocalizedString("ReportMoveRequestCreated", "Ex886978", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString PickupStatusRequestTypeNotSupported(string requestType)
		{
			return new LocalizedString("PickupStatusRequestTypeNotSupported", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestType
			});
		}

		public static LocalizedString ReportRequestSaveFailed(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportRequestSaveFailed", "ExFF7A34", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString FolderHierarchyIsInconsistent
		{
			get
			{
				return new LocalizedString("FolderHierarchyIsInconsistent", "Ex9CA71E", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestDataMissing(Guid mailboxGuid, Guid mdbGuid)
		{
			return new LocalizedString("MoveRequestDataMissing", "ExCB964E", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				mdbGuid
			});
		}

		public static LocalizedString FolderDataContextSearch(string folderName, string entryId, string parentId)
		{
			return new LocalizedString("FolderDataContextSearch", "ExF7F5AE", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderName,
				entryId,
				parentId
			});
		}

		public static LocalizedString ReportMoveRestartedDueToSignatureChange
		{
			get
			{
				return new LocalizedString("ReportMoveRestartedDueToSignatureChange", "Ex81DF08", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotPreventCompletionForCompletingMove
		{
			get
			{
				return new LocalizedString("ErrorCannotPreventCompletionForCompletingMove", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportIncrementalSyncContentChangesPaged2(LocalizedString physicalMailboxId, int batch, int newMessages, int changedMessages, int deletedMessages, int readMessages, int unreadMessages, int total)
		{
			return new LocalizedString("ReportIncrementalSyncContentChangesPaged2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				batch,
				newMessages,
				changedMessages,
				deletedMessages,
				readMessages,
				unreadMessages,
				total
			});
		}

		public static LocalizedString WorkloadTypeOffboarding
		{
			get
			{
				return new LocalizedString("WorkloadTypeOffboarding", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationValueIsMissing(string valueName)
		{
			return new LocalizedString("ValidationValueIsMissing", "ExD12111", false, true, MrsStrings.ResourceManager, new object[]
			{
				valueName
			});
		}

		public static LocalizedString MRSAlreadyConfigured
		{
			get
			{
				return new LocalizedString("MRSAlreadyConfigured", "Ex4C33F5", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportLargeAmountOfDataLossAccepted2(string badItemLimit, string largeItemLimit, string requestorName)
		{
			return new LocalizedString("ReportLargeAmountOfDataLossAccepted2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				badItemLimit,
				largeItemLimit,
				requestorName
			});
		}

		public static LocalizedString ReportLargeItemEncountered(LocalizedString largeItemStr)
		{
			return new LocalizedString("ReportLargeItemEncountered", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				largeItemStr
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToCancelPostponed(DateTime removeAfter)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToCancelPostponed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				removeAfter
			});
		}

		public static LocalizedString ReportMoveRequestResumed(string userName)
		{
			return new LocalizedString("ReportMoveRequestResumed", "ExF4C194", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString ReportRequestIsInvalid(LocalizedString validationMessage)
		{
			return new LocalizedString("ReportRequestIsInvalid", "Ex503A42", false, true, MrsStrings.ResourceManager, new object[]
			{
				validationMessage
			});
		}

		public static LocalizedString PublicFoldersId(string orgID)
		{
			return new LocalizedString("PublicFoldersId", "Ex2A0D2E", false, true, MrsStrings.ResourceManager, new object[]
			{
				orgID
			});
		}

		public static LocalizedString ItemCountsAndSizes(ulong regularCount, string regularSize, ulong delCount, string delSize, ulong faiCount, string faiSize, ulong faiDelCount, string faiDelSize)
		{
			return new LocalizedString("ItemCountsAndSizes", "Ex1398F0", false, true, MrsStrings.ResourceManager, new object[]
			{
				regularCount,
				regularSize,
				delCount,
				delSize,
				faiCount,
				faiSize,
				faiDelCount,
				faiDelSize
			});
		}

		public static LocalizedString ReportTargetPublicFolderDeploymentUnlocked
		{
			get
			{
				return new LocalizedString("ReportTargetPublicFolderDeploymentUnlocked", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationTargetArchiveMDBMismatch(string adDatabase, string mrDatabase)
		{
			return new LocalizedString("ValidationTargetArchiveMDBMismatch", "Ex8C23C1", false, true, MrsStrings.ResourceManager, new object[]
			{
				adDatabase,
				mrDatabase
			});
		}

		public static LocalizedString DestinationMailboxSeedMBICacheFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("DestinationMailboxSeedMBICacheFailed", "ExA75EC7", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReportRequestCancelPostponed
		{
			get
			{
				return new LocalizedString("ReportRequestCancelPostponed", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveHasBeenRelinquishedDueToTargetDatabaseFailover(Guid mbxGuid)
		{
			return new LocalizedString("MoveHasBeenRelinquishedDueToTargetDatabaseFailover", "Ex25A23E", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ServerNotFound(string serverLegDN)
		{
			return new LocalizedString("ServerNotFound", "Ex9F24CB", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverLegDN
			});
		}

		public static LocalizedString ReportIncrementalSyncProgress(LocalizedString physicalMailboxId, int changesApplied, int totalChanges)
		{
			return new LocalizedString("ReportIncrementalSyncProgress", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				changesApplied,
				totalChanges
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToHAStall
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquishedDueToHAStall", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFailedToLinkADPublicFolder(string publicFolderId, string objectId, string entryId)
		{
			return new LocalizedString("ReportFailedToLinkADPublicFolder", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				publicFolderId,
				objectId,
				entryId
			});
		}

		public static LocalizedString RequestPriorityLow
		{
			get
			{
				return new LocalizedString("RequestPriorityLow", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToGetPSTFolderProps(uint folderId)
		{
			return new LocalizedString("UnableToGetPSTFolderProps", "Ex35D2F6", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString MoveRequestDirectionPull
		{
			get
			{
				return new LocalizedString("MoveRequestDirectionPull", "ExB7BE2F", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToApplyFolderHierarchyChanges
		{
			get
			{
				return new LocalizedString("UnableToApplyFolderHierarchyChanges", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportArchiveUpdated(string domainController)
		{
			return new LocalizedString("ReportArchiveUpdated", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				domainController
			});
		}

		public static LocalizedString ReportSourceMailboxResetFailed(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportSourceMailboxResetFailed", "Ex3E2B7E", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString PickupStatusDisabled
		{
			get
			{
				return new LocalizedString("PickupStatusDisabled", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteResource
		{
			get
			{
				return new LocalizedString("RemoteResource", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMoveRequestSaveFailed(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportMoveRequestSaveFailed", "ExED8535", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString MoveRestartedDueToSignatureChange
		{
			get
			{
				return new LocalizedString("MoveRestartedDueToSignatureChange", "Ex367DD9", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IdentityWasNotInValidFormat(string rawIdentity)
		{
			return new LocalizedString("IdentityWasNotInValidFormat", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				rawIdentity
			});
		}

		public static LocalizedString ReportInitialSeedingStarted(int messageCount, string totalSize)
		{
			return new LocalizedString("ReportInitialSeedingStarted", "Ex23334E", false, true, MrsStrings.ResourceManager, new object[]
			{
				messageCount,
				totalSize
			});
		}

		public static LocalizedString DestinationADNotUpToDate(Guid mbxGuid)
		{
			return new LocalizedString("DestinationADNotUpToDate", "Ex580E71", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString EasFolderSyncFailedTransiently(string folderSyncStatus, string httpStatus)
		{
			return new LocalizedString("EasFolderSyncFailedTransiently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderSyncStatus,
				httpStatus
			});
		}

		public static LocalizedString FolderHierarchyContainsNoRoots
		{
			get
			{
				return new LocalizedString("FolderHierarchyContainsNoRoots", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobHasBeenRelinquishedDueToCIStall
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquishedDueToCIStall", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompositeDataContext(LocalizedString firstString, LocalizedString tail)
		{
			return new LocalizedString("CompositeDataContext", "Ex2E1326", false, true, MrsStrings.ResourceManager, new object[]
			{
				firstString,
				tail
			});
		}

		public static LocalizedString BadItemMisplacedFolder(string folderName)
		{
			return new LocalizedString("BadItemMisplacedFolder", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString PublicFolderMigrationNotSupportedFromCurrentExchange2007Version(int major, int minor, int build, int revision)
		{
			return new LocalizedString("PublicFolderMigrationNotSupportedFromCurrentExchange2007Version", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				major,
				minor,
				build,
				revision
			});
		}

		public static LocalizedString ContentIndexing
		{
			get
			{
				return new LocalizedString("ContentIndexing", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxAlreadySynced(Guid mbxGuid)
		{
			return new LocalizedString("MailboxAlreadySynced", "ExF4F35A", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString TooManyLargeItems
		{
			get
			{
				return new LocalizedString("TooManyLargeItems", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationRelationshipNotFound(string domain, string orgId)
		{
			return new LocalizedString("OrganizationRelationshipNotFound", "Ex16E4F7", false, true, MrsStrings.ResourceManager, new object[]
			{
				domain,
				orgId
			});
		}

		public static LocalizedString ReportUnableToUpdateSourceMailbox2(string errorType)
		{
			return new LocalizedString("ReportUnableToUpdateSourceMailbox2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString PickupStatusLightJob(bool suspend, bool rehomeRequest, string priority)
		{
			return new LocalizedString("PickupStatusLightJob", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				suspend,
				rehomeRequest,
				priority
			});
		}

		public static LocalizedString ReportSourceMailUserAfterFinalization(string userDataXML)
		{
			return new LocalizedString("ReportSourceMailUserAfterFinalization", "Ex529A72", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString JobIsStuck(DateTime lastProgressTimestamp, DateTime jobPickupTimestamp)
		{
			return new LocalizedString("JobIsStuck", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				lastProgressTimestamp,
				jobPickupTimestamp
			});
		}

		public static LocalizedString UnexpectedError(int hr)
		{
			return new LocalizedString("UnexpectedError", "ExD60017", false, true, MrsStrings.ResourceManager, new object[]
			{
				hr
			});
		}

		public static LocalizedString OlcSettingNotImplemented(string settingType, string settingName)
		{
			return new LocalizedString("OlcSettingNotImplemented", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				settingType,
				settingName
			});
		}

		public static LocalizedString ReportDestinationMailboxCleanupFailed(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportDestinationMailboxCleanupFailed", "Ex6BFF9C", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString ReportMoveRequestProcessedByAnotherMRS(string mrsName)
		{
			return new LocalizedString("ReportMoveRequestProcessedByAnotherMRS", "ExEE2D31", false, true, MrsStrings.ResourceManager, new object[]
			{
				mrsName
			});
		}

		public static LocalizedString CouldNotConnectToTargetMailbox
		{
			get
			{
				return new LocalizedString("CouldNotConnectToTargetMailbox", "Ex91B8F8", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToReadPSTFolder(uint folderId)
		{
			return new LocalizedString("UnableToReadPSTFolder", "Ex10A2B1", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString PSTIOException
		{
			get
			{
				return new LocalizedString("PSTIOException", "Ex6DEDE4", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxDatabaseNotUnique(string mdbIdentity)
		{
			return new LocalizedString("MailboxDatabaseNotUnique", "Ex01053A", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbIdentity
			});
		}

		public static LocalizedString RequestPriorityNormal
		{
			get
			{
				return new LocalizedString("RequestPriorityNormal", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmtpServerInfoMissing
		{
			get
			{
				return new LocalizedString("SmtpServerInfoMissing", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasFetchFailed(string errorMessage)
		{
			return new LocalizedString("EasFetchFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString FilterOperatorMustBeEQorNE(string propertyName)
		{
			return new LocalizedString("FilterOperatorMustBeEQorNE", "Ex572737", false, true, MrsStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString NoPublicFolderMailboxFoundInSource
		{
			get
			{
				return new LocalizedString("NoPublicFolderMailboxFoundInSource", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMissingItemEncountered(LocalizedString missingItemStr)
		{
			return new LocalizedString("ReportMissingItemEncountered", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				missingItemStr
			});
		}

		public static LocalizedString PickupStatusInvalidJob(string validationResult, LocalizedString validationMessage)
		{
			return new LocalizedString("PickupStatusInvalidJob", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				validationResult,
				validationMessage
			});
		}

		public static LocalizedString WorkloadTypeRemotePstExport
		{
			get
			{
				return new LocalizedString("WorkloadTypeRemotePstExport", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMoveContinued(string syncStage)
		{
			return new LocalizedString("ReportMoveContinued", "Ex8CD301", false, true, MrsStrings.ResourceManager, new object[]
			{
				syncStage
			});
		}

		public static LocalizedString ReportMoveRequestSuspended(string userName)
		{
			return new LocalizedString("ReportMoveRequestSuspended", "Ex04F380", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString PstTracingId(string filepath)
		{
			return new LocalizedString("PstTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				filepath
			});
		}

		public static LocalizedString InvalidUid(string uid)
		{
			return new LocalizedString("InvalidUid", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				uid
			});
		}

		public static LocalizedString FastTransferArgumentError
		{
			get
			{
				return new LocalizedString("FastTransferArgumentError", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToGetPSTHierarchy(string filePath)
		{
			return new LocalizedString("UnableToGetPSTHierarchy", "Ex444B92", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString PickupStatusCompletedJob
		{
			get
			{
				return new LocalizedString("PickupStatusCompletedJob", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCopyProgress2(int itemsWritten, int itemsTotal, string dataSizeCopied, string totalSize, int foldersCompleted, int totalFolders)
		{
			return new LocalizedString("ReportCopyProgress2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				itemsWritten,
				itemsTotal,
				dataSizeCopied,
				totalSize,
				foldersCompleted,
				totalFolders
			});
		}

		public static LocalizedString ReportSoftDeletedItemCountsAndSizesInArchiveLoc(LocalizedString softDeletedItems)
		{
			return new LocalizedString("ReportSoftDeletedItemCountsAndSizesInArchiveLoc", "ExE0E8AE", false, true, MrsStrings.ResourceManager, new object[]
			{
				softDeletedItems
			});
		}

		public static LocalizedString ReportMailboxAfterFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportMailboxAfterFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString ExceptionDetails(string exceptionType, int errorCode, LocalizedString exceptionMessage)
		{
			return new LocalizedString("ExceptionDetails", "Ex665B46", false, true, MrsStrings.ResourceManager, new object[]
			{
				exceptionType,
				errorCode,
				exceptionMessage
			});
		}

		public static LocalizedString ValidationNameMismatch(string jobName, string indexName)
		{
			return new LocalizedString("ValidationNameMismatch", "ExB58CB4", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobName,
				indexName
			});
		}

		public static LocalizedString ReportInitialSyncCheckpointCompleted(int foldersProcessed)
		{
			return new LocalizedString("ReportInitialSyncCheckpointCompleted", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				foldersProcessed
			});
		}

		public static LocalizedString ReportLargeItemsSkipped(int count, string totalSize)
		{
			return new LocalizedString("ReportLargeItemsSkipped", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				count,
				totalSize
			});
		}

		public static LocalizedString ReportJobProcessingDisabled
		{
			get
			{
				return new LocalizedString("ReportJobProcessingDisabled", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxArchiveInfoBeforeMove(string mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxArchiveInfoBeforeMove", "Ex5B06CC", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ReportSourceMailboxUpdateFailed(LocalizedString mailboxId, string errorType, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportSourceMailboxUpdateFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				errorType,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString EasFetchFailedTransiently(string ioStatus, string httpStatus, string folderId, string messageId)
		{
			return new LocalizedString("EasFetchFailedTransiently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				ioStatus,
				httpStatus,
				folderId,
				messageId
			});
		}

		public static LocalizedString ImproperTypeForThisIdParameter
		{
			get
			{
				return new LocalizedString("ImproperTypeForThisIdParameter", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMoveRequestIsInvalid(LocalizedString validationMessage)
		{
			return new LocalizedString("ReportMoveRequestIsInvalid", "ExDD445A", false, true, MrsStrings.ResourceManager, new object[]
			{
				validationMessage
			});
		}

		public static LocalizedString EasSendFailedError(string sendStatus, string httpStatus, string messageId)
		{
			return new LocalizedString("EasSendFailedError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				sendStatus,
				httpStatus,
				messageId
			});
		}

		public static LocalizedString ReportMovedMailUserMorphedToMailbox(string domainController)
		{
			return new LocalizedString("ReportMovedMailUserMorphedToMailbox", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				domainController
			});
		}

		public static LocalizedString CorruptSortOrderData(int flags)
		{
			return new LocalizedString("CorruptSortOrderData", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				flags
			});
		}

		public static LocalizedString ReportRetryingPostMoveCleanup(int delaySecs, int iAttempts, int iMaxRetries)
		{
			return new LocalizedString("ReportRetryingPostMoveCleanup", "Ex1C805B", false, true, MrsStrings.ResourceManager, new object[]
			{
				delaySecs,
				iAttempts,
				iMaxRetries
			});
		}

		public static LocalizedString MoveRequestMissingInfoDelete
		{
			get
			{
				return new LocalizedString("MoveRequestMissingInfoDelete", "Ex417EC3", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasFolderDeleteFailed(string errorMessage)
		{
			return new LocalizedString("EasFolderDeleteFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ErrorResourceReservation(string reservationStatus, Guid reservationId, Guid resourceId, string reservationType, string serverName)
		{
			return new LocalizedString("ErrorResourceReservation", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				reservationStatus,
				reservationId,
				resourceId,
				reservationType,
				serverName
			});
		}

		public static LocalizedString MrsProxyServiceIsDisabled2(string serverName)
		{
			return new LocalizedString("MrsProxyServiceIsDisabled2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ReportRelinquishingJobDueToServiceStop
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToServiceStop", "ExBEAE86", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasConnectFailed(string connectStatus, string httpStatus, string smtpAddress)
		{
			return new LocalizedString("EasConnectFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				connectStatus,
				httpStatus,
				smtpAddress
			});
		}

		public static LocalizedString CorruptNamedPropData(string type)
		{
			return new LocalizedString("CorruptNamedPropData", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString PickupStatusCorruptJob
		{
			get
			{
				return new LocalizedString("PickupStatusCorruptJob", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestHasBeenRelinquishedDueToBadHealthOfBackendServers
		{
			get
			{
				return new LocalizedString("RequestHasBeenRelinquishedDueToBadHealthOfBackendServers", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestMissingInfoSave
		{
			get
			{
				return new LocalizedString("MoveRequestMissingInfoSave", "ExB3585F", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RestartingMove
		{
			get
			{
				return new LocalizedString("RestartingMove", "ExEA1AA5", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceMailboxCleanupFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("SourceMailboxCleanupFailed", "ExEF4B26", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReportMailboxBeforeFinalization(string userDataXML)
		{
			return new LocalizedString("ReportMailboxBeforeFinalization", "Ex78D949", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString RequestHasBeenPostponedDueToBadHealthOfBackendServers(DateTime pickupTime)
		{
			return new LocalizedString("RequestHasBeenPostponedDueToBadHealthOfBackendServers", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString ErrorWhileUpdatingMovedMailbox
		{
			get
			{
				return new LocalizedString("ErrorWhileUpdatingMovedMailbox", "ExFA20DD", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobHasBeenRelinquishedDueToProxyThrottling(DateTime pickupTime)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToProxyThrottling", "ExDAF750", false, true, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString PopTracingId(string emailAddress)
		{
			return new LocalizedString("PopTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString MoveRequestValidationFailed
		{
			get
			{
				return new LocalizedString("MoveRequestValidationFailed", "ExDA15BA", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MustProvideValidSessionForFindingRequests
		{
			get
			{
				return new LocalizedString("MustProvideValidSessionForFindingRequests", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyMissingItems
		{
			get
			{
				return new LocalizedString("TooManyMissingItems", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveHasBeenAutoSuspendedUntilCompleteAfter(Guid mbxGuid, DateTime completeAfter)
		{
			return new LocalizedString("MoveHasBeenAutoSuspendedUntilCompleteAfter", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid,
				completeAfter
			});
		}

		public static LocalizedString UpdateFolderFailed
		{
			get
			{
				return new LocalizedString("UpdateFolderFailed", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OfflinePublicFolderMigrationNotSupported
		{
			get
			{
				return new LocalizedString("OfflinePublicFolderMigrationNotSupported", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MustRehomeRequestToSupportedVersion(string mdbID, string serverVersion)
		{
			return new LocalizedString("MustRehomeRequestToSupportedVersion", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mdbID,
				serverVersion
			});
		}

		public static LocalizedString UnsupportedSyncProtocol(string protocol)
		{
			return new LocalizedString("UnsupportedSyncProtocol", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				protocol
			});
		}

		public static LocalizedString ReportDestinationMailboxResetFailed3(string errorType, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxResetFailed3", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString MoveHasBeenCanceled(Guid mbxGuid)
		{
			return new LocalizedString("MoveHasBeenCanceled", "Ex67DC2A", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString InvalidMoveRequest(string mailboxId)
		{
			return new LocalizedString("InvalidMoveRequest", "Ex11D5F8", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ErrorWlmResourceUnhealthy1(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, double reportedLoadRatio, string reportedLoadState, string metric)
		{
			return new LocalizedString("ErrorWlmResourceUnhealthy1", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceType,
				wlmResourceKey,
				wlmResourceMetricType,
				reportedLoadRatio,
				reportedLoadState,
				metric
			});
		}

		public static LocalizedString TaskCanceled
		{
			get
			{
				return new LocalizedString("TaskCanceled", "Ex6CBC51", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRelinquishBecauseMailboxIsLocked(DateTime pickupTime)
		{
			return new LocalizedString("ReportRelinquishBecauseMailboxIsLocked", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString SourceMailboxAlreadyBeingMoved
		{
			get
			{
				return new LocalizedString("SourceMailboxAlreadyBeingMoved", "Ex7E66C2", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWlmResourceUnhealthy(string resourceName, string resourceType, string wlmResourceKey, double reportedLoadRatio, string reportedLoadState, string metric)
		{
			return new LocalizedString("ErrorWlmResourceUnhealthy", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceType,
				wlmResourceKey,
				reportedLoadRatio,
				reportedLoadState,
				metric
			});
		}

		public static LocalizedString PickupStatusCreateJob(string syncStage, bool cancelRequest, string priority)
		{
			return new LocalizedString("PickupStatusCreateJob", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				syncStage,
				cancelRequest,
				priority
			});
		}

		public static LocalizedString MailboxDatabaseNotFoundById(string mdbIdentity, LocalizedString notFoundReason)
		{
			return new LocalizedString("MailboxDatabaseNotFoundById", "ExD09830", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbIdentity,
				notFoundReason
			});
		}

		public static LocalizedString MoveJobDeserializationFailed
		{
			get
			{
				return new LocalizedString("MoveJobDeserializationFailed", "Ex6EA931", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestNotFoundInQueue
		{
			get
			{
				return new LocalizedString("MoveRequestNotFoundInQueue", "Ex7BDDE5", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobHasBeenCanceled
		{
			get
			{
				return new LocalizedString("JobHasBeenCanceled", "Ex5D2B5E", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedRecipientType(string recipient, string recipientType)
		{
			return new LocalizedString("UnsupportedRecipientType", "Ex936D59", false, true, MrsStrings.ResourceManager, new object[]
			{
				recipient,
				recipientType
			});
		}

		public static LocalizedString SimpleValueDataContext(string name, string value)
		{
			return new LocalizedString("SimpleValueDataContext", "Ex7A0F13", false, true, MrsStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString PropTagsDataContext(string propTagsStr)
		{
			return new LocalizedString("PropTagsDataContext", "Ex785429", false, true, MrsStrings.ResourceManager, new object[]
			{
				propTagsStr
			});
		}

		public static LocalizedString ReportRequestStarted
		{
			get
			{
				return new LocalizedString("ReportRequestStarted", "Ex8B0872", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMergeInitialized(LocalizedString physicalMailboxId, int totalFolders, int messageCount, string totalSizeStr)
		{
			return new LocalizedString("ReportMergeInitialized", "Ex024C01", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				totalFolders,
				messageCount,
				totalSizeStr
			});
		}

		public static LocalizedString InvalidOperationError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("InvalidOperationError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString ErrorDownlevelClientsNotSupported
		{
			get
			{
				return new LocalizedString("ErrorDownlevelClientsNotSupported", "Ex9D981C", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataExportTimeout
		{
			get
			{
				return new LocalizedString("DataExportTimeout", "Ex66C917", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TargetMailboxConnectionWasLost
		{
			get
			{
				return new LocalizedString("TargetMailboxConnectionWasLost", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFailedToUpdateUserSD2(string errorType)
		{
			return new LocalizedString("ReportFailedToUpdateUserSD2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString ReportMoveRequestSet(string userName)
		{
			return new LocalizedString("ReportMoveRequestSet", "Ex6F6EDC", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString PickupStatusJobPoisoned(int poisionCount)
		{
			return new LocalizedString("PickupStatusJobPoisoned", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				poisionCount
			});
		}

		public static LocalizedString UnexpectedFilterType(string filterTypeName)
		{
			return new LocalizedString("UnexpectedFilterType", "ExBB52FB", false, true, MrsStrings.ResourceManager, new object[]
			{
				filterTypeName
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToDatabaseFailover
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquishedDueToDatabaseFailover", "ExBC0F22", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreIntegError(int error)
		{
			return new LocalizedString("StoreIntegError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString PublicFolderMailboxesNotProvisionedForMigration
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxesNotProvisionedForMigration", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWlmCapacityExceeded3(string resourceName, string resourceType, string wlmResourceKey, int wlmResourceMetricType, int capacity)
		{
			return new LocalizedString("ErrorWlmCapacityExceeded3", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceType,
				wlmResourceKey,
				wlmResourceMetricType,
				capacity
			});
		}

		public static LocalizedString ClusterIPNotFound(IPAddress clusterIp)
		{
			return new LocalizedString("ClusterIPNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				clusterIp
			});
		}

		public static LocalizedString RequestPriorityHigher
		{
			get
			{
				return new LocalizedString("RequestPriorityHigher", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobHasBeenRelinquishedDueToHAOrCIStalls
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquishedDueToHAOrCIStalls", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRequestCanceled
		{
			get
			{
				return new LocalizedString("ReportRequestCanceled", "ExAEFE90", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidProxyOperationOrder
		{
			get
			{
				return new LocalizedString("InvalidProxyOperationOrder", "ExEA3D43", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEscapedChar(string folderPath, int charPosition)
		{
			return new LocalizedString("InvalidEscapedChar", "Ex6B03CE", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderPath,
				charPosition
			});
		}

		public static LocalizedString ReportFailedToApplySearchCondition(string folderName, string errorType, LocalizedString error, string trace, LocalizedString dataContext)
		{
			return new LocalizedString("ReportFailedToApplySearchCondition", "Ex04A902", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderName,
				errorType,
				error,
				trace,
				dataContext
			});
		}

		public static LocalizedString BadItemMissingFolder(string folderName)
		{
			return new LocalizedString("BadItemMissingFolder", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString ReportRequestOfflineMovePostponed
		{
			get
			{
				return new LocalizedString("ReportRequestOfflineMovePostponed", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderAliasIsInvalid(string folderAlias)
		{
			return new LocalizedString("FolderAliasIsInvalid", "ExDAD81E", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderAlias
			});
		}

		public static LocalizedString MailboxIsBeingMoved
		{
			get
			{
				return new LocalizedString("MailboxIsBeingMoved", "Ex0B3BF6", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderDataContextGeneric(string folderName, string entryId, string parentId)
		{
			return new LocalizedString("FolderDataContextGeneric", "Ex4EFBAA", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderName,
				entryId,
				parentId
			});
		}

		public static LocalizedString ReportFailingMoveBecauseSyncStateIssue(string mbxId)
		{
			return new LocalizedString("ReportFailingMoveBecauseSyncStateIssue", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxId
			});
		}

		public static LocalizedString NoSuchRequestInSpecifiedIndex
		{
			get
			{
				return new LocalizedString("NoSuchRequestInSpecifiedIndex", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxRemovedRetrying(int delaySecs)
		{
			return new LocalizedString("ReportMailboxRemovedRetrying", "ExF9A9F7", false, true, MrsStrings.ResourceManager, new object[]
			{
				delaySecs
			});
		}

		public static LocalizedString InitializedWithInvalidObjectId
		{
			get
			{
				return new LocalizedString("InitializedWithInvalidObjectId", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCopyPerUserReadUnreadDataCompleted
		{
			get
			{
				return new LocalizedString("ReportCopyPerUserReadUnreadDataCompleted", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRequestSet(string userName)
		{
			return new LocalizedString("ReportRequestSet", "Ex82B05E", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString PublicFolderMoveTracingId(string orgID, Guid mbxGuid)
		{
			return new LocalizedString("PublicFolderMoveTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				orgID,
				mbxGuid
			});
		}

		public static LocalizedString ReportSessionStatisticsUpdated
		{
			get
			{
				return new LocalizedString("ReportSessionStatisticsUpdated", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRelinquishingJobDueToServerThrottling
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToServerThrottling", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToReadPSTMessage(string filePath, uint messageId)
		{
			return new LocalizedString("UnableToReadPSTMessage", "Ex7AC263", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath,
				messageId
			});
		}

		public static LocalizedString ReportTransientExceptionOccurred(string errorType, LocalizedString errorMsg, string trace, int retryCount, int maxRetries, LocalizedString context)
		{
			return new LocalizedString("ReportTransientExceptionOccurred", "ExA03465", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace,
				retryCount,
				maxRetries,
				context
			});
		}

		public static LocalizedString IsIntegAttemptsExceededError(short attempts)
		{
			return new LocalizedString("IsIntegAttemptsExceededError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				attempts
			});
		}

		public static LocalizedString MRSNotConfigured
		{
			get
			{
				return new LocalizedString("MRSNotConfigured", "ExF550B8", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRootFolderNotFound
		{
			get
			{
				return new LocalizedString("MailboxRootFolderNotFound", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasSyncFailedPermanently(string syncStatus, string httpStatus, string folderId)
		{
			return new LocalizedString("EasSyncFailedPermanently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				syncStatus,
				httpStatus,
				folderId
			});
		}

		public static LocalizedString FolderDataContextRoot(string entryId)
		{
			return new LocalizedString("FolderDataContextRoot", "ExF4072D", false, true, MrsStrings.ResourceManager, new object[]
			{
				entryId
			});
		}

		public static LocalizedString SourceMailboxIsNotInSourceMDB(Guid mdbGuid)
		{
			return new LocalizedString("SourceMailboxIsNotInSourceMDB", "Ex8BAFEE", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString DestinationAddMoveHistoryEntryFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("DestinationAddMoveHistoryEntryFailed", "Ex5E766E", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReportReplaySyncStateNull(Guid requestGuid)
		{
			return new LocalizedString("ReportReplaySyncStateNull", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid
			});
		}

		public static LocalizedString JobIsPoisoned(int poisonCount)
		{
			return new LocalizedString("JobIsPoisoned", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				poisonCount
			});
		}

		public static LocalizedString WorkloadTypeLoadBalancing
		{
			get
			{
				return new LocalizedString("WorkloadTypeLoadBalancing", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobIsQuarantined
		{
			get
			{
				return new LocalizedString("JobIsQuarantined", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationFlagsMismatch2(string jobFlags, string indexFlags)
		{
			return new LocalizedString("ValidationFlagsMismatch2", "ExCA5911", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobFlags,
				indexFlags
			});
		}

		public static LocalizedString ReportSourceSDCannotBeRead
		{
			get
			{
				return new LocalizedString("ReportSourceSDCannotBeRead", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMoveRequestIsNoLongerSticky
		{
			get
			{
				return new LocalizedString("ReportMoveRequestIsNoLongerSticky", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDataError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("InvalidDataError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString ClusterNotFound
		{
			get
			{
				return new LocalizedString("ClusterNotFound", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestMessageError(LocalizedString message)
		{
			return new LocalizedString("MoveRequestMessageError", "Ex4657E1", false, true, MrsStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString UnsupportedClientVersion(string clientVersion)
		{
			return new LocalizedString("UnsupportedClientVersion", "ExA7EA19", false, true, MrsStrings.ResourceManager, new object[]
			{
				clientVersion
			});
		}

		public static LocalizedString ICSViewDataContext(string icsViewStr)
		{
			return new LocalizedString("ICSViewDataContext", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				icsViewStr
			});
		}

		public static LocalizedString MoveRestartDueToIsIntegCheck
		{
			get
			{
				return new LocalizedString("MoveRestartDueToIsIntegCheck", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportJobIsStillStalled
		{
			get
			{
				return new LocalizedString("ReportJobIsStillStalled", "ExF99E7D", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationNoIndexEntryForRequest(string requestId)
		{
			return new LocalizedString("ValidationNoIndexEntryForRequest", "Ex219CEC", false, true, MrsStrings.ResourceManager, new object[]
			{
				requestId
			});
		}

		public static LocalizedString MailboxSettingsJunkMailError(string collectionName, string itemList, string validationError)
		{
			return new LocalizedString("MailboxSettingsJunkMailError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				collectionName,
				itemList,
				validationError
			});
		}

		public static LocalizedString WorkloadTypeRemotePstIngestion
		{
			get
			{
				return new LocalizedString("WorkloadTypeRemotePstIngestion", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportInitialSyncCheckpointCreationProgress(int foldersProcessed, int totalFolders, LocalizedString physicalMailboxId)
		{
			return new LocalizedString("ReportInitialSyncCheckpointCreationProgress", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				foldersProcessed,
				totalFolders,
				physicalMailboxId
			});
		}

		public static LocalizedString NestedExceptionMsg(LocalizedString message, LocalizedString innerMessage)
		{
			return new LocalizedString("NestedExceptionMsg", "Ex9D5913", false, true, MrsStrings.ResourceManager, new object[]
			{
				message,
				innerMessage
			});
		}

		public static LocalizedString ReportPrimaryMservEntryPointsToExo
		{
			get
			{
				return new LocalizedString("ReportPrimaryMservEntryPointsToExo", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasFetchFailedPermanently(string ioStatus, string httpStatus, string folderId, string messageId)
		{
			return new LocalizedString("EasFetchFailedPermanently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				ioStatus,
				httpStatus,
				folderId,
				messageId
			});
		}

		public static LocalizedString MdbNotOnServer(string mdbName, Guid mdbId, string mdbServerFqdn, string currentServerFqdn)
		{
			return new LocalizedString("MdbNotOnServer", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mdbName,
				mdbId,
				mdbServerFqdn,
				currentServerFqdn
			});
		}

		public static LocalizedString RPCHTTPMailboxId(string legDN)
		{
			return new LocalizedString("RPCHTTPMailboxId", "Ex9A73BC", false, true, MrsStrings.ResourceManager, new object[]
			{
				legDN
			});
		}

		public static LocalizedString ValidationObjectInvolvedInMultipleRelocations(LocalizedString objectInvolved, string requestGuids)
		{
			return new LocalizedString("ValidationObjectInvolvedInMultipleRelocations", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				objectInvolved,
				requestGuids
			});
		}

		public static LocalizedString ReportDestinationMailboxSeedMBICacheFailed2(string errorType, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxSeedMBICacheFailed2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString UnknownSecurityProp(int securityProp)
		{
			return new LocalizedString("UnknownSecurityProp", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				securityProp
			});
		}

		public static LocalizedString ReportMovedMailboxMorphedToMailUser(string domainController)
		{
			return new LocalizedString("ReportMovedMailboxMorphedToMailUser", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				domainController
			});
		}

		public static LocalizedString ValidationADUserIsNotBeingMoved
		{
			get
			{
				return new LocalizedString("ValidationADUserIsNotBeingMoved", "ExB8C59E", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteServerName(string serverName)
		{
			return new LocalizedString("RemoteServerName", "ExC45085", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ReportMailboxInfoBeforeMove(string mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxInfoBeforeMove", "Ex69C3B4", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ErrorCouldNotFindMoveRequest(string identity)
		{
			return new LocalizedString("ErrorCouldNotFindMoveRequest", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString PostMoveStateIsUncertain
		{
			get
			{
				return new LocalizedString("PostMoveStateIsUncertain", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedValue(string value, string parameterName)
		{
			return new LocalizedString("UnexpectedValue", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				value,
				parameterName
			});
		}

		public static LocalizedString RestoreMailboxId(Guid mailboxGuid)
		{
			return new LocalizedString("RestoreMailboxId", "Ex5477E9", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString UnableToSavePSTSyncState(string filePath)
		{
			return new LocalizedString("UnableToSavePSTSyncState", "Ex934321", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString MissingDatabaseName2(Guid dbGuid, string forestFqdn)
		{
			return new LocalizedString("MissingDatabaseName2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				dbGuid,
				forestFqdn
			});
		}

		public static LocalizedString ReportIncrementalSyncContentChangesSynced(LocalizedString physicalMailboxId, int messageChanges)
		{
			return new LocalizedString("ReportIncrementalSyncContentChangesSynced", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				messageChanges
			});
		}

		public static LocalizedString RequestPriorityHigh
		{
			get
			{
				return new LocalizedString("RequestPriorityHigh", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MRSProxyConnectionLimitReachedError(int activeConnections, int connectionLimit)
		{
			return new LocalizedString("MRSProxyConnectionLimitReachedError", "Ex56F050", false, true, MrsStrings.ResourceManager, new object[]
			{
				activeConnections,
				connectionLimit
			});
		}

		public static LocalizedString ValidationUserIsNotInAD(string mrUserId)
		{
			return new LocalizedString("ValidationUserIsNotInAD", "ExDC37E6", false, true, MrsStrings.ResourceManager, new object[]
			{
				mrUserId
			});
		}

		public static LocalizedString SourceContainer
		{
			get
			{
				return new LocalizedString("SourceContainer", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRequestRemoved(string userName)
		{
			return new LocalizedString("ReportRequestRemoved", "ExA3AF49", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString ReportRequestCreated(string userName)
		{
			return new LocalizedString("ReportRequestCreated", "Ex484BDB", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString WorkloadTypeTenantUpgrade
		{
			get
			{
				return new LocalizedString("WorkloadTypeTenantUpgrade", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DestinationMailboxResetFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("DestinationMailboxResetFailed", "ExCFFF85", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReportMoveAlreadyFinished2(LocalizedString reasonForMoveFinished)
		{
			return new LocalizedString("ReportMoveAlreadyFinished2", "Ex7B031B", false, true, MrsStrings.ResourceManager, new object[]
			{
				reasonForMoveFinished
			});
		}

		public static LocalizedString EasMissingMessageCategory
		{
			get
			{
				return new LocalizedString("EasMissingMessageCategory", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PositionInteger(int position)
		{
			return new LocalizedString("PositionInteger", "Ex0C9383", false, true, MrsStrings.ResourceManager, new object[]
			{
				position
			});
		}

		public static LocalizedString ValidationMoveRequestInWrongMDB(Guid originatingMdbGuid, Guid mrQueueGuid)
		{
			return new LocalizedString("ValidationMoveRequestInWrongMDB", "ExC890DD", false, true, MrsStrings.ResourceManager, new object[]
			{
				originatingMdbGuid,
				mrQueueGuid
			});
		}

		public static LocalizedString BadItemMissingItem(string msgClass, string subject, string folderName)
		{
			return new LocalizedString("BadItemMissingItem", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				msgClass,
				subject,
				folderName
			});
		}

		public static LocalizedString CrossSiteError(Guid mdbGuid, Guid serverGuid, string serverSite, string localSite)
		{
			return new LocalizedString("CrossSiteError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mdbGuid,
				serverGuid,
				serverSite,
				localSite
			});
		}

		public static LocalizedString ReportSourceMailboxCleanupFailed3(LocalizedString mailboxId, string errorType, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportSourceMailboxCleanupFailed3", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				errorType,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString ReportRequestIsSticky(string stickyServer)
		{
			return new LocalizedString("ReportRequestIsSticky", "ExB16C26", false, true, MrsStrings.ResourceManager, new object[]
			{
				stickyServer
			});
		}

		public static LocalizedString SettingRehomeOnRelatedRequestsFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("SettingRehomeOnRelatedRequestsFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString FolderHierarchyContainsParentChainLoop(string folderIdStr)
		{
			return new LocalizedString("FolderHierarchyContainsParentChainLoop", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderIdStr
			});
		}

		public static LocalizedString ReportRestartingMoveBecauseSyncStateDoesNotExist(string mbxId)
		{
			return new LocalizedString("ReportRestartingMoveBecauseSyncStateDoesNotExist", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxId
			});
		}

		public static LocalizedString JobHasBeenRelinquished
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquished", "ExBE7BFD", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CertificateLoadError(string certificateName, string errorMessage)
		{
			return new LocalizedString("CertificateLoadError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				certificateName,
				errorMessage
			});
		}

		public static LocalizedString KBytesPerSec(double kbytesPerSec)
		{
			return new LocalizedString("KBytesPerSec", "Ex56AEBC", false, true, MrsStrings.ResourceManager, new object[]
			{
				kbytesPerSec
			});
		}

		public static LocalizedString ValidationTargetUserMismatch(string jobTgtUser, string indexTgtUser)
		{
			return new LocalizedString("ValidationTargetUserMismatch", "Ex3C4DD6", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobTgtUser,
				indexTgtUser
			});
		}

		public static LocalizedString OperationDataContext(string operation)
		{
			return new LocalizedString("OperationDataContext", "Ex675D9C", false, true, MrsStrings.ResourceManager, new object[]
			{
				operation
			});
		}

		public static LocalizedString ReportRelinquishBecauseResourceReservationFailed(LocalizedString error)
		{
			return new LocalizedString("ReportRelinquishBecauseResourceReservationFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString RecoverySyncNotImplemented
		{
			get
			{
				return new LocalizedString("RecoverySyncNotImplemented", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportIncrementalSyncCompleted2(LocalizedString physicalMailboxId, int numberOfHierarchyUpdates, int numberOfContentUpdates)
		{
			return new LocalizedString("ReportIncrementalSyncCompleted2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				numberOfHierarchyUpdates,
				numberOfContentUpdates
			});
		}

		public static LocalizedString CommunicationError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("CommunicationError", "ExE8AAEA", false, true, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString ErrorTooManyCleanupRetries
		{
			get
			{
				return new LocalizedString("ErrorTooManyCleanupRetries", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportBadItemEncountered(string badItemData)
		{
			return new LocalizedString("ReportBadItemEncountered", "ExE0F6EA", false, true, MrsStrings.ResourceManager, new object[]
			{
				badItemData
			});
		}

		public static LocalizedString InvalidServerName(string serverName)
		{
			return new LocalizedString("InvalidServerName", "Ex5984CC", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ReportFinalSyncStarted
		{
			get
			{
				return new LocalizedString("ReportFinalSyncStarted", "Ex1B5796", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OnlineMoveNotSupported(string mbxGuid)
		{
			return new LocalizedString("OnlineMoveNotSupported", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString UnsupportedRemoteServerVersion(string remoteServerAddress, string serverVersion)
		{
			return new LocalizedString("UnsupportedRemoteServerVersion", "Ex052E7B", false, true, MrsStrings.ResourceManager, new object[]
			{
				remoteServerAddress,
				serverVersion
			});
		}

		public static LocalizedString ReportJobExitedStalledByThrottlingState
		{
			get
			{
				return new LocalizedString("ReportJobExitedStalledByThrottlingState", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportBadItemEncountered2(LocalizedString badItemStr)
		{
			return new LocalizedString("ReportBadItemEncountered2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				badItemStr
			});
		}

		public static LocalizedString MustProvideNonEmptyStringForIdentity
		{
			get
			{
				return new LocalizedString("MustProvideNonEmptyStringForIdentity", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRelinquishingJobDueToNeedForRehome
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToNeedForRehome", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSyncedJob(DateTime pikUpTime)
		{
			return new LocalizedString("ReportSyncedJob", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pikUpTime
			});
		}

		public static LocalizedString NotEnoughInformationSupplied
		{
			get
			{
				return new LocalizedString("NotEnoughInformationSupplied", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationRequestTypeMismatch(string jobType, string indexType)
		{
			return new LocalizedString("ValidationRequestTypeMismatch", "ExFD2B3F", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobType,
				indexType
			});
		}

		public static LocalizedString NoDataContext
		{
			get
			{
				return new LocalizedString("NoDataContext", "Ex488979", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientIsNotAMailbox(string recipient)
		{
			return new LocalizedString("RecipientIsNotAMailbox", "Ex3D8115", false, true, MrsStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ReportFailedToDisconnectFromSource(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportFailedToDisconnectFromSource", "Ex247F65", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString UnableToCreateToken(string user)
		{
			return new LocalizedString("UnableToCreateToken", "ExCFA47C", false, true, MrsStrings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ReportMoveCompleted
		{
			get
			{
				return new LocalizedString("ReportMoveCompleted", "Ex219D9F", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasFetchFailedError(string fetchStatus, string httpStatus, string folderId)
		{
			return new LocalizedString("EasFetchFailedError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				fetchStatus,
				httpStatus,
				folderId
			});
		}

		public static LocalizedString ReportMoveRequestIsSticky(string stickyServer)
		{
			return new LocalizedString("ReportMoveRequestIsSticky", "Ex159027", false, true, MrsStrings.ResourceManager, new object[]
			{
				stickyServer
			});
		}

		public static LocalizedString ReportMailboxArchiveInfoBeforeMoveLoc(LocalizedString mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxArchiveInfoBeforeMoveLoc", "Ex6D940D", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ReportDestinationMailboxCleanupFailed2(string errorType)
		{
			return new LocalizedString("ReportDestinationMailboxCleanupFailed2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString UnableToDeleteMoveRequestMessage
		{
			get
			{
				return new LocalizedString("UnableToDeleteMoveRequestMessage", "Ex24DBFE", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobIsStalled(string jobId, string mdbId, LocalizedString failureReason, string agentName, int agentId)
		{
			return new LocalizedString("JobIsStalled", "Ex9DAB41", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobId,
				mdbId,
				failureReason,
				agentName,
				agentId
			});
		}

		public static LocalizedString ReportRequestResumedWithSuspendWhenReadyToComplete(string userName)
		{
			return new LocalizedString("ReportRequestResumedWithSuspendWhenReadyToComplete", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString DestinationFolderHierarchyInconsistent
		{
			get
			{
				return new LocalizedString("DestinationFolderHierarchyInconsistent", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BlockedType(string type)
		{
			return new LocalizedString("BlockedType", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString PositionIntegerPlus(int position)
		{
			return new LocalizedString("PositionIntegerPlus", "Ex14FD3C", false, true, MrsStrings.ResourceManager, new object[]
			{
				position
			});
		}

		public static LocalizedString MoveHasBeenRelinquished(Guid mbxGuid)
		{
			return new LocalizedString("MoveHasBeenRelinquished", "ExC86B5F", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString FolderIsLive(string folderName)
		{
			return new LocalizedString("FolderIsLive", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString ValidationSourceMDBMismatch(string adDatabase, string mrDatabase)
		{
			return new LocalizedString("ValidationSourceMDBMismatch", "Ex73B8CA", false, true, MrsStrings.ResourceManager, new object[]
			{
				adDatabase,
				mrDatabase
			});
		}

		public static LocalizedString ReportMergingFolder(string sourceFolderName, string targetFolderName)
		{
			return new LocalizedString("ReportMergingFolder", "ExB5FDAB", false, true, MrsStrings.ResourceManager, new object[]
			{
				sourceFolderName,
				targetFolderName
			});
		}

		public static LocalizedString ReportSourceMailboxBeforeFinalization(string userDataXML)
		{
			return new LocalizedString("ReportSourceMailboxBeforeFinalization", "ExDD9B47", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString ValidationSourceArchiveMDBMismatch(string adDatabase, string mrDatabase)
		{
			return new LocalizedString("ValidationSourceArchiveMDBMismatch", "Ex3935BC", false, true, MrsStrings.ResourceManager, new object[]
			{
				adDatabase,
				mrDatabase
			});
		}

		public static LocalizedString ReportDestinationMailboxClearSyncStateFailed2(string errorType, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxClearSyncStateFailed2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString UnableToDetermineMDBSite(Guid mdbGuid)
		{
			return new LocalizedString("UnableToDetermineMDBSite", "Ex44A805", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString KBytes(double kbytes)
		{
			return new LocalizedString("KBytes", "ExDBCEA5", false, true, MrsStrings.ResourceManager, new object[]
			{
				kbytes
			});
		}

		public static LocalizedString NotEnoughInformationToFindMoveRequest
		{
			get
			{
				return new LocalizedString("NotEnoughInformationToFindMoveRequest", "Ex315C49", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StorageConnectionType(string type)
		{
			return new LocalizedString("StorageConnectionType", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString PropTagToPropertyDefinitionConversion(int propTag)
		{
			return new LocalizedString("PropTagToPropertyDefinitionConversion", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				propTag
			});
		}

		public static LocalizedString TaskSchedulerStopped
		{
			get
			{
				return new LocalizedString("TaskSchedulerStopped", "Ex9F286D", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRelinquishingJobDueToCIStall
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToCIStall", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxDataReplicationFailed(LocalizedString failureReason)
		{
			return new LocalizedString("MailboxDataReplicationFailed", "Ex5186AA", false, true, MrsStrings.ResourceManager, new object[]
			{
				failureReason
			});
		}

		public static LocalizedString HandleNotFound(long handle)
		{
			return new LocalizedString("HandleNotFound", "ExAEA2A1", false, true, MrsStrings.ResourceManager, new object[]
			{
				handle
			});
		}

		public static LocalizedString WriteCpu
		{
			get
			{
				return new LocalizedString("WriteCpu", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MdbIsOffline(Guid mdbGuid)
		{
			return new LocalizedString("MdbIsOffline", "ExC2B9A8", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString UnableToLoadPSTSyncState(string filePath)
		{
			return new LocalizedString("UnableToLoadPSTSyncState", "Ex1A9EB5", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString ReportSyncStateLoaded2(Guid requestGuid, int syncStateLength, int icsSyncStateLength, int replaySyncStateLength)
		{
			return new LocalizedString("ReportSyncStateLoaded2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				syncStateLength,
				icsSyncStateLength,
				replaySyncStateLength
			});
		}

		public static LocalizedString ReportSoftDeletedItemCountsAndSizesInArchive(string softDeletedItems)
		{
			return new LocalizedString("ReportSoftDeletedItemCountsAndSizesInArchive", "Ex1F925D", false, true, MrsStrings.ResourceManager, new object[]
			{
				softDeletedItems
			});
		}

		public static LocalizedString ReportHomeMdbPointsToTarget
		{
			get
			{
				return new LocalizedString("ReportHomeMdbPointsToTarget", "Ex47A346", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapTracingId(string emailAddress)
		{
			return new LocalizedString("ImapTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString ReportIncrementalSyncContentChanges(LocalizedString physicalMailboxId, int messageChanges)
		{
			return new LocalizedString("ReportIncrementalSyncContentChanges", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				messageChanges
			});
		}

		public static LocalizedString UnableToFindMbxServer(string server)
		{
			return new LocalizedString("UnableToFindMbxServer", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString MailboxDatabaseNotFoundByGuid(Guid dbGuid)
		{
			return new LocalizedString("MailboxDatabaseNotFoundByGuid", "ExED7079", false, true, MrsStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString RequestTypeNotUnderstoodOnThisServer(string serverName, string serverVersion, int requestType)
		{
			return new LocalizedString("RequestTypeNotUnderstoodOnThisServer", "Ex53ADB0", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				requestType
			});
		}

		public static LocalizedString ReportCopyProgress(int itemsWritten, int itemsTotal, string dataSizeCopied, string totalSize)
		{
			return new LocalizedString("ReportCopyProgress", "Ex2CE5A9", false, true, MrsStrings.ResourceManager, new object[]
			{
				itemsWritten,
				itemsTotal,
				dataSizeCopied,
				totalSize
			});
		}

		public static LocalizedString ValidationSourceUserMismatch(string jobSrcUser, string indexSrcUser)
		{
			return new LocalizedString("ValidationSourceUserMismatch", "Ex6FE853", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobSrcUser,
				indexSrcUser
			});
		}

		public static LocalizedString TargetRecipientIsNotAnMEU(string recipient)
		{
			return new LocalizedString("TargetRecipientIsNotAnMEU", "Ex5E4C38", false, true, MrsStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString CorruptRestrictionData
		{
			get
			{
				return new LocalizedString("CorruptRestrictionData", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportIncrementalMoveRestartDueToGlobalCounterRangeDepletion
		{
			get
			{
				return new LocalizedString("ReportIncrementalMoveRestartDueToGlobalCounterRangeDepletion", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestMessageWarningSeparator(LocalizedString startMessage, LocalizedString additionalMessage)
		{
			return new LocalizedString("MoveRequestMessageWarningSeparator", "Ex296E0F", false, true, MrsStrings.ResourceManager, new object[]
			{
				startMessage,
				additionalMessage
			});
		}

		public static LocalizedString ReportRequestResumed(string userName)
		{
			return new LocalizedString("ReportRequestResumed", "Ex88D0AA", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString ReportSyncStateCorrupt(Guid requestGuid, int length, string start, string end)
		{
			return new LocalizedString("ReportSyncStateCorrupt", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				length,
				start,
				end
			});
		}

		public static LocalizedString ReportMoveIsStalled(LocalizedString failureMsg, int retryCount, int maxRetries)
		{
			return new LocalizedString("ReportMoveIsStalled", "Ex0409FE", false, true, MrsStrings.ResourceManager, new object[]
			{
				failureMsg,
				retryCount,
				maxRetries
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToMailboxLockout(DateTime pickupTime)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToMailboxLockout", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString DestinationMailboxNotCleanedUp(Guid mbxGuid)
		{
			return new LocalizedString("DestinationMailboxNotCleanedUp", "Ex333D54", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString PublicFolderMove(string mbxGuid)
		{
			return new LocalizedString("PublicFolderMove", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ReportSourceMailboxCleanupSucceeded(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportSourceMailboxCleanupSucceeded", "ExC21FE4", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ReportDestinationMailboxResetNotGuaranteed(string errorType)
		{
			return new LocalizedString("ReportDestinationMailboxResetNotGuaranteed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString ReadRpc
		{
			get
			{
				return new LocalizedString("ReadRpc", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkloadTypeLocal
		{
			get
			{
				return new LocalizedString("WorkloadTypeLocal", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CommunicationWithRemoteServiceFailed(string endpoint)
		{
			return new LocalizedString("CommunicationWithRemoteServiceFailed", "ExB22688", false, true, MrsStrings.ResourceManager, new object[]
			{
				endpoint
			});
		}

		public static LocalizedString MailboxIsNotBeingMoved(string mailboxId)
		{
			return new LocalizedString("MailboxIsNotBeingMoved", "Ex0BED2C", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString MoveRequestDirectionPush
		{
			get
			{
				return new LocalizedString("MoveRequestDirectionPush", "ExB24789", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWlmCapacityExceeded2(string resourceName, string resourceType, string wlmResourceKey, int capacity)
		{
			return new LocalizedString("ErrorWlmCapacityExceeded2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceType,
				wlmResourceKey,
				capacity
			});
		}

		public static LocalizedString PublicFolderMigrationNotSupportedFromExchange2003OrEarlier(int major, int minor, int build, int revision)
		{
			return new LocalizedString("PublicFolderMigrationNotSupportedFromExchange2003OrEarlier", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				major,
				minor,
				build,
				revision
			});
		}

		public static LocalizedString SourceFolderHierarchyInconsistent
		{
			get
			{
				return new LocalizedString("SourceFolderHierarchyInconsistent", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotFindDcHavingUmmUpdateError(Guid expectedDb, string recipient)
		{
			return new LocalizedString("CouldNotFindDcHavingUmmUpdateError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				expectedDb,
				recipient
			});
		}

		public static LocalizedString UnknownRestrictionType(string type)
		{
			return new LocalizedString("UnknownRestrictionType", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ReportDestinationMailboxClearSyncStateSucceeded(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportDestinationMailboxClearSyncStateSucceeded", "Ex71ED82", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ReportInitializingMove(string serverName, string serverVersion)
		{
			return new LocalizedString("ReportInitializingMove", "Ex37BEC6", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion
			});
		}

		public static LocalizedString BadItemFolderRule(string folderName)
		{
			return new LocalizedString("BadItemFolderRule", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString FolderIsMissing(string folderPath)
		{
			return new LocalizedString("FolderIsMissing", "Ex996590", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderPath
			});
		}

		public static LocalizedString RequestPriorityHighest
		{
			get
			{
				return new LocalizedString("RequestPriorityHighest", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportInitializingFolderHierarchy(LocalizedString physicalMailboxId, int totalFolders)
		{
			return new LocalizedString("ReportInitializingFolderHierarchy", "Ex56B873", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				totalFolders
			});
		}

		public static LocalizedString ReportSyncStateSaveFailed2(string errorType)
		{
			return new LocalizedString("ReportSyncStateSaveFailed2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString ErrorFinalizationIsBlocked
		{
			get
			{
				return new LocalizedString("ErrorFinalizationIsBlocked", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSyncStateLoaded(Guid requestGuid, int syncStateLength, int icsSyncStateLength)
		{
			return new LocalizedString("ReportSyncStateLoaded", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				syncStateLength,
				icsSyncStateLength
			});
		}

		public static LocalizedString ReportUpdateMovedMailboxFailureAfterADSwitchover(LocalizedString error)
		{
			return new LocalizedString("ReportUpdateMovedMailboxFailureAfterADSwitchover", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString MailboxDoesNotExist(LocalizedString mbxId)
		{
			return new LocalizedString("MailboxDoesNotExist", "ExF60D53", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxId
			});
		}

		public static LocalizedString PositionOfMoveRequestInSystemMailboxQueue(string positionInQueue, string totalQueueLength)
		{
			return new LocalizedString("PositionOfMoveRequestInSystemMailboxQueue", "ExD035A9", false, true, MrsStrings.ResourceManager, new object[]
			{
				positionInQueue,
				totalQueueLength
			});
		}

		public static LocalizedString ArchiveMailboxTracingId(string orgID, Guid mbxGuid)
		{
			return new LocalizedString("ArchiveMailboxTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				orgID,
				mbxGuid
			});
		}

		public static LocalizedString ReportRetryingMailboxCreation(LocalizedString physicalMailboxId, int delaySecs, int iAttempts, int iMaxRetries)
		{
			return new LocalizedString("ReportRetryingMailboxCreation", "ExC8C4B1", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				delaySecs,
				iAttempts,
				iMaxRetries
			});
		}

		public static LocalizedString ReportInitialSeedingCompleted(int messageCount, string totalSize)
		{
			return new LocalizedString("ReportInitialSeedingCompleted", "ExF0A482", false, true, MrsStrings.ResourceManager, new object[]
			{
				messageCount,
				totalSize
			});
		}

		public static LocalizedString MoveRequestTypeCrossOrg
		{
			get
			{
				return new LocalizedString("MoveRequestTypeCrossOrg", "Ex1C329E", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MrsProxyServiceIsDisabled
		{
			get
			{
				return new LocalizedString("MrsProxyServiceIsDisabled", "Ex85E367", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FastTransferBuffer(string property, int value)
		{
			return new LocalizedString("FastTransferBuffer", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString ReportMoveCanceled
		{
			get
			{
				return new LocalizedString("ReportMoveCanceled", "Ex5AA91A", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWlmCapacityExceeded(string resourceName, string resourceKey, double reportedLoadRatio, string reportedLoadState, string metric)
		{
			return new LocalizedString("ErrorWlmCapacityExceeded", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceKey,
				reportedLoadRatio,
				reportedLoadState,
				metric
			});
		}

		public static LocalizedString RestoringConnectedMailboxError(Guid mailboxGuid)
		{
			return new LocalizedString("RestoringConnectedMailboxError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString ErrorCannotPreventCompletionForOfflineMove
		{
			get
			{
				return new LocalizedString("ErrorCannotPreventCompletionForOfflineMove", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportProgress(string syncStage, int percentComplete)
		{
			return new LocalizedString("ReportProgress", "Ex587B63", false, true, MrsStrings.ResourceManager, new object[]
			{
				syncStage,
				percentComplete
			});
		}

		public static LocalizedString EntryIDsDataContext(string entryIdsStr)
		{
			return new LocalizedString("EntryIDsDataContext", "ExADF226", false, true, MrsStrings.ResourceManager, new object[]
			{
				entryIdsStr
			});
		}

		public static LocalizedString EasTracingId(string emailAddress)
		{
			return new LocalizedString("EasTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString ReportTransientException(string errorType, int retryCount, int maxRetries)
		{
			return new LocalizedString("ReportTransientException", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				retryCount,
				maxRetries
			});
		}

		public static LocalizedString RequestGuidNotUnique(string guid, string type)
		{
			return new LocalizedString("RequestGuidNotUnique", "ExE77A61", false, true, MrsStrings.ResourceManager, new object[]
			{
				guid,
				type
			});
		}

		public static LocalizedString MoveIsStalled(string mailboxId, string mdbId, LocalizedString failureReason, string agentName)
		{
			return new LocalizedString("MoveIsStalled", "ExA334F9", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				mdbId,
				failureReason,
				agentName
			});
		}

		public static LocalizedString ContainerMailboxTracingId(Guid containerGuid, Guid mbxGuid)
		{
			return new LocalizedString("ContainerMailboxTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				containerGuid,
				mbxGuid
			});
		}

		public static LocalizedString RequestIsStalled(LocalizedString agent, string throttledResource)
		{
			return new LocalizedString("RequestIsStalled", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				agent,
				throttledResource
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToTransientErrorDuringOfflineMove(DateTime pickupTime)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToTransientErrorDuringOfflineMove", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString ReportFailedToUpdateUserSD(string errorType, LocalizedString errorMsg)
		{
			return new LocalizedString("ReportFailedToUpdateUserSD", "Ex29FD5A", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg
			});
		}

		public static LocalizedString PrimaryMailboxId(Guid mbxGuid)
		{
			return new LocalizedString("PrimaryMailboxId", "Ex539CB5", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ReportIncrementalSyncContentChangesPaged(LocalizedString physicalMailboxId, int messageChanges, int batch)
		{
			return new LocalizedString("ReportIncrementalSyncContentChangesPaged", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				messageChanges,
				batch
			});
		}

		public static LocalizedString InvalidEndpointAddressError(string serviceURI)
		{
			return new LocalizedString("InvalidEndpointAddressError", "ExFEB6F3", false, true, MrsStrings.ResourceManager, new object[]
			{
				serviceURI
			});
		}

		public static LocalizedString DatacenterMissingHosts(string datacenterName)
		{
			return new LocalizedString("DatacenterMissingHosts", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				datacenterName
			});
		}

		public static LocalizedString ReportSyncStateCleared(Guid requestGuid, string reason)
		{
			return new LocalizedString("ReportSyncStateCleared", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				reason
			});
		}

		public static LocalizedString ReportJobIsStalled(LocalizedString failureMsg, int retryCount, int maxRetries)
		{
			return new LocalizedString("ReportJobIsStalled", "ExFEF504", false, true, MrsStrings.ResourceManager, new object[]
			{
				failureMsg,
				retryCount,
				maxRetries
			});
		}

		public static LocalizedString ReportSourceMailboxConnection(LocalizedString mailboxId, LocalizedString serverInformation, string databaseID)
		{
			return new LocalizedString("ReportSourceMailboxConnection", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				serverInformation,
				databaseID
			});
		}

		public static LocalizedString ReportDestinationMailboxSeedMBICacheSucceeded2(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportDestinationMailboxSeedMBICacheSucceeded2", "Ex8BD61A", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString RecipientNotFound(Guid mailboxGuid)
		{
			return new LocalizedString("RecipientNotFound", "Ex0578D0", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString ReportCleanUpFoldersDestination(string stage)
		{
			return new LocalizedString("ReportCleanUpFoldersDestination", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				stage
			});
		}

		public static LocalizedString UnableToOpenPST2(string filePath, string exceptionMessage)
		{
			return new LocalizedString("UnableToOpenPST2", "Ex951A94", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath,
				exceptionMessage
			});
		}

		public static LocalizedString ValidationFlagsMismatch(string adFlags, string mrFlags)
		{
			return new LocalizedString("ValidationFlagsMismatch", "ExDBA7D4", false, true, MrsStrings.ResourceManager, new object[]
			{
				adFlags,
				mrFlags
			});
		}

		public static LocalizedString ReportRequestAllowedMismatch(string userName)
		{
			return new LocalizedString("ReportRequestAllowedMismatch", "Ex5EEEA7", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString RequestHasBeenPostponedDueToBadHealthOfBackendServers2
		{
			get
			{
				return new LocalizedString("RequestHasBeenPostponedDueToBadHealthOfBackendServers2", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationNoCorrespondingIndexEntries
		{
			get
			{
				return new LocalizedString("ValidationNoCorrespondingIndexEntries", "ExF6BA89", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSyncStateData
		{
			get
			{
				return new LocalizedString("InvalidSyncStateData", "ExD8FD56", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRequestSuspended(string userName)
		{
			return new LocalizedString("ReportRequestSuspended", "Ex4DFB2E", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString PublicFolderMigrationNotSupportedFromCurrentExchange2010Version(int major, int minor, int build, int revision)
		{
			return new LocalizedString("PublicFolderMigrationNotSupportedFromCurrentExchange2010Version", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				major,
				minor,
				build,
				revision
			});
		}

		public static LocalizedString ReportMoveRestartedDueToSourceCorruption
		{
			get
			{
				return new LocalizedString("ReportMoveRestartedDueToSourceCorruption", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestDataIsCorrupt(LocalizedString validationMessage)
		{
			return new LocalizedString("MoveRequestDataIsCorrupt", "ExA3AFCA", false, true, MrsStrings.ResourceManager, new object[]
			{
				validationMessage
			});
		}

		public static LocalizedString SourceMailboxUpdateFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("SourceMailboxUpdateFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReportUnableToPreserveMailboxSignature(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportUnableToPreserveMailboxSignature", "ExB2DCA8", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString UsageText(string processName)
		{
			return new LocalizedString("UsageText", "ExF6869B", false, true, MrsStrings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString ReportLargeAmountOfDataLossAccepted(string badItemLimit, string requestorName)
		{
			return new LocalizedString("ReportLargeAmountOfDataLossAccepted", "ExC02752", false, true, MrsStrings.ResourceManager, new object[]
			{
				badItemLimit,
				requestorName
			});
		}

		public static LocalizedString JobHasBeenAutoSuspended
		{
			get
			{
				return new LocalizedString("JobHasBeenAutoSuspended", "ExB8F79F", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndpointNotFoundError(string serviceURI, LocalizedString exceptionMessage)
		{
			return new LocalizedString("EndpointNotFoundError", "ExB791EB", false, true, MrsStrings.ResourceManager, new object[]
			{
				serviceURI,
				exceptionMessage
			});
		}

		public static LocalizedString MailboxNotSynced(Guid mbxGuid)
		{
			return new LocalizedString("MailboxNotSynced", "Ex45D1FE", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ReportSyncStateWrongRequestGuid(Guid requestGuid, Guid returnedGuid)
		{
			return new LocalizedString("ReportSyncStateWrongRequestGuid", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				returnedGuid
			});
		}

		public static LocalizedString ReportDestinationMailboxConnection(LocalizedString mailboxId, LocalizedString serverInformation, string databaseID)
		{
			return new LocalizedString("ReportDestinationMailboxConnection", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				serverInformation,
				databaseID
			});
		}

		public static LocalizedString ReportIncrementalSyncContentChangesSynced2(LocalizedString physicalMailboxId, int newMessages, int changedMessages, int deletedMessages, int readMessages, int unreadMessages, int skipped, int applied)
		{
			return new LocalizedString("ReportIncrementalSyncContentChangesSynced2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				newMessages,
				changedMessages,
				deletedMessages,
				readMessages,
				unreadMessages,
				skipped,
				applied
			});
		}

		public static LocalizedString EasFolderSyncFailedPermanently(string folderSyncStatus, string httpStatus)
		{
			return new LocalizedString("EasFolderSyncFailedPermanently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderSyncStatus,
				httpStatus
			});
		}

		public static LocalizedString ReportFailedToDisconnectFromDestination2(string errorType)
		{
			return new LocalizedString("ReportFailedToDisconnectFromDestination2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString NamedPropsDataContext(string namedPropsStr)
		{
			return new LocalizedString("NamedPropsDataContext", "ExECA2F9", false, true, MrsStrings.ResourceManager, new object[]
			{
				namedPropsStr
			});
		}

		public static LocalizedString InputDataIsInvalid
		{
			get
			{
				return new LocalizedString("InputDataIsInvalid", "ExA48100", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFailingInvalidMoveRequest(LocalizedString validationMessage)
		{
			return new LocalizedString("ReportFailingInvalidMoveRequest", "Ex505BD9", false, true, MrsStrings.ResourceManager, new object[]
			{
				validationMessage
			});
		}

		public static LocalizedString BadItemCorruptMailboxSetting(string typeStr)
		{
			return new LocalizedString("BadItemCorruptMailboxSetting", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				typeStr
			});
		}

		public static LocalizedString ReportJobExitedStalledState
		{
			get
			{
				return new LocalizedString("ReportJobExitedStalledState", "ExA919EC", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActionNotSupported
		{
			get
			{
				return new LocalizedString("ActionNotSupported", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportTargetAuxFolderContentMailboxGuidUpdated
		{
			get
			{
				return new LocalizedString("ReportTargetAuxFolderContentMailboxGuidUpdated", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportStoreMailboxHasFinalized
		{
			get
			{
				return new LocalizedString("ReportStoreMailboxHasFinalized", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxAfterFinalization(string userDataXML)
		{
			return new LocalizedString("ReportMailboxAfterFinalization", "Ex38D684", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString MessageEnumerationFailed(int exists, int messagesEnumeratedCount)
		{
			return new LocalizedString("MessageEnumerationFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				exists,
				messagesEnumeratedCount
			});
		}

		public static LocalizedString ErrorTargetDeliveryDomainMismatch(string targetDeliveryDomain)
		{
			return new LocalizedString("ErrorTargetDeliveryDomainMismatch", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				targetDeliveryDomain
			});
		}

		public static LocalizedString SortOrderDataContext(string sortOrderStr)
		{
			return new LocalizedString("SortOrderDataContext", "ExCA4609", false, true, MrsStrings.ResourceManager, new object[]
			{
				sortOrderStr
			});
		}

		public static LocalizedString CannotCreateEntryId(string input)
		{
			return new LocalizedString("CannotCreateEntryId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				input
			});
		}

		public static LocalizedString ErrorReservationExpired
		{
			get
			{
				return new LocalizedString("ErrorReservationExpired", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyMismatch(uint propTag, string value1, string value2)
		{
			return new LocalizedString("PropertyMismatch", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				propTag,
				value1,
				value2
			});
		}

		public static LocalizedString ReportWaitingIsInteg(Guid mailboxGuid, Guid isIntegRequestGuid, string percentages)
		{
			return new LocalizedString("ReportWaitingIsInteg", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				isIntegRequestGuid,
				percentages
			});
		}

		public static LocalizedString ErrorImplicitSplit
		{
			get
			{
				return new LocalizedString("ErrorImplicitSplit", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SoftDeletedItemsCountAndSize(int items, string size)
		{
			return new LocalizedString("SoftDeletedItemsCountAndSize", "Ex4CAE5B", false, true, MrsStrings.ResourceManager, new object[]
			{
				items,
				size
			});
		}

		public static LocalizedString EasSyncCouldNotFindFolder(string folderId)
		{
			return new LocalizedString("EasSyncCouldNotFindFolder", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString ReportRelinquishingJob
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJob", "Ex84F9BD", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasFolderCreateFailed(string errorMessage)
		{
			return new LocalizedString("EasFolderCreateFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString MailPublicFolderWithLegacyExchangeDnNotFound(string legacyExchangeDN)
		{
			return new LocalizedString("MailPublicFolderWithLegacyExchangeDnNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				legacyExchangeDN
			});
		}

		public static LocalizedString CouldNotConnectToSourceMailbox
		{
			get
			{
				return new LocalizedString("CouldNotConnectToSourceMailbox", "Ex05C4C9", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFolderHierarchyInitialized(LocalizedString physicalMailboxId, int foldersCreated)
		{
			return new LocalizedString("ReportFolderHierarchyInitialized", "Ex05DD9F", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				foldersCreated
			});
		}

		public static LocalizedString ReportRequestContinued(string syncStage)
		{
			return new LocalizedString("ReportRequestContinued", "Ex0E7078", false, true, MrsStrings.ResourceManager, new object[]
			{
				syncStage
			});
		}

		public static LocalizedString NoFoldersIncluded
		{
			get
			{
				return new LocalizedString("NoFoldersIncluded", "Ex16C9A0", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFolderMergeStats(int itemsToCopyCount, string itemsToCopySizeStr, int skippedItemCount, string skippedItemSizeStr)
		{
			return new LocalizedString("ReportFolderMergeStats", "ExD3C1F2", false, true, MrsStrings.ResourceManager, new object[]
			{
				itemsToCopyCount,
				itemsToCopySizeStr,
				skippedItemCount,
				skippedItemSizeStr
			});
		}

		public static LocalizedString ReportSuspendingJob
		{
			get
			{
				return new LocalizedString("ReportSuspendingJob", "Ex1BE920", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WriteRpc
		{
			get
			{
				return new LocalizedString("WriteRpc", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCorruptItemsSkipped(int count, string totalSize)
		{
			return new LocalizedString("ReportCorruptItemsSkipped", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				count,
				totalSize
			});
		}

		public static LocalizedString ValidationUserLacksMailbox(string jobUser)
		{
			return new LocalizedString("ValidationUserLacksMailbox", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				jobUser
			});
		}

		public static LocalizedString NotConnected
		{
			get
			{
				return new LocalizedString("NotConnected", "ExA6F98B", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasSyncFailed(string errorMessage)
		{
			return new LocalizedString("EasSyncFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReportSoftDeletedItemsWillNotBeMigrated(LocalizedString mailboxId, int itemCount, string itemsSize)
		{
			return new LocalizedString("ReportSoftDeletedItemsWillNotBeMigrated", "ExAAC01F", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				itemCount,
				itemsSize
			});
		}

		public static LocalizedString FolderDataContextId(string entryId)
		{
			return new LocalizedString("FolderDataContextId", "Ex280BF7", false, true, MrsStrings.ResourceManager, new object[]
			{
				entryId
			});
		}

		public static LocalizedString ReportIcsSyncStateNull(Guid requestGuid)
		{
			return new LocalizedString("ReportIcsSyncStateNull", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid
			});
		}

		public static LocalizedString ReportFatalExceptionOccurred(string errorType, LocalizedString errorMsg, string trace, LocalizedString context)
		{
			return new LocalizedString("ReportFatalExceptionOccurred", "Ex0B0B72", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace,
				context
			});
		}

		public static LocalizedString MdbReplication
		{
			get
			{
				return new LocalizedString("MdbReplication", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationMailboxIdentitiesDontMatch(string adUserId, string mrUserId)
		{
			return new LocalizedString("ValidationMailboxIdentitiesDontMatch", "Ex03029B", false, true, MrsStrings.ResourceManager, new object[]
			{
				adUserId,
				mrUserId
			});
		}

		public static LocalizedString ReportMailboxArchiveInfoAfterMove(string mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxArchiveInfoAfterMove", "ExFCC1B7", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString FolderHierarchyContainsMultipleRoots(string root1str, string root2str)
		{
			return new LocalizedString("FolderHierarchyContainsMultipleRoots", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				root1str,
				root2str
			});
		}

		public static LocalizedString ValidationStorageMDBMismatch(string indexDatabase, string jobDatabase)
		{
			return new LocalizedString("ValidationStorageMDBMismatch", "Ex8FD9D3", false, true, MrsStrings.ResourceManager, new object[]
			{
				indexDatabase,
				jobDatabase
			});
		}

		public static LocalizedString ReportFatalException(string errorType)
		{
			return new LocalizedString("ReportFatalException", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString ReportRulesWillNotBeCopied
		{
			get
			{
				return new LocalizedString("ReportRulesWillNotBeCopied", "ExFB7131", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxInfoAfterMoveLoc(LocalizedString mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxInfoAfterMoveLoc", "ExCEAE7D", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString ReportSkippingUpdateSourceMailbox
		{
			get
			{
				return new LocalizedString("ReportSkippingUpdateSourceMailbox", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFetchMimeStream(string identity)
		{
			return new LocalizedString("UnableToFetchMimeStream", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString ReportUnableToUpdateSourceMailbox(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportUnableToUpdateSourceMailbox", "Ex1D7F37", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString ErrorNoCASServersInSite(string site, string minVersion)
		{
			return new LocalizedString("ErrorNoCASServersInSite", "Ex4D34FC", false, true, MrsStrings.ResourceManager, new object[]
			{
				site,
				minVersion
			});
		}

		public static LocalizedString ReportSoftDeletedItemsNotMigrated(int itemCount, string itemsSize)
		{
			return new LocalizedString("ReportSoftDeletedItemsNotMigrated", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				itemCount,
				itemsSize
			});
		}

		public static LocalizedString ErrorEmptyMailboxGuid
		{
			get
			{
				return new LocalizedString("ErrorEmptyMailboxGuid", "Ex3953C5", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasMoveFailedError(string moveStatus, string httpStatus, string sourcrFolderId, string destFolderId)
		{
			return new LocalizedString("EasMoveFailedError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				moveStatus,
				httpStatus,
				sourcrFolderId,
				destFolderId
			});
		}

		public static LocalizedString ReportArchiveAlreadyUpdated
		{
			get
			{
				return new LocalizedString("ReportArchiveAlreadyUpdated", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportReplayActionsEnumerated(LocalizedString physicalMailboxId, int numberOfActions, int batch)
		{
			return new LocalizedString("ReportReplayActionsEnumerated", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				numberOfActions,
				batch
			});
		}

		public static LocalizedString JobHasBeenSynced
		{
			get
			{
				return new LocalizedString("JobHasBeenSynced", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobHasBeenRelinquishedDueToLongRun
		{
			get
			{
				return new LocalizedString("JobHasBeenRelinquishedDueToLongRun", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationTargetMDBMismatch(string adDatabase, string mrDatabase)
		{
			return new LocalizedString("ValidationTargetMDBMismatch", "ExAE91DE", false, true, MrsStrings.ResourceManager, new object[]
			{
				adDatabase,
				mrDatabase
			});
		}

		public static LocalizedString ReportRelinquishingJobDueToHAOrCIStalling
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToHAOrCIStalling", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasMoveFailed(string errorMessage)
		{
			return new LocalizedString("EasMoveFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReportRequestSaveFailed2(string errorType)
		{
			return new LocalizedString("ReportRequestSaveFailed2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorType
			});
		}

		public static LocalizedString IsExcludedFromProvisioningError(Guid mdbName)
		{
			return new LocalizedString("IsExcludedFromProvisioningError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mdbName
			});
		}

		public static LocalizedString ClusterIPMissingHosts(IPAddress clusterIp)
		{
			return new LocalizedString("ClusterIPMissingHosts", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				clusterIp
			});
		}

		public static LocalizedString MoveCompleteFailedForAlreadyCanceledMove(Guid mbxGuid)
		{
			return new LocalizedString("MoveCompleteFailedForAlreadyCanceledMove", "Ex45C692", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString WindowsLiveIDAddressIsMissing(string user)
		{
			return new LocalizedString("WindowsLiveIDAddressIsMissing", "Ex3FC517", false, true, MrsStrings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ReportStartedIsInteg(Guid mailboxGuid, Guid isIntegRequestGuid)
		{
			return new LocalizedString("ReportStartedIsInteg", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				isIntegRequestGuid
			});
		}

		public static LocalizedString ContentFilterIsInvalid(string msg)
		{
			return new LocalizedString("ContentFilterIsInvalid", "Ex5E2C36", false, true, MrsStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString Mailbox
		{
			get
			{
				return new LocalizedString("Mailbox", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCompletedIsInteg(Guid mailboxGuid, Guid isIntegRequestGuid)
		{
			return new LocalizedString("ReportCompletedIsInteg", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				isIntegRequestGuid
			});
		}

		public static LocalizedString BadItemSearchFolder(string folderName)
		{
			return new LocalizedString("BadItemSearchFolder", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString UnableToGetPSTReceiveFolder(string filePath, string messageClass)
		{
			return new LocalizedString("UnableToGetPSTReceiveFolder", "Ex58C864", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath,
				messageClass
			});
		}

		public static LocalizedString FolderHierarchyIsInconsistentTemporarily
		{
			get
			{
				return new LocalizedString("FolderHierarchyIsInconsistentTemporarily", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadItemLarge(string msgClass, string subject, string itemSize, string folderName)
		{
			return new LocalizedString("BadItemLarge", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				msgClass,
				subject,
				itemSize,
				folderName
			});
		}

		public static LocalizedString BadItemFolderPropertyMismatch(string folderName, string error)
		{
			return new LocalizedString("BadItemFolderPropertyMismatch", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName,
				error
			});
		}

		public static LocalizedString AuxFolderMoveTracingId(string orgID, Guid mbxGuid)
		{
			return new LocalizedString("AuxFolderMoveTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				orgID,
				mbxGuid
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToDataGuaranteeTimeout(DateTime pickupTime)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToDataGuaranteeTimeout", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				pickupTime
			});
		}

		public static LocalizedString RequestPriorityEmergency
		{
			get
			{
				return new LocalizedString("RequestPriorityEmergency", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EasSendFailed(string errorMessage)
		{
			return new LocalizedString("EasSendFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReportRelinquishingJobDueToHAStall
		{
			get
			{
				return new LocalizedString("ReportRelinquishingJobDueToHAStall", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRegKeyNotExist(string keyPath)
		{
			return new LocalizedString("ErrorRegKeyNotExist", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				keyPath
			});
		}

		public static LocalizedString CorruptSyncState
		{
			get
			{
				return new LocalizedString("CorruptSyncState", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportTooManyTransientFailures(int totalRetryCount)
		{
			return new LocalizedString("ReportTooManyTransientFailures", "ExB4C8AF", false, true, MrsStrings.ResourceManager, new object[]
			{
				totalRetryCount
			});
		}

		public static LocalizedString RestrictionDataContext(string restrictionStr)
		{
			return new LocalizedString("RestrictionDataContext", "Ex75BA5A", false, true, MrsStrings.ResourceManager, new object[]
			{
				restrictionStr
			});
		}

		public static LocalizedString ServerNotFoundByGuid(Guid serverGuid)
		{
			return new LocalizedString("ServerNotFoundByGuid", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				serverGuid
			});
		}

		public static LocalizedString MoveRequestMessageWarning(LocalizedString message)
		{
			return new LocalizedString("MoveRequestMessageWarning", "Ex92F5D6", false, true, MrsStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString MRSInstanceFailed(string mrsInstance, LocalizedString exceptionMessage)
		{
			return new LocalizedString("MRSInstanceFailed", "Ex23B90F", false, true, MrsStrings.ResourceManager, new object[]
			{
				mrsInstance,
				exceptionMessage
			});
		}

		public static LocalizedString ReportTargetMailboxAfterFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportTargetMailboxAfterFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString EasCountFailed(string errorMessage)
		{
			return new LocalizedString("EasCountFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString FolderReferencedMoreThanOnce(string folderPath)
		{
			return new LocalizedString("FolderReferencedMoreThanOnce", "Ex37DDC0", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderPath
			});
		}

		public static LocalizedString RecipientAggregatedMailboxNotFound(string recipient, string recipientAggregatedMailboxGuidsAsString, Guid targetAggregatedMailboxGuid)
		{
			return new LocalizedString("RecipientAggregatedMailboxNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				recipient,
				recipientAggregatedMailboxGuidsAsString,
				targetAggregatedMailboxGuid
			});
		}

		public static LocalizedString PostSaveActionFailed(LocalizedString error)
		{
			return new LocalizedString("PostSaveActionFailed", "ExEEA65D", false, true, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ReportReplayActionsSynced(LocalizedString physicalMailboxId, int numberOfActionsReplayed, int numberOfActionsIgnored)
		{
			return new LocalizedString("ReportReplayActionsSynced", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				numberOfActionsReplayed,
				numberOfActionsIgnored
			});
		}

		public static LocalizedString ReportTargetPublicFolderContentMailboxGuidUpdated
		{
			get
			{
				return new LocalizedString("ReportTargetPublicFolderContentMailboxGuidUpdated", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderPathIsInvalid(string folderPath)
		{
			return new LocalizedString("FolderPathIsInvalid", "Ex7903F7", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderPath
			});
		}

		public static LocalizedString RestoreMailboxTracingId(string dbName, Guid mailboxGuid)
		{
			return new LocalizedString("RestoreMailboxTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				dbName,
				mailboxGuid
			});
		}

		public static LocalizedString ReportMoveRequestRemoved(string userName)
		{
			return new LocalizedString("ReportMoveRequestRemoved", "Ex36440E", false, true, MrsStrings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToServerBusy(LocalizedString error, TimeSpan backoffTimeSpan)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToServerBusy", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error,
				backoffTimeSpan
			});
		}

		public static LocalizedString PickupStatusSubTypeNotSupported(string requestType)
		{
			return new LocalizedString("PickupStatusSubTypeNotSupported", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestType
			});
		}

		public static LocalizedString NoMRSAvailable
		{
			get
			{
				return new LocalizedString("NoMRSAvailable", "ExA4CEA5", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PickupStatusReservationFailure(LocalizedString exceptionMessage)
		{
			return new LocalizedString("PickupStatusReservationFailure", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString ReportIncrementalSyncHierarchyChanges(LocalizedString physicalMailboxId, int changedFolders, int deletedFolders)
		{
			return new LocalizedString("ReportIncrementalSyncHierarchyChanges", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				changedFolders,
				deletedFolders
			});
		}

		public static LocalizedString RequestPriorityLower
		{
			get
			{
				return new LocalizedString("RequestPriorityLower", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidHandleType(long handle, string handleType, string expectedType)
		{
			return new LocalizedString("InvalidHandleType", "Ex3B9BC5", false, true, MrsStrings.ResourceManager, new object[]
			{
				handle,
				handleType,
				expectedType
			});
		}

		public static LocalizedString ProviderAlreadySpecificToDatabase(Guid oldMdbGuid, Guid newMdbGuid)
		{
			return new LocalizedString("ProviderAlreadySpecificToDatabase", "Ex5EF850", false, true, MrsStrings.ResourceManager, new object[]
			{
				oldMdbGuid,
				newMdbGuid
			});
		}

		public static LocalizedString ReportSourceMailboxCleanupFailed2(LocalizedString mailboxId, string errorType, LocalizedString errorMsg, string trace, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportSourceMailboxCleanupFailed2", "ExB72C5D", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId,
				errorType,
				errorMsg,
				trace,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString ReportReplayActionsCompleted(LocalizedString physicalMailboxId, int numberOfActionsReplayed, int numberOfActionsIgnored)
		{
			return new LocalizedString("ReportReplayActionsCompleted", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				numberOfActionsReplayed,
				numberOfActionsIgnored
			});
		}

		public static LocalizedString ReportTargetMailUserBeforeFinalization(string userDataXML)
		{
			return new LocalizedString("ReportTargetMailUserBeforeFinalization", "Ex0F3B97", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString MailPublicFolderWithObjectIdNotFound(Guid objectId)
		{
			return new LocalizedString("MailPublicFolderWithObjectIdNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				objectId
			});
		}

		public static LocalizedString ReportIncrementalSyncCompleted(LocalizedString physicalMailboxId, int numberOfUpdates)
		{
			return new LocalizedString("ReportIncrementalSyncCompleted", "ExD7C287", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				numberOfUpdates
			});
		}

		public static LocalizedString PickupStatusProxyBackoff(string remoteHostName)
		{
			return new LocalizedString("PickupStatusProxyBackoff", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				remoteHostName
			});
		}

		public static LocalizedString MailPublicFolderWithMultipleEntriesFound(string legacyExchangeDN)
		{
			return new LocalizedString("MailPublicFolderWithMultipleEntriesFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				legacyExchangeDN
			});
		}

		public static LocalizedString ReportAutoSuspendingJob
		{
			get
			{
				return new LocalizedString("ReportAutoSuspendingJob", "Ex15DBB7", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveRequestMessageInformational(LocalizedString message)
		{
			return new LocalizedString("MoveRequestMessageInformational", "Ex6C0B53", false, true, MrsStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ReportJobIsStalledWithFailure(LocalizedString failureMsg, DateTime willWaitUntilTimestamp)
		{
			return new LocalizedString("ReportJobIsStalledWithFailure", "Ex821B3B", false, true, MrsStrings.ResourceManager, new object[]
			{
				failureMsg,
				willWaitUntilTimestamp
			});
		}

		public static LocalizedString ReportIncrementalSyncContentChanges2(LocalizedString physicalMailboxId, int newMessages, int changedMessages, int deletedMessages, int readMessages, int unreadMessages, int total)
		{
			return new LocalizedString("ReportIncrementalSyncContentChanges2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				newMessages,
				changedMessages,
				deletedMessages,
				readMessages,
				unreadMessages,
				total
			});
		}

		public static LocalizedString NotSupportedCodePageError(int codePage, string server)
		{
			return new LocalizedString("NotSupportedCodePageError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				codePage,
				server
			});
		}

		public static LocalizedString RecipientInvalidLegDN(string recipient, string legacyExchangeDN)
		{
			return new LocalizedString("RecipientInvalidLegDN", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				recipient,
				legacyExchangeDN
			});
		}

		public static LocalizedString ReportMailboxContentsVerificationComplete(int folderCount, int itemCount, string itemSizeStr)
		{
			return new LocalizedString("ReportMailboxContentsVerificationComplete", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderCount,
				itemCount,
				itemSizeStr
			});
		}

		public static LocalizedString ValidationOrganizationMismatch(string jobOrg, string indexOrg)
		{
			return new LocalizedString("ValidationOrganizationMismatch", "ExC6F293", false, true, MrsStrings.ResourceManager, new object[]
			{
				jobOrg,
				indexOrg
			});
		}

		public static LocalizedString UnableToStreamPSTProp(uint propTag, int offset, int bytesToRead, long length)
		{
			return new LocalizedString("UnableToStreamPSTProp", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				propTag,
				offset,
				bytesToRead,
				length
			});
		}

		public static LocalizedString UnsupportedClientVersionWithOperation(string clientName, string clientVersion, string operationName)
		{
			return new LocalizedString("UnsupportedClientVersionWithOperation", "Ex3032A7", false, true, MrsStrings.ResourceManager, new object[]
			{
				clientName,
				clientVersion,
				operationName
			});
		}

		public static LocalizedString ReportCalendarFolderFaiSaveFailed
		{
			get
			{
				return new LocalizedString("ReportCalendarFolderFaiSaveFailed", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveIsPreventedFromFinalization
		{
			get
			{
				return new LocalizedString("MoveIsPreventedFromFinalization", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMoveAlreadyFinished
		{
			get
			{
				return new LocalizedString("ReportMoveAlreadyFinished", "ExF76633", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RehomeRequestFailure
		{
			get
			{
				return new LocalizedString("RehomeRequestFailure", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportJobRehomed(string sourceMDB, string targetMDB)
		{
			return new LocalizedString("ReportJobRehomed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				sourceMDB,
				targetMDB
			});
		}

		public static LocalizedString ReportSourceMailboxCleanupSkipped(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportSourceMailboxCleanupSkipped", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString RequestIsStalledByHigherPriorityJobs
		{
			get
			{
				return new LocalizedString("RequestIsStalledByHigherPriorityJobs", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkloadTypeEmergency
		{
			get
			{
				return new LocalizedString("WorkloadTypeEmergency", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportFailedToDisconnectFromDestination(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportFailedToDisconnectFromDestination", "Ex37060D", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString ReportCalendarFolderSaveFailed
		{
			get
			{
				return new LocalizedString("ReportCalendarFolderSaveFailed", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IsIntegTooLongError(DateTime firstRepairAttemptedAt)
		{
			return new LocalizedString("IsIntegTooLongError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				firstRepairAttemptedAt
			});
		}

		public static LocalizedString ReportIncrementalSyncChanges(LocalizedString physicalMailboxId, int changedFolders, int deletedFolders, int messageChanges)
		{
			return new LocalizedString("ReportIncrementalSyncChanges", "Ex997352", false, true, MrsStrings.ResourceManager, new object[]
			{
				physicalMailboxId,
				changedFolders,
				deletedFolders,
				messageChanges
			});
		}

		public static LocalizedString UnableToCreatePSTMessage(string filePath)
		{
			return new LocalizedString("UnableToCreatePSTMessage", "Ex9E4BEA", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString ErrorStaticCapacityExceeded(string resourceName, int capacity)
		{
			return new LocalizedString("ErrorStaticCapacityExceeded", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				capacity
			});
		}

		public static LocalizedString FolderAlreadyExists(string name)
		{
			return new LocalizedString("FolderAlreadyExists", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ReportMoveResumed(string status)
		{
			return new LocalizedString("ReportMoveResumed", "Ex64D6A5", false, true, MrsStrings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString ReportSoftDeletedItemCountsAndSizes(string softDeletedItems)
		{
			return new LocalizedString("ReportSoftDeletedItemCountsAndSizes", "Ex9D9096", false, true, MrsStrings.ResourceManager, new object[]
			{
				softDeletedItems
			});
		}

		public static LocalizedString MissingDatabaseName(Guid dbGuid)
		{
			return new LocalizedString("MissingDatabaseName", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString ReportUpdateMovedMailboxError(LocalizedString error)
		{
			return new LocalizedString("ReportUpdateMovedMailboxError", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString UnableToVerifyMailboxConnectivity(LocalizedString mailboxId)
		{
			return new LocalizedString("UnableToVerifyMailboxConnectivity", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString BadItemCorrupt(string msgClass, string subject, string folderName)
		{
			return new LocalizedString("BadItemCorrupt", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				msgClass,
				subject,
				folderName
			});
		}

		public static LocalizedString MRSProxyConnectionNotThrottledError
		{
			get
			{
				return new LocalizedString("MRSProxyConnectionNotThrottledError", "ExD69EFE", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxSignatureIsNotPreserved(LocalizedString mailboxId)
		{
			return new LocalizedString("ReportMailboxSignatureIsNotPreserved", "Ex28305C", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ReportWaitingForMailboxDataReplication
		{
			get
			{
				return new LocalizedString("ReportWaitingForMailboxDataReplication", "Ex806433", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportProxyConnectionLimitMet(DateTime pickUpTime)
		{
			return new LocalizedString("ReportProxyConnectionLimitMet", "Ex1D0478", false, true, MrsStrings.ResourceManager, new object[]
			{
				pickUpTime
			});
		}

		public static LocalizedString FolderAlreadyInTarget(string folderId)
		{
			return new LocalizedString("FolderAlreadyInTarget", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString ReportTargetFolderDeleted(string targetFolderName, string targetFolderEntryIdStr, string sourceFolderName)
		{
			return new LocalizedString("ReportTargetFolderDeleted", "Ex01A8A8", false, true, MrsStrings.ResourceManager, new object[]
			{
				targetFolderName,
				targetFolderEntryIdStr,
				sourceFolderName
			});
		}

		public static LocalizedString FolderCopyFailed(string folderName)
		{
			return new LocalizedString("FolderCopyFailed", "Ex73C57C", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString ReportDatabaseFailedOver
		{
			get
			{
				return new LocalizedString("ReportDatabaseFailedOver", "Ex5E3943", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStaticCapacityExceeded1(string resourceName, string resourceType, int capacity)
		{
			return new LocalizedString("ErrorStaticCapacityExceeded1", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				resourceType,
				capacity
			});
		}

		public static LocalizedString ReportWaitingForAdReplication(int delaySecs, int iAttempts, int iMaxRetries)
		{
			return new LocalizedString("ReportWaitingForAdReplication", "ExAD7B08", false, true, MrsStrings.ResourceManager, new object[]
			{
				delaySecs,
				iAttempts,
				iMaxRetries
			});
		}

		public static LocalizedString FolderIsMissingInMerge
		{
			get
			{
				return new LocalizedString("FolderIsMissingInMerge", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxServerInformation(string serverName, string serverVersion)
		{
			return new LocalizedString("MailboxServerInformation", "Ex0E4F93", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion
			});
		}

		public static LocalizedString ReportTargetMailUserBeforeFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportTargetMailUserBeforeFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString MoveHasBeenAutoSuspended(Guid mbxGuid)
		{
			return new LocalizedString("MoveHasBeenAutoSuspended", "ExBFE36D", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString Unknown
		{
			get
			{
				return new LocalizedString("Unknown", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRelinquishingJobDueToFailover(string serverName)
		{
			return new LocalizedString("ReportRelinquishingJobDueToFailover", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString OrphanedDatabaseName(LocalizedString dbName)
		{
			return new LocalizedString("OrphanedDatabaseName", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString ReportTargetMailboxAfterFinalization(string userDataXML)
		{
			return new LocalizedString("ReportTargetMailboxAfterFinalization", "ExB9CEEC", false, true, MrsStrings.ResourceManager, new object[]
			{
				userDataXML
			});
		}

		public static LocalizedString RecipientPropertyIsNotWriteable(string recipient, string propertyName)
		{
			return new LocalizedString("RecipientPropertyIsNotWriteable", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				recipient,
				propertyName
			});
		}

		public static LocalizedString WorkloadTypeSyncAggregation
		{
			get
			{
				return new LocalizedString("WorkloadTypeSyncAggregation", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyTagsDoNotMatch(uint propTagFromSource, uint propTagFromDestination)
		{
			return new LocalizedString("PropertyTagsDoNotMatch", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				propTagFromSource,
				propTagFromDestination
			});
		}

		public static LocalizedString PrimaryMailboxTracingId(string orgID, Guid mbxGuid)
		{
			return new LocalizedString("PrimaryMailboxTracingId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				orgID,
				mbxGuid
			});
		}

		public static LocalizedString ArchiveMailboxId(Guid mbxGuid)
		{
			return new LocalizedString("ArchiveMailboxId", "Ex9FA974", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ReportTargetUserIsNotMailEnabledUser
		{
			get
			{
				return new LocalizedString("ReportTargetUserIsNotMailEnabledUser", "Ex7AA32B", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImapObjectNotFound(string entryId)
		{
			return new LocalizedString("ImapObjectNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				entryId
			});
		}

		public static LocalizedString FolderReferencedAsBothIncludedAndExcluded(string folderPath)
		{
			return new LocalizedString("FolderReferencedAsBothIncludedAndExcluded", "ExEB6169", false, true, MrsStrings.ResourceManager, new object[]
			{
				folderPath
			});
		}

		public static LocalizedString ReportRequestIsNoLongerSticky
		{
			get
			{
				return new LocalizedString("ReportRequestIsNoLongerSticky", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DestinationMailboxSyncStateDeletionFailed(LocalizedString errorMsg)
		{
			return new LocalizedString("DestinationMailboxSyncStateDeletionFailed", "ExC3111B", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString MoveRequestTypeIntraOrg
		{
			get
			{
				return new LocalizedString("MoveRequestTypeIntraOrg", "ExA16067", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderHierarchyContainsDuplicates(string folder1str, string folder2str)
		{
			return new LocalizedString("FolderHierarchyContainsDuplicates", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folder1str,
				folder2str
			});
		}

		public static LocalizedString ReportSourceFolderDeleted(string sourceFolderName, string sourceFolderEntryIdStr)
		{
			return new LocalizedString("ReportSourceFolderDeleted", "Ex8A2D15", false, true, MrsStrings.ResourceManager, new object[]
			{
				sourceFolderName,
				sourceFolderEntryIdStr
			});
		}

		public static LocalizedString ValidationMoveRequestNotDeserialized
		{
			get
			{
				return new LocalizedString("ValidationMoveRequestNotDeserialized", "Ex6468E8", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSoftDeletedItemCountsAndSizesLoc(LocalizedString softDeletedItems)
		{
			return new LocalizedString("ReportSoftDeletedItemCountsAndSizesLoc", "ExF1A8E0", false, true, MrsStrings.ResourceManager, new object[]
			{
				softDeletedItems
			});
		}

		public static LocalizedString EasSyncFailedTransiently(string syncStatus, string httpStatus, string folderId)
		{
			return new LocalizedString("EasSyncFailedTransiently", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				syncStatus,
				httpStatus,
				folderId
			});
		}

		public static LocalizedString ReportMoveStarted
		{
			get
			{
				return new LocalizedString("ReportMoveStarted", "ExD6C9E1", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportJobResumed(string status)
		{
			return new LocalizedString("ReportJobResumed", "Ex33921C", false, true, MrsStrings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString PickupStatusJobTypeNotSupported(string jobType)
		{
			return new LocalizedString("PickupStatusJobTypeNotSupported", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				jobType
			});
		}

		public static LocalizedString ValidationMailboxGuidsDontMatch(Guid userMailboxGuid, Guid mrExchangeGuid)
		{
			return new LocalizedString("ValidationMailboxGuidsDontMatch", "ExBA892D", false, true, MrsStrings.ResourceManager, new object[]
			{
				userMailboxGuid,
				mrExchangeGuid
			});
		}

		public static LocalizedString ReportReplaySyncStateCorrupt(Guid requestGuid, int length, string start, string end)
		{
			return new LocalizedString("ReportReplaySyncStateCorrupt", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				length,
				start,
				end
			});
		}

		public static LocalizedString ReportPostMoveCleanupStarted
		{
			get
			{
				return new LocalizedString("ReportPostMoveCleanupStarted", "Ex047582", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalAccessFolderCreationIsNotSupported
		{
			get
			{
				return new LocalizedString("InternalAccessFolderCreationIsNotSupported", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportRequestCompleted
		{
			get
			{
				return new LocalizedString("ReportRequestCompleted", "ExDABAF1", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToReadADUser(string userId)
		{
			return new LocalizedString("UnableToReadADUser", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString UnsupportedRemoteServerVersionWithOperation(string remoteServerAddress, string serverVersion, string operationName)
		{
			return new LocalizedString("UnsupportedRemoteServerVersionWithOperation", "Ex26CC4A", false, true, MrsStrings.ResourceManager, new object[]
			{
				remoteServerAddress,
				serverVersion,
				operationName
			});
		}

		public static LocalizedString JobHasBeenRelinquishedDueToResourceReservation(LocalizedString error)
		{
			return new LocalizedString("JobHasBeenRelinquishedDueToResourceReservation", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ReportUnableToComputeTargetAddress(string targetDeliveryDomain, string primarySmtpAddress)
		{
			return new LocalizedString("ReportUnableToComputeTargetAddress", "Ex50BD17", false, true, MrsStrings.ResourceManager, new object[]
			{
				targetDeliveryDomain,
				primarySmtpAddress
			});
		}

		public static LocalizedString ReportSourceMailUserAfterFinalization2(string userID, string domainControllerName)
		{
			return new LocalizedString("ReportSourceMailUserAfterFinalization2", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				userID,
				domainControllerName
			});
		}

		public static LocalizedString RecipientMissingLegDN(string recipient)
		{
			return new LocalizedString("RecipientMissingLegDN", "Ex259BEB", false, true, MrsStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString ReportIcsSyncStateCorrupt(Guid requestGuid, int length, string start, string end)
		{
			return new LocalizedString("ReportIcsSyncStateCorrupt", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				requestGuid,
				length,
				start,
				end
			});
		}

		public static LocalizedString EasFolderUpdateFailed(string errorMessage)
		{
			return new LocalizedString("EasFolderUpdateFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString UnableToUpdateSourceMailbox(LocalizedString errorMsg)
		{
			return new LocalizedString("UnableToUpdateSourceMailbox", "ExE23BCC", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString DestinationMailboxResetNotGuaranteed(LocalizedString errorMsg)
		{
			return new LocalizedString("DestinationMailboxResetNotGuaranteed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReadCpu
		{
			get
			{
				return new LocalizedString("ReadCpu", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxArchiveInfoAfterMoveLoc(LocalizedString mailboxInfoString)
		{
			return new LocalizedString("ReportMailboxArchiveInfoAfterMoveLoc", "Ex7DFF8C", false, true, MrsStrings.ResourceManager, new object[]
			{
				mailboxInfoString
			});
		}

		public static LocalizedString UnableToClosePST(string filePath)
		{
			return new LocalizedString("UnableToClosePST", "Ex423CAC", false, true, MrsStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString ReportRequestProcessedByAnotherMRS(string mrsName)
		{
			return new LocalizedString("ReportRequestProcessedByAnotherMRS", "Ex2DE801", false, true, MrsStrings.ResourceManager, new object[]
			{
				mrsName
			});
		}

		public static LocalizedString TargetContainer
		{
			get
			{
				return new LocalizedString("TargetContainer", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JobIsStalledAndFailed(LocalizedString failureReason, int agentId)
		{
			return new LocalizedString("JobIsStalledAndFailed", "Ex5DA074", false, true, MrsStrings.ResourceManager, new object[]
			{
				failureReason,
				agentId
			});
		}

		public static LocalizedString ReportSyncStateSaveFailed(string errorType, LocalizedString errorMsg, string trace)
		{
			return new LocalizedString("ReportSyncStateSaveFailed", "Ex2D4B71", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace
			});
		}

		public static LocalizedString RequestPriorityLowest
		{
			get
			{
				return new LocalizedString("RequestPriorityLowest", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMoveInProgress(string resourceName, string clientName)
		{
			return new LocalizedString("ErrorMoveInProgress", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				resourceName,
				clientName
			});
		}

		public static LocalizedString ParsingMessageEntryIdFailed(string messageEntryId)
		{
			return new LocalizedString("ParsingMessageEntryIdFailed", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				messageEntryId
			});
		}

		public static LocalizedString WorkloadTypeNone
		{
			get
			{
				return new LocalizedString("WorkloadTypeNone", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportInitializingJob(string serverName, string serverVersion)
		{
			return new LocalizedString("ReportInitializingJob", "Ex7E8DA9", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion
			});
		}

		public static LocalizedString UnableToObtainServersInLocalSite
		{
			get
			{
				return new LocalizedString("UnableToObtainServersInLocalSite", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongUserObjectFound
		{
			get
			{
				return new LocalizedString("WrongUserObjectFound", "Ex5D823D", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoveHasBeenRelinquishedDueToProxyThrottling(Guid mbxGuid, DateTime pickupTime)
		{
			return new LocalizedString("MoveHasBeenRelinquishedDueToProxyThrottling", "Ex35A6DA", false, true, MrsStrings.ResourceManager, new object[]
			{
				mbxGuid,
				pickupTime
			});
		}

		public static LocalizedString RpcClientAccessServerNotConfiguredForMdb(string mdbID)
		{
			return new LocalizedString("RpcClientAccessServerNotConfiguredForMdb", "Ex5B0815", false, true, MrsStrings.ResourceManager, new object[]
			{
				mdbID
			});
		}

		public static LocalizedString GetIdsFromNamesCalledOnDestination
		{
			get
			{
				return new LocalizedString("GetIdsFromNamesCalledOnDestination", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteMailboxServerInformation(string serverName, string serverVersion, string proxyServerName, string proxyServerVersion)
		{
			return new LocalizedString("RemoteMailboxServerInformation", "Ex18ED52", false, true, MrsStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				proxyServerName,
				proxyServerVersion
			});
		}

		public static LocalizedString MoveReportEntryMessage(string timestamp, string serverName, LocalizedString message)
		{
			return new LocalizedString("MoveReportEntryMessage", "Ex95F02C", false, true, MrsStrings.ResourceManager, new object[]
			{
				timestamp,
				serverName,
				message
			});
		}

		public static LocalizedString ReportDestinationMailboxResetFailed2(string errorType, LocalizedString errorMsg, string trace, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxResetFailed2", "Ex09A66C", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString BadItemFolderProperty(string folderName)
		{
			return new LocalizedString("BadItemFolderProperty", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString EasObjectNotFound(string entryId)
		{
			return new LocalizedString("EasObjectNotFound", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				entryId
			});
		}

		public static LocalizedString MaxSubmissionExceeded(string messageSizeStr, LocalizedString errorMsg)
		{
			return new LocalizedString("MaxSubmissionExceeded", "ExCAE458", false, true, MrsStrings.ResourceManager, new object[]
			{
				messageSizeStr,
				errorMsg
			});
		}

		public static LocalizedString DataExportCanceled
		{
			get
			{
				return new LocalizedString("DataExportCanceled", "Ex22658E", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedImapMessageEntryIdVersion(string version)
		{
			return new LocalizedString("UnsupportedImapMessageEntryIdVersion", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString ExceptionWithStack(LocalizedString exceptionMessage, string stackTrace)
		{
			return new LocalizedString("ExceptionWithStack", "Ex8E18E1", false, true, MrsStrings.ResourceManager, new object[]
			{
				exceptionMessage,
				stackTrace
			});
		}

		public static LocalizedString ReportDestinationMailboxSeedMBICacheFailed(string errorType, LocalizedString errorMsg, string trace, int currentAttempt, int totalAttempts)
		{
			return new LocalizedString("ReportDestinationMailboxSeedMBICacheFailed", "ExF078BD", false, true, MrsStrings.ResourceManager, new object[]
			{
				errorType,
				errorMsg,
				trace,
				currentAttempt,
				totalAttempts
			});
		}

		public static LocalizedString TooManyBadItems
		{
			get
			{
				return new LocalizedString("TooManyBadItems", "ExCBFC85", false, true, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportVerifyingMailboxContents
		{
			get
			{
				return new LocalizedString("ReportVerifyingMailboxContents", "", false, false, MrsStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadItemFolderACL(string folderName)
		{
			return new LocalizedString("BadItemFolderACL", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString CannotCreateMessageId(long uid, string folderName)
		{
			return new LocalizedString("CannotCreateMessageId", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				uid,
				folderName
			});
		}

		public static LocalizedString ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent(LocalizedString mbxId, uint originalVersion, uint currentVersion)
		{
			return new LocalizedString("ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent", "", false, false, MrsStrings.ResourceManager, new object[]
			{
				mbxId,
				originalVersion,
				currentVersion
			});
		}

		public static LocalizedString GetLocalizedString(MrsStrings.IDs key)
		{
			return new LocalizedString(MrsStrings.stringIDs[(uint)key], MrsStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(186);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MailboxReplicationService.Strings", typeof(MrsStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidRequestJob = 1424238063U,
			WorkloadTypeOnboarding = 392888166U,
			ReportMessagesCopied = 3088101088U,
			DestMailboxAlreadyBeingMoved = 3176193428U,
			ReportDestinationSDCannotBeRead = 3671912657U,
			ServiceIsStopping = 3429098023U,
			PSTPathMustBeAFile = 1833949493U,
			ReportMovedMailboxAlreadyMorphedToMailUser = 3825549875U,
			UnableToReadAD = 3369317625U,
			MoveRestartDueToContainerMailboxesChanged = 1476056932U,
			ReportCopyPerUserReadUnreadDataStarted = 3728110147U,
			JobCannotBeRehomedWhenInProgress = 1704413971U,
			FolderHierarchyIsInconsistent = 2750851982U,
			ReportMoveRestartedDueToSignatureChange = 3945110826U,
			ErrorCannotPreventCompletionForCompletingMove = 3089757325U,
			WorkloadTypeOffboarding = 4185133072U,
			MRSAlreadyConfigured = 979363606U,
			ReportTargetPublicFolderDeploymentUnlocked = 135363966U,
			ReportRequestCancelPostponed = 815599557U,
			JobHasBeenRelinquishedDueToHAStall = 1388232024U,
			RequestPriorityLow = 738613247U,
			MoveRequestDirectionPull = 1580421706U,
			UnableToApplyFolderHierarchyChanges = 119144040U,
			PickupStatusDisabled = 3837251784U,
			RemoteResource = 2288253100U,
			MoveRestartedDueToSignatureChange = 2018114904U,
			FolderHierarchyContainsNoRoots = 3406658852U,
			JobHasBeenRelinquishedDueToCIStall = 2360217149U,
			ContentIndexing = 2713483251U,
			TooManyLargeItems = 3235245142U,
			CouldNotConnectToTargetMailbox = 2888971502U,
			PSTIOException = 1886860910U,
			RequestPriorityNormal = 2240969000U,
			SmtpServerInfoMissing = 268267125U,
			NoPublicFolderMailboxFoundInSource = 4282140830U,
			WorkloadTypeRemotePstExport = 3551669574U,
			FastTransferArgumentError = 3345143506U,
			PickupStatusCompletedJob = 3435030616U,
			ReportJobProcessingDisabled = 856796896U,
			ImproperTypeForThisIdParameter = 1247936583U,
			MoveRequestMissingInfoDelete = 2006276195U,
			ReportRelinquishingJobDueToServiceStop = 3798869933U,
			PickupStatusCorruptJob = 1827127136U,
			RequestHasBeenRelinquishedDueToBadHealthOfBackendServers = 4161853231U,
			MoveRequestMissingInfoSave = 2260777903U,
			RestartingMove = 3708951374U,
			ErrorWhileUpdatingMovedMailbox = 3631041786U,
			MoveRequestValidationFailed = 1320326778U,
			MustProvideValidSessionForFindingRequests = 199165212U,
			TooManyMissingItems = 4003924173U,
			UpdateFolderFailed = 4230582602U,
			OfflinePublicFolderMigrationNotSupported = 990473213U,
			TaskCanceled = 2906979272U,
			SourceMailboxAlreadyBeingMoved = 2323682581U,
			MoveJobDeserializationFailed = 938866594U,
			MoveRequestNotFoundInQueue = 3959740005U,
			JobHasBeenCanceled = 2697508630U,
			ReportRequestStarted = 1085885350U,
			ErrorDownlevelClientsNotSupported = 90093709U,
			DataExportTimeout = 2295784267U,
			TargetMailboxConnectionWasLost = 2935411904U,
			JobHasBeenRelinquishedDueToDatabaseFailover = 2247143358U,
			PublicFolderMailboxesNotProvisionedForMigration = 3942653727U,
			RequestPriorityHigher = 3540791304U,
			JobHasBeenRelinquishedDueToHAOrCIStalls = 1147283780U,
			ReportRequestCanceled = 1573145718U,
			InvalidProxyOperationOrder = 4082292636U,
			ReportRequestOfflineMovePostponed = 2286820319U,
			MailboxIsBeingMoved = 3759213604U,
			NoSuchRequestInSpecifiedIndex = 3604568438U,
			InitializedWithInvalidObjectId = 859003787U,
			ReportCopyPerUserReadUnreadDataCompleted = 215436927U,
			ReportSessionStatisticsUpdated = 699419898U,
			ReportRelinquishingJobDueToServerThrottling = 2708825588U,
			MRSNotConfigured = 3162969353U,
			MailboxRootFolderNotFound = 2244475911U,
			WorkloadTypeLoadBalancing = 540485114U,
			JobIsQuarantined = 1035843159U,
			ReportSourceSDCannotBeRead = 2538826952U,
			ReportMoveRequestIsNoLongerSticky = 2850410499U,
			ClusterNotFound = 1606949857U,
			MoveRestartDueToIsIntegCheck = 833630790U,
			ReportJobIsStillStalled = 952727642U,
			WorkloadTypeRemotePstIngestion = 4023361926U,
			ReportPrimaryMservEntryPointsToExo = 472899259U,
			ValidationADUserIsNotBeingMoved = 2750716986U,
			PostMoveStateIsUncertain = 702077469U,
			RequestPriorityHigh = 901032809U,
			SourceContainer = 888446530U,
			WorkloadTypeTenantUpgrade = 3595573419U,
			EasMissingMessageCategory = 3655041298U,
			JobHasBeenRelinquished = 336070114U,
			RecoverySyncNotImplemented = 2930966715U,
			ErrorTooManyCleanupRetries = 1123448769U,
			ReportFinalSyncStarted = 3238364772U,
			ReportJobExitedStalledByThrottlingState = 1078342972U,
			MustProvideNonEmptyStringForIdentity = 1302133406U,
			ReportRelinquishingJobDueToNeedForRehome = 2181380613U,
			NotEnoughInformationSupplied = 2761303157U,
			NoDataContext = 4239850164U,
			ReportMoveCompleted = 2023474786U,
			UnableToDeleteMoveRequestMessage = 4145210966U,
			DestinationFolderHierarchyInconsistent = 4239846426U,
			NotEnoughInformationToFindMoveRequest = 4095394241U,
			TaskSchedulerStopped = 949547255U,
			ReportRelinquishingJobDueToCIStall = 1292011054U,
			WriteCpu = 45208631U,
			ReportHomeMdbPointsToTarget = 73509179U,
			CorruptRestrictionData = 672463457U,
			ReportIncrementalMoveRestartDueToGlobalCounterRangeDepletion = 1194778775U,
			ReadRpc = 3109918443U,
			WorkloadTypeLocal = 1212084898U,
			MoveRequestDirectionPush = 3906020551U,
			SourceFolderHierarchyInconsistent = 3213196515U,
			RequestPriorityHighest = 2170397003U,
			ErrorFinalizationIsBlocked = 1787602764U,
			MoveRequestTypeCrossOrg = 1410610418U,
			MrsProxyServiceIsDisabled = 1554654695U,
			ReportMoveCanceled = 2497488330U,
			ErrorCannotPreventCompletionForOfflineMove = 1153630962U,
			RequestHasBeenPostponedDueToBadHealthOfBackendServers2 = 3994387752U,
			ValidationNoCorrespondingIndexEntries = 1608330969U,
			InvalidSyncStateData = 1728063749U,
			ReportMoveRestartedDueToSourceCorruption = 874970588U,
			JobHasBeenAutoSuspended = 3603681089U,
			InputDataIsInvalid = 3843865613U,
			ReportJobExitedStalledState = 3287770946U,
			ActionNotSupported = 1094055921U,
			ReportTargetAuxFolderContentMailboxGuidUpdated = 1690707778U,
			ReportStoreMailboxHasFinalized = 1049832713U,
			ErrorReservationExpired = 1157208427U,
			ErrorImplicitSplit = 3856642209U,
			ReportRelinquishingJob = 1609645991U,
			CouldNotConnectToSourceMailbox = 2756637510U,
			NoFoldersIncluded = 2671521794U,
			ReportSuspendingJob = 780406631U,
			WriteRpc = 1228469352U,
			NotConnected = 2309811282U,
			MdbReplication = 4081437335U,
			ReportRulesWillNotBeCopied = 1499253095U,
			ReportSkippingUpdateSourceMailbox = 772970447U,
			ErrorEmptyMailboxGuid = 2051604558U,
			ReportArchiveAlreadyUpdated = 4227707103U,
			JobHasBeenSynced = 3167108423U,
			JobHasBeenRelinquishedDueToLongRun = 300694364U,
			ReportRelinquishingJobDueToHAOrCIStalling = 2739554404U,
			Mailbox = 3709264734U,
			FolderHierarchyIsInconsistentTemporarily = 2181899002U,
			RequestPriorityEmergency = 1729678064U,
			ReportRelinquishingJobDueToHAStall = 3018124355U,
			CorruptSyncState = 2466192281U,
			ReportTargetPublicFolderContentMailboxGuidUpdated = 1574793031U,
			NoMRSAvailable = 551973766U,
			RequestPriorityLower = 3329536416U,
			ReportAutoSuspendingJob = 2648214014U,
			ReportCalendarFolderFaiSaveFailed = 123753966U,
			MoveIsPreventedFromFinalization = 2000650826U,
			ReportMoveAlreadyFinished = 615074259U,
			RehomeRequestFailure = 3151307821U,
			RequestIsStalledByHigherPriorityJobs = 2161771148U,
			WorkloadTypeEmergency = 951459652U,
			ReportCalendarFolderSaveFailed = 1237434822U,
			MRSProxyConnectionNotThrottledError = 2993893239U,
			ReportWaitingForMailboxDataReplication = 3133742332U,
			ReportDatabaseFailedOver = 1628850222U,
			FolderIsMissingInMerge = 1268966663U,
			Unknown = 2846264340U,
			WorkloadTypeSyncAggregation = 681951690U,
			ReportTargetUserIsNotMailEnabledUser = 1347847794U,
			ReportRequestIsNoLongerSticky = 676706684U,
			MoveRequestTypeIntraOrg = 3237229570U,
			ValidationMoveRequestNotDeserialized = 3107299645U,
			ReportMoveStarted = 2605163142U,
			ReportPostMoveCleanupStarted = 1655277830U,
			InternalAccessFolderCreationIsNotSupported = 704248557U,
			ReportRequestCompleted = 1464484042U,
			ReadCpu = 3109918916U,
			TargetContainer = 202150486U,
			RequestPriorityLowest = 4026185433U,
			WorkloadTypeNone = 630728405U,
			UnableToObtainServersInLocalSite = 4156705784U,
			WrongUserObjectFound = 3403189953U,
			GetIdsFromNamesCalledOnDestination = 3829274068U,
			DataExportCanceled = 1551771611U,
			TooManyBadItems = 3775277574U,
			ReportVerifyingMailboxContents = 1220106367U
		}

		private enum ParamIDs
		{
			EasFolderSyncFailed,
			UnableToOpenMailbox,
			ServerError,
			ReportSyncStateNull,
			RecipientArchiveGuidMismatch,
			ReportDestinationMailboxResetSucceeded,
			ReportMovedMailboxUpdated,
			SystemMailboxNotFound,
			ReportRemovingTargetMailboxDueToOfflineMoveFailure,
			TimeoutError,
			ReportMailboxBeforeFinalization2,
			MoveHasBeenSynced,
			MoveCancelFailedForAlreadyCompletedMove,
			RPCHTTPPublicFoldersId,
			ReportThrottles,
			ReportMailboxInfoAfterMove,
			ReportSourceMailboxBeforeFinalization2,
			RulesDataContext,
			UnableToGetPSTProps,
			ReportFailedToDisconnectFromSource2,
			ReportJobHasBeenRelinquishedDueToServerBusy,
			ReportDestinationMailboxClearSyncStateFailed,
			MoveCompleteFailedForAlreadyFailedMove,
			DatabaseCouldNotBeMapped,
			ReportCopyFolderPropertyProgress,
			ReportUnableToLoadDestinationUser,
			NotImplemented,
			ReportFolderCreationProgress,
			PropValuesDataContext,
			ReportMailboxInfoBeforeMoveLoc,
			ReportMoveRequestCreated,
			PickupStatusRequestTypeNotSupported,
			ReportRequestSaveFailed,
			MoveRequestDataMissing,
			FolderDataContextSearch,
			ReportIncrementalSyncContentChangesPaged2,
			ValidationValueIsMissing,
			ReportLargeAmountOfDataLossAccepted2,
			ReportLargeItemEncountered,
			JobHasBeenRelinquishedDueToCancelPostponed,
			ReportMoveRequestResumed,
			ReportRequestIsInvalid,
			PublicFoldersId,
			ItemCountsAndSizes,
			ValidationTargetArchiveMDBMismatch,
			DestinationMailboxSeedMBICacheFailed,
			MoveHasBeenRelinquishedDueToTargetDatabaseFailover,
			ServerNotFound,
			ReportIncrementalSyncProgress,
			ReportFailedToLinkADPublicFolder,
			UnableToGetPSTFolderProps,
			ReportArchiveUpdated,
			ReportSourceMailboxResetFailed,
			ReportMoveRequestSaveFailed,
			IdentityWasNotInValidFormat,
			ReportInitialSeedingStarted,
			DestinationADNotUpToDate,
			EasFolderSyncFailedTransiently,
			CompositeDataContext,
			BadItemMisplacedFolder,
			PublicFolderMigrationNotSupportedFromCurrentExchange2007Version,
			MailboxAlreadySynced,
			OrganizationRelationshipNotFound,
			ReportUnableToUpdateSourceMailbox2,
			PickupStatusLightJob,
			ReportSourceMailUserAfterFinalization,
			JobIsStuck,
			UnexpectedError,
			OlcSettingNotImplemented,
			ReportDestinationMailboxCleanupFailed,
			ReportMoveRequestProcessedByAnotherMRS,
			UnableToReadPSTFolder,
			MailboxDatabaseNotUnique,
			EasFetchFailed,
			FilterOperatorMustBeEQorNE,
			ReportMissingItemEncountered,
			PickupStatusInvalidJob,
			ReportMoveContinued,
			ReportMoveRequestSuspended,
			PstTracingId,
			InvalidUid,
			UnableToGetPSTHierarchy,
			ReportCopyProgress2,
			ReportSoftDeletedItemCountsAndSizesInArchiveLoc,
			ReportMailboxAfterFinalization2,
			ExceptionDetails,
			ValidationNameMismatch,
			ReportInitialSyncCheckpointCompleted,
			ReportLargeItemsSkipped,
			ReportMailboxArchiveInfoBeforeMove,
			ReportSourceMailboxUpdateFailed,
			EasFetchFailedTransiently,
			ReportMoveRequestIsInvalid,
			EasSendFailedError,
			ReportMovedMailUserMorphedToMailbox,
			CorruptSortOrderData,
			ReportRetryingPostMoveCleanup,
			EasFolderDeleteFailed,
			ErrorResourceReservation,
			MrsProxyServiceIsDisabled2,
			EasConnectFailed,
			CorruptNamedPropData,
			SourceMailboxCleanupFailed,
			ReportMailboxBeforeFinalization,
			RequestHasBeenPostponedDueToBadHealthOfBackendServers,
			JobHasBeenRelinquishedDueToProxyThrottling,
			PopTracingId,
			MoveHasBeenAutoSuspendedUntilCompleteAfter,
			MustRehomeRequestToSupportedVersion,
			UnsupportedSyncProtocol,
			ReportDestinationMailboxResetFailed3,
			MoveHasBeenCanceled,
			InvalidMoveRequest,
			ErrorWlmResourceUnhealthy1,
			ReportRelinquishBecauseMailboxIsLocked,
			ErrorWlmResourceUnhealthy,
			PickupStatusCreateJob,
			MailboxDatabaseNotFoundById,
			UnsupportedRecipientType,
			SimpleValueDataContext,
			PropTagsDataContext,
			ReportMergeInitialized,
			InvalidOperationError,
			ReportFailedToUpdateUserSD2,
			ReportMoveRequestSet,
			PickupStatusJobPoisoned,
			UnexpectedFilterType,
			StoreIntegError,
			ErrorWlmCapacityExceeded3,
			ClusterIPNotFound,
			InvalidEscapedChar,
			ReportFailedToApplySearchCondition,
			BadItemMissingFolder,
			FolderAliasIsInvalid,
			FolderDataContextGeneric,
			ReportFailingMoveBecauseSyncStateIssue,
			ReportMailboxRemovedRetrying,
			ReportRequestSet,
			PublicFolderMoveTracingId,
			UnableToReadPSTMessage,
			ReportTransientExceptionOccurred,
			IsIntegAttemptsExceededError,
			EasSyncFailedPermanently,
			FolderDataContextRoot,
			SourceMailboxIsNotInSourceMDB,
			DestinationAddMoveHistoryEntryFailed,
			ReportReplaySyncStateNull,
			JobIsPoisoned,
			ValidationFlagsMismatch2,
			InvalidDataError,
			MoveRequestMessageError,
			UnsupportedClientVersion,
			ICSViewDataContext,
			ValidationNoIndexEntryForRequest,
			MailboxSettingsJunkMailError,
			ReportInitialSyncCheckpointCreationProgress,
			NestedExceptionMsg,
			EasFetchFailedPermanently,
			MdbNotOnServer,
			RPCHTTPMailboxId,
			ValidationObjectInvolvedInMultipleRelocations,
			ReportDestinationMailboxSeedMBICacheFailed2,
			UnknownSecurityProp,
			ReportMovedMailboxMorphedToMailUser,
			RemoteServerName,
			ReportMailboxInfoBeforeMove,
			ErrorCouldNotFindMoveRequest,
			UnexpectedValue,
			RestoreMailboxId,
			UnableToSavePSTSyncState,
			MissingDatabaseName2,
			ReportIncrementalSyncContentChangesSynced,
			MRSProxyConnectionLimitReachedError,
			ValidationUserIsNotInAD,
			ReportRequestRemoved,
			ReportRequestCreated,
			DestinationMailboxResetFailed,
			ReportMoveAlreadyFinished2,
			PositionInteger,
			ValidationMoveRequestInWrongMDB,
			BadItemMissingItem,
			CrossSiteError,
			ReportSourceMailboxCleanupFailed3,
			ReportRequestIsSticky,
			SettingRehomeOnRelatedRequestsFailed,
			FolderHierarchyContainsParentChainLoop,
			ReportRestartingMoveBecauseSyncStateDoesNotExist,
			CertificateLoadError,
			KBytesPerSec,
			ValidationTargetUserMismatch,
			OperationDataContext,
			ReportRelinquishBecauseResourceReservationFailed,
			ReportIncrementalSyncCompleted2,
			CommunicationError,
			ReportBadItemEncountered,
			InvalidServerName,
			OnlineMoveNotSupported,
			UnsupportedRemoteServerVersion,
			ReportBadItemEncountered2,
			ReportSyncedJob,
			ValidationRequestTypeMismatch,
			RecipientIsNotAMailbox,
			ReportFailedToDisconnectFromSource,
			UnableToCreateToken,
			EasFetchFailedError,
			ReportMoveRequestIsSticky,
			ReportMailboxArchiveInfoBeforeMoveLoc,
			ReportDestinationMailboxCleanupFailed2,
			JobIsStalled,
			ReportRequestResumedWithSuspendWhenReadyToComplete,
			BlockedType,
			PositionIntegerPlus,
			MoveHasBeenRelinquished,
			FolderIsLive,
			ValidationSourceMDBMismatch,
			ReportMergingFolder,
			ReportSourceMailboxBeforeFinalization,
			ValidationSourceArchiveMDBMismatch,
			ReportDestinationMailboxClearSyncStateFailed2,
			UnableToDetermineMDBSite,
			KBytes,
			StorageConnectionType,
			PropTagToPropertyDefinitionConversion,
			MailboxDataReplicationFailed,
			HandleNotFound,
			MdbIsOffline,
			UnableToLoadPSTSyncState,
			ReportSyncStateLoaded2,
			ReportSoftDeletedItemCountsAndSizesInArchive,
			ImapTracingId,
			ReportIncrementalSyncContentChanges,
			UnableToFindMbxServer,
			MailboxDatabaseNotFoundByGuid,
			RequestTypeNotUnderstoodOnThisServer,
			ReportCopyProgress,
			ValidationSourceUserMismatch,
			TargetRecipientIsNotAnMEU,
			MoveRequestMessageWarningSeparator,
			ReportRequestResumed,
			ReportSyncStateCorrupt,
			ReportMoveIsStalled,
			JobHasBeenRelinquishedDueToMailboxLockout,
			DestinationMailboxNotCleanedUp,
			PublicFolderMove,
			ReportSourceMailboxCleanupSucceeded,
			ReportDestinationMailboxResetNotGuaranteed,
			CommunicationWithRemoteServiceFailed,
			MailboxIsNotBeingMoved,
			ErrorWlmCapacityExceeded2,
			PublicFolderMigrationNotSupportedFromExchange2003OrEarlier,
			CouldNotFindDcHavingUmmUpdateError,
			UnknownRestrictionType,
			ReportDestinationMailboxClearSyncStateSucceeded,
			ReportInitializingMove,
			BadItemFolderRule,
			FolderIsMissing,
			ReportInitializingFolderHierarchy,
			ReportSyncStateSaveFailed2,
			ReportSyncStateLoaded,
			ReportUpdateMovedMailboxFailureAfterADSwitchover,
			MailboxDoesNotExist,
			PositionOfMoveRequestInSystemMailboxQueue,
			ArchiveMailboxTracingId,
			ReportRetryingMailboxCreation,
			ReportInitialSeedingCompleted,
			FastTransferBuffer,
			ErrorWlmCapacityExceeded,
			RestoringConnectedMailboxError,
			ReportProgress,
			EntryIDsDataContext,
			EasTracingId,
			ReportTransientException,
			RequestGuidNotUnique,
			MoveIsStalled,
			ContainerMailboxTracingId,
			RequestIsStalled,
			JobHasBeenRelinquishedDueToTransientErrorDuringOfflineMove,
			ReportFailedToUpdateUserSD,
			PrimaryMailboxId,
			ReportIncrementalSyncContentChangesPaged,
			InvalidEndpointAddressError,
			DatacenterMissingHosts,
			ReportSyncStateCleared,
			ReportJobIsStalled,
			ReportSourceMailboxConnection,
			ReportDestinationMailboxSeedMBICacheSucceeded2,
			RecipientNotFound,
			ReportCleanUpFoldersDestination,
			UnableToOpenPST2,
			ValidationFlagsMismatch,
			ReportRequestAllowedMismatch,
			ReportRequestSuspended,
			PublicFolderMigrationNotSupportedFromCurrentExchange2010Version,
			MoveRequestDataIsCorrupt,
			SourceMailboxUpdateFailed,
			ReportUnableToPreserveMailboxSignature,
			UsageText,
			ReportLargeAmountOfDataLossAccepted,
			EndpointNotFoundError,
			MailboxNotSynced,
			ReportSyncStateWrongRequestGuid,
			ReportDestinationMailboxConnection,
			ReportIncrementalSyncContentChangesSynced2,
			EasFolderSyncFailedPermanently,
			ReportFailedToDisconnectFromDestination2,
			NamedPropsDataContext,
			ReportFailingInvalidMoveRequest,
			BadItemCorruptMailboxSetting,
			ReportMailboxAfterFinalization,
			MessageEnumerationFailed,
			ErrorTargetDeliveryDomainMismatch,
			SortOrderDataContext,
			CannotCreateEntryId,
			PropertyMismatch,
			ReportWaitingIsInteg,
			SoftDeletedItemsCountAndSize,
			EasSyncCouldNotFindFolder,
			EasFolderCreateFailed,
			MailPublicFolderWithLegacyExchangeDnNotFound,
			ReportFolderHierarchyInitialized,
			ReportRequestContinued,
			ReportFolderMergeStats,
			ReportCorruptItemsSkipped,
			ValidationUserLacksMailbox,
			EasSyncFailed,
			ReportSoftDeletedItemsWillNotBeMigrated,
			FolderDataContextId,
			ReportIcsSyncStateNull,
			ReportFatalExceptionOccurred,
			ValidationMailboxIdentitiesDontMatch,
			ReportMailboxArchiveInfoAfterMove,
			FolderHierarchyContainsMultipleRoots,
			ValidationStorageMDBMismatch,
			ReportFatalException,
			ReportMailboxInfoAfterMoveLoc,
			UnableToFetchMimeStream,
			ReportUnableToUpdateSourceMailbox,
			ErrorNoCASServersInSite,
			ReportSoftDeletedItemsNotMigrated,
			EasMoveFailedError,
			ReportReplayActionsEnumerated,
			ValidationTargetMDBMismatch,
			EasMoveFailed,
			ReportRequestSaveFailed2,
			IsExcludedFromProvisioningError,
			ClusterIPMissingHosts,
			MoveCompleteFailedForAlreadyCanceledMove,
			WindowsLiveIDAddressIsMissing,
			ReportStartedIsInteg,
			ContentFilterIsInvalid,
			ReportCompletedIsInteg,
			BadItemSearchFolder,
			UnableToGetPSTReceiveFolder,
			BadItemLarge,
			BadItemFolderPropertyMismatch,
			AuxFolderMoveTracingId,
			JobHasBeenRelinquishedDueToDataGuaranteeTimeout,
			EasSendFailed,
			ErrorRegKeyNotExist,
			ReportTooManyTransientFailures,
			RestrictionDataContext,
			ServerNotFoundByGuid,
			MoveRequestMessageWarning,
			MRSInstanceFailed,
			ReportTargetMailboxAfterFinalization2,
			EasCountFailed,
			FolderReferencedMoreThanOnce,
			RecipientAggregatedMailboxNotFound,
			PostSaveActionFailed,
			ReportReplayActionsSynced,
			FolderPathIsInvalid,
			RestoreMailboxTracingId,
			ReportMoveRequestRemoved,
			JobHasBeenRelinquishedDueToServerBusy,
			PickupStatusSubTypeNotSupported,
			PickupStatusReservationFailure,
			ReportIncrementalSyncHierarchyChanges,
			InvalidHandleType,
			ProviderAlreadySpecificToDatabase,
			ReportSourceMailboxCleanupFailed2,
			ReportReplayActionsCompleted,
			ReportTargetMailUserBeforeFinalization,
			MailPublicFolderWithObjectIdNotFound,
			ReportIncrementalSyncCompleted,
			PickupStatusProxyBackoff,
			MailPublicFolderWithMultipleEntriesFound,
			MoveRequestMessageInformational,
			ReportJobIsStalledWithFailure,
			ReportIncrementalSyncContentChanges2,
			NotSupportedCodePageError,
			RecipientInvalidLegDN,
			ReportMailboxContentsVerificationComplete,
			ValidationOrganizationMismatch,
			UnableToStreamPSTProp,
			UnsupportedClientVersionWithOperation,
			ReportJobRehomed,
			ReportSourceMailboxCleanupSkipped,
			ReportFailedToDisconnectFromDestination,
			IsIntegTooLongError,
			ReportIncrementalSyncChanges,
			UnableToCreatePSTMessage,
			ErrorStaticCapacityExceeded,
			FolderAlreadyExists,
			ReportMoveResumed,
			ReportSoftDeletedItemCountsAndSizes,
			MissingDatabaseName,
			ReportUpdateMovedMailboxError,
			UnableToVerifyMailboxConnectivity,
			BadItemCorrupt,
			ReportMailboxSignatureIsNotPreserved,
			ReportProxyConnectionLimitMet,
			FolderAlreadyInTarget,
			ReportTargetFolderDeleted,
			FolderCopyFailed,
			ErrorStaticCapacityExceeded1,
			ReportWaitingForAdReplication,
			MailboxServerInformation,
			ReportTargetMailUserBeforeFinalization2,
			MoveHasBeenAutoSuspended,
			ReportRelinquishingJobDueToFailover,
			OrphanedDatabaseName,
			ReportTargetMailboxAfterFinalization,
			RecipientPropertyIsNotWriteable,
			PropertyTagsDoNotMatch,
			PrimaryMailboxTracingId,
			ArchiveMailboxId,
			ImapObjectNotFound,
			FolderReferencedAsBothIncludedAndExcluded,
			DestinationMailboxSyncStateDeletionFailed,
			FolderHierarchyContainsDuplicates,
			ReportSourceFolderDeleted,
			ReportSoftDeletedItemCountsAndSizesLoc,
			EasSyncFailedTransiently,
			ReportJobResumed,
			PickupStatusJobTypeNotSupported,
			ValidationMailboxGuidsDontMatch,
			ReportReplaySyncStateCorrupt,
			UnableToReadADUser,
			UnsupportedRemoteServerVersionWithOperation,
			JobHasBeenRelinquishedDueToResourceReservation,
			ReportUnableToComputeTargetAddress,
			ReportSourceMailUserAfterFinalization2,
			RecipientMissingLegDN,
			ReportIcsSyncStateCorrupt,
			EasFolderUpdateFailed,
			UnableToUpdateSourceMailbox,
			DestinationMailboxResetNotGuaranteed,
			ReportMailboxArchiveInfoAfterMoveLoc,
			UnableToClosePST,
			ReportRequestProcessedByAnotherMRS,
			JobIsStalledAndFailed,
			ReportSyncStateSaveFailed,
			ErrorMoveInProgress,
			ParsingMessageEntryIdFailed,
			ReportInitializingJob,
			MoveHasBeenRelinquishedDueToProxyThrottling,
			RpcClientAccessServerNotConfiguredForMdb,
			RemoteMailboxServerInformation,
			MoveReportEntryMessage,
			ReportDestinationMailboxResetFailed2,
			BadItemFolderProperty,
			EasObjectNotFound,
			MaxSubmissionExceeded,
			UnsupportedImapMessageEntryIdVersion,
			ExceptionWithStack,
			ReportDestinationMailboxSeedMBICacheFailed,
			BadItemFolderACL,
			CannotCreateMessageId,
			ReportRestartingMoveBecauseMailboxSignatureVersionIsDifferent
		}
	}
}
