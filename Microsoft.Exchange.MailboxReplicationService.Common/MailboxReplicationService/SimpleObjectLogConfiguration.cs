using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SimpleObjectLogConfiguration : ObjectLogConfiguration
	{
		public SimpleObjectLogConfiguration(string logName, string logEnabledKey, string maxDirSizeKey, string maxFileSizeKey)
		{
			this.LogName = logName;
			this.LogEnabledKey = logEnabledKey;
			this.MaxDirSizeKey = maxDirSizeKey;
			this.MaxFileSizeKey = maxFileSizeKey;
			this.DirectoryName = string.Format("{0}s", this.LogName);
			this.logComponentName = string.Format("{0}{1}Log", this.ComponentName, this.LogName);
			this.filenamePrefix = string.Format("{0}_{1}_", this.ComponentName, this.LogName);
		}

		public override bool IsEnabled
		{
			get
			{
				return string.IsNullOrEmpty(this.LogEnabledKey) || ConfigBase<MRSConfigSchema>.GetConfig<bool>(this.LogEnabledKey);
			}
		}

		public override TimeSpan MaxLogAge
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("MaxLogAge");
			}
		}

		public override bool FlushToDisk
		{
			get
			{
				return true;
			}
		}

		public override TimeSpan StreamFlushInterval
		{
			get
			{
				return TimeSpan.FromMinutes(5.0);
			}
		}

		public override string LoggingFolder
		{
			get
			{
				string text = ConfigBase<MRSConfigSchema>.GetConfig<string>("LoggingPath");
				if (string.IsNullOrEmpty(text))
				{
					text = MRSConfigSchema.DefaultLoggingPath;
				}
				return Path.Combine(text, this.DirectoryName);
			}
		}

		public string DirectoryName { get; private set; }

		public override string LogComponentName
		{
			get
			{
				return this.logComponentName;
			}
		}

		public override string FilenamePrefix
		{
			get
			{
				return this.filenamePrefix;
			}
		}

		public override long MaxLogDirSize
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<long>(this.MaxDirSizeKey);
			}
		}

		public override long MaxLogFileSize
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<long>(this.MaxFileSizeKey);
			}
		}

		protected virtual string ComponentName
		{
			get
			{
				return "MRS";
			}
		}

		private protected string LogName { protected get; private set; }

		private protected string LogEnabledKey { protected get; private set; }

		private protected string MaxDirSizeKey { protected get; private set; }

		private protected string MaxFileSizeKey { protected get; private set; }

		private const string FilenamePrefixFormat = "{0}_{1}_";

		private const string LogComponentFormat = "{0}{1}Log";

		private const string DirectoryNameFormat = "{0}s";

		private readonly string logComponentName;

		private readonly string filenamePrefix;
	}
}
