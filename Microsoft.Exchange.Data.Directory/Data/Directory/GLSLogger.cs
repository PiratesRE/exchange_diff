using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class GLSLogger
	{
		private static bool Enabled { get; set; }

		private static bool Initialized { get; set; }

		private static int GetNextSequenceNumber()
		{
			int result;
			lock (GLSLogger.incrementLock)
			{
				if (GLSLogger.sequenceNumber == 2147483647)
				{
					GLSLogger.sequenceNumber = 0;
				}
				else
				{
					GLSLogger.sequenceNumber++;
				}
				result = GLSLogger.sequenceNumber;
			}
			return result;
		}

		private static void Initialize(ExDateTime serviceStartTime, string logFilePath, TimeSpan maxRetentionPeriond, ByteQuantifiedSize directorySizeQuota, ByteQuantifiedSize perFileSizeQuota, bool applyHourPrecision)
		{
			int registryInt;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
			{
				GLSLogger.Enabled = GLSLogger.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
				registryInt = GLSLogger.GetRegistryInt(registryKey, "LogBufferSize", 65536);
			}
			if (GLSLogger.registryWatcher == null)
			{
				GLSLogger.registryWatcher = new RegistryWatcher("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters", false);
			}
			if (GLSLogger.timer == null)
			{
				GLSLogger.timer = new Timer(new TimerCallback(GLSLogger.UpdateConfigIfChanged), null, 0, 300000);
			}
			if (GLSLogger.Enabled)
			{
				GLSLogger.log = new Log(GLSLogger.logFilePrefix, new LogHeaderFormatter(GLSLogger.schema, LogHeaderCsvOption.CsvCompatible), "GLSLogs");
				GLSLogger.log.Configure(logFilePath, maxRetentionPeriond, (long)directorySizeQuota.ToBytes(), (long)perFileSizeQuota.ToBytes(), applyHourPrecision, registryInt, GLSLogger.defaultFlushInterval);
				AppDomain.CurrentDomain.ProcessExit += GLSLogger.CurrentDomain_ProcessExit;
			}
			GLSLogger.Initialized = true;
		}

		private static void UpdateConfigIfChanged(object state)
		{
			if (GLSLogger.registryWatcher.IsChanged())
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters"))
				{
					bool registryBool = GLSLogger.GetRegistryBool(registryKey, "ProtocolLoggingEnabled", true);
					if (registryBool != GLSLogger.Enabled)
					{
						lock (GLSLogger.logLock)
						{
							GLSLogger.Initialized = false;
							GLSLogger.Enabled = registryBool;
						}
					}
				}
			}
		}

		public static void LogResponse(GlsLoggerContext context, GLSLogger.StatusCode statusCode, ResponseBase response, GlsRawResponse rawResponse)
		{
			int num = Environment.TickCount - context.TickStart;
			string resultCode;
			switch (statusCode)
			{
			case GLSLogger.StatusCode.Found:
				resultCode = "Found";
				break;
			case GLSLogger.StatusCode.NotFound:
				resultCode = "NotFound";
				break;
			case GLSLogger.StatusCode.WriteSuccess:
				resultCode = "Success";
				break;
			default:
				throw new ArgumentException("statusCode");
			}
			GLSLogger.BeginAppend(context.MethodName, context.ParameterValue, context.ResolveEndpointToIpAddress(false), resultCode, rawResponse, (long)num, string.Empty, response.TransactionID, context.ConnectionId, response.Diagnostics);
			ADProviderPerf.UpdateGlsCallLatency(context.MethodName, context.IsRead, num, true);
		}

		public static void LogException(GlsLoggerContext context, Exception ex)
		{
			int num = Environment.TickCount - context.TickStart;
			StackTrace stackTrace = new StackTrace(false);
			GLSLogger.BeginAppend(context.MethodName, context.ParameterValue, context.ResolveEndpointToIpAddress(true), "Exception", null, (long)num, ex.Message + stackTrace.ToString(), context.RequestTrackingGuid.ToString(), context.ConnectionId, string.Empty);
			ADProviderPerf.UpdateGlsCallLatency(context.MethodName, context.IsRead, num, false);
		}

		public static T LoggingWrapper<T>(LocatorServiceClientAdapter glsClientAdapter, string parameter, string connectionId, Func<T> method) where T : ResponseBase
		{
			string ipAddress = glsClientAdapter.ResolveEndpointToIpAddress(false);
			string diagnostics = string.Empty;
			string transactionId = string.Empty;
			string text = string.Empty;
			int tickCount = Environment.TickCount;
			T result;
			try
			{
				GLSLogger.FaultInjectionTrace();
				result = method();
				diagnostics = result.Diagnostics;
				transactionId = result.TransactionID;
			}
			catch (Exception ex)
			{
				text = ex.Message;
				ipAddress = glsClientAdapter.ResolveEndpointToIpAddress(true);
				throw;
			}
			finally
			{
				int num = Environment.TickCount - tickCount;
				string resultCode = string.IsNullOrEmpty(text) ? "Success" : "Exception";
				GLSLogger.BeginAppend(method.Method.Name, parameter, ipAddress, resultCode, null, (long)num, text, transactionId, connectionId, diagnostics);
				bool isRead;
				string apiName = GLSLogger.ApiNameFromReturnType<T>(out isRead);
				ADProviderPerf.UpdateGlsCallLatency(apiName, isRead, num, string.IsNullOrEmpty(text));
			}
			return result;
		}

		private static string ApiNameFromReturnType<T>(out bool isRead) where T : ResponseBase
		{
			Type typeFromHandle = typeof(T);
			isRead = true;
			if (typeFromHandle == typeof(FindTenantResponse))
			{
				return "FindTenant";
			}
			if (typeFromHandle == typeof(FindDomainResponse))
			{
				return "FindDomain";
			}
			if (typeFromHandle == typeof(FindDomainsResponse))
			{
				return "FindDomains";
			}
			isRead = false;
			if (typeFromHandle == typeof(SaveTenantResponse))
			{
				return "SaveTenant";
			}
			if (typeFromHandle == typeof(SaveDomainResponse))
			{
				return "SaveDomain";
			}
			if (typeFromHandle == typeof(DeleteTenantResponse))
			{
				return "DeleteTenant";
			}
			if (typeFromHandle == typeof(DeleteDomainResponse))
			{
				return "DeleteDomain";
			}
			throw new ArgumentException("Unknown response type " + typeFromHandle.Name);
		}

		internal static void Append(string operation, string parameter, string ipAddress, string resultCode, GlsRawResponse rawResponse, long processingTime, string failure, string transactionid, string connectionid, string diagnostics)
		{
			Guid activityId = Guid.Empty;
			try
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null && currentActivityScope.Status == ActivityContextStatus.ActivityStarted)
				{
					activityId = currentActivityScope.ActivityId;
				}
			}
			catch (Exception ex)
			{
				diagnostics += ex.ToString();
			}
			GLSLogger.AppendInternal(operation, parameter, ipAddress, resultCode, rawResponse, processingTime, failure, transactionid, connectionid, diagnostics, activityId);
		}

		private static void AppendInternal(string operation, string parameter, string ipAddress, string resultCode, GlsRawResponse rawResponse, long processingTime, string failure, string transactionid, string connectionid, string diagnostics, Guid activityId)
		{
			if (rawResponse == null)
			{
				rawResponse = new GlsRawResponse();
			}
			if (!GLSLogger.Initialized)
			{
				lock (GLSLogger.logLock)
				{
					if (!GLSLogger.Initialized)
					{
						GLSLogger.Initialize(ExDateTime.UtcNow, Path.Combine(GLSLogger.GetExchangeInstallPath(), "Logging\\GLS\\"), GLSLogger.defaultMaxRetentionPeriod, GLSLogger.defaultDirectorySizeQuota, GLSLogger.defaultPerFileSizeQuota, true);
					}
				}
			}
			if (GLSLogger.Enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(GLSLogger.schema);
				logRowFormatter[1] = GLSLogger.GetNextSequenceNumber();
				logRowFormatter[2] = Globals.ProcessName;
				logRowFormatter[3] = Globals.ProcessId;
				logRowFormatter[4] = Globals.ProcessAppName;
				logRowFormatter[5] = Environment.CurrentManagedThreadId;
				logRowFormatter[6] = operation;
				logRowFormatter[7] = parameter;
				logRowFormatter[9] = processingTime;
				logRowFormatter[8] = resultCode;
				logRowFormatter[10] = ipAddress;
				logRowFormatter[11] = rawResponse.ResourceForest;
				logRowFormatter[12] = rawResponse.AccountForest;
				logRowFormatter[13] = rawResponse.TenantContainerCN;
				logRowFormatter[14] = rawResponse.TenantId;
				logRowFormatter[15] = rawResponse.SmtpNextHopDomain;
				logRowFormatter[16] = rawResponse.TenantFlags;
				logRowFormatter[17] = rawResponse.DomainName;
				logRowFormatter[18] = rawResponse.DomainInUse;
				logRowFormatter[19] = rawResponse.DomainFlags;
				logRowFormatter[20] = failure;
				logRowFormatter[21] = transactionid;
				logRowFormatter[22] = connectionid;
				logRowFormatter[23] = diagnostics;
				logRowFormatter[24] = activityId;
				GLSLogger.log.Append(logRowFormatter, 0);
			}
		}

		internal static void BeginAppend(string operation, string parameter, string ipAddress, string resultCode, GlsRawResponse rawResponse, long processingTime, string failure, string transactionId, string connectionId, string diagnostics)
		{
			GLSLogger.AppendDelegate appendDelegate = new GLSLogger.AppendDelegate(GLSLogger.AppendInternal);
			Guid activityId = Guid.Empty;
			try
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null && currentActivityScope.Status == ActivityContextStatus.ActivityStarted)
				{
					activityId = currentActivityScope.ActivityId;
				}
			}
			catch (Exception ex)
			{
				diagnostics += ex.ToString();
			}
			appendDelegate.BeginInvoke(operation, parameter, ipAddress, resultCode, rawResponse, processingTime, failure, transactionId, connectionId, diagnostics, activityId, null, null);
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
			lock (GLSLogger.logLock)
			{
				GLSLogger.Enabled = false;
				GLSLogger.Initialized = false;
				GLSLogger.Shutdown();
			}
		}

		private static void Shutdown()
		{
			if (GLSLogger.log != null)
			{
				GLSLogger.log.Close();
			}
			if (GLSLogger.timer != null)
			{
				GLSLogger.timer.Dispose();
				GLSLogger.timer = null;
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
			string[] array = new string[GLSLogger.Fields.Length];
			for (int i = 0; i < GLSLogger.Fields.Length; i++)
			{
				array[i] = GLSLogger.Fields[i].ColumnName;
			}
			return array;
		}

		internal static void FaultInjectionTrace()
		{
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2785422653U);
			}
			catch (TimeoutException)
			{
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3620089149U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2814782781U);
		}

		internal static void FaultInjectionDelayTraceForAsync()
		{
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3624283453U);
			}
			catch (TimeoutException)
			{
			}
		}

		internal static void FaultInjectionTraceForAsync()
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(4161154365U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2550541629U);
		}

		private const uint TransientGlsExceptionLid = 3620089149U;

		private const uint PermanentGlsExceptionLid = 2814782781U;

		private const uint GlsDelayLid = 2785422653U;

		private const uint AsyncTransientGlsExceptionLid = 4161154365U;

		private const uint AsyncPermanentGlsExceptionLid = 2550541629U;

		private const uint AsyncGlsDelayLid = 3624283453U;

		private const string LogTypeName = "GLS Logs";

		private const string LogComponent = "GLSLogs";

		private const int DefaultLogBufferSize = 65536;

		private const bool DefaultLoggingEnabled = true;

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess\\Parameters";

		private const string LoggingEnabledRegKeyName = "ProtocolLoggingEnabled";

		private const string LogBufferSizeRegKeyName = "LogBufferSize";

		private const string LoggingDirectoryUnderExchangeInstallPath = "Logging\\GLS\\";

		internal static readonly GLSLogger.FieldInfo[] Fields = new GLSLogger.FieldInfo[]
		{
			new GLSLogger.FieldInfo(GLSLogger.Field.DateTime, "date-time"),
			new GLSLogger.FieldInfo(GLSLogger.Field.SequenceNumber, "seq-number"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ClientName, "process-name"),
			new GLSLogger.FieldInfo(GLSLogger.Field.Pid, "process-id"),
			new GLSLogger.FieldInfo(GLSLogger.Field.AppName, "application-name"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ThreadId, "thread-id"),
			new GLSLogger.FieldInfo(GLSLogger.Field.Operation, "operation"),
			new GLSLogger.FieldInfo(GLSLogger.Field.Parameter, "parameter"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ResultCode, "result-code"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ProcessingTime, "processing-time"),
			new GLSLogger.FieldInfo(GLSLogger.Field.IPAddress, "ip-address"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ResourceForest, "resource-forest"),
			new GLSLogger.FieldInfo(GLSLogger.Field.AccountForest, "account-forest"),
			new GLSLogger.FieldInfo(GLSLogger.Field.TenantContainerCN, "tenant-container-CN"),
			new GLSLogger.FieldInfo(GLSLogger.Field.TenantId, "tenant-id"),
			new GLSLogger.FieldInfo(GLSLogger.Field.SmtpNextHopDomain, "smtp-next-hop-domain"),
			new GLSLogger.FieldInfo(GLSLogger.Field.TenantFlags, "tenant-flags"),
			new GLSLogger.FieldInfo(GLSLogger.Field.DomainName, "domain-name"),
			new GLSLogger.FieldInfo(GLSLogger.Field.DomainInUse, "domain-in-use"),
			new GLSLogger.FieldInfo(GLSLogger.Field.DomainFlags, "domain-flags"),
			new GLSLogger.FieldInfo(GLSLogger.Field.Failures, "failures"),
			new GLSLogger.FieldInfo(GLSLogger.Field.TransactionId, "transaction-id"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ConnectionId, "connection-id"),
			new GLSLogger.FieldInfo(GLSLogger.Field.Diagnostics, "diagnostics"),
			new GLSLogger.FieldInfo(GLSLogger.Field.ActivityId, "activity-id")
		};

		internal static IList<TenantProperty> TenantPropertiesToLog = new TenantProperty[]
		{
			TenantProperty.EXOResourceForest,
			TenantProperty.EXOAccountForest,
			TenantProperty.EXOSmtpNextHopDomain,
			TenantProperty.EXOTenantContainerCN
		};

		private static readonly LogSchema schema = new LogSchema("Microsoft Exchange", "15.00.1497.015", "GLS Logs", GLSLogger.GetColumnArray());

		private static TimeSpan defaultMaxRetentionPeriod = TimeSpan.FromHours(8.0);

		private static ByteQuantifiedSize defaultDirectorySizeQuota = ByteQuantifiedSize.Parse("200MB");

		private static ByteQuantifiedSize defaultPerFileSizeQuota = ByteQuantifiedSize.Parse("10MB");

		private static TimeSpan defaultFlushInterval = TimeSpan.FromMinutes(15.0);

		private static string logFilePrefix = Globals.ProcessName + "_";

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
			ThreadId,
			Operation,
			Parameter,
			ResultCode,
			ProcessingTime,
			IPAddress,
			ResourceForest,
			AccountForest,
			TenantContainerCN,
			TenantId,
			SmtpNextHopDomain,
			TenantFlags,
			DomainName,
			DomainInUse,
			DomainFlags,
			Failures,
			TransactionId,
			ConnectionId,
			Diagnostics,
			ActivityId
		}

		internal enum StatusCode
		{
			Found,
			NotFound,
			WriteSuccess
		}

		internal delegate void AppendDelegate(string operation, string parameter, string ipAddress, string resultCode, GlsRawResponse rawResponse, long processingTime, string failure, string transactionid, string connectionid, string diagnostics, Guid activityId);

		internal struct FieldInfo
		{
			public FieldInfo(GLSLogger.Field field, string columnName)
			{
				this.Field = field;
				this.ColumnName = columnName;
			}

			internal readonly GLSLogger.Field Field;

			internal readonly string ColumnName;
		}
	}
}
