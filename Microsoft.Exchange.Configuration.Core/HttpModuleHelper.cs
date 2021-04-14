using System;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	public static class HttpModuleHelper
	{
		internal static IScopedPerformanceMonitor[] HttpPerfMonitors
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext == null)
				{
					return HttpModuleHelper.EmptyMonitors;
				}
				LatencyTracker latencyTracker = httpContext.Items["Logging-HttpRequest-Latency"] as LatencyTracker;
				if (latencyTracker == null)
				{
					return HttpModuleHelper.EmptyMonitors;
				}
				return new IScopedPerformanceMonitor[]
				{
					new LatencyMonitor(latencyTracker)
				};
			}
		}

		internal static UserToken CurrentUserToken(this HttpContext httpContext)
		{
			return HttpContext.Current.Items["X-EX-UserToken"] as UserToken;
		}

		internal static void EndPowerShellRequestWithFriendlyError(HttpContext context, FailureCategory failureCategory, string failureName, string errorMessage, string className, bool isCriticalError)
		{
			context.Response.StatusCode = 400;
			string text = string.Format("[FailureCategory={0}] ", failureCategory + "-" + failureName) + errorMessage;
			HttpLogger.SafeAppendGenericError(className, text, isCriticalError);
			ExTraceGlobals.HttpModuleTracer.TraceError<string>(0L, className + " Get error. {0}", text);
			context.Response.Write(text);
			context.Response.End();
		}

		public const string ExtendedStatusContextKeyName = "ExtendedStatus";

		private static readonly IScopedPerformanceMonitor[] EmptyMonitors = new IScopedPerformanceMonitor[0];
	}
}
