using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ClosedCampusQueryProcessor : EcpCmdletQueryProcessor
	{
		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			RbacQuery rbacQuery = new RbacQuery("Get-SupervisionPolicy");
			if (!rbacQuery.IsInRole(rbacConfiguration))
			{
				return new bool?(false);
			}
			Supervision supervision = new Supervision();
			PowerShellResults<SupervisionStatus> @object = supervision.GetObject(null);
			if (@object.SucceededWithValue)
			{
				foreach (SupervisionStatus supervisionStatus in @object.Output)
				{
					if (supervisionStatus.ClosedCampusPolicyEnabled)
					{
						return new bool?(true);
					}
				}
				return new bool?(false);
			}
			base.LogCmdletError(@object, "ClosedCampus");
			return null;
		}

		internal const string RoleName = "ClosedCampus";
	}
}
