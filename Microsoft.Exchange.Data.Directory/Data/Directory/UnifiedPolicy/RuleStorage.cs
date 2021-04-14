using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RuleStorage : UnifiedPolicyStorageBase
	{
		public RuleStorage()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RuleStorage.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RuleStorage.schema;
			}
		}

		public Guid ParentPolicyId
		{
			get
			{
				return (Guid)this[RuleStorageSchema.ParentPolicyId];
			}
			set
			{
				this[RuleStorageSchema.ParentPolicyId] = value;
			}
		}

		public Mode Mode
		{
			get
			{
				return (Mode)this[RuleStorageSchema.EnforcementMode];
			}
			set
			{
				this[RuleStorageSchema.EnforcementMode] = value;
			}
		}

		public int Priority
		{
			get
			{
				return (int)this[RuleStorageSchema.Priority];
			}
			set
			{
				this[RuleStorageSchema.Priority] = value;
			}
		}

		public string RuleBlob
		{
			get
			{
				return (string)this[RuleStorageSchema.RuleBlob];
			}
			set
			{
				this[RuleStorageSchema.RuleBlob] = value;
			}
		}

		public string Comments
		{
			get
			{
				return (string)this[RuleStorageSchema.Comments];
			}
			set
			{
				this[RuleStorageSchema.Comments] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[RuleStorageSchema.Description];
			}
			set
			{
				this[RuleStorageSchema.Description] = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return (bool)this[RuleStorageSchema.IsEnabled];
			}
			set
			{
				this[RuleStorageSchema.IsEnabled] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[RuleStorageSchema.CreatedBy];
			}
			set
			{
				this[RuleStorageSchema.CreatedBy] = value;
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return (string)this[RuleStorageSchema.LastModifiedBy];
			}
			set
			{
				this[RuleStorageSchema.LastModifiedBy] = value;
			}
		}

		public PolicyScenario Scenario
		{
			get
			{
				return (PolicyScenario)this[RuleStorageSchema.Scenario];
			}
			set
			{
				this[RuleStorageSchema.Scenario] = value;
			}
		}

		private static readonly RuleStorageSchema schema = ObjectSchema.GetInstance<RuleStorageSchema>();

		private static string mostDerivedClass = "msExchUnifiedRule";
	}
}
