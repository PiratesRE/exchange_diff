using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public sealed class PolicyBindingSetConfig : PolicyConfigBase
	{
		public Guid PolicyDefinitionConfigId
		{
			get
			{
				object obj = base[PolicyBindingSetConfigSchema.PolicyDefinitionConfigId];
				if (obj != null)
				{
					return (Guid)obj;
				}
				return default(Guid);
			}
			set
			{
				base[PolicyBindingSetConfigSchema.PolicyDefinitionConfigId] = value;
			}
		}

		public IEnumerable<PolicyBindingConfig> AppliedScopes
		{
			get
			{
				return (IEnumerable<PolicyBindingConfig>)base[PolicyBindingSetConfigSchema.AppliedScopes];
			}
			set
			{
				base[PolicyBindingSetConfigSchema.AppliedScopes] = value;
			}
		}

		public override void ResetChangeTracking()
		{
			if (this.AppliedScopes != null)
			{
				foreach (PolicyBindingConfig policyBindingConfig in this.AppliedScopes)
				{
					policyBindingConfig.ResetChangeTracking();
				}
			}
			base.ResetChangeTracking();
		}

		public override void MarkAsDeleted()
		{
			base.MarkAsDeleted();
			if (this.AppliedScopes != null)
			{
				foreach (PolicyBindingConfig policyBindingConfig in this.AppliedScopes)
				{
					policyBindingConfig.MarkAsDeleted();
				}
			}
		}
	}
}
