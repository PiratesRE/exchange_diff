using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal static class EventLogExtension
	{
		public static bool IsEnabled(this ExEventLog.EventTuple tuple)
		{
			return EventLogExtension.EventLog.IsEventCategoryEnabled(tuple.CategoryId, tuple.Level);
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, params object[] messageArgs)
		{
			EventLogExtension.EventLog.LogEvent(tuple, null, messageArgs);
		}

		public static void LogPeriodicEvent(this ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			EventLogExtension.EventLog.LogEvent(tuple, periodicKey, messageArgs);
		}

		internal static string GetPeriodicKeyPerUser()
		{
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			if (current != null)
			{
				return current.NameForEventLog;
			}
			if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.IsAuthenticated)
			{
				return HttpContext.Current.User.Identity.GetSafeName(true);
			}
			return null;
		}

		internal static string GetUserNameToLog()
		{
			string result = Strings.UserUnauthenticated;
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			if (current != null)
			{
				result = current.NameForEventLog;
			}
			else if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.IsAuthenticated)
			{
				result = HttpContext.Current.User.Identity.GetSafeName(true);
			}
			else if (HttpContext.Current.User == null)
			{
				result = Strings.UserNotSet;
			}
			return result;
		}

		private const string EventSource = "MSExchange ReportingWebService";

		public static readonly ExEventLog EventLog = new ExEventLog(ExTraceGlobals.ReportingWebServiceTracer.Category, "MSExchange ReportingWebService");
	}
}
