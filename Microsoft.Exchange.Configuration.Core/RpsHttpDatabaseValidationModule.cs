using System;
using System.Web;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Configuration.Core
{
	public class RpsHttpDatabaseValidationModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
			context.EndRequest += this.OnEndRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		internal void SendErrorResponse(HttpContextBase context, int httpStatusCode, int httpSubStatusCode, string httpStatusText, Action<HttpResponseBase> customResponseAction, bool closeConnection)
		{
			HttpResponseBase response = context.Response;
			response.Clear();
			response.StatusCode = httpStatusCode;
			response.SubStatusCode = httpSubStatusCode;
			response.StatusDescription = httpStatusText;
			if (customResponseAction != null)
			{
				customResponseAction(response);
			}
			if (closeConnection)
			{
				response.Close();
			}
			context.ApplicationInstance.CompleteRequest();
		}

		private void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			HttpContextBase context = new HttpContextWrapper(((HttpApplication)sender).Context);
			WinRMInfo winRMInfoFromHttpHeaders = WinRMInfo.GetWinRMInfoFromHttpHeaders(context.Request.Headers);
			if (winRMInfoFromHttpHeaders != null)
			{
				string sessionUniqueId = winRMInfoFromHttpHeaders.SessionUniqueId;
				int num = 0;
				if (!string.IsNullOrEmpty(sessionUniqueId) && RpsHttpDatabaseValidationModule.ActiveSessionCache.TryGetValue(sessionUniqueId, out num))
				{
					HttpLogger.SafeAppendGenericInfo("CachedSessionId", sessionUniqueId);
					return;
				}
			}
			using (new MonitoredScope("RpsHttpDatabaseValidationModule", "RpsHttpDatabaseValidationModule", HttpModuleHelper.HttpPerfMonitors))
			{
				HttpDatabaseValidationHelper.ValidateHttpDatabaseHeader(context, delegate
				{
				}, delegate(string routingError)
				{
					if (context.Request.Headers[WellKnownHeader.XCafeLastRetryHeaderKey] != null)
					{
						HttpLogger.SafeAppendGenericInfo("IgnoreRoutingError", "Cafe last retry");
						return;
					}
					HttpLogger.SafeAppendGenericError("ServerRoutingError", routingError, false);
					WinRMInfo.SetFailureCategoryInfo(context.Response.Headers, FailureCategory.DatabaseValidation, "ServerRoutingError");
					this.SendErrorResponse(context, 555, 0, routingError, delegate(HttpResponseBase response)
					{
						response.Headers[WellKnownHeader.BEServerRoutingError] = routingError;
					}, false);
				}, delegate
				{
					HttpLogger.SafeAppendGenericError("InvalidMailboxDatabaseGuid", "Cannot Parse MailboxDatabaseGuid From Header", false);
					WinRMInfo.SetFailureCategoryInfo(context.Response.Headers, FailureCategory.DatabaseValidation, "InvalidDatabaseGuid");
					this.SendErrorResponse(context, 400, 0, "Invalid database guid", null, false);
				});
			}
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			HttpContext context = ((HttpApplication)sender).Context;
			HttpResponse response = context.Response;
			WinRMInfo winRMInfoFromHttpHeaders = WinRMInfo.GetWinRMInfoFromHttpHeaders(context.Request.Headers);
			string text = (winRMInfoFromHttpHeaders != null) ? winRMInfoFromHttpHeaders.SessionUniqueId : null;
			if (response == null || winRMInfoFromHttpHeaders == null || text == null)
			{
				return;
			}
			if ("Remove-PSSession".Equals(winRMInfoFromHttpHeaders.Action, StringComparison.OrdinalIgnoreCase))
			{
				RpsHttpDatabaseValidationModule.ActiveSessionCache.Remove(text);
				return;
			}
			if (response.StatusCode == 200 && !RpsHttpDatabaseValidationModule.ActiveSessionCache.Contains(text))
			{
				RpsHttpDatabaseValidationModule.ActiveSessionCache.TryInsertSliding(text, 0, RpsHttpDatabaseValidationModule.SlidingTimeSpanForActiveSession);
			}
		}

		private const int HttpStatusCodeRoutingError = 555;

		private const string GroupNameForMonitor = "RpsHttpDatabaseValidationModule";

		private static readonly ExactTimeoutCache<string, int> ActiveSessionCache = new ExactTimeoutCache<string, int>(null, null, null, 50000, false);

		private static readonly TimeSpan SlidingTimeSpanForActiveSession = TimeSpan.FromMinutes(5.0);
	}
}
