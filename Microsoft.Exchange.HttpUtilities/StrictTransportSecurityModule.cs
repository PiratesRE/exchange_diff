using System;
using System.Web;
using Microsoft.Exchange.HttpProxy;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpUtilities
{
	internal class StrictTransportSecurityModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.PreSendRequestHeaders += this.Application_PreSendRequestHeaders;
		}

		public void Dispose()
		{
		}

		private void Application_PreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (httpContext.Request.IsSecureConnection || httpContext.Request.Url.Scheme == Uri.UriSchemeHttps)
				{
					httpContext.Response.AddHeader(WellKnownHeader.StrictTransportSecurity, "max-age=31536000; includeSubDomains");
				}
			}, new Diagnostics.LastChanceExceptionHandler(this.LastChanceExceptionHandler));
		}

		private void LastChanceExceptionHandler(Exception unhandledException)
		{
			RequestLogger logger = RequestLogger.GetLogger(new HttpContextWrapper(HttpContext.Current));
			if (logger != null)
			{
				logger.LastChanceExceptionHandler(unhandledException);
			}
		}
	}
}
