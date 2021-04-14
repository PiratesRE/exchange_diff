using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class WTFLogConfiguration : ILogConfiguration
	{
		public WTFLogConfiguration()
		{
			this.IsLoggingEnabled = Settings.IsTraceLoggingEnabled;
			if (string.IsNullOrEmpty(Settings.DefaultTraceLogPath))
			{
				this.IsLoggingEnabled = false;
			}
			if (this.IsLoggingEnabled)
			{
				this.LogPath = WTFLogConfiguration.GetLogDirectoryPath();
				this.MaxLogAge = TimeSpan.FromDays((double)Settings.MaxLogAge);
				this.MaxLogDirectorySizeInBytes = Settings.MaxTraceLogsDirectorySizeInBytes;
				this.MaxLogFileSizeInBytes = Settings.MaxTraceLogFileSizeInBytes;
				this.LogBufferSizeInBytes = Settings.TraceLogBufferSizeInBytes;
				this.FlushIntervalInMinutes = new TimeSpan(0, Settings.TraceLogFlushIntervalInMinutes, 0);
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
				return "ActiveMonitoringTraceLogs";
			}
		}

		public string LogPrefix
		{
			get
			{
				return "ActiveMonitoringTraceLogs";
			}
		}

		public string LogType
		{
			get
			{
				return "ActiveMonitoringTraceLogs";
			}
		}

		public int LogBufferSizeInBytes { get; private set; }

		public TimeSpan FlushIntervalInMinutes { get; private set; }

		private static string GetLogDirectoryPath()
		{
			string processName = Process.GetCurrentProcess().ProcessName;
			return Path.Combine(Path.Combine(Settings.DefaultTraceLogPath, "Monitoring\\Monitoring"), processName, "ActiveMonitoringTraceLogs");
		}

		public const string SoftwareName = "Microsoft Exchange Server Active Monitoring";

		private const string LogName = "ActiveMonitoringTraceLogs";

		private const string LogNamePrefix = "ActiveMonitoringTraceLogs";

		private const string LogNameType = "ActiveMonitoringTraceLogs";

		private const string LogComponentValue = "ActiveMonitoringTraceLogs";

		public readonly string SoftwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}
}
