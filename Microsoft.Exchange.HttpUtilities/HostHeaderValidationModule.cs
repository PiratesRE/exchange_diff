using System;
using System.Web;

namespace Microsoft.Exchange.HttpUtilities
{
	public class HostHeaderValidationModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.OnBeginRequest;
		}

		public void Dispose()
		{
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			try
			{
				Uri url = context.Request.Url;
			}
			catch (UriFormatException)
			{
				HttpResponse response = context.Response;
				response.Clear();
				response.StatusCode = 400;
				response.StatusDescription = "Bad Request - Invalid Host Header.";
				response.AppendToLog("InvalidHostHeader");
				httpApplication.CompleteRequest();
			}
		}

		private const string HttpStatusDescription = "Bad Request - Invalid Host Header.";

		private const string LogDescription = "InvalidHostHeader";
	}
}
