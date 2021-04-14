using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class IsDehydratedQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return new bool?(LocalSession.Current.IsDehydrated);
		}

		internal const string IsDehydratedRoleName = "IsDehydrated";

		internal const string IsHydratedRoleName = "IsHydrated";
	}
}
