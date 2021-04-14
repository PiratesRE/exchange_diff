using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal interface IDiagnosticsSession
	{
		string ComponentName { set; }

		Trace Tracer { get; set; }

		ExEventLog EventLog { get; }

		string[] GetExtendedLoggingInformation();

		void Assert(bool condition, string message, params object[] messageArgs);

		void LogDiagnosticsInfo(DiagnosticsLoggingTag loggingTag, string operation, params object[] operationSpecificData);

		void LogDiagnosticsInfo(DiagnosticsLoggingTag loggingTag, IIdentity documentId, string operation, params object[] operationSpecificData);

		void LogCrawlerInfo(DiagnosticsLoggingTag loggingTag, string operationId, string databaseName, string mailboxGuid, string operation, params object[] operationSpecificData);

		void LogGracefulDegradationInfo(DiagnosticsLoggingTag loggingTag, long totalMemory, long availableMemory, long actualMemoryUsage, long expectedMemoryUsage, float searchMemoryDriftRatio, string operation, params object[] operationSpecificData);

		void LogDictionaryInfo(DiagnosticsLoggingTag loggingTag, int operationId, Guid correlationId, Guid database, Guid mailbox, string operation, params object[] operationSpecificData);

		void LogEvent(ExEventLog.EventTuple eventId, params object[] messageArgs);

		void LogPeriodicEvent(ExEventLog.EventTuple eventId, string periodicLogName, params object[] messageArgs);

		void TraceDebug(string message, params object[] messageArgs);

		void TraceDebug<T0>(string message, T0 arg0);

		void TraceDebug<T0, T1>(string message, T0 arg0, T1 arg1);

		void TraceDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

		void TraceError(string message, params object[] messageArgs);

		void TraceError<T0>(string message, T0 arg0);

		void TraceError<T0, T1>(string message, T0 arg0, T1 arg1);

		void TraceError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

		bool IsTraceEnabled(TraceType traceType);

		void SetCounterRawValue(IExPerformanceCounter counter, long value);

		void DecrementCounter(IExPerformanceCounter counter);

		void IncrementCounter(IExPerformanceCounter counter);

		void IncrementCounterBy(IExPerformanceCounter counter, long incrementValue);

		void SendInformationalWatsonReport(Exception exception, string additionalDetails);

		void SendWatsonReport(Exception exception);

		void SetDefaults(Guid eventLogComponentGuid, string serviceName, string logTypeName, string logFilePath, string logFilePrefix, string logComponent);
	}
}
