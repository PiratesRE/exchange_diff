using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class ScopeStorageSchema : UnifiedPolicyStorageBaseSchema
	{
		public static ADPropertyDefinition Scope = new ADPropertyDefinition("AppliedScope", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchScope", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.NonADProperty, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition EnforcementMode = new ADPropertyDefinition("Mode", ExchangeObjectVersion.Exchange2012, typeof(Mode), "msExchIMAP4Settings", ADPropertyDefinitionFlags.NonADProperty, Mode.Enforce, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
