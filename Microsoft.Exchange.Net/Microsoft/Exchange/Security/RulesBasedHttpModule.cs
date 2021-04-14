using System;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	public class RulesBasedHttpModule : IHttpModule
	{
		internal static ExEventLog EventLogger
		{
			get
			{
				return RulesBasedHttpModule.eventLogger;
			}
		}

		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (!RulesBasedHttpModuleConfiguration.Instance.TryLoad())
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError((long)this.GetHashCode(), "[RulesBasedHttpModule.Init] Failed to load DenyRules");
			}
			context.BeginRequest += this.OnBeginRequest;
			context.AuthenticateRequest += this.OnAuthenticate;
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			HttpModuleAuthenticationDenyRulesCollection authenticationDenyRules = RulesBasedHttpModuleConfiguration.Instance.AuthenticationDenyRules;
			ExTraceGlobals.RulesBasedHttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[RulesBasedHttpModule.OnBeginRequest] Enter");
			if (authenticationDenyRules != null && authenticationDenyRules.EvaluatePreAuthRules(context))
			{
				context.Response.StatusCode = 403;
				context.Response.StatusDescription = "Forbidden";
				context.ApplicationInstance.CompleteRequest();
			}
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnAuthenticate(object source, EventArgs args)
		{
			ExTraceGlobals.RulesBasedHttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[RulesBasedHttpModule.OnAuthenticate] Enter");
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			if (!request.IsAuthenticated)
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[RulesBasedHttpModule.OnAuthenticate] Request is not authenticated. Skip.");
				return;
			}
			HttpModuleAuthenticationDenyRulesCollection authenticationDenyRules = RulesBasedHttpModuleConfiguration.Instance.AuthenticationDenyRules;
			if (authenticationDenyRules != null && authenticationDenyRules.EvaluatePostAuthRules(context))
			{
				context.Response.StatusCode = 403;
				context.Response.StatusDescription = "Forbidden";
				context.ApplicationInstance.CompleteRequest();
			}
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ConfigurationTracer.Category, "MSExchange Common");
	}
}
