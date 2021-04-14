using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	public class ProbePingModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.BeginRequest += this.OnBeginRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ProbePingModule::OnBeginRequest] Enter");
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			if (LoggerHelper.IsProbePingRequest(request))
			{
				HttpLogger.SafeSetLogger(RpsHttpMetadata.Action, "ProbePing");
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ProbePingModule::OnBeginRequest] Exit.");
		}
	}
}
