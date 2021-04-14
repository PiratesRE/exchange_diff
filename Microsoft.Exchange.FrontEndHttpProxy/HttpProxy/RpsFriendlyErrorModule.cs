using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.HttpProxy
{
	public class RpsFriendlyErrorModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			if (WinRMHelper.FriendlyErrorEnabled.Value)
			{
				context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
				context.EndRequest += this.OnEndRequest;
			}
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpResponse response = httpContext.Response;
			if (response != null)
			{
				RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "OnPreSendRequestHeaders.ContentType", response.ContentType);
			}
			HttpContext.Current.Items["X-HeaderPreSent"] = true;
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpResponse response = httpContext.Response;
			if (response == null)
			{
				return;
			}
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "OnEndRequest.Start.ContentType", response.ContentType);
			if (response.Headers["X-RemotePS-RevisedAction"] != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(current, HttpProxyMetadata.ProtocolAction, response.Headers["X-RemotePS-RevisedAction"]);
			}
			bool flag = httpContext.Items.Contains("X-HeaderPreSent") && (bool)httpContext.Items["X-HeaderPreSent"];
			if (flag)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "FriendlyError", "Skip-HeaderPreSent");
				return;
			}
			try
			{
				int statusCode = response.StatusCode;
				int num;
				if (WinRMHelper.TryConvertStatusCode(statusCode, out num))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<int, int>((long)this.GetHashCode(), "[RpsFriendlyErrorModule::OnEndRequest]: Convert status code from {0} to {1}.", statusCode, num);
					response.StatusCode = num;
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(current, ServiceCommonMetadata.HttpStatus, statusCode);
				}
				if (statusCode >= 400 && !"Ping".Equals(response.Headers["X-RemotePS-Ping"], StringComparison.OrdinalIgnoreCase) && !"Possible-Ping".Equals(response.Headers["X-RemotePS-Ping"], StringComparison.OrdinalIgnoreCase))
				{
					response.ContentType = "application/soap+xml;charset=UTF-8";
					if (!WinRMHelper.DiagnosticsInfoHasBeenWritten(response.Headers))
					{
						string diagnosticsInfo = WinRMHelper.GetDiagnosticsInfo(httpContext);
						ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)this.GetHashCode(), "[RpsFriendlyErrorModule::OnEndRequest]: Original Status Code: {0}, Append diagnostics info: {1}.", statusCode, diagnosticsInfo);
						if (statusCode == 401)
						{
							response.Output.Write(diagnosticsInfo + HttpProxyStrings.ErrorAccessDenied);
						}
						else
						{
							response.Output.Write(diagnosticsInfo);
						}
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "FriendlyError", "HttpModule");
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.VerboseTracer.TraceError<Exception>((long)this.GetHashCode(), "[RpsFriendlyErrorModule::OnEndRequest]: Exception = {0}", ex);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(current, "RpsFriendlyErrorModule.OnEndRequest", ex.Message);
			}
			finally
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "OnEndRequest.End.ContentType", response.ContentType);
			}
		}

		private const string HeaderPreSentItemKey = "X-HeaderPreSent";
	}
}
