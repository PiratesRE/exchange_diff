using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.ServiceModel;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools
{
	public class RbacAuthorizationPolicy : IAuthorizationPolicy, IAuthorizationComponent
	{
		public virtual bool Evaluate(EvaluationContext evaluationContext, ref object state)
		{
			evaluationContext.Properties["Principal"] = OperationContext.Current.GetRbacPrincipal().GetWrapperPrincipal();
			return true;
		}

		public ClaimSet Issuer
		{
			get
			{
				return ClaimSet.System;
			}
		}

		public string Id
		{
			get
			{
				return RbacAuthorizationPolicy.PolicyId;
			}
		}

		private static readonly string PolicyId = typeof(RbacAuthorizationPolicy).FullName;
	}
}
