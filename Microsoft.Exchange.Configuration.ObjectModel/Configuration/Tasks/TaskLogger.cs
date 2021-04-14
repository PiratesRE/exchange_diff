using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TaskLogger
	{
		static TaskLogger()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				TaskLogger.processName = currentProcess.MainModule.ModuleName;
				TaskLogger.processId = currentProcess.Id;
			}
		}

		public static bool LogErrorAsWarning { get; set; }

		public static bool LogAllAsInfo { get; set; }

		public static bool IsPrereqLogging { get; set; }

		public static bool IsSetupLogging
		{
			get
			{
				return TaskLogger.isSetupLogging;
			}
			set
			{
				TaskLogger.isSetupLogging = value;
			}
		}

		internal static bool IsFileLoggingEnabled
		{
			get
			{
				return TaskLogger.fileLoggingEnabled;
			}
		}

		private static IFormatProvider FormatProvider
		{
			get
			{
				return TaskLogger.formatProvider;
			}
			set
			{
				TaskLogger.formatProvider = value;
			}
		}

		public static void UnmanagedLog(string s)
		{
			TaskLogger.Log(new LocalizedString(s));
		}

		public static void Log(LocalizedString localizedString)
		{
			if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.LogTracer.Information(0L, TaskLogger.FormatLocalizedString(localizedString));
			}
			if (TaskLogger.fileLoggingEnabled)
			{
				TaskLogger.LogMessageString(TaskLogger.FormatLocalizedString(localizedString));
			}
		}

		public static void LogWarning(LocalizedString localizedString)
		{
			if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.LogTracer.Information(0L, TaskLogger.FormatLocalizedString(localizedString));
			}
			if (TaskLogger.fileLoggingEnabled)
			{
				TaskLogger.LogWarningString(TaskLogger.FormatLocalizedString(localizedString));
			}
		}

		public static void LogError(Exception e)
		{
			while (e != null)
			{
				LocalizedException ex = e as LocalizedException;
				string message;
				if (ex != null && TaskLogger.FormatProvider != null)
				{
					IFormatProvider formatProvider = ex.FormatProvider;
					ex.FormatProvider = TaskLogger.FormatProvider;
					message = ex.Message;
					ex.FormatProvider = formatProvider;
				}
				else
				{
					message = e.Message;
				}
				ExTraceGlobals.ErrorTracer.TraceError<LocalizedString, string, string>(0L, "{0}{1}\n{2}", Strings.LogErrorPrefix, message, e.StackTrace);
				if (TaskLogger.fileLoggingEnabled)
				{
					TaskLogger.LogErrorString(message);
				}
				e = e.InnerException;
			}
		}

		public static void SendWatsonReport(Exception e)
		{
			TaskLogger.SendWatsonReport(e, null, null);
		}

		public static void SendWatsonReport(Exception e, string taskName, PropertyBag boundParameters)
		{
			TaskLogger.StopFileLogging();
			bool flag = true;
			try
			{
				string sourceFileName = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, ConfigurationContext.Setup.SetupLogFileName);
				string text = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, ConfigurationContext.Setup.SetupLogFileNameForWatson);
				File.Copy(sourceFileName, text, true);
				int num = 0;
				while (!ExWatson.TryAddExtraFile(text) && num < 10)
				{
					Thread.Sleep(100);
					num++;
				}
			}
			catch (FileNotFoundException)
			{
			}
			catch (DirectoryNotFoundException)
			{
			}
			catch (IOException)
			{
				flag = false;
				if (TaskLogger.IsFileLoggingEnabled)
				{
					TaskLogger.LogErrorString(Strings.ExchangeSetupCannotCopyWatson(ConfigurationContext.Setup.SetupLogFileName, ConfigurationContext.Setup.SetupLogFileNameForWatson));
				}
			}
			if (flag)
			{
				if (!string.IsNullOrEmpty(taskName))
				{
					ExWatson.AddExtraData("Task Name: " + taskName);
				}
				if (boundParameters != null)
				{
					StringBuilder stringBuilder = new StringBuilder("Parameters:\n");
					foreach (object obj in boundParameters)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						if (dictionaryEntry.Value is IList)
						{
							stringBuilder.AppendLine(string.Format("{0}:{1}", dictionaryEntry.Key, MultiValuedPropertyBase.FormatMultiValuedProperty(dictionaryEntry.Value as IList)));
						}
						else
						{
							stringBuilder.AppendLine(string.Format("{0}:'{1}'", dictionaryEntry.Key, (dictionaryEntry.Value == null) ? "<null>" : dictionaryEntry.Value.ToString()));
						}
					}
					ExWatson.AddExtraData(stringBuilder.ToString());
				}
				ExWatson.SendReport(e, ReportOptions.DoNotFreezeThreads, null);
			}
			try
			{
				TaskLogger.ResumeFileLogging();
			}
			catch (IOException)
			{
				if (TaskLogger.IsFileLoggingEnabled)
				{
					TaskLogger.LogErrorString(Strings.ExchangeSetupCannotResumeLog(ConfigurationContext.Setup.SetupLogFileName));
				}
				throw;
			}
		}

		public static void LogEnter()
		{
			if (!ExTraceGlobals.EnterExitTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			ExTraceGlobals.EnterExitTracer.Information(0L, Strings.LogFunctionEnter(method.ReflectedType, method.Name, ""));
		}

		public static void LogEnter(params object[] arguments)
		{
			if (!ExTraceGlobals.EnterExitTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			StringBuilder stringBuilder = new StringBuilder();
			if (arguments != null)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					stringBuilder.Append((arguments[i] != null) ? arguments[i].ToString() : "null");
					if (i + 1 < arguments.Length)
					{
						stringBuilder.Append(", ");
					}
				}
			}
			ExTraceGlobals.EnterExitTracer.Information(0L, Strings.LogFunctionEnter(method.ReflectedType, method.Name, stringBuilder.ToString()));
		}

		public static void LogExit()
		{
			if (!ExTraceGlobals.EnterExitTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			MethodBase method = stackTrace.GetFrame(1).GetMethod();
			ExTraceGlobals.EnterExitTracer.Information(0L, Strings.LogFunctionExit(method.ReflectedType, method.Name));
		}

		public static void Trace(LocalizedString localizedString)
		{
			ExTraceGlobals.TraceTracer.Information(0L, TaskLogger.FormatLocalizedString(localizedString));
		}

		public static void Trace(string format, params object[] objects)
		{
			ExTraceGlobals.TraceTracer.Information(0L, format, objects);
		}

		public static void StartFileLogging(string path, string dataMiningPath = null)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			TaskLogger.indentationLevel = 0;
			TaskLogger.logFilePath = path;
			TaskLogger.dataMiningLogFilePath = dataMiningPath;
			TaskLogger.ResumeFileLogging();
		}

		public static void StopFileLogging()
		{
			if (!TaskLogger.fileLoggingEnabled)
			{
				return;
			}
			TaskLogger.fileLoggingEnabled = false;
			TaskLogger.sw.Dispose();
			TaskLogger.sw = null;
			if (TaskLogger.swDataMining != null)
			{
				TaskLogger.swDataMining.Dispose();
				TaskLogger.swDataMining = null;
			}
			TaskLogger.FormatProvider = null;
		}

		public static void IncreaseIndentation(LocalizedString tag)
		{
			if (!TaskLogger.fileLoggingEnabled)
			{
				return;
			}
			TaskLogger.indentationLevel++;
			if (!string.IsNullOrEmpty(tag))
			{
				TaskLogger.Log(tag);
			}
		}

		public static void IncreaseIndentation()
		{
			TaskLogger.IncreaseIndentation(LocalizedString.Empty);
		}

		public static void DecreaseIndentation()
		{
			if (!TaskLogger.fileLoggingEnabled)
			{
				return;
			}
			TaskLogger.indentationLevel--;
		}

		private static ExEventLog GetEventLogger(string hostName)
		{
			if (string.Compare(hostName, "Exchange Management Console", StringComparison.Ordinal) == 0)
			{
				return TaskLogger.emcEventLogger.Value;
			}
			return TaskLogger.cmdletIterationLogger.Value;
		}

		internal static void LogRbacEvent(ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 2];
			array[0] = TaskLogger.processName;
			array[1] = TaskLogger.processId;
			messageArguments.CopyTo(array, 2);
			TaskLogger.LogEvent(TaskLogger.rbacEventLogger.Value, eventInfo, periodicKey, array);
		}

		public static void LogEvent(string hostName, ExEventLog.EventTuple eventInfo, params object[] messageArguments)
		{
			TaskLogger.LogEvent(hostName, eventInfo, null, messageArguments);
		}

		public static void LogEvent(string hostName, ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			TaskLogger.LogEvent(TaskLogger.GetEventLogger(hostName), eventInfo, periodicKey, messageArguments);
		}

		public static void LogEvent(ExEventLog.EventTuple eventInfo, TaskInvocationInfo invocationInfo, string periodicKey, params object[] messageArguments)
		{
			if (TaskLogger.IsSetupLogging)
			{
				return;
			}
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 3];
			array[0] = TaskLogger.processId;
			array[1] = Environment.CurrentManagedThreadId;
			array[2] = invocationInfo.DisplayName;
			messageArguments.CopyTo(array, 3);
			TaskLogger.LogEvent(invocationInfo.ShellHostName, eventInfo, periodicKey, array);
		}

		public static void LogDataMiningMessage(string version, string task, DateTime startTime)
		{
			if (TaskLogger.indentationLevel == 0 && TaskLogger.fileLoggingEnabled && TaskLogger.swDataMining != null)
			{
				try
				{
					TimeSpan timeSpan = DateTime.UtcNow.Subtract(startTime);
					TaskLogger.swDataMining.WriteLine(string.Format("{0},{1},{2},{3},{4}", new object[]
					{
						startTime.ToString("MM/dd/yyyy HH:mm:ss"),
						timeSpan.Ticks,
						version,
						TaskLogger.indentationLevel,
						Regex.Replace(task.Replace(',', '_'), "15\\.[\\d|\\.]*", string.Empty)
					}));
				}
				catch (IOException)
				{
				}
			}
		}

		private static void LogEvent(ExEventLog logChannel, ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			logChannel.LogEvent(eventInfo, periodicKey, messageArguments);
			ExTraceGlobals.EventTracer.Information(0L, eventInfo.ToString(), messageArguments);
		}

		internal static void ResumeFileLogging()
		{
			if (TaskLogger.logFilePath != null)
			{
				TaskLogger.FormatProvider = new CultureInfo("en-US");
				FileStream stream = new FileStream(TaskLogger.logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				TaskLogger.sw = new StreamWriter(stream);
				TaskLogger.sw.AutoFlush = true;
				TaskLogger.fileLoggingEnabled = true;
			}
			if (TaskLogger.dataMiningLogFilePath != null)
			{
				FileStream stream2 = new FileStream(TaskLogger.dataMiningLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				TaskLogger.swDataMining = new StreamWriter(stream2);
			}
		}

		private static string FormatLocalizedString(LocalizedString locString)
		{
			if (TaskLogger.FormatProvider != null)
			{
				return locString.ToString(TaskLogger.FormatProvider);
			}
			return locString.ToString();
		}

		private static void LogErrorString(string message)
		{
			if (TaskLogger.IsPrereqLogging)
			{
				TaskLogger.LogMessageString("[REQUIRED] " + message);
				return;
			}
			if (TaskLogger.LogAllAsInfo)
			{
				TaskLogger.LogMessageString(message);
				return;
			}
			if (TaskLogger.LogErrorAsWarning)
			{
				TaskLogger.LogWarningString(message);
				return;
			}
			TaskLogger.LogMessageString("[ERROR] " + message);
		}

		private static void LogWarningString(string message)
		{
			if (TaskLogger.IsPrereqLogging)
			{
				TaskLogger.LogMessageString("[RECOMENDED] " + message);
				return;
			}
			if (TaskLogger.LogAllAsInfo)
			{
				TaskLogger.LogMessageString(message);
				return;
			}
			TaskLogger.LogMessageString("[WARNING] " + message);
		}

		private static void LogMessageString(string message)
		{
			try
			{
				DateTime utcNow = DateTime.UtcNow;
				TaskLogger.sw.WriteLine(string.Format("[{0}.{1:0000}] [{2}] {3}", new object[]
				{
					utcNow.ToString("MM/dd/yyyy HH:mm:ss"),
					utcNow.Millisecond,
					TaskLogger.indentationLevel,
					message
				}));
			}
			catch (IOException)
			{
			}
		}

		internal const string MSExchangeManagementEventLogName = "MSExchange Management";

		private const string emcHostName = "Exchange Management Console";

		private const int minimumIndentationLevel = 0;

		private const int maximumIndentationLevel = 3;

		private const string errorTag = "[ERROR] ";

		private const string warningTag = "[WARNING] ";

		private const string prereqErrorTags = "[REQUIRED] ";

		private const string prereqWarningTags = "[RECOMENDED] ";

		private static readonly Lazy<ExEventLog> emcEventLogger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.LogTracer.Category, "MSExchange Configuration Cmdlet - Management Console", "MSExchange Management"));

		private static readonly Lazy<ExEventLog> cmdletIterationLogger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.LogTracer.Category, "MSExchange CmdletLogs", "MSExchange Management"));

		private static readonly Lazy<ExEventLog> rbacEventLogger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.LogTracer.Category, "MSExchange RBAC"));

		private static int indentationLevel;

		private static bool fileLoggingEnabled = false;

		private static string logFilePath;

		private static string dataMiningLogFilePath;

		private static StreamWriter sw;

		private static StreamWriter swDataMining;

		private static bool isSetupLogging = false;

		private static IFormatProvider formatProvider;

		private static string processName;

		private static int processId;
	}
}
