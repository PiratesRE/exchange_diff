using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal sealed class RpcDatabaseCopyStatus
	{
		public RpcDatabaseCopyStatus(RpcDatabaseCopyStatus2 s)
		{
			Guid dbguid = s.DBGuid;
			this.m_dbGuid = dbguid;
			this.m_mailboxServer = s.MailboxServer;
			this.m_activeDatabaseCopy = s.ActiveDatabaseCopy;
			DateTime statusRetrievedTime = s.StatusRetrievedTime;
			this.m_statusRetrievedTime = statusRetrievedTime;
			DateTime lastInspectedLogTime = s.LastInspectedLogTime;
			this.m_lastInspectedLogTime = lastInspectedLogTime;
			DateTime lastReplayedLogTime = s.LastReplayedLogTime;
			this.m_lastReplayedLogTime = lastReplayedLogTime;
			this.m_lastLogGenerated = s.LastLogGenerated;
			this.m_lastLogCopied = s.LastLogCopied;
			this.m_lastLogInspected = s.LastLogInspected;
			this.m_lastLogReplayed = s.LastLogReplayed;
			this.m_serverVersion = s.ServerVersion;
			this.m_copyStatus = s.CopyStatus;
			this.m_ciCurrentness = s.CICurrentness;
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

		public RpcDatabaseCopyStatus()
		{
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator ==(RpcDatabaseCopyStatus left, RpcDatabaseCopyStatus right)
		{
			return object.ReferenceEquals(left, right) || (left != null && right != null && (left.m_dbGuid == right.m_dbGuid && <Module>.StringEqual(left.m_suspendComment, right.m_suspendComment) && <Module>.StringEqual(left.m_mailboxServer, right.m_mailboxServer) && <Module>.StringEqual(left.m_dumpsterServers, right.m_dumpsterServers) && left.m_dumpsterRequired == right.m_dumpsterRequired && left.m_dumpsterStartTime == right.m_dumpsterStartTime && left.m_dumpsterEndTime == right.m_dumpsterEndTime && left.m_statusRetrievedTime == right.m_statusRetrievedTime && left.m_latestAvailableLogTime == right.m_latestAvailableLogTime && left.m_lastCopyNotifiedLogTime == right.m_lastCopyNotifiedLogTime && left.m_lastCopiedLogTime == right.m_lastCopiedLogTime && left.m_lastInspectedLogTime == right.m_lastInspectedLogTime && left.m_lastReplayedLogTime == right.m_lastReplayedLogTime && left.m_currentReplayLogTime == right.m_currentReplayLogTime && left.m_lastLogGenerated == right.m_lastLogGenerated && left.m_lastLogCopyNotified == right.m_lastLogCopyNotified && left.m_lastLogCopied == right.m_lastLogCopied && left.m_lastLogInspected == right.m_lastLogInspected && left.m_lastLogReplayed == right.m_lastLogReplayed && left.m_latestFullBackupTime == right.m_latestFullBackupTime && left.m_latestIncrementalBackupTime == right.m_latestIncrementalBackupTime && left.m_latestDifferentialBackupTime == right.m_latestDifferentialBackupTime && left.m_latestCopyBackupTime == right.m_latestCopyBackupTime && left.m_snapshotLatestFullBackup == right.m_snapshotLatestFullBackup && left.m_snapshotLatestIncrementalBackup == right.m_snapshotLatestIncrementalBackup && left.m_snapshotLatestDifferentialBackup == right.m_snapshotLatestDifferentialBackup && left.m_snapshotLatestCopyBackup == right.m_snapshotLatestCopyBackup && left.m_copyQueueNotKeepingUp == right.m_copyQueueNotKeepingUp && left.m_replayQueueNotKeepingUp == right.m_replayQueueNotKeepingUp && left.m_viable == right.m_viable && left.m_singlePageRestore == right.m_singlePageRestore && left.m_singlePageRestoreNumber == right.m_singlePageRestoreNumber && left.m_activationSuspended == right.m_activationSuspended && left.m_lostWrite == right.m_lostWrite && left.m_contentIndexStatus == right.m_contentIndexStatus && <Module>.StringEqual(left.m_contentIndexErrorMessage, right.m_contentIndexErrorMessage) && left.m_serverVersion == right.m_serverVersion && left.m_copyStatus == right.m_copyStatus && left.m_ciCurrentness == right.m_ciCurrentness && left.m_actionInitiator == right.m_actionInitiator && left.m_seedingSource == right.m_seedingSource && <Module>.StringEqual(left.m_errorMessage, right.m_errorMessage) && left.m_errorEventId == right.m_errorEventId && left.m_extendedErrorInfo == right.m_extendedErrorInfo && left.m_logsReplayedSinceInstanceStart == right.m_logsReplayedSinceInstanceStart && left.m_logsCopiedSinceInstanceStart == right.m_logsCopiedSinceInstanceStart));
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator !=(RpcDatabaseCopyStatus left, RpcDatabaseCopyStatus right)
		{
			return ((!(left == right)) ? 1 : 0) != 0;
		}

		public Guid DBGuid
		{
			get
			{
				return this.m_dbGuid;
			}
			set
			{
				this.m_dbGuid = value;
			}
		}

		public string MailboxServer
		{
			get
			{
				return this.m_mailboxServer;
			}
			set
			{
				this.m_mailboxServer = value;
			}
		}

		public string ActiveDatabaseCopy
		{
			get
			{
				return this.m_activeDatabaseCopy;
			}
			set
			{
				this.m_activeDatabaseCopy = value;
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

		public DateTime StatusRetrievedTime
		{
			get
			{
				return this.m_statusRetrievedTime;
			}
			set
			{
				this.m_statusRetrievedTime = value;
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

		public long LastLogGenerated
		{
			get
			{
				return this.m_lastLogGenerated;
			}
			set
			{
				this.m_lastLogGenerated = value;
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

		public long LastLogCopied
		{
			get
			{
				return this.m_lastLogCopied;
			}
			set
			{
				this.m_lastLogCopied = value;
			}
		}

		public long LastLogInspected
		{
			get
			{
				return this.m_lastLogInspected;
			}
			set
			{
				this.m_lastLogInspected = value;
			}
		}

		public long LastLogReplayed
		{
			get
			{
				return this.m_lastLogReplayed;
			}
			set
			{
				this.m_lastLogReplayed = value;
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

		public int ServerVersion
		{
			get
			{
				return this.m_serverVersion;
			}
			set
			{
				this.m_serverVersion = value;
			}
		}

		public CopyStatusEnum CopyStatus
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

		public ContentIndexCurrentness CICurrentness
		{
			get
			{
				return this.m_ciCurrentness;
			}
			set
			{
				this.m_ciCurrentness = value;
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

		internal long GetCopyQueueLength()
		{
			long lastLogGenerated = this.m_lastLogGenerated;
			long lastLogInspected = this.m_lastLogInspected;
			return Math.Max(0L, lastLogGenerated - lastLogInspected);
		}

		internal long GetReplayQueueLength()
		{
			long lastLogInspected = this.m_lastLogInspected;
			long lastLogReplayed = this.m_lastLogReplayed;
			return Math.Max(0L, lastLogInspected - lastLogReplayed);
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

		private Guid m_dbGuid;

		private string m_suspendComment;

		private string m_mailboxServer;

		private string m_dumpsterServers;

		private string m_activeDatabaseCopy;

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

		private long m_lastLogGenerated;

		private long m_lastLogCopyNotified;

		private long m_lastLogCopied;

		private long m_lastLogInspected;

		private long m_lastLogReplayed;

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

		private bool m_activationSuspended;

		private bool m_singlePageRestore;

		private long m_singlePageRestoreNumber;

		private bool m_viable;

		private bool m_lostWrite;

		private byte[] m_outgoingConnections;

		private byte[] m_incomingLogCopyingNetwork;

		private byte[] m_seedingNetwork;

		private int m_serverVersion;

		private CopyStatusEnum m_copyStatus;

		private ContentIndexCurrentness m_ciCurrentness;

		private string m_errorMessage;

		private uint m_errorEventId;

		private ExtendedErrorInfo m_extendedErrorInfo;

		private long m_logsReplayedSinceInstanceStart;

		private long m_logsCopiedSinceInstanceStart;

		private DateTime m_statusRetrievedTime;
	}
}
