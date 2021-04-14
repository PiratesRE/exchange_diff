using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderTaskLoggerBase : DisposeTrackableBase, IFolderTaskLogger, IPublicFolderMailboxLoggerBase
	{
		public FolderTaskLoggerBase(IStoreSession storeSession, string logComponent, string logSuffixName, Guid? correlationId = null)
		{
			ArgumentValidator.ThrowIfNull("storeSession", storeSession);
			ArgumentValidator.ThrowIfNullOrEmpty("logComponent", logComponent);
			ArgumentValidator.ThrowIfNullOrEmpty("logSuffixName", logSuffixName);
			this.CorrelationId = (correlationId ?? Guid.NewGuid());
			this.storeSession = storeSession;
			this.logComponent = logComponent;
			this.logSuffixName = logSuffixName;
			this.organizationId = this.storeSession.OrganizationId;
			this.MailboxGuid = this.storeSession.MailboxGuid;
		}

		public Guid MailboxGuid { get; private set; }

		public Guid CorrelationId { get; private set; }

		private static string GetLogFileName(string logSuffixName)
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
				{
					currentProcess.ProcessName,
					logSuffixName
				});
			}
			return result;
		}

		internal static string GetExceptionLogString(Exception exception, FolderTaskLoggerBase.ExceptionLogOption option)
		{
			Exception ex = exception;
			if (ex == null)
			{
				return "[No Exception]";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				stringBuilder.Append("[Message:");
				stringBuilder.Append(ex.Message);
				stringBuilder.Append("]");
				stringBuilder.Append("[Type:");
				stringBuilder.Append(ex.GetType().ToString());
				stringBuilder.Append("]");
				if ((option & FolderTaskLoggerBase.ExceptionLogOption.IncludeStack) == FolderTaskLoggerBase.ExceptionLogOption.IncludeStack)
				{
					stringBuilder.Append("[Stack:");
					stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
					stringBuilder.Append("]");
				}
				int num = 0;
				if ((option & FolderTaskLoggerBase.ExceptionLogOption.IncludeInnerException) != FolderTaskLoggerBase.ExceptionLogOption.IncludeInnerException || ex.InnerException == null || num > 10)
				{
					break;
				}
				ex = ex.InnerException;
				num++;
			}
			return stringBuilder.ToString();
		}

		internal static void LogOnServer(Exception exception, string logComponent, string logSuffixName)
		{
			FolderTaskLoggerBase.LogOnServer(FolderTaskLoggerBase.GetExceptionLogString(exception, FolderTaskLoggerBase.ExceptionLogOption.All), LogEventType.Error, logComponent, logSuffixName, FolderTaskLoggerBase.LogType.Folder, null);
		}

		internal static void LogOnServer(string data, LogEventType eventType, string logComponent, string logSuffixName, FolderTaskLoggerBase.LogType logType, Guid? correlationId = null)
		{
			Log log = FolderTaskLoggerBase.InitializeServerLogging(logComponent, logSuffixName, logType);
			LogRowFormatter logRowFormatter = new LogRowFormatter(FolderTaskLoggerBase.GetLogSchema(logType));
			logRowFormatter[2] = eventType;
			logRowFormatter[5] = data;
			if (correlationId != null)
			{
				logRowFormatter[7] = correlationId;
			}
			log.Append(logRowFormatter, 0);
		}

		private static Log InitializeServerLogging(string logComponent, string logSuffixName, FolderTaskLoggerBase.LogType logType)
		{
			if (!FolderTaskLoggerBase.initializedLogs.ContainsKey(logSuffixName))
			{
				lock (FolderTaskLoggerBase.initializeLockObject)
				{
					if (!FolderTaskLoggerBase.initializedLogs.ContainsKey(logSuffixName))
					{
						Log log = new Log(FolderTaskLoggerBase.GetLogFileName(logSuffixName), new LogHeaderFormatter(FolderTaskLoggerBase.GetLogSchema(logType)), logComponent);
						log.Configure(Path.Combine(ExchangeSetupContext.InstallPath, FolderTaskLoggerBase.GetLogPath(logType)), FolderTaskLoggerBase.LogMaxAge, 262144000L, 10485760L);
						FolderTaskLoggerBase.initializedLogs.Add(logSuffixName, log);
					}
				}
			}
			return FolderTaskLoggerBase.initializedLogs[logSuffixName];
		}

		private static LogSchema GetLogSchema(FolderTaskLoggerBase.LogType logType)
		{
			return FolderTaskLoggerBase.FolderLogSchema;
		}

		private static string GetLogPath(FolderTaskLoggerBase.LogType logType)
		{
			return "Logging\\Folder\\";
		}

		public virtual void ReportError(string errorContextMessage, Exception syncException)
		{
			this.LogEvent(LogEventType.Error, string.Format(CultureInfo.InvariantCulture, "[ErrorContext:{0}] {1}", new object[]
			{
				errorContextMessage,
				FolderTaskLoggerBase.GetExceptionLogString(syncException, FolderTaskLoggerBase.ExceptionLogOption.All)
			}));
		}

		public virtual void LogEvent(LogEventType eventType, string data)
		{
			this.LogEvent(eventType, data, FolderTaskLoggerBase.LogType.Folder);
		}

		public virtual LogRowFormatter LogEvent(LogEventType eventType, string data, FolderTaskLoggerBase.LogType logType)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			Log log = FolderTaskLoggerBase.InitializeServerLogging(this.logComponent, this.logSuffixName, logType);
			LogRowFormatter logRowFormatter = new LogRowFormatter(FolderTaskLoggerBase.GetLogSchema(logType));
			logRowFormatter[2] = eventType;
			logRowFormatter[3] = this.organizationId.ToString();
			logRowFormatter[4] = this.MailboxGuid.ToString();
			logRowFormatter[7] = this.CorrelationId.ToString();
			logRowFormatter[5] = data;
			log.Append(logRowFormatter, 0);
			return logRowFormatter;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FolderTaskLoggerBase>(this);
		}

		internal const string CRLF = "\r\n";

		private const string FolderLogType = "Folder Diagnostics Log";

		private const string FolderLogPath = "Logging\\Folder\\";

		private const int MaxLogDirectorySize = 262144000;

		private const int MaxLogFileSize = 10485760;

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"subcomponent",
			"event-type",
			"organization-id",
			"mailbox-guid",
			"data",
			"context",
			"correlation-id"
		};

		private static readonly LogSchema FolderLogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Folder Diagnostics Log", FolderTaskLoggerBase.Fields);

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly object initializeLockObject = new object();

		private static Dictionary<string, Log> initializedLogs = new Dictionary<string, Log>();

		protected string logComponent;

		protected string logSuffixName;

		protected IStoreSession storeSession;

		protected OrganizationId organizationId;

		internal enum LogType
		{
			Folder
		}

		[Flags]
		internal enum ExceptionLogOption
		{
			Default = 1,
			IncludeStack = 2,
			IncludeInnerException = 4,
			All = 7
		}

		private enum Field
		{
			Time,
			Subcomponent,
			EventType,
			OrganizationId,
			MailboxGuid,
			Data,
			Context,
			CorrelationId
		}
	}
}
