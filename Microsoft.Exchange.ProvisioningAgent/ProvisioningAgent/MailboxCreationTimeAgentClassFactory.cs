using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class MailboxCreationTimeAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			return new MailboxCreationTimeProvisioningHandler();
		}

		private readonly string[] supportedCmdlets = new string[]
		{
			"new-mailbox",
			"New-SiteMailbox",
			"New-GroupMailbox",
			"new-syncmailbox",
			"set-mailbox",
			"Set-SiteMailbox",
			"set-syncmailbox",
			"enable-mailbox",
			"update-movedmailbox",
			"undo-softdeletedmailbox",
			"undo-syncsoftdeletedmailbox"
		};
	}
}
