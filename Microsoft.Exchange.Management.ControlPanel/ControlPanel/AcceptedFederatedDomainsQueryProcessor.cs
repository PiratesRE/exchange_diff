using System;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AcceptedFederatedDomainsQueryProcessor : EcpCmdletQueryProcessor
	{
		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			AcceptedDomains acceptedDomains = new AcceptedDomains();
			PowerShellResults<AcceptedDomain> list = acceptedDomains.GetList(null, null);
			if (list.Succeeded)
			{
				return new bool?((from x in list.Output
				where x.AuthenticationType == AuthenticationType.Federated
				select x).Count<AcceptedDomain>() > 0);
			}
			base.LogCmdletError(list, "OrgHasFederatedDomains");
			return new bool?(false);
		}

		internal const string RoleName = "OrgHasFederatedDomains";
	}
}
