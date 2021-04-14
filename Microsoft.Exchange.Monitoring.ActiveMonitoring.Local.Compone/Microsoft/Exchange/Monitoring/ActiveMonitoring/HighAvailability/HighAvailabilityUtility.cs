using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.Extensions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal static class HighAvailabilityUtility
	{
		public static IRegistryReader RegReader
		{
			get
			{
				return CachedRegistryReader.Instance;
			}
		}

		public static IRegistryReader NonCachedRegReader
		{
			get
			{
				return RegistryReader.Instance;
			}
		}

		public static IRegistryWriter RegWriter
		{
			get
			{
				return RegistryWriter.Instance;
			}
		}

		public static string ConstructProbeName(string mask, string categoryName)
		{
			return string.Format("{0}/{1}", mask, categoryName);
		}

		public static void LogEvent(string eventLogName, string eventSource, long eventId, int categoryId, EventLogEntryType eventType, object[] eventData)
		{
			using (EventLog eventLog = new EventLog(eventLogName))
			{
				eventLog.Source = eventSource;
				EventInstance instance = new EventInstance(eventId, categoryId, eventType);
				eventLog.WriteEvent(instance, eventData);
			}
		}

		public static bool CheckCancellationRequested(CancellationToken token)
		{
			bool result = false;
			try
			{
				result = token.IsCancellationRequested;
			}
			catch
			{
				result = false;
			}
			return result;
		}
	}
}
