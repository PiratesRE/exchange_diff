using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class DiagnosticsModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
			application.EndRequest += this.OnEndRequest;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			if (Utility.IsResourceRequest(httpContext.GetRequestUrl().LocalPath))
			{
				return;
			}
			ActivityContextManager.InitializeActivityContext(httpContext, ActivityContextLoggerId.Request);
			string str = ActivityContext.ActivityId.FormatForLog();
			if (HttpContext.Current.Request.QueryString.ToString().Length > 0)
			{
				HttpContext.Current.Response.AppendToLog("&ActID=" + str);
				return;
			}
			HttpContext.Current.Response.AppendToLog("ActID=" + str);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			if (Utility.IsResourceRequest(httpContext.GetRequestUrl().LocalPath))
			{
				return;
			}
			ActivityContextManager.CleanupActivityContext(httpContext);
		}
	}
}
