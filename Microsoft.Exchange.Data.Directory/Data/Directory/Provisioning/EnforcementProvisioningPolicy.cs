using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	public abstract class EnforcementProvisioningPolicy : ADProvisioningPolicy
	{
		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass),
					new BitMaskAndFilter(ADProvisioningPolicySchema.PolicyType, 2UL)
				});
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return EnforcementProvisioningPolicy.schema;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			this[ADProvisioningPolicySchema.PolicyType] = ProvisioningPolicyType.Enforcement;
			base.StampPersistableDefaultValues();
		}

		internal sealed override IEnumerable<IProvisioningTemplate> ProvisioningTemplateRules
		{
			get
			{
				return null;
			}
		}

		internal sealed override void ProvisionCustomDefaultProperties(IConfigurable provisionedDefault)
		{
			base.ProvisionCustomDefaultProperties(provisionedDefault);
		}

		private static EnforcementProvisioningPolicySchema schema = ObjectSchema.GetInstance<EnforcementProvisioningPolicySchema>();

		internal static readonly ADObjectId RdnEnforcementContainer = ADProvisioningPolicy.RdnContainer.GetChildId("Enforcement");
	}
}
