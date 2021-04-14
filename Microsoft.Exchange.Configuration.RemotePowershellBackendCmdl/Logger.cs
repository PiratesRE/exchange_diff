using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RemotePowershellBackendCmdletProxy;

namespace Microsoft.Exchange.Configuration.RemotePowershellBackendCmdletProxy
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
			Logger.eventLogger = new ExEventLog(ExTraceGlobals.RemotePowershellBackendCmdletProxyModuleTracer.Category, "MSExchange RemotePowershell BackendCmdletProxy Module");
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
			trace.Information(0L, formatString, args);
		}

		internal static void TraceDebug(Microsoft.Exchange.Diagnostics.Trace trace, string formatString, params object[] args)
		{
			trace.TraceDebug(0L, formatString, args);
		}

		internal static void TraceError(Microsoft.Exchange.Diagnostics.Trace trace, string formatString, params object[] args)
		{
			trace.TraceError(0L, formatString, args);
		}

		internal static void LogEvent(ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 2];
			array[0] = Logger.processName;
			array[1] = Logger.processId;
			messageArguments.CopyTo(array, 2);
			Logger.eventLogger.LogEvent(eventInfo, periodicKey, array);
		}

		private static readonly string traceFunctionEnterString = "Enter Function: {0}.";

		private static readonly string traceFunctionExitString = "Exit Function: {0}.";

		private static readonly ExEventLog eventLogger;

		private static string processName;

		private static int processId;
	}
}
