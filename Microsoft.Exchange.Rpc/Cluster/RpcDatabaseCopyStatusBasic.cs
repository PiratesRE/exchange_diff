using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal class RpcDatabaseCopyStatusBasic
	{
		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator ==(RpcDatabaseCopyStatusBasic left, RpcDatabaseCopyStatusBasic right)
		{
			return object.ReferenceEquals(left, right) || (left != null && right != null && (left.m_dbGuid == right.m_dbGuid && <Module>.StringEqual(left.m_dbName, right.m_dbName) && <Module>.StringEqual(left.m_mailboxServer, right.m_mailboxServer) && <Module>.StringEqual(left.m_activeDatabaseCopy, right.m_activeDatabaseCopy) && left.m_statusRetrievedTime == right.m_statusRetrievedTime && left.m_lastLogGeneratedTime == right.m_lastLogGeneratedTime && left.m_dataProtectionTime == right.m_dataProtectionTime && left.m_dataAvailabilityTime == right.m_dataAvailabilityTime && left.m_lastLogGenerated == right.m_lastLogGenerated && left.m_lastLogCopied == right.m_lastLogCopied && left.m_lastLogInspected == right.m_lastLogInspected && left.m_lastLogReplayed == right.m_lastLogReplayed && left.m_serverVersion == right.m_serverVersion && left.m_copyStatus == right.m_copyStatus && left.m_ciCurrentness == right.m_ciCurrentness));
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool operator !=(RpcDatabaseCopyStatusBasic left, RpcDatabaseCopyStatusBasic right)
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

		public string DBName
		{
			get
			{
				return this.m_dbName;
			}
			set
			{
				this.m_dbName = value;
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

		public DateTime LastLogGeneratedTime
		{
			get
			{
				return this.m_lastLogGeneratedTime;
			}
			set
			{
				this.m_lastLogGeneratedTime = value;
			}
		}

		public DateTime DataProtectionTime
		{
			get
			{
				return this.m_dataProtectionTime;
			}
			set
			{
				this.m_dataProtectionTime = value;
			}
		}

		public DateTime DataAvailabilityTime
		{
			get
			{
				return this.m_dataAvailabilityTime;
			}
			set
			{
				this.m_dataAvailabilityTime = value;
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

		[return: MarshalAs(UnmanagedType.U1)]
		internal bool IsActiveCopy()
		{
			string activeDatabaseCopy = this.m_activeDatabaseCopy;
			return string.Equals(this.m_mailboxServer, activeDatabaseCopy, StringComparison.OrdinalIgnoreCase);
		}

		internal long GetCopyQueueLength()
		{
			if (this.IsActiveCopy())
			{
				return 0L;
			}
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

		protected Guid m_dbGuid;

		protected string m_dbName;

		protected string m_mailboxServer;

		protected string m_activeDatabaseCopy;

		protected DateTime m_statusRetrievedTime;

		protected DateTime m_lastLogGeneratedTime;

		protected DateTime m_dataProtectionTime;

		protected DateTime m_dataAvailabilityTime;

		protected long m_lastLogGenerated;

		protected long m_lastLogCopied;

		protected long m_lastLogInspected;

		protected long m_lastLogReplayed;

		protected int m_serverVersion;

		protected CopyStatusEnum m_copyStatus;

		protected ContentIndexCurrentness m_ciCurrentness;
	}
}
