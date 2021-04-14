using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class QueryBaseDNRestrictionAgentClassFactory : IProvisioningAgent
	{
		static QueryBaseDNRestrictionAgentClassFactory()
		{
			QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForUpdate.CopyTo(QueryBaseDNRestrictionAgentClassFactory.allSupportedCmdlets, 0);
			QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForOnComplete.CopyTo(QueryBaseDNRestrictionAgentClassFactory.allSupportedCmdlets, QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForUpdate.Length);
		}

		IEnumerable<string> IProvisioningAgent.GetSupportedCmdlets()
		{
			return QueryBaseDNRestrictionAgentClassFactory.allSupportedCmdlets;
		}

		ProvisioningHandler IProvisioningAgent.GetCmdletHandler(string cmdletName)
		{
			foreach (string text in QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForOnComplete)
			{
				if (text.Equals(cmdletName, StringComparison.OrdinalIgnoreCase))
				{
					return new QueryBaseDNRestrictionNewObjectProvisioningHandler();
				}
			}
			return new QueryBaseDNRestrictionModifyObjectProvisioningHandler();
		}

		private static readonly string[] supportedCmdletsForOnComplete = new string[]
		{
			"new-mailbox",
			"New-SiteMailbox",
			"New-GroupMailbox",
			"new-syncmailbox",
			"undo-softdeletedmailbox",
			"undo-syncsoftdeletedmailbox"
		};

		private static readonly string[] supportedCmdletsForUpdate = new string[]
		{
			"set-mailbox",
			"Set-SiteMailbox",
			"set-syncmailbox",
			"enable-mailbox"
		};

		private static readonly string[] allSupportedCmdlets = new string[QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForOnComplete.Length + QueryBaseDNRestrictionAgentClassFactory.supportedCmdletsForUpdate.Length];
	}
}
