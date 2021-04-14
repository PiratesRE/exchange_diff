using System;
using System.Web.Routing;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal static class HandlerInstaller
	{
		public static void Initialize()
		{
			RouteTable.Routes.Add(new Route("Odata/{*pathInfo}", new RouteHanlder()));
		}
	}
}
