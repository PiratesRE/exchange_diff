using System;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class BindingStorage : UnifiedPolicyStorageBase
	{
		public BindingStorage()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return BindingStorage.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return BindingStorage.schema;
			}
		}

		public Guid PolicyId
		{
			get
			{
				return (Guid)this[BindingStorageSchema.PolicyId];
			}
			set
			{
				this[BindingStorageSchema.PolicyId] = value;
			}
		}

		public MultiValuedProperty<string> Scopes
		{
			get
			{
				return (MultiValuedProperty<string>)this[BindingStorageSchema.Scopes];
			}
			set
			{
				this[BindingStorageSchema.Scopes] = value;
			}
		}

		internal MultiValuedProperty<string> DeletedScopes
		{
			get
			{
				return (MultiValuedProperty<string>)this[BindingStorageSchema.DeletedScopes];
			}
			set
			{
				this[BindingStorageSchema.DeletedScopes] = value;
			}
		}

		public MultiValuedProperty<ScopeStorage> AppliedScopes
		{
			get
			{
				return (MultiValuedProperty<ScopeStorage>)this[BindingStorageSchema.AppliedScopes];
			}
			set
			{
				this[BindingStorageSchema.AppliedScopes] = value;
			}
		}

		internal MultiValuedProperty<ScopeStorage> RemovedScopes
		{
			get
			{
				return (MultiValuedProperty<ScopeStorage>)this[BindingStorageSchema.RemovedScopes];
			}
			set
			{
				this[BindingStorageSchema.RemovedScopes] = value;
			}
		}

		internal object RawObject { get; set; }

		private static readonly BindingStorageSchema schema = ObjectSchema.GetInstance<BindingStorageSchema>();

		private static string mostDerivedClass = "msExchUnifiedBinding";
	}
}
