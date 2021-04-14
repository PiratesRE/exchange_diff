using System;
using System.Diagnostics;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RedirectionModule;

namespace Microsoft.Exchange.Configuration.RedirectionModule
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

		internal static void LogVerbose(TraceSource traceSource, string message, params object[] args)
		{
			traceSource.TraceEvent(TraceEventType.Verbose, 0, message, args);
			ExTraceGlobals.RedirectionTracer.Information(0L, message, args);
		}

		internal static void LogVerbose(TraceSource traceSource, string message)
		{
			Logger.LogVerbose(traceSource, message, new object[0]);
		}

		internal static void LogWarning(TraceSource traceSource, string message)
		{
			traceSource.TraceEvent(TraceEventType.Warning, 0, message);
			ExTraceGlobals.RedirectionTracer.TraceWarning(0L, message);
		}

		internal static void LogError(ExEventLog eventLogger, TraceSource traceSource, string message, Exception exception)
		{
			Logger.LogError(eventLogger, traceSource, message, exception, null, null);
		}

		internal static void LogError(ExEventLog eventLogger, TraceSource traceSource, string message, Exception exception, ExEventLog.EventTuple? eventInfo, string user)
		{
			traceSource.TraceEvent(TraceEventType.Error, 0, "{0} - {1}", new object[]
			{
				message,
				exception
			});
			ExTraceGlobals.RedirectionTracer.TraceError<string, Exception>(0L, "{0} - {1}", message, exception);
			if (eventInfo != null)
			{
				Logger.LogEvent(eventLogger, eventInfo.Value, user, new object[]
				{
					user,
					exception
				});
			}
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

		internal static void GenerateErrorMessage(HttpResponse response, ExEventLog eventLogger, ExEventLog.EventTuple tuple, Exception error, string tenant)
		{
			Logger.LogEvent(eventLogger, tuple, null, new object[]
			{
				error,
				tenant
			});
			response.Clear();
			response.StatusCode = 500;
			response.ContentType = "application/soap+xml;charset=UTF-8";
			response.Write(error.Message);
			response.End();
		}

		internal static void EnterFunction(Microsoft.Exchange.Diagnostics.Trace trace, string functionName)
		{
			trace.TraceFunction<string>(0L, Logger.traceFunctionEnterString, functionName);
		}

		internal static void ExitFunction(Microsoft.Exchange.Diagnostics.Trace trace, string functionName)
		{
			trace.TraceFunction<string>(0L, Logger.traceFunctionExitString, functionName);
		}

		private static readonly string appDomainName = AppDomain.CurrentDomain.FriendlyName;

		private static readonly string appDomainTraceInfo = " AppDomain:" + Logger.appDomainName + ".";

		private static readonly string traceFunctionEnterString = "Enter Function: {0}." + Logger.appDomainTraceInfo;

		private static readonly string traceFunctionExitString = "Exit Function: {0}." + Logger.appDomainTraceInfo;

		private static string processName;

		private static int processId;
	}
}
