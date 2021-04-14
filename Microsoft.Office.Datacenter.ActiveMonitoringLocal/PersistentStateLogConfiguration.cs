using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PersistentStateLogConfiguration : ILogConfiguration
	{
		public PersistentStateLogConfiguration(string logType, long maxLogDirectorySizeInBytes, long maxLogFileSizeInBytes)
		{
			this.logPrefix = logType;
			this.logType = logType;
			this.logComponentValue = logType;
			this.IsLoggingEnabled = Settings.IsPersistentStateEnabled;
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
							this.LogPath = "%systemdrive%\\Program Files\\Microsoft\\Exchange Server\\V15";
						}
					}
					catch (Exception)
					{
						this.LogPath = "%systemdrive%\\Program Files\\Microsoft\\Exchange Server\\V15";
					}
				}
				this.LogPath = Path.Combine(Path.Combine(this.LogPath, "Logging\\Monitoring\\PersistentState"), logType);
				this.MaxLogAge = TimeSpan.FromDays((double)Settings.MaxLogAge);
				this.MaxLogDirectorySizeInBytes = maxLogDirectorySizeInBytes;
				this.MaxLogFileSizeInBytes = maxLogFileSizeInBytes;
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

		private const string DefaultLogDirectoryPath = "%systemdrive%\\Program Files\\Microsoft\\Exchange Server\\V15";

		private readonly string logPrefix;

		private readonly string logType;

		private readonly string logComponentValue;
	}
}
