using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal sealed class RpcDatabaseCopyStatus2 : RpcDatabaseCopyStatusBasic
	{
		public RpcDatabaseCopyStatus2(RpcDatabaseCopyStatus s)
		{
			Guid dbguid = s.DBGuid;
			this.m_dbGuid = dbguid;
			this.m_mailboxServer = s.MailboxServer;
			this.m_activeDatabaseCopy = s.ActiveDatabaseCopy;
			DateTime lastInspectedLogTime = s.LastInspectedLogTime;
			this.m_dataProtectionTime = lastInspectedLogTime;
			DateTime lastReplayedLogTime = s.LastReplayedLogTime;
			this.m_dataAvailabilityTime = lastReplayedLogTime;
			this.m_lastLogGenerated = s.LastLogGenerated;
			this.m_lastLogCopied = s.LastLogCopied;
			this.m_lastLogInspected = s.LastLogInspected;
			this.m_lastLogReplayed = s.LastLogReplayed;
			this.m_serverVersion = s.ServerVersion;
			this.m_copyStatus = s.CopyStatus;
			this.m_ciCurrentness = s.CICurrentness;
			DateTime statusRetrievedTime = s.StatusRetrievedTime;
			this.m_statusRetrievedTime = statusRetrievedTime;
			if (!<Module>.?A0xa7ddfcdb.IsValidDateTime(this.m_statusRetrievedTime))
			{
				DateTime utcNow = DateTime.UtcNow;
				this.m_statusRetrievedTime = utcNow;
			}
			this.m_lastLogGeneratedTime = this.m_statusRetrievedTime;
			this.m_suspendComment = s.SuspendComment;
			this.m_dumpsterServers = s.DumpsterServers;
			this.m_actionInitiator = s.ActionInitiator;
			this.m_seedingSource = s.SeedingSource;
			this.m_dumpsterRequired = s.DumpsterRequired;
			DateTime dumpsterStartTime = s.DumpsterStartTime;
			this.m_dumpsterStartTime = dumpsterStartTime;
			DateTime dumpsterEndTime = s.DumpsterEndTime;
			this.m_dumpsterEndTime = dumpsterEndTime;
			DateTime latestAvailableLogTime = s.LatestAvailableLogTime;
			this.m_latestAvailableLogTime = latestAvailableLogTime;
			DateTime lastCopyNotifiedLogTime = s.LastCopyNotifiedLogTime;
			this.m_lastCopyNotifiedLogTime = lastCopyNotifiedLogTime;
			DateTime lastCopiedLogTime = s.LastCopiedLogTime;
			this.m_lastCopiedLogTime = lastCopiedLogTime;
			DateTime lastInspectedLogTime2 = s.LastInspectedLogTime;
			this.m_lastInspectedLogTime = lastInspectedLogTime2;
			DateTime lastReplayedLogTime2 = s.LastReplayedLogTime;
			this.m_lastReplayedLogTime = lastReplayedLogTime2;
			DateTime currentReplayLogTime = s.CurrentReplayLogTime;
			this.m_currentReplayLogTime = currentReplayLogTime;
			this.m_lastLogCopyNotified = s.LastLogCopyNotified;
			DateTime latestFullBackupTime = s.LatestFullBackupTime;
			this.m_latestFullBackupTime = latestFullBackupTime;
			DateTime latestIncrementalBackupTime = s.LatestIncrementalBackupTime;
			this.m_latestIncrementalBackupTime = latestIncrementalBackupTime;
			DateTime latestDifferentialBackupTime = s.LatestDifferentialBackupTime;
			this.m_latestDifferentialBackupTime = latestDifferentialBackupTime;
			DateTime latestCopyBackupTime = s.LatestCopyBackupTime;
			this.m_latestCopyBackupTime = latestCopyBackupTime;
			this.m_snapshotLatestFullBackup = s.SnapshotLatestFullBackup;
			this.m_snapshotLatestIncrementalBackup = s.SnapshotLatestIncrementalBackup;
			this.m_snapshotLatestDifferentialBackup = s.SnapshotLatestDifferentialBackup;
			this.m_snapshotLatestCopyBackup = s.SnapshotLatestCopyBackup;
			this.m_copyQueueNotKeepingUp = s.CopyQueueNotKeepingUp;
			this.m_replayQueueNotKeepingUp = s.ReplayQueueNotKeepingUp;
			this.m_contentIndexStatus = s.ContentIndexStatus;
			this.m_contentIndexErrorMessage = s.ContentIndexErrorMessage;
			this.m_activationSuspended = s.ActivationSuspended;
			this.m_singlePageRestore = s.SinglePageRestore;
			this.m_singlePageRestoreNumber = s.SinglePageRestoreNumber;
			this.m_viable = s.Viable;
			this.m_lostWrite = s.LostWrite;
			this.m_outgoingConnections = s.OutgoingConnections;
			this.m_incomingLogCopyingNetwork = s.IncomingLogCopyingNetwork;
			this.m_seedingNetwork = s.SeedingNetwork;
			this.m_errorMessage = s.ErrorMessage;
			this.m_errorEventId = s.ErrorEventId;
			this.m_extendedErrorInfo = s.ExtendedErrorInfo;
			this.m_logsReplayedSinceInstanceStart = s.LogsReplayedSinceInstanceStart;
			this.m_logsCopiedSinceInstanceStart = s.LogsCopiedSinceInstanceStart;
		}

		public RpcDatabaseCopyStatus2()
		{
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator ==(RpcDatabaseCopyStatus2 left, RpcDatabaseCopyStatus2 right)
		{
			return object.ReferenceEquals(left, right) || (left != null && right != null && (left == right && <Module>.StringEqual(left.m_suspendComment, right.m_suspendComment) && <Module>.StringEqual(left.m_dumpsterServers, right.m_dumpsterServers) && left.m_instanceStartTime == right.m_instanceStartTime && left.m_lastStatusTransitionTime == right.m_lastStatusTransitionTime && left.m_dumpsterRequired == right.m_dumpsterRequired && left.m_dumpsterStartTime == right.m_dumpsterStartTime && left.m_dumpsterEndTime == right.m_dumpsterEndTime && left.m_latestAvailableLogTime == right.m_latestAvailableLogTime && left.m_lastCopyNotifiedLogTime == right.m_lastCopyNotifiedLogTime && left.m_lastCopiedLogTime == right.m_lastCopiedLogTime && left.m_lastReplayedLogTime == right.m_lastReplayedLogTime && left.m_lastInspectedLogTime == right.m_lastInspectedLogTime && left.m_currentReplayLogTime == right.m_currentReplayLogTime && left.m_lastLogGenerated == right.m_lastLogGenerated && left.m_lastLogCopyNotified == right.m_lastLogCopyNotified && left.m_latestFullBackupTime == right.m_latestFullBackupTime && left.m_latestIncrementalBackupTime == right.m_latestIncrementalBackupTime && left.m_latestDifferentialBackupTime == right.m_latestDifferentialBackupTime && left.m_latestCopyBackupTime == right.m_latestCopyBackupTime && left.m_snapshotLatestFullBackup == right.m_snapshotLatestFullBackup && left.m_snapshotLatestIncrementalBackup == right.m_snapshotLatestIncrementalBackup && left.m_snapshotLatestDifferentialBackup == right.m_snapshotLatestDifferentialBackup && left.m_snapshotLatestCopyBackup == right.m_snapshotLatestCopyBackup && left.m_copyQueueNotKeepingUp == right.m_copyQueueNotKeepingUp && left.m_replayQueueNotKeepingUp == right.m_replayQueueNotKeepingUp && left.m_viable == right.m_viable && left.m_isReplaySuspended == right.m_isReplaySuspended && left.m_resumeBlocked == right.m_resumeBlocked && left.m_reseedBlocked == right.m_reseedBlocked && left.m_workerProcessId == right.m_workerProcessId && left.m_nodeStatus == right.m_nodeStatus && left.m_configuredReplayLagTime == right.m_configuredReplayLagTime && left.m_actualReplayLagTime == right.m_actualReplayLagTime && left.m_replayLagEnabled == right.m_replayLagEnabled && left.m_replayLagPlayDownReason == right.m_replayLagPlayDownReason && left.m_replayLagPercentage == right.m_replayLagPercentage && left.m_singlePageRestore == right.m_singlePageRestore && left.m_singlePageRestoreNumber == right.m_singlePageRestoreNumber && left.m_activationSuspended == right.m_activationSuspended && left.m_lostWrite == right.m_lostWrite && left.m_contentIndexStatus == right.m_contentIndexStatus && <Module>.StringEqual(left.m_contentIndexErrorMessage, right.m_contentIndexErrorMessage) && Nullable.Equals<int>(left.m_contentIndexErrorCode, right.m_contentIndexErrorCode) && Nullable.Equals<int>(left.m_contentIndexVersion, right.m_contentIndexVersion) && Nullable.Equals<int>(left.m_contentIndexBacklog, right.m_contentIndexBacklog) && Nullable.Equals<int>(left.m_contentIndexRetryQueueSize, right.m_contentIndexRetryQueueSize) && Nullable.Equals<int>(left.m_contentIndexMailboxesToCrawl, right.m_contentIndexMailboxesToCrawl) && Nullable.Equals<int>(left.m_contentIndexSeedingPercent, right.m_contentIndexSeedingPercent) && <Module>.StringEqual(left.m_contentIndexSeedingSource, right.m_contentIndexSeedingSource) && left.m_dbSeedingPercent == right.m_dbSeedingPercent && left.m_dbSeedingKBytesRead == right.m_dbSeedingKBytesRead && left.m_dbSeedingKBytesWritten == right.m_dbSeedingKBytesWritten && (double)left.m_dbSeedingKBytesReadPerSec == (double)right.m_dbSeedingKBytesReadPerSec && (double)left.m_dbSeedingKBytesWrittenPerSec == (double)right.m_dbSeedingKBytesWrittenPerSec && left.m_actionInitiator == right.m_actionInitiator && left.m_seedingSource == right.m_seedingSource && <Module>.StringEqual(left.m_errorMessage, right.m_errorMessage) && left.m_errorEventId == right.m_errorEventId && left.m_extendedErrorInfo == right.m_extendedErrorInfo && left.m_logsReplayedSinceInstanceStart == right.m_logsReplayedSinceInstanceStart && left.m_logsCopiedSinceInstanceStart == right.m_logsCopiedSinceInstanceStart && left.m_activationPreference == right.m_activationPreference && left.m_diskFreeSpaceBytes == right.m_diskFreeSpaceBytes && left.m_diskTotalSpaceBytes == right.m_diskTotalSpaceBytes && left.m_diskFreeSpacePercent == right.m_diskFreeSpacePercent && left.m_lastDatabaseVolumeNameTransitionTime == right.m_lastDatabaseVolumeNameTransitionTime && <Module>.StringEqual(left.m_lastDatabaseVolumeName, right.m_lastDatabaseVolumeName) && <Module>.StringEqual(left.m_exchangeVolumeMountPoint, right.m_exchangeVolumeMountPoint) && <Module>.StringEqual(left.m_databaseVolumeMountPoint, right.m_databaseVolumeMountPoint) && <Module>.StringEqual(left.m_databaseVolumeName, right.m_databaseVolumeName) && left.m_isDbOnMountedFolder == right.m_isDbOnMountedFolder && <Module>.StringEqual(left.m_logVolumeMountPoint, right.m_logVolumeMountPoint) && <Module>.StringEqual(left.m_logVolumeName, right.m_logVolumeName) && left.m_isLogOnMountedFolder == right.m_isLogOnMountedFolder && <Module>.StringEqual(left.m_volumeInfoLastError, right.m_volumeInfoLastError)));
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator !=(RpcDatabaseCopyStatus2 left, RpcDatabaseCopyStatus2 right)
		{
			return ((!(left == right)) ? 1 : 0) != 0;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool IsOn(uint statusWord, uint bitsToTest)
		{
			return (((statusWord & bitsToTest) == bitsToTest) ? 1 : 0) != 0;
		}

		public static void TurnOn(ref uint statusWord, uint bitsToSet)
		{
			statusWord |= bitsToSet;
		}

		public static void TurnOff(ref uint statusWord, uint bitsToClear)
		{
			statusWord &= ~bitsToClear;
		}

		public void SetOrClearStatusBits(RpcDatabaseCopyStatus2.StatusBitsMask bitsToChange, [MarshalAs(UnmanagedType.U1)] bool turnBitsOn)
		{
			if (turnBitsOn)
			{
				this.m_statusBits |= (uint)bitsToChange;
			}
			else
			{
				this.m_statusBits &= (uint)(~(uint)bitsToChange);
			}
		}

		public bool ReplicationIsInBlockMode
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return (byte)(this.m_statusBits & 1U) != 0;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				if (value)
				{
					this.m_statusBits |= 1U;
				}
				else
				{
					this.m_statusBits &= 4294967294U;
				}
			}
		}

		public bool ActivationDisabledAndMoveNow
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return (byte)(this.m_statusBits >> 1 & 1U) != 0;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				if (value)
				{
					this.m_statusBits |= 2U;
				}
				else
				{
					this.m_statusBits &= 4294967293U;
				}
			}
		}

		public bool HAComponentOffline
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return (byte)(this.m_statusBits >> 2 & 1U) != 0;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				if (value)
				{
					this.m_statusBits |= 4U;
				}
				else
				{
					this.m_statusBits &= 4294967291U;
				}
			}
		}

		public bool IsPrimaryActiveManager
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return (byte)(this.m_statusBits >> 3 & 1U) != 0;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				if (value)
				{
					this.m_statusBits |= 8U;
				}
				else
				{
					this.m_statusBits &= 4294967287U;
				}
			}
		}

		public int AutoActivationPolicy
		{
			get
			{
				return this.m_serverAutoActivationPolicy;
			}
			set
			{
				this.m_serverAutoActivationPolicy = value;
			}
		}

		public DateTime InstanceStartTime
		{
			get
			{
				return this.m_instanceStartTime;
			}
			set
			{
				this.m_instanceStartTime = value;
			}
		}

		public DateTime LastStatusTransitionTime
		{
			get
			{
				return this.m_lastStatusTransitionTime;
			}
			set
			{
				this.m_lastStatusTransitionTime = value;
			}
		}

		public DateTime LastCopyAvailabilityChecksPassedTime
		{
			get
			{
				return this.m_lastCopyAvailabilityChecksPassedTime;
			}
			set
			{
				this.m_lastCopyAvailabilityChecksPassedTime = value;
			}
		}

		public DateTime LastCopyRedundancyChecksPassedTime
		{
			get
			{
				return this.m_lastCopyRedundancyChecksPassedTime;
			}
			set
			{
				this.m_lastCopyRedundancyChecksPassedTime = value;
			}
		}

		public bool IsLastCopyAvailabilityChecksPassed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isLastCopyAvailabilityChecksPassed;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isLastCopyAvailabilityChecksPassed = value;
			}
		}

		public bool IsLastCopyRedundancyChecksPassed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isLastCopyRedundancyChecksPassed;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isLastCopyRedundancyChecksPassed = value;
			}
		}

		public ContentIndexStatusType ContentIndexStatus
		{
			get
			{
				return this.m_contentIndexStatus;
			}
			set
			{
				this.m_contentIndexStatus = value;
			}
		}

		public string ContentIndexErrorMessage
		{
			get
			{
				return this.m_contentIndexErrorMessage;
			}
			set
			{
				this.m_contentIndexErrorMessage = value;
			}
		}

		public int? ContentIndexBacklog
		{
			get
			{
				return this.m_contentIndexBacklog;
			}
			set
			{
				this.m_contentIndexBacklog = value;
			}
		}

		public int? ContentIndexRetryQueueSize
		{
			get
			{
				return this.m_contentIndexRetryQueueSize;
			}
			set
			{
				this.m_contentIndexRetryQueueSize = value;
			}
		}

		public int? ContentIndexMailboxesToCrawl
		{
			get
			{
				return this.m_contentIndexMailboxesToCrawl;
			}
			set
			{
				this.m_contentIndexMailboxesToCrawl = value;
			}
		}

		public int? ContentIndexSeedingPercent
		{
			get
			{
				return this.m_contentIndexSeedingPercent;
			}
			set
			{
				this.m_contentIndexSeedingPercent = value;
			}
		}

		public string ContentIndexSeedingSource
		{
			get
			{
				return this.m_contentIndexSeedingSource;
			}
			set
			{
				this.m_contentIndexSeedingSource = value;
			}
		}

		public int? ContentIndexVersion
		{
			get
			{
				return this.m_contentIndexVersion;
			}
			set
			{
				this.m_contentIndexVersion = value;
			}
		}

		public int? ContentIndexErrorCode
		{
			get
			{
				return this.m_contentIndexErrorCode;
			}
			set
			{
				this.m_contentIndexErrorCode = value;
			}
		}

		public int DbSeedingPercent
		{
			get
			{
				return this.m_dbSeedingPercent;
			}
			set
			{
				this.m_dbSeedingPercent = value;
			}
		}

		public long DbSeedingKBytesRead
		{
			get
			{
				return this.m_dbSeedingKBytesRead;
			}
			set
			{
				this.m_dbSeedingKBytesRead = value;
			}
		}

		public long DbSeedingKBytesWritten
		{
			get
			{
				return this.m_dbSeedingKBytesWritten;
			}
			set
			{
				this.m_dbSeedingKBytesWritten = value;
			}
		}

		public float DbSeedingKBytesReadPerSec
		{
			get
			{
				return this.m_dbSeedingKBytesReadPerSec;
			}
			set
			{
				this.m_dbSeedingKBytesReadPerSec = value;
			}
		}

		public float DbSeedingKBytesWrittenPerSec
		{
			get
			{
				return this.m_dbSeedingKBytesWrittenPerSec;
			}
			set
			{
				this.m_dbSeedingKBytesWrittenPerSec = value;
			}
		}

		public bool ActivationSuspended
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_activationSuspended;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_activationSuspended = value;
			}
		}

		public bool Viable
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_viable;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_viable = value;
			}
		}

		public bool LostWrite
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_lostWrite;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_lostWrite = value;
			}
		}

		public bool ReplaySuspended
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isReplaySuspended;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isReplaySuspended = value;
			}
		}

		public bool ResumeBlocked
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_resumeBlocked;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_resumeBlocked = value;
			}
		}

		public bool ReseedBlocked
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_reseedBlocked;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_reseedBlocked = value;
			}
		}

		public bool InPlaceReseedBlocked
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_inPlaceReseedBlocked;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_inPlaceReseedBlocked = value;
			}
		}

		public int WorkerProcessId
		{
			get
			{
				return this.m_workerProcessId;
			}
			set
			{
				this.m_workerProcessId = value;
			}
		}

		public NodeUpStatusEnum NodeStatus
		{
			get
			{
				return this.m_nodeStatus;
			}
			set
			{
				this.m_nodeStatus = value;
			}
		}

		public TimeSpan ConfiguredReplayLagTime
		{
			get
			{
				return this.m_configuredReplayLagTime;
			}
			set
			{
				this.m_configuredReplayLagTime = value;
			}
		}

		public TimeSpan ActualReplayLagTime
		{
			get
			{
				return this.m_actualReplayLagTime;
			}
			set
			{
				this.m_actualReplayLagTime = value;
			}
		}

		public string ReplayLagDisabledReason
		{
			get
			{
				return this.m_replayLagDisabledReason;
			}
			set
			{
				this.m_replayLagDisabledReason = value;
			}
		}

		public ReplayLagEnabledEnum ReplayLagEnabled
		{
			get
			{
				return this.m_replayLagEnabled;
			}
			set
			{
				this.m_replayLagEnabled = value;
			}
		}

		public ReplayLagPlayDownReasonEnum ReplayLagPlayDownReason
		{
			get
			{
				return this.m_replayLagPlayDownReason;
			}
			set
			{
				this.m_replayLagPlayDownReason = value;
			}
		}

		public int ReplayLagPercentage
		{
			get
			{
				return this.m_replayLagPercentage;
			}
			set
			{
				this.m_replayLagPercentage = value;
			}
		}

		public bool SinglePageRestore
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_singlePageRestore;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_singlePageRestore = value;
			}
		}

		public long SinglePageRestoreNumber
		{
			get
			{
				return this.m_singlePageRestoreNumber;
			}
			set
			{
				this.m_singlePageRestoreNumber = value;
			}
		}

		public string SuspendComment
		{
			get
			{
				return this.m_suspendComment;
			}
			set
			{
				this.m_suspendComment = value;
			}
		}

		public ActionInitiatorType ActionInitiator
		{
			get
			{
				return this.m_actionInitiator;
			}
			set
			{
				this.m_actionInitiator = value;
			}
		}

		public string DumpsterServers
		{
			get
			{
				return this.m_dumpsterServers;
			}
			set
			{
				this.m_dumpsterServers = value;
			}
		}

		public bool DumpsterRequired
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_dumpsterRequired;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_dumpsterRequired = value;
			}
		}

		public DateTime DumpsterStartTime
		{
			get
			{
				return this.m_dumpsterStartTime;
			}
			set
			{
				this.m_dumpsterStartTime = value;
			}
		}

		public DateTime DumpsterEndTime
		{
			get
			{
				return this.m_dumpsterEndTime;
			}
			set
			{
				this.m_dumpsterEndTime = value;
			}
		}

		public DateTime LatestAvailableLogTime
		{
			get
			{
				return this.m_latestAvailableLogTime;
			}
			set
			{
				this.m_latestAvailableLogTime = value;
			}
		}

		public DateTime LastCopyNotifiedLogTime
		{
			get
			{
				return this.m_lastCopyNotifiedLogTime;
			}
			set
			{
				this.m_lastCopyNotifiedLogTime = value;
			}
		}

		public bool LastLogInfoIsStale
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_lastLogInfoIsStale;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_lastLogInfoIsStale = value;
			}
		}

		public DateTime LastLogInfoFromCopierTime
		{
			get
			{
				return this.m_lastLogInfoFromCopierTime;
			}
			set
			{
				this.m_lastLogInfoFromCopierTime = value;
			}
		}

		public DateTime LastLogInfoFromClusterTime
		{
			get
			{
				return this.m_lastLogInfoFromClusterTime;
			}
			set
			{
				this.m_lastLogInfoFromClusterTime = value;
			}
		}

		public long LastLogInfoFromClusterGen
		{
			get
			{
				return this.m_lastLogInfoFromClusterGen;
			}
			set
			{
				this.m_lastLogInfoFromClusterGen = value;
			}
		}

		public DateTime LastCopiedLogTime
		{
			get
			{
				return this.m_lastCopiedLogTime;
			}
			set
			{
				this.m_lastCopiedLogTime = value;
			}
		}

		public DateTime LastInspectedLogTime
		{
			get
			{
				return this.m_lastInspectedLogTime;
			}
			set
			{
				this.m_lastInspectedLogTime = value;
			}
		}

		public DateTime LastReplayedLogTime
		{
			get
			{
				return this.m_lastReplayedLogTime;
			}
			set
			{
				this.m_lastReplayedLogTime = value;
			}
		}

		public DateTime CurrentReplayLogTime
		{
			get
			{
				return this.m_currentReplayLogTime;
			}
			set
			{
				this.m_currentReplayLogTime = value;
			}
		}

		public long LastLogCopyNotified
		{
			get
			{
				return this.m_lastLogCopyNotified;
			}
			set
			{
				this.m_lastLogCopyNotified = value;
			}
		}

		public DateTime LatestFullBackupTime
		{
			get
			{
				return this.m_latestFullBackupTime;
			}
			set
			{
				this.m_latestFullBackupTime = value;
			}
		}

		public DateTime LatestIncrementalBackupTime
		{
			get
			{
				return this.m_latestIncrementalBackupTime;
			}
			set
			{
				this.m_latestIncrementalBackupTime = value;
			}
		}

		public DateTime LatestDifferentialBackupTime
		{
			get
			{
				return this.m_latestDifferentialBackupTime;
			}
			set
			{
				this.m_latestDifferentialBackupTime = value;
			}
		}

		public DateTime LatestCopyBackupTime
		{
			get
			{
				return this.m_latestCopyBackupTime;
			}
			set
			{
				this.m_latestCopyBackupTime = value;
			}
		}

		public bool SnapshotLatestFullBackup
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_snapshotLatestFullBackup;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_snapshotLatestFullBackup = value;
			}
		}

		public bool SnapshotLatestIncrementalBackup
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_snapshotLatestIncrementalBackup;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_snapshotLatestIncrementalBackup = value;
			}
		}

		public bool SnapshotLatestDifferentialBackup
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_snapshotLatestDifferentialBackup;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_snapshotLatestDifferentialBackup = value;
			}
		}

		public bool SnapshotLatestCopyBackup
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_snapshotLatestCopyBackup;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_snapshotLatestCopyBackup = value;
			}
		}

		public bool CopyQueueNotKeepingUp
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_copyQueueNotKeepingUp;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_copyQueueNotKeepingUp = value;
			}
		}

		public bool ReplayQueueNotKeepingUp
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_replayQueueNotKeepingUp;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_replayQueueNotKeepingUp = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
			set
			{
				this.m_errorMessage = value;
			}
		}

		public uint ErrorEventId
		{
			get
			{
				return this.m_errorEventId;
			}
			set
			{
				this.m_errorEventId = value;
			}
		}

		public ExtendedErrorInfo ExtendedErrorInfo
		{
			get
			{
				return this.m_extendedErrorInfo;
			}
			set
			{
				this.m_extendedErrorInfo = value;
				this.m_extendedErrorInfoBytes = SerializationServices.Serialize(value);
			}
		}

		public long LogsReplayedSinceInstanceStart
		{
			get
			{
				return this.m_logsReplayedSinceInstanceStart;
			}
			set
			{
				this.m_logsReplayedSinceInstanceStart = value;
			}
		}

		public long LogsCopiedSinceInstanceStart
		{
			get
			{
				return this.m_logsCopiedSinceInstanceStart;
			}
			set
			{
				this.m_logsCopiedSinceInstanceStart = value;
			}
		}

		public int ActivationPreference
		{
			get
			{
				return this.m_activationPreference;
			}
			set
			{
				this.m_activationPreference = value;
			}
		}

		public bool SeedingSource
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_seedingSource;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_seedingSource = value;
			}
		}

		public bool SeedingSourceForDB
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_seedingSourceForDB;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_seedingSourceForDB = value;
			}
		}

		public bool SeedingSourceForCI
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_seedingSourceForCI;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_seedingSourceForCI = value;
			}
		}

		public ulong DiskFreeSpaceBytes
		{
			get
			{
				return this.m_diskFreeSpaceBytes;
			}
			set
			{
				this.m_diskFreeSpaceBytes = value;
			}
		}

		public ulong DiskTotalSpaceBytes
		{
			get
			{
				return this.m_diskTotalSpaceBytes;
			}
			set
			{
				this.m_diskTotalSpaceBytes = value;
			}
		}

		public int DiskFreeSpacePercent
		{
			get
			{
				return this.m_diskFreeSpacePercent;
			}
			set
			{
				this.m_diskFreeSpacePercent = value;
			}
		}

		public string LastDatabaseVolumeName
		{
			get
			{
				return this.m_lastDatabaseVolumeName;
			}
			set
			{
				this.m_lastDatabaseVolumeName = value;
			}
		}

		public DateTime LastDatabaseVolumeNameTransitionTime
		{
			get
			{
				return this.m_lastDatabaseVolumeNameTransitionTime;
			}
			set
			{
				this.m_lastDatabaseVolumeNameTransitionTime = value;
			}
		}

		public string ExchangeVolumeMountPoint
		{
			get
			{
				return this.m_exchangeVolumeMountPoint;
			}
			set
			{
				this.m_exchangeVolumeMountPoint = value;
			}
		}

		public string DatabaseVolumeMountPoint
		{
			get
			{
				return this.m_databaseVolumeMountPoint;
			}
			set
			{
				this.m_databaseVolumeMountPoint = value;
			}
		}

		public string DatabaseVolumeName
		{
			get
			{
				return this.m_databaseVolumeName;
			}
			set
			{
				this.m_databaseVolumeName = value;
			}
		}

		public bool DatabasePathIsOnMountedFolder
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isDbOnMountedFolder;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isDbOnMountedFolder = value;
			}
		}

		public string LogVolumeMountPoint
		{
			get
			{
				return this.m_logVolumeMountPoint;
			}
			set
			{
				this.m_logVolumeMountPoint = value;
			}
		}

		public string LogVolumeName
		{
			get
			{
				return this.m_logVolumeName;
			}
			set
			{
				this.m_logVolumeName = value;
			}
		}

		public bool LogPathIsOnMountedFolder
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isLogOnMountedFolder;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isLogOnMountedFolder = value;
			}
		}

		public string VolumeInfoLastError
		{
			get
			{
				return this.m_volumeInfoLastError;
			}
			set
			{
				this.m_volumeInfoLastError = value;
			}
		}

		public int MinimumSupportedDatabaseSchemaVersion
		{
			get
			{
				return this.m_minSupportedSchemaVersion;
			}
			set
			{
				this.m_minSupportedSchemaVersion = value;
			}
		}

		public int MaximumSupportedDatabaseSchemaVersion
		{
			get
			{
				return this.m_maxSupportedSchemaVersion;
			}
			set
			{
				this.m_maxSupportedSchemaVersion = value;
			}
		}

		public int RequestedDatabaseSchemaVersion
		{
			get
			{
				return this.m_requestedSchemaVersion;
			}
			set
			{
				this.m_requestedSchemaVersion = value;
			}
		}

		public long LowestLogPresent
		{
			get
			{
				return this.m_lowestLogPresent;
			}
			set
			{
				this.m_lowestLogPresent = value;
			}
		}

		public long MaxLogToReplay
		{
			get
			{
				return this.m_maxLogToReplay;
			}
			set
			{
				this.m_maxLogToReplay = value;
			}
		}

		internal byte[] OutgoingConnections
		{
			get
			{
				return this.m_outgoingConnections;
			}
			set
			{
				this.m_outgoingConnections = value;
			}
		}

		internal byte[] IncomingLogCopyingNetwork
		{
			get
			{
				return this.m_incomingLogCopyingNetwork;
			}
			set
			{
				this.m_incomingLogCopyingNetwork = value;
			}
		}

		internal byte[] SeedingNetwork
		{
			get
			{
				return this.m_seedingNetwork;
			}
			set
			{
				this.m_seedingNetwork = value;
			}
		}

		internal byte[] ExtendedErrorInfoBytes
		{
			get
			{
				return this.m_extendedErrorInfoBytes;
			}
		}

		private string m_suspendComment;

		private string m_dumpsterServers;

		private ActionInitiatorType m_actionInitiator;

		private bool m_seedingSource;

		private bool m_dumpsterRequired;

		private DateTime m_dumpsterStartTime;

		private DateTime m_dumpsterEndTime;

		private DateTime m_latestAvailableLogTime;

		private DateTime m_lastCopyNotifiedLogTime;

		private DateTime m_lastCopiedLogTime;

		private DateTime m_lastInspectedLogTime;

		private DateTime m_lastReplayedLogTime;

		private DateTime m_currentReplayLogTime;

		private long m_lastLogCopyNotified;

		private DateTime m_latestFullBackupTime;

		private DateTime m_latestIncrementalBackupTime;

		private DateTime m_latestDifferentialBackupTime;

		private DateTime m_latestCopyBackupTime;

		private bool m_snapshotLatestFullBackup;

		private bool m_snapshotLatestIncrementalBackup;

		private bool m_snapshotLatestDifferentialBackup;

		private bool m_snapshotLatestCopyBackup;

		private bool m_copyQueueNotKeepingUp;

		private bool m_replayQueueNotKeepingUp;

		private ContentIndexStatusType m_contentIndexStatus;

		private string m_contentIndexErrorMessage;

		private int? m_contentIndexErrorCode;

		private int? m_contentIndexVersion;

		private int? m_contentIndexBacklog;

		private int? m_contentIndexRetryQueueSize;

		private int? m_contentIndexMailboxesToCrawl;

		private int? m_contentIndexSeedingPercent;

		private string m_contentIndexSeedingSource;

		private int m_dbSeedingPercent;

		private long m_dbSeedingKBytesRead;

		private long m_dbSeedingKBytesWritten;

		private float m_dbSeedingKBytesReadPerSec;

		private float m_dbSeedingKBytesWrittenPerSec;

		private bool m_activationSuspended;

		private bool m_singlePageRestore;

		private long m_singlePageRestoreNumber;

		private bool m_viable;

		private bool m_lostWrite;

		private bool m_isReplaySuspended;

		private int m_workerProcessId;

		private NodeUpStatusEnum m_nodeStatus;

		private TimeSpan m_configuredReplayLagTime;

		private TimeSpan m_actualReplayLagTime;

		private string m_replayLagDisabledReason;

		private ReplayLagEnabledEnum m_replayLagEnabled;

		private ReplayLagPlayDownReasonEnum m_replayLagPlayDownReason;

		private int m_replayLagPercentage;

		private byte[] m_outgoingConnections;

		private byte[] m_incomingLogCopyingNetwork;

		private byte[] m_seedingNetwork;

		private string m_errorMessage;

		private uint m_errorEventId;

		[NonSerialized]
		private ExtendedErrorInfo m_extendedErrorInfo;

		private byte[] m_extendedErrorInfoBytes;

		private long m_logsReplayedSinceInstanceStart;

		private long m_logsCopiedSinceInstanceStart;

		private bool m_lastLogInfoIsStale;

		private DateTime m_lastLogInfoFromCopierTime;

		private DateTime m_lastLogInfoFromClusterTime;

		private long m_lastLogInfoFromClusterGen;

		private int m_activationPreference;

		private ulong m_diskFreeSpaceBytes;

		private ulong m_diskTotalSpaceBytes;

		private int m_diskFreeSpacePercent;

		private DateTime m_lastDatabaseVolumeNameTransitionTime;

		private string m_lastDatabaseVolumeName;

		private string m_exchangeVolumeMountPoint;

		private string m_databaseVolumeMountPoint;

		private string m_databaseVolumeName;

		private bool m_isDbOnMountedFolder;

		private string m_logVolumeMountPoint;

		private string m_logVolumeName;

		private bool m_isLogOnMountedFolder;

		private string m_volumeInfoLastError;

		private DateTime m_lastStatusTransitionTime;

		private DateTime m_instanceStartTime;

		private DateTime m_lastCopyAvailabilityChecksPassedTime;

		private DateTime m_lastCopyRedundancyChecksPassedTime;

		private bool m_isLastCopyAvailabilityChecksPassed;

		private bool m_isLastCopyRedundancyChecksPassed;

		private bool m_resumeBlocked;

		private bool m_reseedBlocked;

		private bool m_inPlaceReseedBlocked;

		private int m_minSupportedSchemaVersion;

		private int m_maxSupportedSchemaVersion;

		private int m_requestedSchemaVersion;

		private long m_lowestLogPresent;

		private long m_maxLogToReplay;

		private int m_serverAutoActivationPolicy;

		private uint m_statusBits;

		private bool m_seedingSourceForDB;

		private bool m_seedingSourceForCI;

		[Flags]
		private enum StatusBitsMask : uint
		{
			None = 0U,
			BlockMode = 1U,
			ActivationDisabledAndMoveNow = 2U,
			HAComponentOffline = 4U,
			IsPrimaryActiveManager = 8U
		}
	}
}
