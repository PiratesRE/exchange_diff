using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class PopImapDisabledQueryProcessor : EcpCmdletQueryProcessor
	{
		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			RbacQuery rbacQuery = new RbacQuery("Get-CasMailbox");
			if (!rbacQuery.IsInRole(rbacConfiguration))
			{
				return new bool?(true);
			}
			return new bool?(!rbacConfiguration.ExecutingUserIsPopEnabled && !rbacConfiguration.ExecutingUserIsImapEnabled);
		}

		internal const string RoleName = "PopImapDisabled";
	}
}
