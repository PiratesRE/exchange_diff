using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADProtocolLog
	{
		private ADProtocolLog()
		{
			this.rows = new List<LogRowFormatter>(32);
		}

		internal void Append(string operation, string dn, string filter, string scope, string dc, string port, string resultCode, long processingTime, string failure, int serverProcessingTime, int entriesVisted, int entriesReturned, Guid activityId, string userEmail, string newValue, string callerInfo)
		{
			if (!this.initialized)
			{
				lock (this.logLock)
				{
					if (!this.initialized)
					{
						this.Initialize(ExDateTime.UtcNow, Path.Combine(ADProtocolLog.GetExchangeInstallPath(), "Logging\\ADDriver\\"), ADProtocolLog.DefaultMaxRetentionPeriod, ADProtocolLog.DefaultDirectorySizeQuota, ADProtocolLog.DefaultPerFileSizeQuota, true);
					}
				}
			}
			if (this.enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(ADProtocolLog.Schema);
				logRowFormatter[1] = this.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[4] = Globals.ProcessAppName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[5] = operation;
				logRowFormatter[7] = processingTime;
				logRowFormatter[8] = serverProcessingTime;
				logRowFormatter[10] = entriesReturned;
				logRowFormatter[9] = entriesVisted;
				logRowFormatter[11] = dn;
				logRowFormatter[12] = filter;
				logRowFormatter[13] = scope;
				logRowFormatter[15] = dc;
				logRowFormatter[16] = port;
				logRowFormatter[6] = resultCode;
				logRowFormatter[14] = failure;
				logRowFormatter[17] = activityId;
				logRowFormatter[20] = callerInfo;
				logRowFormatter[19] = newValue;
				logRowFormatter[18] = userEmail;
				lock (this.logLock)
				{
					this.rows.Add(logRowFormatter);
					if (this.flush == null)
					{
						this.flush = new ADProtocolLog.FlushDelegate(this.FlushRows);
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
			string[] array = new string[ADProtocolLog.Fields.Length];
			for (int i = 0; i < ADProtocolLog.Fields.Length; i++)
			{
				array[i] = ADProtocolLog.Fields[i].ColumnName;
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
				this.enabled = ADProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				registryInt = ADProtocolLog.GetRegistryInt(registryKey, "LogBufferSize", 524288);
				int registryInt2 = ADProtocolLog.GetRegistryInt(registryKey, "FlushIntervalInMinutes", 15);
				if (registryInt2 > 0)
				{
					ADProtocolLog.FlushInterval = TimeSpan.FromMinutes((double)registryInt2);
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
			if (this.enabled)
			{
				this.log = new Log(ADProtocolLog.LogFilePrefix, new LogHeaderFormatter(ADProtocolLog.Schema, LogHeaderCsvOption.CsvCompatible), "ADDriverLogs");
				this.log.Configure(logFilePath, maxRetentionPeriod, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, registryInt, ADProtocolLog.FlushInterval, LogFileRollOver.Hourly);
				AppDomain.CurrentDomain.ProcessExit += this.CurrentDomain_ProcessExit;
			}
			this.initialized = true;
		}

		private void UpdateConfigIfChanged(object state)
		{
			if (this.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
				{
					bool registryBool = ADProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
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
				this.enabled = false;
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

		private const string LogTypeName = "ADDriver Logs";

		private const string LogComponent = "ADDriverLogs";

		private const int DefaultLogBufferSize = 524288;

		private const bool DefaultLoggingEnabled = true;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LogFlushIntervalRegKeyName = "FlushIntervalInMinutes";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\ADDriver\\";

		private const int DefaultRowCount = 32;

		public static readonly ADProtocolLog Instance = new ADProtocolLog();

		internal static readonly ADProtocolLog.FieldInfo[] Fields = new ADProtocolLog.FieldInfo[]
		{
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.DateTime, "date-time"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.SequenceNumber, "seq-number"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.ClientName, "process-name"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Pid, "process-id"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.AppName, "application-name"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Operation, "operation"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.ResultCode, "result-code"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.ProcessingTime, "processing-time"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.ServerProcessingTime, "serverprocessing-time"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.EntriesVisited, "entries-visited"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.EntriesReturned, "entries-returned"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.DN, "distinguished-name"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Filter, "filter"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Scope, "scope"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Failures, "failures"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.DC, "domaincontroller"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.Port, "port"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.ActivityId, "activity-id"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.UserEmail, "user-email"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.NewValue, "new-value"),
			new ADProtocolLog.FieldInfo(ADProtocolLog.Field.CallerInfo, "caller-info")
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange", "15.00.1497.015", "ADDriver Logs", ADProtocolLog.GetColumnArray());

		private static readonly TimeSpan RowTimeLimit = TimeSpan.FromSeconds(5.0);

		private static readonly TimeSpan DefaultMaxRetentionPeriod = TimeSpan.FromHours(8.0);

		private static readonly ByteQuantifiedSize DefaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		private static readonly ByteQuantifiedSize DefaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan FlushInterval = TimeSpan.FromMinutes(15.0);

		private static readonly string LogFilePrefix = Globals.ProcessName + "_" + Globals.ProcessAppName + "_";

		private int sequenceNumber;

		private Timer timer;

		private Log log;

		private object logLock = new object();

		private bool enabled;

		private bool initialized;

		private RegistryWatcher registryWatcher;

		private List<LogRowFormatter> rows;

		private ADProtocolLog.FlushDelegate flush;

		internal delegate void FlushDelegate();

		internal enum Field
		{
			DateTime,
			SequenceNumber,
			ClientName,
			Pid,
			AppName,
			Operation,
			ResultCode,
			ProcessingTime,
			ServerProcessingTime,
			EntriesVisited,
			EntriesReturned,
			DN,
			Filter,
			Scope,
			Failures,
			DC,
			Port,
			ActivityId,
			UserEmail,
			NewValue,
			CallerInfo
		}

		internal struct FieldInfo
		{
			public FieldInfo(ADProtocolLog.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly ADProtocolLog.Field Field;

			internal readonly string ColumnName;
		}
	}
}
