using System;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Clients;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientAccessRulesModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
		}

		private void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication applicationInstance = HttpContext.Current.ApplicationInstance;
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			if (request.IsAuthenticated && RbacPrincipal.GetCurrent(false) != null && ClientAccessRulesModule.ShouldBlockConnection(httpContext, RbacPrincipal.Current.RbacConfiguration))
			{
				if (httpContext.IsWebServiceRequest() || httpContext.IsUploadRequest())
				{
					ClientAccessRulesModule.SendAjaxErrorToClient(httpContext);
				}
				else
				{
					httpContext.Response.Redirect(string.Format("{0}logoff.owa?reason=6", EcpUrl.OwaVDir));
				}
				applicationInstance.CompleteRequest();
			}
		}

		public void Dispose()
		{
		}

		private static void SendAjaxErrorToClient(HttpContext httpContext)
		{
			httpContext.ClearError();
			httpContext.Response.Clear();
			httpContext.Response.TrySkipIisCustomErrors = true;
			httpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
			httpContext.Response.ContentType = (httpContext.IsWebServiceRequest() ? "application/json; charset=utf-8" : "text/html");
			httpContext.Response.Headers["jsonerror"] = "true";
			httpContext.Response.AddHeader("X-ECP-ERROR", "ClientAccessRulesBlock");
			httpContext.Response.Write(AntiXssEncoder.HtmlEncode(Strings.ClientAccessRulesEACBlockMessage, false));
			httpContext.Response.StatusCode = (httpContext.IsWebServiceRequest() ? 500 : 200);
			httpContext.Response.End();
		}

		private static bool ShouldBlockConnection(HttpContext httpContext, ExchangeRunspaceConfiguration exchangeRunspaceConfiguration)
		{
			if (exchangeRunspaceConfiguration == null || exchangeRunspaceConfiguration.ExecutingUser == null || !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Eac.EACClientAccessRulesEnabled.Enabled)
			{
				return false;
			}
			double ruleLatency = 0.0;
			string ruleName = string.Empty;
			string usernameFromADRawEntry = ClientAccessRulesUtils.GetUsernameFromADRawEntry(exchangeRunspaceConfiguration.ExecutingUser);
			bool flag = ClientAccessRulesUtils.ShouldBlockConnection(exchangeRunspaceConfiguration.OrganizationId, usernameFromADRawEntry, ClientAccessProtocol.ExchangeAdminCenter, ClientAccessRulesUtils.GetRemoteEndPointFromContext(httpContext), httpContext.Request.IsAuthenticatedByAdfs() ? ClientAccessAuthenticationMethod.AdfsAuthentication : ClientAccessAuthenticationMethod.BasicAuthentication, exchangeRunspaceConfiguration.ExecutingUser, delegate(ClientAccessRulesEvaluationContext context)
			{
				ruleName = context.CurrentRule.Name;
			}, delegate(double latency)
			{
				ruleLatency = latency;
			});
			if (flag || ruleLatency > 50.0)
			{
				ActivityContextLogger.Instance.LogEvent(new ClientAccessRulesLogEvent(exchangeRunspaceConfiguration.OrganizationId, usernameFromADRawEntry, ClientAccessRulesUtils.GetRemoteEndPointFromContext(httpContext), httpContext.Request.IsAuthenticatedByAdfs() ? ClientAccessAuthenticationMethod.AdfsAuthentication : ClientAccessAuthenticationMethod.BasicAuthentication, ruleName, ruleLatency, flag));
			}
			return flag;
		}

		private const string EACBlockedByClientAccessRulesLogoff = "{0}logoff.owa?reason=6";

		private const string BlockedEcpErrorHeaderContent = "ClientAccessRulesBlock";
	}
}
