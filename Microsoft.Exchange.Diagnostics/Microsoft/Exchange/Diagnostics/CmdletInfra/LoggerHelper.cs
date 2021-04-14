using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class LoggerHelper
	{
		internal static bool IsPswsNormalRequest
		{
			get
			{
				return LoggerSettings.IsPowerShellWebService && HttpContext.Current != null;
			}
		}

		internal static bool IsPswsCmdletDirectInvoke
		{
			get
			{
				return LoggerSettings.IsPowerShellWebService && HttpContext.Current == null;
			}
		}

		internal static string GetContributeToFailFastValue(string blockedScope, string blockedUserOrTenant, string blockedAction, double blockedTime)
		{
			return string.Format("{0}#{1}#{2}#{3}", new object[]
			{
				blockedScope,
				blockedUserOrTenant,
				blockedAction,
				blockedTime
			});
		}

		internal static void UpdateActivityScopeRequestIdFromUrl(string httpUrl)
		{
			if (string.IsNullOrWhiteSpace(httpUrl))
			{
				return;
			}
			Uri uri = new Uri(httpUrl);
			NameValueCollection urlProperties = LoggerHelper.GetUrlProperties(uri);
			string text = urlProperties["RequestId48CD6591-0506-4D6E-9131-797489A3260F"];
			Guid guid;
			if (text == null || !Guid.TryParse(text, out guid))
			{
				return;
			}
			ActivityScopeImpl activityScopeImpl = ActivityContext.GetCurrentActivityScope() as ActivityScopeImpl;
			if (activityScopeImpl == null)
			{
				return;
			}
			if (activityScopeImpl.ActivityId == guid)
			{
				return;
			}
			ActivityContextState state = new ActivityContextState(new Guid?(guid), LoggerHelper.EmptyConcurrentDic);
			activityScopeImpl.UpdateFromState(state);
		}

		internal static NameValueCollection GetUrlProperties(Uri uri)
		{
			if (uri == null)
			{
				return null;
			}
			UriBuilder uriBuilder = new UriBuilder(uri);
			return HttpUtility.ParseQueryString(uriBuilder.Query.Replace(';', '&'));
		}

		internal static bool IsProbePingRequest(HttpRequest request)
		{
			if (request == null)
			{
				return false;
			}
			Uri url = request.Url;
			NameValueCollection urlProperties = LoggerHelper.GetUrlProperties(url);
			string text = (urlProperties != null) ? urlProperties.Get("ping") : null;
			return !string.IsNullOrEmpty(text) && text.Equals("probe", StringComparison.OrdinalIgnoreCase);
		}

		internal const string RemotePSRequestIdUrlParameterName = "RequestId48CD6591-0506-4D6E-9131-797489A3260F";

		private const string ContributeToFailFastFmt = "{0}#{1}#{2}#{3}";

		internal static IScopedPerformanceMonitor[] CmdletPerfMonitors = new IScopedPerformanceMonitor[]
		{
			CmdletLatencyMonitor.Instance
		};

		private static readonly ConcurrentDictionary<Enum, object> EmptyConcurrentDic = new ConcurrentDictionary<Enum, object>();
	}
}
