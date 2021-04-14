using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExExecutionLog : ExecutionLog
	{
		private ExExecutionLog(string path)
		{
			string[] array = new string[ExExecutionLog.CommonFields.Length + CustomDataLogger.CustomFields.Length];
			Array.Copy(ExExecutionLog.CommonFields, array, ExExecutionLog.CommonFields.Length);
			Array.Copy(CustomDataLogger.CustomFields, 0, array, ExExecutionLog.CommonFields.Length, CustomDataLogger.CustomFields.Length);
			this.logSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Unified Policy Log", array);
			this.logInstance = new Log(ExExecutionLog.GetLogFileName(), new LogHeaderFormatter(this.logSchema), "UnifiedPolicyLog");
			this.logInstance.Configure(Path.Combine(ExchangeSetupContext.InstallPath, path), ExExecutionLog.LogMaxAge, 262144000L, 10485760L);
		}

		public static ExExecutionLog CreateForServicelet()
		{
			return new ExExecutionLog("Logging\\UnifiedPolicy\\SyncAgent\\");
		}

		public static ExExecutionLog CreateForCmdlet()
		{
			return new ExExecutionLog("Logging\\UnifiedPolicy\\Cmdlet\\");
		}

		public override void LogInformation(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, ExecutionLog.EventType.Information, string.Empty, contextData, null, customData);
		}

		public override void LogVerbose(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, ExecutionLog.EventType.Verbose, string.Empty, contextData, null, customData);
		}

		public override void LogWarnining(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, ExecutionLog.EventType.Warning, string.Empty, contextData, null, customData);
		}

		public override void LogError(string client, string tenantId, string correlationId, Exception exception, string contextData, params KeyValuePair<string, object>[] customData)
		{
			this.LogOneEntry(client, tenantId, correlationId, ExecutionLog.EventType.Error, string.Empty, contextData, exception, customData);
		}

		public override void LogOneEntry(string client, string correlationId, ExecutionLog.EventType eventType, string contextData, Exception exception)
		{
			this.LogOneEntry(client, null, correlationId, eventType, string.Empty, contextData, exception, new KeyValuePair<string, object>[0]);
		}

		public override void LogOneEntry(string client, string tenantId, string correlationId, ExecutionLog.EventType eventType, string tag, string contextData, Exception exception, params KeyValuePair<string, object>[] customData)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			Stream stream = null;
			logRowFormatter[1] = client;
			logRowFormatter[2] = tenantId;
			logRowFormatter[3] = correlationId;
			logRowFormatter[4] = eventType;
			logRowFormatter[5] = tag;
			logRowFormatter[6] = contextData;
			CustomDataLogger.Log(customData, logRowFormatter, out stream);
			if (exception != null)
			{
				List<string> list = null;
				List<string> list2 = null;
				string value = null;
				ExecutionLog.GetExceptionTypeAndDetails(exception, out list, out list2, out value, false);
				logRowFormatter[7] = list[0];
				logRowFormatter[8] = list2[0];
				if (list.Count > 1)
				{
					logRowFormatter[9] = list[list.Count - 1];
					logRowFormatter[10] = list2[list2.Count - 1];
				}
				if (!ExExecutionLog.ShouldSkipExceptionChainLogging(list))
				{
					logRowFormatter[11] = value;
				}
				logRowFormatter[12] = exception.GetHashCode().ToString();
			}
			this.logInstance.Append(logRowFormatter, 0);
			if (stream != null)
			{
				try
				{
					logRowFormatter.Write(stream);
				}
				catch (StorageTransientException)
				{
				}
				catch (StoragePermanentException)
				{
				}
			}
		}

		private static string GetLogFileName()
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
				{
					currentProcess.ProcessName,
					"UnifiedPolicyLog"
				});
			}
			return result;
		}

		private static bool ShouldSkipExceptionChainLogging(List<string> types)
		{
			if (types == null || types.Count == 0)
			{
				return true;
			}
			foreach (string text in types)
			{
				if (text.IndexOf(typeof(TenantAccessBlockedException).FullName, StringComparison.OrdinalIgnoreCase) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private const string DefaultLogPathForServicelet = "Logging\\UnifiedPolicy\\SyncAgent\\";

		private const string DefaultLogPathForCmdlet = "Logging\\UnifiedPolicy\\Cmdlet\\";

		private const string LogType = "Unified Policy Log";

		private const string LogComponent = "UnifiedPolicyLog";

		private const string LogSuffix = "UnifiedPolicyLog";

		private const int MaxLogDirectorySize = 262144000;

		private const int MaxLogFileSize = 10485760;

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly string[] CommonFields = (from ExExecutionLog.CommonField x in Enum.GetValues(typeof(ExExecutionLog.CommonField))
		select x.ToString()).ToArray<string>();

		private LogSchema logSchema;

		private Log logInstance;

		private enum CommonField
		{
			Time,
			Client,
			TenantId,
			CorrelationId,
			EventType,
			Tag,
			ContextData,
			OuterExceptionType,
			OuterExceptionMessage,
			InnerExceptionType,
			InnerExceptionMessage,
			ExceptionChain,
			ExceptionTag
		}
	}
}
