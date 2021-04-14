using System;
using System.Web;
using System.Web.Routing;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class RouteHanlder : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new HttpHandler();
		}
	}
}
