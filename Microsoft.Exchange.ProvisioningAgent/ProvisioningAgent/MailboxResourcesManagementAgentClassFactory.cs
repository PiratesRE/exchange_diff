using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class MailboxResourcesManagementAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			return new MailboxProvisioningHandler();
		}

		private readonly string[] supportedCmdlets = new string[]
		{
			"new-mailbox",
			"New-SiteMailbox",
			"New-GroupMailbox",
			"new-syncmailbox",
			"enable-mailbox",
			"move-mailbox",
			"enable-mailuser",
			"undo-softdeletedmailbox",
			"undo-syncsoftdeletedmailbox"
		};
	}
}
