using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class TrueQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return new bool?(true);
		}

		internal const string RoleName = "True";
	}
}
