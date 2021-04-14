using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal interface IProvisioningEnforcement : IProvisioningRule
	{
		bool IsApplicable(IConfigurable readOnlyPresentationObject);

		ProvisioningValidationError[] Validate(ADProvisioningPolicy enforcementPolicy, IConfigurable readOnlyPresentationObject);
	}
}
