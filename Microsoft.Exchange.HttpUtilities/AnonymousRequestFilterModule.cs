using System;
using System.Web;
using Microsoft.Exchange.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpUtilities
{
	public class AnonymousRequestFilterModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthenticateRequest((HttpApplication)sender);
			};
		}

		public void Dispose()
		{
		}

		private void OnPostAuthenticateRequest(HttpApplication httpApplication)
		{
			if (!HttpProxySettings.AnonymousRequestFilterEnabled)
			{
				return;
			}
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				this.OnPostAuthenticateInternal(httpApplication);
			}, delegate(Exception exception)
			{
				HttpContext.Current.Items[AnonymousRequestFilterModule.AnonymousRequestFilterModuleLoggingKey] = exception.ToString();
			});
		}

		private void OnPostAuthenticateInternal(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			if (!context.Request.IsAuthenticated)
			{
				bool flag = false;
				bool flag2 = false;
				string value = string.Empty;
				string httpMethod = context.Request.HttpMethod;
				string absolutePath = context.Request.Url.AbsolutePath;
				if (httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) || httpMethod.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
				{
					if (HttpProxyGlobals.ProtocolType != ProtocolType.Autodiscover || (!ProtocolHelper.IsOAuthMetadataRequest(absolutePath) && !ProtocolHelper.IsAutodiscoverV2Request(absolutePath)))
					{
						flag2 = true;
						flag = true;
						value = "AutodiscoverEwsDiscovery";
					}
				}
				else if (!ProtocolHelper.IsAnyWsSecurityRequest(context.Request.Url.AbsolutePath) && HttpProxyGlobals.ProtocolType != ProtocolType.Autodiscover)
				{
					flag2 = true;
					value = "AnonymousRequestDisallowed";
				}
				if (!string.IsNullOrEmpty(value))
				{
					context.Items[AnonymousRequestFilterModule.AnonymousRequestFilterModuleLoggingKey] = value;
				}
				if (flag)
				{
					AutodiscoverEwsDiscoveryResponseHelper.AddEndpointEnabledHeaders(context.Response);
				}
				if (flag2)
				{
					context.Response.StatusCode = 401;
					context.Response.StatusDescription = "Anonymous Request Disallowed";
					httpApplication.CompleteRequest();
				}
			}
		}

		private static readonly string AnonymousRequestFilterModuleLoggingKey = "AnonymousRequestFilterModule";
	}
}
