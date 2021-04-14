using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MonitorResultLogConfiguration : ILogConfiguration
	{
		public MonitorResultLogConfiguration(string logType, string logComponentValue)
		{
			this.logPrefix = logType;
			this.logType = logType;
			this.logComponentValue = logType;
			this.IsLoggingEnabled = (Settings.IsResultsLoggingEnabled && (Datacenter.IsMicrosoftHostedOnly(false) || Datacenter.IsDatacenterDedicated(false)));
			if (this.IsLoggingEnabled)
			{
				string fullName = Assembly.GetEntryAssembly().FullName;
				if (string.IsNullOrEmpty(fullName) || !fullName.StartsWith("MSExchangeHMWorker", StringComparison.OrdinalIgnoreCase))
				{
					this.IsLoggingEnabled = false;
				}
			}
			if (this.IsLoggingEnabled)
			{
				this.LogPath = Settings.DefaultResultsLogPath;
				if (string.IsNullOrEmpty(this.LogPath))
				{
					try
					{
						this.LogPath = ExchangeSetupContext.InstallPath;
						if (string.IsNullOrEmpty(this.LogPath))
						{
							this.LogPath = "C:\\Program Files\\Microsoft\\Exchange Server\\V15";
						}
					}
					catch (Exception)
					{
						this.LogPath = "C:\\Program Files\\Microsoft\\Exchange Server\\V15";
					}
				}
				this.LogPath = Path.Combine(Path.Combine(this.LogPath, "Logging\\Monitoring\\Monitoring"), logType);
				this.MaxLogAge = TimeSpan.FromDays((double)Settings.MaxLogAge);
				this.MaxLogDirectorySizeInBytes = (long)Settings.MaxResultsLogDirectorySizeInBytes;
				this.MaxLogFileSizeInBytes = (long)Settings.MaxResultsLogFileSizeInBytes;
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public bool IsActivityEventHandler
		{
			get
			{
				return true;
			}
		}

		public string LogPath { get; private set; }

		public TimeSpan MaxLogAge { get; private set; }

		public long MaxLogDirectorySizeInBytes { get; private set; }

		public long MaxLogFileSizeInBytes { get; private set; }

		public string LogComponent
		{
			get
			{
				return this.logComponentValue;
			}
		}

		public string LogPrefix
		{
			get
			{
				return this.logPrefix;
			}
		}

		public string LogType
		{
			get
			{
				return this.logType;
			}
		}

		private const string DefaultLogDirectoryPath = "C:\\Program Files\\Microsoft\\Exchange Server\\V15";

		private readonly string logPrefix;

		private readonly string logType;

		private readonly string logComponentValue;
	}
}
