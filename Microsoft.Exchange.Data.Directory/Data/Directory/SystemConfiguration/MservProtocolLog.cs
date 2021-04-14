using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class MservProtocolLog
	{
		private static bool Enabled { get; set; }

		private static bool Initialized { get; set; }

		private static int GetNextSequenceNumber()
		{
			int result;
			lock (MservProtocolLog.incrementLock)
			{
				if (MservProtocolLog.sequenceNumber == 2147483647)
				{
					MservProtocolLog.sequenceNumber = 0;
				}
				else
				{
					MservProtocolLog.sequenceNumber++;
				}
				result = MservProtocolLog.sequenceNumber;
			}
			return result;
		}

		private static void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriond, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision)
		{
			int registryInt;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange Mserv\\Parameters"))
			{
				MservProtocolLog.Enabled = MservProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				registryInt = MservProtocolLog.GetRegistryInt(registryKey, "LogBufferSize", 65536);
			}
			if (MservProtocolLog.registryWatcher == null)
			{
				MservProtocolLog.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange Mserv\\Parameters", false);
			}
			if (MservProtocolLog.timer == null)
			{
				MservProtocolLog.timer = new Timer(new TimerCallback(MservProtocolLog.UpdateConfigIfChanged), null, 0, 300000);
			}
			if (MservProtocolLog.Enabled)
			{
				MservProtocolLog.log = new Log(MservProtocolLog.logFilePrefix, new LogHeaderFormatter(MservProtocolLog.schema, LogHeaderCsvOption.CsvCompatible), "MservLogs");
				MservProtocolLog.log.Configure(logFilePath, maxRetentionPeriond, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, registryInt, MservProtocolLog.defaultFlushInterval);
				AppDomain.CurrentDomain.ProcessExit += MservProtocolLog.CurrentDomain_ProcessExit;
			}
			MservProtocolLog.Initialized = true;
		}

		private static void UpdateConfigIfChanged(object state)
		{
			if (MservProtocolLog.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange Mserv\\Parameters"))
				{
					bool registryBool = MservProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
					if (registryBool != MservProtocolLog.Enabled)
					{
						lock (MservProtocolLog.logLock)
						{
							MservProtocolLog.Initialized = false;
							MservProtocolLog.Enabled = registryBool;
						}
					}
				}
			}
		}

		internal static void Append(string operation, string resultCode, long processingTime, string failure, string lookupKey, string partnerId, string ipAddress, string diagnosticHeader, string transactionId)
		{
			lock (MservProtocolLog.logLock)
			{
				if (!MservProtocolLog.Initialized)
				{
					MservProtocolLog.Initialize(ExDateTime.UtcNow, Path.Combine(MservProtocolLog.GetExchangeInstallPath(), "Logging\\Mserv\\"), MservProtocolLog.defaultMaxRetentionPeriod, MservProtocolLog.defaultDirectorySizeQuota, MservProtocolLog.defaultPerFileSizeQuota, true);
				}
			}
			if (MservProtocolLog.Enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(MservProtocolLog.schema);
				logRowFormatter[1] = MservProtocolLog.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[4] = Globals.ProcessAppName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[5] = operation;
				logRowFormatter[7] = processingTime;
				logRowFormatter[6] = resultCode;
				logRowFormatter[8] = failure;
				logRowFormatter[9] = lookupKey;
				logRowFormatter[10] = partnerId;
				logRowFormatter[11] = ipAddress;
				logRowFormatter[12] = diagnosticHeader;
				logRowFormatter[13] = transactionId;
				MservProtocolLog.log.Append(logRowFormatter, 0);
			}
		}

		internal static void BeginAppend(string operation, string resultCode, long processingTime, string failure, string lookupKey, string partnerId, string ipAddress, string diagnosticHeader, string transactionId)
		{
			MservProtocolLog.AppendDelegate appendDelegate = new MservProtocolLog.AppendDelegate(MservProtocolLog.Append);
			appendDelegate.BeginInvoke(operation, resultCode, processingTime, failure, lookupKey, partnerId, ipAddress, diagnosticHeader, transactionId, null, null);
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
			lock (MservProtocolLog.logLock)
			{
				MservProtocolLog.Enabled = false;
				MservProtocolLog.Initialized = false;
				MservProtocolLog.Shutdown();
			}
		}

		private static void Shutdown()
		{
			if (MservProtocolLog.log != null)
			{
				MservProtocolLog.log.Close();
			}
			if (MservProtocolLog.timer != null)
			{
				MservProtocolLog.timer.Dispose();
				MservProtocolLog.timer = null;
			}
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
			string[] array = new string[MservProtocolLog.Fields.Length];
			for (int i = 0; i < MservProtocolLog.Fields.Length; i++)
			{
				array[i] = MservProtocolLog.Fields[i].ColumnName;
			}
			return array;
		}

		private const string LogTypeName = "Mserv Logs";

		private const string LogComponent = "MservLogs";

		private const int DefaultLogBufferSize = 65536;

		private const bool DefaultLoggingEnabled = true;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange Mserv\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\Mserv\\";

		internal static readonly MservProtocolLog.FieldInfo[] Fields = new MservProtocolLog.FieldInfo[]
		{
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.DateTime, "date-time"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.SequenceNumber, "seq-number"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.ClientName, "process-name"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.Pid, "process-id"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.AppName, "application-name"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.Operation, "operation"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.ResultCode, "result-code"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.ProcessingTime, "processing-time"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.Failures, "failures"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.LookupKey, "lookup-key"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.PartnerId, "partner-id"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.IpAddress, "ip-address"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.DiagnosticHeader, "diagnostic-header"),
			new MservProtocolLog.FieldInfo(MservProtocolLog.Field.TransactionId, "transaction-id")
		};

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "Mserv Logs", MservProtocolLog.GetColumnArray());

		private static TimeSpan defaultMaxRetentionPeriod = TimeSpan.FromHours(24.0);

		private static ByteQuantifiedSize defaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		private static ByteQuantifiedSize defaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan defaultFlushInterval = TimeSpan.FromMinutes(15.0);

		private static string logFilePrefix = Globals.ProcessName + "_" + Globals.ProcessAppName + "_";

		private static int sequenceNumber = 0;

		private static Timer timer;

		private static Log log;

		private static object logLock = new object();

		private static object incrementLock = new object();

		private static RegistryWatcher registryWatcher;

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
			Failures,
			LookupKey,
			PartnerId,
			IpAddress,
			DiagnosticHeader,
			TransactionId
		}

		internal delegate void AppendDelegate(string operation, string resultCode, long processingTime, string failure, string lookupKey, string partnerId, string ipAddress, string diagnosticHeader, string transactionId);

		internal struct FieldInfo
		{
			public FieldInfo(MservProtocolLog.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly MservProtocolLog.Field Field;

			internal readonly string ColumnName;
		}
	}
}
