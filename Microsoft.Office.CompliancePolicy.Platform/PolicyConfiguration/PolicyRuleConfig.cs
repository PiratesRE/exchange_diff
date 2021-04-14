using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public class PolicyRuleConfig : PolicyConfigBase
	{
		public virtual string RuleBlob
		{
			get
			{
				return (string)base[PolicyRuleConfigSchema.RuleBlob];
			}
			set
			{
				base[PolicyRuleConfigSchema.RuleBlob] = value;
			}
		}

		public virtual int Priority
		{
			get
			{
				object obj = base[PolicyRuleConfigSchema.Priority];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				base[PolicyRuleConfigSchema.Priority] = value;
			}
		}

		public virtual string Description
		{
			get
			{
				return (string)base[PolicyRuleConfigSchema.Description];
			}
			set
			{
				base[PolicyRuleConfigSchema.Description] = value;
			}
		}

		public virtual string Comment
		{
			get
			{
				return (string)base[PolicyRuleConfigSchema.Comment];
			}
			set
			{
				base[PolicyRuleConfigSchema.Comment] = value;
			}
		}

		public virtual Guid PolicyDefinitionConfigId
		{
			get
			{
				object obj = base[PolicyRuleConfigSchema.PolicyDefinitionConfigId];
				if (obj != null)
				{
					return (Guid)obj;
				}
				return default(Guid);
			}
			set
			{
				base[PolicyRuleConfigSchema.PolicyDefinitionConfigId] = value;
			}
		}

		public virtual Mode Mode
		{
			get
			{
				object obj = base[PolicyRuleConfigSchema.Mode];
				if (obj != null)
				{
					return (Mode)obj;
				}
				return Mode.Enforce;
			}
			set
			{
				base[PolicyRuleConfigSchema.Mode] = value;
			}
		}

		public virtual PolicyScenario Scenario
		{
			get
			{
				object obj = base[PolicyRuleConfigSchema.Scenario];
				if (obj != null)
				{
					return (PolicyScenario)obj;
				}
				return PolicyScenario.Hold;
			}
			set
			{
				base[PolicyRuleConfigSchema.Scenario] = value;
			}
		}

		public virtual bool Enabled
		{
			get
			{
				object obj = base[PolicyRuleConfigSchema.Enabled];
				return obj != null && (bool)obj;
			}
			set
			{
				base[PolicyRuleConfigSchema.Enabled] = value;
			}
		}

		public virtual string CreatedBy
		{
			get
			{
				return (string)base[PolicyRuleConfigSchema.CreatedBy];
			}
			set
			{
				base[PolicyRuleConfigSchema.CreatedBy] = value;
			}
		}

		public virtual string LastModifiedBy
		{
			get
			{
				return (string)base[PolicyRuleConfigSchema.LastModifiedBy];
			}
			set
			{
				base[PolicyRuleConfigSchema.LastModifiedBy] = value;
			}
		}
	}
}
