using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring
{
	internal static class StxLogger
	{
		private static bool Enabled { get; set; }

		private static void Initialize(StxLoggerBase stxLogger)
		{
			stxLogger.Initialized = true;
			int registryInt;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Monitoring\\Parameters"))
			{
				StxLogger.Enabled = StxLogger.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				registryInt = StxLogger.GetRegistryInt(registryKey, "LogBufferSize", 256);
			}
			if (StxLogger.registryWatcher == null)
			{
				StxLogger.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\Monitoring\\Parameters", false);
			}
			if (StxLogger.timer == null)
			{
				StxLogger.timer = new Timer(new TimerCallback(StxLogger.UpdateConfigIfChanged), null, 0, 300000);
			}
			if (StxLogger.Enabled)
			{
				stxLogger.Log.Configure(Path.Combine(StxLogger.GetExchangeInstallPath(), "Logging\\MonitoringLogs\\STx"), StxLogger.defaultMaxRetentionPeriod, (long)StxLogger.defaultDirectorySizeQuota.ToBytes(), (long)StxLogger.defaultPerFileSizeQuota.ToBytes(), true, registryInt, StxLogger.defaultFlushInterval);
				AppDomain.CurrentDomain.ProcessExit += StxLogger.CurrentDomain_ProcessExit;
			}
		}

		private static void UpdateConfigIfChanged(object state)
		{
			if (StxLogger.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Monitoring\\Parameters"))
				{
					StxLogger.Enabled = StxLogger.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				}
			}
		}

		internal static void Append(StxLoggerBase stxLogger, LogRowFormatter row)
		{
			lock (StxLogger.logLock)
			{
				if (!stxLogger.Initialized)
				{
					StxLogger.Initialize(stxLogger);
				}
			}
			if (StxLogger.Enabled)
			{
				stxLogger.Log.Append(row, stxLogger.DateTimeField);
			}
		}

		internal static void BeginAppend(StxLoggerBase stxLogger, LogRowFormatter row)
		{
			StxLogger.AppendDelegate appendDelegate = new StxLogger.AppendDelegate(StxLogger.Append);
			appendDelegate.BeginInvoke(stxLogger, row, null, null);
		}

		private static bool GetRegistryBool(RegistryKey regkey, string key, bool defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return Convert.ToBoolean(num.Value);
		}

		private static int GetRegistryInt(RegistryKey regkey, string key, int defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return num.Value;
		}

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			StxLogger.Shutdown();
		}

		private static void Shutdown()
		{
			foreach (KeyValuePair<StxLogType, StxLoggerBase> keyValuePair in StxLoggerBase.LogDictionary)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.Log.Close();
					keyValuePair.Value.Initialized = false;
				}
			}
			if (StxLogger.timer != null)
			{
				StxLogger.timer.Dispose();
				StxLogger.timer = null;
			}
		}

		private static string GetExchangeInstallPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				string text = Path.Combine("D:", "Exchange");
				if (registryKey == null)
				{
					result = text;
				}
				else
				{
					object value = registryKey.GetValue("MsiInstallPath");
					registryKey.Close();
					if (value == null)
					{
						result = text;
					}
					else
					{
						result = value.ToString();
					}
				}
			}
			return result;
		}

		private const int DefaultLogBufferSize = 256;

		private const bool DefaultLoggingEnabled = true;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\Monitoring\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\MonitoringLogs\\STx";

		private static TimeSpan defaultMaxRetentionPeriod = TimeSpan.FromHours(720.0);

		private static ByteQuantifiedSize defaultDirectorySizeQuota = ByteQuantifiedSize.Parse("5GB");

		private static ByteQuantifiedSize defaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan defaultFlushInterval = TimeSpan.FromMinutes(15.0);

		private static Timer timer;

		private static object logLock = new object();

		private static RegistryWatcher registryWatcher;

		internal delegate void AppendDelegate(StxLoggerBase stxLogger, LogRowFormatter row);
	}
}
