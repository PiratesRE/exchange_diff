using System;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public static class Logger
	{
		public static ILog Log
		{
			get
			{
				return Microsoft.ExLogAnalyzer.Log.Instance;
			}
			set
			{
				Microsoft.ExLogAnalyzer.Log.Instance = value;
			}
		}

		public static void Cleanup()
		{
			IDisposable disposable = Logger.Log as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			Logger.Log = null;
		}

		public static bool LogEvent(ExEventLog.EventTuple tuple, params object[] args)
		{
			EventLogger eventLogger = TriggerHandler.Instance as EventLogger;
			if (eventLogger != null)
			{
				return eventLogger.LogEvent(tuple, args);
			}
			Logger.Log.LogErrorMessage("Unable to log event, event log source not configured.", new object[0]);
			return false;
		}

		public static void LogErrorMessage(string format, params object[] args)
		{
			Logger.Log.LogErrorMessage(format, args);
		}

		public static void LogWarningMessage(string format, params object[] args)
		{
			Logger.Log.LogWarningMessage(format, args);
		}

		public static void LogInformationMessage(string format, params object[] args)
		{
			Logger.Log.LogInformationMessage(format, args);
		}
	}
}
