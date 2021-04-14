using System;
using System.Diagnostics;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class CoreLogger
	{
		static CoreLogger()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				CoreLogger.processName = currentProcess.MainModule.ModuleName;
				CoreLogger.processId = currentProcess.Id;
			}
		}

		internal static void ExecuteAndLog(string funcName, bool missionCritical, LatencyTracker latencyTracker, Action<Exception> onError, Action action)
		{
			Diagnostics.ExecuteAndLog(funcName, missionCritical, latencyTracker, Constants.CoreEventLogger, TaskEventLogConstants.Tuple_NonCrashingException, ExTraceGlobals.InstrumentationTracer, (object ex) => false, onError, action);
		}

		internal static T ExecuteAndLog<T>(string funcName, bool missionCritical, LatencyTracker latencyTracker, Action<Exception> onError, T defaultReturnValue, Func<T> func)
		{
			return Diagnostics.ExecuteAndLog<T>(funcName, missionCritical, latencyTracker, Constants.CoreEventLogger, TaskEventLogConstants.Tuple_NonCrashingException, ExTraceGlobals.InstrumentationTracer, (object ex) => false, onError, defaultReturnValue, func);
		}

		internal static void TraceInformation(string formatString, params object[] args)
		{
			ExTraceGlobals.InstrumentationTracer.Information(0L, formatString + CoreLogger.appDomainTraceInfo, args);
		}

		internal static void TraceDebug(string formatString, params object[] args)
		{
			ExTraceGlobals.InstrumentationTracer.TraceDebug(0L, formatString + CoreLogger.appDomainTraceInfo, args);
		}

		internal static void TraceError(string formatString, params object[] args)
		{
			ExTraceGlobals.InstrumentationTracer.TraceError(0L, formatString + CoreLogger.appDomainTraceInfo, args);
		}

		internal static void LogEvent(ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 3];
			array[0] = CoreLogger.processName;
			array[1] = CoreLogger.processId;
			array[2] = CoreLogger.appDomainName;
			messageArguments.CopyTo(array, 3);
			Constants.CoreEventLogger.LogEvent(eventInfo, periodicKey, array);
		}

		private static readonly string appDomainName = AppDomain.CurrentDomain.FriendlyName;

		private static readonly string appDomainTraceInfo = " AppDomain:" + CoreLogger.appDomainName + ".";

		private static readonly string processName;

		private static readonly int processId;
	}
}
