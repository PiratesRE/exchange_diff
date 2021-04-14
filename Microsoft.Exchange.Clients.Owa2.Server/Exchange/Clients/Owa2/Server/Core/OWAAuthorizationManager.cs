using System;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OWAAuthorizationManager : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			HttpContext httpContext = HttpContext.Current;
			OwaServerLogger.LogWcfLatency(httpContext);
			return httpContext.Request.IsAuthenticated && base.CheckAccessCore(operationContext);
		}
	}
}
