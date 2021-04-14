using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public class PolicyDefinitionConfig : PolicyConfigBase
	{
		public virtual string Description
		{
			get
			{
				return (string)base[PolicyDefinitionConfigSchema.Description];
			}
			set
			{
				base[PolicyDefinitionConfigSchema.Description] = value;
			}
		}

		public virtual string Comment
		{
			get
			{
				return (string)base[PolicyDefinitionConfigSchema.Comment];
			}
			set
			{
				base[PolicyDefinitionConfigSchema.Comment] = value;
			}
		}

		public virtual Guid? DefaultPolicyRuleConfigId
		{
			get
			{
				return (Guid?)base[PolicyDefinitionConfigSchema.DefaultPolicyRuleConfigId];
			}
			set
			{
				base[PolicyDefinitionConfigSchema.DefaultPolicyRuleConfigId] = value;
			}
		}

		public virtual Mode Mode
		{
			get
			{
				object obj = base[PolicyDefinitionConfigSchema.Mode];
				if (obj != null)
				{
					return (Mode)obj;
				}
				return Mode.Enforce;
			}
			set
			{
				base[PolicyDefinitionConfigSchema.Mode] = value;
			}
		}

		public virtual PolicyScenario Scenario
		{
			get
			{
				object obj = base[PolicyDefinitionConfigSchema.Scenario];
				if (obj != null)
				{
					return (PolicyScenario)obj;
				}
				return PolicyScenario.Retention;
			}
			set
			{
				base[PolicyDefinitionConfigSchema.Scenario] = value;
			}
		}

		public virtual bool Enabled
		{
			get
			{
				object obj = base[PolicyDefinitionConfigSchema.Enabled];
				return obj != null && (bool)obj;
			}
			set
			{
				base[PolicyDefinitionConfigSchema.Enabled] = value;
			}
		}

		public virtual string CreatedBy
		{
			get
			{
				return (string)base[PolicyDefinitionConfigSchema.CreatedBy];
			}
			set
			{
				base[PolicyDefinitionConfigSchema.CreatedBy] = value;
			}
		}

		public virtual string LastModifiedBy
		{
			get
			{
				return (string)base[PolicyDefinitionConfigSchema.LastModifiedBy];
			}
			set
			{
				base[PolicyDefinitionConfigSchema.LastModifiedBy] = value;
			}
		}
	}
}
