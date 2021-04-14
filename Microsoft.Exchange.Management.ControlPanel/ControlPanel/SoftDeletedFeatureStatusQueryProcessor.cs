using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class SoftDeletedFeatureStatusQueryProcessor : EcpCmdletQueryProcessor
	{
		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return new bool?(LocalSession.Current.IsSoftDeletedFeatureEnabled);
		}

		internal const string RoleName = "SoftDeletedFeatureEnabled";
	}
}
