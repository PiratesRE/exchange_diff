using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderMailboxLoggerBase : DisposeTrackableBase, IPublicFolderMailboxLoggerBase
	{
		public PublicFolderMailboxLoggerBase(IPublicFolderSession publicFolderSession, Guid? correlationId = null)
		{
			ArgumentValidator.ThrowIfNull("publicFolderSession", publicFolderSession);
			this.TransactionId = (correlationId ?? Guid.NewGuid());
			this.pfSession = publicFolderSession;
			this.organizationId = this.pfSession.OrganizationId;
			this.MailboxGuid = publicFolderSession.MailboxGuid;
		}

		public Guid MailboxGuid { get; private set; }

		public Guid TransactionId { get; private set; }

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

		public static string GetExceptionLogString(Exception e, PublicFolderMailboxLoggerBase.ExceptionLogOption option)
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
				if ((option & PublicFolderMailboxLoggerBase.ExceptionLogOption.IncludeStack) == PublicFolderMailboxLoggerBase.ExceptionLogOption.IncludeStack)
				{
					stringBuilder.Append("[Stack:");
					stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
					stringBuilder.Append("]");
				}
				if ((option & PublicFolderMailboxLoggerBase.ExceptionLogOption.IncludeInnerException) != PublicFolderMailboxLoggerBase.ExceptionLogOption.IncludeInnerException || ex.InnerException == null || num > 10)
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
			PublicFolderMailboxLoggerBase.LogOnServer(PublicFolderMailboxLoggerBase.GetExceptionLogString(exception, PublicFolderMailboxLoggerBase.ExceptionLogOption.All), LogEventType.Error, logComponent, logSuffixName, null);
		}

		internal static void LogOnServer(string data, LogEventType eventType, string logComponent, string logSuffixName, Guid? transactionId = null)
		{
			Log log = PublicFolderMailboxLoggerBase.InitializeServerLogging(logComponent, logSuffixName);
			LogRowFormatter logRowFormatter = new LogRowFormatter(PublicFolderMailboxLoggerBase.LogSchema);
			logRowFormatter[2] = eventType;
			logRowFormatter[5] = data;
			if (transactionId != null)
			{
				logRowFormatter[7] = transactionId;
			}
			log.Append(logRowFormatter, 0);
		}

		private static Log InitializeServerLogging(string logComponent, string logSuffixName)
		{
			if (!PublicFolderMailboxLoggerBase.initializedLogs.ContainsKey(logSuffixName))
			{
				lock (PublicFolderMailboxLoggerBase.initializeLockObject)
				{
					if (!PublicFolderMailboxLoggerBase.initializedLogs.ContainsKey(logSuffixName))
					{
						Log log = new Log(PublicFolderMailboxLoggerBase.GetLogFileName(logSuffixName), new LogHeaderFormatter(PublicFolderMailboxLoggerBase.LogSchema), logComponent);
						log.Configure(Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\PublicFolder\\"), PublicFolderMailboxLoggerBase.LogMaxAge, 262144000L, 10485760L);
						PublicFolderMailboxLoggerBase.initializedLogs.Add(logSuffixName, log);
					}
				}
			}
			return PublicFolderMailboxLoggerBase.initializedLogs[logSuffixName];
		}

		public virtual void ReportError(string errorContextMessage, Exception syncException)
		{
			this.LogEvent(LogEventType.Error, string.Format(CultureInfo.InvariantCulture, "[ErrorContext:{0}] {1}", new object[]
			{
				errorContextMessage,
				PublicFolderMailboxLoggerBase.GetExceptionLogString(syncException, PublicFolderMailboxLoggerBase.ExceptionLogOption.All)
			}));
		}

		public virtual void LogEvent(LogEventType eventType, string data)
		{
			LogRowFormatter logRowFormatter = null;
			this.LogEvent(eventType, data, out logRowFormatter);
		}

		public virtual void LogEvent(LogEventType eventType, string data, out LogRowFormatter row)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			Log log = PublicFolderMailboxLoggerBase.InitializeServerLogging(this.logComponent, this.logSuffixName);
			row = new LogRowFormatter(PublicFolderMailboxLoggerBase.LogSchema);
			row[2] = eventType;
			row[3] = this.organizationId.ToString();
			row[4] = this.MailboxGuid.ToString();
			row[7] = this.TransactionId.ToString();
			row[5] = data;
			log.Append(row, 0);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMailboxLoggerBase>(this);
		}

		internal const string CRLF = "\r\n";

		private const string LogType = "PublicFolder Diagnostics Log";

		private const string DefaultLogPath = "Logging\\PublicFolder\\";

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
			"transaction-id"
		};

		private static readonly LogSchema LogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "PublicFolder Diagnostics Log", PublicFolderMailboxLoggerBase.Fields);

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly object initializeLockObject = new object();

		private static Dictionary<string, Log> initializedLogs = new Dictionary<string, Log>();

		protected string logComponent;

		protected string logSuffixName;

		protected IPublicFolderSession pfSession;

		protected OrganizationId organizationId;

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
			TransactionId
		}
	}
}
