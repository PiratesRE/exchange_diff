using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class RequestContextHelper
	{
		public static bool IsSuiteServiceProxyRequestType(HttpApplication httpApplication)
		{
			RequestContext requestContext = RequestContext.Get(httpApplication.Context);
			return OwaRequestType.SuiteServiceProxyPage == requestContext.RequestType;
		}
	}
}
