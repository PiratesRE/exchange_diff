using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProtocolLog
	{
		private static void GetExceptionTypeAndDetails(Exception e, out List<string> types, out List<string> messages, out string chain, bool chainOnly)
		{
			Exception ex = e;
			chain = string.Empty;
			types = null;
			messages = null;
			if (!chainOnly)
			{
				types = new List<string>();
				messages = new List<string>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			for (;;)
			{
				string text = ex.GetType().ToString();
				string text2 = ex.Message;
				if (ex is SharePointException && ex.InnerException != null && ex.InnerException is WebException)
				{
					text = text + "; WebException:" + text2;
					text2 = text2 + "; DiagnosticInfo:" + ((SharePointException)ex).DiagnosticInfo;
				}
				if (!chainOnly)
				{
					types.Add(text);
					messages.Add(text2);
				}
				stringBuilder.Append("[Type:");
				stringBuilder.Append(text);
				stringBuilder.Append("]");
				stringBuilder.Append("[Message:");
				stringBuilder.Append(text2);
				stringBuilder.Append("]");
				stringBuilder.Append("[Stack:");
				stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
				stringBuilder.Append("]");
				if (ex.InnerException == null || num > 10)
				{
					break;
				}
				ex = ex.InnerException;
				num++;
			}
			chain = stringBuilder.ToString();
		}

		private static void LogEvent(ProtocolLog.Component component, ProtocolLog.EventType eventType, LoggingContext loggingContext, string data, Exception exception)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			ProtocolLog.InitializeIfNeeded();
			LogRowFormatter logRowFormatter = new LogRowFormatter(ProtocolLog.LogSchema);
			logRowFormatter[1] = component;
			logRowFormatter[2] = eventType;
			if (loggingContext != null)
			{
				logRowFormatter[5] = loggingContext.MailboxGuid.ToString();
				logRowFormatter[3] = loggingContext.TransactionId.ToString();
				logRowFormatter[4] = loggingContext.User;
				logRowFormatter[6] = loggingContext.Context;
			}
			logRowFormatter[7] = data;
			if (exception != null)
			{
				List<string> list = null;
				List<string> list2 = null;
				string value = null;
				ProtocolLog.GetExceptionTypeAndDetails(exception, out list, out list2, out value, false);
				logRowFormatter[8] = list[0];
				logRowFormatter[9] = list2[0];
				if (list.Count > 1)
				{
					logRowFormatter[10] = list[list.Count - 1];
					logRowFormatter[11] = list2[list2.Count - 1];
				}
				logRowFormatter[12] = value;
			}
			ProtocolLog.instance.logInstance.Append(logRowFormatter, 0);
			if (loggingContext != null && loggingContext.LoggingStream != null)
			{
				try
				{
					logRowFormatter.Write(loggingContext.LoggingStream);
				}
				catch (StorageTransientException)
				{
				}
				catch (StoragePermanentException)
				{
				}
			}
		}

		private static void InitializeIfNeeded()
		{
			if (!ProtocolLog.instance.initialized)
			{
				lock (ProtocolLog.instance.initializeLockObject)
				{
					if (!ProtocolLog.instance.initialized)
					{
						ProtocolLog.instance.Initialize();
						ProtocolLog.instance.initialized = true;
					}
				}
			}
		}

		private void Initialize()
		{
			ProtocolLog.instance.logInstance = new Log(ProtocolLog.GetLogFileName(), new LogHeaderFormatter(ProtocolLog.LogSchema), "TeamMailboxSyncLog");
			ProtocolLog.instance.logInstance.Configure(Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\TeamMailbox\\"), ProtocolLog.LogMaxAge, 262144000L, 10485760L);
		}

		public static void LogError(ProtocolLog.Component component, LoggingContext loggingContext, string data, Exception exception)
		{
			ProtocolLog.LogEvent(component, ProtocolLog.EventType.Error, loggingContext, data, exception);
		}

		public static void LogInformation(ProtocolLog.Component component, LoggingContext loggingContext, string data)
		{
			ProtocolLog.LogEvent(component, ProtocolLog.EventType.Information, loggingContext, data, null);
		}

		public static void LogCycleSuccess(ProtocolLog.Component component, LoggingContext loggingContext, string data)
		{
			ProtocolLog.LogEvent(component, ProtocolLog.EventType.CycleSuccess, loggingContext, data, null);
		}

		public static void LogCycleFailure(ProtocolLog.Component component, LoggingContext loggingContext, string data, Exception exception)
		{
			ProtocolLog.LogEvent(component, ProtocolLog.EventType.CycleFailure, loggingContext, data, exception);
		}

		public static void LogStatistics(ProtocolLog.Component component, LoggingContext loggingContext, string data)
		{
			ProtocolLog.LogEvent(component, ProtocolLog.EventType.Statistics, loggingContext, data, null);
		}

		public static string GetLogFileName()
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
				{
					currentProcess.ProcessName,
					"TeamMailboxSyncLog"
				});
			}
			return result;
		}

		public static string GetExceptionLogString(Exception e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e is null");
			}
			string empty = string.Empty;
			List<string> list = null;
			List<string> list2 = null;
			ProtocolLog.GetExceptionTypeAndDetails(e, out list, out list2, out empty, true);
			return empty;
		}

		internal const string CRLF = "\r\n";

		private const string DefaultLogPath = "Logging\\TeamMailbox\\";

		private const string LogType = "TeamMailbox Synchronization Log";

		private const string LogComponent = "TeamMailboxSyncLog";

		private const string LogSuffix = "TeamMailboxSyncLog";

		private const int MaxLogDirectorySize = 262144000;

		private const int MaxLogFileSize = 10485760;

		private static readonly EnhancedTimeSpan LogMaxAge = EnhancedTimeSpan.FromDays(30.0);

		private static readonly ProtocolLog instance = new ProtocolLog();

		private static readonly string[] Fields = new string[]
		{
			"date-time",
			"component",
			"event-type",
			"transaction-id",
			"client",
			"sitemailbox-guid",
			"sharepoint-url",
			"data",
			"outerexception-type",
			"outerexception-message",
			"innerexception-type",
			"innerexception-message",
			"exceptionchain"
		};

		private static readonly LogSchema LogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "TeamMailbox Synchronization Log", ProtocolLog.Fields);

		private readonly object initializeLockObject = new object();

		private Log logInstance;

		private bool initialized;

		private enum Field
		{
			Time,
			Component,
			EventType,
			TransactionId,
			Client,
			SiteMailboxGuid,
			SharepointUrl,
			Data,
			OuterExceptionType,
			OuterExceptionMessage,
			InnerExceptionType,
			InnerExceptionMessage,
			ExceptionChain
		}

		internal enum Component
		{
			DocumentSync,
			MembershipSync,
			Maintenance,
			MoMT,
			Assistant,
			SharepointAccessManager,
			Monitor
		}

		internal enum EventType
		{
			Information,
			Error,
			Statistics,
			CycleSuccess,
			CycleFailure
		}
	}
}
