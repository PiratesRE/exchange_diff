using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal class DiagnosticsSession : IDiagnosticsSession
	{
		internal DiagnosticsSession(string componentName, string eventLogSourceName, Microsoft.Exchange.Diagnostics.Trace tracer, long traceContext, IIdentity documentId, DiagnosticsLogConfig.LogDefaults logDefaults, DiagnosticsLogConfig.LogDefaults crawlerLogDefaults) : this(componentName, eventLogSourceName, tracer, traceContext, documentId, logDefaults, crawlerLogDefaults, DiagnosticsSessionFactory.GracefulDegradationLogDefaults, DiagnosticsSessionFactory.DictionaryLogDefaults)
		{
		}

		internal DiagnosticsSession(string componentName, string eventLogSourceName, Microsoft.Exchange.Diagnostics.Trace tracer, long traceContext, IIdentity documentId, DiagnosticsLogConfig.LogDefaults logDefaults, DiagnosticsLogConfig.LogDefaults crawlerLogDefaults, DiagnosticsLogConfig.LogDefaults gracefulDegradationLogDefaults, DiagnosticsLogConfig.LogDefaults dictionaryLogDefaults)
		{
			this.componentName = (string.IsNullOrEmpty(componentName) ? string.Empty : string.Format("{0}_{1}", componentName, traceContext));
			this.tracer = tracer;
			this.traceContext = traceContext;
			this.documentId = documentId;
			this.logDefaults = logDefaults;
			this.eventLogSourceName = eventLogSourceName;
			this.crawlerLogDefaults = crawlerLogDefaults;
			this.gracefulDegradationLogDefaults = gracefulDegradationLogDefaults;
			this.dictionaryLogDefaults = dictionaryLogDefaults;
		}

		public static bool CrashOnUnhandledException
		{
			get
			{
				return DiagnosticsSession.crashOnUnhandledException;
			}
		}

		public Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return this.tracer;
			}
			set
			{
				this.tracer = value;
			}
		}

		public string ComponentName
		{
			set
			{
				this.componentName = value;
			}
		}

		public ExEventLog EventLog
		{
			get
			{
				if (DiagnosticsSession.eventLog == null)
				{
					DiagnosticsSession.eventLog = new ExEventLog(this.Log.Config.EventLogComponentGuid, this.eventLogSourceName ?? this.Log.Config.ServiceName);
				}
				return DiagnosticsSession.eventLog;
			}
		}

		private static string AppName
		{
			get
			{
				if (DiagnosticsSession.appName == null)
				{
					DiagnosticsSession.appName = ExWatson.AppName;
				}
				return DiagnosticsSession.appName;
			}
		}

		private static string AppVersion
		{
			get
			{
				if (DiagnosticsSession.appVersion == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Version version;
						if (ExWatson.TryGetRealApplicationVersion(currentProcess, out version))
						{
							DiagnosticsSession.appVersion = version.ToString();
						}
						else
						{
							DiagnosticsSession.appVersion = "0";
						}
					}
				}
				return DiagnosticsSession.appVersion;
			}
		}

		private DiagnosticsLog Log
		{
			get
			{
				if (this.diagnosticsLog == null)
				{
					this.diagnosticsLog = new DiagnosticsLog(new DiagnosticsLogConfig(this.logDefaults), DiagnosticsSession.schemaColumns);
				}
				return this.diagnosticsLog;
			}
		}

		private DiagnosticsLog CrawlerLog
		{
			get
			{
				if (this.crawlerLog == null)
				{
					this.crawlerLog = new DiagnosticsLog(new DiagnosticsLogConfig(this.crawlerLogDefaults), DiagnosticsSession.crawlerSchemaColumns);
				}
				return this.crawlerLog;
			}
		}

		private DiagnosticsLog GracefulDegradationLog
		{
			get
			{
				if (this.gracefulDegradationLog == null)
				{
					this.gracefulDegradationLog = new DiagnosticsLog(new DiagnosticsLogConfig(this.gracefulDegradationLogDefaults), DiagnosticsSession.gracefulDegradationSchemaColumns);
				}
				return this.gracefulDegradationLog;
			}
		}

		private DiagnosticsLog DictionaryLog
		{
			get
			{
				if (this.dictionaryLog == null)
				{
					this.dictionaryLog = new DiagnosticsLog(new DiagnosticsLogConfig(this.dictionaryLogDefaults), DiagnosticsSession.dictionarySchemaColumns);
				}
				return this.dictionaryLog;
			}
		}

		public static IDisposable SetFactoryTestHook(IDiagnosticsSessionFactory diagnosticsSessionFactory)
		{
			return DiagnosticsSession.hookableDiagnosticsSessionFactory.SetTestHook(diagnosticsSessionFactory);
		}

		public static IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, Microsoft.Exchange.Diagnostics.Trace tracer, long traceContext)
		{
			return DiagnosticsSession.hookableDiagnosticsSessionFactory.Value.CreateComponentDiagnosticsSession(componentName, tracer, traceContext);
		}

		public static IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, string eventLogSourceName, Microsoft.Exchange.Diagnostics.Trace tracer, long traceContext)
		{
			return DiagnosticsSession.hookableDiagnosticsSessionFactory.Value.CreateComponentDiagnosticsSession(componentName, eventLogSourceName, tracer, traceContext);
		}

		public static IDiagnosticsSession CreateDocumentDiagnosticsSession(IIdentity documentId, Microsoft.Exchange.Diagnostics.Trace tracer)
		{
			return DiagnosticsSession.hookableDiagnosticsSessionFactory.Value.CreateDocumentDiagnosticsSession(documentId, tracer);
		}

		public string[] GetExtendedLoggingInformation()
		{
			return this.Log.GetFormattedExtendedLogging();
		}

		public void Assert(bool condition, string message, params object[] messageArgs)
		{
			ExAssert.RetailAssert(condition, message, messageArgs);
		}

		public void LogDiagnosticsInfo(DiagnosticsLoggingTag loggingTag, string operation, params object[] operationSpecificData)
		{
			this.LogDiagnosticsInfo(loggingTag, this.documentId, operation, operationSpecificData);
		}

		public void LogDiagnosticsInfo(DiagnosticsLoggingTag loggingTag, IIdentity documentId, string operation, params object[] operationSpecificData)
		{
			this.LogDiagnosticsInfo(loggingTag, (documentId == null) ? null : documentId.ToString(), operation, operationSpecificData);
		}

		public void LogDiagnosticsInfo(DiagnosticsLoggingTag loggingTag, string entryId, string operation, params object[] operationSpecificData)
		{
			if (!this.Log.IsEnabled || (loggingTag & this.Log.Config.DiagnosticsLoggingTag) == DiagnosticsLoggingTag.None)
			{
				return;
			}
			string text = string.Empty;
			if (operationSpecificData.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object arg in operationSpecificData)
				{
					stringBuilder.AppendFormat("{0},", arg);
				}
				text = stringBuilder.ToString().TrimEnd(new char[]
				{
					','
				});
			}
			string text2 = (operationSpecificData == null || operationSpecificData.Length == 0) ? operation : string.Format(operation, operationSpecificData);
			this.LogLine(this.Log, loggingTag, new object[]
			{
				null,
				loggingTag.ToString(),
				1,
				this.componentName,
				entryId,
				text2,
				text
			});
		}

		public void LogCrawlerInfo(DiagnosticsLoggingTag loggingTag, string operationId, string databaseName, string mailboxGuid, string operation, params object[] operationSpecificData)
		{
			if (!this.CrawlerLog.IsEnabled || (loggingTag & this.CrawlerLog.Config.DiagnosticsLoggingTag) == DiagnosticsLoggingTag.None)
			{
				return;
			}
			string text = (operationSpecificData == null || operationSpecificData.Length == 0) ? operation : string.Format(operation, operationSpecificData);
			this.LogLine(this.CrawlerLog, loggingTag, new object[]
			{
				null,
				this.componentName,
				3,
				operationId,
				databaseName,
				mailboxGuid,
				text
			});
		}

		public void LogGracefulDegradationInfo(DiagnosticsLoggingTag loggingTag, long totalMemory, long availableMemory, long actualMemoryUsage, long expectedMemoryUsage, float searchMemoryDriftRatio, string operation, params object[] operationSpecificData)
		{
			if (!this.GracefulDegradationLog.IsEnabled || (loggingTag & this.GracefulDegradationLog.Config.DiagnosticsLoggingTag) == DiagnosticsLoggingTag.None)
			{
				return;
			}
			string text = (operationSpecificData == null || operationSpecificData.Length == 0) ? operation : string.Format(operation, operationSpecificData);
			this.LogLine(this.GracefulDegradationLog, loggingTag, new object[]
			{
				null,
				this.componentName,
				1,
				totalMemory,
				availableMemory,
				actualMemoryUsage,
				expectedMemoryUsage,
				searchMemoryDriftRatio.ToString("0.00"),
				text
			});
		}

		public void LogDictionaryInfo(DiagnosticsLoggingTag loggingTag, int operationId, Guid correlationId, Guid database, Guid mailbox, string operation, params object[] operationSpecificData)
		{
			if (!this.DictionaryLog.IsEnabled || (loggingTag & this.DictionaryLog.Config.DiagnosticsLoggingTag) == DiagnosticsLoggingTag.None)
			{
				return;
			}
			string text = (operationSpecificData == null || operationSpecificData.Length == 0) ? operation : string.Format(operation, operationSpecificData);
			this.LogLine(this.DictionaryLog, loggingTag, new object[]
			{
				null,
				this.componentName,
				1,
				operationId,
				correlationId,
				database,
				mailbox,
				text
			});
		}

		public void LogPeriodicEvent(ExEventLog.EventTuple eventId, string periodicLogName, params object[] messageArgs)
		{
			Util.ThrowOnNullArgument(eventId, "eventId");
			this.EventLog.LogEvent(eventId, periodicLogName, messageArgs);
			Util.ThrowOnNullOrEmptyArgument(periodicLogName, "periodicLogName");
			Util.ThrowOnConditionFailed(eventId.Period == ExEventLog.EventPeriod.LogPeriodic, "LogPeriodicEvent method should only be called for LogPeriodic type events");
		}

		public void LogEvent(ExEventLog.EventTuple eventId, params object[] messageArgs)
		{
			Util.ThrowOnNullArgument(eventId, "eventId");
			this.EventLog.LogEvent(eventId, string.Empty, messageArgs);
			Util.ThrowOnConditionFailed(eventId.Period == ExEventLog.EventPeriod.LogAlways, "LogEvent method should only be called for LogAlways type events");
		}

		public void TraceDebug(string message, params object[] messageArgs)
		{
			this.tracer.TraceDebug(this.traceContext, message, messageArgs);
		}

		public void TraceDebug<T0>(string message, T0 arg0)
		{
			this.tracer.TraceDebug<T0>(this.traceContext, message, arg0);
		}

		public void TraceDebug<T0, T1>(string message, T0 arg0, T1 arg1)
		{
			this.tracer.TraceDebug<T0, T1>(this.traceContext, message, arg0, arg1);
		}

		public void TraceDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
		{
			this.tracer.TraceDebug<T0, T1, T2>(this.traceContext, message, arg0, arg1, arg2);
		}

		public void TraceError(string message, params object[] messageArgs)
		{
			this.tracer.TraceError(this.traceContext, message, messageArgs);
		}

		public void TraceError<T0>(string message, T0 arg0)
		{
			this.tracer.TraceError<T0>(this.traceContext, message, arg0);
		}

		public void TraceError<T0, T1>(string message, T0 arg0, T1 arg1)
		{
			this.tracer.TraceError<T0, T1>(this.traceContext, message, arg0, arg1);
		}

		public void TraceError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
		{
			this.tracer.TraceError<T0, T1, T2>(this.traceContext, message, arg0, arg1, arg2);
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			return this.tracer.IsTraceEnabled(traceType);
		}

		public void SetCounterRawValue(IExPerformanceCounter counter, long value)
		{
			Util.ThrowOnNullArgument(counter, "counter");
			counter.RawValue = value;
		}

		public void DecrementCounter(IExPerformanceCounter counter)
		{
			Util.ThrowOnNullArgument(counter, "counter");
			counter.Decrement();
		}

		public void IncrementCounter(IExPerformanceCounter counter)
		{
			Util.ThrowOnNullArgument(counter, "counter");
			counter.Increment();
		}

		public void IncrementCounterBy(IExPerformanceCounter counter, long incrementValue)
		{
			Util.ThrowOnNullArgument(counter, "counter");
			counter.IncrementBy(incrementValue);
		}

		public void SendInformationalWatsonReport(Exception exception, string additionalDetails)
		{
			Util.ThrowOnNullArgument(exception, "exception");
			StackTrace stackTrace = new StackTrace(1);
			string name = exception.GetType().Name;
			MethodBase method = stackTrace.GetFrame(0).GetMethod();
			AssemblyName name2 = method.DeclaringType.Assembly.GetName();
			int hashCode = (method.Name + name).GetHashCode();
			string detailedExceptionInformation = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				additionalDetails ?? string.Empty,
				Environment.NewLine,
				exception.ToString(),
				Environment.NewLine,
				this.GetExtendedLoggingInformation()
			});
			ExWatson.SendGenericWatsonReport("E12", DiagnosticsSession.AppVersion, DiagnosticsSession.AppName, name2.Version.ToString(), name2.Name, name, stackTrace.ToString(), hashCode.ToString("x"), method.Name, detailedExceptionInformation);
		}

		public void SendWatsonReport(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in this.GetExtendedLoggingInformation())
			{
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.AppendLine(value);
				}
			}
			DiagnosticsSession.SendWatsonReport(exception, stringBuilder.ToString(), DiagnosticsSession.CrashOnUnhandledException);
		}

		public void SetDefaults(Guid eventLogComponentGuid, string serviceName, string logTypeName, string logFilePath, string logFilePrefix, string logComponent)
		{
			this.logDefaults = new DiagnosticsLogConfig.LogDefaults(eventLogComponentGuid, serviceName, logTypeName, logFilePath, logFilePrefix, logComponent);
		}

		private static void SendWatsonReport(Exception exception, string extraData, bool terminateProccess)
		{
			ReportOptions reportOptions = ReportOptions.DoNotFreezeThreads;
			try
			{
				if (terminateProccess)
				{
					reportOptions |= ReportOptions.ReportTerminateAfterSend;
					ExWatson.SendReport(exception, reportOptions, extraData);
				}
				else
				{
					ExWatson.SendReportAndCrashOnAnotherThread(exception, reportOptions, null, extraData);
				}
			}
			finally
			{
				if (terminateProccess)
				{
					try
					{
						using (Process currentProcess = Process.GetCurrentProcess())
						{
							currentProcess.Kill();
						}
					}
					catch (Win32Exception)
					{
					}
					Environment.Exit(-559034355);
				}
			}
		}

		private void LogLine(DiagnosticsLog log, DiagnosticsLoggingTag tag, params object[] data)
		{
			string text = log.Append(data);
			if (text != string.Empty)
			{
				if (tag != DiagnosticsLoggingTag.Informational && tag == DiagnosticsLoggingTag.Failures)
				{
					this.TraceError(text, new object[0]);
					return;
				}
				this.TraceDebug(text, new object[0]);
			}
		}

		internal const int DiagnosticsSessionSchemaVersion = 1;

		private const int CrawlerSchemaVersion = 3;

		private const int GracefulDegradationSchemaVersion = 1;

		private const int DictionaryLogSchemaVersion = 1;

		private static string[] schemaColumns = new string[]
		{
			"date-time",
			"type",
			"version",
			"component",
			"entryId",
			"operation",
			"operation-specific"
		};

		private static string[] crawlerSchemaColumns = new string[]
		{
			"date-time",
			"component",
			"version",
			"operationId",
			"databaseName",
			"mailboxGuid",
			"operation"
		};

		private static string[] gracefulDegradationSchemaColumns = new string[]
		{
			"date-time",
			"component",
			"version",
			"totalmemory",
			"availablememory",
			"memoryusage",
			"expectedmemoryusage",
			"searchmemorydriftratio",
			"operation"
		};

		private static string[] dictionarySchemaColumns = new string[]
		{
			"date-time",
			"component",
			"version",
			"operationId",
			"correlationId",
			"databaseGuid",
			"mailboxGuid",
			"operation"
		};

		private static Hookable<IDiagnosticsSessionFactory> hookableDiagnosticsSessionFactory = Hookable<IDiagnosticsSessionFactory>.Create(true, new DiagnosticsSessionFactory());

		private static ExEventLog eventLog;

		private static string appName;

		private static string appVersion;

		private static bool crashOnUnhandledException = true;

		private readonly string eventLogSourceName;

		private string componentName;

		private IIdentity documentId;

		private Microsoft.Exchange.Diagnostics.Trace tracer;

		private long traceContext;

		private DiagnosticsLog diagnosticsLog;

		private DiagnosticsLog crawlerLog;

		private DiagnosticsLog gracefulDegradationLog;

		private DiagnosticsLog dictionaryLog;

		private DiagnosticsLogConfig.LogDefaults logDefaults;

		private DiagnosticsLogConfig.LogDefaults crawlerLogDefaults;

		private DiagnosticsLogConfig.LogDefaults gracefulDegradationLogDefaults;

		private DiagnosticsLogConfig.LogDefaults dictionaryLogDefaults;
	}
}
