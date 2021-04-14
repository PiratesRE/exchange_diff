using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class OwaServiceHttpHandlerBase
	{
		internal OwaServiceHttpHandlerBase(HttpContext httpContext, OWAService service, ServiceMethodInfo methodInfo)
		{
			this.HttpContext = httpContext;
			this.Service = service;
			this.ServiceMethodInfo = methodInfo;
			this.Inspector = new OwaServiceMessageInspector();
			this.MethodDispatcher = new OwaServiceMethodDispatcher(this.Inspector);
		}

		private protected OWAService Service { protected get; private set; }

		private protected IOwaServiceMessageInspector Inspector { protected get; private set; }

		private protected ServiceMethodInfo ServiceMethodInfo { protected get; private set; }

		private protected OwaServiceMethodDispatcher MethodDispatcher { protected get; private set; }

		private protected HttpContext HttpContext { protected get; private set; }

		protected void Initialize(HttpResponse response)
		{
			response.ContentType = "application/json; charset=utf-8";
			response.AddHeader("X-OWA-HttpHandler", "true");
			if (!this.ServiceMethodInfo.IsResponseCacheable)
			{
				response.Cache.SetNoServerCaching();
				response.Cache.SetCacheability(HttpCacheability.NoCache);
				response.Cache.SetNoStore();
			}
		}
	}
}
