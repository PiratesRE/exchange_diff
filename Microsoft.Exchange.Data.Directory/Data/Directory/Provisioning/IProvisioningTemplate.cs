using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal interface IProvisioningTemplate : IProvisioningRule
	{
		void Provision(ADProvisioningPolicy templatePolicy, IConfigurable writablePresentationObject);
	}
}
