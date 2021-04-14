using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DatabaseInfo
	{
		public Guid MdbGuid { get; private set; }

		public string MdbName { get; private set; }

		public Guid DagOrServerGuid { get; private set; }

		public string FilePath { get; private set; }

		public string FileName { get; private set; }

		public string LogPath { get; private set; }

		public string LegacyDN { get; private set; }

		public TimeSpan EventHistoryRetentionPeriod { get; private set; }

		public bool IsPublicFolderDatabase { get; private set; }

		public bool IsRecoveryDatabase { get; private set; }

		public string Description { get; private set; }

		public string ServerName { get; private set; }

		public string ForestName { get; private set; }

		public SecurityDescriptor NTSecurityDescriptor { get; private set; }

		public bool CircularLoggingEnabled { get; private set; }

		public TimeSpan MailboxRetentionPeriod { get; private set; }

		public string[] HostServerNames { get; private set; }

		public bool AllowFileRestore { get; private set; }

		public DatabaseOptions Options
		{
			get
			{
				if (this.databaseOptions == null)
				{
					return null;
				}
				return this.databaseOptions.Clone();
			}
			private set
			{
				this.databaseOptions = ((value != null) ? value.Clone() : null);
			}
		}

		public QuotaInfo QuotaInfo { get; private set; }

		public int DataMoveReplicationConstraint { get; private set; }

		public DatabaseInfo(Guid mdbGuid, string mdbName, Guid dagOrServerGuid, string filePath, string fileName, string logPath, string legacyDN, TimeSpan eventHistoryRetentionPeriod, bool isPublicFolderDatabase, bool isRecoveryDatabase, string description, string serverName, SecurityDescriptor ntSecurityDescriptor, bool circularLoggingEnabled, bool allowFileRestore, TimeSpan mailboxRetentionPeriod, string[] hostServerNames, DatabaseOptions databaseOptions, QuotaInfo quotaInfo, int dataMoveReplicationConstraint, string forestName)
		{
			this.MdbGuid = mdbGuid;
			this.MdbName = mdbName;
			this.DagOrServerGuid = dagOrServerGuid;
			this.FilePath = filePath;
			this.FileName = fileName;
			this.LogPath = logPath;
			this.LegacyDN = legacyDN;
			this.EventHistoryRetentionPeriod = eventHistoryRetentionPeriod;
			this.IsPublicFolderDatabase = isPublicFolderDatabase;
			this.IsRecoveryDatabase = isRecoveryDatabase;
			this.Description = description;
			this.ServerName = serverName;
			this.NTSecurityDescriptor = ntSecurityDescriptor;
			this.CircularLoggingEnabled = circularLoggingEnabled;
			this.AllowFileRestore = allowFileRestore;
			this.MailboxRetentionPeriod = mailboxRetentionPeriod;
			this.HostServerNames = hostServerNames;
			this.Options = databaseOptions;
			this.QuotaInfo = quotaInfo;
			this.DataMoveReplicationConstraint = dataMoveReplicationConstraint;
			this.ForestName = forestName;
		}

		private DatabaseOptions databaseOptions;
	}
}
