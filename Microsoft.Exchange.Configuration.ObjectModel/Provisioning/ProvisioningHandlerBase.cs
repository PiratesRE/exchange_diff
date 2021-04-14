using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Provisioning
{
	public abstract class ProvisioningHandlerBase : ProvisioningHandler
	{
		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			return null;
		}

		public override bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable)
		{
			return false;
		}

		public override bool PreInternalProcessRecord(IConfigurable writeableIConfigurable)
		{
			return false;
		}

		public override ProvisioningValidationError[] Validate(IConfigurable readOnlyIConfigurable)
		{
			return null;
		}

		public override ProvisioningValidationError[] ValidateUserScope()
		{
			return null;
		}

		public override void OnComplete(bool succeeded, Exception e)
		{
		}
	}
}
