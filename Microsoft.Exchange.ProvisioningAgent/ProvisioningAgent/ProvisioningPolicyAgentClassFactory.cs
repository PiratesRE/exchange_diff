using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class ProvisioningPolicyAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			return new ADPolicyProvisioningHandler();
		}

		private readonly string[] supportedCmdlets = new string[]
		{
			"enable-Mailbox",
			"enable-MailContact",
			"Enable-MailPublicFolder",
			"enable-MailUser",
			"enable-RemoteMailbox",
			"enable-DistributionGroup",
			"new-Mailbox",
			"New-SiteMailbox",
			"New-GroupMailbox",
			"new-MailContact",
			"new-MailUser",
			"new-RemoteMailbox",
			"new-SyncMailbox",
			"new-SyncMailContact",
			"New-SyncMailPublicFolder",
			"new-SyncMailUser",
			"new-DistributionGroup",
			"new-SyncDistributionGroup",
			"new-DynamicDistributionGroup",
			"undo-softdeletedmailbox",
			"undo-syncsoftdeletedmailbox",
			"undo-syncsoftdeletedmailuser"
		};
	}
}
