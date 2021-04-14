using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public class PolicyAssociationConfig : PolicyConfigBase
	{
		public virtual string Description
		{
			get
			{
				return (string)base[PolicyAssociationConfigSchema.Description];
			}
			set
			{
				base[PolicyAssociationConfigSchema.Description] = value;
			}
		}

		public virtual string Comment
		{
			get
			{
				return (string)base[PolicyAssociationConfigSchema.Comment];
			}
			set
			{
				base[PolicyAssociationConfigSchema.Comment] = value;
			}
		}

		public virtual IEnumerable<Guid> PolicyDefinitionConfigIds
		{
			get
			{
				return (IEnumerable<Guid>)base[PolicyAssociationConfigSchema.PolicyDefinitionConfigIds];
			}
			set
			{
				base[PolicyAssociationConfigSchema.PolicyDefinitionConfigIds] = value;
			}
		}

		public virtual Guid? DefaultPolicyDefinitionConfigId
		{
			get
			{
				return (Guid?)base[PolicyAssociationConfigSchema.DefaultPolicyDefinitionConfigId];
			}
			set
			{
				base[PolicyAssociationConfigSchema.DefaultPolicyDefinitionConfigId] = value;
			}
		}

		public virtual bool AllowOverride
		{
			get
			{
				object obj = base[PolicyAssociationConfigSchema.AllowOverride];
				return obj != null && (bool)obj;
			}
			set
			{
				base[PolicyAssociationConfigSchema.AllowOverride] = value;
			}
		}

		public virtual string Scope
		{
			get
			{
				return (string)base[PolicyAssociationConfigSchema.Scope];
			}
			set
			{
				base[PolicyAssociationConfigSchema.Scope] = value;
			}
		}

		public virtual DateTime? WhenAppliedUTC
		{
			get
			{
				return (DateTime?)base[PolicyAssociationConfigSchema.WhenAppliedUTC];
			}
			set
			{
				base[PolicyAssociationConfigSchema.WhenAppliedUTC] = value;
			}
		}

		public virtual PolicyApplyStatus PolicyApplyStatus
		{
			get
			{
				object obj = base[PolicyAssociationConfigSchema.PolicyApplyStatus];
				if (obj != null)
				{
					return (PolicyApplyStatus)obj;
				}
				return PolicyApplyStatus.Pending;
			}
			set
			{
				base[PolicyAssociationConfigSchema.PolicyApplyStatus] = value;
			}
		}
	}
}
