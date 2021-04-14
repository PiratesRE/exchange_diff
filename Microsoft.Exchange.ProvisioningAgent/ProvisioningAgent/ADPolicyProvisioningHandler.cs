using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class ADPolicyProvisioningHandler : ProvisioningHandlerBase
	{
		internal ProvisioningSession Session
		{
			get
			{
				if (this.provisioningSession == null)
				{
					this.provisioningSession = new ProvisioningSession(this);
				}
				return this.provisioningSession;
			}
		}

		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			return this.Session.ProvisionDefaultProperties();
		}

		public override ProvisioningValidationError[] Validate(IConfigurable readOnlyADObject)
		{
			if (readOnlyADObject == null)
			{
				return null;
			}
			return this.Session.Validate(readOnlyADObject);
		}

		private ProvisioningSession provisioningSession;
	}
}
