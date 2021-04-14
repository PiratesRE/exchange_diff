using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ReplayStrings
	{
		static ReplayStrings()
		{
			ReplayStrings.stringIDs.Add(800943078U, "RepairStateClusterIsNotRunning");
			ReplayStrings.stringIDs.Add(3145329300U, "RemoveLogReasonE00OutOfDate");
			ReplayStrings.stringIDs.Add(2169511412U, "FailedToOpenShipLogContextAccessDenied");
			ReplayStrings.stringIDs.Add(3896662565U, "ReplayServiceRpcUnknownInstanceException");
			ReplayStrings.stringIDs.Add(1265493030U, "LockOwnerIdle");
			ReplayStrings.stringIDs.Add(1348435424U, "IncSeedNotSupportedWithShrinkDatabaseError");
			ReplayStrings.stringIDs.Add(1095788331U, "CantStartFromCommandLineTitle");
			ReplayStrings.stringIDs.Add(1390940095U, "AmBcsDatabaseHasNoCopies");
			ReplayStrings.stringIDs.Add(1065254054U, "ErrorCouldNotFindClusterGroupOwnerNodeForAmConfig");
			ReplayStrings.stringIDs.Add(615086071U, "SeederEcDBNotFound");
			ReplayStrings.stringIDs.Add(276770850U, "SeederEcSeedingCancelled");
			ReplayStrings.stringIDs.Add(3718483653U, "DagTaskInstalledFailoverClustering");
			ReplayStrings.stringIDs.Add(2876590454U, "SeederEcDeviceNotReady");
			ReplayStrings.stringIDs.Add(3794143290U, "ErrorReadingDagServerForAmConfig");
			ReplayStrings.stringIDs.Add(3273471454U, "NetworkReadEOF");
			ReplayStrings.stringIDs.Add(3630272863U, "RemoveLogReasonFailedInspection");
			ReplayStrings.stringIDs.Add(3925066867U, "InvalidLogCopyResponse");
			ReplayStrings.stringIDs.Add(2936934256U, "MonitoringADInitNotCompleteException");
			ReplayStrings.stringIDs.Add(861304653U, "AutoReseedFailedToFindVolumeName");
			ReplayStrings.stringIDs.Add(1749232750U, "DagTaskComponentManagerAnotherInstanceRunning");
			ReplayStrings.stringIDs.Add(4014700033U, "AutoReseedManualReseedLaunched");
			ReplayStrings.stringIDs.Add(3988781731U, "SeederEcNoOnlineEdb");
			ReplayStrings.stringIDs.Add(3096281422U, "FullServerSeedInProgressException");
			ReplayStrings.stringIDs.Add(418608118U, "ReplayServiceSuspendCommentException");
			ReplayStrings.stringIDs.Add(856033660U, "InvalidDbForSeedSpecifiedException");
			ReplayStrings.stringIDs.Add(798341136U, "VolumeMountPathDoesNotExistException");
			ReplayStrings.stringIDs.Add(1105864708U, "AmDbOperationWaitFailedException");
			ReplayStrings.stringIDs.Add(2630456743U, "ErrorAmConfigNotInitialized");
			ReplayStrings.stringIDs.Add(2364081162U, "NetworkCorruptDataGeneric");
			ReplayStrings.stringIDs.Add(2266299742U, "ErrorAutomountConsensusNotReached");
			ReplayStrings.stringIDs.Add(1607989356U, "AmBcsNoneSpecified");
			ReplayStrings.stringIDs.Add(3766459347U, "SeederEcNotEnoughDiskException");
			ReplayStrings.stringIDs.Add(1148446483U, "NetworkFailedToAuthServer");
			ReplayStrings.stringIDs.Add(4045836413U, "ErrorFailedToFindLocalServer");
			ReplayStrings.stringIDs.Add(4106538885U, "FailedToOpenShipLogContextDatabaseNotMounted");
			ReplayStrings.stringIDs.Add(910204805U, "ReplayServiceSuspendBlockedAcllException");
			ReplayStrings.stringIDs.Add(1295385018U, "DatabaseCopyLayoutTableNullException");
			ReplayStrings.stringIDs.Add(820683245U, "AmServiceShuttingDown");
			ReplayStrings.stringIDs.Add(512707820U, "DagTaskDagIpAddressesMustBeIpv4Exception");
			ReplayStrings.stringIDs.Add(3351215994U, "UnknownError");
			ReplayStrings.stringIDs.Add(3572810924U, "EseBackFileSystemCorruption");
			ReplayStrings.stringIDs.Add(214622620U, "SuspendOperationName");
			ReplayStrings.stringIDs.Add(3342720985U, "PagePatchLegacyFileExistsException");
			ReplayStrings.stringIDs.Add(606338949U, "SeederEcSuccess");
			ReplayStrings.stringIDs.Add(1055054291U, "NoServices");
			ReplayStrings.stringIDs.Add(1298429885U, "StoreServiceMonitorCriticalError");
			ReplayStrings.stringIDs.Add(3887748188U, "CannotChangeProperties");
			ReplayStrings.stringIDs.Add(2034469932U, "SeederEchrInvalidCallSequence");
			ReplayStrings.stringIDs.Add(3043738041U, "FailedToOpenShipLogContextEseCircularLoggingEnabled");
			ReplayStrings.stringIDs.Add(2591444232U, "ReplayServiceResumeRpcFailedException");
			ReplayStrings.stringIDs.Add(1344658319U, "NetworkSecurityFailed");
			ReplayStrings.stringIDs.Add(3184656137U, "ReplayServiceSuspendRpcFailedException");
			ReplayStrings.stringIDs.Add(1163467430U, "SuspendWantedWriteFailedException");
			ReplayStrings.stringIDs.Add(3512548730U, "ReplayServiceSuspendResumeBlockedException");
			ReplayStrings.stringIDs.Add(1378783691U, "AmDbActionDismountFailedException");
			ReplayStrings.stringIDs.Add(387713678U, "AutoReseedNeverMountedWorkflowReason");
			ReplayStrings.stringIDs.Add(1967903804U, "AutoReseedLogAndDbNotOnSameVolume");
			ReplayStrings.stringIDs.Add(85017254U, "FullServerSeedSkippedShutdownException");
			ReplayStrings.stringIDs.Add(2554265426U, "SeederEcOverlappedWriteErr");
			ReplayStrings.stringIDs.Add(3580573960U, "FailToCleanUpFiles");
			ReplayStrings.stringIDs.Add(2045268632U, "SeederEcNotEnoughDisk");
			ReplayStrings.stringIDs.Add(1045878879U, "MonitoringADServiceShuttingDownException");
			ReplayStrings.stringIDs.Add(1612966754U, "DbHTServiceShuttingDownException");
			ReplayStrings.stringIDs.Add(4049732327U, "NullDbCopyException");
			ReplayStrings.stringIDs.Add(2602551968U, "ErrorClusterServiceNotRunningForAmConfig");
			ReplayStrings.stringIDs.Add(3128283390U, "SeederEcError");
			ReplayStrings.stringIDs.Add(129692820U, "AutoReseedFailedAdminSuspended");
			ReplayStrings.stringIDs.Add(726910307U, "NetworkNoUsableEndpoints");
			ReplayStrings.stringIDs.Add(3772399040U, "ClusterServiceMonitorCriticalError");
			ReplayStrings.stringIDs.Add(1871702564U, "Resynchronizing");
			ReplayStrings.stringIDs.Add(3929636263U, "ReplayServiceSuspendBlockedResynchronizingException");
			ReplayStrings.stringIDs.Add(828128533U, "LockOwnerComponent");
			ReplayStrings.stringIDs.Add(4232351636U, "NetworkIsDisabled");
			ReplayStrings.stringIDs.Add(707284339U, "ResumeOperationName");
			ReplayStrings.stringIDs.Add(2777244007U, "ReplayServiceSuspendReseedBlockedException");
			ReplayStrings.stringIDs.Add(544686542U, "SuspendMessageWriteFailedException");
			ReplayStrings.stringIDs.Add(627050854U, "SyncSuspendResumeOperationName");
			ReplayStrings.stringIDs.Add(1279119335U, "FailedAndSuspended");
			ReplayStrings.stringIDs.Add(1839653327U, "TPRProviderNotListening");
			ReplayStrings.stringIDs.Add(3099361335U, "Suspended");
			ReplayStrings.stringIDs.Add(4137367259U, "ReplayServiceSuspendInPlaceReseedBlockedException");
			ReplayStrings.stringIDs.Add(3167054525U, "AutoReseedMoveActiveBeforeRebuildCatalog");
			ReplayStrings.stringIDs.Add(448095525U, "ErrorCouldNotConnectClusterForAmConfig");
			ReplayStrings.stringIDs.Add(3631647333U, "ReplayServiceShuttingDownException");
			ReplayStrings.stringIDs.Add(2056943490U, "ErrorFailedToOpenClusterObjects");
			ReplayStrings.stringIDs.Add(4042526291U, "FailedToOpenShipLogContextStoreStopped");
			ReplayStrings.stringIDs.Add(1622537741U, "NullDatabaseException");
			ReplayStrings.stringIDs.Add(865952845U, "SeederEcCommunicationsError");
			ReplayStrings.stringIDs.Add(1445478759U, "DagTaskPamNotMovedSubsequentOperationsMayBeSlowOrUnreliable");
			ReplayStrings.stringIDs.Add(482041519U, "SeederEcFailAcqRight");
			ReplayStrings.stringIDs.Add(204895011U, "ProgressStatusInProgress");
			ReplayStrings.stringIDs.Add(3439296139U, "CouldNotGetMountStatus");
			ReplayStrings.stringIDs.Add(3630311467U, "AmClusterNotRunningException");
			ReplayStrings.stringIDs.Add(2719597889U, "LockOwnerConfigChecker");
			ReplayStrings.stringIDs.Add(200627337U, "PrepareToStopCalled");
			ReplayStrings.stringIDs.Add(394420742U, "ReplayServiceSuspendWantedClearedException");
			ReplayStrings.stringIDs.Add(913564495U, "DBCHasNoValidTargetEdbPath");
			ReplayStrings.stringIDs.Add(682288005U, "DeleteChkptReasonCorrupted");
			ReplayStrings.stringIDs.Add(3559163736U, "TPRChangeFailedBecauseNotDismounted");
			ReplayStrings.stringIDs.Add(3778987676U, "CouldNotFindVolumeForFormatException");
			ReplayStrings.stringIDs.Add(2242986398U, "CannotChangeName");
			ReplayStrings.stringIDs.Add(3293194015U, "NetworkManagerInitError");
			ReplayStrings.stringIDs.Add(1105077015U, "PassiveCopyDisconnected");
			ReplayStrings.stringIDs.Add(3691652467U, "SeederEchrErrorFromESECall");
			ReplayStrings.stringIDs.Add(2139980255U, "ReplayServiceSuspendBlockedBackupInProgressException");
			ReplayStrings.stringIDs.Add(3673369909U, "ErrorCouldNotFindServerForAmConfig");
			ReplayStrings.stringIDs.Add(2812191260U, "FailedAtReplacingLogFiles");
			ReplayStrings.stringIDs.Add(887012549U, "ReplayServiceResumeRpcFailedSeedingException");
			ReplayStrings.stringIDs.Add(3025714949U, "StoreNotOnline");
			ReplayStrings.stringIDs.Add(2151299999U, "LockOwnerAttemptCopyLastLogs");
			ReplayStrings.stringIDs.Add(833784388U, "MsexchangereplLong");
			ReplayStrings.stringIDs.Add(2787647009U, "AmOperationInvalidForStandaloneRoleException");
			ReplayStrings.stringIDs.Add(1556772377U, "JetErrorDatabaseNotFound");
			ReplayStrings.stringIDs.Add(2325224058U, "SeederEcBackupInProgress");
			ReplayStrings.stringIDs.Add(511004165U, "LogCopierFailedToGetSuspendLock");
			ReplayStrings.stringIDs.Add(3508567603U, "NetworkDataOverflowGeneric");
			ReplayStrings.stringIDs.Add(4084429598U, "TPRNotEnabled");
			ReplayStrings.stringIDs.Add(458303596U, "LockOwnerSuspend");
			ReplayStrings.stringIDs.Add(1791510079U, "DbValidationFullCopyStatusResultsLabel");
			ReplayStrings.stringIDs.Add(3318672153U, "LogRepairNotPossibleInsuffientToCheckDivergence");
			ReplayStrings.stringIDs.Add(1539727809U, "DagTaskComponentManagerWantsToRebootException");
			ReplayStrings.stringIDs.Add(3985107625U, "NetworkCancelled");
			ReplayStrings.stringIDs.Add(413167004U, "ErrorFailedToGetClusterCoreGroup");
			ReplayStrings.stringIDs.Add(1348455624U, "NotInPendingState");
			ReplayStrings.stringIDs.Add(662858283U, "ErrorInvalidPamServerName");
			ReplayStrings.stringIDs.Add(2070740709U, "SeederEchrErrorFromCallbackCall");
			ReplayStrings.stringIDs.Add(4031202440U, "EnableReplayLagOperationName");
			ReplayStrings.stringIDs.Add(2490829887U, "SeederInstanceAlreadyFailedException");
			ReplayStrings.stringIDs.Add(854026562U, "ReplayServiceSuspendWantedSetException");
			ReplayStrings.stringIDs.Add(1857824471U, "SeederEcOOMem");
			ReplayStrings.stringIDs.Add(399717229U, "DbValidationCopyStatusNameLabel");
			ReplayStrings.stringIDs.Add(3239149605U, "SeederEchrRestoreAtFileLevel");
			ReplayStrings.stringIDs.Add(2097320554U, "TPRNotYetStarted");
			ReplayStrings.stringIDs.Add(560332821U, "SeederEcInvalidInput");
			ReplayStrings.stringIDs.Add(3655345505U, "ErrorRemoteSiteNotConnected");
			ReplayStrings.stringIDs.Add(4251585129U, "ErrorLocalNodeNotUpYet");
			ReplayStrings.stringIDs.Add(1556943668U, "DagTaskClusteringRequiresEnterpriseSkuException");
			ReplayStrings.stringIDs.Add(844399741U, "MsexchangesearchLong");
			ReplayStrings.stringIDs.Add(3809273542U, "LockOwnerBackup");
			ReplayStrings.stringIDs.Add(1718068369U, "AmDbActionMoveFailedException");
			ReplayStrings.stringIDs.Add(1054423051U, "Failed");
			ReplayStrings.stringIDs.Add(3438214222U, "GranularReplicationOverflow");
			ReplayStrings.stringIDs.Add(3142177591U, "CantStartFromCommandLine");
			ReplayStrings.stringIDs.Add(1944472637U, "AmDbActionMountFailedException");
			ReplayStrings.stringIDs.Add(2389546758U, "InvalidSavedStateException");
			ReplayStrings.stringIDs.Add(1836563684U, "SeederOperationAborted");
			ReplayStrings.stringIDs.Add(3897089617U, "FailedToOpenShipLogContextInvalidParameter");
			ReplayStrings.stringIDs.Add(2203487748U, "ErrorDagDoesNotHaveAnyMemberServers");
			ReplayStrings.stringIDs.Add(1450196582U, "ErrorAmInjectedUnknownConfig");
			ReplayStrings.stringIDs.Add(663986635U, "DisableReplayLagOperationName");
		}

		public static LocalizedString RepairStateClusterIsNotRunning
		{
			get
			{
				return new LocalizedString("RepairStateClusterIsNotRunning", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederFailedToCreateDirectory(string directory, string error)
		{
			return new LocalizedString("SeederFailedToCreateDirectory", "Ex37435E", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directory,
				error
			});
		}

		public static LocalizedString RepairStateLocalServerIsNotInDag(string serverName)
		{
			return new LocalizedString("RepairStateLocalServerIsNotInDag", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString PagePatchFileDeletionException(string file, string error)
		{
			return new LocalizedString("PagePatchFileDeletionException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				error
			});
		}

		public static LocalizedString RemoveLogReasonE00OutOfDate
		{
			get
			{
				return new LocalizedString("RemoveLogReasonE00OutOfDate", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AcllLastLogNotFoundException(string dbCopy, long generation)
		{
			return new LocalizedString("AcllLastLogNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				generation
			});
		}

		public static LocalizedString ReplayServiceResumeInvalidDuringMoveException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceResumeInvalidDuringMoveException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString DumpsterRedeliveryException(string errorMsg)
		{
			return new LocalizedString("DumpsterRedeliveryException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString LastLogReplacementFileNotSubsetException(string dbCopy, string subsetFile, string superSetFile)
		{
			return new LocalizedString("LastLogReplacementFileNotSubsetException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				subsetFile,
				superSetFile
			});
		}

		public static LocalizedString ReplayServiceRestartInvalidSeedingException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceRestartInvalidSeedingException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString DagTaskComponentManagerGenericFailure(int error)
		{
			return new LocalizedString("DagTaskComponentManagerGenericFailure", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DumpsterCouldNotFindHubServerException(string hubServerName)
		{
			return new LocalizedString("DumpsterCouldNotFindHubServerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				hubServerName
			});
		}

		public static LocalizedString LastLogReplacementRollbackFailedException(string dbCopy, string error)
		{
			return new LocalizedString("LastLogReplacementRollbackFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				error
			});
		}

		public static LocalizedString AmPreMountCallbackFailedMountInhibitException(string dbName, string server, string errMsg)
		{
			return new LocalizedString("AmPreMountCallbackFailedMountInhibitException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				server,
				errMsg
			});
		}

		public static LocalizedString FailedToOpenShipLogContextAccessDenied
		{
			get
			{
				return new LocalizedString("FailedToOpenShipLogContextAccessDenied", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRcrConfigOnNonMailboxException(string nodeName)
		{
			return new LocalizedString("InvalidRcrConfigOnNonMailboxException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString SeederEcLogAlreadyExist(string directory)
		{
			return new LocalizedString("SeederEcLogAlreadyExist", "ExF56313", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString AutoReseedTooManyConcurrentSeeds(int maxSeedsLimit)
		{
			return new LocalizedString("AutoReseedTooManyConcurrentSeeds", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				maxSeedsLimit
			});
		}

		public static LocalizedString FileCheckAccessDeniedDismountFailedException(string file, string dismountError)
		{
			return new LocalizedString("FileCheckAccessDeniedDismountFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				dismountError
			});
		}

		public static LocalizedString ReplayServiceRpcUnknownInstanceException
		{
			get
			{
				return new LocalizedString("ReplayServiceRpcUnknownInstanceException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GranularReplicationInitFailed(string reason)
		{
			return new LocalizedString("GranularReplicationInitFailed", "Ex4A3FF0", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AmDbNotMountedMultipleServersException(string dbName, string detailedMsg)
		{
			return new LocalizedString("AmDbNotMountedMultipleServersException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				detailedMsg
			});
		}

		public static LocalizedString LockOwnerIdle
		{
			get
			{
				return new LocalizedString("LockOwnerIdle", "Ex9D8503", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetCopyStatus(string server, string db)
		{
			return new LocalizedString("FailedToGetCopyStatus", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				db
			});
		}

		public static LocalizedString DisableReplayLagWriteFailedException(string dbCopy)
		{
			return new LocalizedString("DisableReplayLagWriteFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString TargetDbNotThere(string path)
		{
			return new LocalizedString("TargetDbNotThere", "ExE14333", false, true, ReplayStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString CiSeederExchangeSearchTransientException(string message)
		{
			return new LocalizedString("CiSeederExchangeSearchTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString PotentialRedundancyValidationDBReplicationStalled(string dbName, int totalPassiveCopiesCount, string detailedMsg)
		{
			return new LocalizedString("PotentialRedundancyValidationDBReplicationStalled", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				totalPassiveCopiesCount,
				detailedMsg
			});
		}

		public static LocalizedString LastLogReplacementTempOldFileNotDeletedException(string dbCopy, string tempOldFile, string error)
		{
			return new LocalizedString("LastLogReplacementTempOldFileNotDeletedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				tempOldFile,
				error
			});
		}

		public static LocalizedString PreserveInspectorLogsError(string errorText)
		{
			return new LocalizedString("PreserveInspectorLogsError", "Ex304365", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errorText
			});
		}

		public static LocalizedString ServerStopped(string server)
		{
			return new LocalizedString("ServerStopped", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString SeederInstanceAlreadyAddedException(string sourceMachine)
		{
			return new LocalizedString("SeederInstanceAlreadyAddedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceMachine
			});
		}

		public static LocalizedString AutoReseedAllCatalogFailed(string databaseName)
		{
			return new LocalizedString("AutoReseedAllCatalogFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString LogCopierE00InconsistentError(long e00Gen, long expectedGen)
		{
			return new LocalizedString("LogCopierE00InconsistentError", "Ex17629C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				e00Gen,
				expectedGen
			});
		}

		public static LocalizedString FailedToDisableMountPointConfigurationException(string regkeyroot)
		{
			return new LocalizedString("FailedToDisableMountPointConfigurationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				regkeyroot
			});
		}

		public static LocalizedString AcllFailedLogDivergenceDetected(string dbCopy, string sourceServer)
		{
			return new LocalizedString("AcllFailedLogDivergenceDetected", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				sourceServer
			});
		}

		public static LocalizedString IncSeedNotSupportedWithShrinkDatabaseError
		{
			get
			{
				return new LocalizedString("IncSeedNotSupportedWithShrinkDatabaseError", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedFailedCopyWorkflowSuspendedCopy(string failedSuppressionDuration)
		{
			return new LocalizedString("AutoReseedFailedCopyWorkflowSuspendedCopy", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				failedSuppressionDuration
			});
		}

		public static LocalizedString CiSeederSearchCatalogRpcPermanentException(string message)
		{
			return new LocalizedString("CiSeederSearchCatalogRpcPermanentException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString InvalidRCROperationOnNonRcrDB(string dbName)
		{
			return new LocalizedString("InvalidRCROperationOnNonRcrDB", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString CouldNotDeleteDbMountPointException(string database, string dbMountPoint, string errMsg)
		{
			return new LocalizedString("CouldNotDeleteDbMountPointException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				dbMountPoint,
				errMsg
			});
		}

		public static LocalizedString CantStartFromCommandLineTitle
		{
			get
			{
				return new LocalizedString("CantStartFromCommandLineTitle", "ExC1715A", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedCatalogActiveException(string databaseName, string serverName)
		{
			return new LocalizedString("AutoReseedCatalogActiveException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString SafeDeleteExistingFilesDataRedundancyNoResultException(string db)
		{
			return new LocalizedString("SafeDeleteExistingFilesDataRedundancyNoResultException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db
			});
		}

		public static LocalizedString AmBcsDatabaseHasNoCopies
		{
			get
			{
				return new LocalizedString("AmBcsDatabaseHasNoCopies", "Ex479B43", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkAddressResolutionFailed(string nodeName, string errMsg)
		{
			return new LocalizedString("NetworkAddressResolutionFailed", "Ex01C78B", false, true, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				errMsg
			});
		}

		public static LocalizedString ErrorCouldNotFindClusterGroupOwnerNodeForAmConfig
		{
			get
			{
				return new LocalizedString("ErrorCouldNotFindClusterGroupOwnerNodeForAmConfig", "Ex6C1CEC", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileCheckDatabaseLogfileSignature(string database, string logfileSignature, string expectedSignature)
		{
			return new LocalizedString("FileCheckDatabaseLogfileSignature", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				logfileSignature,
				expectedSignature
			});
		}

		public static LocalizedString AcllCopyStatusResumeBlockedException(string dbCopy, string errorMsg)
		{
			return new LocalizedString("AcllCopyStatusResumeBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				errorMsg
			});
		}

		public static LocalizedString CiServiceDownException(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("CiServiceDownException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString SeederEcDBNotFound
		{
			get
			{
				return new LocalizedString("SeederEcDBNotFound", "Ex6D88D5", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsDatabaseCopyIsSeedingSource(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyIsSeedingSource", "Ex2A550B", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString FailedToDeleteTempDatabase(string db, string error)
		{
			return new LocalizedString("FailedToDeleteTempDatabase", "Ex6FBC6C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				error
			});
		}

		public static LocalizedString SeederFailedToInspectLogException(string logfileName, string inspectionError)
		{
			return new LocalizedString("SeederFailedToInspectLogException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfileName,
				inspectionError
			});
		}

		public static LocalizedString ClusterBatchWriter_CommitFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_CommitFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString DbAvailabilityActiveCopyUnknownState(string dbName, string serverName, string copyStatus)
		{
			return new LocalizedString("DbAvailabilityActiveCopyUnknownState", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				copyStatus
			});
		}

		public static LocalizedString AmBcsSelectionException(string bcsMessage)
		{
			return new LocalizedString("AmBcsSelectionException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				bcsMessage
			});
		}

		public static LocalizedString TargetChkptAlreadyExists(string chkFile)
		{
			return new LocalizedString("TargetChkptAlreadyExists", "Ex9D81E3", false, true, ReplayStrings.ResourceManager, new object[]
			{
				chkFile
			});
		}

		public static LocalizedString DatabaseDismountOrKillStoreException(string databaseName, string serverName, string errorText)
		{
			return new LocalizedString("DatabaseDismountOrKillStoreException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName,
				errorText
			});
		}

		public static LocalizedString LogTruncationException(string error)
		{
			return new LocalizedString("LogTruncationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SeederFailedToFindDirectoriesUnderMountPoint(string name)
		{
			return new LocalizedString("SeederFailedToFindDirectoriesUnderMountPoint", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AmDbActionRejectedMountAtStartupNotEnabledException(string actionCode)
		{
			return new LocalizedString("AmDbActionRejectedMountAtStartupNotEnabledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionCode
			});
		}

		public static LocalizedString DagNetworkRpcServerError(string rpcName, string errMsg)
		{
			return new LocalizedString("DagNetworkRpcServerError", "ExCE1A30", false, true, ReplayStrings.ResourceManager, new object[]
			{
				rpcName,
				errMsg
			});
		}

		public static LocalizedString AmDbMoveOperationNotSupportedException(string dbName)
		{
			return new LocalizedString("AmDbMoveOperationNotSupportedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString FailedToDeserializeStr(string stringToDeserialize, string typeName)
		{
			return new LocalizedString("FailedToDeserializeStr", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				stringToDeserialize,
				typeName
			});
		}

		public static LocalizedString SeederEcSeedingCancelled
		{
			get
			{
				return new LocalizedString("SeederEcSeedingCancelled", "Ex5D30C7", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskInstalledFailoverClustering
		{
			get
			{
				return new LocalizedString("DagTaskInstalledFailoverClustering", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederEcDeviceNotReady
		{
			get
			{
				return new LocalizedString("SeederEcDeviceNotReady", "Ex476112", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsDatabaseCopyCatalogUnhealthy(string db, string server, string catalogState)
		{
			return new LocalizedString("AmBcsDatabaseCopyCatalogUnhealthy", "Ex2173EF", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				catalogState
			});
		}

		public static LocalizedString LogRepairFailedToVerifyFromActive(string tempLogName, string activeServerName, string exceptionText)
		{
			return new LocalizedString("LogRepairFailedToVerifyFromActive", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				tempLogName,
				activeServerName,
				exceptionText
			});
		}

		public static LocalizedString SafetyNetVersionCheckException(string error)
		{
			return new LocalizedString("SafetyNetVersionCheckException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SuspendCommentTooLong(int length, int limit)
		{
			return new LocalizedString("SuspendCommentTooLong", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				length,
				limit
			});
		}

		public static LocalizedString AmBcsDatabaseCopyTotalQueueLengthTooHigh(string db, string server, long length, long maxLength)
		{
			return new LocalizedString("AmBcsDatabaseCopyTotalQueueLengthTooHigh", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				length,
				maxLength
			});
		}

		public static LocalizedString SeederOperationFailedException(string errMessage)
		{
			return new LocalizedString("SeederOperationFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString ErrorReadingDagServerForAmConfig
		{
			get
			{
				return new LocalizedString("ErrorReadingDagServerForAmConfig", "Ex691995", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedInvalidLogFolderPath(string actualPath, string expectedPath)
		{
			return new LocalizedString("AutoReseedInvalidLogFolderPath", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actualPath,
				expectedPath
			});
		}

		public static LocalizedString AmBcsDatabaseCopyAlreadyTried(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyAlreadyTried", "Ex9CF852", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString InsufficientSparesForExtraCopiesException(int spares, int copies)
		{
			return new LocalizedString("InsufficientSparesForExtraCopiesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				spares,
				copies
			});
		}

		public static LocalizedString AcllCouldNotControlReplicaInstanceException(string dbCopy)
		{
			return new LocalizedString("AcllCouldNotControlReplicaInstanceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString NetworkReadEOF
		{
			get
			{
				return new LocalizedString("NetworkReadEOF", "ExC387A2", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskClusteringMustBeInstalledException(string serverName)
		{
			return new LocalizedString("DagTaskClusteringMustBeInstalledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DagTaskRemoveNodeNotUpException(string machineName, string clusterName, string machineState)
		{
			return new LocalizedString("DagTaskRemoveNodeNotUpException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				machineName,
				clusterName,
				machineState
			});
		}

		public static LocalizedString DagTaskComponentManagerServerManagerPSFailure(string error)
		{
			return new LocalizedString("DagTaskComponentManagerServerManagerPSFailure", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CouldNotFindDatabase(string dbGuid, string error)
		{
			return new LocalizedString("CouldNotFindDatabase", "Ex2324E2", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbGuid,
				error
			});
		}

		public static LocalizedString DbMoveSkippedBecauseNotFoundInClusDb(string dbName)
		{
			return new LocalizedString("DbMoveSkippedBecauseNotFoundInClusDb", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(string machineNames)
		{
			return new LocalizedString("DagTaskSetDagNeedsAllNodesUpToChangeQuorumException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				machineNames
			});
		}

		public static LocalizedString RemoveLogReasonFailedInspection
		{
			get
			{
				return new LocalizedString("RemoveLogReasonFailedInspection", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmInvalidActionCodeException(int actionCode, string member, string value)
		{
			return new LocalizedString("AmInvalidActionCodeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionCode,
				member,
				value
			});
		}

		public static LocalizedString ReplayConfigNotFoundException(string dbName, string serverName)
		{
			return new LocalizedString("ReplayConfigNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName
			});
		}

		public static LocalizedString InvalidLogCopyResponse
		{
			get
			{
				return new LocalizedString("InvalidLogCopyResponse", "ExD7CFAC", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseLogFoldersNotUnderMountpoint(string name)
		{
			return new LocalizedString("DatabaseLogFoldersNotUnderMountpoint", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ReplayServiceTooMuchMemoryNoDumpException(double memoryUsageInMib, long maximumMemoryUsageInMib, string enableWatsonRegKey)
		{
			return new LocalizedString("ReplayServiceTooMuchMemoryNoDumpException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				memoryUsageInMib,
				maximumMemoryUsageInMib,
				enableWatsonRegKey
			});
		}

		public static LocalizedString NetworkNameNotFound(string netName)
		{
			return new LocalizedString("NetworkNameNotFound", "Ex6D44A7", false, true, ReplayStrings.ResourceManager, new object[]
			{
				netName
			});
		}

		public static LocalizedString AmMountCallbackFailedWithDBFolderNotUnderMountPointException(string dbName, string error)
		{
			return new LocalizedString("AmMountCallbackFailedWithDBFolderNotUnderMountPointException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				error
			});
		}

		public static LocalizedString MonitoringADInitNotCompleteException
		{
			get
			{
				return new LocalizedString("MonitoringADInitNotCompleteException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedFailedToFindVolumeName
		{
			get
			{
				return new LocalizedString("AutoReseedFailedToFindVolumeName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegistryParameterReadException(string valueName, string errMsg)
		{
			return new LocalizedString("RegistryParameterReadException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				valueName,
				errMsg
			});
		}

		public static LocalizedString IncrementalReseedFailedException(string msg, uint error)
		{
			return new LocalizedString("IncrementalReseedFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				msg,
				error
			});
		}

		public static LocalizedString DagTaskComponentManagerAnotherInstanceRunning
		{
			get
			{
				return new LocalizedString("DagTaskComponentManagerAnotherInstanceRunning", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedManualReseedLaunched
		{
			get
			{
				return new LocalizedString("AutoReseedManualReseedLaunched", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToNotifySourceLogTrunc(string dbSrc, uint hresult, string optionalFriendlyError)
		{
			return new LocalizedString("FailedToNotifySourceLogTrunc", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbSrc,
				hresult,
				optionalFriendlyError
			});
		}

		public static LocalizedString FileCheckInvalidDatabaseState(string database, string state)
		{
			return new LocalizedString("FileCheckInvalidDatabaseState", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				state
			});
		}

		public static LocalizedString SeederFailedToDeleteCheckpoint(string file, string error)
		{
			return new LocalizedString("SeederFailedToDeleteCheckpoint", "Ex71D111", false, true, ReplayStrings.ResourceManager, new object[]
			{
				file,
				error
			});
		}

		public static LocalizedString SeederEcNoOnlineEdb
		{
			get
			{
				return new LocalizedString("SeederEcNoOnlineEdb", "ExC73175", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogRepairNotPossibleActiveIsDivergent(string activeServerName)
		{
			return new LocalizedString("LogRepairNotPossibleActiveIsDivergent", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				activeServerName
			});
		}

		public static LocalizedString FullServerSeedInProgressException
		{
			get
			{
				return new LocalizedString("FullServerSeedInProgressException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToOpenBackupFileHandle(string databaseSource, string serverSrc, int ec, string errorMessage)
		{
			return new LocalizedString("FailedToOpenBackupFileHandle", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseSource,
				serverSrc,
				ec,
				errorMessage
			});
		}

		public static LocalizedString ReplayServiceSuspendCommentException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendCommentException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteChkptReasonTooFarBehindAndLogMissing(long checkpointGeneration)
		{
			return new LocalizedString("DeleteChkptReasonTooFarBehindAndLogMissing", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				checkpointGeneration
			});
		}

		public static LocalizedString NetworkNotUsable(string netName, string nodeName, string reason)
		{
			return new LocalizedString("NetworkNotUsable", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				netName,
				nodeName,
				reason
			});
		}

		public static LocalizedString AutoReseedWorkflowNotSupportedOnTPR(string dagName)
		{
			return new LocalizedString("AutoReseedWorkflowNotSupportedOnTPR", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString ReplayLagRpcUnsupportedException(string serverName, string serverVersion, string supportedVersion)
		{
			return new LocalizedString("ReplayLagRpcUnsupportedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				supportedVersion
			});
		}

		public static LocalizedString RepairStateDatabaseShouldBeDismounted(string dbName, string mountedServer)
		{
			return new LocalizedString("RepairStateDatabaseShouldBeDismounted", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				mountedServer
			});
		}

		public static LocalizedString RegistryParameterKeyNotOpenedException(string keyName)
		{
			return new LocalizedString("RegistryParameterKeyNotOpenedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString DagTaskServerException(string errorMessage)
		{
			return new LocalizedString("DagTaskServerException", "ExC9C315", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReplayServiceRpcCopyStatusTimeoutException(string dbCopyName, int timeoutSecs)
		{
			return new LocalizedString("ReplayServiceRpcCopyStatusTimeoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopyName,
				timeoutSecs
			});
		}

		public static LocalizedString AutoReseedTooManyFailedCopies(int numFailedCopies)
		{
			return new LocalizedString("AutoReseedTooManyFailedCopies", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				numFailedCopies
			});
		}

		public static LocalizedString CiSeederCatalogCouldNotDismount(string ex)
		{
			return new LocalizedString("CiSeederCatalogCouldNotDismount", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString CancelSeedingDueToFailed(string id, string machine)
		{
			return new LocalizedString("CancelSeedingDueToFailed", "Ex6D1C26", false, true, ReplayStrings.ResourceManager, new object[]
			{
				id,
				machine
			});
		}

		public static LocalizedString IOBufferPoolLimitError(int limit, int bufSize)
		{
			return new LocalizedString("IOBufferPoolLimitError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				limit,
				bufSize
			});
		}

		public static LocalizedString AutoReseedInPlaceReseedTooSoon(string failedElapsedTimeStr, string inPlaceDelayTimeString)
		{
			return new LocalizedString("AutoReseedInPlaceReseedTooSoon", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				failedElapsedTimeStr,
				inPlaceDelayTimeString
			});
		}

		public static LocalizedString AmBcsDagNotFoundInAd(string server, string adError)
		{
			return new LocalizedString("AmBcsDagNotFoundInAd", "Ex7E61AD", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				adError
			});
		}

		public static LocalizedString AmBcsDatabaseCopyResynchronizing(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyResynchronizing", "Ex1B8D35", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString FoundTooManyVolumesWithSameVolumeLabelException(string volumeNames, string volumeLabel)
		{
			return new LocalizedString("FoundTooManyVolumesWithSameVolumeLabelException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeNames,
				volumeLabel
			});
		}

		public static LocalizedString AmDbMoveOperationOnTimeoutFailureCancelled(string dbName, string fromServer)
		{
			return new LocalizedString("AmDbMoveOperationOnTimeoutFailureCancelled", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				fromServer
			});
		}

		public static LocalizedString DbFixupFailedVolumeHasMaxDbMountPointsException(string dbName, string volumeName)
		{
			return new LocalizedString("DbFixupFailedVolumeHasMaxDbMountPointsException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				volumeName
			});
		}

		public static LocalizedString MissingActiveLogRequiredForDivergenceDetection(string file, string sourceServer)
		{
			return new LocalizedString("MissingActiveLogRequiredForDivergenceDetection", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				sourceServer
			});
		}

		public static LocalizedString InvalidDbForSeedSpecifiedException
		{
			get
			{
				return new LocalizedString("InvalidDbForSeedSpecifiedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterBatchWriter_FailedToReadClusterRegistry(string msg)
		{
			return new LocalizedString("ClusterBatchWriter_FailedToReadClusterRegistry", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString AutoReseedCatalogSkipRebuild(string databaseName, string serverName)
		{
			return new LocalizedString("AutoReseedCatalogSkipRebuild", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString SeederInstanceAlreadyInProgressException(string sourceMachine)
		{
			return new LocalizedString("SeederInstanceAlreadyInProgressException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceMachine
			});
		}

		public static LocalizedString FileCheckUnableToDeleteCheckpointError(string file, string errorMessage)
		{
			return new LocalizedString("FileCheckUnableToDeleteCheckpointError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				errorMessage
			});
		}

		public static LocalizedString AcllInvalidForSingleCopyException(string dbCopy)
		{
			return new LocalizedString("AcllInvalidForSingleCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString VolumeMountPathDoesNotExistException
		{
			get
			{
				return new LocalizedString("VolumeMountPathDoesNotExistException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CiSeederRpcOperationFailedException(string errMessage)
		{
			return new LocalizedString("CiSeederRpcOperationFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString MonitoringADConfigStaleException(string age, string maxTTL, string lastError)
		{
			return new LocalizedString("MonitoringADConfigStaleException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				age,
				maxTTL,
				lastError
			});
		}

		public static LocalizedString AmDbOperationWaitFailedException
		{
			get
			{
				return new LocalizedString("AmDbOperationWaitFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EseutilParseError(string line, string regex)
		{
			return new LocalizedString("EseutilParseError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				line,
				regex
			});
		}

		public static LocalizedString ErrorAmConfigNotInitialized
		{
			get
			{
				return new LocalizedString("ErrorAmConfigNotInitialized", "Ex6447E3", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmClusterEvictWithoutCleanupException(string nodeName)
		{
			return new LocalizedString("AmClusterEvictWithoutCleanupException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString FileSystemCorruptionDetected(string filePath)
		{
			return new LocalizedString("FileSystemCorruptionDetected", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				filePath
			});
		}

		public static LocalizedString DbValidationPassiveCopyUnhealthyState(string dbCopyName, string copyStatus, string errorMessage, string suspendComment)
		{
			return new LocalizedString("DbValidationPassiveCopyUnhealthyState", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopyName,
				copyStatus,
				errorMessage,
				suspendComment
			});
		}

		public static LocalizedString LogCopierE00MissingPrevious(long e00Gen, string filename)
		{
			return new LocalizedString("LogCopierE00MissingPrevious", "Ex87ED89", false, true, ReplayStrings.ResourceManager, new object[]
			{
				e00Gen,
				filename
			});
		}

		public static LocalizedString LogDriveNotBigEnough(string path, long expectedSize, ulong actualSize)
		{
			return new LocalizedString("LogDriveNotBigEnough", "Ex40DD54", false, true, ReplayStrings.ResourceManager, new object[]
			{
				path,
				expectedSize,
				actualSize
			});
		}

		public static LocalizedString DagTaskAddingServerToDag(string serverName, string dagName)
		{
			return new LocalizedString("DagTaskAddingServerToDag", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				dagName
			});
		}

		public static LocalizedString NetworkCorruptDataGeneric
		{
			get
			{
				return new LocalizedString("NetworkCorruptDataGeneric", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseNotFound(Guid dbGuid)
		{
			return new LocalizedString("DatabaseNotFound", "ExEBD8A9", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString ErrorAutomountConsensusNotReached
		{
			get
			{
				return new LocalizedString("ErrorAutomountConsensusNotReached", "Ex3569DC", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsDatabaseCopyFailed(string db, string server, string failedMessage)
		{
			return new LocalizedString("AmBcsDatabaseCopyFailed", "ExB5E79C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				failedMessage
			});
		}

		public static LocalizedString AmDbRemountSkippedSinceMasterChanged(string dbName, string currentActive, string prevActive)
		{
			return new LocalizedString("AmDbRemountSkippedSinceMasterChanged", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				currentActive,
				prevActive
			});
		}

		public static LocalizedString AutoReseedNoExchangeVolumesConfigured(string exchangeVolumeRootPath)
		{
			return new LocalizedString("AutoReseedNoExchangeVolumesConfigured", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				exchangeVolumeRootPath
			});
		}

		public static LocalizedString AmDatabaseNameNotFoundException(string dbName)
		{
			return new LocalizedString("AmDatabaseNameNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString AmBcsNoneSpecified
		{
			get
			{
				return new LocalizedString("AmBcsNoneSpecified", "Ex621F21", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoInstancesFoundForManagementPath(string path)
		{
			return new LocalizedString("NoInstancesFoundForManagementPath", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString LastLogReplacementFailedUnexpectedFileFoundException(string dbCopy, string unexpectedFile, string e00log)
		{
			return new LocalizedString("LastLogReplacementFailedUnexpectedFileFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				unexpectedFile,
				e00log
			});
		}

		public static LocalizedString ExchangeVolumeInfoMultipleDbMountPointsException(string volumeName, string dbVolRootPath, string dbMountPoints, int maxDbs)
		{
			return new LocalizedString("ExchangeVolumeInfoMultipleDbMountPointsException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				dbVolRootPath,
				dbMountPoints,
				maxDbs
			});
		}

		public static LocalizedString SeederEcNotEnoughDiskException
		{
			get
			{
				return new LocalizedString("SeederEcNotEnoughDiskException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkFailedToAuthServer
		{
			get
			{
				return new LocalizedString("NetworkFailedToAuthServer", "ExE3255A", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFailedToFindLocalServer
		{
			get
			{
				return new LocalizedString("ErrorFailedToFindLocalServer", "ExDA9608", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringServerSiteIsNullException(string serverName)
		{
			return new LocalizedString("MonitoringServerSiteIsNullException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString AmServiceMonitorSystemShutdownException(string serviceName)
		{
			return new LocalizedString("AmServiceMonitorSystemShutdownException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serviceName
			});
		}

		public static LocalizedString DatabaseFailedToGetVolumeInfo(string name)
		{
			return new LocalizedString("DatabaseFailedToGetVolumeInfo", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LogRepairFailedTransient(string reason)
		{
			return new LocalizedString("LogRepairFailedTransient", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AcllCopyStatusInvalidException(string dbCopy, string status)
		{
			return new LocalizedString("AcllCopyStatusInvalidException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				status
			});
		}

		public static LocalizedString DatabaseGroupNotSetException(string databaseName)
		{
			return new LocalizedString("DatabaseGroupNotSetException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString FailedToOpenShipLogContextDatabaseNotMounted
		{
			get
			{
				return new LocalizedString("FailedToOpenShipLogContextDatabaseNotMounted", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceSuspendBlockedAcllException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendBlockedAcllException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AcllBackupInProgressException(string dbCopy)
		{
			return new LocalizedString("AcllBackupInProgressException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString AmDbMoveOperationNoLongerApplicableException(string dbName, string fromServer, string activeServer)
		{
			return new LocalizedString("AmDbMoveOperationNoLongerApplicableException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				fromServer,
				activeServer
			});
		}

		public static LocalizedString SeederSuspendFailedException(string specificError)
		{
			return new LocalizedString("SeederSuspendFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				specificError
			});
		}

		public static LocalizedString CouldNotCreateDbMountPointFolderException(string database, string dbMountPoint, string errMsg)
		{
			return new LocalizedString("CouldNotCreateDbMountPointFolderException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				dbMountPoint,
				errMsg
			});
		}

		public static LocalizedString DatabaseCopyLayoutTableNullException
		{
			get
			{
				return new LocalizedString("DatabaseCopyLayoutTableNullException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteRegistryTimedOutException(string machineName, int secondsTimeout)
		{
			return new LocalizedString("RemoteRegistryTimedOutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				machineName,
				secondsTimeout
			});
		}

		public static LocalizedString AmServiceShuttingDown
		{
			get
			{
				return new LocalizedString("AmServiceShuttingDown", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringCouldNotFindDatabasesException(string serverName, string adError)
		{
			return new LocalizedString("MonitoringCouldNotFindDatabasesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				adError
			});
		}

		public static LocalizedString TargetDBAlreadyExists(string edbFile)
		{
			return new LocalizedString("TargetDBAlreadyExists", "Ex8F2543", false, true, ReplayStrings.ResourceManager, new object[]
			{
				edbFile
			});
		}

		public static LocalizedString SeederCatalogNotHealthyErr(string reason)
		{
			return new LocalizedString("SeederCatalogNotHealthyErr", "ExA4EA78", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ReplayServiceUnknownReplicaInstanceException(string operationName, string db)
		{
			return new LocalizedString("ReplayServiceUnknownReplicaInstanceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName,
				db
			});
		}

		public static LocalizedString PagePatchFileReadException(string fileName, long actualBytesRead, long expectedBytesRead)
		{
			return new LocalizedString("PagePatchFileReadException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				fileName,
				actualBytesRead,
				expectedBytesRead
			});
		}

		public static LocalizedString SeedingChannelIsClosedException(Guid g)
		{
			return new LocalizedString("SeedingChannelIsClosedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				g
			});
		}

		public static LocalizedString ReplayDatabaseOperationTimedoutException(string operationName, string db, TimeSpan timeout)
		{
			return new LocalizedString("ReplayDatabaseOperationTimedoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName,
				db,
				timeout
			});
		}

		public static LocalizedString AutoReseedFailedSeedRetryExceeded(int maxRetryCount)
		{
			return new LocalizedString("AutoReseedFailedSeedRetryExceeded", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				maxRetryCount
			});
		}

		public static LocalizedString AcllLastLogTimeErrorException(string dbCopy, string logfilePath, string err)
		{
			return new LocalizedString("AcllLastLogTimeErrorException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				logfilePath,
				err
			});
		}

		public static LocalizedString SeederOperationFailedWithEcException(int ec, string errMessage)
		{
			return new LocalizedString("SeederOperationFailedWithEcException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec,
				errMessage
			});
		}

		public static LocalizedString VolumeCouldNotBeReclaimedException(string volumeName, string mountPoint)
		{
			return new LocalizedString("VolumeCouldNotBeReclaimedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				mountPoint
			});
		}

		public static LocalizedString DbValidationNotEnoughCopies(string dbName)
		{
			return new LocalizedString("DbValidationNotEnoughCopies", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString DagTaskDagIpAddressesMustBeIpv4Exception
		{
			get
			{
				return new LocalizedString("DagTaskDagIpAddressesMustBeIpv4Exception", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RepairStateFailedToCreateTempLogFile(string dbName, string errorMsg)
		{
			return new LocalizedString("RepairStateFailedToCreateTempLogFile", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				errorMsg
			});
		}

		public static LocalizedString DagTaskRemoveDagServerMustHaveQuorumException(string dagName)
		{
			return new LocalizedString("DagTaskRemoveDagServerMustHaveQuorumException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString AcllFailedCurrentLogPresent(string dbCopy, string e00logfile, string sourceServer)
		{
			return new LocalizedString("AcllFailedCurrentLogPresent", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				e00logfile,
				sourceServer
			});
		}

		public static LocalizedString AutoReseedFailedToFindTargetVolumeName(string volumeInfoErr)
		{
			return new LocalizedString("AutoReseedFailedToFindTargetVolumeName", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeInfoErr
			});
		}

		public static LocalizedString AmBcsTargetServerIsHAComponentOffline(string server)
		{
			return new LocalizedString("AmBcsTargetServerIsHAComponentOffline", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UnknownError
		{
			get
			{
				return new LocalizedString("UnknownError", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoDivergedPointFound(string dbCopy, string sourceServer)
		{
			return new LocalizedString("NoDivergedPointFound", "Ex4229B4", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				sourceServer
			});
		}

		public static LocalizedString SeederRpcServerLevelUnsupportedException(string serverName, string serverVersion, string supportedVersion)
		{
			return new LocalizedString("SeederRpcServerLevelUnsupportedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				supportedVersion
			});
		}

		public static LocalizedString CouldNotCreateDbMountPointException(string database, string dbMountPoint, string volumeName, string errMsg)
		{
			return new LocalizedString("CouldNotCreateDbMountPointException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				dbMountPoint,
				volumeName,
				errMsg
			});
		}

		public static LocalizedString FailedToConfigureMountPointException(string volumeName, string reason)
		{
			return new LocalizedString("FailedToConfigureMountPointException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				reason
			});
		}

		public static LocalizedString AmDismountSucceededButStillMounted(string serverName, string dbName)
		{
			return new LocalizedString("AmDismountSucceededButStillMounted", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				dbName
			});
		}

		public static LocalizedString EseBackFileSystemCorruption
		{
			get
			{
				return new LocalizedString("EseBackFileSystemCorruption", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuspendOperationName
		{
			get
			{
				return new LocalizedString("SuspendOperationName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PagePatchLegacyFileExistsException
		{
			get
			{
				return new LocalizedString("PagePatchLegacyFileExistsException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogCopierInitFailedBecauseNoLogsOnSource(string srcServer)
		{
			return new LocalizedString("LogCopierInitFailedBecauseNoLogsOnSource", "Ex3D0F10", false, true, ReplayStrings.ResourceManager, new object[]
			{
				srcServer
			});
		}

		public static LocalizedString FailureItemRecoveryFailed(string dbName, string msg)
		{
			return new LocalizedString("FailureItemRecoveryFailed", "Ex1DA2B7", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				msg
			});
		}

		public static LocalizedString AcllCopyStatusFailedException(string dbCopy, string status, string errorMsg)
		{
			return new LocalizedString("AcllCopyStatusFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				status,
				errorMsg
			});
		}

		public static LocalizedString LastLogReplacementTooManyTempFilesException(string dbCopy, string filter, int count, string logPath)
		{
			return new LocalizedString("LastLogReplacementTooManyTempFilesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				filter,
				count,
				logPath
			});
		}

		public static LocalizedString SeederEcSuccess
		{
			get
			{
				return new LocalizedString("SeederEcSuccess", "Ex9395B4", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsTargetServerActivationDisabled(string server)
		{
			return new LocalizedString("AmBcsTargetServerActivationDisabled", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ManagementApiError(string api)
		{
			return new LocalizedString("ManagementApiError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				api
			});
		}

		public static LocalizedString NoServices
		{
			get
			{
				return new LocalizedString("NoServices", "Ex5154B4", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KernelWatchdogTimerError(string msg)
		{
			return new LocalizedString("KernelWatchdogTimerError", "Ex4842FB", false, true, ReplayStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString AmDbActionTransientException(string actionError)
		{
			return new LocalizedString("AmDbActionTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionError
			});
		}

		public static LocalizedString AmBcsActiveCopyIsSeedingSource(string db, string server)
		{
			return new LocalizedString("AmBcsActiveCopyIsSeedingSource", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString NetworkAddressResolutionFailedNoDnsEntry(string nodeName)
		{
			return new LocalizedString("NetworkAddressResolutionFailedNoDnsEntry", "Ex496595", false, true, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString AmBcsDatabaseCopyInitializing(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyInitializing", "Ex1C33B4", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString RepairStateFailedPendingPagePatchException(string dbName, string errorMsg)
		{
			return new LocalizedString("RepairStateFailedPendingPagePatchException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				errorMsg
			});
		}

		public static LocalizedString NetworkTimeoutError(string remoteNodeName, string errorText)
		{
			return new LocalizedString("NetworkTimeoutError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				remoteNodeName,
				errorText
			});
		}

		public static LocalizedString StoreServiceMonitorCriticalError
		{
			get
			{
				return new LocalizedString("StoreServiceMonitorCriticalError", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotFindVolumeException(string volumeName)
		{
			return new LocalizedString("CouldNotFindVolumeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName
			});
		}

		public static LocalizedString AmCommonException(string error)
		{
			return new LocalizedString("AmCommonException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DagTaskRemoteOperationLogEnd(string serverName)
		{
			return new LocalizedString("DagTaskRemoteOperationLogEnd", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString RepairStateDatabaseIsActive(string dbName)
		{
			return new LocalizedString("RepairStateDatabaseIsActive", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString DirectoryEnumeratorIOError(string apiName, string ioErrorMessage, int win32ErrCode, string directoryName)
		{
			return new LocalizedString("DirectoryEnumeratorIOError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				apiName,
				ioErrorMessage,
				win32ErrCode,
				directoryName
			});
		}

		public static LocalizedString NetworkTransportError(string err)
		{
			return new LocalizedString("NetworkTransportError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				err
			});
		}

		public static LocalizedString DagTaskMovedPam(string newPam)
		{
			return new LocalizedString("DagTaskMovedPam", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				newPam
			});
		}

		public static LocalizedString LogInspectorE00OutOfSequence(string logfileInspected, long genFromHeader, long highestGenPresent)
		{
			return new LocalizedString("LogInspectorE00OutOfSequence", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfileInspected,
				genFromHeader,
				highestGenPresent
			});
		}

		public static LocalizedString LogCopierInitFailedActiveTruncatingException(string srcServer, long startingLogGen, long srcLowestGen)
		{
			return new LocalizedString("LogCopierInitFailedActiveTruncatingException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				srcServer,
				startingLogGen,
				srcLowestGen
			});
		}

		public static LocalizedString CannotChangeProperties
		{
			get
			{
				return new LocalizedString("CannotChangeProperties", "Ex939705", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmFailedToDetermineDatabaseMountStatus(string serverName, string dbName)
		{
			return new LocalizedString("AmFailedToDetermineDatabaseMountStatus", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				dbName
			});
		}

		public static LocalizedString ReplayFailedToFindServerRpcVersionException(string serverName)
		{
			return new LocalizedString("ReplayFailedToFindServerRpcVersionException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString AmBcsDatabaseCopyActivationSuspended(string db, string server, string reason)
		{
			return new LocalizedString("AmBcsDatabaseCopyActivationSuspended", "Ex80D1AB", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				reason
			});
		}

		public static LocalizedString IncrementalReseedRetryableException(string error)
		{
			return new LocalizedString("IncrementalReseedRetryableException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AmMountBlockedOnStandaloneDbWithMissingEdbException(string dbName, long highestLogGen, string edbFilePath)
		{
			return new LocalizedString("AmMountBlockedOnStandaloneDbWithMissingEdbException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				highestLogGen,
				edbFilePath
			});
		}

		public static LocalizedString SeederInstanceAlreadyCompletedException(string sourceMachine)
		{
			return new LocalizedString("SeederInstanceAlreadyCompletedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceMachine
			});
		}

		public static LocalizedString CouldNotFindDagObjectLookupErrorForServer(string serverName, string error)
		{
			return new LocalizedString("CouldNotFindDagObjectLookupErrorForServer", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				error
			});
		}

		public static LocalizedString ServiceName(string name, string length)
		{
			return new LocalizedString("ServiceName", "ExA57B32", false, true, ReplayStrings.ResourceManager, new object[]
			{
				name,
				length
			});
		}

		public static LocalizedString AmClusterNodeNotFoundException(string nodeName)
		{
			return new LocalizedString("AmClusterNodeNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString AutoReseedFailedResumeBlocked(string error)
		{
			return new LocalizedString("AutoReseedFailedResumeBlocked", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString SeederEchrInvalidCallSequence
		{
			get
			{
				return new LocalizedString("SeederEchrInvalidCallSequence", "Ex87FBE8", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederFailedToFindValidVolumeInfo(string name, string error)
		{
			return new LocalizedString("SeederFailedToFindValidVolumeInfo", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString FailedToOpenShipLogContextEseCircularLoggingEnabled
		{
			get
			{
				return new LocalizedString("FailedToOpenShipLogContextEseCircularLoggingEnabled", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederEcJtxAlreadyExist(string directory)
		{
			return new LocalizedString("SeederEcJtxAlreadyExist", "Ex1CD4A4", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString SeederFailedToDeleteDatabase(string file, string error)
		{
			return new LocalizedString("SeederFailedToDeleteDatabase", "Ex9D8082", false, true, ReplayStrings.ResourceManager, new object[]
			{
				file,
				error
			});
		}

		public static LocalizedString ReplayServiceResumeRpcFailedException
		{
			get
			{
				return new LocalizedString("ReplayServiceResumeRpcFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskComponentManagerServerManagerCmdFailure(string error)
		{
			return new LocalizedString("DagTaskComponentManagerServerManagerCmdFailure", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString FailedToGetDiskSpace(string path, string systemErrMessage)
		{
			return new LocalizedString("FailedToGetDiskSpace", "ExAE22D9", false, true, ReplayStrings.ResourceManager, new object[]
			{
				path,
				systemErrMessage
			});
		}

		public static LocalizedString NetworkSecurityFailed
		{
			get
			{
				return new LocalizedString("NetworkSecurityFailed", "Ex80869A", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbRedundancyValidationErrorsOccurred(string dbName, int healthyCopiesCount, int expectedHealthyCount, string detailedMsg)
		{
			return new LocalizedString("DbRedundancyValidationErrorsOccurred", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				healthyCopiesCount,
				expectedHealthyCount,
				detailedMsg
			});
		}

		public static LocalizedString AcllUnboundedDatalossDetectedException(string dbName, string lastUpdatedTimeStr, string allowedDurationStr, string actualDurationStr)
		{
			return new LocalizedString("AcllUnboundedDatalossDetectedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				lastUpdatedTimeStr,
				allowedDurationStr,
				actualDurationStr
			});
		}

		public static LocalizedString ClusterBatchWriter_BatchAddCommandFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_BatchAddCommandFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString ReplayServiceSuspendRpcFailedException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendRpcFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuspendWantedWriteFailedException
		{
			get
			{
				return new LocalizedString("SuspendWantedWriteFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeedingSourceReplicaInstanceNotFoundException(Guid guid, string sourceServer)
		{
			return new LocalizedString("SeedingSourceReplicaInstanceNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				guid,
				sourceServer
			});
		}

		public static LocalizedString AmBcsDatabaseCopySuspended(string db, string server, string reason)
		{
			return new LocalizedString("AmBcsDatabaseCopySuspended", "ExF2C537", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				reason
			});
		}

		public static LocalizedString FileCheckLogfileCreationTime(string logfile, DateTime previousGenerationCreationTime, DateTime previousGenerationCreationTimeActual)
		{
			return new LocalizedString("FileCheckLogfileCreationTime", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile,
				previousGenerationCreationTime,
				previousGenerationCreationTimeActual
			});
		}

		public static LocalizedString ReplayServiceSuspendResumeBlockedException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendResumeBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbActionDismountFailedException
		{
			get
			{
				return new LocalizedString("AmDbActionDismountFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedNeverMountedWorkflowReason
		{
			get
			{
				return new LocalizedString("AutoReseedNeverMountedWorkflowReason", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedLogAndDbNotOnSameVolume
		{
			get
			{
				return new LocalizedString("AutoReseedLogAndDbNotOnSameVolume", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FullServerSeedSkippedShutdownException
		{
			get
			{
				return new LocalizedString("FullServerSeedSkippedShutdownException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayLagManagerException(string errorMsg)
		{
			return new LocalizedString("ReplayLagManagerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString DatabaseVolumeInfoInitException(string databaseCopy, string errMsg)
		{
			return new LocalizedString("DatabaseVolumeInfoInitException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseCopy,
				errMsg
			});
		}

		public static LocalizedString ReplayDbOperationTransientException(string opError)
		{
			return new LocalizedString("ReplayDbOperationTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				opError
			});
		}

		public static LocalizedString VolumeFormatFailedException(string volumeName, string mountPoint, string err)
		{
			return new LocalizedString("VolumeFormatFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				mountPoint,
				err
			});
		}

		public static LocalizedString SeederEcOverlappedWriteErr
		{
			get
			{
				return new LocalizedString("SeederEcOverlappedWriteErr", "ExCEBAF4", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedNoCopiesException(string databaseName)
		{
			return new LocalizedString("AutoReseedNoCopiesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ReplayDatabaseOperationCancelledException(string operationName, string db)
		{
			return new LocalizedString("ReplayDatabaseOperationCancelledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName,
				db
			});
		}

		public static LocalizedString CiStatusIsFailed(string server, string db)
		{
			return new LocalizedString("CiStatusIsFailed", "Ex650952", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				db
			});
		}

		public static LocalizedString NetworkEndOfData(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkEndOfData", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString DbAvailabilityActiveCopyDismountedError(string dbName, string serverName, string error)
		{
			return new LocalizedString("DbAvailabilityActiveCopyDismountedError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				error
			});
		}

		public static LocalizedString FailToCleanUpFiles
		{
			get
			{
				return new LocalizedString("FailToCleanUpFiles", "Ex1AF8B4", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkReadTimeout(int waitInsecs)
		{
			return new LocalizedString("NetworkReadTimeout", "ExC36360", false, true, ReplayStrings.ResourceManager, new object[]
			{
				waitInsecs
			});
		}

		public static LocalizedString CiSeederSearchCatalogException(string sourceServer, Guid database, string specificError)
		{
			return new LocalizedString("CiSeederSearchCatalogException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceServer,
				database,
				specificError
			});
		}

		public static LocalizedString TPRExchangeNotListening(string reason)
		{
			return new LocalizedString("TPRExchangeNotListening", "ExDACB39", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AmClusterEventNotifierTransientException(int errCode)
		{
			return new LocalizedString("AmClusterEventNotifierTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errCode
			});
		}

		public static LocalizedString MissingPassiveLogRequiredForDivergenceDetection(string file)
		{
			return new LocalizedString("MissingPassiveLogRequiredForDivergenceDetection", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString AmBcsTargetServerPreferredMaxActivesExceeded(string server, string maxActiveDatabases)
		{
			return new LocalizedString("AmBcsTargetServerPreferredMaxActivesExceeded", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				maxActiveDatabases
			});
		}

		public static LocalizedString AmClusterNodeJoinedException(string nodeName)
		{
			return new LocalizedString("AmClusterNodeJoinedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString AmBcsTargetServerPreferredMaxActivesReached(string server, string maxActiveDatabases)
		{
			return new LocalizedString("AmBcsTargetServerPreferredMaxActivesReached", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				maxActiveDatabases
			});
		}

		public static LocalizedString AmDbLockConflict(Guid dbGuid, string reqReason, string ownerReason)
		{
			return new LocalizedString("AmDbLockConflict", "Ex0F5D8B", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbGuid,
				reqReason,
				ownerReason
			});
		}

		public static LocalizedString FileCheck(string error)
		{
			return new LocalizedString("FileCheck", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString LogInspectorGenerationMismatch(string logfileInspected, long genFromHeader, long genFromFileName)
		{
			return new LocalizedString("LogInspectorGenerationMismatch", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfileInspected,
				genFromHeader,
				genFromFileName
			});
		}

		public static LocalizedString SeederInstanceNotFoundException(string dbGuid)
		{
			return new LocalizedString("SeederInstanceNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString FailedToOpenLogTruncContext(string dbSrc, uint hresult, string optionalFriendlyError)
		{
			return new LocalizedString("FailedToOpenLogTruncContext", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbSrc,
				hresult,
				optionalFriendlyError
			});
		}

		public static LocalizedString SeederEcNotEnoughDisk
		{
			get
			{
				return new LocalizedString("SeederEcNotEnoughDisk", "ExF8A360", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskOperationFailedWithEcException(int ec)
		{
			return new LocalizedString("DagTaskOperationFailedWithEcException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString DagTaskServerTransientException(string errorMessage)
		{
			return new LocalizedString("DagTaskServerTransientException", "Ex5C50F1", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReplayServiceSuspendRpcPartialSuccessCatalogFailedException(string errMsg)
		{
			return new LocalizedString("ReplayServiceSuspendRpcPartialSuccessCatalogFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString PathIsAlreadyAValidMountPoint(string path)
		{
			return new LocalizedString("PathIsAlreadyAValidMountPoint", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString MonitoringADServiceShuttingDownException
		{
			get
			{
				return new LocalizedString("MonitoringADServiceShuttingDownException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbHTServiceShuttingDownException
		{
			get
			{
				return new LocalizedString("DbHTServiceShuttingDownException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmLastLogPropertyCorruptedException(string property, string corruptedValue)
		{
			return new LocalizedString("AmLastLogPropertyCorruptedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				property,
				corruptedValue
			});
		}

		public static LocalizedString CouldNotFindDagObjectForServer(string serverName)
		{
			return new LocalizedString("CouldNotFindDagObjectForServer", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString TagHandlerFormatMsgFailed(uint msgId)
		{
			return new LocalizedString("TagHandlerFormatMsgFailed", "ExCFCA21", false, true, ReplayStrings.ResourceManager, new object[]
			{
				msgId
			});
		}

		public static LocalizedString IncrementalReseedPrereqException(string error)
		{
			return new LocalizedString("IncrementalReseedPrereqException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString NullDbCopyException
		{
			get
			{
				return new LocalizedString("NullDbCopyException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorClusterServiceNotRunningForAmConfig
		{
			get
			{
				return new LocalizedString("ErrorClusterServiceNotRunningForAmConfig", "Ex576E35", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoredDatabaseInitFailure(string dbName, string error)
		{
			return new LocalizedString("MonitoredDatabaseInitFailure", "Ex3B2197", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				error
			});
		}

		public static LocalizedString AmBcsTargetServerActivationBlocked(string server)
		{
			return new LocalizedString("AmBcsTargetServerActivationBlocked", "ExC2D6D2", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString RepairStateDatabaseCopyShouldBeSuspended(string dbName)
		{
			return new LocalizedString("RepairStateDatabaseCopyShouldBeSuspended", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString InvalidRcrConfigAlreadyHostsDb(string nodeName, string dbName)
		{
			return new LocalizedString("InvalidRcrConfigAlreadyHostsDb", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				dbName
			});
		}

		public static LocalizedString RegistryParameterException(string errorMsg)
		{
			return new LocalizedString("RegistryParameterException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString FailedToDeserializeDumpsterRequestStrException(string dbName, string stringToDeserialize, string typeName, string serializationError)
		{
			return new LocalizedString("FailedToDeserializeDumpsterRequestStrException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				stringToDeserialize,
				typeName,
				serializationError
			});
		}

		public static LocalizedString SeederEcUndefined(int ec)
		{
			return new LocalizedString("SeederEcUndefined", "Ex82B4D6", false, true, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString CiSeederGenericException(string sourceServer, string destServer, string specificError)
		{
			return new LocalizedString("CiSeederGenericException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceServer,
				destServer,
				specificError
			});
		}

		public static LocalizedString FileCheckEDBMissing(string edbFileName)
		{
			return new LocalizedString("FileCheckEDBMissing", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				edbFileName
			});
		}

		public static LocalizedString AcllCopyIsNotViableErrorException(string dbCopy, string err)
		{
			return new LocalizedString("AcllCopyIsNotViableErrorException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				err
			});
		}

		public static LocalizedString DatabaseVolumeInfoException(string errorMsg)
		{
			return new LocalizedString("DatabaseVolumeInfoException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ReplayServiceResumeBlockedException(string previousError)
		{
			return new LocalizedString("ReplayServiceResumeBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				previousError
			});
		}

		public static LocalizedString ReplaySystemOperationTimedoutException(string operationName, TimeSpan timeout)
		{
			return new LocalizedString("ReplaySystemOperationTimedoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName,
				timeout
			});
		}

		public static LocalizedString AmBcsSourceServerADError(string server, string adError)
		{
			return new LocalizedString("AmBcsSourceServerADError", "Ex7274B9", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				adError
			});
		}

		public static LocalizedString AmDbActionWrapperTransientException(string dbActionError)
		{
			return new LocalizedString("AmDbActionWrapperTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbActionError
			});
		}

		public static LocalizedString FileCheckAccessDenied(string file)
		{
			return new LocalizedString("FileCheckAccessDenied", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString ActiveRecoveryNotApplicableException(string dbName)
		{
			return new LocalizedString("ActiveRecoveryNotApplicableException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString SeederServerException(string errorMessage)
		{
			return new LocalizedString("SeederServerException", "Ex4543D9", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString AutoReseedCatalogIsBehindRetry(int retryCount)
		{
			return new LocalizedString("AutoReseedCatalogIsBehindRetry", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				retryCount
			});
		}

		public static LocalizedString AutoReseedWrongNumberOfCopiesOnVolume(string volumeName, int numDbs, int expectedDbs)
		{
			return new LocalizedString("AutoReseedWrongNumberOfCopiesOnVolume", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				numDbs,
				expectedDbs
			});
		}

		public static LocalizedString SourceLogBreakStallsPassiveError(string sourceServerName, string error)
		{
			return new LocalizedString("SourceLogBreakStallsPassiveError", "ExB93F7E", false, true, ReplayStrings.ResourceManager, new object[]
			{
				sourceServerName,
				error
			});
		}

		public static LocalizedString TagHandlerSuspendCopy(string suspendReason)
		{
			return new LocalizedString("TagHandlerSuspendCopy", "Ex8F58E2", false, true, ReplayStrings.ResourceManager, new object[]
			{
				suspendReason
			});
		}

		public static LocalizedString TPRExchangeListenerNotResponding(string reason)
		{
			return new LocalizedString("TPRExchangeListenerNotResponding", "Ex423627", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString EnableReplayLagAlreadyDisabledFailedException(string dbCopy)
		{
			return new LocalizedString("EnableReplayLagAlreadyDisabledFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString SeederEcError
		{
			get
			{
				return new LocalizedString("SeederEcError", "Ex84746E", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncSeedDivergenceCheckFailedException(string dbName, string sourceServer, string error)
		{
			return new LocalizedString("IncSeedDivergenceCheckFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				sourceServer,
				error
			});
		}

		public static LocalizedString TPRInitFailure(string errMsg)
		{
			return new LocalizedString("TPRInitFailure", "Ex6D1D36", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString FileCheckRequiredGenerationCorrupt(string file, long min, long max)
		{
			return new LocalizedString("FileCheckRequiredGenerationCorrupt", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				min,
				max
			});
		}

		public static LocalizedString AmCommonTransientException(string error)
		{
			return new LocalizedString("AmCommonTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ClusterBatchWriter_OpenClusterFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_OpenClusterFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString CIStatusFailedException(string server, string db)
		{
			return new LocalizedString("CIStatusFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				db
			});
		}

		public static LocalizedString AutoReseedFailedAdminSuspended
		{
			get
			{
				return new LocalizedString("AutoReseedFailedAdminSuspended", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterSafetyNetRpcFailedException(string hubServerName, string rpcStatus)
		{
			return new LocalizedString("DumpsterSafetyNetRpcFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				hubServerName,
				rpcStatus
			});
		}

		public static LocalizedString NetworkNoUsableEndpoints
		{
			get
			{
				return new LocalizedString("NetworkNoUsableEndpoints", "Ex84B8D7", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbActionException(string actionError)
		{
			return new LocalizedString("AmDbActionException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionError
			});
		}

		public static LocalizedString InstanceSuspendedAutoInitialSeed(string databaseName)
		{
			return new LocalizedString("InstanceSuspendedAutoInitialSeed", "Ex0E8355", false, true, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ReseedCheckMissingLogfile(string logfile)
		{
			return new LocalizedString("ReseedCheckMissingLogfile", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile
			});
		}

		public static LocalizedString LogCopierFailsBecauseLogGap(string srcServer, string missingFileName)
		{
			return new LocalizedString("LogCopierFailsBecauseLogGap", "Ex3EA540", false, true, ReplayStrings.ResourceManager, new object[]
			{
				srcServer,
				missingFileName
			});
		}

		public static LocalizedString LogDirectoryCreationDisabled(string directoryPath)
		{
			return new LocalizedString("LogDirectoryCreationDisabled", "ExE59C1D", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directoryPath
			});
		}

		public static LocalizedString DbAvailabilityActiveCopyMountState(string dbName, string serverName, string copyStatus)
		{
			return new LocalizedString("DbAvailabilityActiveCopyMountState", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				copyStatus
			});
		}

		public static LocalizedString LogGapDetected(string file)
		{
			return new LocalizedString("LogGapDetected", "ExF43290", false, true, ReplayStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString ClusterServiceMonitorCriticalError
		{
			get
			{
				return new LocalizedString("ClusterServiceMonitorCriticalError", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileCheckEseutilError(string errorMessage)
		{
			return new LocalizedString("FileCheckEseutilError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString Resynchronizing
		{
			get
			{
				return new LocalizedString("Resynchronizing", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterBatchWriter_CreateBatchFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_CreateBatchFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString AmRefreshConfigTimeoutError(int timeoutSecs)
		{
			return new LocalizedString("AmRefreshConfigTimeoutError", "Ex80A46E", false, true, ReplayStrings.ResourceManager, new object[]
			{
				timeoutSecs
			});
		}

		public static LocalizedString AmDbOperationException(string opError)
		{
			return new LocalizedString("AmDbOperationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				opError
			});
		}

		public static LocalizedString DbCopyNotTargetException(string dbName, string serverName)
		{
			return new LocalizedString("DbCopyNotTargetException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName
			});
		}

		public static LocalizedString LogRepairDivergenceCheckFailedDueToCorruptEndOfLog(string logName, string exceptionText)
		{
			return new LocalizedString("LogRepairDivergenceCheckFailedDueToCorruptEndOfLog", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logName,
				exceptionText
			});
		}

		public static LocalizedString ReplayServiceSuspendBlockedResynchronizingException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendBlockedResynchronizingException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsDatabaseCopyHostedOnTarget(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyHostedOnTarget", "Ex632DC8", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString SearchProxyRpcException(string msg)
		{
			return new LocalizedString("SearchProxyRpcException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString AutoReseedCatalogIsBehindBacklog(int backlog)
		{
			return new LocalizedString("AutoReseedCatalogIsBehindBacklog", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				backlog
			});
		}

		public static LocalizedString CiSeederCatalogCouldNotPause(string ex)
		{
			return new LocalizedString("CiSeederCatalogCouldNotPause", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString TPRChangeFailedBecauseAlreadyActive(string curServerName)
		{
			return new LocalizedString("TPRChangeFailedBecauseAlreadyActive", "ExF4AFD9", false, true, ReplayStrings.ResourceManager, new object[]
			{
				curServerName
			});
		}

		public static LocalizedString DatabaseValidationException(string errorMsg)
		{
			return new LocalizedString("DatabaseValidationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString FileCheckLogfileMissing(string logfile)
		{
			return new LocalizedString("FileCheckLogfileMissing", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile
			});
		}

		public static LocalizedString FileCheckLogfileSignature(string logfile, string logfileSignature, string expectedSignature)
		{
			return new LocalizedString("FileCheckLogfileSignature", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile,
				logfileSignature,
				expectedSignature
			});
		}

		public static LocalizedString LockOwnerComponent
		{
			get
			{
				return new LocalizedString("LockOwnerComponent", "Ex2BE9EF", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NetworkIsDisabled
		{
			get
			{
				return new LocalizedString("NetworkIsDisabled", "ExB66A66", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileCheckJustCreatedEDB(string file)
		{
			return new LocalizedString("FileCheckJustCreatedEDB", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString AmDbOperationTimedoutException(string dbName, string opr, TimeSpan timeout)
		{
			return new LocalizedString("AmDbOperationTimedoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				opr,
				timeout
			});
		}

		public static LocalizedString ResumeOperationName
		{
			get
			{
				return new LocalizedString("ResumeOperationName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceSuspendReseedBlockedException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendReseedBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuspendMessageWriteFailedException
		{
			get
			{
				return new LocalizedString("SuspendMessageWriteFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbActionWrapperException(string dbActionError)
		{
			return new LocalizedString("AmDbActionWrapperException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbActionError
			});
		}

		public static LocalizedString UnexpectedEOF(string filename)
		{
			return new LocalizedString("UnexpectedEOF", "Ex012C8C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString SyncSuspendResumeOperationName
		{
			get
			{
				return new LocalizedString("SyncSuspendResumeOperationName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbDriveNotBigEnough(string path, long expectedSize, ulong actualSize)
		{
			return new LocalizedString("DbDriveNotBigEnough", "Ex1081AE", false, true, ReplayStrings.ResourceManager, new object[]
			{
				path,
				expectedSize,
				actualSize
			});
		}

		public static LocalizedString PagePatchApiFailedException(string msg)
		{
			return new LocalizedString("PagePatchApiFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString AutoReseedCatalogToUpgrade(int current, int latest)
		{
			return new LocalizedString("AutoReseedCatalogToUpgrade", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				current,
				latest
			});
		}

		public static LocalizedString RlmDatabaseCopyInvalidException(string databaseName, string serverName)
		{
			return new LocalizedString("RlmDatabaseCopyInvalidException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString LogRepairDivergenceCheckFailedError(string localEndOfLogFilename, string remoteDataInTempFilename, string exceptionText)
		{
			return new LocalizedString("LogRepairDivergenceCheckFailedError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				localEndOfLogFilename,
				remoteDataInTempFilename,
				exceptionText
			});
		}

		public static LocalizedString FailedAndSuspended
		{
			get
			{
				return new LocalizedString("FailedAndSuspended", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbValidationActiveCopyStatusUnknown(string dbName)
		{
			return new LocalizedString("DbValidationActiveCopyStatusUnknown", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString SeederReplayServiceDownException(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("SeederReplayServiceDownException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString TPRProviderNotListening
		{
			get
			{
				return new LocalizedString("TPRProviderNotListening", "Ex5CF21B", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Suspended
		{
			get
			{
				return new LocalizedString("Suspended", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbMoveSkippedBecauseNotActive(string dbName, string serverName, string activeServerName)
		{
			return new LocalizedString("DbMoveSkippedBecauseNotActive", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				activeServerName
			});
		}

		public static LocalizedString ReplayServiceSuspendInPlaceReseedBlockedException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendInPlaceReseedBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogInspectorSignatureMismatch(string logfileInspected, long genFromHeader)
		{
			return new LocalizedString("LogInspectorSignatureMismatch", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfileInspected,
				genFromHeader
			});
		}

		public static LocalizedString GranularReplicationMsgSequenceError(string msgContext)
		{
			return new LocalizedString("GranularReplicationMsgSequenceError", "Ex5A0B0D", false, true, ReplayStrings.ResourceManager, new object[]
			{
				msgContext
			});
		}

		public static LocalizedString PagePatchTooManyPagesToPatchException(int numPages, int maxSupported)
		{
			return new LocalizedString("PagePatchTooManyPagesToPatchException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				numPages,
				maxSupported
			});
		}

		public static LocalizedString LogRepairNotPossible(string reason)
		{
			return new LocalizedString("LogRepairNotPossible", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AmRoleChangedWhileOperationIsInProgress(string roleStart, string roleCurrent)
		{
			return new LocalizedString("AmRoleChangedWhileOperationIsInProgress", "ExCDF904", false, true, ReplayStrings.ResourceManager, new object[]
			{
				roleStart,
				roleCurrent
			});
		}

		public static LocalizedString AmPreMountCallbackFailedNoReplicaInstanceException(string dbName, string server)
		{
			return new LocalizedString("AmPreMountCallbackFailedNoReplicaInstanceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				server
			});
		}

		public static LocalizedString AutoReseedMoveActiveBeforeRebuildCatalog
		{
			get
			{
				return new LocalizedString("AutoReseedMoveActiveBeforeRebuildCatalog", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskMovingPam(string serverName)
		{
			return new LocalizedString("DagTaskMovingPam", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ErrorCouldNotConnectClusterForAmConfig
		{
			get
			{
				return new LocalizedString("ErrorCouldNotConnectClusterForAmConfig", "ExFDD9FF", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceSuspendRpcInvalidForActiveCopyException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceSuspendRpcInvalidForActiveCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString ReplayServiceTooMuchMemoryException(double memoryUsageInMib, long maximumMemoryUsageInMib)
		{
			return new LocalizedString("ReplayServiceTooMuchMemoryException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				memoryUsageInMib,
				maximumMemoryUsageInMib
			});
		}

		public static LocalizedString LogInspectorCouldNotMoveLogFileException(string oldpath, string newpath, string error)
		{
			return new LocalizedString("LogInspectorCouldNotMoveLogFileException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				oldpath,
				newpath,
				error
			});
		}

		public static LocalizedString ServerMoveAllDatabasesFailed(int numberFailedMoves)
		{
			return new LocalizedString("ServerMoveAllDatabasesFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				numberFailedMoves
			});
		}

		public static LocalizedString CouldNotMoveLogFile(string oldpath, string newpath)
		{
			return new LocalizedString("CouldNotMoveLogFile", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				oldpath,
				newpath
			});
		}

		public static LocalizedString DagTaskInstallingFailoverClustering(string serverName)
		{
			return new LocalizedString("DagTaskInstallingFailoverClustering", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString AmBcsDatabaseCopyIsHAComponentOffline(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopyIsHAComponentOffline", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString CopyStatusIsNotHealthy(string server, string db, string status)
		{
			return new LocalizedString("CopyStatusIsNotHealthy", "ExF39BB2", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				db,
				status
			});
		}

		public static LocalizedString ReplayServiceShuttingDownException
		{
			get
			{
				return new LocalizedString("ReplayServiceShuttingDownException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmLastServerTimeStampCorruptedException(string property, string corruptedValue)
		{
			return new LocalizedString("AmLastServerTimeStampCorruptedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				property,
				corruptedValue
			});
		}

		public static LocalizedString PagePatchInvalidPageSizeException(long dataSize, long expectedPageSize)
		{
			return new LocalizedString("PagePatchInvalidPageSizeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dataSize,
				expectedPageSize
			});
		}

		public static LocalizedString FileStateInternalError(string condition)
		{
			return new LocalizedString("FileStateInternalError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				condition
			});
		}

		public static LocalizedString SeederRpcUnsupportedException(string serverName, string serverVersion, string supportedVersion)
		{
			return new LocalizedString("SeederRpcUnsupportedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				supportedVersion
			});
		}

		public static LocalizedString ErrorFailedToOpenClusterObjects
		{
			get
			{
				return new LocalizedString("ErrorFailedToOpenClusterObjects", "Ex2A5125", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToOpenShipLogContextStoreStopped
		{
			get
			{
				return new LocalizedString("FailedToOpenShipLogContextStoreStopped", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WarningPerformingFastOperationException(string db, string error)
		{
			return new LocalizedString("WarningPerformingFastOperationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				error
			});
		}

		public static LocalizedString SeedInProgressException(string errMessage)
		{
			return new LocalizedString("SeedInProgressException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString AmMountTimeoutError(string dbName, string serverName, int timeoutInSecs)
		{
			return new LocalizedString("AmMountTimeoutError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				timeoutInSecs
			});
		}

		public static LocalizedString LastLogReplacementTempNewFileNotDeletedException(string dbCopy, string tempNewFile, string error)
		{
			return new LocalizedString("LastLogReplacementTempNewFileNotDeletedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				tempNewFile,
				error
			});
		}

		public static LocalizedString CiSeederExchangeSearchPermanentException(string message)
		{
			return new LocalizedString("CiSeederExchangeSearchPermanentException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ReplayServiceTooManyHandlesException(long numberOfHandles, long maxNumberOfHandles)
		{
			return new LocalizedString("ReplayServiceTooManyHandlesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				numberOfHandles,
				maxNumberOfHandles
			});
		}

		public static LocalizedString NullDatabaseException
		{
			get
			{
				return new LocalizedString("NullDatabaseException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsTargetServerIsStoppedOnDAC(string server)
		{
			return new LocalizedString("AmBcsTargetServerIsStoppedOnDAC", "ExBEB4AA", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString DagTaskNetFtProblem(int specificErrorCode)
		{
			return new LocalizedString("DagTaskNetFtProblem", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				specificErrorCode
			});
		}

		public static LocalizedString SeederEcCommunicationsError
		{
			get
			{
				return new LocalizedString("SeederEcCommunicationsError", "ExE04686", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbRemountSkippedSinceDatabaseWasAdminDismounted(string dbName)
		{
			return new LocalizedString("AmDbRemountSkippedSinceDatabaseWasAdminDismounted", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString DagTaskPamNotMovedSubsequentOperationsMayBeSlowOrUnreliable
		{
			get
			{
				return new LocalizedString("DagTaskPamNotMovedSubsequentOperationsMayBeSlowOrUnreliable", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederEcFailAcqRight
		{
			get
			{
				return new LocalizedString("SeederEcFailAcqRight", "Ex7B8F91", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskNotEnoughStaticIPAddresses(string network, string staticIps)
		{
			return new LocalizedString("DagTaskNotEnoughStaticIPAddresses", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				network,
				staticIps
			});
		}

		public static LocalizedString AmClusterException(string clusterError)
		{
			return new LocalizedString("AmClusterException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				clusterError
			});
		}

		public static LocalizedString SeedDivergenceFailedException(string targetCopyName, string divergenceFileName, string errorMsg)
		{
			return new LocalizedString("SeedDivergenceFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				targetCopyName,
				divergenceFileName,
				errorMsg
			});
		}

		public static LocalizedString ProgressStatusInProgress
		{
			get
			{
				return new LocalizedString("ProgressStatusInProgress", "Ex3EC6C5", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederInstanceReseedBlockedException(string dbCopyName, string errorMsg)
		{
			return new LocalizedString("SeederInstanceReseedBlockedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopyName,
				errorMsg
			});
		}

		public static LocalizedString CouldNotGetMountStatus
		{
			get
			{
				return new LocalizedString("CouldNotGetMountStatus", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(string computerAccount, string userAccount)
		{
			return new LocalizedString("DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				computerAccount,
				userAccount
			});
		}

		public static LocalizedString ReplayServiceSuspendRpcInvalidSeedingSourceException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceSuspendRpcInvalidSeedingSourceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString FailedToTruncateLocallyException(uint hresult, string optionalFriendlyError)
		{
			return new LocalizedString("FailedToTruncateLocallyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				hresult,
				optionalFriendlyError
			});
		}

		public static LocalizedString AmClusterNotRunningException
		{
			get
			{
				return new LocalizedString("AmClusterNotRunningException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LockOwnerConfigChecker
		{
			get
			{
				return new LocalizedString("LockOwnerConfigChecker", "Ex662E4A", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayDbOperationException(string opError)
		{
			return new LocalizedString("ReplayDbOperationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				opError
			});
		}

		public static LocalizedString PrepareToStopCalled
		{
			get
			{
				return new LocalizedString("PrepareToStopCalled", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceSuspendWantedClearedException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendWantedClearedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbActionRejectedAdminDismountedException(string actionCode)
		{
			return new LocalizedString("AmDbActionRejectedAdminDismountedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionCode
			});
		}

		public static LocalizedString ExchangeVolumeInfoMultipleExMountPointsException(string volumeName, string exVolRootPath, string exMountPoints)
		{
			return new LocalizedString("ExchangeVolumeInfoMultipleExMountPointsException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				exVolRootPath,
				exMountPoints
			});
		}

		public static LocalizedString SeederInstanceInvalidStateForEndException(string dbGuid)
		{
			return new LocalizedString("SeederInstanceInvalidStateForEndException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString ServerNotFoundException(string serverName)
		{
			return new LocalizedString("ServerNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString DBCHasNoValidTargetEdbPath
		{
			get
			{
				return new LocalizedString("DBCHasNoValidTargetEdbPath", "ExFC74E4", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederServerTransientException(string errorMessage)
		{
			return new LocalizedString("SeederServerTransientException", "Ex21B881", false, true, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ReplayConfigPropException(string id, string propertyName)
		{
			return new LocalizedString("ReplayConfigPropException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				id,
				propertyName
			});
		}

		public static LocalizedString DeleteChkptReasonCorrupted
		{
			get
			{
				return new LocalizedString("DeleteChkptReasonCorrupted", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TPRChangeFailedBecauseNotDismounted
		{
			get
			{
				return new LocalizedString("TPRChangeFailedBecauseNotDismounted", "Ex7237ED", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PagePatchInvalidFileException(string patchFile)
		{
			return new LocalizedString("PagePatchInvalidFileException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				patchFile
			});
		}

		public static LocalizedString CouldNotFindVolumeForFormatException
		{
			get
			{
				return new LocalizedString("CouldNotFindVolumeForFormatException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeedPrepareException(string errMessage)
		{
			return new LocalizedString("SeedPrepareException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString FileCheckRequiredLogfileGapException(string logfile)
		{
			return new LocalizedString("FileCheckRequiredLogfileGapException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile
			});
		}

		public static LocalizedString DeleteChkptReasonForce(long checkpointGeneration)
		{
			return new LocalizedString("DeleteChkptReasonForce", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				checkpointGeneration
			});
		}

		public static LocalizedString DbValidationInspectorQueueLengthTooHigh(string dbCopyName, long length, long maxLength)
		{
			return new LocalizedString("DbValidationInspectorQueueLengthTooHigh", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopyName,
				length,
				maxLength
			});
		}

		public static LocalizedString CannotChangeName
		{
			get
			{
				return new LocalizedString("CannotChangeName", "Ex07B680", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoReseedPrereqFailedException(string databaseName, string serverName, string error)
		{
			return new LocalizedString("AutoReseedPrereqFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName,
				error
			});
		}

		public static LocalizedString NetworkManagerInitError
		{
			get
			{
				return new LocalizedString("NetworkManagerInitError", "Ex5905F3", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PassiveCopyDisconnected
		{
			get
			{
				return new LocalizedString("PassiveCopyDisconnected", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CopyUnknownToActiveLogTruncationException(string db, string activeNode, string targetNode, uint hresult)
		{
			return new LocalizedString("CopyUnknownToActiveLogTruncationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				activeNode,
				targetNode,
				hresult
			});
		}

		public static LocalizedString AutoReseedFailedInPlaceReseedBlocked(string error)
		{
			return new LocalizedString("AutoReseedFailedInPlaceReseedBlocked", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString TPRChangeFailedServerValidation(string databaseName, string curServerName, string validationError)
		{
			return new LocalizedString("TPRChangeFailedServerValidation", "Ex2A382B", false, true, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				curServerName,
				validationError
			});
		}

		public static LocalizedString AmDbMoveSkippedSinceMasterChanged(string dbName)
		{
			return new LocalizedString("AmDbMoveSkippedSinceMasterChanged", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString HungDetectionGumIdChanged(int localGumId, int remoteGumId, string lockOwnerName, long hungNodesMask)
		{
			return new LocalizedString("HungDetectionGumIdChanged", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				localGumId,
				remoteGumId,
				lockOwnerName,
				hungNodesMask
			});
		}

		public static LocalizedString SeederInstanceAlreadyCancelledException(string sourceMachine)
		{
			return new LocalizedString("SeederInstanceAlreadyCancelledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				sourceMachine
			});
		}

		public static LocalizedString CancelSeedingDueToConfigChangeOrServiceShutdown(string id, string machine, string reasonCode)
		{
			return new LocalizedString("CancelSeedingDueToConfigChangeOrServiceShutdown", "ExBFF2AD", false, true, ReplayStrings.ResourceManager, new object[]
			{
				id,
				machine,
				reasonCode
			});
		}

		public static LocalizedString SeederEchrErrorFromESECall
		{
			get
			{
				return new LocalizedString("SeederEchrErrorFromESECall", "Ex7E026C", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterNetworkNullSubnetError(string clusterNetworkName)
		{
			return new LocalizedString("ClusterNetworkNullSubnetError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				clusterNetworkName
			});
		}

		public static LocalizedString ReplayServiceSuspendBlockedBackupInProgressException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendBlockedBackupInProgressException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCouldNotFindServerForAmConfig
		{
			get
			{
				return new LocalizedString("ErrorCouldNotFindServerForAmConfig", "Ex3242AD", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TPRProviderNotResponding(string reason)
		{
			return new LocalizedString("TPRProviderNotResponding", "ExB10FA4", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString SpareConflictInLayoutException(int spares)
		{
			return new LocalizedString("SpareConflictInLayoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				spares
			});
		}

		public static LocalizedString ErrorDagMisconfiguredForAmConfig(string serverName, string dagName)
		{
			return new LocalizedString("ErrorDagMisconfiguredForAmConfig", "Ex0EB713", false, true, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				dagName
			});
		}

		public static LocalizedString AmBcsFailedToQueryCopiesException(string dbName, string queryError)
		{
			return new LocalizedString("AmBcsFailedToQueryCopiesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				queryError
			});
		}

		public static LocalizedString ReplayServiceTooManyThreadsException(long numberOfThreads, long maxNumberOfThreads)
		{
			return new LocalizedString("ReplayServiceTooManyThreadsException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				numberOfThreads,
				maxNumberOfThreads
			});
		}

		public static LocalizedString AmDbActionCancelledException(string dbName, string opr)
		{
			return new LocalizedString("AmDbActionCancelledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				opr
			});
		}

		public static LocalizedString FileIOonSourceException(string serverName, string fileFullPath, string ioErrorMessage)
		{
			return new LocalizedString("FileIOonSourceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				fileFullPath,
				ioErrorMessage
			});
		}

		public static LocalizedString FileOpenError(string fileName, string errMsg)
		{
			return new LocalizedString("FileOpenError", "ExBB666A", false, true, ReplayStrings.ResourceManager, new object[]
			{
				fileName,
				errMsg
			});
		}

		public static LocalizedString SeederEcTargetDbFileInUse(string dbFilePath)
		{
			return new LocalizedString("SeederEcTargetDbFileInUse", "Ex1A9177", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbFilePath
			});
		}

		public static LocalizedString AutoReseedUnhandledException(string databaseName, string serverName)
		{
			return new LocalizedString("AutoReseedUnhandledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString FailedAtReplacingLogFiles
		{
			get
			{
				return new LocalizedString("FailedAtReplacingLogFiles", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDbStateForIncReseed(string dbState)
		{
			return new LocalizedString("InvalidDbStateForIncReseed", "Ex659260", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbState
			});
		}

		public static LocalizedString ReplayServiceResumeRpcFailedSeedingException
		{
			get
			{
				return new LocalizedString("ReplayServiceResumeRpcFailedSeedingException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastLogReplacementUnexpectedTempFilesException(string dbCopy, string logPath)
		{
			return new LocalizedString("LastLogReplacementUnexpectedTempFilesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				logPath
			});
		}

		public static LocalizedString AmBcsDatabaseCopySeeding(string db, string server)
		{
			return new LocalizedString("AmBcsDatabaseCopySeeding", "ExA513C3", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server
			});
		}

		public static LocalizedString AmInvalidDbState(Guid databaseGuid, string stateStr)
		{
			return new LocalizedString("AmInvalidDbState", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseGuid,
				stateStr
			});
		}

		public static LocalizedString StoreNotOnline
		{
			get
			{
				return new LocalizedString("StoreNotOnline", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmBcsGetCopyStatusRpcException(string server, string database, string rpcError)
		{
			return new LocalizedString("AmBcsGetCopyStatusRpcException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				database,
				rpcError
			});
		}

		public static LocalizedString AmDbMoveOperationSkippedException(string dbName, string reason)
		{
			return new LocalizedString("AmDbMoveOperationSkippedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				reason
			});
		}

		public static LocalizedString SeederFailedToFindDirectory(string directory)
		{
			return new LocalizedString("SeederFailedToFindDirectory", "Ex656E3A", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString NetworkUnexpectedMessage(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkUnexpectedMessage", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString LockOwnerAttemptCopyLastLogs
		{
			get
			{
				return new LocalizedString("LockOwnerAttemptCopyLastLogs", "Ex35A464", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterBatchWriter_OpenActiveManagerKeyFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_OpenActiveManagerKeyFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString SeederRpcSafeDeleteUnsupportedException(string serverName, string serverVersion, string supportedVersion)
		{
			return new LocalizedString("SeederRpcSafeDeleteUnsupportedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				serverVersion,
				supportedVersion
			});
		}

		public static LocalizedString AutoReseedNotAllCopiesOnVolumeFailedSuspended(string dbNames)
		{
			return new LocalizedString("AutoReseedNotAllCopiesOnVolumeFailedSuspended", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbNames
			});
		}

		public static LocalizedString MsexchangereplLong
		{
			get
			{
				return new LocalizedString("MsexchangereplLong", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastLogReplacementFailedFileNotFoundException(string dbCopy, string missingFile, string e00log)
		{
			return new LocalizedString("LastLogReplacementFailedFileNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				missingFile,
				e00log
			});
		}

		public static LocalizedString DatabaseNotHealthyOnVolume(string databaseName, string volumeName)
		{
			return new LocalizedString("DatabaseNotHealthyOnVolume", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				volumeName
			});
		}

		public static LocalizedString DagTaskComputerAccountCouldNotBeValidatedException(string computerAccount, string userAccount, string error)
		{
			return new LocalizedString("DagTaskComputerAccountCouldNotBeValidatedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				computerAccount,
				userAccount,
				error
			});
		}

		public static LocalizedString AmDbActionRejectedLastAdminActionDidNotSucceedException(string actionCode)
		{
			return new LocalizedString("AmDbActionRejectedLastAdminActionDidNotSucceedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actionCode
			});
		}

		public static LocalizedString AcllInvalidForActiveCopyException(string dbCopy)
		{
			return new LocalizedString("AcllInvalidForActiveCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString DumpsterCouldNotReadMaxDumpsterTimeException(string dbName)
		{
			return new LocalizedString("DumpsterCouldNotReadMaxDumpsterTimeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString AmPreMountCallbackFailedNoReplicaInstanceErrorException(string dbName, string server, string errMsg)
		{
			return new LocalizedString("AmPreMountCallbackFailedNoReplicaInstanceErrorException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				server,
				errMsg
			});
		}

		public static LocalizedString MonitoringCouldNotFindDagException(string dagName, string adError)
		{
			return new LocalizedString("MonitoringCouldNotFindDagException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dagName,
				adError
			});
		}

		public static LocalizedString AcllAlreadyRunningException(string dbCopy)
		{
			return new LocalizedString("AcllAlreadyRunningException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString AmOperationInvalidForStandaloneRoleException
		{
			get
			{
				return new LocalizedString("AmOperationInvalidForStandaloneRoleException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AcllCopyIsNotViableException(string dbCopy)
		{
			return new LocalizedString("AcllCopyIsNotViableException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString DatabaseFailoverFailedException(string dbName, string msg)
		{
			return new LocalizedString("DatabaseFailoverFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				msg
			});
		}

		public static LocalizedString VolumeRecentlyModifiedException(string volumeName, TimeSpan threshold, string dateTimeUtc, string mountPoint, string lastUpdatePath)
		{
			return new LocalizedString("VolumeRecentlyModifiedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				threshold,
				dateTimeUtc,
				mountPoint,
				lastUpdatePath
			});
		}

		public static LocalizedString JetErrorDatabaseNotFound
		{
			get
			{
				return new LocalizedString("JetErrorDatabaseNotFound", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskQuorumNotAchievedException(string dagName)
		{
			return new LocalizedString("DagTaskQuorumNotAchievedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString OperationTimeoutErr(int secs)
		{
			return new LocalizedString("OperationTimeoutErr", "ExA61C08", false, true, ReplayStrings.ResourceManager, new object[]
			{
				secs
			});
		}

		public static LocalizedString DagTaskFormingClusterToLog(string clusterName, string firstServer, string ipAddresses, string ipAddressMasks)
		{
			return new LocalizedString("DagTaskFormingClusterToLog", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				clusterName,
				firstServer,
				ipAddresses,
				ipAddressMasks
			});
		}

		public static LocalizedString ErrorNullServerFromDb(string dbName)
		{
			return new LocalizedString("ErrorNullServerFromDb", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString SeederEcBackupInProgress
		{
			get
			{
				return new LocalizedString("SeederEcBackupInProgress", "ExDB80C0", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TPREnabledInvalidOperation(string operationName)
		{
			return new LocalizedString("TPREnabledInvalidOperation", "Ex70DB25", false, true, ReplayStrings.ResourceManager, new object[]
			{
				operationName
			});
		}

		public static LocalizedString AcllSetCurrentLogGenerationException(string dbCopy, string e00logPath, string err)
		{
			return new LocalizedString("AcllSetCurrentLogGenerationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				e00logPath,
				err
			});
		}

		public static LocalizedString FailedToReadDatabasePage(int error)
		{
			return new LocalizedString("FailedToReadDatabasePage", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DagTaskRemoteOperationLogData(string verboseData)
		{
			return new LocalizedString("DagTaskRemoteOperationLogData", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				verboseData
			});
		}

		public static LocalizedString AmBcsTargetServerMaxActivesReached(string server, string maxActiveDatabases)
		{
			return new LocalizedString("AmBcsTargetServerMaxActivesReached", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				server,
				maxActiveDatabases
			});
		}

		public static LocalizedString NetworkCorruptData(string srcNode)
		{
			return new LocalizedString("NetworkCorruptData", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				srcNode
			});
		}

		public static LocalizedString LogInspectorFailedGeneral(string fileName, string specificError)
		{
			return new LocalizedString("LogInspectorFailedGeneral", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				fileName,
				specificError
			});
		}

		public static LocalizedString LogCopierFailedToGetSuspendLock
		{
			get
			{
				return new LocalizedString("LogCopierFailedToGetSuspendLock", "Ex2707FB", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceRpcArgumentException(string argument)
		{
			return new LocalizedString("ReplayServiceRpcArgumentException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString AcllFailedException(string error)
		{
			return new LocalizedString("AcllFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString NetworkRemoteErrorUnknown(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkRemoteErrorUnknown", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString FileCheckCorruptFile(string file, string errorMessage)
		{
			return new LocalizedString("FileCheckCorruptFile", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				file,
				errorMessage
			});
		}

		public static LocalizedString DagTaskFileShareWitnessResourceIsStillNotOnlineException(string fswResource, string currentState)
		{
			return new LocalizedString("DagTaskFileShareWitnessResourceIsStillNotOnlineException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				fswResource,
				currentState
			});
		}

		public static LocalizedString DeleteChkptReasonTooAdvanced(long checkpointGeneration)
		{
			return new LocalizedString("DeleteChkptReasonTooAdvanced", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				checkpointGeneration
			});
		}

		public static LocalizedString NetworkDataOverflowGeneric
		{
			get
			{
				return new LocalizedString("NetworkDataOverflowGeneric", "Ex473BF9", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbOperationAttempedTooSoonException(string dbName)
		{
			return new LocalizedString("AmDbOperationAttempedTooSoonException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString AmTransientException(string errMessage)
		{
			return new LocalizedString("AmTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString AmGetServiceProcessFailed(string serviceName, int state, int pid)
		{
			return new LocalizedString("AmGetServiceProcessFailed", "Ex466359", false, true, ReplayStrings.ResourceManager, new object[]
			{
				serviceName,
				state,
				pid
			});
		}

		public static LocalizedString SourceDatabaseNotFound(Guid g, string sourceServer)
		{
			return new LocalizedString("SourceDatabaseNotFound", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				g,
				sourceServer
			});
		}

		public static LocalizedString DatabaseRemountFailedException(string dbName, string msg)
		{
			return new LocalizedString("DatabaseRemountFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				msg
			});
		}

		public static LocalizedString LogRepairUnexpectedVerifyError(string logName, string exceptionText)
		{
			return new LocalizedString("LogRepairUnexpectedVerifyError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logName,
				exceptionText
			});
		}

		public static LocalizedString TPRNotEnabled
		{
			get
			{
				return new LocalizedString("TPRNotEnabled", "Ex668CBF", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LockOwnerSuspend
		{
			get
			{
				return new LocalizedString("LockOwnerSuspend", "Ex4BDE99", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbValidationFullCopyStatusResultsLabel
		{
			get
			{
				return new LocalizedString("DbValidationFullCopyStatusResultsLabel", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogRepairNotPossibleInsuffientToCheckDivergence
		{
			get
			{
				return new LocalizedString("LogRepairNotPossibleInsuffientToCheckDivergence", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EseLogEnumeratorIOError(string apiName, string ioErrorMessage, int win32ErrCode, string directoryName)
		{
			return new LocalizedString("EseLogEnumeratorIOError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				apiName,
				ioErrorMessage,
				win32ErrCode,
				directoryName
			});
		}

		public static LocalizedString UnExpectedPageSize(string db, long pageSize)
		{
			return new LocalizedString("UnExpectedPageSize", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				pageSize
			});
		}

		public static LocalizedString ErrorCouldNotConnectNativeClusterForAmConfig(int ec)
		{
			return new LocalizedString("ErrorCouldNotConnectNativeClusterForAmConfig", "Ex05C2B1", false, true, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString DbHTFirstLookupTimeoutException(int timeoutMs)
		{
			return new LocalizedString("DbHTFirstLookupTimeoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				timeoutMs
			});
		}

		public static LocalizedString AmBcsTargetNodeDownError(string server)
		{
			return new LocalizedString("AmBcsTargetNodeDownError", "ExF8DBF3", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString DagTaskComponentManagerWantsToRebootException
		{
			get
			{
				return new LocalizedString("DagTaskComponentManagerWantsToRebootException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RegistryParameterWriteException(string valueName, string errMsg)
		{
			return new LocalizedString("RegistryParameterWriteException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				valueName,
				errMsg
			});
		}

		public static LocalizedString DbAvailabilityValidationErrorsOccurred(string dbName, int healthyCopiesCount, int expectedHealthyCount, string detailedMsg)
		{
			return new LocalizedString("DbAvailabilityValidationErrorsOccurred", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				healthyCopiesCount,
				expectedHealthyCount,
				detailedMsg
			});
		}

		public static LocalizedString MonitoringCouldNotFindMiniServerException(string serverName)
		{
			return new LocalizedString("MonitoringCouldNotFindMiniServerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString NetworkCancelled
		{
			get
			{
				return new LocalizedString("NetworkCancelled", "ExC24353", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayDbOperationWrapperException(string operationError)
		{
			return new LocalizedString("ReplayDbOperationWrapperException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationError
			});
		}

		public static LocalizedString ErrorFailedToGetClusterCoreGroup
		{
			get
			{
				return new LocalizedString("ErrorFailedToGetClusterCoreGroup", "ExE99407", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotInPendingState
		{
			get
			{
				return new LocalizedString("NotInPendingState", "Ex69E4EF", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindLocalServerException(string serverName)
		{
			return new LocalizedString("FailedToFindLocalServerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CallWithoutnumberOfExtraCopiesOnSparesException(string errMsg)
		{
			return new LocalizedString("CallWithoutnumberOfExtraCopiesOnSparesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString AmBcsTargetServerADError(string server, string adError)
		{
			return new LocalizedString("AmBcsTargetServerADError", "Ex5DD649", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				adError
			});
		}

		public static LocalizedString ReplaySystemOperationCancelledException(string operationName)
		{
			return new LocalizedString("ReplaySystemOperationCancelledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName
			});
		}

		public static LocalizedString ServiceNotRunningOnNodeException(string serviceName, string nodeName)
		{
			return new LocalizedString("ServiceNotRunningOnNodeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serviceName,
				nodeName
			});
		}

		public static LocalizedString ErrorInvalidPamServerName
		{
			get
			{
				return new LocalizedString("ErrorInvalidPamServerName", "Ex5C007A", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederEchrErrorFromCallbackCall
		{
			get
			{
				return new LocalizedString("SeederEchrErrorFromCallbackCall", "Ex685E98", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFileCheckError(string logFileName, string errMsg)
		{
			return new LocalizedString("LogFileCheckError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logFileName,
				errMsg
			});
		}

		public static LocalizedString EnableReplayLagOperationName
		{
			get
			{
				return new LocalizedString("EnableReplayLagOperationName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederInstanceAlreadyFailedException
		{
			get
			{
				return new LocalizedString("SeederInstanceAlreadyFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerIsNotJoinedYet(string server)
		{
			return new LocalizedString("ServerIsNotJoinedYet", "Ex18095F", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString LastLogGenerationTimeStampStale(string timeStamp)
		{
			return new LocalizedString("LastLogGenerationTimeStampStale", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				timeStamp
			});
		}

		public static LocalizedString DbValidationDbNotReplicated(string dbName)
		{
			return new LocalizedString("DbValidationDbNotReplicated", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString ReplayDbOperationWrapperTransientException(string operationError)
		{
			return new LocalizedString("ReplayDbOperationWrapperTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationError
			});
		}

		public static LocalizedString ReplayStoreOperationAbortedException(string operationName)
		{
			return new LocalizedString("ReplayStoreOperationAbortedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName
			});
		}

		public static LocalizedString ReplayServiceSuspendWantedSetException
		{
			get
			{
				return new LocalizedString("ReplayServiceSuspendWantedSetException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DBCNotSuspendedYet(string db)
		{
			return new LocalizedString("DBCNotSuspendedYet", "ExE73AC3", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db
			});
		}

		public static LocalizedString AutoReseedThrottledException(string databaseName, string serverName, string throttlingInterval)
		{
			return new LocalizedString("AutoReseedThrottledException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName,
				throttlingInterval
			});
		}

		public static LocalizedString DbValidationActiveCopyStatusRpcFailed(string dbName, string serverName, string error)
		{
			return new LocalizedString("DbValidationActiveCopyStatusRpcFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				error
			});
		}

		public static LocalizedString SeederEcOOMem
		{
			get
			{
				return new LocalizedString("SeederEcOOMem", "ExAF161F", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidVolumeMissingException(string volumeName)
		{
			return new LocalizedString("InvalidVolumeMissingException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName
			});
		}

		public static LocalizedString DagTaskJoinedNodeToCluster(string serverName)
		{
			return new LocalizedString("DagTaskJoinedNodeToCluster", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString CouldNotGetMountStatusError(string errorMessage)
		{
			return new LocalizedString("CouldNotGetMountStatusError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString LastLogReplacementException(string msg)
		{
			return new LocalizedString("LastLogReplacementException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString DatabasesMissingInCopyStatusLookUpTable(string databaseNames)
		{
			return new LocalizedString("DatabasesMissingInCopyStatusLookUpTable", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseNames
			});
		}

		public static LocalizedString DbValidationCopyStatusNameLabel
		{
			get
			{
				return new LocalizedString("DbValidationCopyStatusNameLabel", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncSeedConfigNotSupportedError(string field)
		{
			return new LocalizedString("IncSeedConfigNotSupportedError", "Ex719D9C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				field
			});
		}

		public static LocalizedString SeederFailedToDeleteLogs(string directory, string error)
		{
			return new LocalizedString("SeederFailedToDeleteLogs", "Ex2C38CB", false, true, ReplayStrings.ResourceManager, new object[]
			{
				directory,
				error
			});
		}

		public static LocalizedString ReplayServiceResumeRpcInvalidForActiveCopyException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceResumeRpcInvalidForActiveCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString TPRmmediateDismountFailed(Guid dbId, string reason)
		{
			return new LocalizedString("TPRmmediateDismountFailed", "Ex1BA4FE", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbId,
				reason
			});
		}

		public static LocalizedString RepairStateError(string error)
		{
			return new LocalizedString("RepairStateError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CannotBeginSeedingInstanceNotInStateException(string dbName, string state)
		{
			return new LocalizedString("CannotBeginSeedingInstanceNotInStateException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				state
			});
		}

		public static LocalizedString RepairStateDatabaseNotReplicated(string dbName)
		{
			return new LocalizedString("RepairStateDatabaseNotReplicated", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString MonitoringADFirstLookupTimeoutException(int timeoutMs)
		{
			return new LocalizedString("MonitoringADFirstLookupTimeoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				timeoutMs
			});
		}

		public static LocalizedString AmBcsManagedAvailabilityCheckFailed(string srcServer, string tgtServer, string componentName, string failures)
		{
			return new LocalizedString("AmBcsManagedAvailabilityCheckFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				srcServer,
				tgtServer,
				componentName,
				failures
			});
		}

		public static LocalizedString SeederEchrRestoreAtFileLevel
		{
			get
			{
				return new LocalizedString("SeederEchrRestoreAtFileLevel", "Ex99EC0F", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TPRNotYetStarted
		{
			get
			{
				return new LocalizedString("TPRNotYetStarted", "Ex0F2837", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeederEcInvalidInput
		{
			get
			{
				return new LocalizedString("SeederEcInvalidInput", "Ex69E7D7", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplayServiceSyncStateInvalidDuringMoveException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceSyncStateInvalidDuringMoveException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString DatabasePageSizeUnexpected(long size, long expected)
		{
			return new LocalizedString("DatabasePageSizeUnexpected", "Ex6E8BA7", false, true, ReplayStrings.ResourceManager, new object[]
			{
				size,
				expected
			});
		}

		public static LocalizedString AmBcsDatabaseCopyQueueLengthTooHigh(string db, string server, long length, long maxLength)
		{
			return new LocalizedString("AmBcsDatabaseCopyQueueLengthTooHigh", "Ex18998C", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				length,
				maxLength
			});
		}

		public static LocalizedString DagTaskFswNeedsCnoPermissionException(string fswPath, string accountName)
		{
			return new LocalizedString("DagTaskFswNeedsCnoPermissionException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				fswPath,
				accountName
			});
		}

		public static LocalizedString ReplayServiceResumeRpcPartialSuccessCatalogFailedException(string errMsg)
		{
			return new LocalizedString("ReplayServiceResumeRpcPartialSuccessCatalogFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString ReplayServiceCouldNotFindReplayConfigException(string database, string server)
		{
			return new LocalizedString("ReplayServiceCouldNotFindReplayConfigException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				server
			});
		}

		public static LocalizedString SeederCopyNotSuspended(string db)
		{
			return new LocalizedString("SeederCopyNotSuspended", "Ex01168E", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db
			});
		}

		public static LocalizedString DagTaskFormingClusterProgress(string clusterName, string firstServer)
		{
			return new LocalizedString("DagTaskFormingClusterProgress", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				clusterName,
				firstServer
			});
		}

		public static LocalizedString NetworkConnectionTimeout(int waitInsecs)
		{
			return new LocalizedString("NetworkConnectionTimeout", "Ex84B58B", false, true, ReplayStrings.ResourceManager, new object[]
			{
				waitInsecs
			});
		}

		public static LocalizedString DbFixupFailedException(string dbName, string volumeName, string reason)
		{
			return new LocalizedString("DbFixupFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				volumeName,
				reason
			});
		}

		public static LocalizedString AmPreMountCallbackFailedException(string dbName, string error)
		{
			return new LocalizedString("AmPreMountCallbackFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				error
			});
		}

		public static LocalizedString AmBcsSingleCopyValidationException(string bcsMessage)
		{
			return new LocalizedString("AmBcsSingleCopyValidationException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				bcsMessage
			});
		}

		public static LocalizedString AutoReseedCatalogSourceException(string databaseName, string serverName)
		{
			return new LocalizedString("AutoReseedCatalogSourceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				serverName
			});
		}

		public static LocalizedString ErrorRemoteSiteNotConnected
		{
			get
			{
				return new LocalizedString("ErrorRemoteSiteNotConnected", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmMountBlockedDbMountedBeforeWithMissingEdbException(string dbName, string edbFilePath)
		{
			return new LocalizedString("AmMountBlockedDbMountedBeforeWithMissingEdbException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				edbFilePath
			});
		}

		public static LocalizedString AutoReseedInvalidEdbFolderPath(string actualPath, string expectedPath)
		{
			return new LocalizedString("AutoReseedInvalidEdbFolderPath", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				actualPath,
				expectedPath
			});
		}

		public static LocalizedString MissingLogRequired(string file)
		{
			return new LocalizedString("MissingLogRequired", "Ex5DBF20", false, true, ReplayStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString SeederFailedToDeployDatabase(string src, string dest, string error)
		{
			return new LocalizedString("SeederFailedToDeployDatabase", "Ex07D113", false, true, ReplayStrings.ResourceManager, new object[]
			{
				src,
				dest,
				error
			});
		}

		public static LocalizedString PreferFullReseed(int wayPoint)
		{
			return new LocalizedString("PreferFullReseed", "Ex12ED74", false, true, ReplayStrings.ResourceManager, new object[]
			{
				wayPoint
			});
		}

		public static LocalizedString DagReplayServiceDownException(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("DagReplayServiceDownException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString AutoReseedFailedResumeRetryExceeded(int maxRetryCount)
		{
			return new LocalizedString("AutoReseedFailedResumeRetryExceeded", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				maxRetryCount
			});
		}

		public static LocalizedString AcllTempLogCreationFailedException(string dbCopy, string err)
		{
			return new LocalizedString("AcllTempLogCreationFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				err
			});
		}

		public static LocalizedString NetworkCommunicationError(string remoteNodeName, string errorText)
		{
			return new LocalizedString("NetworkCommunicationError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				remoteNodeName,
				errorText
			});
		}

		public static LocalizedString SetBrokenWatsonException(string errMsg)
		{
			return new LocalizedString("SetBrokenWatsonException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString LogRepairRetryCountExceeded(long retryCount)
		{
			return new LocalizedString("LogRepairRetryCountExceeded", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				retryCount
			});
		}

		public static LocalizedString FileCheckLogfileGeneration(string logfile, long logfileGeneration, long expectedGeneration)
		{
			return new LocalizedString("FileCheckLogfileGeneration", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				logfile,
				logfileGeneration,
				expectedGeneration
			});
		}

		public static LocalizedString ErrorLocalNodeNotUpYet
		{
			get
			{
				return new LocalizedString("ErrorLocalNodeNotUpYet", "Ex4FCD4B", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetProcessForServiceException(string serviceName, string msg)
		{
			return new LocalizedString("FailedToGetProcessForServiceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serviceName,
				msg
			});
		}

		public static LocalizedString DatabasesMissingInADException(string databaseName, string volumeName)
		{
			return new LocalizedString("DatabasesMissingInADException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName,
				volumeName
			});
		}

		public static LocalizedString VolumeNotSafeForFormatException(string volumeName, string mountPoint)
		{
			return new LocalizedString("VolumeNotSafeForFormatException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				mountPoint
			});
		}

		public static LocalizedString AmFailedToReadClusdbException(string error)
		{
			return new LocalizedString("AmFailedToReadClusdbException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString AcllLossDeterminationFailedException(string error)
		{
			return new LocalizedString("AcllLossDeterminationFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DagTaskClusteringRequiresEnterpriseSkuException
		{
			get
			{
				return new LocalizedString("DagTaskClusteringRequiresEnterpriseSkuException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseHealthTrackerException(string errorMsg)
		{
			return new LocalizedString("DatabaseHealthTrackerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString DagTaskJoiningNodeToCluster(string serverName)
		{
			return new LocalizedString("DagTaskJoiningNodeToCluster", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString LastLogReplacementFailedErrorException(string dbCopy, string e00log, string error)
		{
			return new LocalizedString("LastLogReplacementFailedErrorException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy,
				e00log,
				error
			});
		}

		public static LocalizedString MsexchangesearchLong
		{
			get
			{
				return new LocalizedString("MsexchangesearchLong", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LockOwnerBackup
		{
			get
			{
				return new LocalizedString("LockOwnerBackup", "ExC9C62B", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskRemoteOperationLogBegin(string serverName)
		{
			return new LocalizedString("DagTaskRemoteOperationLogBegin", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString AmBcsDatabaseCopyReplayQueueLengthTooHigh(string db, string server, long length, long maxLength)
		{
			return new LocalizedString("AmBcsDatabaseCopyReplayQueueLengthTooHigh", "ExDAC1F8", false, true, ReplayStrings.ResourceManager, new object[]
			{
				db,
				server,
				length,
				maxLength
			});
		}

		public static LocalizedString CorruptLogDetectedError(string filename, string errorText)
		{
			return new LocalizedString("CorruptLogDetectedError", "ExCE54EB", false, true, ReplayStrings.ResourceManager, new object[]
			{
				filename,
				errorText
			});
		}

		public static LocalizedString SafeDeleteExistingFilesDataRedundancyException(string db, string errMsg2)
		{
			return new LocalizedString("SafeDeleteExistingFilesDataRedundancyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				db,
				errMsg2
			});
		}

		public static LocalizedString AmDbActionMoveFailedException
		{
			get
			{
				return new LocalizedString("AmDbActionMoveFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Failed
		{
			get
			{
				return new LocalizedString("Failed", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GranularReplicationOverflow
		{
			get
			{
				return new LocalizedString("GranularReplicationOverflow", "Ex267116", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterInvalidResubmitRequestException(string dbName)
		{
			return new LocalizedString("DumpsterInvalidResubmitRequestException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString FileCheckIsamError(string errorMessage)
		{
			return new LocalizedString("FileCheckIsamError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString CantStartFromCommandLine
		{
			get
			{
				return new LocalizedString("CantStartFromCommandLine", "ExD57B51", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbActionMountFailedException
		{
			get
			{
				return new LocalizedString("AmDbActionMountFailedException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmClusterNoServerToConnect(string dagName)
		{
			return new LocalizedString("AmClusterNoServerToConnect", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString DatabaseValidationNoCopiesException(string databaseName)
		{
			return new LocalizedString("DatabaseValidationNoCopiesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ClusterBatchWriter_OpenCopyStateKeyFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_OpenCopyStateKeyFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString FileCheckIOError(string errorMessage)
		{
			return new LocalizedString("FileCheckIOError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString CouldNotFindSpareVolumeException(string databases)
		{
			return new LocalizedString("CouldNotFindSpareVolumeException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databases
			});
		}

		public static LocalizedString FileReadException(string fileName, int expectedBytes, int actualBytes)
		{
			return new LocalizedString("FileReadException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				fileName,
				expectedBytes,
				actualBytes
			});
		}

		public static LocalizedString ReplayServiceResumeRpcInvalidForSingleCopyException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceResumeRpcInvalidForSingleCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString InvalidSavedStateException
		{
			get
			{
				return new LocalizedString("InvalidSavedStateException", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DbValidationCopyStatusRpcFailed(string dbName, string serverName, string error)
		{
			return new LocalizedString("DbValidationCopyStatusRpcFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				serverName,
				error
			});
		}

		public static LocalizedString AmClusterFileNotFoundException(string nodeName)
		{
			return new LocalizedString("AmClusterFileNotFoundException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString SeedingAnotherServerException(string seedingServerName, string requestServerName)
		{
			return new LocalizedString("SeedingAnotherServerException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				seedingServerName,
				requestServerName
			});
		}

		public static LocalizedString DagTaskOperationFailedException(string errMessage)
		{
			return new LocalizedString("DagTaskOperationFailedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString AmDbMoveOperationNotSupportedStandaloneException(string dbName)
		{
			return new LocalizedString("AmDbMoveOperationNotSupportedStandaloneException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString AutoReseedNotAllCopiesPassive(string dbNames)
		{
			return new LocalizedString("AutoReseedNotAllCopiesPassive", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbNames
			});
		}

		public static LocalizedString MonitoringCouldNotFindHubServersException(string siteName, string adError)
		{
			return new LocalizedString("MonitoringCouldNotFindHubServersException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				siteName,
				adError
			});
		}

		public static LocalizedString FailedToFindDatabaseException(string databaseName)
		{
			return new LocalizedString("FailedToFindDatabaseException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString DbValidationCopyStatusTooOld(string dbCopyName, TimeSpan actualAgeOfStatus, TimeSpan maxAge)
		{
			return new LocalizedString("DbValidationCopyStatusTooOld", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopyName,
				actualAgeOfStatus,
				maxAge
			});
		}

		public static LocalizedString CouldNotCreateDbDirectoriesException(string database, string volumeName, string errMsg)
		{
			return new LocalizedString("CouldNotCreateDbDirectoriesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				database,
				volumeName,
				errMsg
			});
		}

		public static LocalizedString DatabaseCopyLayoutException(string errorMsg)
		{
			return new LocalizedString("DatabaseCopyLayoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString DatabaseLogCorruptRecoveryFailed(string dbName, string msg)
		{
			return new LocalizedString("DatabaseLogCorruptRecoveryFailed", "ExC836D7", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				msg
			});
		}

		public static LocalizedString MonitoringADLookupTimeoutException(int timeoutMs)
		{
			return new LocalizedString("MonitoringADLookupTimeoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				timeoutMs
			});
		}

		public static LocalizedString SeederEcDirDoesNotExist(string tempSeedDirectory)
		{
			return new LocalizedString("SeederEcDirDoesNotExist", "Ex5EB5A5", false, true, ReplayStrings.ResourceManager, new object[]
			{
				tempSeedDirectory
			});
		}

		public static LocalizedString FailedToKillProcessForServiceException(string serviceName, string msg)
		{
			return new LocalizedString("FailedToKillProcessForServiceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serviceName,
				msg
			});
		}

		public static LocalizedString SeederOperationAborted
		{
			get
			{
				return new LocalizedString("SeederOperationAborted", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JetErrorFileIOBeyondEOFException(string pageno)
		{
			return new LocalizedString("JetErrorFileIOBeyondEOFException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				pageno
			});
		}

		public static LocalizedString DatabaseCopySuspendException(string dbName, string server, string msg)
		{
			return new LocalizedString("DatabaseCopySuspendException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbName,
				server,
				msg
			});
		}

		public static LocalizedString DatabaseNotMounted(string dbName)
		{
			return new LocalizedString("DatabaseNotMounted", "Ex9F64C8", false, true, ReplayStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString ExchangeVolumeInfoInitException(string volumeName, string errMsg)
		{
			return new LocalizedString("ExchangeVolumeInfoInitException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				errMsg
			});
		}

		public static LocalizedString CopyPageFailed(int pageno, string source)
		{
			return new LocalizedString("CopyPageFailed", "ExDA5292", false, true, ReplayStrings.ResourceManager, new object[]
			{
				pageno,
				source
			});
		}

		public static LocalizedString ReplayTestStoreConnectivityTimedoutException(string operationName, string errorMsg)
		{
			return new LocalizedString("ReplayTestStoreConnectivityTimedoutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				operationName,
				errorMsg
			});
		}

		public static LocalizedString ReplayServiceSuspendRpcInvalidForSingleCopyException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceSuspendRpcInvalidForSingleCopyException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString MonitoringADConfigException(string errorMsg)
		{
			return new LocalizedString("MonitoringADConfigException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString FailedToOpenShipLogContextInvalidParameter
		{
			get
			{
				return new LocalizedString("FailedToOpenShipLogContextInvalidParameter", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogRepairFailedToCopyFromActive(string tempLogName, string exceptionText)
		{
			return new LocalizedString("LogRepairFailedToCopyFromActive", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				tempLogName,
				exceptionText
			});
		}

		public static LocalizedString ErrorDagDoesNotHaveAnyMemberServers
		{
			get
			{
				return new LocalizedString("ErrorDagDoesNotHaveAnyMemberServers", "Ex25A318", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagTaskValidateNodeTimedOutException(string serverName)
		{
			return new LocalizedString("DagTaskValidateNodeTimedOutException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString SeederEcStoreNotOnline(string sourceServerName)
		{
			return new LocalizedString("SeederEcStoreNotOnline", "ExCD6093", false, true, ReplayStrings.ResourceManager, new object[]
			{
				sourceServerName
			});
		}

		public static LocalizedString AmRegistryException(string apiName)
		{
			return new LocalizedString("AmRegistryException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				apiName
			});
		}

		public static LocalizedString ReplayServiceSuspendInvalidDuringMoveException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceSuspendInvalidDuringMoveException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString ReplayServiceRestartInvalidDuringMoveException(string dbCopy)
		{
			return new LocalizedString("ReplayServiceRestartInvalidDuringMoveException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				dbCopy
			});
		}

		public static LocalizedString ClusterBatchWriter_OpenClusterRootKeyFailed(int ec)
		{
			return new LocalizedString("ClusterBatchWriter_OpenClusterRootKeyFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString AmBcsException(string bcsError)
		{
			return new LocalizedString("AmBcsException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				bcsError
			});
		}

		public static LocalizedString FileSharingViolationOnSourceException(string serverName, string fileFullPath)
		{
			return new LocalizedString("FileSharingViolationOnSourceException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				serverName,
				fileFullPath
			});
		}

		public static LocalizedString NetworkRemoteError(string nodeName, string messageText)
		{
			return new LocalizedString("NetworkRemoteError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				nodeName,
				messageText
			});
		}

		public static LocalizedString DbVolumeInvalidDirectoriesException(string volumeName, string mountedFolder, int numExpected, int numActual)
		{
			return new LocalizedString("DbVolumeInvalidDirectoriesException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				volumeName,
				mountedFolder,
				numExpected,
				numActual
			});
		}

		public static LocalizedString LogInspectorFailed(string errorMsg)
		{
			return new LocalizedString("LogInspectorFailed", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString AmBcsTargetServerActivationIntraSite(string targetServer, string sourceServer, string targetSite, string sourceSite)
		{
			return new LocalizedString("AmBcsTargetServerActivationIntraSite", "ExA93FD4", false, true, ReplayStrings.ResourceManager, new object[]
			{
				targetServer,
				sourceServer,
				targetSite,
				sourceSite
			});
		}

		public static LocalizedString AmBcsTargetNodeDebugOptionEnabled(string server, string debugOptions)
		{
			return new LocalizedString("AmBcsTargetNodeDebugOptionEnabled", "Ex05A3FC", false, true, ReplayStrings.ResourceManager, new object[]
			{
				server,
				debugOptions
			});
		}

		public static LocalizedString ErrorAmInjectedUnknownConfig
		{
			get
			{
				return new LocalizedString("ErrorAmInjectedUnknownConfig", "Ex46DB23", false, true, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GranularReplicationTerminated(string reason)
		{
			return new LocalizedString("GranularReplicationTerminated", "ExBDC931", false, true, ReplayStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AutoReseedException(string errorMsg)
		{
			return new LocalizedString("AutoReseedException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString FailedToGetDatabaseInfo(int error)
		{
			return new LocalizedString("FailedToGetDatabaseInfo", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CiSeederSearchCatalogRpcTransientException(string message)
		{
			return new LocalizedString("CiSeederSearchCatalogRpcTransientException", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString AutoReseedFailedReseedBlocked(string error)
		{
			return new LocalizedString("AutoReseedFailedReseedBlocked", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString DisableReplayLagOperationName
		{
			get
			{
				return new LocalizedString("DisableReplayLagOperationName", "", false, false, ReplayStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FileCheckInternalError(string condition)
		{
			return new LocalizedString("FileCheckInternalError", "", false, false, ReplayStrings.ResourceManager, new object[]
			{
				condition
			});
		}

		public static LocalizedString GetLocalizedString(ReplayStrings.IDs key)
		{
			return new LocalizedString(ReplayStrings.stringIDs[(uint)key], ReplayStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(150);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Cluster.Replay.Strings", typeof(ReplayStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			RepairStateClusterIsNotRunning = 800943078U,
			RemoveLogReasonE00OutOfDate = 3145329300U,
			FailedToOpenShipLogContextAccessDenied = 2169511412U,
			ReplayServiceRpcUnknownInstanceException = 3896662565U,
			LockOwnerIdle = 1265493030U,
			IncSeedNotSupportedWithShrinkDatabaseError = 1348435424U,
			CantStartFromCommandLineTitle = 1095788331U,
			AmBcsDatabaseHasNoCopies = 1390940095U,
			ErrorCouldNotFindClusterGroupOwnerNodeForAmConfig = 1065254054U,
			SeederEcDBNotFound = 615086071U,
			SeederEcSeedingCancelled = 276770850U,
			DagTaskInstalledFailoverClustering = 3718483653U,
			SeederEcDeviceNotReady = 2876590454U,
			ErrorReadingDagServerForAmConfig = 3794143290U,
			NetworkReadEOF = 3273471454U,
			RemoveLogReasonFailedInspection = 3630272863U,
			InvalidLogCopyResponse = 3925066867U,
			MonitoringADInitNotCompleteException = 2936934256U,
			AutoReseedFailedToFindVolumeName = 861304653U,
			DagTaskComponentManagerAnotherInstanceRunning = 1749232750U,
			AutoReseedManualReseedLaunched = 4014700033U,
			SeederEcNoOnlineEdb = 3988781731U,
			FullServerSeedInProgressException = 3096281422U,
			ReplayServiceSuspendCommentException = 418608118U,
			InvalidDbForSeedSpecifiedException = 856033660U,
			VolumeMountPathDoesNotExistException = 798341136U,
			AmDbOperationWaitFailedException = 1105864708U,
			ErrorAmConfigNotInitialized = 2630456743U,
			NetworkCorruptDataGeneric = 2364081162U,
			ErrorAutomountConsensusNotReached = 2266299742U,
			AmBcsNoneSpecified = 1607989356U,
			SeederEcNotEnoughDiskException = 3766459347U,
			NetworkFailedToAuthServer = 1148446483U,
			ErrorFailedToFindLocalServer = 4045836413U,
			FailedToOpenShipLogContextDatabaseNotMounted = 4106538885U,
			ReplayServiceSuspendBlockedAcllException = 910204805U,
			DatabaseCopyLayoutTableNullException = 1295385018U,
			AmServiceShuttingDown = 820683245U,
			DagTaskDagIpAddressesMustBeIpv4Exception = 512707820U,
			UnknownError = 3351215994U,
			EseBackFileSystemCorruption = 3572810924U,
			SuspendOperationName = 214622620U,
			PagePatchLegacyFileExistsException = 3342720985U,
			SeederEcSuccess = 606338949U,
			NoServices = 1055054291U,
			StoreServiceMonitorCriticalError = 1298429885U,
			CannotChangeProperties = 3887748188U,
			SeederEchrInvalidCallSequence = 2034469932U,
			FailedToOpenShipLogContextEseCircularLoggingEnabled = 3043738041U,
			ReplayServiceResumeRpcFailedException = 2591444232U,
			NetworkSecurityFailed = 1344658319U,
			ReplayServiceSuspendRpcFailedException = 3184656137U,
			SuspendWantedWriteFailedException = 1163467430U,
			ReplayServiceSuspendResumeBlockedException = 3512548730U,
			AmDbActionDismountFailedException = 1378783691U,
			AutoReseedNeverMountedWorkflowReason = 387713678U,
			AutoReseedLogAndDbNotOnSameVolume = 1967903804U,
			FullServerSeedSkippedShutdownException = 85017254U,
			SeederEcOverlappedWriteErr = 2554265426U,
			FailToCleanUpFiles = 3580573960U,
			SeederEcNotEnoughDisk = 2045268632U,
			MonitoringADServiceShuttingDownException = 1045878879U,
			DbHTServiceShuttingDownException = 1612966754U,
			NullDbCopyException = 4049732327U,
			ErrorClusterServiceNotRunningForAmConfig = 2602551968U,
			SeederEcError = 3128283390U,
			AutoReseedFailedAdminSuspended = 129692820U,
			NetworkNoUsableEndpoints = 726910307U,
			ClusterServiceMonitorCriticalError = 3772399040U,
			Resynchronizing = 1871702564U,
			ReplayServiceSuspendBlockedResynchronizingException = 3929636263U,
			LockOwnerComponent = 828128533U,
			NetworkIsDisabled = 4232351636U,
			ResumeOperationName = 707284339U,
			ReplayServiceSuspendReseedBlockedException = 2777244007U,
			SuspendMessageWriteFailedException = 544686542U,
			SyncSuspendResumeOperationName = 627050854U,
			FailedAndSuspended = 1279119335U,
			TPRProviderNotListening = 1839653327U,
			Suspended = 3099361335U,
			ReplayServiceSuspendInPlaceReseedBlockedException = 4137367259U,
			AutoReseedMoveActiveBeforeRebuildCatalog = 3167054525U,
			ErrorCouldNotConnectClusterForAmConfig = 448095525U,
			ReplayServiceShuttingDownException = 3631647333U,
			ErrorFailedToOpenClusterObjects = 2056943490U,
			FailedToOpenShipLogContextStoreStopped = 4042526291U,
			NullDatabaseException = 1622537741U,
			SeederEcCommunicationsError = 865952845U,
			DagTaskPamNotMovedSubsequentOperationsMayBeSlowOrUnreliable = 1445478759U,
			SeederEcFailAcqRight = 482041519U,
			ProgressStatusInProgress = 204895011U,
			CouldNotGetMountStatus = 3439296139U,
			AmClusterNotRunningException = 3630311467U,
			LockOwnerConfigChecker = 2719597889U,
			PrepareToStopCalled = 200627337U,
			ReplayServiceSuspendWantedClearedException = 394420742U,
			DBCHasNoValidTargetEdbPath = 913564495U,
			DeleteChkptReasonCorrupted = 682288005U,
			TPRChangeFailedBecauseNotDismounted = 3559163736U,
			CouldNotFindVolumeForFormatException = 3778987676U,
			CannotChangeName = 2242986398U,
			NetworkManagerInitError = 3293194015U,
			PassiveCopyDisconnected = 1105077015U,
			SeederEchrErrorFromESECall = 3691652467U,
			ReplayServiceSuspendBlockedBackupInProgressException = 2139980255U,
			ErrorCouldNotFindServerForAmConfig = 3673369909U,
			FailedAtReplacingLogFiles = 2812191260U,
			ReplayServiceResumeRpcFailedSeedingException = 887012549U,
			StoreNotOnline = 3025714949U,
			LockOwnerAttemptCopyLastLogs = 2151299999U,
			MsexchangereplLong = 833784388U,
			AmOperationInvalidForStandaloneRoleException = 2787647009U,
			JetErrorDatabaseNotFound = 1556772377U,
			SeederEcBackupInProgress = 2325224058U,
			LogCopierFailedToGetSuspendLock = 511004165U,
			NetworkDataOverflowGeneric = 3508567603U,
			TPRNotEnabled = 4084429598U,
			LockOwnerSuspend = 458303596U,
			DbValidationFullCopyStatusResultsLabel = 1791510079U,
			LogRepairNotPossibleInsuffientToCheckDivergence = 3318672153U,
			DagTaskComponentManagerWantsToRebootException = 1539727809U,
			NetworkCancelled = 3985107625U,
			ErrorFailedToGetClusterCoreGroup = 413167004U,
			NotInPendingState = 1348455624U,
			ErrorInvalidPamServerName = 662858283U,
			SeederEchrErrorFromCallbackCall = 2070740709U,
			EnableReplayLagOperationName = 4031202440U,
			SeederInstanceAlreadyFailedException = 2490829887U,
			ReplayServiceSuspendWantedSetException = 854026562U,
			SeederEcOOMem = 1857824471U,
			DbValidationCopyStatusNameLabel = 399717229U,
			SeederEchrRestoreAtFileLevel = 3239149605U,
			TPRNotYetStarted = 2097320554U,
			SeederEcInvalidInput = 560332821U,
			ErrorRemoteSiteNotConnected = 3655345505U,
			ErrorLocalNodeNotUpYet = 4251585129U,
			DagTaskClusteringRequiresEnterpriseSkuException = 1556943668U,
			MsexchangesearchLong = 844399741U,
			LockOwnerBackup = 3809273542U,
			AmDbActionMoveFailedException = 1718068369U,
			Failed = 1054423051U,
			GranularReplicationOverflow = 3438214222U,
			CantStartFromCommandLine = 3142177591U,
			AmDbActionMountFailedException = 1944472637U,
			InvalidSavedStateException = 2389546758U,
			SeederOperationAborted = 1836563684U,
			FailedToOpenShipLogContextInvalidParameter = 3897089617U,
			ErrorDagDoesNotHaveAnyMemberServers = 2203487748U,
			ErrorAmInjectedUnknownConfig = 1450196582U,
			DisableReplayLagOperationName = 663986635U
		}

		private enum ParamIDs
		{
			SeederFailedToCreateDirectory,
			RepairStateLocalServerIsNotInDag,
			PagePatchFileDeletionException,
			AcllLastLogNotFoundException,
			ReplayServiceResumeInvalidDuringMoveException,
			DumpsterRedeliveryException,
			LastLogReplacementFileNotSubsetException,
			ReplayServiceRestartInvalidSeedingException,
			DagTaskComponentManagerGenericFailure,
			DumpsterCouldNotFindHubServerException,
			LastLogReplacementRollbackFailedException,
			AmPreMountCallbackFailedMountInhibitException,
			InvalidRcrConfigOnNonMailboxException,
			SeederEcLogAlreadyExist,
			AutoReseedTooManyConcurrentSeeds,
			FileCheckAccessDeniedDismountFailedException,
			GranularReplicationInitFailed,
			AmDbNotMountedMultipleServersException,
			FailedToGetCopyStatus,
			DisableReplayLagWriteFailedException,
			TargetDbNotThere,
			CiSeederExchangeSearchTransientException,
			PotentialRedundancyValidationDBReplicationStalled,
			LastLogReplacementTempOldFileNotDeletedException,
			PreserveInspectorLogsError,
			ServerStopped,
			SeederInstanceAlreadyAddedException,
			AutoReseedAllCatalogFailed,
			LogCopierE00InconsistentError,
			FailedToDisableMountPointConfigurationException,
			AcllFailedLogDivergenceDetected,
			AutoReseedFailedCopyWorkflowSuspendedCopy,
			CiSeederSearchCatalogRpcPermanentException,
			InvalidRCROperationOnNonRcrDB,
			CouldNotDeleteDbMountPointException,
			AutoReseedCatalogActiveException,
			SafeDeleteExistingFilesDataRedundancyNoResultException,
			NetworkAddressResolutionFailed,
			FileCheckDatabaseLogfileSignature,
			AcllCopyStatusResumeBlockedException,
			CiServiceDownException,
			AmBcsDatabaseCopyIsSeedingSource,
			FailedToDeleteTempDatabase,
			SeederFailedToInspectLogException,
			ClusterBatchWriter_CommitFailed,
			DbAvailabilityActiveCopyUnknownState,
			AmBcsSelectionException,
			TargetChkptAlreadyExists,
			DatabaseDismountOrKillStoreException,
			LogTruncationException,
			SeederFailedToFindDirectoriesUnderMountPoint,
			AmDbActionRejectedMountAtStartupNotEnabledException,
			DagNetworkRpcServerError,
			AmDbMoveOperationNotSupportedException,
			FailedToDeserializeStr,
			AmBcsDatabaseCopyCatalogUnhealthy,
			LogRepairFailedToVerifyFromActive,
			SafetyNetVersionCheckException,
			SuspendCommentTooLong,
			AmBcsDatabaseCopyTotalQueueLengthTooHigh,
			SeederOperationFailedException,
			AutoReseedInvalidLogFolderPath,
			AmBcsDatabaseCopyAlreadyTried,
			InsufficientSparesForExtraCopiesException,
			AcllCouldNotControlReplicaInstanceException,
			DagTaskClusteringMustBeInstalledException,
			DagTaskRemoveNodeNotUpException,
			DagTaskComponentManagerServerManagerPSFailure,
			CouldNotFindDatabase,
			DbMoveSkippedBecauseNotFoundInClusDb,
			DagTaskSetDagNeedsAllNodesUpToChangeQuorumException,
			AmInvalidActionCodeException,
			ReplayConfigNotFoundException,
			DatabaseLogFoldersNotUnderMountpoint,
			ReplayServiceTooMuchMemoryNoDumpException,
			NetworkNameNotFound,
			AmMountCallbackFailedWithDBFolderNotUnderMountPointException,
			RegistryParameterReadException,
			IncrementalReseedFailedException,
			FailedToNotifySourceLogTrunc,
			FileCheckInvalidDatabaseState,
			SeederFailedToDeleteCheckpoint,
			LogRepairNotPossibleActiveIsDivergent,
			FailedToOpenBackupFileHandle,
			DeleteChkptReasonTooFarBehindAndLogMissing,
			NetworkNotUsable,
			AutoReseedWorkflowNotSupportedOnTPR,
			ReplayLagRpcUnsupportedException,
			RepairStateDatabaseShouldBeDismounted,
			RegistryParameterKeyNotOpenedException,
			DagTaskServerException,
			ReplayServiceRpcCopyStatusTimeoutException,
			AutoReseedTooManyFailedCopies,
			CiSeederCatalogCouldNotDismount,
			CancelSeedingDueToFailed,
			IOBufferPoolLimitError,
			AutoReseedInPlaceReseedTooSoon,
			AmBcsDagNotFoundInAd,
			AmBcsDatabaseCopyResynchronizing,
			FoundTooManyVolumesWithSameVolumeLabelException,
			AmDbMoveOperationOnTimeoutFailureCancelled,
			DbFixupFailedVolumeHasMaxDbMountPointsException,
			MissingActiveLogRequiredForDivergenceDetection,
			ClusterBatchWriter_FailedToReadClusterRegistry,
			AutoReseedCatalogSkipRebuild,
			SeederInstanceAlreadyInProgressException,
			FileCheckUnableToDeleteCheckpointError,
			AcllInvalidForSingleCopyException,
			CiSeederRpcOperationFailedException,
			MonitoringADConfigStaleException,
			EseutilParseError,
			AmClusterEvictWithoutCleanupException,
			FileSystemCorruptionDetected,
			DbValidationPassiveCopyUnhealthyState,
			LogCopierE00MissingPrevious,
			LogDriveNotBigEnough,
			DagTaskAddingServerToDag,
			DatabaseNotFound,
			AmBcsDatabaseCopyFailed,
			AmDbRemountSkippedSinceMasterChanged,
			AutoReseedNoExchangeVolumesConfigured,
			AmDatabaseNameNotFoundException,
			NoInstancesFoundForManagementPath,
			LastLogReplacementFailedUnexpectedFileFoundException,
			ExchangeVolumeInfoMultipleDbMountPointsException,
			MonitoringServerSiteIsNullException,
			AmServiceMonitorSystemShutdownException,
			DatabaseFailedToGetVolumeInfo,
			LogRepairFailedTransient,
			AcllCopyStatusInvalidException,
			DatabaseGroupNotSetException,
			AcllBackupInProgressException,
			AmDbMoveOperationNoLongerApplicableException,
			SeederSuspendFailedException,
			CouldNotCreateDbMountPointFolderException,
			RemoteRegistryTimedOutException,
			MonitoringCouldNotFindDatabasesException,
			TargetDBAlreadyExists,
			SeederCatalogNotHealthyErr,
			ReplayServiceUnknownReplicaInstanceException,
			PagePatchFileReadException,
			SeedingChannelIsClosedException,
			ReplayDatabaseOperationTimedoutException,
			AutoReseedFailedSeedRetryExceeded,
			AcllLastLogTimeErrorException,
			SeederOperationFailedWithEcException,
			VolumeCouldNotBeReclaimedException,
			DbValidationNotEnoughCopies,
			RepairStateFailedToCreateTempLogFile,
			DagTaskRemoveDagServerMustHaveQuorumException,
			AcllFailedCurrentLogPresent,
			AutoReseedFailedToFindTargetVolumeName,
			AmBcsTargetServerIsHAComponentOffline,
			NoDivergedPointFound,
			SeederRpcServerLevelUnsupportedException,
			CouldNotCreateDbMountPointException,
			FailedToConfigureMountPointException,
			AmDismountSucceededButStillMounted,
			LogCopierInitFailedBecauseNoLogsOnSource,
			FailureItemRecoveryFailed,
			AcllCopyStatusFailedException,
			LastLogReplacementTooManyTempFilesException,
			AmBcsTargetServerActivationDisabled,
			ManagementApiError,
			KernelWatchdogTimerError,
			AmDbActionTransientException,
			AmBcsActiveCopyIsSeedingSource,
			NetworkAddressResolutionFailedNoDnsEntry,
			AmBcsDatabaseCopyInitializing,
			RepairStateFailedPendingPagePatchException,
			NetworkTimeoutError,
			CouldNotFindVolumeException,
			AmCommonException,
			DagTaskRemoteOperationLogEnd,
			RepairStateDatabaseIsActive,
			DirectoryEnumeratorIOError,
			NetworkTransportError,
			DagTaskMovedPam,
			LogInspectorE00OutOfSequence,
			LogCopierInitFailedActiveTruncatingException,
			AmFailedToDetermineDatabaseMountStatus,
			ReplayFailedToFindServerRpcVersionException,
			AmBcsDatabaseCopyActivationSuspended,
			IncrementalReseedRetryableException,
			AmMountBlockedOnStandaloneDbWithMissingEdbException,
			SeederInstanceAlreadyCompletedException,
			CouldNotFindDagObjectLookupErrorForServer,
			ServiceName,
			AmClusterNodeNotFoundException,
			AutoReseedFailedResumeBlocked,
			SeederFailedToFindValidVolumeInfo,
			SeederEcJtxAlreadyExist,
			SeederFailedToDeleteDatabase,
			DagTaskComponentManagerServerManagerCmdFailure,
			FailedToGetDiskSpace,
			DbRedundancyValidationErrorsOccurred,
			AcllUnboundedDatalossDetectedException,
			ClusterBatchWriter_BatchAddCommandFailed,
			SeedingSourceReplicaInstanceNotFoundException,
			AmBcsDatabaseCopySuspended,
			FileCheckLogfileCreationTime,
			ReplayLagManagerException,
			DatabaseVolumeInfoInitException,
			ReplayDbOperationTransientException,
			VolumeFormatFailedException,
			AutoReseedNoCopiesException,
			ReplayDatabaseOperationCancelledException,
			CiStatusIsFailed,
			NetworkEndOfData,
			DbAvailabilityActiveCopyDismountedError,
			NetworkReadTimeout,
			CiSeederSearchCatalogException,
			TPRExchangeNotListening,
			AmClusterEventNotifierTransientException,
			MissingPassiveLogRequiredForDivergenceDetection,
			AmBcsTargetServerPreferredMaxActivesExceeded,
			AmClusterNodeJoinedException,
			AmBcsTargetServerPreferredMaxActivesReached,
			AmDbLockConflict,
			FileCheck,
			LogInspectorGenerationMismatch,
			SeederInstanceNotFoundException,
			FailedToOpenLogTruncContext,
			DagTaskOperationFailedWithEcException,
			DagTaskServerTransientException,
			ReplayServiceSuspendRpcPartialSuccessCatalogFailedException,
			PathIsAlreadyAValidMountPoint,
			AmLastLogPropertyCorruptedException,
			CouldNotFindDagObjectForServer,
			TagHandlerFormatMsgFailed,
			IncrementalReseedPrereqException,
			MonitoredDatabaseInitFailure,
			AmBcsTargetServerActivationBlocked,
			RepairStateDatabaseCopyShouldBeSuspended,
			InvalidRcrConfigAlreadyHostsDb,
			RegistryParameterException,
			FailedToDeserializeDumpsterRequestStrException,
			SeederEcUndefined,
			CiSeederGenericException,
			FileCheckEDBMissing,
			AcllCopyIsNotViableErrorException,
			DatabaseVolumeInfoException,
			ReplayServiceResumeBlockedException,
			ReplaySystemOperationTimedoutException,
			AmBcsSourceServerADError,
			AmDbActionWrapperTransientException,
			FileCheckAccessDenied,
			ActiveRecoveryNotApplicableException,
			SeederServerException,
			AutoReseedCatalogIsBehindRetry,
			AutoReseedWrongNumberOfCopiesOnVolume,
			SourceLogBreakStallsPassiveError,
			TagHandlerSuspendCopy,
			TPRExchangeListenerNotResponding,
			EnableReplayLagAlreadyDisabledFailedException,
			IncSeedDivergenceCheckFailedException,
			TPRInitFailure,
			FileCheckRequiredGenerationCorrupt,
			AmCommonTransientException,
			ClusterBatchWriter_OpenClusterFailed,
			CIStatusFailedException,
			DumpsterSafetyNetRpcFailedException,
			AmDbActionException,
			InstanceSuspendedAutoInitialSeed,
			ReseedCheckMissingLogfile,
			LogCopierFailsBecauseLogGap,
			LogDirectoryCreationDisabled,
			DbAvailabilityActiveCopyMountState,
			LogGapDetected,
			FileCheckEseutilError,
			ClusterBatchWriter_CreateBatchFailed,
			AmRefreshConfigTimeoutError,
			AmDbOperationException,
			DbCopyNotTargetException,
			LogRepairDivergenceCheckFailedDueToCorruptEndOfLog,
			AmBcsDatabaseCopyHostedOnTarget,
			SearchProxyRpcException,
			AutoReseedCatalogIsBehindBacklog,
			CiSeederCatalogCouldNotPause,
			TPRChangeFailedBecauseAlreadyActive,
			DatabaseValidationException,
			FileCheckLogfileMissing,
			FileCheckLogfileSignature,
			FileCheckJustCreatedEDB,
			AmDbOperationTimedoutException,
			AmDbActionWrapperException,
			UnexpectedEOF,
			DbDriveNotBigEnough,
			PagePatchApiFailedException,
			AutoReseedCatalogToUpgrade,
			RlmDatabaseCopyInvalidException,
			LogRepairDivergenceCheckFailedError,
			DbValidationActiveCopyStatusUnknown,
			SeederReplayServiceDownException,
			DbMoveSkippedBecauseNotActive,
			LogInspectorSignatureMismatch,
			GranularReplicationMsgSequenceError,
			PagePatchTooManyPagesToPatchException,
			LogRepairNotPossible,
			AmRoleChangedWhileOperationIsInProgress,
			AmPreMountCallbackFailedNoReplicaInstanceException,
			DagTaskMovingPam,
			ReplayServiceSuspendRpcInvalidForActiveCopyException,
			ReplayServiceTooMuchMemoryException,
			LogInspectorCouldNotMoveLogFileException,
			ServerMoveAllDatabasesFailed,
			CouldNotMoveLogFile,
			DagTaskInstallingFailoverClustering,
			AmBcsDatabaseCopyIsHAComponentOffline,
			CopyStatusIsNotHealthy,
			AmLastServerTimeStampCorruptedException,
			PagePatchInvalidPageSizeException,
			FileStateInternalError,
			SeederRpcUnsupportedException,
			WarningPerformingFastOperationException,
			SeedInProgressException,
			AmMountTimeoutError,
			LastLogReplacementTempNewFileNotDeletedException,
			CiSeederExchangeSearchPermanentException,
			ReplayServiceTooManyHandlesException,
			AmBcsTargetServerIsStoppedOnDAC,
			DagTaskNetFtProblem,
			AmDbRemountSkippedSinceDatabaseWasAdminDismounted,
			DagTaskNotEnoughStaticIPAddresses,
			AmClusterException,
			SeedDivergenceFailedException,
			SeederInstanceReseedBlockedException,
			DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException,
			ReplayServiceSuspendRpcInvalidSeedingSourceException,
			FailedToTruncateLocallyException,
			ReplayDbOperationException,
			AmDbActionRejectedAdminDismountedException,
			ExchangeVolumeInfoMultipleExMountPointsException,
			SeederInstanceInvalidStateForEndException,
			ServerNotFoundException,
			SeederServerTransientException,
			ReplayConfigPropException,
			PagePatchInvalidFileException,
			SeedPrepareException,
			FileCheckRequiredLogfileGapException,
			DeleteChkptReasonForce,
			DbValidationInspectorQueueLengthTooHigh,
			AutoReseedPrereqFailedException,
			CopyUnknownToActiveLogTruncationException,
			AutoReseedFailedInPlaceReseedBlocked,
			TPRChangeFailedServerValidation,
			AmDbMoveSkippedSinceMasterChanged,
			HungDetectionGumIdChanged,
			SeederInstanceAlreadyCancelledException,
			CancelSeedingDueToConfigChangeOrServiceShutdown,
			ClusterNetworkNullSubnetError,
			TPRProviderNotResponding,
			SpareConflictInLayoutException,
			ErrorDagMisconfiguredForAmConfig,
			AmBcsFailedToQueryCopiesException,
			ReplayServiceTooManyThreadsException,
			AmDbActionCancelledException,
			FileIOonSourceException,
			FileOpenError,
			SeederEcTargetDbFileInUse,
			AutoReseedUnhandledException,
			InvalidDbStateForIncReseed,
			LastLogReplacementUnexpectedTempFilesException,
			AmBcsDatabaseCopySeeding,
			AmInvalidDbState,
			AmBcsGetCopyStatusRpcException,
			AmDbMoveOperationSkippedException,
			SeederFailedToFindDirectory,
			NetworkUnexpectedMessage,
			ClusterBatchWriter_OpenActiveManagerKeyFailed,
			SeederRpcSafeDeleteUnsupportedException,
			AutoReseedNotAllCopiesOnVolumeFailedSuspended,
			LastLogReplacementFailedFileNotFoundException,
			DatabaseNotHealthyOnVolume,
			DagTaskComputerAccountCouldNotBeValidatedException,
			AmDbActionRejectedLastAdminActionDidNotSucceedException,
			AcllInvalidForActiveCopyException,
			DumpsterCouldNotReadMaxDumpsterTimeException,
			AmPreMountCallbackFailedNoReplicaInstanceErrorException,
			MonitoringCouldNotFindDagException,
			AcllAlreadyRunningException,
			AcllCopyIsNotViableException,
			DatabaseFailoverFailedException,
			VolumeRecentlyModifiedException,
			DagTaskQuorumNotAchievedException,
			OperationTimeoutErr,
			DagTaskFormingClusterToLog,
			ErrorNullServerFromDb,
			TPREnabledInvalidOperation,
			AcllSetCurrentLogGenerationException,
			FailedToReadDatabasePage,
			DagTaskRemoteOperationLogData,
			AmBcsTargetServerMaxActivesReached,
			NetworkCorruptData,
			LogInspectorFailedGeneral,
			ReplayServiceRpcArgumentException,
			AcllFailedException,
			NetworkRemoteErrorUnknown,
			FileCheckCorruptFile,
			DagTaskFileShareWitnessResourceIsStillNotOnlineException,
			DeleteChkptReasonTooAdvanced,
			AmDbOperationAttempedTooSoonException,
			AmTransientException,
			AmGetServiceProcessFailed,
			SourceDatabaseNotFound,
			DatabaseRemountFailedException,
			LogRepairUnexpectedVerifyError,
			EseLogEnumeratorIOError,
			UnExpectedPageSize,
			ErrorCouldNotConnectNativeClusterForAmConfig,
			DbHTFirstLookupTimeoutException,
			AmBcsTargetNodeDownError,
			RegistryParameterWriteException,
			DbAvailabilityValidationErrorsOccurred,
			MonitoringCouldNotFindMiniServerException,
			ReplayDbOperationWrapperException,
			FailedToFindLocalServerException,
			CallWithoutnumberOfExtraCopiesOnSparesException,
			AmBcsTargetServerADError,
			ReplaySystemOperationCancelledException,
			ServiceNotRunningOnNodeException,
			LogFileCheckError,
			ServerIsNotJoinedYet,
			LastLogGenerationTimeStampStale,
			DbValidationDbNotReplicated,
			ReplayDbOperationWrapperTransientException,
			ReplayStoreOperationAbortedException,
			DBCNotSuspendedYet,
			AutoReseedThrottledException,
			DbValidationActiveCopyStatusRpcFailed,
			InvalidVolumeMissingException,
			DagTaskJoinedNodeToCluster,
			CouldNotGetMountStatusError,
			LastLogReplacementException,
			DatabasesMissingInCopyStatusLookUpTable,
			IncSeedConfigNotSupportedError,
			SeederFailedToDeleteLogs,
			ReplayServiceResumeRpcInvalidForActiveCopyException,
			TPRmmediateDismountFailed,
			RepairStateError,
			CannotBeginSeedingInstanceNotInStateException,
			RepairStateDatabaseNotReplicated,
			MonitoringADFirstLookupTimeoutException,
			AmBcsManagedAvailabilityCheckFailed,
			ReplayServiceSyncStateInvalidDuringMoveException,
			DatabasePageSizeUnexpected,
			AmBcsDatabaseCopyQueueLengthTooHigh,
			DagTaskFswNeedsCnoPermissionException,
			ReplayServiceResumeRpcPartialSuccessCatalogFailedException,
			ReplayServiceCouldNotFindReplayConfigException,
			SeederCopyNotSuspended,
			DagTaskFormingClusterProgress,
			NetworkConnectionTimeout,
			DbFixupFailedException,
			AmPreMountCallbackFailedException,
			AmBcsSingleCopyValidationException,
			AutoReseedCatalogSourceException,
			AmMountBlockedDbMountedBeforeWithMissingEdbException,
			AutoReseedInvalidEdbFolderPath,
			MissingLogRequired,
			SeederFailedToDeployDatabase,
			PreferFullReseed,
			DagReplayServiceDownException,
			AutoReseedFailedResumeRetryExceeded,
			AcllTempLogCreationFailedException,
			NetworkCommunicationError,
			SetBrokenWatsonException,
			LogRepairRetryCountExceeded,
			FileCheckLogfileGeneration,
			FailedToGetProcessForServiceException,
			DatabasesMissingInADException,
			VolumeNotSafeForFormatException,
			AmFailedToReadClusdbException,
			AcllLossDeterminationFailedException,
			DatabaseHealthTrackerException,
			DagTaskJoiningNodeToCluster,
			LastLogReplacementFailedErrorException,
			DagTaskRemoteOperationLogBegin,
			AmBcsDatabaseCopyReplayQueueLengthTooHigh,
			CorruptLogDetectedError,
			SafeDeleteExistingFilesDataRedundancyException,
			DumpsterInvalidResubmitRequestException,
			FileCheckIsamError,
			AmClusterNoServerToConnect,
			DatabaseValidationNoCopiesException,
			ClusterBatchWriter_OpenCopyStateKeyFailed,
			FileCheckIOError,
			CouldNotFindSpareVolumeException,
			FileReadException,
			ReplayServiceResumeRpcInvalidForSingleCopyException,
			DbValidationCopyStatusRpcFailed,
			AmClusterFileNotFoundException,
			SeedingAnotherServerException,
			DagTaskOperationFailedException,
			AmDbMoveOperationNotSupportedStandaloneException,
			AutoReseedNotAllCopiesPassive,
			MonitoringCouldNotFindHubServersException,
			FailedToFindDatabaseException,
			DbValidationCopyStatusTooOld,
			CouldNotCreateDbDirectoriesException,
			DatabaseCopyLayoutException,
			DatabaseLogCorruptRecoveryFailed,
			MonitoringADLookupTimeoutException,
			SeederEcDirDoesNotExist,
			FailedToKillProcessForServiceException,
			JetErrorFileIOBeyondEOFException,
			DatabaseCopySuspendException,
			DatabaseNotMounted,
			ExchangeVolumeInfoInitException,
			CopyPageFailed,
			ReplayTestStoreConnectivityTimedoutException,
			ReplayServiceSuspendRpcInvalidForSingleCopyException,
			MonitoringADConfigException,
			LogRepairFailedToCopyFromActive,
			DagTaskValidateNodeTimedOutException,
			SeederEcStoreNotOnline,
			AmRegistryException,
			ReplayServiceSuspendInvalidDuringMoveException,
			ReplayServiceRestartInvalidDuringMoveException,
			ClusterBatchWriter_OpenClusterRootKeyFailed,
			AmBcsException,
			FileSharingViolationOnSourceException,
			NetworkRemoteError,
			DbVolumeInvalidDirectoriesException,
			LogInspectorFailed,
			AmBcsTargetServerActivationIntraSite,
			AmBcsTargetNodeDebugOptionEnabled,
			GranularReplicationTerminated,
			AutoReseedException,
			FailedToGetDatabaseInfo,
			CiSeederSearchCatalogRpcTransientException,
			AutoReseedFailedReseedBlocked,
			FileCheckInternalError
		}
	}
}
