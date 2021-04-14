using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal class Logger
	{
		static Logger()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				Logger.processName = currentProcess.MainModule.ModuleName;
				Logger.processId = currentProcess.Id;
			}
			Logger.eventLogger = new ExEventLog(ExTraceGlobals.FailFastCacheTracer.Category, "MSExchange FailFast Module");
			Logger.eventLogger.SetEventPeriod(300);
		}

		internal static void EnterFunction(Microsoft.Exchange.Diagnostics.Trace trace, string functionName)
		{
			trace.TraceFunction<string>(0L, Logger.traceFunctionEnterString, functionName);
		}

		internal static void ExitFunction(Microsoft.Exchange.Diagnostics.Trace trace, string functionName)
		{
			trace.TraceFunction<string>(0L, Logger.traceFunctionExitString, functionName);
		}

		internal static void TraceInformation(Microsoft.Exchange.Diagnostics.Trace trace, string formatString, params object[] args)
		{
			trace.Information(0L, formatString + Logger.appDomainTraceInfo, args);
		}

		internal static void TraceDebug(Microsoft.Exchange.Diagnostics.Trace trace, string formatString, params object[] args)
		{
			trace.TraceDebug(0L, formatString + Logger.appDomainTraceInfo, args);
		}

		internal static void TraceError(Microsoft.Exchange.Diagnostics.Trace trace, string formatString, params object[] args)
		{
			trace.TraceError(0L, formatString + Logger.appDomainTraceInfo, args);
		}

		internal static void LogEvent(ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 3];
			array[0] = Logger.processName;
			array[1] = Logger.processId;
			array[2] = Logger.appDomainName;
			messageArguments.CopyTo(array, 3);
			Logger.eventLogger.LogEvent(eventInfo, periodicKey, array);
		}

		private static readonly string appDomainName = AppDomain.CurrentDomain.FriendlyName;

		private static readonly string appDomainTraceInfo = " AppDomain:" + Logger.appDomainName + ".";

		private static readonly string traceFunctionEnterString = "Enter Function: {0}." + Logger.appDomainTraceInfo;

		private static readonly string traceFunctionExitString = "Exit Function: {0}." + Logger.appDomainTraceInfo;

		private static readonly ExEventLog eventLogger;

		private static string processName;

		private static int processId;
	}
}
