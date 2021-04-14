using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MonitoringLogConfiguration : ILogConfiguration
	{
		public MonitoringLogConfiguration(string logRelativePath) : this(logRelativePath, MonitoringLogConfiguration.LogPrefixValue)
		{
		}

		public MonitoringLogConfiguration(string logRelativePath, string logPrefix)
		{
			this.IsLoggingEnabled = true;
			this.LogPath = Path.Combine(Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Monitoring\\"), logRelativePath);
			this.MaxLogAge = TimeSpan.FromDays(7.0);
			this.MaxLogDirectorySizeInBytes = (long)ByteQuantifiedSize.FromMB(100UL).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ByteQuantifiedSize.FromMB(10UL).ToBytes();
			this.LogPrefix = logPrefix;
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
				return "MonitoringLog";
			}
		}

		public string LogPrefix { get; private set; }

		public string LogType
		{
			get
			{
				return "Active Monitoring Log";
			}
		}

		private const string LogTypeValue = "Active Monitoring Log";

		private const string LogComponentValue = "MonitoringLog";

		private const string DefaultRelativeFilePath = "Logging\\Monitoring\\";

		public static readonly string LogPrefixValue = "Monitoring";
	}
}
