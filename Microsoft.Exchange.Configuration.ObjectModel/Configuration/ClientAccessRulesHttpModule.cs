using System;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration
{
	public class ClientAccessRulesHttpModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("application");
			}
			context.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
		}

		public void Dispose()
		{
		}

		private void OnPostAuthenticateRequest(object source, EventArgs args)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] Entering");
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.RpsClientAccessRulesEnabled.Enabled && HttpRuntime.AppDomainAppVirtualPath.IndexOf("/PowerShell", StringComparison.OrdinalIgnoreCase) == 0)
			{
				IClientAccessRulesAuthorizer clientAccessRulesAuthorizer = new RemotePowershellClientAccessRulesAuthorizer();
				HttpContext httpContext = HttpContext.Current;
				if (httpContext.Request.IsAuthenticated)
				{
					ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] Request is already authenticated.");
					OrganizationId userOrganization = clientAccessRulesAuthorizer.GetUserOrganization();
					if (userOrganization == null)
					{
						ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] orgId = null.");
						return;
					}
					UserToken userToken = HttpContext.Current.CurrentUserToken();
					bool flag = ClientAccessRulesUtils.ShouldBlockConnection(userOrganization, ClientAccessRulesUtils.GetUsernameFromContext(httpContext), clientAccessRulesAuthorizer.Protocol, ClientAccessRulesUtils.GetRemoteEndPointFromContext(httpContext), ClientAccessRulesHttpModule.GetAuthenticationTypeFromContext(httpContext), userToken.Recipient, delegate(ClientAccessRulesEvaluationContext context)
					{
						clientAccessRulesAuthorizer.SafeAppendGenericInfo(httpContext, ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
					}, delegate(double latency)
					{
						if (latency > 50.0)
						{
							clientAccessRulesAuthorizer.SafeAppendGenericInfo(httpContext, ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
							ExTraceGlobals.HttpModuleTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] {0} = {1}.", ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
						}
					});
					if (flag)
					{
						ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] ClientAccessRules' evaluation for the organization blocks the connection");
						clientAccessRulesAuthorizer.ResponseToError(httpContext);
						httpContext.ApplicationInstance.CompleteRequest();
					}
				}
				ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] Exit");
				return;
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ClientAccessRulesHttpModule::OnPostAuthenticateRequest] Exit");
		}

		private static ClientAccessAuthenticationMethod GetAuthenticationTypeFromContext(HttpContext httpContext)
		{
			if (httpContext.Items["AuthType"] != null && "BASIC".Equals(httpContext.Items["AuthType"].ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return ClientAccessAuthenticationMethod.BasicAuthentication;
			}
			return ClientAccessAuthenticationMethod.NonBasicAuthentication;
		}

		private const string AuthenticationTypeItemName = "AuthType";

		private const string BasicAuthenticationType = "BASIC";

		private const string ModuleName = "ClientAccessRulesHttpModule";
	}
}
