using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class PopImapDisabledQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			RbacQuery rbacQuery = new RbacQuery("Get-CASMailbox");
			if (!rbacQuery.IsInRole(rbacConfiguration))
			{
				return new bool?(true);
			}
			return new bool?(!rbacConfiguration.ExecutingUserIsPopEnabled && !rbacConfiguration.ExecutingUserIsImapEnabled);
		}

		internal const string RoleName = "PopImapDisabled";
	}
}
