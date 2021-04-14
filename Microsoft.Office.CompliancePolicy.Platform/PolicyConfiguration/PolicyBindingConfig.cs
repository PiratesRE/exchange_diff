using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public class PolicyBindingConfig : PolicyConfigBase
	{
		public virtual Guid PolicyDefinitionConfigId
		{
			get
			{
				object obj = base[PolicyBindingConfigSchema.PolicyDefinitionConfigId];
				if (obj != null)
				{
					return (Guid)obj;
				}
				return default(Guid);
			}
			set
			{
				base[PolicyBindingConfigSchema.PolicyDefinitionConfigId] = value;
			}
		}

		public virtual Guid? PolicyRuleConfigId
		{
			get
			{
				return (Guid?)base[PolicyBindingConfigSchema.PolicyRuleConfigId];
			}
			set
			{
				base[PolicyBindingConfigSchema.PolicyRuleConfigId] = value;
			}
		}

		public virtual Guid? PolicyAssociationConfigId
		{
			get
			{
				return (Guid?)base[PolicyBindingConfigSchema.PolicyAssociationConfigId];
			}
			set
			{
				base[PolicyBindingConfigSchema.PolicyAssociationConfigId] = value;
			}
		}

		public virtual BindingMetadata Scope
		{
			get
			{
				return (BindingMetadata)base[PolicyBindingConfigSchema.Scope];
			}
			set
			{
				base[PolicyBindingConfigSchema.Scope] = value;
			}
		}

		public virtual bool IsExempt
		{
			get
			{
				object obj = base[PolicyBindingConfigSchema.IsExempt];
				return obj != null && (bool)obj;
			}
			set
			{
				base[PolicyBindingConfigSchema.IsExempt] = value;
			}
		}

		public virtual DateTime? WhenAppliedUTC
		{
			get
			{
				return (DateTime?)base[PolicyBindingConfigSchema.WhenAppliedUTC];
			}
			set
			{
				base[PolicyBindingConfigSchema.WhenAppliedUTC] = value;
			}
		}

		public virtual Mode Mode
		{
			get
			{
				object obj = base[PolicyBindingConfigSchema.Mode];
				if (obj != null)
				{
					return (Mode)obj;
				}
				return Mode.Enforce;
			}
			set
			{
				base[PolicyBindingConfigSchema.Mode] = value;
			}
		}

		public virtual PolicyApplyStatus PolicyApplyStatus
		{
			get
			{
				object obj = base[PolicyBindingConfigSchema.PolicyApplyStatus];
				if (obj != null)
				{
					return (PolicyApplyStatus)obj;
				}
				return PolicyApplyStatus.Pending;
			}
			set
			{
				base[PolicyBindingConfigSchema.PolicyApplyStatus] = value;
			}
		}
	}
}
