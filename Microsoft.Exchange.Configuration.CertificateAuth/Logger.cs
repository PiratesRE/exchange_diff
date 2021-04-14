using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.CertificateAuthentication;

namespace Microsoft.Exchange.Configuration.CertificateAuthentication
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

		internal static void LogVerbose(string message, params object[] args)
		{
			ExTraceGlobals.CertAuthTracer.Information(0L, message, args);
		}

		internal static void LogVerbose(string message)
		{
			Logger.LogVerbose(message, new object[0]);
		}

		internal static void LogError(string message, Exception exception)
		{
			ExTraceGlobals.CertAuthTracer.TraceError<string, Exception>(0L, "{0} - {1}", message, exception);
		}

		internal static void LogEvent(ExEventLog eventLogger, ExEventLog.EventTuple eventInfo, string periodicKey, params object[] messageArguments)
		{
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

		private static string processName;

		private static int processId;
	}
}
