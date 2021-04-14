using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal class DirectoryThrottlingLog : DisposeTrackableBase
	{
		internal static int CountOfDCsToLog
		{
			get
			{
				return DirectoryThrottlingLog.countOfDCsToLog;
			}
		}

		internal static bool LoggingEnabled
		{
			get
			{
				return DirectoryThrottlingLog.loggingEnabled.Value;
			}
		}

		private DirectoryThrottlingLog()
		{
			this.logSchema = new LogSchema("Microsoft Exchange AD Throttling", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Directory Throttling Log", Enum.GetNames(typeof(DirectoryThrottlingLog.DirectoryThrottlingLogFields)));
			this.log = new Log(DirectoryThrottlingLog.FileNamePrefixName, new LogHeaderFormatter(this.logSchema, true), "DirectoryThrottling");
			DirectoryThrottlingLog.ReadConfigData();
			DirectoryThrottlingLog.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", false);
			DirectoryThrottlingLog.timer = new Timer(new TimerCallback(DirectoryThrottlingLog.UpdateConfigIfChanged), null, 0, 300000);
			this.Configure();
		}

		public static DirectoryThrottlingLog Instance
		{
			get
			{
				return DirectoryThrottlingLog.instance;
			}
		}

		public void Configure()
		{
			if (!base.IsDisposed)
			{
				lock (this.logLock)
				{
					this.log.Configure(Path.Combine(DirectoryLogUtils.GetExchangeInstallPath(), "Logging\\DirectoryThrottling\\"), TimeSpan.FromDays(30.0), 104857600L, 104857600L, 10485760, TimeSpan.FromSeconds(10.0), true);
				}
			}
		}

		public void Close()
		{
			if (!base.IsDisposed && this.log != null)
			{
				this.log.Close();
				this.log = null;
			}
		}

		public void Log(string targetForest, ResourceLoadState resourceLoadState, int metricValue, Dictionary<string, ADServerMetrics> topDCsToLog)
		{
			if (DirectoryThrottlingLog.LoggingEnabled)
			{
				int count = topDCsToLog.Count;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, ADServerMetrics> keyValuePair in topDCsToLog)
				{
					stringBuilder.AppendFormat("{0},{1},", keyValuePair.Key, keyValuePair.Value.IncomingDebt);
				}
				string topDomainControllersIncomingDebt = stringBuilder.ToString().Trim(new char[]
				{
					','
				});
				this.LogRow(Globals.ProcessName, Globals.ProcessId, Thread.CurrentThread.ManagedThreadId, targetForest, resourceLoadState, metricValue, topDomainControllersIncomingDebt);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DirectoryThrottlingLog>(this);
		}

		private void LogRow(string processName, int processId, int threadId, string targetForest, ResourceLoadState resourceLoadState, int metricValue, string topDomainControllersIncomingDebt)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[1] = processName;
			logRowFormatter[2] = processId;
			logRowFormatter[3] = threadId;
			logRowFormatter[4] = targetForest;
			logRowFormatter[5] = resourceLoadState;
			logRowFormatter[6] = metricValue;
			logRowFormatter[7] = topDomainControllersIncomingDebt;
			this.AppendLogRow(logRowFormatter);
		}

		private void AppendLogRow(LogRowFormatter row)
		{
			if (!base.IsDisposed)
			{
				this.log.Append(row, 0);
			}
		}

		private static void ReadConfigData()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				DirectoryThrottlingLog.loggingEnabled = new bool?(DirectoryLogUtils.GetRegistryBool(registryKey, "DirectoryThrottlingLogEnabled", true));
				DirectoryThrottlingLog.countOfDCsToLog = DirectoryLogUtils.GetRegistryInt(registryKey, "DirectoryThrottlingNumberOfDCsToLog", 5);
			}
		}

		private static void UpdateConfigIfChanged(object state)
		{
			if (DirectoryThrottlingLog.registryWatcher.IsChanged())
			{
				DirectoryThrottlingLog.ReadConfigData();
			}
		}

		private const string LogTypeName = "Directory Throttling Log";

		private const string LogComponentName = "DirectoryThrottling";

		private const string SoftwareName = "Microsoft Exchange AD Throttling";

		protected const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string LoggingEnabledRegKeyName = "DirectoryThrottlingLogEnabled";

		private const string NumberOfDCsToLogRegKeyName = "DirectoryThrottlingNumberOfDCsToLog";

		private const bool DefaultLogEnabled = true;

		private const int DefaultNumberOfDCsToLog = 5;

		private static readonly string FileNamePrefixName = Globals.ProcessName + "_";

		private static readonly DirectoryThrottlingLog instance = new DirectoryThrottlingLog();

		private static RegistryWatcher registryWatcher;

		private Log log;

		private LogSchema logSchema;

		private static int countOfDCsToLog;

		private static bool? loggingEnabled;

		private static Timer timer;

		private object logLock = new object();

		internal enum DirectoryThrottlingLogFields
		{
			Timestamp,
			ProcessName,
			ProcessId,
			ThreadId,
			TargetForest,
			ResourceLoadState,
			MetricValue,
			TopDomainControllersIncomingDebt
		}
	}
}
