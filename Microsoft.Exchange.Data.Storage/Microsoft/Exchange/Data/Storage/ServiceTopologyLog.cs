using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServiceTopologyLog
	{
		private ServiceTopologyLog()
		{
			this.rows = new List<LogRowFormatter>(32);
		}

		internal void Append(string callerFilePath, string memberName, int callerFileLine)
		{
			if (!this.initialized)
			{
				lock (this.logLock)
				{
					if (!this.initialized)
					{
						this.Initialize(ExDateTime.UtcNow, Path.Combine(ServiceTopologyLog.GetExchangeInstallPath(), "Logging\\ServiceTopology\\"), ServiceTopologyLog.DefaultMaxRetentionPeriod, ServiceTopologyLog.DefaultDirectorySizeQuota, ServiceTopologyLog.DefaultPerFileSizeQuota, true);
					}
				}
			}
			if (this.enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(ServiceTopologyLog.Schema);
				logRowFormatter[1] = this.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[4] = string.Format("{0}: Method {1}: Line {2}", callerFilePath, memberName, callerFileLine);
				lock (this.logLock)
				{
					this.rows.Add(logRowFormatter);
					if (this.flush == null)
					{
						this.flush = new ServiceTopologyLog.FlushDelegate(this.FlushRows);
						this.flush.BeginInvoke(null, null);
					}
				}
			}
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

		private static string GetExchangeInstallPath()
		{
			string result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey == null)
				{
					result = string.Empty;
				}
				else
				{
					object value = registryKey.GetValue("MsiInstallPath");
					registryKey.Close();
					if (value == null)
					{
						result = string.Empty;
					}
					else
					{
						result = value.ToString();
					}
				}
			}
			return result;
		}

		private static string[] GetColumnArray()
		{
			string[] array = new string[ServiceTopologyLog.Fields.Length];
			for (int i = 0; i < ServiceTopologyLog.Fields.Length; i++)
			{
				array[i] = ServiceTopologyLog.Fields[i].ColumnName;
			}
			return array;
		}

		private int GetNextSequenceNumber()
		{
			return Interlocked.Increment(ref this.sequenceNumber) & int.MaxValue;
		}

		private void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriod, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision)
		{
			int registryInt;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				this.enabled = ServiceTopologyLog.GetRegistryBool(registryKey, "ServiceTopologyLoggingEnabled", false);
				registryInt = ServiceTopologyLog.GetRegistryInt(registryKey, "LogBufferSize", 524288);
				int registryInt2 = ServiceTopologyLog.GetRegistryInt(registryKey, "FlushIntervalInMinutes", 15);
				if (registryInt2 > 0)
				{
					ServiceTopologyLog.FlushInterval = TimeSpan.FromMinutes((double)registryInt2);
				}
			}
			if (this.registryWatcher == null)
			{
				this.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", false);
			}
			if (this.timer == null)
			{
				this.timer = new Timer(new TimerCallback(this.UpdateConfigIfChanged), null, 0, 300000);
			}
			this.log = new Log(ServiceTopologyLog.LogFilePrefix, new LogHeaderFormatter(ServiceTopologyLog.Schema, LogHeaderCsvOption.CsvCompatible), "ServiceTopologyLogs");
			this.log.Configure(logFilePath, maxRetentionPeriod, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, registryInt, ServiceTopologyLog.FlushInterval, LogFileRollOver.Hourly);
			AppDomain.CurrentDomain.ProcessExit += this.CurrentDomain_ProcessExit;
			this.initialized = true;
		}

		private void UpdateConfigIfChanged(object state)
		{
			if (this.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
				{
					bool registryBool = ServiceTopologyLog.GetRegistryBool(registryKey, "ServiceTopologyLoggingEnabled", false);
					if (registryBool != this.enabled)
					{
						lock (this.logLock)
						{
							this.initialized = false;
							this.enabled = registryBool;
						}
					}
				}
			}
		}

		private void FlushRows()
		{
			List<LogRowFormatter> list;
			lock (this.logLock)
			{
				list = this.rows;
				this.rows = new List<LogRowFormatter>(32);
			}
			this.log.Append(list, 0);
			this.flush = null;
		}

		private void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			lock (this.logLock)
			{
				this.initialized = false;
				this.rows.Clear();
				this.Shutdown();
			}
		}

		private void Shutdown()
		{
			if (this.log != null)
			{
				this.log.Close();
			}
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		private const string LogTypeName = "ServiceTopology Logs";

		private const string LogComponent = "ServiceTopologyLogs";

		private const int DefaultLogBufferSize = 524288;

		private const bool DefaultLoggingEnabled = false;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string LoggingEnabledRegKeyName = "ServiceTopologyLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LogFlushIntervalRegKeyName = "FlushIntervalInMinutes";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\ServiceTopology\\";

		private const int DefaultRowCount = 32;

		public static readonly ServiceTopologyLog Instance = new ServiceTopologyLog();

		internal static readonly ServiceTopologyLog.FieldInfo[] Fields = new ServiceTopologyLog.FieldInfo[]
		{
			new ServiceTopologyLog.FieldInfo(ServiceTopologyLog.Field.DateTime, "date-time"),
			new ServiceTopologyLog.FieldInfo(ServiceTopologyLog.Field.SequenceNumber, "seq-number"),
			new ServiceTopologyLog.FieldInfo(ServiceTopologyLog.Field.ClientName, "process-name"),
			new ServiceTopologyLog.FieldInfo(ServiceTopologyLog.Field.Pid, "process-id"),
			new ServiceTopologyLog.FieldInfo(ServiceTopologyLog.Field.CallerInfo, "caller-info")
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "ServiceTopology Logs", ServiceTopologyLog.GetColumnArray());

		private static readonly TimeSpan DefaultMaxRetentionPeriod = TimeSpan.FromHours(8.0);

		private static readonly ByteQuantifiedSize DefaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		private static readonly ByteQuantifiedSize DefaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan FlushInterval = TimeSpan.FromMinutes(15.0);

		private static readonly string LogFilePrefix = Globals.ProcessName + "_" + Globals.ProcessAppName + "_";

		private int sequenceNumber;

		private Timer timer;

		private Log log;

		private readonly object logLock = new object();

		private bool enabled;

		private bool initialized;

		private RegistryWatcher registryWatcher;

		private List<LogRowFormatter> rows;

		private ServiceTopologyLog.FlushDelegate flush;

		internal delegate void FlushDelegate();

		internal enum Field
		{
			DateTime,
			SequenceNumber,
			ClientName,
			Pid,
			CallerInfo
		}

		internal struct FieldInfo
		{
			public FieldInfo(ServiceTopologyLog.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly ServiceTopologyLog.Field Field;

			internal readonly string ColumnName;
		}
	}
}
