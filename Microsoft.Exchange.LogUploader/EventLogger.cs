using System;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class EventLogger : IEventLogger
	{
		private EventLogger()
		{
		}

		private EventLogger(Guid eventLogAppGuid, string serviceName)
		{
			if (eventLogAppGuid == Guid.Empty)
			{
				throw new ArgumentException("EventLogger is not configured.", "eventLogAppGuid");
			}
			if (string.IsNullOrWhiteSpace(serviceName))
			{
				throw new ArgumentException("EventLogger is not configured.", "serviceName");
			}
			this.eventLog = new ExEventLog(eventLogAppGuid, serviceName);
		}

		internal static IEventLogger Logger
		{
			get
			{
				if (EventLogger.instance == null)
				{
					EventLogger.instance = new EventLogger();
				}
				return EventLogger.instance;
			}
		}

		public static void Configure(Guid eventLogAppGuid, string eventSource)
		{
			lock (EventLogger.instanceMutex)
			{
				if (EventLogger.instance != null && EventLogger.instance.eventLog != null && EventLogger.eventSource != eventSource)
				{
					throw new InvalidOperationException("EventLogger is configured for a different event source already.");
				}
				EventLogger.instance = new EventLogger(eventLogAppGuid, eventSource);
				EventLogger.eventSource = eventSource;
			}
		}

		public void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			if (this.eventLog != null)
			{
				this.eventLog.LogEvent(tuple, periodicKey, messageArgs);
			}
		}

		private static readonly object instanceMutex = new object();

		private static EventLogger instance;

		private static string eventSource;

		private readonly ExEventLog eventLog;
	}
}
