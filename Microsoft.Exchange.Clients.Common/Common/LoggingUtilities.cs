using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	public static class LoggingUtilities
	{
		public static bool LogEvent(ExEventLog.EventTuple tuple, params object[] eventLogParams)
		{
			return LoggingUtilities.logger.LogEvent(tuple, string.Empty, eventLogParams);
		}

		public static void SendWatson(Exception exception)
		{
			bool flag = true;
			if (!bool.TryParse(ConfigurationManager.AppSettings["SendWatsonReport"], out flag))
			{
				flag = true;
			}
			if (flag)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Sending Watson report");
				ExWatson.SendReport(exception, ReportOptions.None, null);
			}
		}

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.CoreTracer.Category, "MSExchange OWA");
	}
}
