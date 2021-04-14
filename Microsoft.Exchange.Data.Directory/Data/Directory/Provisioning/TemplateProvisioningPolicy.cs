using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	public abstract class TemplateProvisioningPolicy : ADProvisioningPolicy
	{
		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass),
					new BitMaskAndFilter(ADProvisioningPolicySchema.PolicyType, 1UL)
				});
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			this[ADProvisioningPolicySchema.PolicyType] = ProvisioningPolicyType.Template;
			base.StampPersistableDefaultValues();
		}

		internal sealed override IEnumerable<IProvisioningEnforcement> ProvisioningEnforcementRules
		{
			get
			{
				return null;
			}
		}

		internal sealed override ProvisioningValidationError[] ProvisioningCustomValidate(IConfigurable roPresentationObject)
		{
			return base.ProvisioningCustomValidate(roPresentationObject);
		}

		internal static readonly ADObjectId RdnTemplateContainer = ADProvisioningPolicy.RdnContainer.GetChildId("Template");
	}
}
