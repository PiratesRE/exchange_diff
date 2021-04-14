using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class MailboxPermissionsAgentClassFactory : IProvisioningAgent
	{
		public IEnumerable<string> GetSupportedCmdlets()
		{
			return this.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			return new MailboxPermissionsProvisioningHandler();
		}

		private readonly string[] supportedCmdlets = new string[]
		{
			"new-mailbox",
			"New-SiteMailbox",
			"enable-mailbox",
			"move-mailbox",
			"disable-mailbox",
			"update-movedmailbox",
			"undo-softdeletedmailbox",
			"New-GroupMailbox"
		};
	}
}
