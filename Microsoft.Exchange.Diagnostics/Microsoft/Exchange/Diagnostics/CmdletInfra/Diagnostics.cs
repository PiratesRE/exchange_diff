using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class Diagnostics
	{
		static Diagnostics()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				Diagnostics.processName = currentProcess.MainModule.ModuleName;
				Diagnostics.processId = currentProcess.Id;
			}
		}

		internal static string GetGenericErrorKey(string key, bool isUnhandledException)
		{
			if (isUnhandledException)
			{
				return key + "_UnhandledException";
			}
			return key;
		}

		internal static void ReportException(Exception exception, ExEventLog exEventLog, ExEventLog.EventTuple eventTuple, ExWatson.IsExceptionInteresting knownExceptions, object eventObject, Trace trace, string traceFormat)
		{
			Diagnostics.LogExceptionWithTrace(exEventLog, eventTuple, null, trace, eventObject, traceFormat, exception);
			if (Diagnostics.IsSendReportValid(exception, knownExceptions))
			{
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, false), ReportOptions.None);
				ExWatson.SetWatsonReportAlreadySent(exception);
			}
		}

		internal static bool LogExceptionWithTrace(ExEventLog exEventLog, ExEventLog.EventTuple tuple, string periodicKey, Trace tagTracer, object thisObject, string traceFormat, Exception exception)
		{
			tagTracer.TraceError<Exception>((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), traceFormat, exception);
			string text = string.Format(traceFormat, exception.ToString());
			if (text.Length > 32000)
			{
				text = text.Substring(0, 2000) + "...\n" + text.Substring(text.Length - 20000, 20000);
			}
			return exEventLog.LogEvent(tuple, periodicKey, new object[]
			{
				Diagnostics.processId,
				Diagnostics.processName,
				text
			});
		}

		internal static void ExecuteAndLog(string funcName, bool missionCritical, LatencyTracker latencyTracker, ExEventLog eventLog, ExEventLog.EventTuple eventTuple, Trace tracer, ExWatson.IsExceptionInteresting isExceptionInteresting, Action<Exception> onError, Action action)
		{
			Diagnostics.ExecuteAndLog<bool>(funcName, missionCritical, latencyTracker, eventLog, eventTuple, tracer, isExceptionInteresting, onError, true, delegate()
			{
				action();
				return true;
			});
		}

		internal static T ExecuteAndLog<T>(string funcName, bool missionCritical, LatencyTracker latencyTracker, ExEventLog eventLog, ExEventLog.EventTuple eventTuple, Trace tracer, ExWatson.IsExceptionInteresting isExceptionInteresting, Action<Exception> onError, T defaultReturnValue, Func<T> func)
		{
			bool flag = false;
			T result;
			try
			{
				tracer.TraceDebug<Func<T>>(0L, "[{0}] Enter.", func);
				if (latencyTracker != null)
				{
					flag = latencyTracker.StartInternalTracking(funcName);
				}
				result = func();
			}
			catch (Exception ex)
			{
				if (onError != null)
				{
					onError(ex);
				}
				Diagnostics.ReportException(ex, eventLog, eventTuple, isExceptionInteresting, null, tracer, string.Format("Func {0} throws Exception {{0}}.", funcName));
				if (missionCritical)
				{
					throw;
				}
				result = defaultReturnValue;
			}
			finally
			{
				if (flag)
				{
					latencyTracker.EndInternalTracking(funcName);
				}
				tracer.TraceDebug<Func<T>>(0L, "[{0}] Exit.", func);
			}
			return result;
		}

		private static bool IsSendReportValid(Exception exception, ExWatson.IsExceptionInteresting isExceptionInteresting)
		{
			if (ExWatson.IsWatsonReportAlreadySent(exception))
			{
				return false;
			}
			bool flag = isExceptionInteresting == null || isExceptionInteresting(exception);
			ExTraceGlobals.InstrumentationTracer.TraceDebug<bool>(0L, "IsSendReportValid isSendReportValid: {0}", flag);
			return flag;
		}

		internal const string UnHandledErrorType = "UnHandled";

		internal const string UnhandledExceptionSuffix = "_UnhandledException";

		private static readonly string processName;

		private static readonly int processId;
	}
}
