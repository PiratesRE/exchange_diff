using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal static class ExManagementApplicationLogger
	{
		public static void LogEvent(ExEventLog.EventTuple eventInfo, params string[] messageArguments)
		{
			ExManagementApplicationLogger.eventLogger.LogEvent(eventInfo, null, messageArguments);
			ExTraceGlobals.EventTracer.Information<string[]>(0L, eventInfo.ToString(), messageArguments);
		}

		public static void LogEvent(IOrganizationIdForEventLog organizationId, ExEventLog.EventTuple eventInfo, params string[] messageArguments)
		{
			ExManagementApplicationLogger.eventLogger.LogEvent(organizationId, eventInfo, null, messageArguments);
			ExTraceGlobals.EventTracer.Information<string[]>(0L, eventInfo.ToString(), messageArguments);
		}

		public static bool IsLowEventCategoryEnabled(short eventCategory)
		{
			return ExManagementApplicationLogger.eventLogger.IsEventCategoryEnabled(eventCategory, ExEventLog.EventLevel.Low);
		}

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.LogTracer.Category, "MSExchange Management Application");
	}
}
