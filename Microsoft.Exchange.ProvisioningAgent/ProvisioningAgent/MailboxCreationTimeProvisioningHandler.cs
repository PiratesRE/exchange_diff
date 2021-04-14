using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class MailboxCreationTimeProvisioningHandler : ProvisioningHandlerBase
	{
		public override bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable)
		{
			if (base.UserSpecifiedParameters["Credential"] != null)
			{
				return false;
			}
			ADPresentationObject adpresentationObject = writeableIConfigurable as ADPresentationObject;
			ADUser aduser;
			if (adpresentationObject != null)
			{
				aduser = (adpresentationObject.DataObject as ADUser);
			}
			else
			{
				aduser = (writeableIConfigurable as ADUser);
			}
			return aduser != null && aduser.SetWhenMailboxCreatedIfNotSet();
		}

		private const string MailboxParameterSetArbitration = "Arbitration";

		private const string MailboxParameterSetDiscovery = "Discovery";
	}
}
