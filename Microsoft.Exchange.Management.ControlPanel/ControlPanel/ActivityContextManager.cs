using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ActivityContextManager
	{
		public static void InitializeActivityContext(HttpContext httpContext, ActivityContextLoggerId eventId = ActivityContextLoggerId.Request)
		{
			try
			{
				if (httpContext != null && !ActivityContext.IsStarted)
				{
					ActivityScope activityScope = ActivityContext.DeserializeFrom(httpContext.Request, null);
					activityScope.SetProperty(ExtensibleLoggerMetadata.EventId, eventId.ToString());
					if (activityScope.DisposeTracker is DisposeTrackerObject<ActivityScope>)
					{
						activityScope.DisposeTracker.AddExtraData(httpContext.GetRequestUrl().ToString());
					}
					httpContext.Items[ActivityContextManager.ECPActivityScopePropertyName] = activityScope;
				}
			}
			catch (Exception exception)
			{
				EcpEventLogConstants.Tuple_ActivityContextError.LogPeriodicFailure(EcpEventLogExtensions.GetUserNameToLog(), httpContext.GetRequestUrlForLog(), exception, EcpEventLogExtensions.GetFlightInfoForLog());
			}
		}

		public static void CleanupActivityContext(HttpContext httpContext)
		{
			try
			{
				if (httpContext != null)
				{
					ActivityScope activityScope = (ActivityScope)httpContext.Items[ActivityContextManager.ECPActivityScopePropertyName];
					if (activityScope != null)
					{
						httpContext.Items.Remove(ActivityContextManager.ECPActivityScopePropertyName);
						if (!activityScope.IsDisposed)
						{
							activityScope.End();
						}
					}
				}
			}
			catch (Exception exception)
			{
				EcpEventLogConstants.Tuple_ActivityContextError.LogPeriodicFailure(EcpEventLogExtensions.GetUserNameToLog(), httpContext.GetRequestUrlForLog(), exception, EcpEventLogExtensions.GetFlightInfoForLog());
			}
		}

		private static string ECPActivityScopePropertyName = "ECPActivityScope";
	}
}
