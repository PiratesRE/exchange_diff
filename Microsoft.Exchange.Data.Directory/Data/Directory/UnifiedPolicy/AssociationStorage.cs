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
	public class AssociationStorage : UnifiedPolicyStorageBase
	{
		public AssociationStorage()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AssociationStorage.mostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AssociationStorage.schema;
			}
		}

		public AssociationType AssociationType
		{
			get
			{
				return (AssociationType)this[AssociationStorageSchema.Type];
			}
			set
			{
				this[AssociationStorageSchema.Type] = value;
			}
		}

		public bool AllowOverride
		{
			get
			{
				return (bool)this[AssociationStorageSchema.AllowOverride];
			}
			set
			{
				this[AssociationStorageSchema.AllowOverride] = value;
			}
		}

		public Guid? DefaultPolicyId
		{
			get
			{
				return (Guid?)this[AssociationStorageSchema.DefaultPolicyId];
			}
			set
			{
				this[AssociationStorageSchema.DefaultPolicyId] = value;
			}
		}

		public string Scope
		{
			get
			{
				return (string)this[AssociationStorageSchema.Scope];
			}
			set
			{
				this[AssociationStorageSchema.Scope] = value;
			}
		}

		public MultiValuedProperty<Guid> PolicyIds
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[AssociationStorageSchema.PolicyIds];
			}
			set
			{
				this[AssociationStorageSchema.PolicyIds] = value;
			}
		}

		public string Comments
		{
			get
			{
				return (string)this[AssociationStorageSchema.Comments];
			}
			set
			{
				this[AssociationStorageSchema.Comments] = value;
			}
		}

		public string Description
		{
			get
			{
				return (string)this[AssociationStorageSchema.Description];
			}
			set
			{
				this[AssociationStorageSchema.Description] = value;
			}
		}

		private static readonly AssociationStorageSchema schema = ObjectSchema.GetInstance<AssociationStorageSchema>();

		private static string mostDerivedClass = "msExchUnifiedAssociation";
	}
}
