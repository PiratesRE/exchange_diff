using System;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Core
{
	public class PingDetectionModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
			context.EndRequest += this.OnEndRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			this.ReviseAction("OnEndRequest");
		}

		private void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			this.ReviseAction("OnPreSendRequestHeaders");
		}

		private void ReviseAction(string funcName)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction<string>((long)this.GetHashCode(), "[PingDetectionModule::{0}] Enter", funcName);
			HttpContext httpContext = HttpContext.Current;
			HttpResponse response = httpContext.Response;
			if (response == null)
			{
				return;
			}
			if (httpContext.Items["ActionHasBeenRevised"] != null)
			{
				return;
			}
			httpContext.Items["ActionHasBeenRevised"] = "Y";
			WinRMInfo winRMInfoFromHttpHeaders = WinRMInfo.GetWinRMInfoFromHttpHeaders(httpContext.Request.Headers);
			string text;
			if (WinRMRequestTracker.TryReviseAction(winRMInfoFromHttpHeaders, response.StatusCode, response.SubStatusCode, out text))
			{
				HttpLogger.SafeSetLogger(RpsHttpMetadata.Action, text);
				string headerValue = "Ping".Equals(text) ? "Ping" : "Non-Ping";
				this.SafeSetResponseHeader(response, "X-RemotePS-Ping", headerValue);
				this.SafeSetResponseHeader(response, "X-RemotePS-RevisedAction", text);
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction<string>((long)this.GetHashCode(), "[PingDetectionModule::{0}] Exit.", funcName);
		}

		private void SafeSetResponseHeader(HttpResponse response, string header, string headerValue)
		{
			try
			{
				response.Headers[header] = headerValue;
			}
			catch (Exception ex)
			{
				HttpLogger.SafeAppendGenericError("PingDetctionModule.SafeSetResponseHeader", ex.ToString(), false);
			}
		}

		private const string ActionHasBeenRevisedItemKey = "ActionHasBeenRevised";
	}
}
