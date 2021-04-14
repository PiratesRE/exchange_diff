using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class EcpEventLogExtensions
	{
		public static ExEventLog EventLog
		{
			get
			{
				return EcpEventLogExtensions.eventLog;
			}
		}

		public static bool IsEnabled(this ExEventLog.EventTuple tuple)
		{
			return EcpEventLogExtensions.eventLog.IsEventCategoryEnabled(tuple.CategoryId, tuple.Level);
		}

		public static void LogEvent(this ExEventLog.EventTuple tuple, params object[] messageArgs)
		{
			EcpEventLogExtensions.InternalLogEvent(tuple, null, messageArgs);
		}

		public static void LogPeriodicEvent(this ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			EcpEventLogExtensions.InternalLogEvent(tuple, periodicKey, messageArgs);
		}

		public static void LogPeriodicFailure(this ExEventLog.EventTuple tuple, string userName, string requestUrl, Exception exception, string featureInfo)
		{
			if (tuple.IsEnabled())
			{
				using (new ThreadCultureSwitch())
				{
					HttpContext httpContext = HttpContext.Current;
					if (string.IsNullOrEmpty(userName))
					{
						userName = ((httpContext != null && httpContext.User != null && httpContext.User.Identity != null) ? httpContext.User.Identity.GetSafeName(true) : string.Empty);
					}
					string periodicKey = requestUrl + LocalizedException.GenerateErrorCode(exception) + userName;
					tuple.LogPeriodicEvent(periodicKey, new object[]
					{
						userName,
						requestUrl,
						exception.GetTraceFormatter().ToString(),
						featureInfo
					});
				}
			}
		}

		internal static string GetUserNameToLog()
		{
			string result = Strings.UserUnauthenticated;
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			if (current != null)
			{
				result = current.NameForEventLog;
			}
			else if (HttpContext.Current.Request.IsAuthenticated)
			{
				result = HttpContext.Current.User.Identity.GetSafeName(true);
			}
			else if (HttpContext.Current.User == null)
			{
				result = Strings.UserNotSet;
			}
			return result;
		}

		internal static string GetFlightInfoForLog()
		{
			string result;
			try
			{
				bool flag;
				VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser(out flag);
				Dictionary<string, bool> allEacRelatedFeatures = EacFlightUtility.GetAllEacRelatedFeatures(snapshotForCurrentUser);
				string[] flights = snapshotForCurrentUser.Flights;
				KeyValuePair<string, string>[] constraints = snapshotForCurrentUser.Constraints;
				result = string.Format("Features:{0},  Flights:[{1}],  Constraints:{2}, IsGlobalSnapshot: {3}", new object[]
				{
					allEacRelatedFeatures.ToLogString<string, bool>(),
					(flights == null) ? "" : string.Join(",", flights),
					constraints.ToLogString<string, string>(),
					flag.ToString()
				});
			}
			catch (Exception)
			{
				result = "Feature status not available, Flight APIs is in Invalid Status";
			}
			return result;
		}

		internal static string GetPeriodicKeyPerUser()
		{
			RbacPrincipal current = RbacPrincipal.GetCurrent(false);
			if (current != null)
			{
				return current.NameForEventLog;
			}
			if (HttpContext.Current.Request.IsAuthenticated)
			{
				return HttpContext.Current.User.Identity.GetSafeName(true);
			}
			return null;
		}

		private static void InternalLogEvent(ExEventLog.EventTuple tuple, string periodicKey, object[] messageArgs)
		{
			int num = messageArgs.Length;
			object[] array = new object[num + 1];
			Array.Copy(messageArgs, array, num);
			array[num] = "ActivityId: " + ActivityContext.ActivityId.FormatForLog();
			EcpEventLogExtensions.eventLog.LogEvent(tuple, periodicKey, array);
		}

		private const string EventSource = "MSExchange Control Panel";

		private static ExEventLog eventLog = new ExEventLog(ExTraceGlobals.EventLogTracer.Category, "MSExchange Control Panel");
	}
}
