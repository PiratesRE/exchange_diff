using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class OutboundProtocolLog
	{
		private static bool Enabled { get; set; }

		private static bool Initialized { get; set; }

		private static int GetNextSequenceNumber()
		{
			int result;
			lock (OutboundProtocolLog.incrementLock)
			{
				if (OutboundProtocolLog.sequenceNumber == 2147483647)
				{
					OutboundProtocolLog.sequenceNumber = 0;
				}
				else
				{
					OutboundProtocolLog.sequenceNumber++;
				}
				result = OutboundProtocolLog.sequenceNumber;
			}
			return result;
		}

		private static void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriond, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision)
		{
			int registryInt;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OAuth\\Parameters"))
			{
				OutboundProtocolLog.Enabled = OutboundProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				registryInt = OutboundProtocolLog.GetRegistryInt(registryKey, "LogBufferSize", 1048576);
			}
			if (OutboundProtocolLog.registryWatcher == null)
			{
				OutboundProtocolLog.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange OAuth\\Parameters", false);
			}
			if (OutboundProtocolLog.timer == null)
			{
				OutboundProtocolLog.timer = new Timer(new TimerCallback(OutboundProtocolLog.UpdateConfigIfChanged), null, 0, 300000);
			}
			if (OutboundProtocolLog.Enabled)
			{
				OutboundProtocolLog.log = new Log(OutboundProtocolLog.logFilePrefix, new LogHeaderFormatter(OutboundProtocolLog.schema, LogHeaderCsvOption.CsvCompatible), "OAuthOutbound");
				OutboundProtocolLog.log.Configure(logFilePath, maxRetentionPeriond, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, registryInt, OutboundProtocolLog.defaultFlushInterval, LogFileRollOver.Hourly);
				AppDomain.CurrentDomain.ProcessExit += OutboundProtocolLog.CurrentDomain_ProcessExit;
			}
			OutboundProtocolLog.Initialized = true;
		}

		private static void UpdateConfigIfChanged(object state)
		{
			if (OutboundProtocolLog.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OAuth\\Parameters"))
				{
					bool registryBool = OutboundProtocolLog.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
					if (registryBool != OutboundProtocolLog.Enabled)
					{
						lock (OutboundProtocolLog.logLock)
						{
							OutboundProtocolLog.Initialized = false;
							OutboundProtocolLog.Enabled = registryBool;
						}
					}
				}
			}
		}

		internal static void Append(string operation, string resultCode, long processingTime, string userAgent, Guid? clientRequestId, string targetUri, string tenantInfo, string resource, string errorString, string errorDetail, string devMetrics, TimeSpan remainingLifetime, TokenResult tokenResult)
		{
			lock (OutboundProtocolLog.logLock)
			{
				if (!OutboundProtocolLog.Initialized)
				{
					OutboundProtocolLog.Initialize(ExDateTime.UtcNow, Path.Combine(OutboundProtocolLog.GetExchangeInstallPath(), "Logging\\OAuthOutbound\\"), OutboundProtocolLog.defaultMaxRetentionPeriod, OutboundProtocolLog.defaultDirectorySizeQuota, OutboundProtocolLog.defaultPerFileSizeQuota, true);
				}
			}
			if (OutboundProtocolLog.Enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(OutboundProtocolLog.schema);
				logRowFormatter[1] = OutboundProtocolLog.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessId;
				logRowFormatter[3] = "15.00.1497.012";
				logRowFormatter[4] = operation;
				logRowFormatter[5] = resultCode;
				logRowFormatter[6] = processingTime;
				logRowFormatter[7] = userAgent;
				logRowFormatter[8] = ((clientRequestId != null) ? clientRequestId.Value.ToString() : null);
				logRowFormatter[9] = targetUri;
				logRowFormatter[10] = tenantInfo;
				logRowFormatter[11] = resource;
				logRowFormatter[12] = Globals.ProcessAppName + "_" + OAuthCommon.CurrentAppPoolName;
				logRowFormatter[13] = errorString;
				logRowFormatter[14] = ((errorDetail == null) ? null : errorDetail.Replace(",", " ").Replace("\r\n", "  "));
				logRowFormatter[15] = devMetrics;
				logRowFormatter[16] = (int)remainingLifetime.TotalSeconds;
				logRowFormatter[17] = ((tokenResult == null) ? string.Empty : tokenResult.Base64String);
				OutboundProtocolLog.log.Append(logRowFormatter, 0);
			}
		}

		internal static void BeginAppend(string operation, string resultCode, long processingTime, string userAgent, Guid? clientRequestId, string targetUri, string tenantInfo, string resource, string errorString, string errorDetail, string devMetrics, TimeSpan remainingLifetime, TokenResult tokenResult)
		{
			OutboundProtocolLog.AppendDelegate appendDelegate = new OutboundProtocolLog.AppendDelegate(OutboundProtocolLog.Append);
			appendDelegate.BeginInvoke(operation, resultCode, processingTime, userAgent, clientRequestId, targetUri, tenantInfo, resource, errorString, errorDetail, devMetrics, remainingLifetime, tokenResult, null, null);
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
			lock (OutboundProtocolLog.logLock)
			{
				OutboundProtocolLog.Enabled = false;
				OutboundProtocolLog.Initialized = false;
				OutboundProtocolLog.Shutdown();
			}
		}

		private static void Shutdown()
		{
			if (OutboundProtocolLog.log != null)
			{
				OutboundProtocolLog.log.Close();
			}
			if (OutboundProtocolLog.timer != null)
			{
				OutboundProtocolLog.timer.Dispose();
				OutboundProtocolLog.timer = null;
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
			string[] array = new string[OutboundProtocolLog.Fields.Length];
			for (int i = 0; i < OutboundProtocolLog.Fields.Length; i++)
			{
				array[i] = OutboundProtocolLog.Fields[i].ColumnName;
			}
			return array;
		}

		private const string LogTypeName = "OAuth Outbound Logs";

		private const string LogComponent = "OAuthOutbound";

		private const int DefaultLogBufferSize = 1048576;

		private const bool DefaultLoggingEnabled = true;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OAuth\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\OAuthOutbound\\";

		internal static readonly OutboundProtocolLog.FieldInfo[] Fields = new OutboundProtocolLog.FieldInfo[]
		{
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.DateTime, "date-time"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.SequenceNumber, "seq-number"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.Pid, "process-id"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.BuildNumber, "build-number"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.Operation, "operation"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ResultCode, "result-code"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ProcessingTime, "processing-time"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.UserAgent, "user-agent"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ClientRequestId, "client-request-id"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.TargetUri, "target"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.TenantInfo, "tenant-info"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.Resource, "resource"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ProcessName, "process-name"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ErrorString, "error-string"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.ErrorDetail, "error-detail"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.DevMetrics, "dev-metrics"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.RemainingLifetime, "remaining-lifetime"),
			new OutboundProtocolLog.FieldInfo(OutboundProtocolLog.Field.TokenResult, "token-result")
		};

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", "OAuth Outbound Logs", OutboundProtocolLog.GetColumnArray());

		private static TimeSpan defaultMaxRetentionPeriod = TimeSpan.FromHours(48.0);

		private static ByteQuantifiedSize defaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		private static ByteQuantifiedSize defaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan defaultFlushInterval = TimeSpan.FromMinutes(5.0);

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
			Pid,
			BuildNumber,
			Operation,
			ResultCode,
			ProcessingTime,
			UserAgent,
			ClientRequestId,
			TargetUri,
			TenantInfo,
			Resource,
			ProcessName,
			ErrorString,
			ErrorDetail,
			DevMetrics,
			RemainingLifetime,
			TokenResult
		}

		internal delegate void AppendDelegate(string operation, string resultCode, long processingTime, string userAgent, Guid? clientRequestId, string targetUri, string tenantInfo, string resource, string errorString, string errorDetail, string devMetrics, TimeSpan remainingLifetime, TokenResult tokenResult);

		internal struct FieldInfo
		{
			public FieldInfo(OutboundProtocolLog.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly OutboundProtocolLog.Field Field;

			internal readonly string ColumnName;
		}
	}
}
