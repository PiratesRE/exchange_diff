using System;
using System.ServiceModel;
using System.Web;

namespace Microsoft.Exchange.Services.Wcf
{
	public class UMAuthorizationManager : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			HttpContext httpContext = HttpContext.Current;
			if (!httpContext.Request.IsAuthenticated)
			{
				EWSAuthorizationManager.Return401UnauthorizedResponse(operationContext, "Request was unauthenticated.");
				return false;
			}
			return base.CheckAccessCore(operationContext);
		}
	}
}
