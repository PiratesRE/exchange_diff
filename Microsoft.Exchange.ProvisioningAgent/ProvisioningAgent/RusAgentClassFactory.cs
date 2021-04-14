using System;
using System.Collections.Generic;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[ProvisioningAgentClassFactory]
	internal class RusAgentClassFactory : IProvisioningAgent
	{
		static RusAgentClassFactory()
		{
			List<string> list = new List<string>();
			list.AddRange(AddressBookRUSProvisioningHandler.SupportedTasks);
			list.AddRange(EmailAddressPolicyRUSProvisioningHandler.SupportedTasks);
			list.AddRange(RusProvisioningHandlerForRemove.SupportedTasks);
			list.AddRange(DefaultRUSProvisioningHandler.SupportedTasks);
			RusAgentClassFactory.supportedCmdlets = list.ToArray();
		}

		public IEnumerable<string> GetSupportedCmdlets()
		{
			return RusAgentClassFactory.supportedCmdlets;
		}

		public ProvisioningHandler GetCmdletHandler(string cmdletName)
		{
			if (Array.Exists<string>(AddressBookRUSProvisioningHandler.SupportedTasks, (string value) => StringComparer.InvariantCultureIgnoreCase.Equals(value, cmdletName)))
			{
				return new AddressBookRUSProvisioningHandler();
			}
			if (Array.Exists<string>(EmailAddressPolicyRUSProvisioningHandler.SupportedTasks, (string value) => StringComparer.InvariantCultureIgnoreCase.Equals(value, cmdletName)))
			{
				return new EmailAddressPolicyRUSProvisioningHandler();
			}
			if (Array.Exists<string>(RusProvisioningHandlerForRemove.SupportedTasks, (string value) => value.Equals(cmdletName, StringComparison.InvariantCultureIgnoreCase)))
			{
				return new RusProvisioningHandlerForRemove();
			}
			return new DefaultRUSProvisioningHandler();
		}

		private static readonly string[] supportedCmdlets;
	}
}
