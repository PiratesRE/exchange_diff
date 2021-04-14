using System;
using System.ServiceModel;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools
{
	internal static class OperationContextExtension
	{
		public static RbacPrincipal GetRbacPrincipal(this OperationContext operationContext)
		{
			return operationContext.Extensions.Find<RbacPrincipal>();
		}

		public static void SetRbacPrincipal(this OperationContext operationContext, RbacPrincipal principal)
		{
			RbacPrincipal rbacPrincipal = operationContext.GetRbacPrincipal();
			if (rbacPrincipal != null)
			{
				operationContext.Extensions.Remove(rbacPrincipal);
			}
			if (principal != null)
			{
				operationContext.Extensions.Add(principal);
			}
		}
	}
}
