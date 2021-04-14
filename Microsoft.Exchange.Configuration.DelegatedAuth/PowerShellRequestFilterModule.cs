using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.DelegatedAuthentication;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	public class PowerShellRequestFilterModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.BeginRequest += PowerShellRequestFilterModule.OnBeginRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.DelegatedAuthTracer.TraceFunction<string>(0L, "Enter Function: {0}.", "OnBeginRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			PowerShellRequestFilterModule.AddHeadersForDelegation(request);
			ExTraceGlobals.DelegatedAuthTracer.TraceFunction<string>(0L, "Exit Function: {0}.", "OnBeginRequest");
		}

		private static void AddHeadersForDelegation(HttpRequest request)
		{
			string targetOrgName = PowerShellRequestFilterModule.GetTargetOrgName(request);
			if (targetOrgName != null)
			{
				request.Headers.Add("msExchTargetTenant", targetOrgName);
			}
			request.Headers.Add("msExchOriginalUrl", request.Url.AbsoluteUri);
		}

		private static string GetTargetOrgName(HttpRequest request)
		{
			UriBuilder uriBuilder = new UriBuilder(request.Url);
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query.Replace(';', '&'));
			return nameValueCollection["DelegatedOrg"];
		}
	}
}
