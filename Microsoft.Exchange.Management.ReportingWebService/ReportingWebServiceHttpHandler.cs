using System;
using System.Security;
using System.Web;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class ReportingWebServiceHttpHandler : IHttpHandler
	{
		public ReportingWebServiceHttpHandler()
		{
			this.actualHandler = (IHttpHandler)Activator.CreateInstance("System.ServiceModel.Activation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.ServiceModel.Activation.HttpHandler").Unwrap();
		}

		public bool IsReusable
		{
			get
			{
				return this.actualHandler.IsReusable;
			}
		}

		[SecurityCritical]
		public void ProcessRequest(HttpContext context)
		{
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.HttpHandlerProcessRequestLatency, delegate
			{
				if (request.IsAuthenticated)
				{
					this.actualHandler.ProcessRequest(context);
					return;
				}
				response.StatusCode = 401;
			});
		}

		private IHttpHandler actualHandler;
	}
}
