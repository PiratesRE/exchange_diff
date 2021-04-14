using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	public class PowerShellBasicAuthNRedirectModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Enter.");
			HttpContext httpContext = HttpContext.Current;
			if (httpContext.Request.IsAuthenticated)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Already authenticated.");
				return;
			}
			string text = httpContext.Request.Headers["Authorization"];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] No Authorization header.");
				return;
			}
			if (!text.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Not Basic AuthN. authHeader = {0}.", text);
				return;
			}
			Uri url = httpContext.Request.Url;
			UriBuilder uriBuilder = new UriBuilder(url);
			if ("PowerShell-LiveID".Equals(uriBuilder.Path, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<Uri>((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Url {0} is already with path PowerShell-LiveID.", url);
				return;
			}
			uriBuilder.Path = "PowerShell-LiveID";
			string text2 = uriBuilder.Uri.ToString();
			ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Redirect to Url {0}.", text2);
			httpContext.Response.Redirect(text2);
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[PowerShellBasicAuthNRedirectModule::OnAuthenticateRequest] Exit.");
		}
	}
}
