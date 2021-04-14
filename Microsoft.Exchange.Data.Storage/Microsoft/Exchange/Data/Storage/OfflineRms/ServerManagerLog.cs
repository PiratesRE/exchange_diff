using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ServerManagerLog
	{
		public static string GetExceptionLogString(Exception e, ServerManagerLog.ExceptionLogOption option)
		{
			Exception ex = e;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (;;)
			{
				stringBuilder.Append("[Message:");
				stringBuilder.Append(ex.Message);
				stringBuilder.Append("]");
				stringBuilder.Append("[Type:");
				stringBuilder.Append(ex.GetType().ToString());
				stringBuilder.Append("]");
				if ((option & ServerManagerLog.ExceptionLogOption.IncludeStack) == ServerManagerLog.ExceptionLogOption.IncludeStack)
				{
					stringBuilder.Append("[Stack:");
					stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
					stringBuilder.Append("]");
				}
				if ((option & ServerManagerLog.ExceptionLogOption.IncludeInnerException) != ServerManagerLog.ExceptionLogOption.IncludeInnerException || ex.InnerException == null || num > 10)
				{
					break;
				}
				ex = ex.InnerException;
				num++;
			}
			return stringBuilder.ToString();
		}

		public static void LogEvent(ServerManagerLog.Subcomponent subcomponent, ServerManagerLog.EventType eventType, RmsClientManagerContext clientManagerContext, string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			ServerManagerLog.InitializeIfNeeded();
			LogRowFormatter logRowFormatter = new LogRowFormatter(ServerManagerLog.LogSchema);
			logRowFormatter[1] = subcomponent;
			logRowFormatter[2] = eventType;
			if (clientManagerContext != null)
			{
				logRowFormatter[3] = clientManagerContext.OrgId.OrganizationalUnit.ToString();
				logRowFormatter[6] = clientManagerContext.TransactionId.ToString();
				if (clientManagerContext.ContextID != RmsClientManagerContext.ContextId.None && !string.IsNullOrEmpty(clientManagerContext.ContextStringForm))
				{
					logRowFormatter[5] = clientManagerContext.ContextStringForm;
				}
			}
			logRowFormatter[4] = data;
			ServerManagerLog.instance.logInstance.Append(logRowFormatter, 0);
		}

		private static void InitializeIfNeeded()
		{
			if (!ServerManagerLog.instance.initialized)
			{
				lock (ServerManagerLog.instance.initializeLockObject)
				{
					if (!ServerManagerLog.instance.initialized)
					{
						ServerManagerLog.instance.Initialize();
						ServerManagerLog.instance.initialized = true;
					}
				}
			}
		}

		private void Initialize()
		{
			ServerManagerLog.instance.logInstance = new Log(RmsClientManagerUtils.GetUniqueFileNameForProcess("OfflineRMSLog", true), new LogHeaderFormatter(ServerManagerLog.LogSchema), "OfflineRMSServerLog");
			ServerManagerLog.instance.logInstance.Configure(Path.Combine(ExchangeSetupContext.InstallPath, "TransportRoles\\Logs\\OfflineRMS\\"), ServerManagerLog.LogMaxAge, 262144000L, 10485760L);
		}

		internal const string CRLF = "\r\n";

		private const string DefaultLogPath = "TransportRoles\\Logs\\OfflineRMS\\";

		private const string LogType = "OfflineRMS Server Log";

		private const string LogComponent = "OfflineRMSServerLog";

		private const string LogSuffix = "OfflineRMSLog";

		private const int MaxLogDirectorySize = 262144000;

		private const int MaxLogFileSize = 10485760;

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly ServerManagerLog instance = new ServerManagerLog();

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"subcomponent",
			"event-type",
			"tenant-id",
			"data",
			"context",
			"transaction-id"
		};

		private static readonly LogSchema LogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "OfflineRMS Server Log", ServerManagerLog.Fields);

		private readonly object initializeLockObject = new object();

		private Log logInstance;

		private bool initialized;

		internal enum Subcomponent
		{
			ServerInit,
			AcquireUseLicense,
			AcquireTenantLicenses,
			DirectoryServiceProvider,
			RpcClientWrapper,
			RpcServerWrapper,
			TrustedPublishingDomainPrivateKeyProvider,
			PerTenantRMSTrustedPublishingDomainConfiguration
		}

		internal enum EventType
		{
			Entry,
			Verbose,
			Success,
			Warning,
			Error,
			Statistics
		}

		[Flags]
		internal enum ExceptionLogOption
		{
			Default = 1,
			IncludeStack = 2,
			IncludeInnerException = 4
		}

		private enum Field
		{
			Time,
			Subcomponent,
			EventType,
			TenantId,
			Data,
			Context,
			TransactionId
		}
	}
}
