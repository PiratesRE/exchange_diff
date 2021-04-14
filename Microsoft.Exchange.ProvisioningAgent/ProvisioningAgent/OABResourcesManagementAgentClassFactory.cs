using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class OABResourcesManagementAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			if (cmdletName.ToLower() == this.supportedCmdlets[0])
			{
				return new MoveOfflineAddressbookProvisioningHandler();
			}
			return new NewOfflineAddressbookProvisioningHandler();
		}

		private readonly string[] supportedCmdlets = new string[]
		{
			"move-offlineaddressbook",
			"new-offlineaddressbook"
		};
	}
}
