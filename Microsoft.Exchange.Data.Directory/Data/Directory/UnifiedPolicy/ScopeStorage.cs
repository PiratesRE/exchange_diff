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
	public class ScopeStorage : UnifiedPolicyStorageBase
	{
		public ScopeStorage()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ScopeStorage.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ScopeStorage.schema;
			}
		}

		public string Scope
		{
			get
			{
				return (string)this[ScopeStorageSchema.Scope];
			}
			set
			{
				this[ScopeStorageSchema.Scope] = value;
			}
		}

		public Mode Mode
		{
			get
			{
				return (Mode)this[ScopeStorageSchema.EnforcementMode];
			}
			set
			{
				this[ScopeStorageSchema.EnforcementMode] = value;
			}
		}

		private static readonly ScopeStorageSchema schema = ObjectSchema.GetInstance<ScopeStorageSchema>();

		private static string mostDerivedClass = "msExchUnifiedBindingScope";
	}
}
