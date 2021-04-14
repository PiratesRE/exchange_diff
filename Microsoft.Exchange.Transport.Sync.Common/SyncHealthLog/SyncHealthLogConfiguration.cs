using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncHealthLogConfiguration
	{
		public SyncHealthLogConfiguration()
		{
			this.syncHealthLogMaxAge = TimeSpan.FromDays(14.0);
			this.syncHealthLogMaxFile = 10485760L;
			this.syncHealthLogMaxDirectorySize = 10737418240L;
			this.syncHealthLogPath = Path.Combine(SyncHealthLogConfiguration.exchangeInstallDirPath, "TransportRoles\\Logs\\SyncHealth");
		}

		public bool SyncHealthLogEnabled
		{
			get
			{
				return this.syncHealthLogEnabled;
			}
			set
			{
				this.syncHealthLogEnabled = value;
			}
		}

		public string SyncHealthLogPath
		{
			get
			{
				return this.syncHealthLogPath;
			}
			set
			{
				this.syncHealthLogPath = value;
			}
		}

		public TimeSpan SyncHealthLogMaxAge
		{
			get
			{
				return this.syncHealthLogMaxAge;
			}
			set
			{
				this.syncHealthLogMaxAge = value;
			}
		}

		public long SyncHealthLogMaxFileSize
		{
			get
			{
				return this.syncHealthLogMaxFile;
			}
			set
			{
				this.syncHealthLogMaxFile = value;
			}
		}

		public long SyncHealthLogMaxDirectorySize
		{
			get
			{
				return this.syncHealthLogMaxDirectorySize;
			}
			set
			{
				this.syncHealthLogMaxDirectorySize = value;
			}
		}

		public static SyncHealthLogConfiguration CreateSyncHubHealthLogConfiguration(Server server)
		{
			SyncUtilities.ThrowIfArgumentNull("server", server);
			if (!server.IsHubTransportServer)
			{
				throw new ArgumentException("Should be Hub Transport Server", "server");
			}
			SyncHealthLogConfiguration syncHealthLogConfiguration = new SyncHealthLogConfiguration();
			syncHealthLogConfiguration.SyncHealthLogEnabled = server.TransportSyncHubHealthLogEnabled;
			syncHealthLogConfiguration.SyncHealthLogMaxAge = server.TransportSyncHubHealthLogMaxAge;
			syncHealthLogConfiguration.SyncHealthLogMaxDirectorySize = (long)((double)server.TransportSyncHubHealthLogMaxDirectorySize);
			syncHealthLogConfiguration.SyncHealthLogMaxFileSize = (long)((double)server.TransportSyncHubHealthLogMaxFileSize);
			if (server.TransportSyncHubHealthLogFilePath != null && !string.IsNullOrEmpty(server.TransportSyncHubHealthLogFilePath.PathName))
			{
				syncHealthLogConfiguration.SyncHealthLogPath = server.TransportSyncHubHealthLogFilePath.PathName;
			}
			else
			{
				syncHealthLogConfiguration.SyncHealthLogEnabled = false;
			}
			return syncHealthLogConfiguration;
		}

		public static SyncHealthLogConfiguration CreateSyncMailboxHealthLogConfiguration(Server server)
		{
			SyncUtilities.ThrowIfArgumentNull("server", server);
			if (!server.IsMailboxServer)
			{
				throw new ArgumentException("Should be Mailbox Server", "server");
			}
			SyncHealthLogConfiguration syncHealthLogConfiguration = new SyncHealthLogConfiguration();
			syncHealthLogConfiguration.SyncHealthLogEnabled = server.TransportSyncMailboxHealthLogEnabled;
			syncHealthLogConfiguration.SyncHealthLogMaxAge = server.TransportSyncMailboxHealthLogMaxAge;
			syncHealthLogConfiguration.SyncHealthLogMaxDirectorySize = (long)((double)server.TransportSyncMailboxHealthLogMaxDirectorySize);
			syncHealthLogConfiguration.SyncHealthLogMaxFileSize = (long)((double)server.TransportSyncMailboxHealthLogMaxFileSize);
			if (server.TransportSyncMailboxHealthLogFilePath != null && !string.IsNullOrEmpty(server.TransportSyncMailboxHealthLogFilePath.PathName))
			{
				syncHealthLogConfiguration.SyncHealthLogPath = server.TransportSyncMailboxHealthLogFilePath.PathName;
			}
			else
			{
				syncHealthLogConfiguration.SyncHealthLogEnabled = false;
			}
			return syncHealthLogConfiguration;
		}

		private const string DefaultRelativeSyncHealthLogPath = "TransportRoles\\Logs\\SyncHealth";

		private static readonly string exchangeInstallDirPath = Assembly.GetExecutingAssembly().Location + "\\..\\..\\";

		private bool syncHealthLogEnabled;

		private string syncHealthLogPath;

		private TimeSpan syncHealthLogMaxAge;

		private long syncHealthLogMaxFile;

		private long syncHealthLogMaxDirectorySize;
	}
}
