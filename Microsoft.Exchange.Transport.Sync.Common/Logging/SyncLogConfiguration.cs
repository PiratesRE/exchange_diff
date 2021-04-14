using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncLogConfiguration
	{
		public SyncLogConfiguration() : this(SyncLoggingLevel.None)
		{
		}

		public SyncLogConfiguration(SyncLoggingLevel syncLoggingLevel)
		{
			this.softwareVersion = Assembly.GetExecutingAssembly().GetName().Version;
			this.logFilePrefix = string.Empty;
			this.logComponent = "SyncLogs";
			this.logFilePath = Path.Combine(SyncLogConfiguration.exchangeInstallDirPath, SyncLogConfiguration.defaultRelativePath);
			this.ageQuotaInHours = 168L;
			this.directorySizeQuota = 256000L;
			this.perFileSizeQuota = 10240L;
			this.syncLoggingLevel = syncLoggingLevel;
		}

		public string SoftwareName
		{
			get
			{
				return "Microsoft Exchange Server";
			}
		}

		public Version SoftwareVersion
		{
			get
			{
				return this.softwareVersion;
			}
		}

		public string LogTypeName
		{
			get
			{
				return "Sync Logs";
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return this.logFilePrefix;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentNullOrEmpty("LogFilePrefix", value);
				this.logFilePrefix = value;
			}
		}

		public string LogComponent
		{
			get
			{
				return this.logComponent;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentNullOrEmpty("LogComponent", value);
				this.logComponent = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public string LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentNullOrEmpty("LogFilePath", value);
				this.logFilePath = Path.Combine(SyncLogConfiguration.exchangeInstallDirPath, value);
			}
		}

		public long AgeQuotaInHours
		{
			get
			{
				return this.ageQuotaInHours;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentLessThanZero("ageQuotaInHours", value);
				this.ageQuotaInHours = value;
			}
		}

		public long DirectorySizeQuota
		{
			get
			{
				return this.directorySizeQuota;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentLessThanZero("DirectorySizeQuota", value);
				this.directorySizeQuota = value;
			}
		}

		public long PerFileSizeQuota
		{
			get
			{
				return this.perFileSizeQuota;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentLessThanZero("PerFileSizeQuota", value);
				this.perFileSizeQuota = value;
			}
		}

		public SyncLoggingLevel SyncLoggingLevel
		{
			get
			{
				return this.syncLoggingLevel;
			}
			set
			{
				this.syncLoggingLevel = value;
			}
		}

		public LogSchema CreateLogSchema(string[] fields)
		{
			return new LogSchema(this.SoftwareName, this.SoftwareVersion.ToString(), this.LogTypeName, fields);
		}

		private const string SoftwareNameValue = "Microsoft Exchange Server";

		private const string LogTypeNameValue = "Sync Logs";

		private static readonly string exchangeInstallDirPath = Assembly.GetExecutingAssembly().Location + "\\..\\..\\";

		private static readonly string defaultRelativePath = "TransportRoles\\Logs\\SyncLog";

		private Version softwareVersion;

		private string logFilePrefix;

		private string logComponent;

		private bool enabled;

		private string logFilePath;

		private long ageQuotaInHours;

		private long directorySizeQuota;

		private long perFileSizeQuota;

		private SyncLoggingLevel syncLoggingLevel;
	}
}
