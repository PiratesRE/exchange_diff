using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.DiagnosticsModules
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

		internal static void LogEvent(ExEventLog eventLogger, ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
			if (eventLogger == null)
			{
				return;
			}
			if (messageArguments == null)
			{
				throw new ArgumentNullException("messageArguments");
			}
			object[] array = new object[messageArguments.Length + 2];
			array[0] = Logger.processName;
			array[1] = Logger.processId;
			messageArguments.CopyTo(array, 2);
			eventLogger.LogEvent(eventInfo, periodicKey, array);
		}

		private static readonly string appDomainName = AppDomain.CurrentDomain.FriendlyName;

		private static readonly string appDomainTraceInfo = " AppDomain:" + Logger.appDomainName + ".";

		private static readonly string traceFunctionEnterString = "Enter Function: {0}." + Logger.appDomainTraceInfo;

		private static readonly string traceFunctionExitString = "Exit Function: {0}." + Logger.appDomainTraceInfo;

		private static string processName;

		private static int processId;
	}
}
