using System;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RbacAuthorizationManager : ServiceAuthorizationManager
	{
		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			operationContext.SetRbacPrincipal((RbacPrincipal)HttpContext.Current.User);
			return base.CheckAccessCore(operationContext);
		}
	}
}
