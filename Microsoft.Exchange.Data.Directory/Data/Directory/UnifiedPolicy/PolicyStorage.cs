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
	public class PolicyStorage : UnifiedPolicyStorageBase
	{
		public PolicyStorage()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PolicyStorage.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return PolicyStorage.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return PolicyStorage.PoliciesContainer;
			}
		}

		public Mode Mode
		{
			get
			{
				return (Mode)this[PolicyStorageSchema.EnforcementMode];
			}
			set
			{
				this[PolicyStorageSchema.EnforcementMode] = value;
			}
		}

		public PolicyScenario Scenario
		{
			get
			{
				return (PolicyScenario)this[PolicyStorageSchema.Scenario];
			}
			set
			{
				this[PolicyStorageSchema.Scenario] = value;
			}
		}

		public Guid? DefaultRuleId
		{
			get
			{
				return (Guid?)this[PolicyStorageSchema.DefaultRuleId];
			}
			set
			{
				this[PolicyStorageSchema.DefaultRuleId] = value;
			}
		}

		public string Comments
		{
			get
			{
				return (string)this[PolicyStorageSchema.Comments];
			}
			set
			{
				this[PolicyStorageSchema.Comments] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[PolicyStorageSchema.Description];
			}
			set
			{
				this[PolicyStorageSchema.Description] = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return (bool)this[PolicyStorageSchema.IsEnabled];
			}
			set
			{
				this[PolicyStorageSchema.IsEnabled] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[PolicyStorageSchema.CreatedBy];
			}
			set
			{
				this[PolicyStorageSchema.CreatedBy] = value;
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return (string)this[PolicyStorageSchema.LastModifiedBy];
			}
			set
			{
				this[PolicyStorageSchema.LastModifiedBy] = value;
			}
		}

		private static readonly PolicyStorageSchema schema = ObjectSchema.GetInstance<PolicyStorageSchema>();

		private static readonly string mostDerivedClass = "msExchUnifiedPolicy";

		internal static readonly ADObjectId PoliciesContainer = new ADObjectId("CN=Unified Policies");
	}
}
