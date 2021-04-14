using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DatabaseCopyStatusEntry : IConfigurable
	{
		public ObjectId Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		public ADObjectId Id
		{
			get
			{
				return this.m_identity;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_databaseName;
			}
		}

		public CopyStatus Status
		{
			get
			{
				return this.m_copyStatus;
			}
			set
			{
				this.m_copyStatus = value;
			}
		}

		public DateTime? InstanceStartTime { get; internal set; }

		public DateTime? LastStatusTransitionTime
		{
			get
			{
				return this.m_lastStatusTransitionTime;
			}
		}

		public string MailboxServer
		{
			get
			{
				return this.m_mailboxServer;
			}
		}

		public string ActiveDatabaseCopy
		{
			get
			{
				return this.m_activeDatabaseCopy;
			}
		}

		public bool ActiveCopy
		{
			get
			{
				return this.m_isActiveCopy;
			}
			internal set
			{
				this.m_isActiveCopy = value;
			}
		}

		public int? ActivationPreference { get; internal set; }

		public DateTime? StatusRetrievedTime
		{
			get
			{
				return this.m_statusRetrievedTime;
			}
		}

		public int? WorkerProcessId
		{
			get
			{
				return this.m_workerProcessId;
			}
		}

		public bool? IsLastCopyAvailabilityChecksPassed { get; internal set; }

		public DateTime? LastCopyAvailabilityChecksPassedTime { get; internal set; }

		public bool? IsLastCopyRedundancyChecksPassed { get; internal set; }

		public DateTime? LastCopyRedundancyChecksPassedTime { get; internal set; }

		public bool ActivationSuspended
		{
			get
			{
				return this.m_activationSuspended;
			}
		}

		public ActionInitiatorType ActionInitiator
		{
			get
			{
				return this.m_actionInitiator;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
		}

		public uint? ErrorEventId
		{
			get
			{
				return this.m_errorEventId;
			}
		}

		public ExtendedErrorInfo ExtendedErrorInfo
		{
			get
			{
				return this.m_extendedErrorInfo;
			}
		}

		public string SuspendComment
		{
			get
			{
				return this.m_suspendMessage;
			}
		}

		public bool? RequiredLogsPresent
		{
			get
			{
				return this.m_requiredLogsPresent;
			}
		}

		public long SinglePageRestore
		{
			get
			{
				return this.m_singlePageRestore;
			}
		}

		public ContentIndexStatusType ContentIndexState
		{
			get
			{
				return this.m_contentIndexState;
			}
		}

		public string ContentIndexErrorMessage
		{
			get
			{
				return this.m_contentIndexErrorMessage;
			}
		}

		public int? ContentIndexErrorCode
		{
			get
			{
				return this.m_contentIndexErrorCode;
			}
		}

		public int? ContentIndexVersion
		{
			get
			{
				return this.m_contentIndexVersion;
			}
		}

		public int? ContentIndexBacklog
		{
			get
			{
				return this.m_contentIndexBacklog;
			}
		}

		public int? ContentIndexRetryQueueSize
		{
			get
			{
				return this.m_contentIndexRetryQueueSize;
			}
		}

		public int? ContentIndexMailboxesToCrawl
		{
			get
			{
				return this.m_contentIndexMailboxesToCrawl;
			}
		}

		public int? ContentIndexSeedingPercent
		{
			get
			{
				return this.m_contentIndexSeedingPercent;
			}
		}

		public string ContentIndexSeedingSource
		{
			get
			{
				return this.m_contentIndexSeedingSource;
			}
		}

		public string ContentIndexServerSource
		{
			get
			{
				string text = this.ContentIndexSeedingSource;
				if (string.IsNullOrEmpty(text))
				{
					return string.Empty;
				}
				int num = text.IndexOf(":");
				int num2 = text.IndexOf(".");
				int num3 = num2 - (num + 1);
				if (num2 != -1 && num3 > 0)
				{
					text = text.Substring(num + 1, num3);
				}
				return text;
			}
		}

		public long? CopyQueueLength
		{
			get
			{
				if (this.ActiveCopy)
				{
					return new long?(0L);
				}
				return new long?(Math.Max(0L, this.m_latestLogGenerationNumber - this.m_inspectorGenerationNumber));
			}
		}

		public long? ReplayQueueLength
		{
			get
			{
				return new long?(Math.Max(0L, this.m_inspectorGenerationNumber - this.m_replayGenerationNumber));
			}
		}

		public bool? ReplaySuspended
		{
			get
			{
				return this.m_replaySuspended;
			}
		}

		public bool? ResumeBlocked { get; internal set; }

		public bool? ReseedBlocked { get; internal set; }

		public string MinimumSupportedDatabaseSchemaVersion { get; internal set; }

		public string MaximumSupportedDatabaseSchemaVersion { get; internal set; }

		public string RequestedDatabaseSchemaVersion { get; internal set; }

		public DateTime? LatestAvailableLogTime
		{
			get
			{
				return this.m_latestAvailableLogTime;
			}
		}

		public DateTime? LastCopyNotificationedLogTime
		{
			get
			{
				return this.m_latestCopyNotificationTime;
			}
		}

		public DateTime? LastCopiedLogTime
		{
			get
			{
				return this.m_latestCopyTime;
			}
		}

		public DateTime? LastInspectedLogTime
		{
			get
			{
				return this.m_latestInspectorTime;
			}
		}

		public DateTime? LastReplayedLogTime
		{
			get
			{
				return this.m_latestReplayTime;
			}
		}

		public long LastLogGenerated
		{
			get
			{
				return this.m_latestLogGenerationNumber;
			}
		}

		public long LastLogCopyNotified
		{
			get
			{
				return this.m_copyNotificationGenerationNumber;
			}
		}

		public long LastLogCopied
		{
			get
			{
				return this.m_copyGenerationNumber;
			}
		}

		public long LastLogInspected
		{
			get
			{
				return this.m_inspectorGenerationNumber;
			}
		}

		public long LastLogReplayed
		{
			get
			{
				return this.m_replayGenerationNumber;
			}
		}

		public long LowestLogPresent { get; set; }

		public bool LastLogInfoIsStale { get; set; }

		public DateTime? LastLogInfoFromCopierTime { get; set; }

		public DateTime? LastLogInfoFromClusterTime { get; set; }

		public long LastLogInfoFromClusterGen { get; set; }

		public bool ReplicationIsInBlockMode { get; set; }

		public bool ActivationDisabledAndMoveNow { get; set; }

		public DatabaseCopyAutoActivationPolicyType AutoActivationPolicy { get; set; }

		public long? LogsReplayedSinceInstanceStart
		{
			get
			{
				return this.m_logsReplayedSinceInstanceStart;
			}
		}

		public long? LogsCopiedSinceInstanceStart
		{
			get
			{
				return this.m_logsCopiedSinceInstanceStart;
			}
		}

		public DateTime? LatestFullBackupTime
		{
			get
			{
				return this.m_latestFullBackupTime;
			}
		}

		public DateTime? LatestIncrementalBackupTime
		{
			get
			{
				return this.m_latestIncrementalBackupTime;
			}
		}

		public DateTime? LatestDifferentialBackupTime
		{
			get
			{
				return this.m_latestDifferentialBackupTime;
			}
		}

		public DateTime? LatestCopyBackupTime
		{
			get
			{
				return this.m_latestCopyBackupTime;
			}
		}

		public bool? SnapshotBackup
		{
			get
			{
				return this.m_snapshotBackup;
			}
		}

		public bool? SnapshotLatestFullBackup
		{
			get
			{
				return this.m_snapshotLatestFullBackup;
			}
		}

		public bool? SnapshotLatestIncrementalBackup
		{
			get
			{
				return this.m_snapshotLatestIncrementalBackup;
			}
		}

		public bool? SnapshotLatestDifferentialBackup
		{
			get
			{
				return this.m_snapshotLatestDifferentialBackup;
			}
		}

		public bool? SnapshotLatestCopyBackup
		{
			get
			{
				return this.m_snapshotLatestCopyBackup;
			}
		}

		public bool? LogReplayQueueIncreasing
		{
			get
			{
				return new bool?(this.m_logReplayQueueIncreasing ?? false);
			}
		}

		public bool? LogCopyQueueIncreasing
		{
			get
			{
				return new bool?(this.m_logCopyQueueIncreasing ?? false);
			}
		}

		public ReplayLagStatus ReplayLagStatus
		{
			get
			{
				return this.m_replayLagStatus;
			}
		}

		public DatabaseSeedStatus DatabaseSeedStatus
		{
			get
			{
				return this.m_dbSeedStatus;
			}
		}

		public DumpsterRequestEntry[] OutstandingDumpsterRequests
		{
			get
			{
				return this.m_outstandingDumpsterRequests;
			}
		}

		public ConnectionStatus[] OutgoingConnections
		{
			get
			{
				return this.m_outgoingConnections;
			}
		}

		public ConnectionStatus IncomingLogCopyingNetwork
		{
			get
			{
				return this.m_incomingLogCopyingNetwork;
			}
		}

		public ConnectionStatus SeedingNetwork
		{
			get
			{
				return this.m_seedingNetwork;
			}
		}

		public int DiskFreeSpacePercent { get; internal set; }

		public ByteQuantifiedSize DiskFreeSpace { get; internal set; }

		public ByteQuantifiedSize DiskTotalSpace { get; internal set; }

		public string ExchangeVolumeMountPoint { get; internal set; }

		public string DatabaseVolumeMountPoint { get; internal set; }

		public string DatabaseVolumeName { get; internal set; }

		public bool? DatabasePathIsOnMountedFolder { get; internal set; }

		public string LogVolumeMountPoint { get; internal set; }

		public string LogVolumeName { get; internal set; }

		public bool? LogPathIsOnMountedFolder { get; internal set; }

		public string LastDatabaseVolumeName { get; internal set; }

		public DateTime? LastDatabaseVolumeNameTransitionTime { get; internal set; }

		public string VolumeInfoError { get; internal set; }

		public long MaxLogToReplay { get; internal set; }

		internal bool IsValid
		{
			get
			{
				return true;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return this.IsValid;
			}
		}

		internal ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return this.ObjectState;
			}
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		internal ADObjectId m_identity;

		internal string m_name;

		internal string m_databaseName;

		internal CopyStatus m_copyStatus;

		internal DateTime? m_lastStatusTransitionTime;

		internal string m_mailboxServer;

		internal string m_activeDatabaseCopy;

		private bool m_isActiveCopy;

		internal DateTime? m_statusRetrievedTime;

		internal int? m_workerProcessId;

		internal bool m_activationSuspended;

		internal ActionInitiatorType m_actionInitiator;

		internal string m_errorMessage;

		internal uint? m_errorEventId;

		internal ExtendedErrorInfo m_extendedErrorInfo;

		internal string m_suspendMessage;

		internal bool? m_requiredLogsPresent;

		internal long m_singlePageRestore;

		internal ContentIndexStatusType m_contentIndexState;

		internal string m_contentIndexErrorMessage;

		internal int? m_contentIndexErrorCode;

		internal int? m_contentIndexVersion;

		internal int? m_contentIndexBacklog;

		internal int? m_contentIndexRetryQueueSize;

		internal int? m_contentIndexMailboxesToCrawl;

		internal int? m_contentIndexSeedingPercent;

		internal string m_contentIndexSeedingSource;

		internal bool? m_replaySuspended;

		internal DateTime? m_latestAvailableLogTime;

		internal DateTime? m_latestCopyNotificationTime;

		internal DateTime? m_latestCopyTime;

		internal DateTime? m_latestInspectorTime;

		internal DateTime? m_latestReplayTime;

		internal long m_latestLogGenerationNumber;

		internal long m_copyNotificationGenerationNumber;

		internal long m_copyGenerationNumber;

		internal long m_inspectorGenerationNumber;

		internal long m_replayGenerationNumber;

		internal long? m_logsReplayedSinceInstanceStart;

		internal long? m_logsCopiedSinceInstanceStart;

		internal DateTime? m_latestFullBackupTime;

		internal DateTime? m_latestIncrementalBackupTime;

		internal DateTime? m_latestDifferentialBackupTime;

		internal DateTime? m_latestCopyBackupTime;

		internal bool? m_snapshotBackup;

		internal bool? m_snapshotLatestFullBackup;

		internal bool? m_snapshotLatestIncrementalBackup;

		internal bool? m_snapshotLatestDifferentialBackup;

		internal bool? m_snapshotLatestCopyBackup;

		internal bool? m_logReplayQueueIncreasing;

		internal bool? m_logCopyQueueIncreasing;

		internal ReplayLagStatus m_replayLagStatus;

		internal DatabaseSeedStatus m_dbSeedStatus;

		internal DumpsterRequestEntry[] m_outstandingDumpsterRequests;

		internal ConnectionStatus[] m_outgoingConnections;

		internal ConnectionStatus m_incomingLogCopyingNetwork;

		internal ConnectionStatus m_seedingNetwork;
	}
}
