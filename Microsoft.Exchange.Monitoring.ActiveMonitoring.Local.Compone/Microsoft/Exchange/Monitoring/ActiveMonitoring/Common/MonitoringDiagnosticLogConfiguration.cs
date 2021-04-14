using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MonitoringDiagnosticLogConfiguration : ILogConfiguration
	{
		public MonitoringDiagnosticLogConfiguration(string logRelativePath)
		{
			this.IsLoggingEnabled = true;
			this.LogPath = Path.Combine("D:\\MonitoringDiagnosticLogs", logRelativePath);
			this.MaxLogAge = TimeSpan.FromDays(7.0);
			this.MaxLogDirectorySizeInBytes = (long)ByteQuantifiedSize.FromMB(100UL).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ByteQuantifiedSize.FromMB(10UL).ToBytes();
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
				return "MSExchangeHMHost";
			}
		}

		public string LogPrefix
		{
			get
			{
				return MonitoringDiagnosticLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "MSExchangeHMHost diagnostic log";
			}
		}

		private const string LogTypeValue = "MSExchangeHMHost diagnostic log";

		private const string LogComponentValue = "MSExchangeHMHost";

		private const string RootLogFilePath = "D:\\MonitoringDiagnosticLogs";

		public static readonly string LogPrefixValue = "MSExchangeHMHost";
	}
}
